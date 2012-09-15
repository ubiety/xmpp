// XmppTagAttribute.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2009 Dieter Lunn
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

namespace ubiety.attributes
{
	/// <remarks>
	/// Used to denote which classes in an assembly are proper XMPP tags.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class XmppTagAttribute : Attribute
	{
		private readonly string _namespace;
		private readonly string _prefix;
		private readonly Type _type;

		/// <summary>
		/// Creates a new instance of <see cref="XmppTagAttribute"/>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="ns">The namespace the tag applies to</param>
		/// <param name="type">The class-type the tag is for</param>
		public XmppTagAttribute(string name, string ns, Type type)
		{
			_prefix = name;
			_namespace = ns;
			_type = type;
		}

		/// <summary>
		/// The tag namespace prefix
		/// </summary>
		public string Name
		{
			get { return _prefix; }
		}

		/// <summary>
		/// The tags namespace
		/// </summary>
		public string Ns
		{
			get { return _namespace; }
		}

		/// <summary>
		/// The <see cref="Type"/> used to create an instance
		/// </summary>
		public Type ClassType
		{
			get { return _type; }
		}
	}
}