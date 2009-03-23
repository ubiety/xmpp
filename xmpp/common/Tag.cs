// Tag.cs
//
//XMPP .NET Library Copyright (C) 2006 - 2009 Dieter Lunn
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

using System.Xml;
using System;

namespace ubiety.common
{
    /// <summary>
    /// Tag is the class from which all tags are subclassed.
    /// </summary>
	public class Tag : XmlElement
	{
		static int _packetCounter;
	
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
		
		public Tag() : base("", "", "", null)
		{
		}
		
		public void AddChildTag(Tag child)
		{
			if (this.OwnerDocument == child.OwnerDocument)
				this.AppendChild(child);
			else
				this.AppendChild(this.OwnerDocument.ImportNode(child, true));
		}
		
		public T GetEnumAttribute<T>(string name)
		{
			string a = GetAttribute(name);
			return (T) Enum.Parse(typeof(T), name, false);
		}
		
		public string GetNextID()
		{
			System.Threading.Interlocked.Increment(ref _packetCounter);
			return "U" + _packetCounter.ToString();
		}
		
		#region << Properties >>
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
			set { SetAttribute("id", value); }
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
		
		/// <value>
		/// 
		/// </value>
		public byte[] Bytes
			{
			get { return Convert.FromBase64String(InnerText); }
			set { InnerText = Convert.ToBase64String(value); }
		}
		#endregion
	}
}
