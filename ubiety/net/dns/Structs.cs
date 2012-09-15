/*
 * http://www.iana.org/assignments/dns-parameters
 * 
 * 
 * 
 */


namespace ubiety.net.dns
{
	/*
	 * 3.2.2. TYPE values
	 *
	 * TYPE fields are used in resource records.
	 * Note that these types are a subset of QTYPEs.
	 *
	 *		TYPE		value			meaning
	 */
	/// <summary>
	/// 
	/// </summary>
	public enum Type : ushort
	{
		/// <summary>
		/// a IPv4 host address
		/// </summary>
		A = 1,
		/// <summary>
		/// an authoritative name server
		/// </summary>
		NS = 2,
		/// <summary>
		/// a mail destination (Obsolete - use MX)
		/// </summary>
		MD = 3,
		/// <summary>
		/// a mail forwarder (Obsolete - use MX)
		/// </summary>
		MF = 4,
		/// <summary>
		/// the canonical name for an alias
		/// </summary>
		CNAME = 5,
		/// <summary>
		/// marks the start of a zone of authority
		/// </summary>
		SOA = 6,
		/// <summary>
		/// a mailbox domain name (EXPERIMENTAL)
		/// </summary>
		MB = 7,
		/// <summary>
		/// a mail group member (EXPERIMENTAL)
		/// </summary>
		MG = 8,
		/// <summary>
		/// a mail rename domain name (EXPERIMENTAL)
		/// </summary>
		MR = 9,
		/// <summary>
		/// a null RR (EXPERIMENTAL)
		/// </summary>
		NULL = 10,
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
		/// The Responsible Person rfc1183
		/// </summary>
		RP = 17,
		/// <summary>
		/// AFS Data Base location
		/// </summary>
		AFSDB = 18,
		/// <summary>
		/// X.25 address rfc1183
		/// </summary>
		X25 = 19,
		/// <summary>
		/// ISDN address rfc1183
		/// </summary>
		ISDN = 20,
		/// <summary>
		/// The Route Through rfc1183
		/// </summary>
		RT = 21,

		/// <summary>
		/// Network service access point address rfc1706
		/// </summary>
		NSAP = 22,
		/// <summary>
		/// Obsolete, rfc1348
		/// </summary>
		NSAPPTR = 23,

		/// <summary>
		/// Cryptographic public key signature rfc2931 / rfc2535
		/// </summary>
		SIG = 24,
		/// <summary>
		/// Public key as used in DNSSEC rfc2535
		/// </summary>
		KEY = 25,

		/// <summary>
		/// Pointer to X.400/RFC822 mail mapping information rfc2163
		/// </summary>
		PX = 26,

		/// <summary>
		/// Geographical position rfc1712 (obsolete)
		/// </summary>
		GPOS = 27,

		/// <summary>
		/// a IPv6 host address, rfc3596
		/// </summary>
		AAAA = 28,

		/// <summary>
		/// Location information rfc1876
		/// </summary>
		LOC = 29,

		/// <summary>
		/// Next Domain, Obsolete rfc2065 / rfc2535
		/// </summary>
		NXT = 30,

		/// <summary>
		/// *** Endpoint Identifier (Patton)
		/// </summary>
		EID = 31,
		/// <summary>
		/// *** Nimrod Locator (Patton)
		/// </summary>
		NIMLOC = 32,

		/// <summary>
		/// Location of services rfc2782
		/// </summary>
		SRV = 33,

		/// <summary>
		/// *** ATM Address (Dobrowski)
		/// </summary>
		ATMA = 34,

		/// <summary>
		/// The Naming Authority Pointer rfc3403
		/// </summary>
		NAPTR = 35,

		/// <summary>
		/// Key Exchange Delegation Record rfc2230
		/// </summary>
		KX = 36,

		CERT = 37,			// *** CERT RFC2538

