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
	///			XMPP ubiety = XMPP.Connect(id, "password");
	///		}
	/// }
	/// </code>
	/// </example>
	public class XMPP
    {
        #region Private Members
        private TagRegistry _reg = TagRegistry.Instance;
		private static Errors _errors = Errors.Instance;
		private static ProtocolState _states = ProtocolState.Instance;
		//private static string _version = "";
        #endregion

        /// <summary>
		/// Initializes a new instance of the <see cref="XMPP"/> class.
		/// </summary>
		private XMPP()
		{
			Assembly x = Assembly.GetAssembly(typeof(XMPP));
			_reg.AddAssembly(x);
			_errors.OnError += new EventHandler<ErrorEventArgs>(OnError);
			//_version = x.GetName().Version.ToString();
		}

        /// <summary>
        /// Connect to an XMPP server on port 5222 unencrypted with the XID and password.
        /// </summary>
        /// <param name="id">The XID that should be used for connecting.</param>
        /// <param name="password">The password that should be used for authentication.</param>
        public static XMPP Connect(XID id, string password)
        {
			return Connect(id, password, id.Server, 5222, false);
        }

		/// <summary>
		/// Connect to an XMPP server.
		/// </summary>
		/// <param name="id">The XID to be used for connecting.</param>
		/// <param name="password">The password to be used for authentication</param>
		/// <param name="hostname">The hostname to connect to. Can be set to null to use XID server.</param>
		/// <param name="port">The port to connect to.</param>
		/// <param name="ssl">Use encryption if available?</param>
		/// <returns></returns>
		public static XMPP Connect(XID id, string password, string hostname, int port, bool ssl)
        {
			XMPP me = new XMPP();

			_states.Socket = new AsyncSocket();
        	// We need an XID and Password to connect to the server.
			if (String.IsNullOrEmpty(password))
			{
				_errors.SendError(typeof(XMPP), ErrorType.MissingPassword, "Set the Password property before connecting.");
				return null;
			}
			else if (String.IsNullOrEmpty(id))
			{
				_errors.SendError(typeof(XMPP), ErrorType.MissingID, "Set the ID property before connecting.");
				return null;
			}

			// Do we use the server supplied from the XID or the alternate provided by the developer?
			if (!String.IsNullOrEmpty(hostname))
				_states.Socket.Hostname = hostname;
			else
				_states.Socket.Hostname = id.Server;

			Logger.InfoFormat(typeof(XMPP), "Connecting to {0}", _states.Socket.Hostname);

			// Set the values we need to connect.
			_states.Socket.SSL = ssl;
            _states.Socket.Port = port;
			// Set the current state to connecting and start the process.
			_states.State = new ConnectingState();
			_states.Execute();

			return me;
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
        /// Provides the Ubiety assembly version
        /// </summary>
        public static string Version
        {
			get { return "0.2"; }
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
