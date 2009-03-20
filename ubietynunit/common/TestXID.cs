// TestXID.cs
//
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
using NUnit.Core;
using NUnit.Framework;
//using NUnit.Framework.SyntaxHelpers;
using ubiety.common;

namespace ubietynunit
{
	[TestFixture]
	public class TestXID
	{
		[Test]
		[Ignore("String constructor deprecated because it doesn't work.  Will reenable when working.")]
		public void EscapeUsernameFromString()
		{
			XID id = new XID("d'artangan@garcon.fr/testing");
			
			Assert.That(id.ToString(), Is.EqualTo(@"d\27artangan@garcon.fr/testing"));
		}
		
		[Test]
		public void EscapeUsernameFromParts()
		{
			XID id = new XID("d'artangan", "garcon.fr", "testing");
			
			Assert.That(id.ToString(), Is.EqualTo(@"d\27artangan@garcon.fr/testing"));
		}
		
		[Test]
		public void NewXIDFromString()
		{
			XID id = new XID("testing@jabber.org/home");
			
			Assert.That(id.User, Is.EqualTo("testing"));
			Assert.That(id.Server, Is.EqualTo("jabber.org"));
			Assert.That(id.Resource, Is.EqualTo("home"));
		}
	}
}