		A6 = 38,			// IPv6 address rfc3363 (rfc2874 rfc3226)
		DNAME = 39,			// A way to provide aliases for a whole domain, not just a single domain name as with CNAME. rfc2672

		SINK = 40,			// *** SINK Eastlake
		OPT = 41,			// *** OPT RFC2671

		APL = 42,			// *** APL [RFC3123]

		DS = 43,			// Delegation Signer rfc3658

		SSHFP = 44,			// SSH Key Fingerprint rfc4255
		IPSECKEY = 45,		// IPSECKEY rfc4025
		RRSIG = 46,			// RRSIG rfc3755
		NSEC = 47,			// NSEC rfc3755
		DNSKEY = 48,		// DNSKEY 3755
		DHCID = 49,			// DHCID rfc4701

		NSEC3 = 50,			// NSEC3 rfc5155
		NSEC3PARAM = 51,	// NSEC3PARAM rfc5155

		HIP = 55,			// Host Identity Protocol  [RFC-ietf-hip-dns-09.txt]

		SPF = 99,			// SPF rfc4408

		UINFO = 100,		// *** IANA-Reserved
		UID = 101,			// *** IANA-Reserved
		GID = 102,			// *** IANA-Reserved
		UNSPEC = 103,		// *** IANA-Reserved

		TKEY = 249,			// Transaction key rfc2930
		TSIG = 250,			// Transaction signature rfc2845

		TA=32768,			// DNSSEC Trust Authorities          [Weiler]  13 December 2005
		DLV=32769			// DNSSEC Lookaside Validation       [RFC4431]
	}

	/*
	 * 3.2.3. QTYPE values
	 *
	 * QTYPE fields appear in the question part of a query.  QTYPES are a
	 * superset of TYPEs, hence all TYPEs are valid QTYPEs.  In addition, the
	 * following QTYPEs are defined:
	 *
	 *		QTYPE		value			meaning
	 */
	public enum QType : ushort
	{
		A = Type.A,			// a IPV4 host address
		NS = Type.NS,		// an authoritative name server
		MD = Type.MD,		// a mail destination (Obsolete - use MX)
		MF = Type.MF,		// a mail forwarder (Obsolete - use MX)
		CNAME = Type.CNAME,	// the canonical name for an alias
		SOA = Type.SOA,		// marks the start of a zone of authority
		MB = Type.MB,		// a mailbox domain name (EXPERIMENTAL)
		MG = Type.MG,		// a mail group member (EXPERIMENTAL)
		MR = Type.MR,		// a mail rename domain name (EXPERIMENTAL)
		NULL = Type.NULL,	// a null RR (EXPERIMENTAL)
		WKS = Type.WKS,		// a well known service description
		PTR = Type.PTR,		// a domain name pointer
		HINFO = Type.HINFO,	// host information
		MINFO = Type.MINFO,	// mailbox or mail list information
		MX = Type.MX,		// mail exchange
		TXT = Type.TXT,		// text strings

		RP = Type.RP,		// The Responsible Person rfc1183
		AFSDB = Type.AFSDB,	// AFS Data Base location
		X25 = Type.X25,		// X.25 address rfc1183
		ISDN = Type.ISDN,	// ISDN address rfc1183
		RT = Type.RT,		// The Route Through rfc1183

		NSAP = Type.NSAP,	// Network service access point address rfc1706
		NSAP_PTR = Type.NSAPPTR, // Obsolete, rfc1348

		SIG = Type.SIG,		// Cryptographic public key signature rfc2931 / rfc2535
		KEY = Type.KEY,		// Public key as used in DNSSEC rfc2535

		PX = Type.PX,		// Pointer to X.400/RFC822 mail mapping information rfc2163

		GPOS = Type.GPOS,	// Geographical position rfc1712 (obsolete)

		AAAA = Type.AAAA,	// a IPV6 host address

		LOC = Type.LOC,		// Location information rfc1876

		NXT = Type.NXT,		// Obsolete rfc2065 / rfc2535

