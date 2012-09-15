namespace ubiety.net.dns.Records.NotUsed
{
	public class RecordUnknown : Record
	{
		public byte[] RDATA;
		public RecordUnknown(RecordReader rr)
		{
			// re-read length
			ushort RDLENGTH = rr.ReadUInt16(-2);
			RDATA = rr.ReadBytes(RDLENGTH);
		}
	}
}
