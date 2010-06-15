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
// 

using System;
using System.IO;
using Ionic.Zlib;

using ubiety.attributes;

namespace ubiety.compression.dotnetzip
{
	[Compression("zlib", typeof(ZlibStream))]
	public class ZlibStream : Stream
	{
		private Stream _inner;
		private ZlibCodec _in;
		private ZlibCodec _out;
		
		private byte[] _inBuff;
		private byte[] _outBuff;
		
		private const int _bufferSize = 1024;
		
		public ZlibStream(Stream innerStream)
		{
			_inner = innerStream;
			
			if (_inner.CanRead)
			{
				_in = new ZlibCodec(CompressionMode.Decompress);
                //int ret = _in.InitializeInflate();
                //if (ret != ZlibConstants.Z_OK)
                //    throw new ZlibException("Unable to initialize Inflate");
				_inBuff = new byte[_bufferSize];
				_in.AvailableBytesIn = 0;
				_in.InputBuffer = _inBuff;
				_in.NextIn = 0;
			}
			
			if (_inner.CanWrite)
			{
				_out = new ZlibCodec(CompressionMode.Compress);
				int ret = _out.InitializeDeflate(CompressionLevel.Default);
				if (ret != ZlibConstants.Z_OK)
					throw new ZlibException("Unable to initialize Deflate");
				_outBuff = new byte[_bufferSize];
				_out.OutputBuffer = _outBuff;
			}
		}
		
		public override bool CanRead {
			get {
				return _inner.CanRead;
			}
		}

		public override bool CanWrite {
			get {
				return _inner.CanWrite;
			}
		}
		
		public override void Flush ()
		{
			_inner.Flush();
		}

		#region << Not Implemented Overrides >>
		public override bool CanSeek {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public override long Length {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public override long Position {
			get {
				throw new System.NotImplementedException ();
			}
			set {
				throw new System.NotImplementedException ();
			}
		}

		public override long Seek (long offset, System.IO.SeekOrigin origin)
		{
			throw new System.NotImplementedException ();
		}

		public override void SetLength (long value)
		{
			throw new System.NotImplementedException ();
		}
		#endregion
		
		public override IAsyncResult BeginRead (byte[] buffer, int offset, int count, System.AsyncCallback callback, object state)
		{
			if (count <= 0)
				throw new ArgumentOutOfRangeException("count", "Can't read zero or less bytes");
			
			_in.OutputBuffer = buffer;
			_in.NextOut = offset;
			_in.AvailableBytesOut = count;
			if (_in.AvailableBytesIn == 0)
			{
				_in.NextIn = 0;
				return _inner.BeginRead(_inBuff, 0, _bufferSize, callback, state);
			}
			ZlibStreamAsyncResult ar = new ZlibStreamAsyncResult(state);
			callback(ar);
			return ar;
		}
		
		public override int EndRead (System.IAsyncResult asyncResult)
		{
			if (!(asyncResult is ZlibStreamAsyncResult))
				_in.AvailableBytesIn = _inner.EndRead(asyncResult);
			return Inflate();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			if (count <= 0)
				return 0;
			
			_in.OutputBuffer = buffer;
			_in.NextOut = offset;
			_in.AvailableBytesOut = count;
			
			if (_in.AvailableBytesIn == 0)
			{
				_in.NextIn = 0;
				_in.AvailableBytesIn = _inner.Read(_inBuff, 0, _bufferSize);
				if (_in.AvailableBytesIn == 0)
					return 0;
			}
			
			return Inflate();
		}
		
		private int Inflate()
		{
			int count = _in.AvailableBytesOut;
			int error = _in.Inflate(FlushType.None);
			if ((error != ZlibConstants.Z_OK) && (error != ZlibConstants.Z_STREAM_END))
			{
				if (error == ZlibConstants.Z_STREAM_ERROR)
					return 0;
				if (error != ZlibConstants.Z_OK)
					throw new ZlibException("Unable to inflate data: " + _in.Message);
			}
			return (count - _in.AvailableBytesOut);
		}
		
		public override void Write (byte[] buffer, int offset, int count)
		{
			if (count <= 0)
				return;
			
			_out.InputBuffer = buffer;
			_out.NextIn = offset;
			_out.AvailableBytesIn = count;
			
			while (_out.AvailableBytesIn > 0)
			{
				_out.NextOut = 0;
				_out.AvailableBytesOut = _bufferSize;
                //int error = _out.Deflate(ZlibConstants.Z_PARTIAL_FLUSH);
                //if (error != ZlibConstants.Z_STREAM_END)
                //{
                //    if (error != ZlibConstants.Z_OK)
                //        throw new ZlibException("Unable to deflate data: " + _out.Message);
                //}

                while (_out.TotalBytesOut < _bufferSize)
                {
                    _out.Deflate(FlushType.None);
                }

                while (true)
                {
                    int rc = _out.Deflate(FlushType.Finish);
                    if (rc == ZlibConstants.Z_STREAM_END) break;
                }

                _out.EndDeflate();
				
				_inner.Write(_outBuff, 0, _bufferSize - _out.AvailableBytesOut);
			}
		}

		#region << ZlibStreamAsyncResult >>
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
		#endregion
	}
}
