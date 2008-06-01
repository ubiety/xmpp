//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.

//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System.Net;

namespace xmpp.net
{
    /// <remarks>
    /// Implements a method of resolving urls to an <see cref="IPEndPoint"/>.
    /// </remarks>
	public class Address
	{
		private int _port;
		private IPAddress _ip;
		private string _hostname;

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="port">Port for the <see cref="IPEndPoint"/></param>
		public Address(int port)
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

        /// <summary>
        /// <see cref="IPEndPoint"/> resolved from the hostname or ip address.
        /// </summary>
		public IPEndPoint EndPoint
		{
			get { return new IPEndPoint(_ip, _port); }
		}

        /// <summary>
        /// Resolves a hostname to its ip address.
        /// </summary>
        /// <param name="hostname">Hostname to resolve.</param>
        /// <param name="port">Port to connect to</param>
        /// <returns>An instance of the <see cref="Address"/> class.</returns>
		public static Address Resolve(string hostname, int port)
		{
			IPHostEntry hostInfo = Dns.GetHostEntry(hostname);
			Address temp = new Address(hostname, port);
			temp.IP = hostInfo.AddressList[0];
            temp.Hostname = hostname;
			
			return temp;
		}
	}
}
