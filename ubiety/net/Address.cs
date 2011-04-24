// Address.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2010 Dieter Lunn, 2010 nickwhaley
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
using ubiety.net.dns;

namespace ubiety.net
{
	/// <summary>
	/// Resolves the IM server address from the hostname provided by the XID.
	/// </summary>
	internal class Address
	{
		private static Dictionary<string, Address> _cache = new Dictionary<string, Address>(10);
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
				return ResolveSystem(UbietySettings.Hostname);
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
            // Try and find IPv6 address
            var req = new Request();
            req.AddQuestion(new Question(UbietySettings.Hostname, DnsType.AAAA, DnsClass.IN));
            var res = Resolver.Lookup(req, DnsAddresses[_dnsAttempts]);
            if (res.Answers.Length == 0)
            {
                // No IPv6 finding IPv4 address
                req = new Request();
                req.AddQuestion(new Question(UbietySettings.Hostname, DnsType.ANAME, DnsClass.IN));
                res = Resolver.Lookup(req, DnsAddresses[_dnsAttempts]);
            }


            return null;
        }

		/// <summary>
		/// Is the address IPV6?
		/// </summary>
		public bool IPv6
		{
			get { return false; }
		}

		public string Hostname { get; private set; }

/*
		private static Address ResolveDNS(string hostname, int port)
		{
			if (hostname == "localhost" || hostname == "ubiety")
				return null;

			//var temp = new Address(hostname, port);

			try
			{
				string lookup;

				var srv = Resolver.SRVLookup("_xmpp-client._tcp." + hostname, DnsAddresses[0]);
				if (srv.Length > 0)
				{
					Logger.DebugFormat(typeof (Address), "SRV Address: {0}", srv[0].Target);
					lookup = srv[0].Target;
					//_port = srv[0].Port;
				}
				else
					lookup = hostname;

				var req = new Request();
				req.AddQuestion(new Question(lookup, DnsType.ANAME, DnsClass.IN));
				var resp = Resolver.Lookup(req, DnsAddresses[0]);
				if (resp.Answers.Length > 0)
				{
					foreach (var ans in resp.Answers)
					{
						if (ans.Type == DnsType.ANAME)
						{
							var rec = (ANameRecord) ans.Record;
							//temp.Ip = rec.IPAddress;
						}
						else
						{
							Logger.Debug(typeof (Address), "DNS Type: " + ans.Type);
						}
					}
				}
				else
				{
					Logger.Error(typeof (Address), "Error resolving DNS address.");
					return null;
				}

				//temp.EndPoint = new IPEndPoint(temp.Ip, _port);
			}
			catch (Exception e)
			{
				Logger.ErrorFormat(typeof (Address), "Error resolving DNS address: {0}", e);
				return null;
			}

			//return temp;
			return null;
		}
*/

		private static IPAddress ResolveSystem(string hostname)
		{
			try
			{
				return Dns.GetHostEntry(hostname).AddressList[0];
			}
			catch (Exception e)
			{
				Logger.ErrorFormat(typeof (Address), "Error resolving address: {0}", e);
				return null;
			}
		}
	}
}