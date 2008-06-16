//XMPP .NET Library Copyright (C) 2006, 2007, 2008 Dieter Lunn
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

#region Usings
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
#endregion

namespace xmpp
{
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

        #region Private Members
        private TagRegistry _reg = TagRegistry.Instance;
		private AsyncSocket _socket = new AsyncSocket();
		private ProtocolParser _parser;
		private String _password;
		private XID _id;
		private int _port;
		private Boolean _ssl;
        private string _hostName = null;
		
		private ProtocolState _states;
        #endregion

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
			
			_states.Execute(e.Tag);
		}

		private void _parser_StreamStart(object sender, TagEventArgs e)
		{
			Stream stream = e.Tag as Stream;
			if (stream.Version.StartsWith("1."))
			{
				_states.State = new ServerFeaturesState(_states);
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

            if (!String.IsNullOrEmpty(_hostName))
                _socket.Hostname = _hostName;
            else
                _socket.Hostname = _id.Server;

			_socket.SSL = _ssl;
            _socket.Port = _port;
			_states.ID = _id;
			_states.Password = _password;
			_states.State = new ConnectingState(_states);
			_states.Execute(null);
		}

		private void _socket_Connection(object sender, EventArgs e)
		{	
			_states.State = new ConnectedState(_states);
			_states.Execute(null);
        }

        #region Properties
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

        /// <summary>
        /// 
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }
        #endregion
    }
}
