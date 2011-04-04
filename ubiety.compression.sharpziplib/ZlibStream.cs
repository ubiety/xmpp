// ZLibStream.cs
//
//XMPP .NET Library Copyright (C) 2008, 2011 Dieter Lunn
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
using ubiety.attributes;
using ubiety.common;

namespace ubiety.compression.sharpziplib
{
	[Compression("zlib", typeof(ZlibStream))]
	public class ZlibStream : Stream
	{
		private Stream _innerStream;
		private Inflater _in;
		private Deflater _out;
		private byte[] _inBuff;
		private byte[] _outBuff;

		public ZlibStream(Stream inner, Inflater inflater, int buffSize)
		{
			_innerStream = inner;
			_in = inflater;
			_inBuff = new byte[buffSize];
			_outBuff = _inBuff;
			_out = new Deflater();
		}
		
		public ZlibStream(Stream inner, Inflater inflater) : this(inner, inflater, 4096)
		{
		}
		
		public ZlibStream(Stream inner) : this(inner, new Inflater())
		{
		}
		
		public override bool CanRead {
			get { return _innerStream.CanRead; }
		}

		public override bool CanWrite {
			get { return _innerStream.CanWrite; }
		}
		
		public override bool CanSeek {
			get { return false; }
		}
		
		public override long Length {
			get { return _inBuff.Length; }
		}
		
		public override long Position {
			get { return _innerStream.Position; }
			set { throw new NotSupportedException(); }
		}
		
		public override void Flush ()
		{
			_innerStream.Flush();
		}
		
		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}

		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			_out.SetInput(buffer, offset, count);
			_out.Flush();
			
			while (!_out.IsNeedingInput)
			{
				int avail = _out.Deflate(_outBuff, 0, _outBuff.Length);
				_innerStream.Write(_outBuff, 0, avail);
			}
		}

		public override void WriteByte (byte value)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite (byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			throw new NotSupportedException();
		}

		public override void Close ()
		{
			base.Close ();
		}
		
		public override int Read (byte[] buffer, int offset, int count)
		{
			if (count <= 0)
				return 0;

			int r = _innerStream.Read(_inBuff, 0, _inBuff.Length);
			try
			{
				buffer = Inflate(_inBuff, r);
			}
			catch (Exception e)
			{
				Errors.Instance.SendError(this, ErrorType.CompressionFailed, e.Message);
			}

			return r;
		}

		public override IAsyncResult BeginRead (byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			_outBuff = buffer;
			if ( _in.IsNeedingInput )
				return _innerStream.BeginRead(_inBuff, 0, _inBuff.Length, cback, state);
				
			ZlibStreamAsyncResult ar = new ZlibStreamAsyncResult(state);
			cback(ar);
			return ar;
		}

		public override int EndRead (IAsyncResult async_result)
		{
			int avail = 0;
			
			if ( !(async_result is ZlibStreamAsyncResult) )
			{
				avail = _innerStream.EndRead(async_result);
			}
			
			//Logger.Debug(this, _inBuff);
			
			try
			{
				_outBuff = Inflate(_inBuff, avail);
			}
			catch (Exception e)
			{
				Errors.Instance.SendError(this, ErrorType.CompressionFailed, e.Message);
			}
			
			//Logger.Debug(this, _outBuff);
			
			return avail;
		}

		private byte[] Inflate(byte[] data, int length)
		{
			int ret;

			_in.SetInput(data, 0, length);

			MemoryStream ms = new MemoryStream();
			do
			{
				byte[] buffer = new byte[_outBuff.Length];
				ret = _in.Inflate(buffer);
				if (ret > 0)
					ms.Write(buffer, 0, ret);
			} while (ret > 0);

			return ms.ToArray();
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
