// Register.cs
//
//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 2.1 of the License, or (at your option)
//any later version.
//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//details.
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System.Xml;
using ubiety.common;
using ubiety.attributes;

namespace ubiety.core.iq
{
    /// <summary>
    /// 
    /// </summary>
	[XmppTag("register", Namespaces.REGISTER, typeof(Register))]
	public class Register : ubiety.common.Tag
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
		public Register(XmlDocument doc)
			: base("", new XmlQualifiedName("register", Namespaces.REGISTER), doc)
		{
		}

	}
}
