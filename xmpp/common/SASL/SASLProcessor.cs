//XMPP .NET Library Copyright (C) 2006, 2007 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using xmpp.core;
using xmpp.logging;

namespace xmpp.common.SASL
{
    ///<summary>
    ///</summary>
    public abstract class SASLProcessor
	{
		protected XID _id;
		protected string _password;

        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        ///<exception cref="NotSupportedException"></exception>
        public static SASLProcessor CreateProcessor(MechanismType type)
        {
            if ((type & MechanismType.EXTERNAL) == MechanismType.EXTERNAL)
				{
				Logger.Debug(typeof(SASLProcessor), "External Not Supported");
                //throw new NotSupportedException();
            }

            if ((type & MechanismType.DIGEST_MD5) == MechanismType.DIGEST_MD5)
				{
				Logger.Debug(typeof(SASLProcessor), "DIGEST-MD5 Not Supported");
                //throw new NotSupportedException();
            }

            if ((type & MechanismType.PLAIN) == MechanismType.PLAIN)
			{
				Logger.Debug(typeof(SASLProcessor), "Creating PLAIN SASL processor");
            	return new PlainProcessor();
            }
            return null;
        }

        ///<summary>
        ///</summary>
        public abstract void Step(Tag tag);

    	///<summary>
    	///</summary>
    	public virtual Tag Initialize(XID id, string password)
		{
			Logger.Debug(this, "Initializing Base Processor");
			
			_id = id;
			_password = password;

    		return null;
    	}
    }
}
