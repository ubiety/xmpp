// Zlib.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2015 Dieter Lunn
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
using ICSharpCode.SharpZipLib.Zip.Compression;
using Ubiety.Infrastructure.Attributes;

namespace Ubiety.Common.Compression
{
    /// <summary>
    /// </summary>
    [Compression("zlib", typeof (Zlib))]
    public class Zlib : ICompression
    {
        private readonly Deflater _deflate;
        private readonly Inflater _inflate;

        /// <summary>
        /// </summary>
        public Zlib()
        {
            _inflate = new Inflater();
            _deflate = new Deflater();
        }

        #region ICompression Members

        /// <summary>
        ///     Called when the stream needs to decompress the incoming data.
        /// </summary>
        /// <param name="data">The data to be decompressed as a byte array.</param>
        /// <returns>A byte array containiong the decompressed data.</returns>
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

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
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