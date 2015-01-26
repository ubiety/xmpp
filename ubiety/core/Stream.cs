// Stream.cs
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

using System.Text;
using System.Xml;
using ubiety.common;
using ubiety.infrastructure.attributes;

namespace ubiety.core
{
    /// <summary>
    /// </summary>
    [XmppTag("stream", Namespaces.Stream, typeof (Stream))]
    public class Stream : Stanza
    {
        /// <summary>
        /// </summary>
        public Stream()
            : base("stream", new XmlQualifiedName("stream", Namespaces.Stream))
        {
        }

        /// <summary>
        /// </summary>
        public string Version
        {
            get { return GetAttribute("version"); }
            set { SetAttribute("version", value); }
        }

        /// <summary>
        /// </summary>
        public string Namespace
        {
            get { return GetAttribute("xmlns"); }
            set { SetAttribute("xmlns", value); }
        }

        /// <summary>
        /// </summary>
        public Features Features
        {
            get { return this["features", Namespaces.Stream] as Features; }
        }

        /// <summary>
        /// </summary>
        public string Language
        {
            get { return !HasAttribute("lang", Namespaces.Xml) ? null : GetAttribute("lang", Namespaces.Xml); }
            set
            {
                if (HasAttribute("lang", Namespaces.Xml))
                    RemoveAttribute("lang", Namespaces.Xml);
                if (value == null) return;
                if (OwnerDocument == null) return;
                XmlAttribute attr = OwnerDocument.CreateAttribute("xml:lang", Namespaces.Xml);
                attr.Value = value;
                Attributes.Append(attr);
            }
        }

        /// <summary>
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