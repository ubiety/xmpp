// Iq.cs
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
using ubiety.common;
using ubiety.infrastructure.attributes;

namespace ubiety.core.iq
{
	///<summary>
	///</summary>
	public enum IqType
	{
		///<summary>
		///</summary>
		Get,
		///<summary>
		///</summary>
		Set,
		///<summary>
		///</summary>
		Error,
		///<summary>
		///</summary>
		Result
	}

	///<summary>
	///</summary>
	[XmppTag("iq", Namespaces.Client, typeof (Iq))]
	public class Iq : Stanza
	{
		///<summary>
		///</summary>
		public Iq() : base("", new XmlQualifiedName("iq", Namespaces.Client))
		{
			Id = GetNextId();
		}

		///<summary>
		///</summary>
		public IqType IqType
		{
			get { return GetEnumAttribute<IqType>("type"); }
			set { SetAttribute("type", value.ToString().ToLower()); }
		}

		///<summary>
		///</summary>
		public Tag Payload
		{
			get { return (Tag) FirstChild; }
			set { AddChildTag(value); }
		}
	}
}