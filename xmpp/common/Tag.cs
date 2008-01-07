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
using System;

namespace xmpp.common
{
    /// <summary>
    /// Tag is the class from which all tags are subclassed.
    /// </summary>
	public class Tag : XmlElement
	{
        /// <summary>
        /// Creates a new tag
        /// </summary>
        /// <param name="prefix">Tag Prefix</param>
        /// <param name="qname">Qualified Namespace</param>
        /// <param name="doc">XmlDocument associated with the tag</param>
		public Tag(string prefix, XmlQualifiedName qname, XmlDocument doc)
			: base(prefix, qname.Name, qname.Namespace, doc)
		{
		}

        /// <summary>
        /// Where the message is going.
        /// </summary>
		public XID To
		{
			get { return new XID(GetAttribute("to")); }
			set { SetAttribute("to", value); }
		}

        /// <summary>
        /// Where the message came from.
        /// </summary>
		public XID From
		{
			get { return new XID(GetAttribute("from")); }
			set { SetAttribute("from", value); }
		}

        /// <summary>
        /// The server id.
        /// </summary>
		public string ID
		{
			get { return GetAttribute("id"); }
		}

        ///<summary>
        ///</summary>
        ///<param name="one"></param>
        ///<returns></returns>
        public static implicit operator string(Tag one)
        {
            return one.ToString();
        }

        /// <summary>
        /// Returns a string representation of the tag.
        /// </summary>
        /// <returns>String object representing the tag.</returns>
		public override string ToString()
		{
			return OuterXml;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="one">
		/// A <see cref="Tag"/>
		/// </param>
		/// <param name="two">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator ==(Tag one, string two)
		{
			return one.LocalName == two;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="one">
		/// A <see cref="Tag"/>
		/// </param>
		/// <param name="two">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator !=(Tag one, string two)
		{
            if (two == null)
                return false;

			return one.LocalName != two;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool Equals (object o)
		{
			return (this == (Tag)o);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
		
		public byte[] Bytes
			{
			get { return Convert.FromBase64String(InnerText); }
			set { InnerText = Convert.ToBase64String(value); }
		}
	}
}
