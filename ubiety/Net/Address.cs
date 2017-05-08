// Address.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2017 Dieter Lunn, 2010 nickwhaley
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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Heijden.DNS;
using Serilog;
using Ubiety.States;
using TransportType = Heijden.DNS.TransportType;

namespace Ubiety.Net
{
    /// <summary>
    ///     Resolves the IM server address from the hostname provided by the XID.
    /// </summary>
    internal class Address
    {
        private readonly Resolver _resolver;
        private int _srvAttempts;
        private bool _srvFailed;
        private List<RecordSRV> _srvRecords;

        public Address()
        {
            _resolver = new Resolver("8.8.8.8") {UseCache = true, TimeOut = 5, TransportType = TransportType.Tcp};
            Log.Debug("Default DNS Servers: {DnsServers}", _resolver.DnsServer);
            _resolver.OnVerbose += _resolver_OnVerbose;
        }

        /// <summary>
        ///     Is the address IPV6?
        /// </summary>
        public bool IsIPv6 { get; private set; }

        public string Hostname { get; private set; }

        private static void _resolver_OnVerbose(object sender, Resolver.VerboseEventArgs e)
        {
            Log.Debug("DNS Resolver Verbose Message: {Message}", e.Message);
        }

        public IPAddress NextIpAddress()
        {
            Hostname = !string.IsNullOrEmpty(ProtocolState.Settings.Hostname)
                ? ProtocolState.Settings.Hostname
                : ProtocolState.Settings.Id.Server;

            if (IPAddress.TryParse(Hostname, out IPAddress address))
            {
                return address;
            }

            if (Hostname == "dieter-pc")
            {
                return IPAddress.Parse("127.0.0.1");
            }

            if (_srvRecords == null && !_srvFailed)
                _srvRecords = FindSrv();

            if (_srvFailed || _srvRecords == null)
            {
                Log.Debug("No SRV records for {0}", Hostname);

                return Resolve(Hostname);
            }

            if (_srvAttempts >= _srvRecords?.Count) return null;
            if (_srvRecords == null) return null;
            ProtocolState.Settings.Port = _srvRecords[_srvAttempts].PORT;
            var ip = Resolve(_srvRecords[_srvAttempts]?.TARGET);
            if (ip == null)
                _srvAttempts++;
            else
                return ip;

            return null;
        }

        private List<RecordSRV> FindSrv()
        {
            Log.Debug("Trying SRV lookup...");

            var resp = _resolver.Query("_xmpp-client._tcp." + Hostname, QType.SRV, QClass.IN);

            if (resp.header.ANCOUNT > 0)
            {
                _srvFailed = false;
                return resp.Answers.Select(record => record.RECORD as RecordSRV).ToList();
            }

            _srvFailed = true;
            return null;
        }

        private IPAddress Resolve(string hostname)
        {
            Response resp = null;

            Log.Debug("Trying standard lookup for {0}...", hostname);

            if (Socket.OSSupportsIPv6 && ProtocolState.Settings.UseIPv6)
            {
                Log.Debug("Trying IPv6 resolution...");
                resp = _resolver.Query(hostname, QType.AAAA, QClass.IN);
            }

            if (resp?.Answers.Count > 0)
            {
                Log.Debug("Found IPv6 address");
                IsIPv6 = true;
                return ((RecordAAAA) resp.Answers[0].RECORD).Address;
            }

            Log.Debug("Trying IPv4 resolution...");
            resp = _resolver.Query(hostname, QType.A, QClass.IN);

            IsIPv6 = false;
            return resp.Answers.Select(answer => answer.RECORD).OfType<RecordA>().Select(a => a.Address).FirstOrDefault();
        }
    }
}