// Auth.cs
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

namespace ubiety.core.SASL
{
	///<summary>
	///</summary>
	[XmppTag("auth", Namespaces.SASL, typeof (Auth))]
	public class Auth : Tag
	{
		///<summary>
		///</summary>
		///<param name="doc"></param>
		public Auth() : base("", new XmlQualifiedName("auth", Namespaces.SASL))
		{
		}

		///<summary>
		///</summary>
		public string Mechanism
		{
			get { return GetAttribute("mechanism"); }
			set { SetAttribute("mechanism", value); }
		}

		///<summary>
		///</summary>
		public string Text
		{
			get { return InnerText; }
			set { InnerText = value; }
		}
	}
}