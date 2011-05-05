// C.cs
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

using System.Xml;
using ubiety.attributes;
using ubiety.common;

namespace ubiety.core.entity
{
	/// <summary>
	/// &lt;c/&gt; element as described in XEP-0115
	/// </summary>
	[XmppTag("c", Namespaces.Entity, typeof (C))]
	public class C : Tag
	{
		///<summary>
		///</summary>
		///<param name="doc"></param>
		public C(XmlDocument doc)
			: base("", new XmlQualifiedName("c", Namespaces.Entity), doc)
		{
		}

		///<summary>
		/// A URI that uniquely identifies a software application, typically a URL at the website of the project or company that produces the software.
		///</summary>
		public string Node
		{
			get { return GetAttribute("node"); }
		}

		///<summary>
		/// A string that is used to verify the identity and supported features of the entity.
		///</summary>
		public string Ver
		{
			get { return GetAttribute("ver"); }
		}
	}
}