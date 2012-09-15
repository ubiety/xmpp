// Tag.cs
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

using System;
using System.Threading;
using System.Xml;
using ubiety.states;

namespace ubiety.common
{
	/// <summary>
	/// Tag is the class from which all tags are subclassed.
	/// </summary>
	public abstract class Tag : XmlElement
	{
		private static int _packetCounter;

		/// <summary>
		/// Creates a new tag
		/// </summary>
		/// <param name="prefix">Tag Prefix</param>
		/// <param name="qname">Qualified Namespace</param>
		/// <param name="doc">XmlDocument associated with the tag</param>
		protected Tag(string prefix, XmlQualifiedName qname)
			: base(prefix, qname.Name, qname.Namespace, ProtocolState.Document)
		{
		}

		///<summary>
		///</summary>
		protected Tag() : base("", "", "", ProtocolState.Document)
		{
		}

		///<summary>
		///</summary>
		///<param name="child"></param>
		public void AddChildTag(Tag child)
		{
			if (OwnerDocument == child.OwnerDocument)
				AppendChild(child);
			else if (OwnerDocument != null) AppendChild(OwnerDocument.ImportNode(child, true));
		}

		///<summary>
		///</summary>
		///<param name="name"></param>
		///<typeparam name="T"></typeparam>
		///<returns></returns>
		public T GetEnumAttribute<T>(string name)
		{
			var value = Attributes[name].Value;
			return (T) Enum.Parse(typeof (T), value, true);
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public string GetNextId()
		{
			Interlocked.Increment(ref _packetCounter);
			return "U" + _packetCounter;
		}

		#region << Properties >>

		/// <value>
		/// 
		/// </value>
		public byte[] Bytes
		{
			get { return Convert.FromBase64String(InnerText); }
			set { InnerText = Convert.ToBase64String(value); }
		}

		#endregion
	
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
	}
}