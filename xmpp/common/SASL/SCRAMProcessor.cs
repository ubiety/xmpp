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
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
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
        private ASCIIEncoding _ae = new ASCIIEncoding();
        private readonly Encoding _utf = Encoding.UTF8;

        private string _nonce;
        private string _snonce;
        private byte[] _salt;
        private int _i;
        private byte[] _client_first;
        private byte[] _server_first;
        private byte[] _client_final;

        private string _server_signature;
        private string _client_proof;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override Tag Initialize()
        {
            base.Initialize();
            Logger.Debug(this, "Initializing SCRAM Processor");

            Logger.Debug(this, "Generating nonce");
            _nonce = NextInt64().ToString();
            Logger.DebugFormat(this, "Nonce: {0}", _nonce);
            Logger.Debug(this, "Building Initial Message");
            StringBuilder msg = new StringBuilder();
            msg.Append("n,,n=");
            msg.Append(_id.User);
            msg.Append(",r=");
            msg.Append(_nonce);
            Logger.DebugFormat(this, "Message: {0}", msg.ToString());

            Auth tag = (Auth)TagRegistry.Instance.GetTag("auth", Namespaces.SASL, new XmlDocument());
            tag.Mechanism = Mechanism.GetMechanism(MechanismType.SCRAM);
            tag.Bytes = _client_first = _utf.GetBytes(msg.ToString());
            return tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public override Tag Step(Tag tag)
        {
            Challenge c = tag as Challenge;
            _server_first = c.Bytes;
            string response = _utf.GetString(c.Bytes);
            Logger.DebugFormat(this, "Challenge: {0}", response);

            // Split challenge into pieces
            string[] tokens = response.Split(',');

            _snonce = tokens[0].Substring(2);
            // Ensure that the first length of nonce is the same nonce we sent.
            string r = tokens[0].Substring(2, _nonce.Length);
            if (0 != String.Compare(r, _nonce))
            {
                Logger.DebugFormat(this, "{0} does not match {1}", r, _nonce);
            }

            Logger.Debug(this, "Getting Salt");
            string a = tokens[1].Substring(2);
            _salt = Convert.FromBase64String(a);
            //string b = _utf.GetString(_salt);
            //b += "1";
            //_salt = _utf.GetBytes(b);
            Logger.DebugFormat(this, "Salt: {0}", _salt);

            Logger.Debug(this, "Getting Iterations");
            string i = tokens[2].Substring(2);
            _i = int.Parse(i);
            Logger.DebugFormat(this, "Iterations: {0}", _i);

            StringBuilder final = new StringBuilder();
            final.Append("c=biws,r=");
            final.Append(_snonce);

            _client_final = _utf.GetBytes(final.ToString());

            CalculateProofs();

            final.Append(",p=");
            final.Append(_client_proof);

            Logger.DebugFormat(this, "Final Message: {0}", final.ToString());

            Response resp = (Response)TagRegistry.Instance.GetTag("response", Namespaces.SASL, new XmlDocument());
            resp.Bytes = _utf.GetBytes(final.ToString());

            return resp;
        }

        private void CalculateProofs()
        {
            HMACSHA1 hmac = new HMACSHA1();
            SHA1 hash = new SHA1CryptoServiceProvider();

            byte[] salted_password = Hi();
            
            hmac.Key = _utf.GetBytes("Client Key");
            byte[] client_key = hmac.ComputeHash(salted_password);

            hmac.Key = _utf.GetBytes("Server Key");
            byte[] server_key = hmac.ComputeHash(salted_password);

            byte[] stored_key = hash.ComputeHash(client_key);

            StringBuilder a = new StringBuilder();
            a.Append(_utf.GetString(_client_first));
            a.Append(",");
            a.Append(_utf.GetString(_server_first));
            a.Append(",");
            a.Append(_utf.GetString(_client_final));

            byte[] auth = _utf.GetBytes(a.ToString());

            hmac.Key = auth;
            byte[] signature = hmac.ComputeHash(stored_key);
            byte[] server = hmac.ComputeHash(server_key);

            _server_signature = _utf.GetString(server);

            byte[] proof = new byte[20];
            for (int i = 0; i < signature.Length; ++i)
            {
                proof[i] = (byte)(client_key[i] ^ signature[i]);
            }

            _client_proof = _utf.GetString(proof);
        }

        private byte[] Hi()
        {
            HMACSHA1 hmac;
            byte[] prev = new byte[20];
            byte[] temp = new byte[20];
            byte[] result = new byte[20];
            byte[] password = _utf.GetBytes(Stringprep.SASLPrep(_password));

            // Add 1 to the end of salt with most significat octet first
            byte[] key = new byte[_salt.Length + 4];

            Array.Copy(_salt, key, _salt.Length);
            byte[] g = { 0, 0, 0, 1 };
            Array.Copy(g, 0, key, _salt.Length, 4);

            // Compute initial hash
            hmac = new HMACSHA1(key);
            result = hmac.ComputeHash(password);
            Array.Copy(result, prev, result.Length);

            for (int i = 1; i < _i; ++i)
            {
                hmac.Key = prev;
                temp = hmac.ComputeHash(password);
                for (int j = 1; j < temp.Length; ++j)
                {
                    result[j] ^= temp[j];
                }

                Array.Copy(temp, prev, temp.Length);
            }

            return result;
        }
    }
}
