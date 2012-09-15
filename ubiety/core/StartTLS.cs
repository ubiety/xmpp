// StartTLS.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2012 Dieter Lunn
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
using ubiety.common.attributes;

namespace ubiety.core
{
	/// <summary>
	/// StartTLS is used to start an encrypted authentication session.
	/// </summary>
	[XmppTag("starttls", Namespaces.StartTLS, typeof (StartTLS))]
	public class StartTLS : Tag
	{
		/// <summary>
		/// Creates a new instance of the StartTLS tag.
		/// </summary>
		/// <param name="doc">XmlDocument used for the tag.</param>
		public StartTLS()
			: base("", new XmlQualifiedName("starttls", Namespaces.StartTLS))
		{
		}

		/// <summary>
		/// Is StartTLS required by the server for authentication.
		/// </summary>
		public bool Required
		{
			get { return this["required"] != null; }
		}
	}
}