		EID = Type.EID,		// *** Endpoint Identifier (Patton)
		NIMLOC = Type.NIMLOC,// *** Nimrod Locator (Patton)

		SRV = Type.SRV,		// Location of services rfc2782

		ATMA = Type.ATMA,	// *** ATM Address (Dobrowski)

		NAPTR = Type.NAPTR,	// The Naming Authority Pointer rfc3403

		KX = Type.KX,		// Key Exchange Delegation Record rfc2230

		CERT = Type.CERT,	// *** CERT RFC2538

		A6 = Type.A6,		// IPv6 address rfc3363
		DNAME = Type.DNAME,	// A way to provide aliases for a whole domain, not just a single domain name as with CNAME. rfc2672

		SINK = Type.SINK,	// *** SINK Eastlake
		OPT = Type.OPT,		// *** OPT RFC2671

		APL = Type.APL,		// *** APL [RFC3123]

		DS = Type.DS,		// Delegation Signer rfc3658

		SSHFP = Type.SSHFP,	// *** SSH Key Fingerprint RFC-ietf-secsh-dns
		IPSECKEY = Type.IPSECKEY, // rfc4025
		RRSIG = Type.RRSIG,	// *** RRSIG RFC-ietf-dnsext-dnssec-2535
		NSEC = Type.NSEC,	// *** NSEC RFC-ietf-dnsext-dnssec-2535
		DNSKEY = Type.DNSKEY,// *** DNSKEY RFC-ietf-dnsext-dnssec-2535
		DHCID = Type.DHCID,	// rfc4701

		NSEC3 = Type.NSEC3,	// RFC5155
		NSEC3PARAM = Type.NSEC3PARAM, // RFC5155

		HIP = Type.HIP,		// RFC-ietf-hip-dns-09.txt

		SPF = Type.SPF,		// RFC4408
		UINFO = Type.UINFO,	// *** IANA-Reserved
		UID = Type.UID,		// *** IANA-Reserved
		GID = Type.GID,		// *** IANA-Reserved
		UNSPEC = Type.UNSPEC,// *** IANA-Reserved

		TKEY = Type.TKEY,	// Transaction key rfc2930
		TSIG = Type.TSIG,	// Transaction signature rfc2845

		IXFR = 251,			// incremental transfer                  [RFC1995]
		AXFR = 252,			// transfer of an entire zone            [RFC1035]
		MAILB = 253,		// mailbox-related RRs (MB, MG or MR)    [RFC1035]
		MAILA = 254,		// mail agent RRs (Obsolete - see MX)    [RFC1035]
		ANY = 255,			// A request for all records             [RFC1035]

		TA = Type.TA,		// DNSSEC Trust Authorities    [Weiler]  13 December 2005
		DLV = Type.DLV		// DNSSEC Lookaside Validation [RFC4431]
	}
	/*
	 * 3.2.4. CLASS values
	 *
	 * CLASS fields appear in resource records.  The following CLASS mnemonics
	 *and values are defined:
	 *
	 *		CLASS		value			meaning
	 */
	/// <summary>
	/// 
	/// </summary>
	public enum Class : ushort
	{
		/// <summary>
		/// 
		/// </summary>
		IN = 1,				// the Internet
		/// <summary>
		/// 
		/// </summary>
		CS = 2,				// the CSNET class (Obsolete - used only for examples in some obsolete RFCs)
		/// <summary>
		/// 
		/// </summary>
		CH = 3,				// the CHAOS class
		/// <summary>
		/// 
		/// </summary>
		HS = 4				// Hesiod [Dyer 87]
	}
	/*
	 * 3.2.5. QCLASS values
	 *
	 * QCLASS fields appear in the question section of a query.  QCLASS values
	 * are a superset of CLASS values; every CLASS is a valid QCLASS.  In
	 * addition to CLASS values, the following QCLASSes are defined:
	 *
	 *		QCLASS		value			meaning
	 */
	/// <summary>
	/// 
	/// </summary>
	public enum QClass : ushort
	{
		/// <summary>
		/// 
		/// </summary>
		IN = Class.IN,		// the Internet
		/// <summary>
		/// 
		/// </summary>
		CS = Class.CS,		// the CSNET class (Obsolete - used only for examples in some obsolete RFCs)
		/// <summary>
		/// 
		/// </summary>
		CH = Class.CH,		// the CHAOS class
		/// <summary>
		/// 
		/// </summary>
		HS = Class.HS,		// Hesiod [Dyer 87]

