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
using System.Reflection;
using System.Xml;
#if __MonoCS__
using System.Security.Cryptography.X509Certificates;
#endif
using xmpp.common;
using xmpp.core;
using xmpp.net;
using xmpp.common.SASL;
using xmpp.registries;
using xmpp.states;
using xmpp.logging;

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
	/// of this library are as committed to open standards as the committees who created the original protocol.
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
	///			xmpp.ID = id;
	///			xmpp.Password = "password";
	/// 
	///			// Connect to the server
	///			xmpp.Connect();
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
    	private XID _id;
    	private int _port;
		private Boolean _ssl;
		
		private ProtocolState _states;

		/// <summary>
		/// Initializes a new instance of the <see cref="XMPP"/> class.
		/// </summary>
		public XMPP()
		{
			_parser = new ProtocolParser();
			_parser.StreamStart += new EventHandler<TagEventArgs>(_parser_StreamStart);
            _parser.StreamEnd += new EventHandler(_parser_StreamEnd);
			_parser.Tag += new EventHandler<TagEventArgs>(_parser_Tag);

			_socket.Connection += new EventHandler(_socket_Connection);
			_socket.Message += new EventHandler<MessageEventArgs>(_socket_Message);

			_reg.AddAssembly(Assembly.GetAssembly(typeof(XMPP)));
			_states = new ProtocolState(_socket);
		}

        private void _parser_StreamEnd(object sender, EventArgs e)
        {
            Logger.Debug(this, "Socket closing");
            _socket.Close();
        }

		private void _parser_Tag(object sender, TagEventArgs e)
		{
			Logger.Debug(this, "Got Tag...");
			Logger.DebugFormat(this, "State: {0}", _states.State.GetType().Name);
			
			_states.Execute(e);
		}

		private void _parser_StreamStart(object sender, TagEventArgs e)
		{
			Stream stream = e.Tag as Stream;
			if (stream != null)
			{
				if (stream.Version.StartsWith("1."))
				{
					if (_state == States.SASL)
					{
						_state = States.SASLAuthed;
					}
					else
					{
						_states.State = new ServerFeaturesState(_states);
					}
				}
			}
		}

    	private void _socket_Message(object sender, MessageEventArgs e)
		{
			_parser.Parse(e.Message);
		}

        /// <summary>
        /// Connect to the XMPP server for the XID
        /// </summary>
        public void Connect()
        {
            Logger.DebugFormat(this, "Connecting to {0}", _id.Server);
			_socket.Hostname = _id.Server;
			_socket.SSL = _ssl;
			_states.State = new ConnectingState(_states);
			_states.Execute(null);
        }

		private void _socket_Connection(object sender, EventArgs e)
		{	
			_states.State = new ConnectedState(_states);
			_states.Execute(null);
		}

        /// <summary>
        /// 
        /// </summary>
        public Boolean SSL
        {
            get { return _ssl; }
            set { _ssl = value; }
        }

    	///<summary>
    	///</summary>
    	public XID ID
    	{
    		get { return _id; }
			set { _id = value; }
    	}

    	///<summary>
    	///</summary>
    	public String Password
    	{
    		get { return _password; }
			set { _password = value; }
    	}

    	///<summary>
    	///</summary>
    	public int Port
    	{
    		get { return _port; }
			set { _port = value; }
		}

#if __MonoCS__
		/// <summary>
		/// 
		/// </summary>
		public X509Certificate LocalCertificate
		{
			get { return _socket.LocalCertificate; }
			set { _socket.LocalCertificate = value; }
		}
#endif
	}
}
