//XMPP .NET Library Copyright (C) 2006 Dieter Lunn

//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 2.1 of the License, or (at your option)
//any later version.

//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//details.

//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Xml;

using xmpp.common;

namespace xmpp.core
{
    public enum MechanismType
    {
        None,
        PLAIN = (1 << 0),
        DIGEST_MD5 = (1 << 1),
        EXTERNAL = (1 << 2)
    }

    /// <summary>
    /// 
    /// </summary>
	[XmppTag("mechanisms", xmpp.common.Namespaces.SASL, typeof(Mechanisms))]
	public class Mechanisms : Tag
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
		public Mechanisms(string prefix, XmlQualifiedName qname, XmlDocument doc)
			: base(prefix, qname, doc)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Mechanism[] GetMechanisms()
        {
            XmlNodeList nl = GetElementsByTagName("mechanism", xmpp.common.Namespaces.SASL);
            Mechanism[] items = new Mechanism[nl.Count];
            int i = 0;
            foreach (XmlNode node in nl)
            {
                items[i] = (Mechanism)node;
                i++;
            }
            return items;
        }

        public MechanismType SupportedTypes
        {
            get
            {
                MechanismType type = MechanismType.None;
                foreach (Mechanism m in GetMechanisms())
                {
                    type |= m.Type;
                }

                return type;
            }
        }
	}

    /// <summary>
    /// 
    /// </summary>
	[XmppTag("mechanism", xmpp.common.Namespaces.SASL, typeof(Mechanism))]
	public class Mechanism : Tag
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
		public Mechanism(string prefix, XmlQualifiedName qname, XmlDocument doc)
			: base(prefix, qname, doc)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get { return this.InnerText; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MechanismType Type
        {
            get { return Mechanism.GetType(Name); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MechanismType GetType(string type)
        {
            switch (type)
            {
                case "PLAIN":
                    return MechanismType.PLAIN;
                case "DIGEST-MD5":
                    return MechanismType.DIGEST_MD5;
                case "EXTERNAL":
                    return MechanismType.EXTERNAL;
                default:
                    return MechanismType.None;
            }
        }
	}
}
