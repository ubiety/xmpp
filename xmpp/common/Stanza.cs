// Stanza.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2009 Dieter Lunn
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

namespace ubiety.common
{
	public class Stanza : Tag
	{
		public Stanza(string prefix, XmlQualifiedName qname, XmlDocument doc) : base (prefix, qname, doc)
		{
		}

        /// <summary>
        /// Where the message is going.
        /// </summary>
		public XID To
		{
			get { return new XID(GetAttribute("to")); }
			set { SetAttribute("to", value); }
		}

        /// <summary>
        /// Where the message came from.
        /// </summary>
		public XID From
		{
			get { return new XID(GetAttribute("from")); }
			set { SetAttribute("from", value); }
		}

        /// <summary>
        /// The server id.
        /// </summary>
		public string ID
		{
			get { return GetAttribute("id"); }
			set { SetAttribute("id", value); }
		}	
	}
}
