// Address.cs
//
//XMPP .NET Library Copyright (C) 2006, 2008 Dieter Lunn
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
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using ubiety.logging;
using ubiety.net.dns;

namespace ubiety.net
{
    /// <remarks>
    /// Implements a method of resolving urls to an <see cref="IPEndPoint"/>.
    /// </remarks>
	public class Address
	{
		private static int _port;
		private IPAddress _ip;
		private string _hostname;
		private IPEndPoint _end;

		private static Dictionary<string, Address> _cache = new Dictionary<string, Address>();
		private static IPAddress[] _dns = new IPAddress[4];

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="port">Port for the <see cref="IPEndPoint"/></param>
		private Address(int port)
		{
			_port = port;
		}

		private Address(string hostname, int port)
			: this(port)
		{
			Hostname = hostname;
		}

        /// <summary>
        /// Hostname to connect to.
        /// </summary>
		public string Hostname
		{
			get { return _hostname; }
			set { _hostname = value; }
		}

        /// <summary>
        /// IP Address of the host to connect to.
        /// </summary>
		public IPAddress IP
		{
			get { return _ip; }
			set { _ip = value; }
		}
		
		public bool IPV6
		{
			get { return (_end.AddressFamily == AddressFamily.InterNetworkV6); }
		}

        /// <summary>
        /// <see cref="IPEndPoint"/> resolved from the hostname or ip address.
        /// </summary>
		public IPEndPoint EndPoint
		{
			get { return _end; }
		}

        /// <summary>
        /// Resolves a hostname to its ip address.
        /// </summary>
        /// <param name="hostname">Hostname to resolve.</param>
        /// <param name="port">Port to connect to</param>
        /// <returns>An instance of the <see cref="Address"/> class.</returns>
		public static Address Resolve(string hostname, int port)
		{
			Address temp;

			if (_cache.TryGetValue(hostname, out temp))
			{
				Logger.Info(typeof(Address), "Using cached address for " + hostname);
				return temp;
			}
			
			temp = new Address(hostname, port);
			
			Logger.Debug(typeof(Address), "Getting DNS addresses");
			NetworkInterface[] net = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface n in net)
			{
				if (true)
				{
					IPInterfaceProperties i = n.GetIPProperties();
					i.DnsAddresses.CopyTo(_dns, 0);
					foreach(IPAddress dns in i.DnsAddresses)
					{
						Logger.DebugFormat(typeof(Address), "Dns Address: {0}", dns.ToString());
					}
				}
			}

            try
            {
                string lookup;
                
                SRVRecord[] srv = Resolver.SRVLookup("_xmpp-client._tcp." + hostname, _dns[0]);
                if (srv.Length > 0)
                {
                	Logger.DebugFormat(typeof(Address), "SRV Address: {0}", srv[0].Target);
                	lookup = srv[0].Target;
                	_port = srv[0].Port;
                }
                else
                	lookup = hostname;
                	
                Request req = new Request();
                req.AddQuestion(new Question(lookup, DnsType.ANAME, DnsClass.IN));
                Response resp = Resolver.Lookup(req, _dns[0]);
                if (resp.Answers.Length > 0 && resp != null)
                {
                	foreach (Answer ans in resp.Answers)
                	{
                		if (ans.Type == DnsType.ANAME)
                		{
                			ANameRecord rec = (ANameRecord)ans.Record;
							temp.IP = rec.IPAddress;
						}
						else
						{
							Logger.Debug(typeof(Address), "DNS Type: " + ans.Type.ToString());
						}
					}
				}
				else
				{
					Logger.Error(typeof(Address), "Error resolving address.  Ending connection");
					return null;
				}
				
				temp._end = new IPEndPoint(temp.IP, _port);
            }
            catch (Exception e)
            {
            	Logger.ErrorFormat(typeof(Address), "Error resolving address: {0}", e);
            }
            
            _cache.Add(hostname, temp);
			
			return temp;
		}
	}
}
