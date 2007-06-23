//XMPP .NET Library Copyright (C) 2006 Dieter Lunn

//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 2.1 of the License, or (at your option)
//any later version.

//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//details.

//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Xml;
using System.IO;
using System.Reflection;

using xmpp.core;
using xmpp.net;
using xmpp.common;

using log4net;
using log4net.Config;

namespace xmpp
{
	#region State Enumeration
    /// <summary>
    /// The current state the connection is currently in.
    /// </summary>
	public enum States
	{
		/// <summary>
		/// All Done and working
		/// </summary>
		Running,
		/// <summary>
		/// No connection
		/// </summary>
		Closed,
		/// <summary>
		/// Starting the connection process
		/// </summary>
		Connecting,
		/// <summary>
		/// We have a connection
		/// </summary>
		Connected,
		/// <summary>
		/// Received a stream:stream
		/// </summary>
		Stream,
		/// <summary>
		/// Closing the connection
		/// </summary>
		Closing,
		/// <summary>
		/// Waiting for connection timeout
		/// </summary>
		Reconnecting,
		/// <summary>
		/// Excepting a stream:features tag from the server
		/// </summary>
		ServerFeatures,
		/// <summary>
		/// Starting SASL authentication
		/// </summary>
		SASL,
		/// <summary>
		/// We are authenticated
		/// </summary>
		SASLAuthed,
		/// <summary>
		/// Binding to a server resource
		/// </summary>
		Bind,
        /// <summary>
        /// Starting TLS socket encryption
        /// </summary>
        StartTLS
	}
	#endregion

    /// <summary>
    /// Implements the XMPP(Jabber) Core and IM protocols
    /// </summary>
	/// <remarks>
	/// <para>
	/// The Extensible Messaging and Presence Protocol (XMPP) is an open XML technology for real-time
	/// communications, which powers a wide range of applications including instant messaging, presence,
	/// media negotiation, whiteboarding, collaboration, lightweight middleware, content syndication, and
	/// generalized XML delivery.
	/// </para>
	/// <para>
	/// This library is an implementation of this protocol.  Those involved with the design and development
	/// of this library are as commited to open standards as the committees who created the original protocol.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// public class Test
	/// {
	///		public static Main()
	///		{
	///			// Create a new ID for authentication
	///			XID id = new XID("user@jabber.org/chat");
	/// 
	///			// Create a new instance of the XMPP class
	///			XMPP xmpp = new XMPP();
	/// 
	///			// Connect to the server
	///			xmpp.Connect(id.Server);
	///		}
	/// }
	/// </code>
	/// </example>
	public class XMPP
	{
		private TagRegistry _reg = TagRegistry.Instance;
		private AsyncSocket _socket = new AsyncSocket();
		private ProtocolParser _parser;
		private States _state;

        private String _password;
        private Boolean _ssl;

        private static readonly ILog logger = LogManager.GetLogger(typeof(XMPP));

		/// <summary>
		/// Initializes a new instance of the <see cref="XMPP"/> class.
		/// </summary>
		public XMPP()
		{
            XmlConfigurator.Configure();
			_parser = new ProtocolParser();
			_parser.StreamStart += new EventHandler<TagEventArgs>(_parser_StreamStart);
            _parser.StreamEnd += new EventHandler(_parser_StreamEnd);
			_parser.Tag += new EventHandler<TagEventArgs>(_parser_Tag);

			_socket.Connection += new EventHandler(_socket_Connection);
			_socket.Message += new EventHandler<MessageEventArgs>(_socket_Message);

			_reg.AddAssembly(Assembly.GetAssembly(typeof(XMPP)));
		}

        private void _parser_StreamEnd(object sender, EventArgs e)
        {
            logger.Debug("Socket closing");
            _socket.Close();
        }

		private void _parser_Tag(object sender, TagEventArgs e)
		{
			logger.Debug("Got Tag...");
			switch(_state)
			{
                case States.ServerFeatures:
                    logger.Debug("Determining features...");
    				Features f = e.Tag as Features;
	    			if (f == null)
		    		{
			    		throw new Exception("Expecting stream:features from a 1.x server");
				    }

                    logger.Debug(f.StartTLS);
    				if (f.StartTLS != null && _ssl)
	    			{
                        logger.Debug("Starting TLS encryption..State: StartTLS");
                        _state = States.StartTLS;
                        StartTLS tls = (StartTLS)_reg.GetTag("", new XmlQualifiedName("starttls", xmpp.common.Namespaces.START_TLS), new XmlDocument());
                        _socket.Write(tls);
				    }
                    break;
                case States.StartTLS:
                    if (e.Tag.Name == "proceed")
                    {
                        _socket.StartSecure();
                        SendStartStream();
                    }
                    break;
			}
		}

		private void _parser_StreamStart(object sender, TagEventArgs e)
		{
			xmpp.core.Stream stream = e.Tag as xmpp.core.Stream;
			if (stream.Version.StartsWith("1."))
			{
				if (_state == States.SASL)
				{
					_state = States.SASLAuthed;
				}
				else
				{
                    logger.Debug("State: ServerFeatures");
					_state = States.ServerFeatures;
				}
			}
		}

		private void _socket_Message(object sender, MessageEventArgs e)
		{
			_parser.Parse(e.Message);
		}

        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="address">The address to connect to.</param>
        [Obsolete("Deprecated for Connect(XID, Password)")]
		public void Connect(string address)
		{
            logger.Debug("Connecting to " + address);
			_state = States.Connecting;
			_socket.Connect(address, _ssl);
		}

        /// <summary>
        /// Connect to the XMPP server for the XID
        /// </summary>
        /// <param name="id">Id used for connecting.</param>
        /// <param name="Password">Password to login with.</param>
        public void Connect(XID id, string Password)
        {
            _password = Password;
            logger.Debug("Connecting to " + id.Server);
            _state = States.Connecting;
            _socket.Connect(id.Server, _ssl);
        }

		private void _socket_Connection(object sender, EventArgs e)
		{
			_state = States.Connected;
            SendStartStream();
		}

        private void SendStartStream()
        {
            xmpp.core.Stream stream = (xmpp.core.Stream)_reg.GetTag("stream", new XmlQualifiedName("stream", xmpp.common.Namespaces.STREAM), new XmlDocument());
            stream.Version = "1.0";
            stream.To = "localhost";
            stream.NS = "jabber:client";
            _socket.Write("<?xml version='1.0' encoding='UTF-8'?>" + stream.StartTag());
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean SSL
        {
            get { return _ssl; }
            set { _ssl = value; }
        }
	}
}
