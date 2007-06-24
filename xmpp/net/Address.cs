using System.Net;
using System.Net.NetworkInformation;
using Bdev.Net.Dns;

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
        	NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface ni in interfaces)
			{
				if (ni.OperationalStatus == OperationalStatus.Up)
				{
					IPInterfaceProperties prop = ni.GetIPProperties();
					foreach (IPAddress dns in prop.DnsAddresses)
					{
						Request req = new Request();
						req.AddQuestion(new Question(hostname, DnsType.ANAME, DnsClass.IN));
						Response res = Resolver.Lookup(req, dns);
						if (res != null)
						{
							Address temp = new Address(hostname, port);
							temp.IP = ((ANameRecord)res.Answers[0].Record).IPAddress;
							return temp;
						}
					}
				}
			}

			return null;
		}
	}
}
