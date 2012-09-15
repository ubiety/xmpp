using System.IO;
using ubiety.common;
using ubiety.common.attributes;
using ubiety.compression.sharpziplib.Zip.Compression;

namespace ubiety.compression.sharpziplib
{
	[Compression("zlib", typeof (Zlib))]
	public class Zlib : ICompression
	{
		private readonly Deflater _deflate;
		private readonly Inflater _inflate;

		public Zlib()
		{
			_inflate = new Inflater();
			_deflate = new Deflater();
		}

		#region ICompression Members

		///<summary>
		/// Called when the stream needs to decompress the incoming data.
		///</summary>
		///<param name="data">The data to be decompressed as a byte array.</param>
		///<returns>A byte array containiong the decompressed data.</returns>
		public byte[] Deflate(byte[] data)
		{
			int ret;

			_deflate.SetInput(data);
			_deflate.Flush();

			var ms = new MemoryStream();
			do
			{
				var buf = new byte[4096];
				ret = _deflate.Deflate(buf);
				if (ret > 0)
					ms.Write(buf, 0, ret);
			} while (ret > 0);

			return ms.ToArray();
		}

		///<summary>
		///</summary>
		///<param name="data"></param>
		///<param name="length"></param>
		///<returns></returns>
		public byte[] Inflate(byte[] data, int length)
		{
			int ret;

			_inflate.SetInput(data, 0, length);

			var ms = new MemoryStream();
			do
			{
				var buffer = new byte[4096];
				ret = _inflate.Inflate(buffer);
				if (ret > 0)
					ms.Write(buffer, 0, ret);
			} while (ret > 0);

			return ms.ToArray();
		}

		#endregion
	}
}