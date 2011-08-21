// BindingState.cs
//
//Ubiety XMPP Library Copyright (C) 2009 Dieter Lunn
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

using ubiety.common;
using ubiety.core;
using ubiety.core.iq;
using ubiety.logging;

namespace ubiety.states
{
	///<summary>
	///</summary>
	public class BindingState : State
	{
		public override void Execute(Tag data)
		{
			if (data == null)
			{
				var a = (Bind) Reg.GetTag("bind", Namespaces.Bind, Current.Document);
				var b = (Iq) Reg.GetTag("iq", Namespaces.Client, Current.Document);

				if (UbietySettings.Id.Resource != null)
				{
					var res = Reg.GetTag("resource", Namespaces.Bind, Current.Document);
					res.InnerText = UbietySettings.Id.Resource;
					a.AddChildTag(res);
				}

				b.IqType = IqType.Set;
				b.Payload = a;

				Current.Socket.Write(b);
			}
			else
			{
				var iq = data as Iq;
				Bind bind = null;
				if (iq != null)
				{
					if (iq.IqType == IqType.Error)
					{
						var e = iq["error"];
						if (e != null) Errors.Instance.SendError(this, ErrorType.XMLError, e.InnerText);
					}
					bind = iq.Payload as Bind;
				}
				if (bind != null) UbietySettings.Id = bind.XID.XID;
				Logger.InfoFormat(this, "Current XID is now: {0}", UbietySettings.Id);

				Current.State = new SessionState();
				Current.State.Execute(null);
			}
		}
	}
}