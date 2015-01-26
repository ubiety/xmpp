// Compression.cs
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

using System.Collections.Generic;
using System.Xml;
using ubiety.common;
using ubiety.infrastructure.attributes;

namespace ubiety.core.compression
{
    /// <summary>
    /// </summary>
    [XmppTag("compression", Namespaces.Compression, typeof (Compression))]
    public class Compression : Tag
    {
        /// <summary>
        /// </summary>
        public Compression()
            : base("", new XmlQualifiedName("compression", Namespaces.Compression))
        {
        }

        /// <summary>
        /// </summary>
        public IEnumerable<string> Algorithms
        {
            get
            {
                XmlNodeList nl = GetElementsByTagName("method", Namespaces.Compression);
                var als = new string[nl.Count];
                int i = 0;
                foreach (XmlElement m in nl)
                {
                    als[i] = m.InnerText;
                    i++;
                }
                return als;
            }
        }
    }
}