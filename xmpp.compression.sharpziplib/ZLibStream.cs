// ZLibStream.cs
//
//XMPP .NET Library Copyright (C) 2008 Dieter Lunn
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
using ICSharpCode.SharpZipLib.Zip.Compression;
using xmpp.attributes;

namespace xmpp.compression.sharpziplib
{
	public class InputBuffer
	{
		private Stream _innerStream;
		private int _rawLength;
		private byte[] _rawData;
		
		private int _clearTextLength;
		private byte[] _clearText;
		
		private int _available;
		
		public InputBuffer(Stream inner) : this(inner, 4096)
		{
		}
		
		public InputBuffer(Stream inner, int buffsize)
		{
			_innerStream = inner;
			
			if ( buffsize < 1024)
				buffsize = 1024;
				
			_rawData = new byte[buffsize];
			_clearText = _rawData;
		}
		
		public void 
	}

	[Compression("zlib", typeof(ZLibStream))]
	public class ZLibStream : Stream
	{

		private Stream _innerStream;
		private const int _buffSize = 4096;
		
		private byte[] _inBuff = new byte[_buffSize];

		public ZLibStream(Stream inner)
		{
			_innerStream = inner;
		}
	}
}
