// Settings.cs
//
//Ubiety XMPP Library Copyright (C) 2011 Dieter Lunn
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

using ubiety.core;
using ubiety.common;

namespace ubiety
{
	///<summary>
	///</summary>
	public static class UbietySettings
	{
		private static int _port = 5222;

		/// <summary>
		/// Gets or sets the authentication types to be used when connecting to a server.
		/// </summary>
		/// <value>
		/// The authentication type or types to be used.
		/// </value>
		public static MechanismType AuthenticationTypes
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ID of the user for authentication.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		public static XID Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the users password for authentication.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		public static string Password
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the hostname used to connect to the server if you don't want to connect to the server provided in the ID.
		/// </summary>
		/// <value>
		/// The hostname.
		/// </value>
		public static string Hostname
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the port to connect to if your server doesn't support the default 5222 and its not set with SRV.
		/// </summary>
		/// <value>
		/// The port.
		/// </value>
		public static int Port
		{
			get { return _port; }
			set { _port = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this connection should use SSL encryption.
		/// </summary>
		/// <value>
		///   <c>true</c> if SSL is to be used; otherwise, <c>false</c>.
		/// </value>
		public static bool SSL { get; set; }
	}
}
