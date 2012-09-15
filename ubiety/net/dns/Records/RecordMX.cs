using System;

namespace ubiety.net.dns.Records
{
	/*
	3.3.9. MX RDATA format

		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                  PREFERENCE                   |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		/                   EXCHANGE                    /
		/                                               /
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

	where:

	PREFERENCE      A 16 bit integer which specifies the preference given to
					this RR among others at the same owner.  Lower values
					are preferred.

	EXCHANGE        A <domain-name> which specifies a host willing to act as
					a mail exchange for the owner name.

	MX records cause type A additional section processing for the host
	specified by EXCHANGE.  The use of MX RRs is explained in detail in
	[RFC-974].
	*/

	///<summary>
	///</summary>
	public class RecordMX : Record, IComparable
	{
		///<summary>
		///</summary>
		public string Exchange;
		///<summary>
		///</summary>
		public ushort Preference;

		///<summary>
		///</summary>
		///<param name="rr"></param>
		public RecordMX(RecordReader rr)
		{
			Preference = rr.ReadUInt16();
			Exchange = rr.ReadDomainName();
		}

		#region IComparable Members

		public int CompareTo(object objA)
		{
			var recordMX = objA as RecordMX;
			if (recordMX == null)
				return -1;
			if (Preference > recordMX.Preference)
				return 1;
			if (Preference < recordMX.Preference)
				return -1;
			return string.Compare(Exchange, recordMX.Exchange, true);
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0} {1}", Preference, Exchange);
		}
	}
}