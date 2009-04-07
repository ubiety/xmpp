//XMPP .NET Library Copyright (C) 2009 Dieter Lunn
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
using System.Xml;
using ubiety.common;
using ubiety.core;
using ubiety.core.iq;
using ubiety.logging;

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
				
				if (_current.ID.Resource != null)
				{
					Resource res = (Resource)_reg.GetTag(new XmlQualifiedName("resource", Namespaces.BIND), _current.Document);
					res.InnerText = _current.ID.Resource;
					a.AddChildTag(res);
				}
				
				b.From = _current.ID;
				b.To = _current.ID.Server;
				b.Type = IQType.Set;
				b.Payload = a;
				
				_current.Socket.Write(b);
			}
			else
			{
				Iq iq = data as Iq;
				Bind bind = iq.Payload as Bind;
				_current.ID = bind.XID.XID;
				Logger.InfoFormat(this, "Current XID is now: {0}", _current.ID);
				
				_current.State = new SessionState();
				_current.Execute(null);
			}
		}
	}
}
