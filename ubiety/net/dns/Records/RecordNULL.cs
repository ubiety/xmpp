/*
3.3.10. NULL RDATA format (EXPERIMENTAL)

	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	/                  <anything>                   /
	/                                               /
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

Anything at all may be in the RDATA field so long as it is 65535 octets
or less.

NULL records cause no additional section processing.  NULL RRs are not
allowed in master files.  NULLs are used as placeholders in some
experimental extensions of the DNS.
*/
namespace ubiety.net.dns.Records
{
	///<summary>
	///</summary>
	public class RecordNULL : Record
	{
		///<summary>
		///</summary>
		public byte[] Anything;

		///<summary>
		///</summary>
		///<param name="rr"></param>
		public RecordNULL(RecordReader rr)
		{
			rr.Position -= 2;
			// re-read length
			var rdlength = rr.ReadUInt16();
			Anything = new byte[rdlength];
			Anything = rr.ReadBytes(rdlength);
		}

		public override string ToString()
		{
			return string.Format("...binary data... ({0}) bytes", Anything.Length);
		}
	}
}