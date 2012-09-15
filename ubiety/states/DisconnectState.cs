// DisconnectState.cs
//
//Ubiety XMPP Library Copyright (C) 2008 - 2012 Dieter Lunn
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

using ubiety.common;

namespace ubiety.states
{
	/// <summary>
	/// The state that disconnects from the server.
	/// </summary>
	public class DisconnectState : State
	{
		/// <summary>
		/// Executes the disconnect command by sending the closing stream tag and closing the socket.
		/// </summary>
		/// <param name="data">
		/// No <see cref="ubiety.common.Tag"/> needed so we pass null.
		/// </param>
		public override void Execute(Tag data = null)
		{
			if (ProtocolState.Socket.Connected)
			{
				ProtocolState.Socket.Write("</stream:stream>");
			}
		}
	}
}