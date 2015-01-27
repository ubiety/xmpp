// ConnectedState.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2015 Dieter Lunn
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
using Ubiety.Registries;

namespace Ubiety.States
{
	/// <summary>
	/// The state which occurs just after connecting and sends the starting stream:stream tag.
	/// </summary>
	public class ConnectedState : State
	{
		/// <summary>
		/// Executes the state sending the tag to the just connected socket.
		/// </summary>
		/// <param name="data">
		/// The <see cref="Ubiety.Common.Tag"/> to parse.  In this case null.
		/// </param>
		public override void Execute(Tag data = null)
		{
			var stream = TagRegistry.GetTag<Stream>("stream", Namespaces.Stream);
			stream.Version = "1.0";
			stream.To = UbietySettings.Id.Server;
			stream.Namespace = Namespaces.Client;
			stream.Language = "en";
			ProtocolState.Socket.Write("<?xml version='1.0' encoding='UTF-8'?>" + stream.StartTag());
			ProtocolState.State = new ServerFeaturesState();
		}
	}
}