// SASLProcessor.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2009 Dieter Lunn
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
using ubiety.core;
using ubiety.logging;
using System.Text;
using System.Security.Cryptography;
using ubiety;

namespace ubiety.common.SASL
{
	///<summary>
    ///</summary>
    public abstract class SASLProcessor
	{
		protected XID _id;
		protected string _password;
		
		private Hashtable directives = new Hashtable();

        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        ///<exception cref="NotSupportedException"></exception>
        public static SASLProcessor CreateProcessor(MechanismType type)
        {
            if ((type & MechanismType.EXTERNAL & Settings.AuthenticationTypes) == MechanismType.EXTERNAL)
			{
				Logger.Debug(typeof(SASLProcessor), "External Not Supported");
                throw new NotSupportedException();
            }

            if ((type & MechanismType.SCRAM & Settings.AuthenticationTypes) == MechanismType.SCRAM)
            {
                Logger.Debug(typeof(SASLProcessor), "Creating SCRAM-SHA-1 Processor");
                return new SCRAMProcessor();
            }

            if ((type & MechanismType.DIGEST_MD5 & Settings.AuthenticationTypes) == MechanismType.DIGEST_MD5)
			{
				Logger.Debug(typeof(SASLProcessor), "Creating DIGEST-MD5 Processor");
				return new MD5Processor();
            }

            if ((type & MechanismType.PLAIN & Settings.AuthenticationTypes) == MechanismType.PLAIN)
			{
				Logger.Debug(typeof(SASLProcessor), "Creating PLAIN SASL processor");
            	return new PlainProcessor();
            }

            return null;
        }

        ///<summary>
        ///</summary>
        public abstract Tag Step(Tag tag);

    	///<summary>
    	///</summary>
    	public virtual Tag Initialize(XID id, string password)
		{
			Logger.Debug(this, "Initializing Base Processor");
			
			_id = id;
			_password = password;

			return null;
		}
		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directive"></param>
        /// <returns></returns>
		public string this[string directive]
		{
			get { return (string)directives[directive]; }
			set { directives[directive] = value; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        protected string HexString(byte[] buff)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in buff)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a new random 64bit number
        /// </summary>
        /// <returns>Random Int64</returns>
        protected static Int64 NextInt64()
        {
            var bytes = new byte[sizeof(Int64)];
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            rand.GetBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
