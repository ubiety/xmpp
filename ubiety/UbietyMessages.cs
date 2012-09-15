// UbietyMessages.cs
//
//Ubiety XMPP Library Copyright (C) 2011 - 2012 Dieter Lunn
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
using ubiety.common;
using ubiety.states;

namespace ubiety
{
	/// <summary>
	/// 
	/// </summary>
	public class MessageEventArgs : EventArgs
	{
		/// <summary>
		/// 
		/// </summary>
		public Tag Tag;
	}

	/// <summary>
	/// 
	/// </summary>
	public static class UbietyMessages
	{
		/// <summary>
		/// 
		/// </summary>
		public static event EventHandler<MessageEventArgs> AllMessages;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public static void OnAllMessages(MessageEventArgs e)
		{
			if (AllMessages != null) AllMessages(typeof(UbietyMessages), e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <exception cref="NotImplementedException"></exception>
		public static void SendMessage(string text)
		{
			ProtocolState.Socket.Write(text);
		}
	}
}