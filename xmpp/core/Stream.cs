// Stream.cs
//
//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.//
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
using ubiety.attributes;

namespace ubiety.core
{
    /// <summary>
    /// 
    /// </summary>
	[XmppTag("stream", Namespaces.STREAM, typeof(Stream))]
	public class Stream : ubiety.common.Tag
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
		public Stream(XmlDocument doc)
			: base("stream", new XmlQualifiedName("stream", Namespaces.STREAM), doc)
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
		public string NS
		{
			get { return GetAttribute("xmlns"); }
			set { SetAttribute("xmlns", value); }
		}
		
		public Features Features
		{
			get { return this["features", Namespaces.STREAM] as Features; }
		}
		
		public string Lang
		{
			get 
			{
				if (!HasAttribute("lang", Namespaces.XML))
					return null;
				return GetAttribute("lang", Namespaces.XML);
			}
			set 
			{
				if (HasAttribute("lang", Namespaces.XML))
					RemoveAttribute("lang", Namespaces.XML);
				if (value != null)
				{
					XmlAttribute attr = OwnerDocument.CreateAttribute("xml:lang", Namespaces.XML);
					attr.Value = value;
					this.Attributes.Append(attr);
				}
			}
		}
		
		public string To
		{
			get { return GetAttribute("to"); }
			set { SetAttribute("to", value); }
		}
		
		public string ID
		{
			get { return GetAttribute("id"); }
			set { SetAttribute("id", value); }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public string StartTag()
		{
			StringBuilder sb = new StringBuilder("<");
			sb.Append(Name);
			if (NamespaceURI != null)
			{
				sb.Append(" xmlns");
				if (Prefix != null)
				{
					sb.Append(":");
					sb.Append(Prefix);
				}
				sb.Append("=\"");
				sb.Append(NamespaceURI);
				sb.Append("\"");
			}
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
