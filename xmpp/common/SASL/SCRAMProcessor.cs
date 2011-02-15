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
        private byte[] _salt;
        private int _i;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override Tag Initialize(XID id, string password)
        {
            base.Initialize(id, password);
            Logger.Debug(this, "Initializing SCRAM Processor");

            Logger.Debug(this, "Generating nonce");
            _nonce = NextInt64().ToString();
            Logger.DebugFormat(this, "Nonce: {0}", _nonce);
            Logger.Debug(this, "Building Initial Message");
            StringBuilder msg = new StringBuilder();
            msg.Append("n,,n=");
            msg.Append(id.User);
            msg.Append(",r=");
            msg.Append(_nonce);
            Logger.DebugFormat(this, "Message: {0}", msg.ToString());

            Auth tag = (Auth)TagRegistry.Instance.GetTag("auth", Namespaces.SASL, new XmlDocument());
            tag.Mechanism = Mechanism.GetMechanism(MechanismType.SCRAM);
            tag.Bytes = _utf.GetBytes(msg.ToString());
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
            string response = _utf.GetString(c.Bytes);
            Logger.DebugFormat(this, "Challenge: {0}", response);

            // Split challenge into pieces
            string[] tokens = response.Split(',');

            // Ensure that the first length of nonce is the same nonce we sent.
            string r = tokens[0].Substring(2, _nonce.Length);
            if (0 != String.Compare(r, _nonce))
            {
                Logger.DebugFormat(this, "{0} does not match {1}", r, _nonce);
            }

            Logger.Debug(this, "Getting Salt");
            string a = tokens[1].Substring(2);
            _salt = Convert.FromBase64String(a);
            string b = _utf.GetString(_salt);
            Logger.DebugFormat(this, "Salt: {0}", b);

            Logger.Debug(this, "Getting Iterations");
            string i = tokens[2].Substring(2);
            _i = int.Parse(i);
            Logger.DebugFormat(this, "Iterations: {0}", _i);

            CalculateProofs();

            return TagRegistry.Instance.GetTag("response", Namespaces.SASL, new XmlDocument());
        }

        private void CalculateProofs()
        {
            string salted_password = Hi();
            Logger.DebugFormat(this, "Salted Password: {0}", salted_password);
        }

        private string Hi()
        {
            HMACSHA1 hmac;
            byte[] prev = new byte[20];
            byte[] temp = new byte[20];
            byte[] result = new byte[20];
            byte[] password = _utf.GetBytes(_password);

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

            return _utf.GetString(result);
        }
    }
}
