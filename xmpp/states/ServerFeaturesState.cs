//XMPP .NET Library Copyright (C) 2006, 2007 Dieter Lunn
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
using xmpp.common.SASL;

namespace xmpp.states
{
	/// <summary>
	/// 
	/// </summary>
	public class ServerFeaturesState : State
	{
		private TagRegistry _reg = TagRegistry.Instance;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state">
		/// A <see cref="ProtocolState"/>
		/// </param>
		public ServerFeaturesState(ProtocolState state)
		{
			current = state;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Object"/>
		/// </param>
		public override void Execute (object data)
		{
			TagEventArgs e = data as TagEventArgs;
			Features f = e.Tag as Features;
			
			if (f == null)
				throw new Exception("Expecting stream:features from 1.x server");
			
			if (f.StartTLS != null && current.Socket.SSL)
			{
				current.State = new StartTLSState(current);
				StartTLS tls = (StartTLS)_reg.GetTag("", new XmlQualifiedName("starttls", xmpp.common.Namespaces.START_TLS), new XmlDocument());
				current.Socket.Write(tls);
			}
			
			current.Processor = SASLProcessor.CreateProcessor(f.StartSASL.SupportedTypes);
			current.Socket.Write(current.Processor.Initialize(current.ID, current.Password));
		}
	}
}
