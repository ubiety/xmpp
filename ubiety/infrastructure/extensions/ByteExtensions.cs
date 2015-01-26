using System;
using System.Collections.Generic;

namespace ubiety.common.extensions
{
	public static class ByteExtensions
	{
		/// <summary>
		/// Trims null values from the end of a byte array.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static byte[] TrimNull(this IList<byte> message)
		{
			if (message.Count > 1)
			{
				var c = message.Count - 1;
				while (message[c] == 0x00)
				{
					c--;
				}

				var r = new byte[(c + 1)];
				for (var i = 0; i < (c + 1); i++)
				{
					r[i] = message[i];
				}

				return r;
			}

			return null;
		}

		/// <summary>
		/// Clears a byte array
		/// </summary>
		/// <param name="data"></param>
		public static void Clear(this byte[] data)
		{
			Array.Clear(data, 0, data.Length);
		}
	}
}
