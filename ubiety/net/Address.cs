// Address.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2011 Dieter Lunn, 2010 nickwhaley
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ubiety.logging;
using ubiety.net.dns;

namespace ubiety.net
{
	/// <summary>
	/// Resolves the IM server address from the hostname provided by the XID.
	/// </summary>
	internal class Address
	{
		private static readonly List<IPAddress> DnsAddresses = new List<IPAddress>();
		private bool _srvFailed;
		private SRVRecord[] _srvRecords;
		private int _dnsAttempts;
		private int _srvAttempts;

		public Address()
		{
			Logger.Debug(this, "Getting DNS addresses");
			var net = NetworkInterface.GetAllNetworkInterfaces();
			foreach (var dns in from n in net
									  where
										n.OperationalStatus == OperationalStatus.Up &&
										n.NetworkInterfaceType != NetworkInterfaceType.Loopback
									  select n.GetIPProperties()
										  into i
										  from dns in i.DnsAddresses
										  where dns.AddressFamily == AddressFamily.InterNetwork
										  select dns)
			{
				DnsAddresses.Add(dns);
				Logger.DebugFormat(this, "Dns Address: {0}", dns.ToString());
			}
		}

		public IPAddress NextIPAddress()
		{
			Hostname = !String.IsNullOrEmpty(UbietySettings.Hostname) ? UbietySettings.Hostname : UbietySettings.Id.Server;

			if (Hostname == "localhost")
			{
				return IPAddress.Parse("127.0.0.1");
			}

			if(_srvRecords == null && !_srvFailed)
				_srvRecords = FindSRV();

			if (_srvRecords != null)
			{
				if (_srvAttempts < _srvRecords.Length)
				{
					UbietySettings.Port = _srvRecords[_srvAttempts].Port;
					UbietySettings.Hostname = _srvRecords[_srvAttempts].Target;
					_srvAttempts++;
				}
				return Resolve();
			}
			return null;
		}

		private SRVRecord[] FindSRV()
		{
			while (_dnsAttempts < DnsAddresses.Count)
			{
				Logger.DebugFormat(this, "Using DNS {0}", DnsAddresses[_dnsAttempts].ToString());
				try
				{
					var records = Resolver.SRVLookup("_xmpp-client._tcp." + Hostname, DnsAddresses[_dnsAttempts]);
					if (records.Length == 0)
					{
						_srvFailed = true;
						return null;
					}

					return records;
				}
				catch (NoResponseException)
				{
					_dnsAttempts++;
				}
			}

			throw new NoResponseException();
		}

		private IPAddress Resolve()
		{
			while (true)
			{
				var req = new Request();

				//req.AddQuestion(Socket.OSSupportsIPv6
				//                    ? new Question(UbietySettings.Hostname, DnsType.AAAA, DnsClass.IN)
				//                    : new Question(UbietySettings.Hostname, DnsType.ANAME, DnsClass.IN));

				req.AddQuestion(new Question(UbietySettings.Hostname, DnsType.ANAME, DnsClass.IN));

				var res = Resolver.Lookup(req, DnsAddresses[_dnsAttempts]);

				if (res.Answers.Length <= 0) continue;

				if (res.Answers[0].Type == DnsType.AAAA)
				{
					IPv6 = true;
					var aa = (AAAARecord)res.Answers[0].Record;
					return aa.IPAddress;
				}

				IPv6 = false;
				var a = (ANameRecord)res.Answers[0].Record;
				return a.IPAddress;
			}
		}

		/// <summary>
		/// Is the address IPV6?
		/// </summary>
// ReSharper disable InconsistentNaming
		public bool IPv6 { get; private set; }
// ReSharper restore InconsistentNaming

		public string Hostname { get; private set; }
	}
}