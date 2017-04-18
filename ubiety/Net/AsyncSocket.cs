// AsyncSocket.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2017 Dieter Lunn
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
using Serilog;
using Ubiety.Common;
using Ubiety.Infrastructure;
using Ubiety.Infrastructure.Extensions;
using Ubiety.Registries;
using Ubiety.States;

namespace Ubiety.Net
{
    /// <remarks>
    ///     AsyncSocket is the class that communicates with the server.
    /// </remarks>
    internal class AsyncSocket : IDisposable
    {
        // Timeout after 5 seconds by default
        /*
                private const int Timeout = 5000;
        */
        private const int BufferSize = 4096;

        private readonly byte[] _bufferBytes = new byte[BufferSize];
        private readonly Address _destinationAddress;
        private readonly ManualResetEvent _timeoutEvent = new ManualResetEvent(false);
        private readonly UTF8Encoding _utf = new UTF8Encoding();
        private bool _compressed;
        private ICompression _compression;
        private Socket _socket;
        private Stream _stream;

        public AsyncSocket()
        {
            _destinationAddress = new Address();
            ProtocolState.Events.OnSend += Events_OnSend;
        }

        #region Properties

        /// <summary>
        ///     Gets the current status of the socket.
        /// </summary>
        public bool Connected { get; private set; }

        #endregion

        public void Dispose()
        {
            _timeoutEvent.Dispose();
            _socket.Dispose();
        }

        private void Events_OnSend(object sender, TagEventArgs e)
        {
            Write(e.Tag.ToString());
        }

        /// <summary>
        ///     Establishes a connection to the specified remote host.
        /// </summary>
        /// <returns>True if we connected, false if we didn't</returns>
        public void Connect()
        {
            var address = _destinationAddress.NextIpAddress();
            IPEndPoint end;
            if (address != null)
            {
                end = new IPEndPoint(address, ProtocolState.Settings.Port);
            }
            else
            {
                ProtocolState.Events.Error(this, ErrorType.ConnectionTimeout, ErrorSeverity.Fatal,
                    "Unable to obtain server IP address.");
                return;
            }

            _socket = !_destinationAddress.IsIPv6
                ? new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                : new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

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
                Log.Error(e, "Error in connecting socket.");
                ProtocolState.Events.Error(this, ErrorType.ConnectionTimeout, ErrorSeverity.Fatal,
                    "Unable to connect to server.");
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

                if (ProtocolState.Settings.Ssl)
                {
                    if (StartSecure())
                        _stream.BeginRead(_bufferBytes, 0, BufferSize, Receive, null);
                }
                else
                {
                    _stream.BeginRead(_bufferBytes, 0, BufferSize, Receive, null);
                }

                ProtocolState.State = new ConnectedState();
                ProtocolState.State.Execute();
            }
            finally
            {
                _timeoutEvent.Set();
            }
        }

        /// <summary>
        ///     Disconnects the socket from the server.
        /// </summary>
        public void Disconnect()
        {
            Connected = false;
            _stream.Close();
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Disconnect(true);
        }

        /// <summary>
        ///     Encrypts the connection using SSL/TLS
        /// </summary>
        public bool StartSecure()
        {
            var sslstream = new SslStream(_stream, true, RemoteValidation);
            try
            {
                Log.Debug("Authenticating SSL connection...");
                sslstream.AuthenticateAsClient(_destinationAddress.Hostname, null, SslProtocols.Tls, false);
                if (sslstream.IsAuthenticated)
                {
                    Log.Debug("SSL Authenticated");
                    _stream = sslstream;
                    ProtocolState.Encrypted = true;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in starting secure connection.");
                ProtocolState.Events.Error(this, ErrorType.XmlError, ErrorSeverity.Fatal, "Cannot connect with SSL.");
                return false;
            }
        }

        private static bool RemoteValidation(object sender, X509Certificate cert, X509Chain chain,
            SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;

            Log.Debug(cert.ToString());

            Log.Error("SSL Policy Errors: {0}", errors);
            foreach (var chainStatus in chain.ChainStatus)
                Log.Debug("X509Chain Information: {0} - Flags: {1}", chainStatus.StatusInformation, chainStatus.Status);

            return false;
        }

        /// <summary>
        ///     Writes data to the current connection.
        /// </summary>
        /// <param name="msg">Message to send</param>
        public void Write(string msg)
        {
            Log.Debug("(AsyncSocket:Write) Outgoing message: {Message}", msg);

            if (!Connected) return;
            var mesg = _utf.GetBytes(msg);
            mesg = _compressed ? _compression.Deflate(mesg) : mesg;
            _stream.Write(mesg, 0, mesg.Length);
        }

        private void Receive(IAsyncResult ar)
        {
            try
            {
                _stream.EndRead(ar);

                Log.Debug("(AsyncSocket:Receive) Raw Buffer: {Buffer}", _bufferBytes);

                var t = _bufferBytes.TrimNull();

                var m = _utf.GetString(_compressed ? _compression.Inflate(t, t.Length) : t);

                Log.Debug("(AsyncSocket:Receive) Incoming Message: {Message}", m);

                ProtocolParser.Parse(m);

                // Clear the buffer otherwise we get leftover tags and it confuses the parser.
                _bufferBytes.Clear();

                if (!Connected || ProtocolState.State is DisconnectedState) return;

                _stream.BeginRead(_bufferBytes, 0, _bufferBytes.Length, Receive, null);
            }
            catch (SocketException e)
            {
                Log.Error(e, "Error in socket receiving data.");
            }
            catch (InvalidOperationException e)
            {
                Log.Error(e, "Socket committed an invalid operation trying to receive data.");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="algorithm"></param>
        public void StartCompression(string algorithm)
        {
            _compression = CompressionRegistry.GetCompression(algorithm);
            _compressed = true;
        }
    }
}