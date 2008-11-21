// XID.cs
//
//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
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
using System.Text;

using Gnu.Inet.Encoding;

namespace xmpp.common
{
	/// <summary>
	/// Manages all aspects of a users identity on an XMPP network.
	/// </summary>
	public class XID : IComparable
	{
		private string _xid = null;
		private string _user = null;
		private string _resource = null;
		private string _server = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xid"></param>
		public XID(string xid)
		{
			XmppID = xid;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="server"></param>
        /// <param name="resource"></param>
		public XID(string user, string server, string resource)
		{
			User = user;
			Server = server;
			Resource = resource;
		}

		/// <summary>
		/// String representation of the id.
		/// </summary>
		public string XmppID
		{
			get { return (_xid == null) ? build_xid() : _xid; }
			set { 
					_xid = value;
					parse(value);
				}
		}

		/// <summary>
		/// Username of the user.
		/// </summary>
		public string User
		{
			get { return _user; }
			set { _user = (value == null) ? null : Stringprep.NodePrep(value); }
		}

		/// <summary>
		/// Server the user is logged into.
		/// </summary>
		public string Server
		{
			get { return _server; }
			set { _server = (value == null) ? null : Stringprep.NamePrep(value); }
		}

		/// <summary>
		/// Resource the user is communicating from.
		/// </summary>
		public string Resource
		{
			get { return _resource; }
			set { _resource = (value == null) ? null : Stringprep.ResourcePrep(value); }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public override int GetHashCode()
		{
			return _xid.GetHashCode();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return XmppID;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is string)
			{
				return XmppID.Equals(obj);
			}
			if (!(obj is XID))
			{
				return false;
			}

			return XmppID.Equals(((XID)obj).XmppID);
		}

		#region Operators
        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
		public static bool operator ==(XID one, XID two)
		{
			if ((object)one == null)
			{
				return ((object)two == null);
			}

			return one.Equals(two);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
		public static bool operator ==(string one, XID two)
		{
			if ((object)two == null)
			{
				return ((object)one == null);
			}

			return two.Equals(one);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
		public static bool operator !=(XID one, XID two)
		{
			if ((object)one == null)
			{
				return ((object)two != null);
			}

			return !one.Equals(two);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
		public static bool operator !=(string one, XID two)
		{
			if ((object)two == null)
			{
				return ((object)one != null);
			}

			return !two.Equals(one);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <returns></returns>
		public static implicit operator XID(string one)
		{
			return new XID(one);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <returns></returns>
		public static implicit operator string(XID one)
		{
			return one.XmppID;
		}
		#endregion

		#region Build and Parse functions
		/// <summary>
		/// Builds a string version of an XID from the three parts.
		/// </summary>
		/// <returns>string version of xid</returns>
		private string build_xid()
		{
			StringBuilder sb = new StringBuilder();
			if (_user != null)
			{
				sb.Append(_user);
				sb.Append("@");
			}
			sb.Append(_server);
			if (_resource != null)
			{
				sb.Append("/");
				sb.Append(_resource);
			}

			_xid = sb.ToString();
			return _xid;
		}

		/// <summary>
		/// Takes a string xid and breaks it into its parts.
		/// </summary>
		/// <param name="value">xid to parse</param>
		private void parse(string value)
		{
			int at = value.IndexOf('@');
			int slash = value.IndexOf('/');

			if (at == -1)
			{
				if (slash == -1)
				{
					Server = value;
				}
				else
				{
					Server = value.Substring(0, slash);
					Resource = value.Substring(slash + 1);
				}
			}
			else
			{
				if (slash == -1)
				{
					User = value.Substring(0, at);
					Server = value.Substring(at + 1);
				}
				else
				{
					if (at < slash)
					{
						User = value.Substring(0, at);
						Server = value.Substring(at + 1, slash - at - 1);
						Resource = value.Substring(slash + 1);
					}
					else
					{
						Server = value.Substring(0, slash);
						Resource = value.Substring(slash + 1);
					}
				}
			}
		}
		#endregion

		#region IComparable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public int CompareTo(object obj)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
