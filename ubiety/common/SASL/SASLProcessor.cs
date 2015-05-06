// SASLProcessor.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2015 Dieter Lunn
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
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Ubiety.Core;

namespace Ubiety.Common.Sasl
{
    /// <summary>
    /// </summary>
    public abstract class SaslProcessor
    {
        private readonly Hashtable _directives = new Hashtable();

        /// <summary>
        /// </summary>
        protected Jid Id;

        /// <summary>
        /// </summary>
        protected string Password;

        /// <summary>
        /// </summary>
        /// <param name="directive"></param>
        /// <returns></returns>
        protected string this[string directive]
        {
            get { return (string) _directives[directive]; }
            set { _directives[directive] = value; }
        }

        /// <summary>
        ///     Creates the appropriate authentication processor based on the types supported by the server and client.
        /// </summary>
        /// <param name="serverTypes">Authentication methods supported by the server</param>
        /// <param name="clientTypes">Authentication methods supported by the client</param>
        /// <returns>Authentication processor that is common between client and server.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public static SaslProcessor CreateProcessor(MechanismType serverTypes, MechanismType clientTypes)
        {
            if ((serverTypes & MechanismType.External & clientTypes) == MechanismType.External)
            {
                throw new NotSupportedException();
            }

            if ((serverTypes & MechanismType.Scram & clientTypes) == MechanismType.Scram)
            {
                return new ScramProcessor();
            }

            if ((serverTypes & MechanismType.DigestMd5 & clientTypes) == MechanismType.DigestMd5)
            {
                return new Md5Processor();
            }

            if ((serverTypes & MechanismType.Plain & clientTypes) == MechanismType.Plain)
            {
                return new PlainProcessor();
            }

            return null;
        }

        /// <summary>
        /// </summary>
        public abstract Tag Step(Tag tag);

        /// <summary>
        /// </summary>
        public virtual Tag Initialize(string id, string password)
        {
            Id = id;
            Password = password;

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        protected string HexString(IEnumerable<byte> buff)
        {
            var sb = new StringBuilder();
            foreach (byte b in buff)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Generates a new random 64bit number
        /// </summary>
        /// <returns>Random Int64</returns>
        protected static long NextInt64()
        {
            var bytes = new byte[sizeof (long)];
            var rand = new RNGCryptoServiceProvider();
            rand.GetBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}