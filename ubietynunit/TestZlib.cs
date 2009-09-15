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
using System.IO;
using NUnit.Core;
using NUnit.Framework;
using ubiety.compression.zlib;

namespace ubietynunit
{
	[TestFixture]
	public class TestZlib
	{
		private static readonly System.Text.Encoding _enc = System.Text.Encoding.UTF8;
		
		private const string _hello = "Hello, world";
		private static readonly byte[] _hello_bytes = new byte[]
		{
			120, 156, 243, 72, 205, 201, 201, 215, 81, 40, 207, 47, 202, 73, 1, 0, 27, 212, 4, 105
		};
		
		[Test]
		public void Read()
		{
			MemoryStream mem = new MemoryStream();
			mem.Write(_hello_bytes, 0, _hello_bytes.Length);
			mem.Seek(0, SeekOrigin.Begin);
			ZlibStream z = new ZlibStream(mem);
			byte[] buff = new byte[1024];
			int len = z.Read(buff, 0, buff.Length);
			Assert.That(len, Is.GreaterThan(0));
			string final = _enc.GetString(buff, 0, len);
			Assert.That(final, Is.EqualTo(_hello));
		}
		
		[Test]
		public void Write()
		{
			byte[] buff = _enc.GetBytes(_hello);
			MemoryStream mem = new MemoryStream();
			ZlibStream z = new ZlibStream(mem);
			z.Write(buff, 0, buff.Length);
			mem.Seek(0, SeekOrigin.Begin);
			byte[] result = mem.ToArray();
			Assert.That(result.Length, Is.EqualTo(_hello_bytes.Length));
			int count = 0;
			foreach (byte b in result)
			{
				Assert.That(b, Is.EqualTo(_hello_bytes[count++]));
			}
		}
	}
}
