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

using System;
using System.IO;
using System.Net;
#if NET20
using System.Net.Security;
using System.Security.Authentication;
#elif __MonoCS__
using System.Net.Security;
using Mono.Security.Protocol.Tls;
#endif
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using xmpp.logging;

using System.Diagnostics;

namespace xmpp.net
{
    /// <remarks>
    /// AsyncSocket is the class that communicates with the server.
    /// </remarks>
	public class AsyncSocket
	{
        const int ConnectPortNo = 5222;
        const int SslConnectPortNo = 5223;

		private Socket _socket;
		private Decoder _decoder = Encoding.UTF8.GetDecoder();
		private UTF8Encoding _utf = new UTF8Encoding();
		private Address _dest;
		private byte[] _buff = new byte[4096];
		private Stream _stream;
		private string _hostname;
		private bool _ssl;
		private bool _secure;
		private NetworkStream _netstream;
#if NET20
		private SslStream _sslstream;
#endif
#if __MonoCS__
		private X509Certificate _local;
		private SslClientStream _sslstream;
#endif

        /// <summary>
        /// Occurs when a connection is established with a server.
        /// </summary>
		public event EventHandler Connection;

        /// <summary>
        /// Occurs when a message has been received from the server.
        /// </summary>
		public event EventHandler<MessageEventArgs> Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSocket"/> class.
        /// </summary>
		public AsyncSocket()
		{
		}

        /// <summary>
        /// Establishes a connection to the specified remote host.
        /// </summary>
		public void Connect()
		{
            int portNo;
            if (SSL)
                portNo = SslConnectPortNo;
            else
                portNo = ConnectPortNo;

			_dest = Address.Resolve(_hostname, portNo);
			Logger.InfoFormat(this, "Connecting to: {0} on port {1}", _dest.IP.ToString(), portNo.ToString());
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Connect(_dest.EndPoint);
            }
            catch (SocketException ex)
            {
                //We Failed to connect
            }
            if (_socket.Connected)
            {
                _netstream = new NetworkStream(_socket, true);
                _stream = _netstream;
                _stream.BeginRead(_buff, 0, _buff.Length, new AsyncCallback(Receive), null);
                OnConnect();
            }
		}

#if NET20
        /// <summary>
        /// Encrypts the connection using SSL/TLS
        /// </summary>
        public void StartSecure()
        {
			Logger.Debug(this, "Starting .NET Secure Mode");
            _sslstream = new SslStream(_netstream, false, new RemoteCertificateValidationCallback(RemoteValidation), null);
			Logger.Debug(this, "Authenticating as Client");
			try
			{
				_sslstream.AuthenticateAsClient(_dest.Hostname, null, SslProtocols.Tls | SslProtocols.Ssl2, false);
			} catch (Exception e)
			{
				Logger.ErrorFormat(this, "SSL Error: {0}", e);
			}
			_stream = _sslstream;
        }

        private static bool RemoteValidation(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
            {
                return true;
            }

			Logger.DebugFormat(typeof(AsyncCallback), "Policy Errors: {0}", errors);
            return false;
        }
#endif

#if __MonoCS__
		/// <summary>
		/// 
		/// </summary>
		public void StartSecure() 
		{
			Logger.Debug(this, "Starting Mono Secure Mode");
			try
			{
				Logger.Debug(this, "Creating Cert Collection");
				X509CertificateCollection certs = new X509CertificateCollection();
				Logger.DebugFormat(this, "Certificate Name: {0}", _local.Subject);
				certs.Add(_local);
				Logger.Debug(this, "Creating SslClientStream");
	            _sslstream = new SslClientStream(_netstream, _dest.Hostname, false, Mono.Security.Protocol.Tls.SecurityProtocolType.Tls, null);
				Logger.Debug(this, "Adding Validation Callback");
	            _sslstream.ServerCertValidationDelegate = new CertificateValidationCallback(this.RemoteValidation);
				Logger.Debug(this, "Changing variable to secure stream");
				_stream = _sslstream;
	            Logger.Debug(this, "Sending whitespace to start handshake");
	            Write(" ");
            }
            catch (Exception e)
            {
            	Logger.ErrorFormat(this, "Error starting secure socket - {0}", e);
            	throw;
            }
		}

		private bool RemoteValidation(X509Certificate certificate, int[] certificateErrors)
		{
			Logger.Debug(this, "Returning true from validation callback");
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public X509Certificate LocalCertificate
		{
			get { return _local; }
			set { _local = value; }
		}
#endif
        /// <summary>
        /// Closes the current socket.
        /// </summary>
        public void Close()
        {
            Logger.Debug(this, "Closing socket (Graceful Shutdown)");
            _socket.Close();
        }

        /// <summary>
        /// Writes data to the current connection.
        /// </summary>
        /// <param name="msg">Message to send</param>
		public void Write(string msg)
		{
			Logger.DebugFormat(this, "Outgoing Message: {0}", msg);
            byte[] mesg = _utf.GetBytes(msg);
			//_socket.Send(mesg, 0, mesg.Length, SocketFlags.None);
			_stream.Write(mesg, 0, mesg.Length);
		}

		private void OnConnect()
		{
			if (Connection != null)
			{
				Connection(this, EventArgs.Empty);
			}
		}

		private void OnMessage(String message)
		{
			if (Message != null)
			{
				Message(this, new MessageEventArgs(message));
			}
		}

		private void Receive(IAsyncResult ar)
		{
			try
			{
				int rx = _stream.EndRead(ar);
				char[] chars = new char[rx];
				_decoder.GetChars(_buff, 0, rx, chars, 0);
				string msg = new string(chars);
				Logger.DebugFormat(this, "Incoming Message: {0}", msg);
				_stream.BeginRead(_buff, 0, _buff.Length, new AsyncCallback(Receive), null);
				OnMessage(msg);
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
        /// Gets the current status of the socket.
        /// </summary>
		public bool Connected
		{
			get { return _socket.Connected; }
		}
		
		/// <value>
		/// 
		/// </value>
		public string Hostname
		{
			get { return _hostname; }
			set { _hostname = value; }
		}
		
		/// <value>
		/// 
		/// </value>
		public bool SSL
		{
			get { return _ssl; }
			set { _ssl = value; }
		}
		
		/// <value>
		/// 
		/// </value>
		public bool Secure
		{
			get { return _secure; }
			set { _secure = value; }
		}
	}

	/// <remarks>
	/// Provides data for the Message events.
	/// </remarks>
	public class MessageEventArgs : EventArgs
	{
		private string _message;

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
		/// </summary>
		/// <param name="message">The message received from the stream</param>
		public MessageEventArgs(String message)
		{
			_message = message;
		}

		/// <summary>
		/// Gets the message received from the stream.
		/// </summary>
		public String Message
		{
			get { return _message; }
			set { _message = value; }
		}
	}
}
