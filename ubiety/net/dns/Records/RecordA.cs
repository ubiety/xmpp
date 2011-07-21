/*
 3.4.1. A RDATA format

	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                    ADDRESS                    |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

ADDRESS         A 32 bit Internet address.

Hosts that have multiple Internet addresses will have multiple A
records.
 * 
 */
using System.Net;

namespace ubiety.net.dns.Records
{
	///<summary>
	///</summary>
	public class RecordA : Record
	{
		///<summary>
		///</summary>
		public IPAddress Address;

		///<summary>
		///</summary>
		///<param name="rr"></param>
		public RecordA(RecordReader rr)
		{
			IPAddress.TryParse(string.Format("{0}.{1}.{2}.{3}",
			                                 rr.ReadByte(),
			                                 rr.ReadByte(),
			                                 rr.ReadByte(),
			                                 rr.ReadByte()), out Address);
		}

		public override string ToString()
		{
			return Address.ToString();
		}
	}
}