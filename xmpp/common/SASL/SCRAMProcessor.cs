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
            Logger.DebugFormat(this, "Challenge: {0}", _utf.GetString(c.Bytes));
            return null;
        }
    }
}
