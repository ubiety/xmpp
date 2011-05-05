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

using System.IO;
using NUnit.Framework;
using ubiety.compression.sharpziplib;

namespace ubietynunit
{
	[TestFixture]
	public class TestSharpZipLib
	{
		private static readonly System.Text.Encoding Enc = System.Text.Encoding.UTF8;
		
		private const string Hello = "Hello, world";
		private static readonly byte[] HelloBytes = new byte[]
		{
			120, 156, 243, 72, 205, 201, 201, 215, 81, 40, 207, 47, 202, 73, 1, 0, 27, 212, 4, 105
		};
		
		[Test]
		public void Read()
		{
			var mem = new MemoryStream();
			mem.Write(HelloBytes, 0, HelloBytes.Length);
			mem.Seek(0, SeekOrigin.Begin);
			var z = new ZlibStream(mem);
			var buff = new byte[1024];
			var len = z.Read(buff, 0, buff.Length);
			Assert.That(len, Is.GreaterThan(0));
			var final = Enc.GetString(buff, 0, len);
			Assert.That(final, Is.EqualTo(Hello));
		}
		
		[Test]
		public void Write()
		{
			var buff = Enc.GetBytes(Hello);
			var mem = new MemoryStream();
			var z = new ZlibStream(mem);
			z.Write(buff, 0, buff.Length);
			mem.Seek(0, SeekOrigin.Begin);
			var result = mem.ToArray();
			Assert.That(result.Length, Is.EqualTo(HelloBytes.Length));
			var count = 0;
			foreach (var b in result)
			{
				Assert.That(b, Is.EqualTo(HelloBytes[count++]));
			}
		}
	}
}
