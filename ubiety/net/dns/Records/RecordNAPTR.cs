/*
 * http://www.faqs.org/rfcs/rfc2915.html
 * 
 8. DNS Packet Format

		 The packet format for the NAPTR record is:

										  1  1  1  1  1  1
			0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
		  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		  |                     ORDER                     |
		  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		  |                   PREFERENCE                  |
		  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		  /                     FLAGS                     /
		  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		  /                   SERVICES                    /
		  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		  /                    REGEXP                     /
		  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		  /                  REPLACEMENT                  /
		  /                                               /
		  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

	where:

   FLAGS A <character-string> which contains various flags.

   SERVICES A <character-string> which contains protocol and service
	  identifiers.

   REGEXP A <character-string> which contains a regular expression.

   REPLACEMENT A <domain-name> which specifies the new value in the
	  case where the regular expression is a simple replacement
	  operation.

   <character-string> and <domain-name> as used here are defined in
   RFC1035 [1].

 */

namespace ubiety.net.dns.Records
{
	///<summary>
	///</summary>
	public class RecordNAPTR : Record
	{
		///<summary>
		///</summary>
		public string Flags;
		///<summary>
		///</summary>
		public ushort Order;
		///<summary>
		///</summary>
		public ushort Preference;
		///<summary>
		///</summary>
		public string Regexp;
		///<summary>
		///</summary>
		public string Replacement;
		///<summary>
		///</summary>
		public string Services;

		///<summary>
		///</summary>
		///<param name="rr"></param>
		public RecordNAPTR(RecordReader rr)
		{
			Order = rr.ReadUInt16();
			Preference = rr.ReadUInt16();
			Flags = rr.ReadString();
			Services = rr.ReadString();
			Regexp = rr.ReadString();
			Replacement = rr.ReadDomainName();
		}

		public override string ToString()
		{
			return string.Format("{0} {1} \"{2}\" \"{3}\" \"{4}\" {5}",
			                     Order,
			                     Preference,
			                     Flags,
			                     Services,
			                     Regexp,
			                     Replacement);
		}
	}
}