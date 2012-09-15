using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;

/*
 * Network Working Group                                     P. Mockapetris
 * Request for Comments: 1035                                           ISI
 *                                                            November 1987
 *
 *           DOMAIN NAMES - IMPLEMENTATION AND SPECIFICATION
 *
 */

namespace ubiety.net.dns
{
	/// <summary>
	/// Resolver is the main class to do DNS query lookups
	/// </summary>
	public class Resolver
	{
		#region Delegates

		///<summary>
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		public delegate void VerboseEventHandler(object sender, VerboseEventArgs e);

		#endregion

		/// <summary>
		/// Default DNS port
		/// </summary>
		public const int DefaultPort = 53;

		/// <summary>
		/// Gets list of OPENDNS servers
		/// </summary>
		public static readonly IPEndPoint[] DefaultDnsServers =
			{
				new IPEndPoint(IPAddress.Parse("208.67.222.222"), DefaultPort),
				new IPEndPoint(IPAddress.Parse("208.67.220.220"), DefaultPort)
			};

		private readonly List<IPEndPoint> _mDnsServers;

		private readonly Dictionary<string, Response> _mResponseCache;

		private bool _mRecursion;
		private int _mRetries;
		private int _mTimeout;
		private TransportType _mTransportType;
		private ushort _mUnique;
		private bool _mUseCache;

		/// <summary>
		/// Constructor of Resolver using DNS servers specified.
		/// </summary>
		/// <param name="dnsServers">Set of DNS servers</param>
		public Resolver(IEnumerable<IPEndPoint> dnsServers)
		{
			_mResponseCache = new Dictionary<string, Response>();
			_mDnsServers = new List<IPEndPoint>();
			_mDnsServers.AddRange(dnsServers);

			_mUnique = (ushort) (new Random()).Next();
			_mRetries = 3;
			_mTimeout = 1;
			_mRecursion = true;
			_mUseCache = true;
			_mTransportType = TransportType.Udp;
		}

		/// <summary>
		/// Constructor of Resolver using DNS server specified.
		/// </summary>
		/// <param name="dnsServer">DNS server to use</param>
		public Resolver(IPEndPoint dnsServer)
			: this(new[] {dnsServer})
		{
		}

		/// <summary>
		/// Constructor of Resolver using DNS server and port specified.
		/// </summary>
		/// <param name="serverIpAddress">DNS server to use</param>
		/// <param name="serverPortNumber">DNS port to use</param>
		public Resolver(IPAddress serverIpAddress, int serverPortNumber)
			: this(new IPEndPoint(serverIpAddress, serverPortNumber))
		{
		}

		/// <summary>
		/// Constructor of Resolver using DNS address and port specified.
		/// </summary>
		/// <param name="serverIpAddress">DNS server address to use</param>
		/// <param name="serverPortNumber">DNS port to use</param>
		public Resolver(string serverIpAddress, int serverPortNumber)
			: this(IPAddress.Parse(serverIpAddress), serverPortNumber)
		{
		}

		/// <summary>
		/// Constructor of Resolver using DNS address.
		/// </summary>
		/// <param name="serverIpAddress">DNS server address to use</param>
		public Resolver(string serverIpAddress)
			: this(IPAddress.Parse(serverIpAddress), DefaultPort)
		{
		}

		/// <summary>
		/// Resolver constructor, using DNS servers specified by Windows
		/// </summary>
		public Resolver()
			: this(GetDnsServers())
		{
		}

		/// <summary>
		/// Version of this set of routines, when not in a library
		/// </summary>
		public string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}


		/// <summary>
		/// Gets or sets timeout in milliseconds
		/// </summary>
		public int TimeOut
		{
			get { return _mTimeout; }
			set { _mTimeout = value; }
		}

		/// <summary>
		/// Gets or sets number of retries before giving up
		/// </summary>
		public int Retries
		{
			get { return _mRetries; }
			set
			{
				if (value >= 1)
					_mRetries = value;
			}
		}

		/// <summary>
		/// Gets or set recursion for doing queries
		/// </summary>
		public bool Recursion
		{
			get { return _mRecursion; }
			set { _mRecursion = value; }
		}

