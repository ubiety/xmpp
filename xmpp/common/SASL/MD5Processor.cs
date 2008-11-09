// MD5Processor.cs
//
//XMPP .NET Library Copyright (C) 2006, 2007 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using xmpp.registries;
using xmpp.core.SASL;
using xmpp.common;
using xmpp.core;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using xmpp.logging;
using xmpp;

namespace xmpp.common.SASL
{
	public class MD5Processor : SASLProcessor
	{
		private readonly MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
		private readonly Encoding _enc = Encoding.UTF8;
		private readonly Regex _csv = new Regex(@"(?<tag>[^=]+)=(?:(?<data>[^,""]+)|(?:""(?<data>[^""]*)"")),?", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		
		private string _responseHash;
		private string _cnonce;
		private int _nc;
		private string _ncString;
		
		public MD5Processor()
		{
			_nc = 0;
		}
		
		public override Tag Initialize(XID id, string password)
		{
			base.Initialize(id, password);
			
			Auth tag = (Auth)TagRegistry.Instance.GetTag("", new XmlQualifiedName("auth", Namespaces.SASL), new XmlDocument());
			tag.Mechanism = Mechanism.GetMechanism(MechanismType.DIGEST_MD5);
			return tag;
		}
		
		public override Tag Step(Tag tag)
		{
			if (tag is Success)
			{
				Success succ = tag as Success;
				populateDirectives(succ);
				Logger.DebugFormat(this, "rspauth = {0}", this["rspauth"]);
				
				// Either this isn't returning or an exception is being thrown around here.
				return succ;
			}
			else if (tag is Failure)
			{
				Errors.Instance.SendError(this, ErrorType.AuthorizationFailed, "Failed authorization");
				return tag;
			}
			
			Challenge chall = tag as Challenge;
			populateDirectives(chall);
			Response res = (Response)TagRegistry.Instance.GetTag("", new XmlQualifiedName("response", Namespaces.SASL), new XmlDocument());
			generateResponseHash();
			res.Bytes = generateResponse();
			
			return res;
		}
		
		private void populateDirectives(Tag tag)
		{
			MatchCollection col = _csv.Matches(_enc.GetString(tag.Bytes));
			foreach(Match m in col)
			{
				this[m.Groups["tag"].Value] = m.Groups["data"].Value;
			}
		}
		
		private byte[] generateResponse()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("username=\"");
			sb.Append(_id.User);
			sb.Append("\",");
			sb.Append("realm=\"");
			sb.Append(this["realm"]);
			sb.Append("\",");
			sb.Append("nonce=\"");
			sb.Append(this["nonce"]);
			sb.Append("\",");
			sb.Append("cnonce=\"");
			sb.Append(_cnonce);
			sb.Append("\",");
			sb.Append("nc=");
			sb.Append(_ncString);
			sb.Append(",");
			sb.Append("qop=");
			sb.Append(this["qop"]);
			sb.Append(",");
			sb.Append("digest-uri=\"");
			sb.Append("xmpp/");
			sb.Append(this["realm"]);
			sb.Append("\",");
			sb.Append("response=");
			sb.Append(_responseHash);
			sb.Append(",");
			sb.Append("charset=");
			sb.Append(this["charset"]);
			string temp = sb.ToString();
			Logger.Debug(this, temp);
			return _enc.GetBytes(temp);
		}
		
		private void generateResponseHash()
		{
			ASCIIEncoding ae = new ASCIIEncoding();
			byte[] H1, H2, H3, temp;
			string A1, A2, A3, uri, p1, p2;
			
			uri = "xmpp/" + this["realm"];
			Random r = new Random();
			int v = r.Next(1024);
			
			// Create cnonce value using a random number, username and password
			StringBuilder sb = new StringBuilder();
			sb.Append(v.ToString());
			sb.Append(":");
			sb.Append(_id.User);
			sb.Append(":");
			sb.Append(_password);
			
			_cnonce = HexString(ae.GetBytes(sb.ToString())).ToLower();
			
			// Create the nonce count which states how many times we have sent this packet.
			_nc++;
			_ncString = _nc.ToString().PadLeft(8, '0');
			
			// Create H1.  This value is the username/password portion of A1 according to the SASL DIGEST-MD5 RFC.
			sb.Remove(0, sb.Length);
			sb.Append(_id.User);
			sb.Append(":");
			sb.Append(this["realm"]);
			sb.Append(":");
			sb.Append(_password);
			H1 = _md5.ComputeHash(ae.GetBytes(sb.ToString()));
			
			// Create the rest of A1 as stated in the RFC.
			sb.Remove(0, sb.Length);
			sb.Append(":");
			sb.Append(this["nonce"]);
			sb.Append(":");
			sb.Append(_cnonce);
			
			if (this["authzid"] != null)
			{
				sb.Append(":");
				sb.Append(this["authzid"]);
			}
			
			A1 = sb.ToString();
			
			// Combine H1 and A1 into final A1
			MemoryStream ms = new MemoryStream();
			ms.Write(H1, 0, 16);
			temp = ae.GetBytes(A1);
			ms.Write(temp, 0, temp.Length);
			ms.Seek(0, SeekOrigin.Begin);
			H1 = _md5.ComputeHash(ms);
			
			//Create A2
			sb.Remove(0, sb.Length);
			sb.Append("AUTHENTICATE:");
			sb.Append(uri);
			if (this["qop"].CompareTo("auth") != 0)
			{
				sb.Append(":00000000000000000000000000000000");
			}
			A2 = sb.ToString();
			H2 = _md5.ComputeHash(ae.GetBytes(A2));
			
			// Make A1 and A2 hex strings
			p1 = HexString(H1).ToLower();
			p2 = HexString(H2).ToLower();
			
			// Combine all portions into the final response hex string
			sb.Remove(0, sb.Length);
			sb.Append(p1);
			sb.Append(":");
			sb.Append(this["nonce"]);
			sb.Append(":");
			sb.Append(_ncString);
			sb.Append(":");
			sb.Append(_cnonce);
			sb.Append(":");
			sb.Append(this["qop"]);
			sb.Append(":");
			sb.Append(p2);
			
			A3 = sb.ToString();
			H3 = _md5.ComputeHash(ae.GetBytes(A3));
			_responseHash = HexString(H3).ToLower();
		}
		
		private string HexString(byte[] buff)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in buff)
			{
				sb.Append(b.ToString("x2"));
			}
			
			return sb.ToString();
		}
	}
}