		/// <summary>
		/// 
		/// </summary>
		ANY = 255			// any class
	}

	/*
RCODE           Response code - this 4 bit field is set as part of
				responses.  The values have the following
				interpretation:
	 */
	public enum RCode
	{
		NoError = 0,		// No Error                           [RFC1035]
		FormErr = 1,		// Format Error                       [RFC1035]
		ServFail = 2,		// Server Failure                     [RFC1035]
		NXDomain = 3,		// Non-Existent Domain                [RFC1035]
		NotImp = 4,			// Not Implemented                    [RFC1035]
		Refused = 5,		// Query Refused                      [RFC1035]
		YXDomain = 6,		// Name Exists when it should not     [RFC2136]
		YXRRSet = 7,		// RR Set Exists when it should not   [RFC2136]
		NXRRSet = 8,		// RR Set that should exist does not  [RFC2136]
		NotAuth = 9,		// Server Not Authoritative for zone  [RFC2136]
		NotZone = 10,		// Name not contained in zone         [RFC2136]

		RESERVED11 = 11,	// Reserved
		RESERVED12 = 12,	// Reserved
		RESERVED13 = 13,	// Reserved
		RESERVED14 = 14,	// Reserved
		RESERVED15 = 15,	// Reserved

		BADVERSSIG = 16,	// Bad OPT Version                    [RFC2671]
							// TSIG Signature Failure             [RFC2845]
		BADKEY = 17,		// Key not recognized                 [RFC2845]
		BADTIME = 18,		// Signature out of time window       [RFC2845]
		BADMODE = 19,		// Bad TKEY Mode                      [RFC2930]
		BADNAME = 20,		// Duplicate key name                 [RFC2930]
		BADALG = 21,		// Algorithm not supported            [RFC2930]
		BADTRUNC = 22		// Bad Truncation                     [RFC4635]
		/*
			23-3840              available for assignment
				0x0016-0x0F00
			3841-4095            Private Use
				0x0F01-0x0FFF
			4096-65535           available for assignment
				0x1000-0xFFFF
		*/

	}

	/*
OPCODE          A four bit field that specifies kind of query in this
				message.  This value is set by the originator of a query
				and copied into the response.  The values are:

				0               a standard query (QUERY)

				1               an inverse query (IQUERY)

				2               a server status request (STATUS)

				3-15            reserved for future use
	 */
	public enum OPCode
	{
		Query = 0,				// a standard query (QUERY)
		IQUERY = 1,				// OpCode Retired (previously IQUERY - No further [RFC3425]
								// assignment of this code available)
		Status = 2,				// a server status request (STATUS) RFC1035
		RESERVED3 = 3,			// IANA

		Notify = 4,				// RFC1996
		Update = 5,				// RFC2136

		RESERVED6 = 6,
		RESERVED7 = 7,
		RESERVED8 = 8,
		RESERVED9 = 9,
		RESERVED10 = 10,
		RESERVED11 = 11,
		RESERVED12 = 12,
		RESERVED13 = 13,
		RESERVED14 = 14,
		RESERVED15 = 15,
	}

	/// <summary>
	/// 
	/// </summary>
	public enum TransportType
	{
		/// <summary>
		/// 
		/// </summary>
		Udp,
		/// <summary>
		/// 
		/// </summary>
		Tcp
	}
}
