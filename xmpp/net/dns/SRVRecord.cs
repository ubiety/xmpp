// SRVRecord.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2009 Dieter Lunn
// 
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// 
// You should have received a copy of the GNU Lesser General Public License along
// with this library; if not, write to the Free Software Foundation, Inc., 59
// Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

namespace ubiety.net.dns
{
	/// <summary>
	/// Summary description for SRVRecord.
	/// </summary>
   public class SRVRecord : RecordBase, IComparable
   {
       /// <summary>
       /// Constructs a NS record by reading bytes from a return message
       /// </summary>
       /// <param name="pointer">A logical pointer to the bytes holding the record</param>
       internal SRVRecord(Pointer pointer)
       {
           _Priority = pointer.ReadShort();
           _Weight = pointer.ReadShort();
           _Port = pointer.ReadShort();
           _Target = pointer.ReadDomain();
       }
           
        // the fields exposed outside the assembly
        private int     _Priority;
        private int     _Weight;
        private int     _Port;
        private string  _Target;

        public int Priority	
        { 
          get { return _Priority; } 
        }

        public int Weight	
        { 
          get { return _Weight; } 
        }

        public int Port
        { 
          get { return _Port; } 
        }
      
        public string Target	
        { 
            get { return _Target; } 
        }
				
        public override string ToString()
        {
			return string.Format("\n   priority   = {0}\n   weight     = {1}\n   port       = {2}\n   target     = {3}",
            _Priority,
            _Weight,
            _Port,
            _Target);
        }

        /// <summary>
        /// Implements the IComparable interface so that we can sort the SRV records by their
        /// lowest priority
        /// </summary>
        /// <param name="other">the other SRVRecord to compare against</param>
        /// <returns>1, 0, -1</returns>
        public int CompareTo(object obj)
        {
            SRVRecord srvOther = (SRVRecord)obj;

            // we want to be able to sort them by priority from lowest to highest.
            if (_Priority < srvOther._Priority) return -1;
            if (_Priority > srvOther._Priority) return 1;

            // if the priority is the same, sort by highest weight to lowest (higher
            // weighting means that server should get more of the requests)
            if (_Weight > srvOther._Weight) return -1;
            if (_Weight < srvOther._Weight) return 1;

            return 0;
        }
    }
}