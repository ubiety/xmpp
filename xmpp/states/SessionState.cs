// Ubiety XMPP .NET Library Copyright (C) 2009 Dieter Lunn
// 
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// 
// You should have received a copy of the GNU Lesser General Public License along
// with this library; if not, write to the Free Software Foundation, Inc., 59
// Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System.Xml;
using ubiety.common;
using ubiety.core;
using ubiety.core.iq;

namespace ubiety.states
{
	public class SessionState : State
	{
		public SessionState() : base()
		{
		}
		
		public override void Execute (Tag data)
		{
			if (data == null)
			{
				Iq iq = (Iq)_reg.GetTag(new XmlQualifiedName("iq", Namespaces.CLIENT), _current.Document);
				Session sess = (Session)_reg.GetTag(new XmlQualifiedName("session", Namespaces.SESSION), _current.Document);
				
				iq.Type = IQType.Set;
				iq.AddChildTag(sess);
				
				_current.Socket.Write(iq);
			}
		}
	}
}
