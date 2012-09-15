/*
3.3.6. MG RDATA format (EXPERIMENTAL)

	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	/                   MGMNAME                     /
	/                                               /
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

MGMNAME         A <domain-name> which specifies a mailbox which is a
				member of the mail group specified by the domain name.

MG records cause no additional section processing.
*/

namespace ubiety.net.dns.Records
{
	///<summary>
	///</summary>
	public class RecordMG : Record
	{
		///<summary>
		///</summary>
		public string MgmName;

		///<summary>
		///</summary>
		///<param name="rr"></param>
		public RecordMG(RecordReader rr)
		{
			MgmName = rr.ReadDomainName();
		}

		public override string ToString()
		{
			return MgmName;
		}
	}
}