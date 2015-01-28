// Stanza.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2015 Dieter Lunn
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

namespace Ubiety.Common
{
	///<summary>
	///</summary>
	public class Stanza : Tag
	{
		///<summary>
		///</summary>
		///<param name="prefix"></param>
		///<param name="qname"></param>
		protected Stanza(string prefix, XmlQualifiedName qname) : base(prefix, qname)
		{
		}

		/// <summary>
		/// Where the message is going.
		/// </summary>
		public JID To
		{
			get { return new JID(GetAttribute("to")); }
			set { SetAttribute("to", value); }
		}

		/// <summary>
		/// Where the message came from.
		/// </summary>
		public JID From
		{
			get { return new JID(GetAttribute("from")); }
			set { SetAttribute("from", value); }
		}

		/// <summary>
		/// The server id.
		/// </summary>
		public string Id
		{
			get { return GetAttribute("id"); }
			set { SetAttribute("id", value); }
		}
	}
}