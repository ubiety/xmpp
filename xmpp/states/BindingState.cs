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

using System;
using System.Xml;
using ubiety.common;
using ubiety.core;
using ubiety.core.iq;

namespace ubiety.states
{
	public class BindingState : State
	{
		public BindingState() : base()
		{
		}
		
		public override void Execute (Tag data)
		{
			if (data == null)
			{
				Bind a = (Bind)_reg.GetTag(new XmlQualifiedName("bind", Namespaces.BIND), _current.Document);
				Iq b = (Iq)_reg.GetTag(new XmlQualifiedName("iq", Namespaces.CLIENT), _current.Document);
				
				b.Type = IQType.Set;
				b.AddChildTag(a);
				
				_current.Socket.Write(b);
			}
		}
	}
}
