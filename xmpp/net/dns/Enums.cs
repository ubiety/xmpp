// Ubiety XMPP .NET Library Copyright (C) 2009 Dieter Lunn
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
	/// The DNS TYPE (RFC1035 3.2.2/3).  Not all are supported
	/// </summary>
	public enum DnsType
	{
		None = 0,
        /// <summary>
        /// a host address
        /// </summary>
        ANAME = 1,

        /// <summary>
        /// an authoritative name server
        /// </summary>
        NS = 2,
        //	MD    = 3,  Obsolete
        //	MF    = 5,  Obsolete

        /// <summary>
        /// the canonical name for an alias
        /// </summary>
        CNAME = 5,

        /// <summary>
        /// marks the start of a zone of authority
        /// </summary>
        SOA = 6,
        //	MB    = 7,  EXPERIMENTAL
        //	MG    = 8,  EXPERIMENTAL
        //  MR    = 9,  EXPERIMENTAL
        //	NULL  = 10, EXPERIMENTAL
        /// <summary>
        /// a well known service description
        /// </summary>
        WKS = 11,

        /// <summary>
        /// a domain name pointer
        /// </summary>
        PTR = 12,

        /// <summary>
        /// host information
        /// </summary>
        HINFO = 13,

        /// <summary>
        /// mailbox or mail list information
        /// </summary>
        MINFO = 14,

        /// <summary>
        /// mail exchange
        /// </summary>
        MX = 15,

        /// <summary>
        /// text strings
        /// </summary>
        TXT = 16,

        /// <summary>
        /// SRV Records
        /// </summary>
        SRV = 33,
	}

	/// <summary>
	/// The DNS CLASS (RFC1035 3.2.4/5)
	/// Internet will be the one we'll be using (IN), the others are for completeness
	/// </summary>
	public enum DnsClass
	{
		None = 0,
        IN = 1,
        CS = 2,
        CH = 3,
        HS = 4
	}

	/// <summary>
	/// (RFC1035 4.1.1) These are the return codes the server can send back
	/// </summary>
	public enum ReturnCode
	{
		Success = 0,
		FormatError = 1,
		ServerFailure = 2,
		NameError = 3,
		NotImplemented = 4,
		Refused = 5,
		Other = 6
	}

	/// <summary>
	/// (RFC1035 4.1.1) These are the Query Types which apply to all questions in a request
	/// </summary>
	public enum Opcode
	{
		StandardQuery = 0,
		InverseQuery = 1,
		StatusRequest = 2,
		Reserved3 = 3,
		Reserved4 = 4,
		Reserved5 = 5,
		Reserved6 = 6,
		Reserved7 = 7,
		Reserved8 = 8,
		Reserved9 = 9,
		Reserved10 = 10,
		Reserved11 = 11,
		Reserved12 = 12,
		Reserved13 = 13,
		Reserved14 = 14,
		Reserved15 = 15,
	}
}
