using System;

/*
 * http://tools.ietf.org/rfc/rfc2930.txt
 * 
2. The TKEY Resource Record

   The TKEY resource record (RR) has the structure given below.  Its RR
   type code is 249.

	  Field       Type         Comment
	  -----       ----         -------
	   Algorithm:   domain
	   Inception:   u_int32_t
	   Expiration:  u_int32_t
	   Mode:        u_int16_t
	   Error:       u_int16_t
	   Key Size:    u_int16_t
	   Key Data:    octet-stream
	   Other Size:  u_int16_t
	   Other Data:  octet-stream  undefined by this specification

 */

namespace ubiety.net.dns.Records
{
	///<summary>
	///</summary>
	public class RecordTKEY : Record
	{
		///<summary>
		///</summary>
		public string Algorithm;

		///<summary>
		///</summary>
		public UInt16 Error;

		///<summary>
		///</summary>
		public UInt32 Expiration;

		///<summary>
		///</summary>
		public UInt32 Inception;

		///<summary>
		///</summary>
		public byte[] Keydata;

		///<summary>
		///</summary>
		public UInt16 Keysize;

		///<summary>
		///</summary>
		public UInt16 Mode;

		///<summary>
		///</summary>
		public byte[] Otherdata;

		///<summary>
		///</summary>
		public UInt16 Othersize;

		///<summary>
		///</summary>
		///<param name="rr"></param>
		public RecordTKEY(RecordReader rr)
		{
			Algorithm = rr.ReadDomainName();
			Inception = rr.ReadUInt32();
			Expiration = rr.ReadUInt32();
			Mode = rr.ReadUInt16();
			Error = rr.ReadUInt16();
			Keysize = rr.ReadUInt16();
			Keydata = rr.ReadBytes(Keysize);
			Othersize = rr.ReadUInt16();
			Otherdata = rr.ReadBytes(Othersize);
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4}",
			                     Algorithm,
			                     Inception,
			                     Expiration,
			                     Mode,
			                     Error);
		}
	}
}