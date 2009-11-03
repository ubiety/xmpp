// XMPP.cs
//
//XMPP .NET Library Copyright (C) 2006 - 2009 Dieter Lunn
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
using ubiety.common;
using ubiety.core;
using ubiety.net;
using ubiety.registries;
using ubiety.states;
using ubiety.logging;
#endregion

namespace ubiety
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
	///			XMPP ubiety = new XMPP();
	///
	///			ubiety.ID = id;
	///			ubiety.Password = "password";
	/// 
	///			// Connect to the server
	///			ubiety.Connect();
	///		}
	/// }
	/// </code>
	/// </example>
	public class XMPP
    {
        #region Private Members
        private TagRegistry _reg = TagRegistry.Instance;
		private int _port = 5222;
		private Boolean _ssl = false;
        private string _hostName = null;
		private Errors _errors = Errors.Instance;
		private ProtocolState _states = ProtocolState.Instance;
		private static string _version = "";
        #endregion

        /// <summary>
		/// Initializes a new instance of the <see cref="XMPP"/> class.
		/// </summary>
		public XMPP()
		{
			Assembly x = Assembly.GetAssembly(typeof(XMPP));
			_reg.AddAssembly(x);
			_errors.OnError += new EventHandler<ErrorEventArgs>(OnError);
			_version = x.GetName().Version.ToString();
			_states.Socket = new AsyncSocket();
		}

        /// <summary>
        /// Connect to an XMPP server with an XID, hostname and password.
        /// </summary>
        /// <param name="id">The XID that should be used for connecting.</param>
        /// <param name="hostname">The hostname we should connect to instead of the XID's.</param>
        /// <param name="password">The password that should be used for authentication.</param>
        public void Connect(XID id, string hostname, string password)
        {
            this.ID = id;
            this.HostName = hostname;
            this.Password = password;
            Connect();
        }

        /// <summary>
        /// Connect to an XMPP server with the XID and password.
        /// </summary>
        /// <param name="id">The XID that should be used for connecting.</param>
        /// <param name="password">The password that should be used for authentication.</param>
        public void Connect(XID id, string password)
        {
            this.ID = id;
            this.Password = password;
            Connect();
        }

        /// <summary>
        /// Connect to the XMPP server for the XID
        /// </summary>
        public void Connect()
        {
        	// We need an XID and Password to connect to the server.
			if (String.IsNullOrEmpty(_states.Password))
			{
				_errors.SendError(this, ErrorType.MissingPassword, "Set the Password property before connecting.");
				return;
			}
			else if (String.IsNullOrEmpty(_states.ID))
			{
				_errors.SendError(this, ErrorType.MissingID, "Set the ID property before connecting.");
				return;
			}

			// Do we use the server supplied from the XID or the alternate provided by the developer?
            if (!String.IsNullOrEmpty(_hostName))
                _states.Socket.Hostname = _hostName;
            else
                _states.Socket.Hostname = _states.ID.Server;

			Logger.InfoFormat(this, "Connecting to {0}", _states.Socket.Hostname);

			// Set the values we need to connect.
			_states.Socket.SSL = _ssl;
            _states.Socket.Port = _port;
			// Set the current state to connecting and start the process.
			_states.State = new ConnectingState();
			_states.Execute();
		}
		
		/// <summary>
		/// Disconnect from the XMPP server
		/// </summary>
		public void Disconnect()
		{
			_states.State = new DisconnectState();
			_states.Execute();
		}
		
		private void OnError(object sender, ErrorEventArgs e)
		{
			Logger.ErrorFormat(this, "Error from {0}: {1}", sender, e.Message);
		}

        #region Properties
        /// <summary>
        /// This property defines whether SSL should be used to connect to the server.
        /// </summary>
        public Boolean SSL
        {
            get { return _ssl; }
            set { _ssl = value; }
        }

		/// <summary>
		/// This is the XID we should use to determine the server and user name to connect to.
		/// </summary>
		public XID ID
		{
			get { return _states.ID; }
			set { _states.ID = value; }
		}

		/// <summary>
		/// The password for the username supplied in the XID.
		/// </summary>
		public String Password
		{
			get { return _states.Password; }
			set { _states.Password = value; }
		}

		/// <summary>
		/// The port to connect to.  Should only be used if not connecting to the default 5222.
		/// </summary>
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}

        /// <summary>
        /// The hostname to connect to.  Should only be used if the hostname is not part of the XID.
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        /// <summary>
        /// Provides the Ubiety assembly version
        /// </summary>
        public static string Version
        {
        	get { return _version; }
        }
		
        /// <summary>
        /// Returns true if it is not in a closed state.
        /// </summary>
		public bool Connected
		{
			get 
			{
				if (_states.State.GetType() == typeof(ClosedState))
					return false;
				else
					return true;
			}
		}
        #endregion
    }
}
