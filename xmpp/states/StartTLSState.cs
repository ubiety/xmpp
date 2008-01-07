//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
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
using xmpp;
using xmpp.core;
using xmpp.registries;
using xmpp.common;

namespace xmpp.states
{
	/// <summary>
	/// 
	/// </summary>
	public class StartTLSState : State
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state">
		/// A <see cref="ProtocolState"/>
		/// </param>
		public StartTLSState(ProtocolState state)
		{
			current = state;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Object"/>
		/// </param>
		public override void Execute (xmpp.common.Tag data)
		{
			TagRegistry reg = TagRegistry.Instance;
			
			if (data == "proceed")
			{
				current.Socket.StartSecure();
				Stream stream = (Stream)reg.GetTag("stream", new XmlQualifiedName("stream", Namespaces.STREAM), new XmlDocument());
				stream.Version = "1.0";
				stream.To = current.Socket.Hostname;
				stream.NS = "jabber:client";
				current.Socket.Write(stream.StartTag());
			}
		}

	}
}
