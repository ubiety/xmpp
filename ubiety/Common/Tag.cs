// Tag.cs
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
using System.Threading;
using System.Xml;

namespace Ubiety.Common
{
    /// <summary>
    ///     Tag is the class from which all tags are subclassed.
    /// </summary>
    public abstract class Tag : XmlElement
    {
        private static int _packetCounter;

        /// <summary>
        ///     XML document all messages to and from the server will be a part of.
        /// </summary>
        public static readonly XmlDocument Document = new XmlDocument();

        /// <summary>
        ///     Initializes a new instance of a tag with the described prefix and qualified name.
        /// </summary>
        /// <param name="prefix">Tag Prefix</param>
        /// <param name="qname">Qualified Namespace</param>
        protected Tag(string prefix, XmlQualifiedName qname)
            : base(prefix, qname.Name, qname.Namespace, Document)
        {
        }

        /// <summary>
        ///     Initializes a new blank tag instance.
        /// </summary>
        protected Tag() : base("", "", "", Document)
        {
        }

        /// <summary>
        ///     Adds a tag as a child of the current tag.
        /// </summary>
        /// <param name="child">Tag to add as the child.</param>
        public void AddChildTag(Tag child)
        {
            if (OwnerDocument == child.OwnerDocument)
                AppendChild(child);
            else if (OwnerDocument != null) AppendChild(OwnerDocument.ImportNode(child, true));
        }

        /// <summary>
        ///     Returns the value of the names attribute as the specified enumeration.
        /// </summary>
        /// <param name="name">Attribute to return.</param>
        /// <typeparam name="T">Enumeration type to parse the attribute as.</typeparam>
        /// <returns>Enumeration value of the named attribute.</returns>
        protected T GetEnumAttribute<T>(string name)
        {
            var value = Attributes[name].Value;
            return (T) Enum.Parse(typeof (T), value, true);
        }

        /// <summary>
        ///     Calculates and returns a new unique id
        /// </summary>
        /// <returns>The next available unique id</returns>
        protected string GetNextId()
        {
            Interlocked.Increment(ref _packetCounter);
            return "U" + _packetCounter;
        }

        /// <summary>
        ///     Implicit cast of a tag to a string.
        /// </summary>
        /// <param name="one">Tag to cast to string</param>
        /// <returns>String version of the tag</returns>
        public static implicit operator string(Tag one)
        {
            return one.ToString();
        }

        /// <summary>
        ///     Returns a string representation of the tag.
        /// </summary>
        /// <returns>String object representing the tag.</returns>
        public override string ToString()
        {
            return OuterXml;
        }

        #region << Properties >>

        /// <value>
        ///     Tag child contents as a byte array
        /// </value>
        public byte[] Bytes
        {
            get { return Convert.FromBase64String(InnerText); }
            set { InnerText = Convert.ToBase64String(value); }
        }

        #endregion
    }
}