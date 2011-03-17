// Settings.cs
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

using ubiety.core;
using ubiety.common;

namespace ubiety
{
    public static class Settings
    {
        private static bool _ssl = false;
        private static int _port = 5222;

        public static MechanismType AuthenticationTypes
        {
            get;
            set;
        }

        public static XID ID
        {
            get;
            set;
        }

        public static string Password
        {
            get;
            set;
        }

        public static string Hostname
        {
            get;
            set;
        }

        public static int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public static bool SSL
        {
            get { return _ssl; }
            set { _ssl = value; }
        }
    }
}
