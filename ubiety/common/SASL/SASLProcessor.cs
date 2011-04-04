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
using System.Text;
using System.Security.Cryptography;

namespace ubiety.common.SASL
{
	///<summary>
    ///</summary>
    public abstract class SASLProcessor
	{
		protected XID Id;
		protected string Password;
		
		private readonly Hashtable _directives = new Hashtable();

        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        ///<exception cref="NotSupportedException"></exception>
        public static SASLProcessor CreateProcessor(MechanismType type)
        {
            if ((type & MechanismType.External & Settings.AuthenticationTypes) == MechanismType.External)
			{
				Logger.Debug(typeof(SASLProcessor), "External Not Supported");
                throw new NotSupportedException();
            }

            if ((type & MechanismType.SCRAM & Settings.AuthenticationTypes) == MechanismType.SCRAM)
            {
                Logger.Debug(typeof(SASLProcessor), "Creating SCRAM-SHA-1 Processor");
                return new SCRAMProcessor();
            }

            if ((type & MechanismType.DigestMD5 & Settings.AuthenticationTypes) == MechanismType.DigestMD5)
			{
				Logger.Debug(typeof(SASLProcessor), "Creating DIGEST-MD5 Processor");
				return new MD5Processor();
            }

            if ((type & MechanismType.Plain & Settings.AuthenticationTypes) == MechanismType.Plain)
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
    	public virtual Tag Initialize()
		{
			Logger.Debug(this, "Initializing Base Processor");
			
			Id = Settings.Id;
			Password = Settings.Password;

			return null;
		}
		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directive"></param>
        /// <returns></returns>
		public string this[string directive]
		{
			get { return (string)_directives[directive]; }
			set { _directives[directive] = value; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        protected string HexString(byte[] buff)
        {
            var sb = new StringBuilder();
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
            var rand = new RNGCryptoServiceProvider();
            rand.GetBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
