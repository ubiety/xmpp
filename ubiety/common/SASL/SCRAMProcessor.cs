// SCRAMProcessor.cs
//
//Ubiety XMPP Library Copyright (C) 2011 - 2015 Dieter Lunn
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
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Gnu.Inet.Encoding;
using Ubiety.Core;
using Ubiety.Core.Sasl;
using Ubiety.Registries;

namespace Ubiety.Common.Sasl
{
    /// <summary>
    /// </summary>
    public class ScramProcessor : SaslProcessor
    {
        private readonly Encoding _utf = Encoding.UTF8;
        private string _clientFinal;
        private string _clientFirst;
        private string _clientProof;
        private int _i;

        private string _nonce;
        private byte[] _salt;
        private byte[] _serverFirst;

        private byte[] _serverSignature;
        private string _snonce;

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override Tag Initialize(String id, String password)
        {
            base.Initialize(id, password);

            _nonce = NextInt64().ToString(CultureInfo.InvariantCulture);
            var msg = new StringBuilder();
            msg.Append("n,,n=");
            msg.Append(Id.User);
            msg.Append(",r=");
            msg.Append(_nonce);

            _clientFirst = msg.ToString().Substring(3);

            var tag = TagRegistry.GetTag<Auth>("auth", Namespaces.Sasl);
            tag.Mechanism = Mechanism.GetMechanism(MechanismType.Scram);
            tag.Bytes = _utf.GetBytes(msg.ToString());
            return tag;
        }

        /// <summary>
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public override Tag Step(Tag tag)
        {
            switch (tag.Name)
            {
                case "challenge":
                {
                    _serverFirst = tag.Bytes;
                    string response = _utf.GetString(tag.Bytes);

                    // Split challenge into pieces
                    string[] tokens = response.Split(',');

                    _snonce = tokens[0].Substring(2);
                    // Ensure that the first length of nonce is the same nonce we sent.
                    string r = _snonce.Substring(0, _nonce.Length);
                    if (0 != String.CompareOrdinal(r, _nonce))
                    {
                        throw new Exception("Error in authenticating server nonce.");
                    }

                    string a = tokens[1].Substring(2);
                    _salt = Convert.FromBase64String(a);

                    string i = tokens[2].Substring(2);
                    _i = int.Parse(i);

                    var final = new StringBuilder();
                    final.Append("c=biws,r=");
                    final.Append(_snonce);

                    _clientFinal = final.ToString();

                    CalculateProofs();

                    final.Append(",p=");
                    final.Append(_clientProof);

                    var resp = TagRegistry.GetTag<GenericTag>("response", Namespaces.Sasl);
                    resp.Bytes = _utf.GetBytes(final.ToString());

                    return resp;
                }

                case "success":
                {
                    string response = _utf.GetString(tag.Bytes);
                    byte[] signature = Convert.FromBase64String(response.Substring(2));
                    return _utf.GetString(signature) == _utf.GetString(_serverSignature) ? tag : null;
                }
                case "failure":
                    return tag;
            }

            return null;
        }

        private void CalculateProofs()
        {
            var hmac = new HMACSHA1();
            SHA1 hash = new SHA1CryptoServiceProvider();

            byte[] saltedPassword = Hi();

            // Calculate Client Key
            hmac.Key = saltedPassword;
            byte[] clientKey = hmac.ComputeHash(_utf.GetBytes("Client Key"));

            // Calculate Server Key
            byte[] serverKey = hmac.ComputeHash(_utf.GetBytes("Server Key"));

            // Calculate Stored Key
            byte[] storedKey = hash.ComputeHash(clientKey);

            var a = new StringBuilder();
            a.Append(_clientFirst);
            a.Append(",");
            a.Append(_utf.GetString(_serverFirst));
            a.Append(",");
            a.Append(_clientFinal);

            byte[] auth = _utf.GetBytes(a.ToString());

            // Calculate Client Signature
            hmac.Key = storedKey;
            byte[] signature = hmac.ComputeHash(auth);

            // Calculate Server Signature
            hmac.Key = serverKey;
            _serverSignature = hmac.ComputeHash(auth);

            // Calculate Client Proof
            var proof = new byte[20];
            for (int i = 0; i < signature.Length; ++i)
            {
                proof[i] = (byte) (clientKey[i] ^ signature[i]);
            }

            _clientProof = Convert.ToBase64String(proof);
        }

        private byte[] Hi()
        {
            var prev = new byte[20];
            byte[] password = _utf.GetBytes(Stringprep.SASLPrep(Password));

            // Add 1 to the end of salt with most significat octet first
            var key = new byte[_salt.Length + 4];

            Array.Copy(_salt, key, _salt.Length);
            byte[] g = {0, 0, 0, 1};
            Array.Copy(g, 0, key, _salt.Length, 4);

            // Compute initial hash
            var hmac = new HMACSHA1(password);
            byte[] result = hmac.ComputeHash(key);
            Array.Copy(result, prev, result.Length);

            for (int i = 1; i < _i; ++i)
            {
                byte[] temp = hmac.ComputeHash(prev);
                for (int j = 0; j < temp.Length; ++j)
                {
                    result[j] ^= temp[j];
                }

                Array.Copy(temp, prev, temp.Length);
            }

            return result;
        }
    }
}