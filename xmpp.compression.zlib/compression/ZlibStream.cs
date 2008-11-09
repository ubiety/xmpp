// ZlibStream.cs
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
using xmpp.attributes;
using xmpp.logging;
using ComponentAce.Compression.Libs.zlib;

namespace xmpp.compression.zlib
{
	[Compression("zlib", typeof(ZlibStream))]
	public class ZlibStream : Stream
	{
		private Stream _innerStream;
		private ZStream _in;
		private ZStream _out;
		
		private int _flush = zlibConst.Z_PARTIAL_FLUSH;
		
		private byte[] _inBuff;
		private byte[] _outBuff;
		
		private const int _buffSize = 4096;
		
		public ZlibStream(Stream innerStream)
		{
			_innerStream = innerStream;
			
			if(_innerStream.CanRead)
			{
				_in = new ZStream();
				int ret = _in.inflateInit();
				if (ret != zlibConst.Z_OK)
					throw new ZStreamException("Unable to initialize inflate");
				_inBuff = new byte[_buffSize];
				_in.avail_in = 0;
				_in.next_in = _inBuff;
				_in.next_in_index = 0;
			}
			
			if(_innerStream.CanWrite)
			{
				_out = new ZStream();
				int ret = _out.deflateInit(zlibConst.Z_DEFAULT_COMPRESSION);
				if (ret != zlibConst.Z_OK)
					throw new ZStreamException("Unable to initialize deflate");
				_outBuff = new byte[_buffSize];
				_out.next_out = _outBuff;
			}
		}
		
		public override bool CanRead {
			get { return _innerStream.CanRead; }
		}
		
		public override bool CanSeek {
			get { return false; }
		}

		public override bool CanWrite {
			get { return _innerStream.CanWrite; }
		}

		public override long Length {
			get { throw new NotImplementedException(); }
		}

		public override long Position {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public override void Flush ()
		{
			_innerStream.Flush();
		}

		public override IAsyncResult BeginRead (byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			if (count <= 0)
				throw new ArgumentException("Cannot read 0 bytes", "count");
				
			_in.next_out = buffer;
			_in.next_out_index = offset;
			_in.avail_out = count;
			if (_in.avail_in == 0)
			{
				_in.next_in_index = 0;
				return _innerStream.BeginRead(_inBuff, 0, _buffSize, cback, state);
			}
			
			ZlibStreamAsyncResult ar = new ZlibStreamAsyncResult(state);
			cback(ar);
			return ar;
		}
		
		public override int EndRead (IAsyncResult async_result)
		{
			if (!(async_result is ZlibStreamAsyncResult))
				_in.avail_in = _innerStream.EndRead(async_result);
			return Inflate();
		}


		public override int Read (byte[] buffer, int offset, int count)
		{
			if (count <= 0)
				return 0;
				
			_in.next_out = buffer;
			_in.next_out_index = offset;
			_in.avail_out = count;
			
			if (_in.avail_in == 0)
			{
				_in.next_in_index = 0;
				_in.avail_in = _innerStream.Read(_inBuff, 0, _buffSize);
				if (_in.avail_in == 0)
					return 0;
			}
			
			return Inflate();
		}
		
		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}
		
		public override void Write (byte[] buffer, int offset, int count)
		{
			if (count <= 0)
				return;
				
			_out.next_in = buffer;
			_out.next_in_index = offset;
			_out.avail_in = count;
			
			while(_out.avail_in > 0)
			{
				_out.next_out_index = 0;
				_out.avail_out = _buffSize;
				int error = _out.deflate(_flush);
				if (error != zlibConst.Z_STREAM_END)
				{
					if (error != zlibConst.Z_OK)
						throw new ZStreamException("Compress Failed: " + _out.msg);
				}
				_innerStream.Write(_outBuff, 0, _buffSize - _out.avail_out);
			}
		}
		
		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}
		
		private int Inflate()
		{
			int count = _in.avail_out;
			int error = _in.inflate(_flush);
			if ((error != zlibConst.Z_OK) && (error != zlibConst.Z_STREAM_END))
			{
				if (error == zlibConst.Z_STREAM_ERROR)
					return 0;
				if (error != zlibConst.Z_OK)
					throw new ZStreamException("Decompress failed: " + _in.msg);
			}
			return (count - _in.avail_out);
		}

		private class ZlibStreamAsyncResult : IAsyncResult
		{
			private object _state = null;
			private Exception _exception;
			
			public ZlibStreamAsyncResult(object state)
			{
				_state = state;
			}
			
			public ZlibStreamAsyncResult(object state, Exception ex)
			{
				_state = state;
				_exception = ex;
			}
			
			public Exception Exception
			{
				get { return _exception; }
			}
			
			public object AsyncState
			{
				get { return _state; }
			}
			
			public System.Threading.WaitHandle AsyncWaitHandle
			{
				get { throw new NotImplementedException(); }
			}
			
			public bool CompletedSynchronously
			{
				get { return true; }
			}
			
			public bool IsCompleted
			{
				get { return true; }
			}
		}
	}
}
