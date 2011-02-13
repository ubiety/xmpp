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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override Tag Initialize(XID id, string password)
        {
            base.Initialize(id, password);
            Logger.Debug(this, "Intializing SCRAM Processor");
            Auth tag = (Auth)TagRegistry.Instance.GetTag("auth", Namespaces.SASL, new XmlDocument());
            tag.Mechanism = Mechanism.GetMechanism(MechanismType.SCRAM);
            return tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public override Tag Step(Tag tag)
        {
            throw new NotImplementedException();
        }
    }
}
