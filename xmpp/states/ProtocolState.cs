// ProtocolState.cs
//
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

using System.Xml;
using ubiety.net;
using ubiety.common.SASL;
using ubiety.common;

namespace ubiety.states
{
	/// <summary>
	/// Keeps track of all the current state information like id, password, socket and the current state.
	/// </summary>
	public class ProtocolState
	{
		private State _state;
		private AsyncSocket _socket;
		private SASLProcessor _proc;
		private XID _id;
		private string _password;
		private bool _authenticated = false;
		private bool _compress = false;
		private string _algorithm;
		private XmlDocument _doc = new XmlDocument();
		
		private static ProtocolState _instance = new ProtocolState();
		
		private ProtocolState()
		{
			_state = new ClosedState();
		}
		
		/// <summary>
		/// Executes the current state.
		/// </summary>
		/// <param name="data">
		/// The <see cref="Tag"/> used in the state.
		/// </param>
		public void Execute(Tag data)
		{
			_state.Execute(data);
		}
		
		/// <value>
		/// The current state we are in.
		/// </value>
		public State State
		{
			get { return _state; }
			set { _state = value; }
		}
		
		/// <value>
		/// The socket used for connecting to the server.
		/// </value>
		public AsyncSocket Socket
		{
			get { return _socket; }
			set { _socket = value; }
		}
		
		/// <value>
		/// The current SASL processor based on server communication.
		/// </value>
		public SASLProcessor Processor
		{
			get { return _proc; }
			set { _proc = value; }
		}
		
		/// <value>
		/// The current XID used by the socket to connect.
		/// </value>
		public XID ID
		{
			get { return _id; }
			set { _id = value; }
		}
		
		/// <value>
		/// The password used to authenticate the user.
		/// </value>
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}
		
		/// <value>
		/// Are we authenticated yet?
		/// </value>
		public bool Authenticated
		{
			get { return _authenticated; }
			set { _authenticated = value; }
		}
		
		/// <value>
		/// Should we compress the stream if available?
		/// </value>
		public bool Compress
		{
			get { return _compress; }
			set { _compress = value; }
		}
		
		public string Algorithm
		{
			get { return _algorithm; }
			set { _algorithm = value; }
		}
		
		public XmlDocument Document
		{
			get { return _doc; }
		}
		
		/// <value>
		/// The current instance of the ProtocolState class.
		/// </value>
		public static ProtocolState Instance
		{
			get { return _instance; }
		}
	}
}
