// BindingState.cs
//
//Ubiety XMPP Library Copyright (C) 2009 - 2015 Dieter Lunn
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

using Ubiety.Common;
using Ubiety.Core;
using Ubiety.Core.Iq;
using Ubiety.Registries;

namespace Ubiety.States
{
	///<summary>
	///</summary>
	public class BindingState : State
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		public override void Execute(Tag data = null)
		{
			if (data == null)
			{
				var a = TagRegistry.GetTag<Bind>("bind", Namespaces.Bind);
				var b = TagRegistry.GetTag<Iq>("iq", Namespaces.Client);

				if (UbietySettings.Id.Resource != null)
				{
					var res = TagRegistry.GetTag<GenericTag>("resource", Namespaces.Bind);
					res.InnerText = UbietySettings.Id.Resource;
					a.AddChildTag(res);
				}

				b.IqType = IqType.Set;
				b.Payload = a;

				ProtocolState.Socket.Write(b);
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
						if (e != null) Errors.SendError(this, ErrorType.XmlError, e.InnerText);
					}
					bind = iq.Payload as Bind;
				}
				if (bind != null) UbietySettings.Id = bind.JidTag.JID;

				ProtocolState.State = new SessionState();
				ProtocolState.State.Execute();
			}
		}
	}
}