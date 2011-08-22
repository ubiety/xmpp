// ProtocolState.cs
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

using System.Xml;
using ubiety.common;
using ubiety.common.SASL;
using ubiety.net;

namespace ubiety.states
{
	/// <summary>
	/// Keeps track of all the current state information like id, password, socket and the current state.
	/// </summary>
	internal class ProtocolState
	{
		public static readonly ProtocolState Instance = new ProtocolState();
		private readonly XmlDocument _doc = new XmlDocument();

		private ProtocolState()
		{
			State = new DisconnectedState();
			UbietyMessages.Instance.AllMessages += UbietyMessagesAllMessages;
		}

		private void UbietyMessagesAllMessages(object sender, MessageArgs e)
		{
			State.Execute(e.Tag);
		}

		/// <value>
		/// The current state we are in.
		/// </value>
		public State State { get; set; }

		/// <value>
		/// The socket used for connecting to the server.
		/// </value>
		internal AsyncSocket Socket { get; set; }

		/// <value>
		/// The current SASL processor based on server communication.
		/// </value>
		public SASLProcessor Processor { get; set; }

		/// <value>
		/// Are we authenticated yet?
		/// </value>
		public bool Authenticated { get; set; }

		/// <summary>
		/// Is the stream currently compressed?
		/// </summary>
		public bool Compressed { get; set; }

		public string Algorithm { get; set; }

		public XmlDocument Document
		{
			get { return _doc; }
		}

		/// <summary>
		/// Executes the current state.
		/// </summary>
		/// <param name="data">
		/// The <see cref="Tag"/> used in the state.
		/// </param>
		//public void Execute(Tag data = null)
		//{
		//    _state.Execute(data);
		//}
	}
}