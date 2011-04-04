// Question.cs
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
using System.Text.RegularExpressions;

namespace ubiety.net.dns
{
	/// <summary>
	/// Represents a DNS Question, comprising of a domain to query, the type of query (QTYPE) and the class
	/// of query (QCLASS). This class is an encapsulation of these three things, and extensive argument checking
	/// in the constructor as this may well be created outside the assembly (public protection)
	/// </summary>	
	public class Question
	{
		// A question is these three things combined
		private readonly string		_domain;
		private readonly DnsType	_dnsType;
		private readonly DnsClass	_dnsClass;

		// expose them read/only to the world
		public string	Domain		{ get { return _domain;		}}
		public DnsType	Type		{ get { return _dnsType;	}}
		public DnsClass	Class		{ get { return _dnsClass;	}}

		/// <summary>
		/// Construct the question from parameters, checking for safety
		/// </summary>
		/// <param name="domain">the domain name to query eg. bigdevelopments.co.uk</param>
		/// <param name="dnsType">the QTYPE of query eg. DnsType.MX</param>
		/// <param name="dnsClass">the CLASS of query, invariably DnsClass.IN</param>
		public Question(string domain, DnsType dnsType, DnsClass dnsClass)
		{
			// check the input parameters
			if (domain == null) 
                throw new ArgumentNullException("domain");

			// do a sanity check on the domain name to make sure its legal
			if (domain.Length ==0 || domain.Length>255 || !Regex.IsMatch(domain, @"^[a-z|A-Z|0-9|\-|_]{1,63}(\.[a-z|A-Z|0-9|\-|_]{1,63})+$"))
			{
				// domain names can't be bigger tan 255 chars, and individal labels can't be bigger than 63 chars
				throw new ArgumentException("The supplied domain name was not in the correct form", "domain");
			}

			// sanity check the DnsType parameter
			if (!Enum.IsDefined(typeof(DnsType), dnsType) || dnsType == DnsType.None)
			{
				throw new ArgumentOutOfRangeException("dnsType");
			}

			// sanity check the DnsClass parameter
			if (!Enum.IsDefined(typeof(DnsClass), dnsClass) || dnsClass == DnsClass.None)
			{                
				throw new ArgumentOutOfRangeException("dnsClass");
			}

			// just remember the values
			_domain = domain;
			_dnsType = dnsType;
			_dnsClass = dnsClass;
		}

		/// <summary>
		/// Construct the question reading from a DNS Server response. Consult RFC1035 4.1.2
		/// for byte-wise details of this structure in byte array form
		/// </summary>
		/// <param name="pointer">a logical pointer to the Question in byte array form</param>
		internal Question(Pointer pointer)
		{
			// extract from the message
			_domain = pointer.ReadDomain();
			_dnsType = (DnsType)pointer.ReadShort();
			_dnsClass = (DnsClass)pointer.ReadShort();
		}
	}
}
