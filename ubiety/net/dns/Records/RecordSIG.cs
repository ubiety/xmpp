using System;

#region Rfc info

/*
 * http://www.ietf.org/rfc/rfc2535.txt
 * 4.1 SIG RDATA Format

   The RDATA portion of a SIG RR is as shown below.  The integrity of
   the RDATA information is protected by the signature field.

						   1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
	   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
	  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
	  |        type covered           |  algorithm    |     labels    |
	  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
	  |                         original TTL                          |
	  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
	  |                      signature expiration                     |
	  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
	  |                      signature inception                      |
	  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
	  |            key  tag           |                               |
	  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+         signer's name         +
	  |                                                               /
	  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-/
	  /                                                               /
	  /                            signature                          /
	  /                                                               /
	  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+


*/

#endregion

namespace ubiety.net.dns.Records
{
	///<summary>
	///</summary>
	public class RecordSIG : Record
	{
		///<summary>
		///</summary>
		public byte Algorithm;
		///<summary>
		///</summary>
		public UInt16 Keytag;
		///<summary>
		///</summary>
		public byte Labels;
		///<summary>
		///</summary>
		public UInt32 OriginalTTL;
		///<summary>
		///</summary>
		public string Signature;
		///<summary>
		///</summary>
		public UInt32 SignatureExpiration;
		///<summary>
		///</summary>
		public UInt32 SignatureInception;
		///<summary>
		///</summary>
		public string SignersName;
		///<summary>
		///</summary>
		public UInt16 TypeCovered;

		///<summary>
		///</summary>
		///<param name="rr"></param>
		public RecordSIG(RecordReader rr)
		{
			TypeCovered = rr.ReadUInt16();
			Algorithm = rr.ReadByte();
			Labels = rr.ReadByte();
			OriginalTTL = rr.ReadUInt32();
			SignatureExpiration = rr.ReadUInt32();
			SignatureInception = rr.ReadUInt32();
			Keytag = rr.ReadUInt16();
			SignersName = rr.ReadDomainName();
			Signature = rr.ReadString();
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} \"{8}\"",
			                     TypeCovered,
			                     Algorithm,
			                     Labels,
			                     OriginalTTL,
			                     SignatureExpiration,
			                     SignatureInception,
			                     Keytag,
			                     SignersName,
			                     Signature);
		}
	}
}