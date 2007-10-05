//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System.Xml;
using xmpp.common;

namespace xmpp.core
{
    /// <summary>
    /// 
    /// </summary>
	[XmppTag("features", Namespaces.STREAM, typeof(Features))]
	public class Features : xmpp.common.Tag
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
		public Features(string prefix, XmlQualifiedName qname, XmlDocument doc)
			: base(prefix, qname, doc)
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
			get { return this["starttls", Namespaces.START_TLS] as StartTLS; }
		}
	}
}