		/// <summary>
		/// Gets or sets protocol to use
		/// </summary>
		public TransportType TransportType
		{
			get { return _mTransportType; }
			set { _mTransportType = value; }
		}

		/// <summary>
		/// Gets or sets list of DNS servers to use
		/// </summary>
		public IPEndPoint[] DnsServers
		{
			get { return _mDnsServers.ToArray(); }
			set
			{
				_mDnsServers.Clear();
				_mDnsServers.AddRange(value);
			}
		}

		/// <summary>
		/// Gets first DNS server address or sets single DNS server to use
		/// </summary>
		public string DnsServer
		{
			get { return _mDnsServers[0].Address.ToString(); }
			set
			{
				IPAddress ip;
				if (IPAddress.TryParse(value, out ip))
				{
					_mDnsServers.Clear();
					_mDnsServers.Add(new IPEndPoint(ip, DefaultPort));
					return;
				}
				var response = Query(value, QType.A);
				if (response.RecordsA.Length <= 0) return;
				_mDnsServers.Clear();
				_mDnsServers.Add(new IPEndPoint(response.RecordsA[0].Address, DefaultPort));
			}
		}


		///<summary>
		///</summary>
		public bool UseCache
		{
			get { return _mUseCache; }
			set
			{
				_mUseCache = value;
				if (!_mUseCache)
					_mResponseCache.Clear();
			}
		}

		private void Verbose(string format, params object[] args)
		{
			if (OnVerbose != null)
				OnVerbose(this, new VerboseEventArgs(string.Format(format, args)));
		}

		/// <summary>
		/// Verbose messages from internal operations
		/// </summary>
		public event VerboseEventHandler OnVerbose;

		/// <summary>
		/// Clear the resolver cache
		/// </summary>
		public void ClearCache()
		{
			_mResponseCache.Clear();
		}

		private Response SearchInCache(Question question)
		{
			if (!_mUseCache)
				return null;

			var strKey = question.QClass + "-" + question.QType + "-" + question.QName;

			Response response;

			lock (_mResponseCache)
			{
				if (!_mResponseCache.ContainsKey(strKey))
					return null;

				response = _mResponseCache[strKey];
			}

			var timeLived = (int) ((DateTime.Now.Ticks - response.TimeStamp.Ticks)/TimeSpan.TicksPerSecond);
			foreach (var rr in response.RecordsRR)
			{
				rr.TimeLived = timeLived;
				// The TTL property calculates its actual time to live
				if (rr.TTL == 0)
					return null; // out of date
			}
			return response;
		}

		private void AddToCache(Response response)
		{
			if (!_mUseCache)
				return;

			// No question, no caching
			if (response.Questions.Count == 0)
				return;

			// Only cached non-error responses
			if (response.Header.RCODE != RCode.NoError)
				return;

			var question = response.Questions[0];

			var strKey = question.QClass + "-" + question.QType + "-" + question.QName;

			lock (_mResponseCache)
			{
				if (_mResponseCache.ContainsKey(strKey))
					_mResponseCache.Remove(strKey);

				_mResponseCache.Add(strKey, response);
			}
		}

