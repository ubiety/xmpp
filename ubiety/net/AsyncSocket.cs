// AsyncSocket.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2011 Dieter Lunn
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
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using ubiety.common;
using ubiety.common.extensions;
using ubiety.common.logging;
using ubiety.registries;
using ubiety.states;

namespace ubiety.net
{
	/// <remarks>
	/// AsyncSocket is the class that communicates with the server.
	/// </remarks>
	internal class AsyncSocket
	{
		// Timeout after 5 seconds by default
		private const int Timeout = 5000;
		private const int BufferSize = 4096;
		private readonly byte[] _buff = new byte[BufferSize];
		private readonly Address _dest;
		private readonly ProtocolState _states = ProtocolState.Instance;
		private readonly ManualResetEvent _timeoutEvent = new ManualResetEvent(false);
		private readonly UTF8Encoding _utf = new UTF8Encoding();
		private ICompression _comp;
		private bool _compressed;
		private Socket _socket;
		private Stream _stream;

		#region Properties

		public AsyncSocket()
		{
			_dest = new Address();
		}

		/// <summary>
		/// Gets the current status of the socket.
		/// </summary>
		public bool Connected { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string Hostname
		{
			get { return _dest.Hostname; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Secure { get; set; }

		#endregion

		/// <summary>
		/// Establishes a connection to the specified remote host.
		/// </summary>
		/// <returns>True if we connected, false if we didn't</returns>
		public void Connect()
		{
			var address = _dest.NextIPAddress();
			IPEndPoint end;
			if (address != null)
			{
				end = new IPEndPoint(address, UbietySettings.Port);
			}
			else
			{
				Errors.Instance.SendError(this, ErrorType.ConnectionTimeout, "Unable to obtain server IP address.");
				return;
			}

			Logger.InfoFormat(this, "Trying to connect to: {2}({0}:{1})", end.Address, UbietySettings.Port.ToString(),
				                  UbietySettings.Hostname);

			if (!_dest.IPv6)
			{
				Logger.Debug(this, "Connecting using IPv4");
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			}
			else
			{
				Logger.Debug(this, "Connecting using IPv6");
				_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
			}

			try
			{
				_socket.BeginConnect(end, FinishConnect, _socket);
				//if (!_timeoutEvent.WaitOne(Timeout))
				//{
				//    Errors.Instance.SendError(this, ErrorType.ConnectionTimeout, "Timed out connecting to server.");
				//    return;
				//}
			}
			catch (SocketException e)
			{
				Errors.Instance.SendError(this, ErrorType.ConnectionTimeout, e.Message);
			}
		}

		private void FinishConnect(IAsyncResult ar)
		{
			try
			{
				var socket = (Socket) ar.AsyncState;
				socket.EndConnect(ar);

				Connected = true;

				var netstream = new NetworkStream(socket);
				_stream = netstream;

				_stream.BeginRead(_buff, 0, BufferSize, Receive, null);

				_states.State = new ConnectedState();
				_states.State.Execute();
			}
			finally
			{
				_timeoutEvent.Set();
			}
		}

		/// <summary>
		/// Disconnects the socket from the server.
		/// </summary>
		public void Disconnect()
		{
			Logger.Debug(this, "Closing socket (Graceful Shutdown)");
			Connected = false;
			_stream.Close();
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Disconnect(true);
		}

		/// <summary>
		/// Encrypts the connection using SSL/TLS
		/// </summary>
		public void StartSecure()
		{
			Logger.Debug(this, "Starting .NET Secure Mode");
			var sslstream = new SslStream(_stream, true, RemoteValidation);
			Logger.Debug(this, "Authenticating as Client");
			try
			{
				sslstream.AuthenticateAsClient(_dest.Hostname, null, SslProtocols.Tls, false);
				if (sslstream.IsAuthenticated)
				{
					_stream = sslstream;
				}
			}
			catch (Exception e)
			{
				Logger.ErrorFormat(this, "SSL Error: {0}", e);
				Errors.Instance.SendError(this, ErrorType.XMLError, "SSL connection error", true);
			}
		}

		private static bool RemoteValidation(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
		{
			if (errors == SslPolicyErrors.None)
			{
				return true;
			}

			Logger.DebugFormat(typeof (AsyncSocket), "X509Chain {0}", chain.ChainStatus[0].Status);
			Logger.DebugFormat(typeof (AsyncSocket), "Policy Errors: {0}", errors);
			return false;
		}

		/// <summary>
		/// Writes data to the current connection.
		/// </summary>
		/// <param name="msg">Message to send</param>
		public void Write(string msg)
		{
			if (!Connected) return;
			Logger.DebugFormat(this, "Outgoing Message: {0}", msg);
			var mesg = _utf.GetBytes(msg);
			mesg = _compressed ? _comp.Deflate(mesg) : mesg;
			_stream.Write(mesg, 0, mesg.Length);
		}

		private void Receive(IAsyncResult ar)
		{
			try
			{
				_stream.EndRead(ar);

				var t = _buff.TrimNull();

				var m = _utf.GetString(_compressed ? _comp.Inflate(t, t.Length) : t);

				Logger.DebugFormat(this, "Incoming Message: {0}", m);
				ProtocolParser.Parse(m);

				// Clear the buffer otherwise we get leftover tags and it confuses the parser.
				_buff.Clear();

				if (!Connected || _states.State is DisconnectedState) return;

				_stream.BeginRead(_buff, 0, _buff.Length, Receive, null);
			}
			catch (SocketException e)
			{
				Logger.DebugFormat(this, "Socket Exception: {0}", e);
			}
			catch (InvalidOperationException e)
			{
				Logger.DebugFormat(this, "Invalid Operation: {0}", e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="algorithm"></param>
		public void StartCompression(string algorithm)
		{
			_comp = CompressionRegistry.Instance.GetCompression(algorithm);
			_compressed = true;
		}
	}
}