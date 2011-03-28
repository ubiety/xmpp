// SCRAMProcessor.cs
//
//Ubiety XMPP Library Copyright (C) 2011 Dieter Lunn
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
using System.Xml;
using System.Text;
using System.Security.Cryptography;
using ubiety.logging;
using ubiety.registries;
using ubiety.core.SASL;
using ubiety.core;
using Gnu.Inet.Encoding;

namespace ubiety.common.SASL
{
    /// <summary>
    /// 
    /// </summary>
    public class SCRAMProcessor : SASLProcessor
    {
    	private readonly Encoding _utf = Encoding.UTF8;

        private string _nonce;
        private string _snonce;
        private byte[] _salt;
        private int _i;
        private byte[] _clientFirst;
        private byte[] _serverFirst;
        private byte[] _clientFinal;

        private string _serverSignature;
        private string _clientProof;

    	/// <summary>
    	/// 
    	/// </summary>
    	/// <returns></returns>
    	public override Tag Initialize()
        {
            base.Initialize();
            Logger.Debug(this, "Initializing SCRAM Processor");

            Logger.Debug(this, "Generating nonce");
            _nonce = NextInt64().ToString();
            Logger.DebugFormat(this, "Nonce: {0}", _nonce);
            Logger.Debug(this, "Building Initial Message");
            var msg = new StringBuilder();
            msg.Append("n,,n=");
            msg.Append(Id.User);
            msg.Append(",r=");
            msg.Append(_nonce);
            Logger.DebugFormat(this, "Message: {0}", msg.ToString());

            var tag = (Auth)TagRegistry.Instance.GetTag("auth", Namespaces.SASL, new XmlDocument());
            tag.Mechanism = Mechanism.GetMechanism(MechanismType.SCRAM);
            tag.Bytes = _clientFirst = _utf.GetBytes(msg.ToString());
            return tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public override Tag Step(Tag tag)
        {
            var c = tag;
            _serverFirst = c.Bytes;
            var response = _utf.GetString(c.Bytes);
            Logger.DebugFormat(this, "Challenge: {0}", response);

            // Split challenge into pieces
            var tokens = response.Split(',');

            _snonce = tokens[0].Substring(2);
            // Ensure that the first length of nonce is the same nonce we sent.
            var r = tokens[0].Substring(2, _nonce.Length);
            if (0 != String.Compare(r, _nonce))
            {
                Logger.DebugFormat(this, "{0} does not match {1}", r, _nonce);
            }

            Logger.Debug(this, "Getting Salt");
            var a = tokens[1].Substring(2);
            _salt = Convert.FromBase64String(a);
            //string b = _utf.GetString(_salt);
            //b += "1";
            //_salt = _utf.GetBytes(b);
            Logger.DebugFormat(this, "Salt: {0}", _salt);

            Logger.Debug(this, "Getting Iterations");
            var i = tokens[2].Substring(2);
            _i = int.Parse(i);
            Logger.DebugFormat(this, "Iterations: {0}", _i);

            var final = new StringBuilder();
            final.Append("c=biws,r=");
            final.Append(_snonce);

            _clientFinal = _utf.GetBytes(final.ToString());

            CalculateProofs();

            final.Append(",p=");
            final.Append(_clientProof);

            Logger.DebugFormat(this, "Final Message: {0}", final.ToString());

            var resp = TagRegistry.Instance.GetTag("response", Namespaces.SASL, new XmlDocument());
            resp.Bytes = _utf.GetBytes(final.ToString());

            return resp;
        }

        private void CalculateProofs()
        {
            var hmac = new HMACSHA1();
            SHA1 hash = new SHA1CryptoServiceProvider();

            var saltedPassword = Hi();
            
            hmac.Key = _utf.GetBytes("Client Key");
            var clientKey = hmac.ComputeHash(saltedPassword);

            hmac.Key = _utf.GetBytes("Server Key");
            var serverKey = hmac.ComputeHash(saltedPassword);

            var storedKey = hash.ComputeHash(clientKey);

            var a = new StringBuilder();
            a.Append(_utf.GetString(_clientFirst));
            a.Append(",");
            a.Append(_utf.GetString(_serverFirst));
            a.Append(",");
            a.Append(_utf.GetString(_clientFinal));

            var auth = _utf.GetBytes(a.ToString());

            hmac.Key = auth;
            var signature = hmac.ComputeHash(storedKey);
            var server = hmac.ComputeHash(serverKey);

            _serverSignature = _utf.GetString(server);

            var proof = new byte[20];
            for (var i = 0; i < signature.Length; ++i)
            {
                proof[i] = (byte)(clientKey[i] ^ signature[i]);
            }

            _clientProof = _utf.GetString(proof);
        }

        private byte[] Hi()
        {
        	var prev = new byte[20];
            byte[] temp;
        	var password = _utf.GetBytes(Stringprep.SASLPrep(Password));

            // Add 1 to the end of salt with most significat octet first
            var key = new byte[_salt.Length + 4];

            Array.Copy(_salt, key, _salt.Length);
            byte[] g = { 0, 0, 0, 1 };
            Array.Copy(g, 0, key, _salt.Length, 4);

            // Compute initial hash
            var hmac = new HMACSHA1(key);
            var result = hmac.ComputeHash(password);
            Array.Copy(result, prev, result.Length);

            for (var i = 1; i < _i; ++i)
            {
                hmac.Key = prev;
                temp = hmac.ComputeHash(password);
                for (var j = 1; j < temp.Length; ++j)
                {
                    result[j] ^= temp[j];
                }

                Array.Copy(temp, prev, temp.Length);
            }

            return result;
        }
    }
}
