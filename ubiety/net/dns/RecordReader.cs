using System;
using System.Collections.Generic;
using System.Text;
using ubiety.net.dns.Records;
using ubiety.net.dns.Records.NotUsed;
using ubiety.net.dns.Records.Obsolete;

namespace ubiety.net.dns
{
	///<summary>
	///</summary>
	public class RecordReader
	{
		private readonly byte[] _mData;
		private int _mPosition;

		///<summary>
		///</summary>
		///<param name="data"></param>
		public RecordReader(byte[] data)
		{
			_mData = data;
			_mPosition = 0;
		}

		///<summary>
		///</summary>
		///<param name="data"></param>
		///<param name="position"></param>
		public RecordReader(byte[] data, int position)
		{
			_mData = data;
			_mPosition = position;
		}

		///<summary>
		///</summary>
		public int Position
		{
			get { return _mPosition; }
			set { _mPosition = value; }
		}


		///<summary>
		///</summary>
		///<returns></returns>
		public byte ReadByte()
		{
			return (byte) (_mPosition >= _mData.Length ? 0 : _mData[_mPosition++]);
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public char ReadChar()
		{
			return (char) ReadByte();
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public UInt16 ReadUInt16()
		{
			return (UInt16) (ReadByte() << 8 | ReadByte());
		}

		///<summary>
		///</summary>
		///<param name="offset"></param>
		///<returns></returns>
		public UInt16 ReadUInt16(int offset)
		{
			_mPosition += offset;
			return ReadUInt16();
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public UInt32 ReadUInt32()
		{
			return (UInt32) (ReadUInt16() << 16 | ReadUInt16());
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public string ReadDomainName()
		{
			var name = new StringBuilder();
			int length;

			// get  the length of the first label
			while ((length = ReadByte()) != 0)
			{
				// top 2 bits set denotes domain name compression and to reference elsewhere
				if ((length & 0xc0) == 0xc0)
				{
					// work out the existing domain name, copy this pointer
					var newRecordReader = new RecordReader(_mData, (length & 0x3f) << 8 | ReadByte());

					name.Append(newRecordReader.ReadDomainName());
					return name.ToString();
				}

				// if not using compression, copy a char at a time to the domain name
				while (length > 0)
				{
					name.Append(ReadChar());
					length--;
				}
				name.Append('.');
			}
			return name.Length == 0 ? "." : name.ToString();
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public string ReadString()
		{
			short length = ReadByte();

			var name = new StringBuilder();
			for (var intI = 0; intI < length; intI++)
				name.Append(ReadChar());
			return name.ToString();
		}

		///<summary>
		///</summary>
		///<param name="intLength"></param>
		///<returns></returns>
		public byte[] ReadBytes(int intLength)
		{
			var list = new List<byte>();
			for (var intI = 0; intI < intLength; intI++)
				list.Add(ReadByte());
			return list.ToArray();
		}

		///<summary>
		///</summary>
		///<param name="type"></param>
		///<returns></returns>
		public Record ReadRecord(Type type)
		{
			switch (type)
			{
				case Type.A:
					return new RecordA(this);
				case Type.NS:
					return new RecordNS(this);
				case Type.MD:
					return new RecordMD(this);
				case Type.MF:
					return new RecordMF(this);
				case Type.CNAME:
					return new RecordCNAME(this);
				case Type.SOA:
					return new RecordSOA(this);
				case Type.MB:
					return new RecordMB(this);
				case Type.MG:
					return new RecordMG(this);
				case Type.MR:
					return new RecordMR(this);
				case Type.NULL:
					return new RecordNULL(this);
				case Type.WKS:
					return new RecordWKS(this);
				case Type.PTR:
					return new RecordPTR(this);
				case Type.HINFO:
					return new RecordHINFO(this);
				case Type.MINFO:
					return new RecordMINFO(this);
				case Type.MX:
					return new RecordMX(this);
				case Type.TXT:
					return new RecordTXT(this);
				case Type.RP:
					return new RecordRP(this);
				case Type.AFSDB:
					return new RecordAFSDB(this);
				case Type.X25:
					return new RecordX25(this);
				case Type.ISDN:
					return new RecordISDN(this);
				case Type.RT:
					return new RecordRT(this);
				case Type.NSAP:
					return new RecordNSAP(this);
				case Type.NSAPPTR:
					return new RecordNSAPPTR(this);
				case Type.SIG:
					return new RecordSIG(this);
				case Type.KEY:
					return new RecordKEY(this);
				case Type.PX:
					return new RecordPX(this);
				case Type.GPOS:
					return new RecordGPOS(this);
				case Type.AAAA:
					return new RecordAAAA(this);
				case Type.LOC:
					return new RecordLOC(this);
				case Type.NXT:
					return new RecordNXT(this);
				case Type.EID:
					return new RecordEID(this);
				case Type.NIMLOC:
					return new RecordNIMLOC(this);
				case Type.SRV:
					return new RecordSRV(this);
				case Type.ATMA:
					return new RecordATMA(this);
				case Type.NAPTR:
					return new RecordNAPTR(this);
				case Type.KX:
					return new RecordKX(this);
				case Type.CERT:
					return new RecordCERT(this);
				case Type.A6:
					return new RecordA6(this);
				case Type.DNAME:
					return new RecordDNAME(this);
				case Type.SINK:
					return new RecordSINK(this);
				case Type.OPT:
					return new RecordOPT(this);
				case Type.APL:
					return new RecordAPL(this);
				case Type.DS:
					return new RecordDS(this);
				case Type.SSHFP:
					return new RecordSSHFP(this);
				case Type.IPSECKEY:
					return new RecordIPSECKEY(this);
				case Type.RRSIG:
					return new RecordRRSIG(this);
				case Type.NSEC:
					return new RecordNSEC(this);
				case Type.DNSKEY:
					return new RecordDNSKEY(this);
				case Type.DHCID:
					return new RecordDHCID(this);
				case Type.NSEC3:
					return new RecordNSEC3(this);
				case Type.NSEC3PARAM:
					return new RecordNSEC3PARAM(this);
				case Type.HIP:
					return new RecordHIP(this);
				case Type.SPF:
					return new RecordSPF(this);
				case Type.UINFO:
					return new RecordUINFO(this);
				case Type.UID:
					return new RecordUID(this);
				case Type.GID:
					return new RecordGID(this);
				case Type.UNSPEC:
					return new RecordUNSPEC(this);
				case Type.TKEY:
					return new RecordTKEY(this);
				case Type.TSIG:
					return new RecordTSIG(this);
				default:
					return new RecordUnknown(this);
			}
		}
	}
}