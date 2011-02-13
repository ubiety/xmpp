// GenericTag.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2010 Dieter Lunn
// 
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// 
// You should have received a copy of the GNU Lesser General Public License along
// with this library; if not, write to the Free Software Foundation, Inc., 59
// Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Xml;
using ubiety.common;
using ubiety.attributes;

namespace ubiety.core
{
    /// <summary>
    /// 
    /// </summary>
	[XmppTag("register", Namespaces.REGISTER, typeof(GenericTag))]
	[XmppTag("auth", Namespaces.AUTH, typeof(GenericTag))]
	[XmppTag("bad-request", Namespaces.STANZAS, typeof(GenericTag))]
    [XmppTag("ver", Namespaces.ROSTVER, typeof(GenericTag))]
    [XmppTag("optional", Namespaces.ROSTVER, typeof(GenericTag))]
    [XmppTag("optional", Namespaces.SESSION, typeof(GenericTag))]
    [XmppTag("required", Namespaces.START_TLS, typeof(GenericTag))]
    [XmppTag("required", Namespaces.BIND, typeof(GenericTag))]
    [XmppTag("unsupported-stanza-type", Namespaces.XMPP_STREAMS, typeof(GenericTag))]
    // Compression Tags
    [XmppTag("method", Namespaces.COMPRESSION, typeof(GenericTag))]
    [XmppTag("compress", Namespaces.COMPRESSION_PROTOCOL, typeof(GenericTag))]
    [XmppTag("method", Namespaces.COMPRESSION_PROTOCOL, typeof(GenericTag))]
    [XmppTag("presence", Namespaces.CLIENT, typeof(GenericTag))]
    [XmppTag("proceed", Namespaces.START_TLS, typeof(GenericTag))]
    [XmppTag("malformed-request", Namespaces.SASL, typeof(GenericTag))]
    public class GenericTag : Tag
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="qname"></param>
		public GenericTag(XmlDocument doc, XmlQualifiedName qname) : base("", qname, doc)
		{
		}
	}
}