		private Response UdpRequest(Request request)
		{
			// RFC1035 max. size of a UDP datagram is 512 bytes
			var responseMessage = new byte[512];

			for (var intAttempts = 0; intAttempts < _mRetries; intAttempts++)
			{
				for (var intDnsServer = 0; intDnsServer < _mDnsServers.Count; intDnsServer++)
				{
					var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _mTimeout*1000);

					try
					{
						socket.SendTo(request.Data, _mDnsServers[intDnsServer]);
						var intReceived = socket.Receive(responseMessage);
						var data = new byte[intReceived];
						Array.Copy(responseMessage, data, intReceived);
						var response = new Response(_mDnsServers[intDnsServer], data);
						AddToCache(response);
						return response;
					}
					catch (SocketException)
					{
						Verbose(string.Format(";; Connection to nameserver {0} failed", (intDnsServer + 1)));
						continue; // next try
					}
					finally
					{
						_mUnique++;

						// close the socket
						socket.Close();
					}
				}
			}
			var responseTimeout = new Response {Error = "Timeout Error"};
			return responseTimeout;
		}

		private Response TcpRequest(Request request)
		{
			//System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			//sw.Start();

			for (var intAttempts = 0; intAttempts < _mRetries; intAttempts++)
			{
				for (var intDnsServer = 0; intDnsServer < _mDnsServers.Count; intDnsServer++)
				{
					//var tcpClient = new TcpClient(AddressFamily.InterNetworkV6) {ReceiveTimeout = _mTimeout*1000};
					var tcpClient = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
					//tcpClient.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

					try
					{
						Verbose(";; Connecting to nameserver {0}", _mDnsServers[intDnsServer].Address);
						var result = tcpClient.BeginConnect(_mDnsServers[intDnsServer].Address, _mDnsServers[intDnsServer].Port,
						                                             null, null);

						var success = result.AsyncWaitHandle.WaitOne(_mTimeout*1000, true);

						if (!success || !tcpClient.Connected)
						{
							tcpClient.Close();
							Verbose(string.Format(";; Connection to nameserver {0} failed", _mDnsServers[intDnsServer].Address));
							continue;
						}

						var bs = new BufferedStream(new NetworkStream(tcpClient));

						var data = request.Data;
						bs.WriteByte((byte) ((data.Length >> 8) & 0xff));
						bs.WriteByte((byte) (data.Length & 0xff));
						bs.Write(data, 0, data.Length);
						bs.Flush();

						var transferResponse = new Response();
						var intSoa = 0;
						var intMessageSize = 0;

						//Debug.WriteLine("Sending "+ (request.Length+2) + " bytes in "+ sw.ElapsedMilliseconds+" mS");

						while (true)
						{
							var intLength = bs.ReadByte() << 8 | bs.ReadByte();
							if (intLength <= 0)
							{
								tcpClient.Close();
								Verbose(string.Format(";; Connection to nameserver {0} failed", (intDnsServer + 1)));
								throw new SocketException(); // next try
							}

							intMessageSize += intLength;

							data = new byte[intLength];
							bs.Read(data, 0, intLength);
							var response = new Response(_mDnsServers[intDnsServer], data);

							//Debug.WriteLine("Received "+ (intLength+2)+" bytes in "+sw.ElapsedMilliseconds +" mS");

							if (response.Header.RCODE != RCode.NoError)
								return response;

							if (response.Questions[0].QType != QType.AXFR)
							{
								AddToCache(response);
								return response;
							}

							// Zone transfer!!

							if (transferResponse.Questions.Count == 0)
								transferResponse.Questions.AddRange(response.Questions);
							transferResponse.Answers.AddRange(response.Answers);
							transferResponse.Authorities.AddRange(response.Authorities);
							transferResponse.Additionals.AddRange(response.Additionals);

							if (response.Answers[0].Type == Type.SOA)
								intSoa++;

							if (intSoa != 2) continue;
							transferResponse.Header.QDCOUNT = (ushort) transferResponse.Questions.Count;
							transferResponse.Header.ANCOUNT = (ushort) transferResponse.Answers.Count;
							transferResponse.Header.NSCOUNT = (ushort) transferResponse.Authorities.Count;
							transferResponse.Header.ARCOUNT = (ushort) transferResponse.Additionals.Count;
							transferResponse.MessageSize = intMessageSize;
							return transferResponse;
						}
					} // try
					catch (SocketException)
					{
						continue; // next try
					}
					finally
					{
						_mUnique++;

						// close the socket
						tcpClient.Close();
					}
				}
			}
			var responseTimeout = new Response {Error = "Timeout Error"};
			return responseTimeout;
		}

		/// <summary>
		/// Do Query on specified DNS servers
		/// </summary>
		/// <param name="name">Name to query</param>
		/// <param name="qtype">Question type</param>
		/// <param name="qclass">Class type</param>
		/// <returns>Response of the query</returns>
		public Response Query(string name, QType qtype, QClass qclass)
		{
			var question = new Question(name, qtype, qclass);
			var response = SearchInCache(question);
			if (response != null)
				return response;

			var request = new Request();
			request.AddQuestion(question);
			return GetResponse(request);
		}

		/// <summary>
		/// Do an QClass=IN Query on specified DNS servers
		/// </summary>
		/// <param name="name">Name to query</param>
		/// <param name="qtype">Question type</param>
		/// <returns>Response of the query</returns>
		public Response Query(string name, QType qtype)
		{
			var question = new Question(name, qtype, QClass.IN);
			var response = SearchInCache(question);
			if (response != null)
				return response;

			var request = new Request();
			request.AddQuestion(question);
			return GetResponse(request);
		}

		private Response GetResponse(Request request)
		{
			request.Header.ID = _mUnique;
			request.Header.RD = _mRecursion;

			if (_mTransportType == TransportType.Udp)
				return UdpRequest(request);

			if (_mTransportType == TransportType.Tcp)
				return TcpRequest(request);

			var response = new Response {Error = "Unknown TransportType"};
			return response;
		}

		/// <summary>
		/// Gets a list of default DNS servers used on the Windows machine.
		/// </summary>
		/// <returns></returns>
		public static IPEndPoint[] GetDnsServers()
		{
			var list = new List<IPEndPoint>();

			var adapters = NetworkInterface.GetAllNetworkInterfaces();
			foreach (var entry in from n in adapters
			                             where n.OperationalStatus == OperationalStatus.Up
			                             select n.GetIPProperties()
			                             into ipProps from ipAddr in ipProps.DnsAddresses
			                             	//where ipAddr.AddressFamily == AddressFamily.InterNetwork
			                             select new IPEndPoint(ipAddr, DefaultPort)
			                             into entry where !list.Contains(entry) select entry)
			{
				list.Add(entry);
			}
			return list.ToArray();
		}


		//

		private IPHostEntry MakeEntry(string hostName)
		{
			var entry = new IPHostEntry {HostName = hostName};

			var response = Query(hostName, QType.A, QClass.IN);

			// fill AddressList and aliases
			var addressList = new List<IPAddress>();
			var aliases = new List<string>();
			foreach (var answerRr in response.Answers)
			{
				switch (answerRr.Type)
				{
					case Type.A:
						addressList.Add(IPAddress.Parse((answerRr.Record.ToString())));
						entry.HostName = answerRr.Name;
						break;
					case Type.CNAME:
						aliases.Add(answerRr.Name);
						break;
				}
			}
			entry.AddressList = addressList.ToArray();
			entry.Aliases = aliases.ToArray();

			return entry;
		}

		/// <summary>
		/// Translates the IPV4 or IPV6 address into an arpa address
		/// </summary>
		/// <param name="ip">IP address to get the arpa address form</param>
		/// <returns>The 'mirrored' IPV4 or IPV6 arpa address</returns>
		public static string GetArpaFromIp(IPAddress ip)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				var sb = new StringBuilder();
				sb.Append("in-addr.arpa.");
				foreach (var b in ip.GetAddressBytes())
				{
					sb.Insert(0, string.Format("{0}.", b));
				}
				return sb.ToString();
			}
			if (ip.AddressFamily == AddressFamily.InterNetworkV6)
			{
				var sb = new StringBuilder();
				sb.Append("ip6.arpa.");
				foreach (byte b in ip.GetAddressBytes())
				{
					sb.Insert(0, string.Format("{0:x}.", (b >> 4) & 0xf));
					sb.Insert(0, string.Format("{0:x}.", (b >> 0) & 0xf));
				}
				return sb.ToString();
			}
			return "?";
		}

		///<summary>
		///</summary>
		///<param name="strEnum"></param>
		///<returns></returns>
		public static string GetArpaFromEnum(string strEnum)
		{
			var sb = new StringBuilder();
			var number = Regex.Replace(strEnum, "[^0-9]", "");
			sb.Append("e164.arpa.");
			foreach (var c in number)
			{
				sb.Insert(0, string.Format("{0}.", c));
			}
			return sb.ToString();
		}

		/// <summary>
		///		Resolves an IP address to an System.Net.IPHostEntry instance.
		/// </summary>
		/// <param name="ip">An IP address.</param>
		/// <returns>
		///		An System.Net.IPHostEntry instance that contains address information about
		///		the host specified in address.
		///</returns>
		public IPHostEntry GetHostEntry(IPAddress ip)
		{
			var response = Query(GetArpaFromIp(ip), QType.PTR, QClass.IN);
			return response.RecordsPTR.Length > 0 ? MakeEntry(response.RecordsPTR[0].PTRDNAME) : new IPHostEntry();
		}

		/// <summary>
		///		Resolves a host name or IP address to an System.Net.IPHostEntry instance.
		/// </summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <returns>
		///		An System.Net.IPHostEntry instance that contains address information about
		///		the host specified in hostNameOrAddress. 
		///</returns>
		public IPHostEntry GetHostEntry(string hostNameOrAddress)
		{
			IPAddress iPAddress;
			return IPAddress.TryParse(hostNameOrAddress, out iPAddress) ? GetHostEntry(iPAddress) : MakeEntry(hostNameOrAddress);
		}

		/// <summary>
		/// Asynchronously resolves a host name or IP address to an System.Net.IPHostEntry instance.
		/// </summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <param name="requestCallback">
		///		An System.AsyncCallback delegate that references the method to invoke when
		///		the operation is complete.
		///</param>
		/// <param name="stateObject">
		///		A user-defined object that contains information about the operation. This
		///		object is passed to the requestCallback delegate when the operation is complete.
		/// </param>
		/// <returns>An System.IAsyncResult instance that references the asynchronous request.</returns>
		public IAsyncResult BeginGetHostEntry(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
		{
			GetHostEntryDelegate g = GetHostEntry;
			return g.BeginInvoke(hostNameOrAddress, requestCallback, stateObject);
		}

		/// <summary>
		/// Asynchronously resolves an IP address to an System.Net.IPHostEntry instance.
		/// </summary>
		/// <param name="ip">The IP address to resolve.</param>
		/// <param name="requestCallback">
		///		An System.AsyncCallback delegate that references the method to invoke when
		///		the operation is complete.
		/// </param>
		/// <param name="stateObject">
		///		A user-defined object that contains information about the operation. This
		///     object is passed to the requestCallback delegate when the operation is complete.
		/// </param>
		/// <returns>An System.IAsyncResult instance that references the asynchronous request.</returns>
		public IAsyncResult BeginGetHostEntry(IPAddress ip, AsyncCallback requestCallback, object stateObject)
		{
			GetHostEntryViaIPDelegate g = GetHostEntry;
			return g.BeginInvoke(ip, requestCallback, stateObject);
		}

		/// <summary>
		/// Ends an asynchronous request for DNS information.
		/// </summary>
		/// <param name="asyncResult">
		///		An System.IAsyncResult instance returned by a call to an 
		///		Overload:Heijden.Dns.Resolver.BeginGetHostEntry method.
		/// </param>
		/// <returns>
		///		An System.Net.IPHostEntry instance that contains address information about
		///		the host. 
		///</returns>
		public IPHostEntry EndGetHostEntry(IAsyncResult asyncResult)
		{
			var aResult = (AsyncResult) asyncResult;
			if (aResult.AsyncDelegate is GetHostEntryDelegate)
			{
				var g = (GetHostEntryDelegate) aResult.AsyncDelegate;
				return g.EndInvoke(asyncResult);
			}
			if (aResult.AsyncDelegate is GetHostEntryViaIPDelegate)
			{
				var g = (GetHostEntryViaIPDelegate) aResult.AsyncDelegate;
				return g.EndInvoke(asyncResult);
			}
			return null;
		}

		///<summary>
		///</summary>
		///<param name="strPath"></param>
		public void LoadRootFile(string strPath)
		{
			var sr = new StreamReader(strPath);
			while (!sr.EndOfStream)
			{
				var strLine = sr.ReadLine();
				if (strLine == null)
					break;
				var intI = strLine.IndexOf(';');
				if (intI >= 0)
					strLine = strLine.Substring(0, intI);
				strLine = strLine.Trim();
				if (strLine.Length == 0)
					continue;
				var status = RRRecordStatus.Name;
				var Name = "";
				var Ttl = "";
				var Class = "";
				var Type = "";
				var Value = "";
				var strW = "";
				for (intI = 0; intI < strLine.Length; intI++)
				{
					var c = strLine[intI];

					if (c <= ' ' && strW != "")
					{
						switch (status)
						{
							case RRRecordStatus.Name:
								Name = strW;
								status = RRRecordStatus.TTL;
								break;
							case RRRecordStatus.TTL:
								Ttl = strW;
								status = RRRecordStatus.Class;
								break;
							case RRRecordStatus.Class:
								Class = strW;
								status = RRRecordStatus.Type;
								break;
							case RRRecordStatus.Type:
								Type = strW;
								status = RRRecordStatus.Value;
								break;
							case RRRecordStatus.Value:
								Value = strW;
								status = RRRecordStatus.Unknown;
								break;
						}
						strW = "";
					}
					if (c > ' ')
						strW += c;
				}
			}
			sr.Close();
		}

		#region Deprecated methods in the original System.Net.DNS class

		/// <summary>
		///		Returns the Internet Protocol (IP) addresses for the specified host.
		/// </summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <returns>
		///		An array of type System.Net.IPAddress that holds the IP addresses for the
		///		host that is specified by the hostNameOrAddress parameter. 
		///</returns>
		public IPAddress[] GetHostAddresses(string hostNameOrAddress)
		{
			var entry = GetHostEntry(hostNameOrAddress);
			return entry.AddressList;
		}

		/// <summary>
		///		Asynchronously returns the Internet Protocol (IP) addresses for the specified
		///     host.
		/// </summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <param name="requestCallback">
		///		An System.AsyncCallback delegate that references the method to invoke when
		///     the operation is complete.
		/// </param>
		/// <param name="stateObject">
		///		A user-defined object that contains information about the operation. This
		///     object is passed to the requestCallback delegate when the operation is complete.
		///</param>
		/// <returns>An System.IAsyncResult instance that references the asynchronous request.</returns>
		public IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
		{
			GetHostAddressesDelegate g = GetHostAddresses;
			return g.BeginInvoke(hostNameOrAddress, requestCallback, stateObject);
		}

		/// <summary>
		///		Ends an asynchronous request for DNS information.
		/// </summary>
		/// <param name="asyncResult">
		///		An System.IAsyncResult instance returned by a call to the Heijden.Dns.Resolver.BeginGetHostAddresses(System.String,System.AsyncCallback,System.Object)
		///		method.
		/// </param>
		/// <returns></returns>
		public IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
		{
			var aResult = (AsyncResult) asyncResult;
			var g = (GetHostAddressesDelegate) aResult.AsyncDelegate;
			return g.EndInvoke(asyncResult);
		}

		/// <summary>
		///		Creates an System.Net.IPHostEntry instance from the specified System.Net.IPAddress.
		/// </summary>
		/// <param name="ip">An System.Net.IPAddress.</param>
		/// <returns>An System.Net.IPHostEntry.</returns>
		public IPHostEntry GetHostByAddress(IPAddress ip)
		{
			return GetHostEntry(ip);
		}

		/// <summary>
		///		Creates an System.Net.IPHostEntry instance from an IP address.
		/// </summary>
		/// <param name="address">An IP address.</param>
		/// <returns>An System.Net.IPHostEntry instance.</returns>
		public IPHostEntry GetHostByAddress(string address)
		{
			return GetHostEntry(address);
		}

		/// <summary>
		///		Gets the DNS information for the specified DNS host name.
		/// </summary>
		/// <param name="hostName">The DNS name of the host</param>
		/// <returns>An System.Net.IPHostEntry object that contains host information for the address specified in hostName.</returns>
		public IPHostEntry GetHostByName(string hostName)
		{
			return MakeEntry(hostName);
		}

		/// <summary>
		///		Asynchronously resolves an IP address to an System.Net.IPHostEntry instance.
		/// </summary>
		/// <param name="hostName">The DNS name of the host</param>
		/// <param name="requestCallback">An System.AsyncCallback delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="stateObject">
		///		A user-defined object that contains information about the operation. This
		///		object is passed to the requestCallback delegate when the operation is complete.
		/// </param>
		/// <returns>An System.IAsyncResult instance that references the asynchronous request.</returns>
		public IAsyncResult BeginGetHostByName(string hostName, AsyncCallback requestCallback, object stateObject)
		{
			GetHostByNameDelegate g = GetHostByName;
			return g.BeginInvoke(hostName, requestCallback, stateObject);
		}

		/// <summary>
		///		Ends an asynchronous request for DNS information.
		/// </summary>
		/// <param name="asyncResult">
		///		An System.IAsyncResult instance returned by a call to an 
		///		Heijden.Dns.Resolver.BeginGetHostByName method.
		/// </param>
		/// <returns></returns>
		public IPHostEntry EndGetHostByName(IAsyncResult asyncResult)
		{
			var aResult = (AsyncResult) asyncResult;
			var g = (GetHostByNameDelegate) aResult.AsyncDelegate;
			return g.EndInvoke(asyncResult);
		}

		/// <summary>
		///		Resolves a host name or IP address to an System.Net.IPHostEntry instance.
		/// </summary>
		/// <param name="hostName">A DNS-style host name or IP address.</param>
		/// <returns></returns>
		//[Obsolete("no problem",false)]
		public IPHostEntry Resolve(string hostName)
		{
			return MakeEntry(hostName);
		}

		/// <summary>
		///		Begins an asynchronous request to resolve a DNS host name or IP address to
		///     an System.Net.IPAddress instance.
		/// </summary>
		/// <param name="hostName">The DNS name of the host.</param>
		/// <param name="requestCallback">
		///		An System.AsyncCallback delegate that references the method to invoke when
		///     the operation is complete.
		///	</param>
		/// <param name="stateObject">
		///		A user-defined object that contains information about the operation. This
		///     object is passed to the requestCallback delegate when the operation is complete.
		/// </param>
		/// <returns>An System.IAsyncResult instance that references the asynchronous request.</returns>
		public IAsyncResult BeginResolve(string hostName, AsyncCallback requestCallback, object stateObject)
		{
			ResolveDelegate g = Resolve;
			return g.BeginInvoke(hostName, requestCallback, stateObject);
		}

		/// <summary>
		///		Ends an asynchronous request for DNS information.
		/// </summary>
		/// <param name="asyncResult">
		///		An System.IAsyncResult instance that is returned by a call to the System.Net.Dns.BeginResolve(System.String,System.AsyncCallback,System.Object)
		///     method.
		/// </param>
		/// <returns>An System.Net.IPHostEntry object that contains DNS information about a host.</returns>
		public IPHostEntry EndResolve(IAsyncResult asyncResult)
		{
			var aResult = (AsyncResult) asyncResult;
			var g = (ResolveDelegate) aResult.AsyncDelegate;
			return g.EndInvoke(asyncResult);
		}

		#region Nested type: GetHostAddressesDelegate

		private delegate IPAddress[] GetHostAddressesDelegate(string hostNameOrAddress);

		#endregion

		#region Nested type: GetHostByNameDelegate

		private delegate IPHostEntry GetHostByNameDelegate(string hostName);

		#endregion

		#region Nested type: ResolveDelegate

		private delegate IPHostEntry ResolveDelegate(string hostName);

		#endregion

		#endregion

		#region Nested type: GetHostEntryDelegate

		private delegate IPHostEntry GetHostEntryDelegate(string hostNameOrAddress);

		#endregion

		#region Nested type: GetHostEntryViaIPDelegate

		private delegate IPHostEntry GetHostEntryViaIPDelegate(IPAddress ip);

		#endregion

		#region Nested type: RRRecordStatus

		private enum RRRecordStatus
		{
			Unknown,
			Name,
			TTL,
			Class,
			Type,
			Value
		}

		#endregion

		#region Nested type: VerboseEventArgs

		///<summary>
		///</summary>
		public class VerboseEventArgs : EventArgs
		{
			///<summary>
			///</summary>
			public string Message;

			///<summary>
			///</summary>
			///<param name="message"></param>
			public VerboseEventArgs(string message)
			{
				Message = message;
			}
		}

		#endregion

		#region Nested type: VerboseOutputEventArgs

		///<summary>
		///</summary>
		public class VerboseOutputEventArgs : EventArgs
		{
			///<summary>
			///</summary>
			public string Message;

			///<summary>
			///</summary>
			///<param name="message"></param>
			public VerboseOutputEventArgs(string message)
			{
				Message = message;
			}
		}

		#endregion
	}

	// class
}