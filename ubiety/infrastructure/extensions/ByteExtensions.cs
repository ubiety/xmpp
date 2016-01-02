// ByteExtensions.cs
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

using System;
using System.Collections.Generic;

namespace Ubiety.Infrastructure.Extensions
{
    /// <summary>
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        ///     Trims null values from the end of a byte array.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] TrimNull(this IList<byte> message)
        {
            if (message.Count > 1)
            {
                int c = message.Count - 1;
                while (c >= 0 && message[c] == 0x00)
                {
                    c--;
                }

                var r = new byte[(c + 1)];
                for (int i = 0; i < (c + 1); i++)
                {
                    r[i] = message[i];
                }

                return r;
            }

            return null;
        }

        /// <summary>
        ///     Clears a byte array
        /// </summary>
        /// <param name="data"></param>
        public static void Clear(this byte[] data)
        {
            Array.Clear(data, 0, data.Length);
        }
    }
}