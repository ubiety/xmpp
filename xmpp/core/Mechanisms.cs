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
    ///<summary>
	///</summary>
    public enum MechanismType
    {
        ///<summary>
        ///</summary>
        None,
        ///<summary>
        ///</summary>
        PLAIN = (1 << 0),
        ///<summary>
        ///</summary>
        DIGEST_MD5 = (1 << 1),
        ///<summary>
        ///</summary>
        EXTERNAL = (1 << 2)
    }

    /// <summary>
    /// 
    /// </summary>
	[XmppTag("mechanisms", Namespaces.SASL, typeof(Mechanisms))]
	public class Mechanisms : xmpp.common.Tag
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
            XmlNodeList nl = GetElementsByTagName("mechanism", Namespaces.SASL);
            Mechanism[] items = new Mechanism[nl.Count];
            int i = 0;
            foreach (XmlNode node in nl)
            {
                items[i] = (Mechanism)node;
                i++;
            }
            return items;
        }

        ///<summary>
        ///</summary>
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
	[XmppTag("mechanism", Namespaces.SASL, typeof(Mechanism))]
	public class Mechanism : xmpp.common.Tag
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
        public string Text
        {
            get { return InnerText; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MechanismType Type
        {
            get { return GetType(Text); }
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

		///<summary>
		///</summary>
		///<param name="type"></param>
		///<returns></returns>
		public static string GetMechanism(MechanismType type)
		{
			switch (type)
			{
				case MechanismType.PLAIN:
					return "PLAIN";
				case MechanismType.EXTERNAL:
					return "EXTERNAL";
				case MechanismType.DIGEST_MD5:
					return "DIGEST-MD5";
				default:
					return "";
			}
		}
	}
}
