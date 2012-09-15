// Errors.cs
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

using System;
using ubiety.common;

namespace ubiety
{
	/// <summary>
	/// Arguments class used to pass errors to the application.
	/// </summary>
	public class ErrorEventArgs : EventArgs
	{
		private readonly Boolean _fatal;
		private readonly string _message;
		private readonly ErrorType _type;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="fatal"></param>
		public ErrorEventArgs(string message, ErrorType type, Boolean fatal)
		{
			_message = message;
			_type = type;
			_fatal = fatal;
		}

		/// <value>
		/// The default error message.
		/// </value>
		public string Message
		{
			get { return _message; }
		}

		/// <value>
		/// The type of error that is being returned.
		/// </value>
		public ErrorType Type
		{
			get { return _type; }
		}

		///<summary>
		///</summary>
		public Boolean Fatal
		{
			get { return _fatal; }
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static class Errors
	{
		///<summary>
		///</summary>
		//public static readonly Errors Instance = new Errors();

		/// <summary>
		/// Subscribing to this event will allow you to receive all errors generated in the library.
		/// </summary>
		public static event EventHandler<ErrorEventArgs> OnError;

		/// <summary>
		/// Sends an error from the calling class to the application.
		/// </summary>
		/// <param name="sender">
		/// The object sending the error.
		/// </param>
		/// <param name="type">
		/// The type of error as defined by <see cref="ErrorType"/>.
		/// </param>
		/// <param name="message">
		/// The default message to be supplied with the error.
		/// </param>
		public static void SendError(object sender, ErrorType type, string message, Boolean fatal = false)
		{
			if (OnError != null)
			{
				OnError(sender, new ErrorEventArgs(message, type, fatal));
			}
		}
	}
}