// GenericTag.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2012 Dieter Lunn
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

using System.Xml;
using ubiety.common;
using ubiety.common.attributes;

namespace ubiety.core
{
	/// <summary>
	/// 
	/// </summary>
	[XmppTag("register", Namespaces.Register, typeof (GenericTag))]
	[XmppTag("auth", Namespaces.Auth, typeof (GenericTag))]
	[XmppTag("bad-request", Namespaces.Stanzas, typeof (GenericTag))]
	[XmppTag("ver", Namespaces.Rostver, typeof (GenericTag))]
	[XmppTag("optional", Namespaces.Rostver, typeof (GenericTag))]
	[XmppTag("optional", Namespaces.Session, typeof (GenericTag))]
	[XmppTag("required", Namespaces.StartTLS, typeof (GenericTag))]
	[XmppTag("required", Namespaces.Bind, typeof (GenericTag))]
	[XmppTag("session", Namespaces.Session, typeof(GenericTag))]
	[XmppTag("resource", Namespaces.Bind, typeof(GenericTag))]
	[XmppTag("error", Namespaces.Client, typeof(GenericTag))]
	[XmppTag("ping", Namespaces.Ping, typeof(GenericTag))]
	[XmppTag("query", Namespaces.Roster, typeof(GenericTag))]

	// SASL
	[XmppTag("success", Namespaces.SASL, typeof(GenericTag))]
	[XmppTag("response", Namespaces.SASL, typeof(GenericTag))]
	[XmppTag("failure", Namespaces.SASL, typeof(GenericTag))]
	[XmppTag("not-authorized", Namespaces.SASL, typeof(GenericTag))]
	[XmppTag("bad-protocol", Namespaces.SASL, typeof(GenericTag))]
	[XmppTag("challenge", Namespaces.SASL, typeof(GenericTag))]

	// Errors
	[XmppTag("unsupported-stanza-type", Namespaces.XMPPStreams, typeof (GenericTag))]

	// Compression Tags
	[XmppTag("method", Namespaces.Compression, typeof (GenericTag))]
	[XmppTag("compress", Namespaces.CompressionProtocol, typeof (GenericTag))]
	[XmppTag("compressed", Namespaces.CompressionProtocol, typeof(GenericTag))]
	[XmppTag("method", Namespaces.CompressionProtocol, typeof (GenericTag))]
	[XmppTag("presence", Namespaces.Client, typeof (GenericTag))]
	[XmppTag("proceed", Namespaces.StartTLS, typeof (GenericTag))]
	[XmppTag("malformed-request", Namespaces.SASL, typeof (GenericTag))]
	public class GenericTag : Tag
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="qname"></param>
		public GenericTag(XmlQualifiedName qname) : base("", qname)
		{
		}
	}
}