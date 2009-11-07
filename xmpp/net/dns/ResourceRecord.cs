// ResourceRecord.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2009 Dieter Lunn
// 
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// 
// You should have received a copy of the GNU Lesser General Public License along
// with this library; if not, write to the Free Software Foundation, Inc., 59
// Temple Place, Suite 330, Boston, MA 02111-1307 USA

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

using System;

namespace ubiety.net.dns
{
	/// <summary>
	/// Represents a Resource Record as detailed in RFC1035 4.1.3
	/// </summary>	
	public class ResourceRecord
	{
		// private, constructor initialised fields
		private readonly string		_domain;
		private readonly DnsType	_dnsType;
		private readonly DnsClass	_dnsClass;
		private readonly int		_Ttl;
		private readonly RecordBase	_record;

		// read only properties applicable for all records
		public string Domain
        { 
            get { return _domain; }
        }
		
        public DnsType Type
        {
            get { return _dnsType; }
        }

		public DnsClass	Class		   
        { 
            get { return _dnsClass;	}
        }
		
        public int Ttl
        { 
            get { return _Ttl; }
        }

		public RecordBase Record
        { 
            get { return _record; }
        }

		/// <summary>
		/// Construct a resource record from a pointer to a byte array
		/// </summary>
		/// <param name="pointer">the position in the byte array of the record</param>
		internal ResourceRecord(Pointer pointer)
		{
			// extract the domain, question type, question class and Ttl
			_domain     = pointer.ReadDomain();
			_dnsType    = (DnsType) pointer.ReadShort();
			_dnsClass   = (DnsClass) pointer.ReadShort();
			_Ttl        = pointer.ReadInt();

			// the next short is the record length, we only use it for unrecognised record types
			int recordLength = pointer.ReadShort();

			// and create the appropriate RDATA record based on the dnsType
			switch (_dnsType)
			{
                case DnsType.SRV:
                    _record = new SRVRecord(pointer);
                    break;
                case DnsType.ANAME:
                	_record = new ANameRecord(pointer);
                	break;
				
                default:
				{
					// move the pointer over this unrecognised record
					pointer.Position += recordLength;
					break;
				}
			}
		}
	}

	// Answers, Name Servers and Additional Records all share the same RR format	
	public class Answer : ResourceRecord
	{
		internal Answer(Pointer pointer) : base(pointer) {}
	}
    	
	public class NameServer : ResourceRecord
	{
		internal NameServer(Pointer pointer) : base(pointer) {}
	}
    	
	public class AdditionalRecord : ResourceRecord
	{
		internal AdditionalRecord(Pointer pointer) : base(pointer) {}
	}
}