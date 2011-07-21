using System;
using System.Text;

/*
 * http://tools.ietf.org/rfc/rfc3658.txt
 * 
2.4.  Wire Format of the DS record

   The DS (type=43) record contains these fields: key tag, algorithm,
   digest type, and the digest of a public key KEY record that is
   allowed and/or used to sign the child's apex KEY RRset.  Other keys
   MAY sign the child's apex KEY RRset.

						1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
	0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |           key tag             |  algorithm    |  Digest type  |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                digest  (length depends on type)               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                (SHA-1 digest is 20 bytes)                     |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

 */

namespace ubiety.net.dns.Records
{
	public class RecordDS : Record
	{
		public byte ALGORITHM;
		public byte[] DIGEST;
		public byte DIGESTTYPE;
		public UInt16 KEYTAG;

		public RecordDS(RecordReader rr)
		{
			ushort length = rr.ReadUInt16(-2);
			KEYTAG = rr.ReadUInt16();
			ALGORITHM = rr.ReadByte();
			DIGESTTYPE = rr.ReadByte();
			length -= 4;
			DIGEST = new byte[length];
			DIGEST = rr.ReadBytes(length);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (var t in DIGEST)
				sb.AppendFormat("{0:x2}", t);
			return string.Format("{0} {1} {2} {3}",
			                     KEYTAG,
			                     ALGORITHM,
			                     DIGESTTYPE,
			                     sb);
		}
	}
}