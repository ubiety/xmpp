// Resolver.cs
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
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ubiety.net.dns
{
	/// <summary>
	/// Summary description for Dns.
	/// </summary>
	public static class Resolver
	{
		private const int DNSPort = 53;
		private const int UdpRetryAttempts = 2;
		private const int UdpTimeout = 1000;
		private static int _uniqueId;

		/// <summary>
		/// Shorthand form to make SRV querying easier, essentially wraps up the retreival
		/// of the SRV records, and sorts them by preference
		/// </summary>
		/// <param name="domain">domain name to retreive SRV RRs for</param>
		/// <param name="dnsServer">the server we're going to ask</param>
		/// <returns>An array of SRVRecords</returns>
		public static SRVRecord[] SRVLookup(string domain, IPAddress dnsServer)
		{
			// check the inputs
			if (domain == null) throw new ArgumentNullException("domain");
			if (dnsServer == null) throw new ArgumentNullException("dnsServer");

			// create a request for this
			var request = new Request();

			// add one question - the SRV IN lookup for the supplied domain
			request.AddQuestion(new Question(domain, DnsType.SRV, DnsClass.IN));

			// fire it off
			var response = Lookup(request, dnsServer);

			// if we didn't get a response, then return null
			if (response == null) return null;

			// create a growable array of SRV records
			var resourceRecords = new ArrayList();

			// add each of the answers to the array
			foreach (var answer in response.Answers.Where(answer => answer.Type == DnsType.SRV))
			{
				// add it to our array
				resourceRecords.Add(answer.Record);
			}

			// create array of MX records
			var srvRecords = new SRVRecord[resourceRecords.Count];

			// copy from the array list
			resourceRecords.CopyTo(srvRecords);

			// sort into lowest preference order
			Array.Sort(srvRecords);

			// and return
			return srvRecords;
		}

		/// <summary>
		/// The principal look up function, which sends a request message to the given
		/// DNS server and collects a response. This implementation re-sends the message
		/// via UDP up to two times in the event of no response/packet loss
		/// </summary>
		/// <param name="request">The logical request to send to the server</param>
		/// <param name="dnsServer">The IP address of the DNS server we are querying</param>
		/// <returns>The logical response from the DNS server or null if no response</returns>
		public static Response Lookup(Request request, IPAddress dnsServer)
		{
			// check the inputs
			if (request == null) throw new ArgumentNullException("request");
			if (dnsServer == null) throw new ArgumentNullException("dnsServer");

			// We will not catch exceptions here, rather just refer them to the caller

			// create an end point to communicate with
			var server = new IPEndPoint(dnsServer, DNSPort);

			// get the message
			var requestMessage = request.GetMessage();

			// send the request and get the response
			var responseMessage = UdpTransfer(server, requestMessage);

			// and populate a response object from that and return it
			return new Response(responseMessage);
		}

		private static byte[] UdpTransfer(EndPoint server, byte[] requestMessage)
		{
			// UDP can fail - if it does try again keeping track of how many attempts we've made
			var attempts = 0;

			// try repeatedly in case of failure
			while (attempts <= UdpRetryAttempts)
			{
				// firstly, uniquely mark this request with an id
				unchecked
				{
					// substitute in an id unique to this lookup, the request has no idea about this
					requestMessage[0] = (byte) (_uniqueId >> 8);
					requestMessage[1] = (byte) _uniqueId;
				}

				// we'll be send and receiving a UDP packet
				var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				// we will wait at most 1 second for a dns reply
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, UdpTimeout);

				// send it off to the server
				socket.SendTo(requestMessage, requestMessage.Length, SocketFlags.None, server);

				// RFC1035 states that the maximum size of a UDP datagram is 512 octets (bytes)
				var responseMessage = new byte[512];

				try
				{
					// wait for a response upto 1 second
					socket.Receive(responseMessage);

					// make sure the message returned is ours
					if (responseMessage[0] == requestMessage[0] && responseMessage[1] == requestMessage[1])
					{
						// its a valid response - return it, this is our successful exit point
						return responseMessage;
					}
				}
				catch (SocketException)
				{
					// failure - we better try again, but remember how many attempts
					attempts++;
				}
				finally
				{
					// increase the unique id
					_uniqueId++;

					// close the socket
					socket.Close();
				}
			}

			// the operation has failed, this is our unsuccessful exit point
			throw new NoResponseException();
		}
	}
}