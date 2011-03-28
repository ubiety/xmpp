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
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ubiety.common;
using ubiety.logging;
using ubiety.states;

namespace ubiety.net
{
	/// <remarks>
	/// AsyncSocket is the class that communicates with the server.
	/// </remarks>
	internal class AsyncSocket
	{
		private readonly byte[] _buff = new byte[4096];
		private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
		private readonly ProtocolState _states = ProtocolState.Instance;
		private readonly UTF8Encoding _utf = new UTF8Encoding();
		private bool _compressed;
		private Deflater _deflate;
		private Address _dest;

		// Used to determine if we are encrypting the socket to turn off returning the message to the parser
		private bool _encrypting;
		private Inflater _inflate;
		private NetworkStream _netstream;
		private Socket _socket;
		private SslStream _sslstream;
		private Stream _stream;

		// Timeout after 15 seconds by default
		private const int Timeout = 15000;

		/// <summary>
		/// Gets the current status of the socket.
		/// </summary>
		public bool Connected { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string Hostname { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public bool Secure { get; set; }

		/// <summary>
		/// Establishes a connection to the specified remote host.
		/// </summary>
		/// <returns>True if we connected, false if we didn't</returns>
		public void Connect()
		{
			Hostname = !String.IsNullOrEmpty(Settings.Hostname) ? Settings.Hostname : Settings.Id.Server;

			_dest = Address.Resolve(Hostname, Settings.Port);
			if (_dest == null)
				return;
			Logger.InfoFormat(this, "Connecting to: {0} on port {1}", _dest.Ip.ToString(), _dest.Port.ToString());
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
				//_socket.Connect(_dest.EndPoint);
				_socket.BeginConnect(_dest.EndPoint, FinishConnect, null);
				if (_resetEvent.WaitOne(Timeout, false))
				{
					if (Connected)
					{
						_netstream = new NetworkStream(_socket, true);
						_stream = _netstream;
						_stream.BeginRead(_buff, 0, _buff.Length, Receive, null);
						_states.State = new ConnectedState();
						_states.Execute();
					}
				}
				else
				{
					Errors.Instance.SendError(this, ErrorType.ConnectionTimeout, "Timed out while connecting to server.");
				}
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
				_socket.EndConnect(ar);
				Connected = true;
			}
			finally
			{
				_resetEvent.Set();
			}
		}

		/// <summary>
		/// Encrypts the connection using SSL/TLS
		/// </summary>
		public void StartSecure()
		{
			//_encrypting = true;
			Logger.Debug(this, "Starting .NET Secure Mode");
			_sslstream = new SslStream(_stream, true, RemoteValidation, null);
			Logger.Debug(this, "Authenticating as Client");
			try
			{
				_sslstream.AuthenticateAsClient(_dest.Hostname, null, SslProtocols.Tls, false);
				if (_sslstream.IsAuthenticated)
				{
					_stream = _sslstream;
					//_resetEvent.Set();
				}
				//_resetEvent.WaitOne();
			}
			catch (Exception e)
			{
				Logger.ErrorFormat(this, "SSL Error: {0}", e);
				Errors.Instance.SendError(this, ErrorType.XMLError, "SSL connection error", true);
			}
			//_encrypting = false;
		}

		/*
		private void EndAuthenticate(IAsyncResult result)
		{
			_sslstream.EndAuthenticateAsClient(result);
			if (_sslstream.IsAuthenticated)
			{
				_stream = _sslstream;
				_resetEvent.Set();
			}
		} */

		private static bool RemoteValidation(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
		{
			if (errors == SslPolicyErrors.None)
			{
				return true;
			}

			Logger.DebugFormat(typeof (AsyncSocket), "Policy Errors: {0}", errors);
			return false;
		}

		/// <summary>
		/// Closes the current socket.
		/// </summary>
		public void Close()
		{
			Logger.Debug(this, "Closing socket (Graceful Shutdown)");
			_stream.Close();
			_socket.Close();
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
			mesg = _compressed ? Deflate(mesg) : mesg;
			_stream.Write(mesg, 0, mesg.Length);
		}

		private void Receive(IAsyncResult ar)
		{
			try
			{
				if (!Connected || _states.State is ClosedState)
				{
					return;
				}
				//Logger.Debug(this, ar.GetType().FullName);
				var rx = _stream.EndRead(ar);

				var t = TrimNull(_buff);

				var m = _utf.GetString(_compressed ? Inflate(t, t.Length) : t);

				Logger.DebugFormat(this, "Incoming Message: {0}", m);
				if (!_encrypting)
					ProtocolParser.Parse(m, rx);
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
			//Logger.DebugFormat(this, "Replacing stream with {0} compressed version.", algorithm);
			//_stream = CompressionRegistry.Instance.GetCompression(algorithm, _stream);
			Logger.Debug(this, "Starting compression with SharpZipLib");
			_inflate = new Inflater();
			_deflate = new Deflater();
			_compressed = true;
		}

		private static byte[] TrimNull(byte[] message)
		{
			if (message.Length > 1)
			{
				var c = message.Length - 1;
				while (message[c] == 0x00)
				{
					c--;
				}

				var r = new byte[(c + 1)];
				for (var i = 0; i < (c + 1); i++)
				{
					r[i] = message[i];
				}

				return r;
			}

			return null;
		}

		#region << Compression >>

		private byte[] Deflate(byte[] data)
		{
			int ret;

			_deflate.SetInput(data);
			_deflate.Flush();

			var ms = new MemoryStream();
			do
			{
				var buf = new byte[4096];
				ret = _deflate.Deflate(buf);
				if (ret > 0)
					ms.Write(buf, 0, ret);
			} while (ret > 0);

			return ms.ToArray();
		}

		private byte[] Inflate(byte[] data, int length)
		{
			int ret;

			_inflate.SetInput(data, 0, length);

			var ms = new MemoryStream();
			do
			{
				var buffer = new byte[4096];
				ret = _inflate.Inflate(buffer);
				if (ret > 0)
					ms.Write(buffer, 0, ret);
			} while (ret > 0);

			return ms.ToArray();
		}

		#endregion
	}
}