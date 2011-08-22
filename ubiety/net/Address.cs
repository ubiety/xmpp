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
using ubiety.logging;
using ubiety.net.dns;
using ubiety.net.dns.Records;
using TransportType = ubiety.net.dns.TransportType;

namespace ubiety.net
{
	/// <summary>
	/// Resolves the IM server address from the hostname provided by the XID.
	/// </summary>
	internal class Address
	{
		private bool _srvFailed;
		private List<RecordSRV> _srvRecords;
		private int _srvAttempts;
		private readonly Resolver _resolver;

		public Address()
		{
			_resolver = new Resolver {UseCache = true, TimeOut = 5, TransportType = TransportType.Tcp};
			_resolver.OnVerbose += _resolver_OnVerbose;
		}

		void _resolver_OnVerbose(object sender, Resolver.VerboseEventArgs e)
		{
			Logger.Debug(this, e.Message);
		}

		public IPAddress NextIPAddress()
		{
			Hostname = !String.IsNullOrEmpty(UbietySettings.Hostname) ? UbietySettings.Hostname : UbietySettings.Id.Server;
			if(_srvRecords == null && !_srvFailed)
				_srvRecords = FindSRV();

			if (!_srvFailed && _srvRecords != null)
			{
				if (_srvAttempts < _srvRecords.Count)
				{
					UbietySettings.Port = _srvRecords[_srvAttempts].Port;
					var ip = Resolve(_srvRecords[_srvAttempts].Target);
					if (ip == null)
						_srvAttempts++;
					else
						return ip;
				}
			}
			return null;
		}

		private List<RecordSRV> FindSRV()
		{
			var resp = _resolver.Query("_xmpp-client._tcp." + Hostname, QType.SRV, QClass.IN);

			if (resp.Header.ANCOUNT > 0)
			{
				_srvFailed = false;
				return resp.Answers.Select(record => record.Record as RecordSRV).ToList();
			}

			_srvFailed = true;
			return null;
		}

		private IPAddress Resolve(string hostname)
		{
			var resp = _resolver.Query(hostname, QType.A, QClass.IN);

			IPv6 = false;
			return ((RecordA) resp.Answers[0].Record).Address;

			//while (true)
			//{
			//    var req = new Request();

			//    //req.AddQuestion(Socket.OSSupportsIPv6
			//    //                    ? new Question(UbietySettings.Hostname, DnsType.AAAA, DnsClass.IN)
			//    //                    : new Question(UbietySettings.Hostname, DnsType.ANAME, DnsClass.IN));

			//    req.AddQuestion(new Question(UbietySettings.Hostname, DnsType.ANAME, DnsClass.IN));

			//    var res = Resolver.Lookup(req, DnsAddresses[_dnsAttempts]);

			//    if (res.Answers.Length <= 0) continue;

			//    if (res.Answers[0].Type == DnsType.AAAA)
			//    {
			//        IPv6 = true;
			//        var aa = (AAAARecord)res.Answers[0].Record;
			//        return aa.IPAddress;
			//    }

			//    IPv6 = false;
			//    var a = (ANameRecord)res.Answers[0].Record;
			//    return a.IPAddress;
			//}
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