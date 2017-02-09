// Mechanisms.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2017 Dieter Lunn
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Ubiety.Common;
using Ubiety.Infrastructure.Attributes;

namespace Ubiety.Core
{
    /// <summary>
    /// </summary>
    [Flags]
    public enum MechanismType
    {
        /// <summary>
        ///     No Authentication
        /// </summary>
        None,

        /// <summary>
        ///     Plain Text Authentication (Only use on encrypted connections)
        /// </summary>
        Plain = (1 << 0),

        /// <summary>
        ///     DIGEST-MD5 Authentication
        /// </summary>
        DigestMd5 = (1 << 1),

        /// <summary>
        ///     External Certificate Authentication (Not Implmented Yet)
        /// </summary>
        External = (1 << 2),

        /// <summary>
        ///     SCRAM-SHA-1 Authentication
        /// </summary>
        Scram = (1 << 3),

        /// <summary>
        ///     Default Authentication Types (SCRAM-SHA-1 and DIGEST-MD5)
        /// </summary>
        Default = Scram | DigestMd5
    }

    /// <summary>
    /// </summary>
    [XmppTag("mechanisms", Namespaces.Sasl, typeof (Mechanisms))]
    public class Mechanisms : Tag
    {
        /// <summary>
        /// </summary>
        public Mechanisms()
            : base("", new XmlQualifiedName("mechanisms", Namespaces.Sasl))
        {
        }

        /// <summary>
        /// </summary>
        public MechanismType SupportedTypes
        {
            get { return GetMechanisms().Aggregate(MechanismType.None, (current, m) => current | m.Type); }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Mechanism> GetMechanisms()
        {
            var nl = GetElementsByTagName("mechanism", Namespaces.Sasl);
            var items = new Mechanism[nl.Count];
            var i = 0;
            foreach (XmlNode node in nl)
            {
                items[i] = (Mechanism) node;
                i++;
            }
            return items;
        }
    }

    /// <summary>
    /// </summary>
    [XmppTag("mechanism", Namespaces.Sasl, typeof (Mechanism))]
    public class Mechanism : Tag
    {
        /// <summary>
        /// </summary>
        public Mechanism()
            : base("", new XmlQualifiedName("mechanism", Namespaces.Sasl))
        {
        }

        /// <summary>
        /// </summary>
        public string Text => InnerText;

        /// <summary>
        /// </summary>
        public MechanismType Type => GetType(Text);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MechanismType GetType(string type)
        {
            switch (type)
            {
                case "PLAIN":
                    return MechanismType.Plain;
                case "DIGEST-MD5":
                    return MechanismType.DigestMd5;
                case "EXTERNAL":
                    return MechanismType.External;
                case "SCRAM-SHA-1":
                    return MechanismType.Scram;
                default:
                    return MechanismType.None;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMechanism(MechanismType type)
        {
            switch (type)
            {
                case MechanismType.Plain:
                    return "PLAIN";
                case MechanismType.External:
                    return "EXTERNAL";
                case MechanismType.DigestMd5:
                    return "DIGEST-MD5";
                case MechanismType.Scram:
                    return "SCRAM-SHA-1";
                case MechanismType.None:
                    return "";
                case MechanismType.Default:
                    return "";
                default:
                    return "";
            }
        }
    }
}