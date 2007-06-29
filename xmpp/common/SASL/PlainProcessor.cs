/**********************************************************************************/
/*																				  */
/* XMPP .NET Library Copyright (C) 2006 Dieter Lunn								  */
/*														                          */
/* This library is free software; you can redistribute it and/or modify it under  */
/* the terms of the GNU Lesser General Public License as published by the Free	  */
/* Software Foundation; either version 2.1 of the License, or (at your option)	  */
/* any later version.															  */
/*														                          */
/* This library is distributed in the hope that it will be useful, but WITHOUT	  */
/* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS  */
/* FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more	  */
/* details.																		  */
/*														                          */
/* You should have received a copy of the GNU Lesser General Public License along */
/* with this library; if not, write to the Free Software Foundation, Inc., 59	  */
/* Temple Place, Suite 330, Boston, MA 02111-1307 USA							  */
/**********************************************************************************/

using System;
using System.Text;
using System.Xml;
using xmpp.core;
using xmpp.core.SASL;

namespace xmpp.common.SASL
{
	///<summary>
	///</summary>
	public class PlainProcessor : SASLProcessor
	{
		///<summary>
		///</summary>
		///<param name="tag"></param>
		///<exception cref="NotImplementedException"></exception>
		public override void Step(Tag tag)
		{
			throw new NotImplementedException();
		}

		///<summary>
		///</summary>
		///<exception cref="NotImplementedException"></exception>
		public override Tag Initialize(XID id, string password)
		{
			base.Initialize(id, password);

			StringBuilder sb = new StringBuilder();

			sb.Append((char) 0);
			sb.Append(_id.User);
			sb.Append((char) 0);
			sb.Append(_password);

			Auth auth = (Auth)TagRegistry.Instance.GetTag("", new XmlQualifiedName("auth", Namespaces.SASL), new XmlDocument());

			auth.Text = sb.ToString();
			auth.Mechanism = Mechanism.GetMechanism(MechanismType.PLAIN);

			return auth;
		}
	}
}
