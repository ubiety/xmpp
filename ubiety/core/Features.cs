// Features.cs
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
using ubiety.attributes;
using ubiety.common;
using ubiety.core.compression;

namespace ubiety.core
{
	/// <summary>
	/// 
	/// </summary>
	[XmppTag("features", Namespaces.Stream, typeof (Features))]
	public class Features : Tag
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="doc"></param>
		public Features(XmlDocument doc)
			: base("stream", new XmlQualifiedName("features", Namespaces.Stream), doc)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public Mechanisms StartSASL
		{
			get { return this["mechanisms", Namespaces.SASL] as Mechanisms; }
		}

		/// <summary>
		/// 
		/// </summary>
		public StartTLS StartTLS
		{
			get { return this["starttls", Namespaces.StartTLS] as StartTLS; }
		}

		///<summary>
		///</summary>
		public Compression Compression
		{
			get { return this["compression", Namespaces.Compression] as Compression; }
		}
	}
}