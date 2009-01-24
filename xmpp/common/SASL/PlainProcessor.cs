// PlainProcessor.cs
//
//XMPP .NET Library Copyright (C) 2006, 2007 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Text;
using System.Xml;
using ubiety.core;
using ubiety.core.SASL;
using ubiety.registries;
using ubiety.logging;

namespace ubiety.common.SASL
{
	///<summary>
	///</summary>
	public class PlainProcessor : SASLProcessor
	{
		///<summary>
		///</summary>
		///<param name="tag"></param>
		///<exception cref="NotImplementedException"></exception>
		public override Tag Step(Tag tag)
		{
			throw new NotImplementedException();
		}

		///<summary>
		///</summary>
		///<exception cref="NotImplementedException"></exception>
		public override Tag Initialize(XID id, string password)
		{
			//base.Initialize(id, password);
			
			Logger.Debug(this, "Initializing Plain Processor");
			Logger.DebugFormat(this, "ID User: {0}", id.User);
			Logger.DebugFormat(this, "Password: {0}", password);

			StringBuilder sb = new StringBuilder();

			sb.Append((char) 0);
			sb.Append(id.User);
			sb.Append((char) 0);
			sb.Append(password);

			Auth auth = (Auth)TagRegistry.Instance.GetTag("", new XmlQualifiedName("auth", Namespaces.SASL), new XmlDocument());

			auth.Text = sb.ToString();
			auth.Mechanism = Mechanism.GetMechanism(MechanismType.PLAIN);

			return auth;
		}
	}
}
