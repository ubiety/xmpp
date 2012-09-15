using System;

/*
 * http://www.ietf.org/rfc/rfc2845.txt
 * 
 * Field Name       Data Type      Notes
	  --------------------------------------------------------------
	  Algorithm Name   domain-name    Name of the algorithm
									  in domain name syntax.
	  Time Signed      u_int48_t      seconds since 1-Jan-70 UTC.
	  Fudge            u_int16_t      seconds of error permitted
									  in Time Signed.
	  MAC Size         u_int16_t      number of octets in MAC.
	  MAC              octet stream   defined by Algorithm Name.
	  Original ID      u_int16_t      original message ID
	  Error            u_int16_t      expanded RCODE covering
									  TSIG processing.
	  Other Len        u_int16_t      length, in octets, of
									  Other Data.
	  Other Data       octet stream   empty unless Error == BADTIME

 */

namespace ubiety.net.dns.Records
{
	///<summary>
	///</summary>
	public class RecordTSIG : Record
	{
		///<summary>
		///</summary>
		public string AlgorithmName;

		///<summary>
		///</summary>
		public UInt16 Error;

		///<summary>
		///</summary>
		public UInt16 Fudge;

		///<summary>
		///</summary>
		public byte[] Mac;

		///<summary>
		///</summary>
		public UInt16 MacSize;

		///<summary>
		///</summary>
		public UInt16 OriginalId;

		///<summary>
		///</summary>
		public byte[] OtherData;

		///<summary>
		///</summary>
		public UInt16 OtherLen;

		///<summary>
		///</summary>
		public long TimeSigned;

		///<summary>
		///</summary>
		///<param name="rr"></param>
		public RecordTSIG(RecordReader rr)
		{
			AlgorithmName = rr.ReadDomainName();
			TimeSigned = rr.ReadUInt32() << 32 | rr.ReadUInt32();
			Fudge = rr.ReadUInt16();
			MacSize = rr.ReadUInt16();
			Mac = rr.ReadBytes(MacSize);
			OriginalId = rr.ReadUInt16();
			Error = rr.ReadUInt16();
			OtherLen = rr.ReadUInt16();
			OtherData = rr.ReadBytes(OtherLen);
		}

		public override string ToString()
		{
			var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			dateTime = dateTime.AddSeconds(TimeSigned);
			var printDate = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
			return string.Format("{0} {1} {2} {3} {4}",
			                     AlgorithmName,
			                     printDate,
			                     Fudge,
			                     OriginalId,
			                     Error);
		}
	}
}