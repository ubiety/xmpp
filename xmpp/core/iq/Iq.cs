//  
//  Copyright (C) 2009 Dieter Lunn
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System.Xml;
using ubiety.common;
using ubiety.attributes;

namespace ubiety.core.iq
{
	public enum IQType
	{
		Get,
		Set,
		Error,
		Result
	}

	[XmppTag("iq", Namespaces.CLIENT, typeof(Iq))]
	public class Iq : Tag
	{
		public Iq(XmlDocument doc) : base("", new XmlQualifiedName("iq", Namespaces.CLIENT), doc)
		{
			ID = GetNextID();
		}
		
		public IQType Type
		{
			get { return GetEnumAttribute<IQType>("type"); }
			set { SetAttribute("type", value.ToString().ToLower()); }
		}
	}
}
