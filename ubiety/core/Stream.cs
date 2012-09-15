// Stream.cs
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

using System.Text;
using System.Xml;
using ubiety.common;
using ubiety.common.attributes;

namespace ubiety.core
{
	/// <summary>
	/// 
	/// </summary>
	[XmppTag("stream", Namespaces.Stream, typeof (Stream))]
	public class Stream : Stanza
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="doc"></param>
		public Stream(XmlDocument doc)
			: base("stream", new XmlQualifiedName("stream", Namespaces.Stream), doc)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public string Version
		{
			get { return GetAttribute("version"); }
			set { SetAttribute("version", value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Ns
		{
			get { return GetAttribute("xmlns"); }
			set { SetAttribute("xmlns", value); }
		}

		///<summary>
		///</summary>
		public Features Features
		{
			get { return this["features", Namespaces.Stream] as Features; }
		}

		///<summary>
		///</summary>
		public string Lang
		{
			get { return !HasAttribute("lang", Namespaces.XML) ? null : GetAttribute("lang", Namespaces.XML); }
			set
			{
				if (HasAttribute("lang", Namespaces.XML))
					RemoveAttribute("lang", Namespaces.XML);
				if (value == null) return;
				if (OwnerDocument == null) return;
				var attr = OwnerDocument.CreateAttribute("xml:lang", Namespaces.XML);
				attr.Value = value;
				Attributes.Append(attr);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string StartTag()
		{
			var sb = new StringBuilder("<");
			sb.Append(Name);
			sb.Append(" xmlns");
			if (Prefix != null)
			{
				sb.Append(":");
				sb.Append(Prefix);
			}
			sb.Append("=\"");
			sb.Append(NamespaceURI);
			sb.Append("\"");

			foreach (XmlAttribute attr in Attributes)
			{
				sb.Append(" ");
				sb.Append(attr.Name);
				sb.Append("=\"");
				sb.Append(attr.Value);
				sb.Append("\"");
			}
			sb.Append(">");
			return sb.ToString();
		}
	}
}