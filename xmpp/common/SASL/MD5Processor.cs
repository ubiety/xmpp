// MD5Processor.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2011 Dieter Lunn
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

using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ubiety.core;
using ubiety.core.SASL;
using ubiety.logging;
using ubiety.registries;

namespace ubiety.common.SASL
{
	/// <summary>
	/// 
	/// </summary>
	public class MD5Processor : SASLProcessor
	{
		private readonly Regex _csv = new Regex(@"(?<tag>[^=]+)=(?:(?<data>[^,""]+)|(?:""(?<data>[^""]*)"")),?",
		                                        RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private readonly Encoding _enc = Encoding.UTF8;
		private readonly MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();

		private string _cnonce;
		private string _digestUri;
		private int _nc;
		private string _ncString;
		private string _responseHash;

		/// <summary>
		/// 
		/// </summary>
		public MD5Processor()
		{
			_nc = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override Tag Initialize()
		{
			base.Initialize();

			var tag = (Auth) TagRegistry.Instance.GetTag("auth", Namespaces.SASL, new XmlDocument());
			tag.Mechanism = Mechanism.GetMechanism(MechanismType.DigestMD5);
			return tag;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public override Tag Step(Tag tag)
		{
			if (tag.Name == "success")
			{
				var succ = tag;
				PopulateDirectives(succ);
				Logger.DebugFormat(this, "rspauth = {0}", this["rspauth"]);

				return succ;
			}

			if (tag.Name == "failure")
			{
				Errors.Instance.SendError(this, ErrorType.AuthorizationFailed, "Failed authorization");
				return tag;
			}

			var chall = tag;
			Logger.Debug(this, _enc.GetString(tag.Bytes));
			PopulateDirectives(chall);
			var res = TagRegistry.Instance.GetTag("response", Namespaces.SASL, new XmlDocument());
			if (this["rspauth"] == null)
			{
				GenerateResponseHash();
				res.Bytes = GenerateResponse();
			}

			return res;
		}

		private void PopulateDirectives(Tag tag)
		{
			var col = _csv.Matches(_enc.GetString(tag.Bytes));
			foreach (Match m in col)
			{
				this[m.Groups["tag"].Value] = m.Groups["data"].Value;
			}

			if (this["realm"] != null)
				_digestUri = "xmpp/" + this["realm"];
			else
				_digestUri = "xmpp/" + Id.Server;
		}

		private byte[] GenerateResponse()
		{
			var sb = new StringBuilder();
			sb.Append("username=\"");
			sb.Append(Id.User);
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
			sb.Append(_digestUri);
			sb.Append("\",");
			sb.Append("response=");
			sb.Append(_responseHash);
			sb.Append(",");
			sb.Append("charset=");
			sb.Append(this["charset"]);
			var temp = sb.ToString();
			Logger.Debug(this, temp);
			return _enc.GetBytes(temp);
		}

		private void GenerateResponseHash()
		{
			var ae = new ASCIIEncoding();

			var v = NextInt64();

			// Create cnonce value using a random number, username and password
			var sb = new StringBuilder();
			sb.Append(v.ToString());
			sb.Append(":");
			sb.Append(Id.User);
			sb.Append(":");
			sb.Append(Password);

			_cnonce = HexString(ae.GetBytes(sb.ToString())).ToLower();

			// Create the nonce count which states how many times we have sent this packet.
			_nc++;
			_ncString = _nc.ToString().PadLeft(8, '0');

			// Create H1.  This value is the username/password portion of A1 according to the SASL DIGEST-MD5 RFC.
			sb.Remove(0, sb.Length);
			sb.Append(Id.User);
			sb.Append(":");
			sb.Append(this["realm"]);
			sb.Append(":");
			sb.Append(Password);
			var h1 = _md5.ComputeHash(ae.GetBytes(sb.ToString()));

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

			var a1 = sb.ToString();

			// Combine H1 and A1 into final A1
			var ms = new MemoryStream();
			ms.Write(h1, 0, 16);
			var temp = ae.GetBytes(a1);
			ms.Write(temp, 0, temp.Length);
			ms.Seek(0, SeekOrigin.Begin);
			h1 = _md5.ComputeHash(ms);

			//Create A2
			sb.Remove(0, sb.Length);
			sb.Append("AUTHENTICATE:");
			sb.Append(_digestUri);
			if (this["qop"].CompareTo("auth") != 0)
			{
				sb.Append(":00000000000000000000000000000000");
			}
			var a2 = sb.ToString();
			var h2 = _md5.ComputeHash(ae.GetBytes(a2));

			// Make A1 and A2 hex strings
			var p1 = HexString(h1).ToLower();
			var p2 = HexString(h2).ToLower();

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

			var a3 = sb.ToString();
			var h3 = _md5.ComputeHash(ae.GetBytes(a3));
			_responseHash = HexString(h3).ToLower();
		}
	}
}