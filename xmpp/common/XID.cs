// XID.cs
//
//XMPP .NET Library Copyright (C) 2006, 2008, 2009 Dieter Lunn
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
using System.Text;
using System.Text.RegularExpressions;

using System.Diagnostics;

using Gnu.Inet.Encoding;

namespace ubiety.common
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
        [Obsolete]
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

		#region {{ Properties }}
		/// <summary>
		/// String representation of the id.
		/// </summary>
		public string XmppID
		{
			get { return (_xid == null) ? build_xid() : _xid; }
			set { parse(value); }
		}

		/// <summary>
		/// Username of the user.
		/// </summary>
		public string User
		{
			get { return _user; }
			set 
			{
				string tmp = Escape(value);
				_user = Stringprep.NodePrep(tmp);
			}
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
		#endregion

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

		#region {{ Operators }}
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

		#region {{ Build and Parse functions }}
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
		private void parse(string id)
		{
			int at = id.IndexOf('@');
			int slash = id.IndexOf('/');

			if (at == -1)
			{
				if (slash == -1)
				{
					Server = id;
				}
				else
				{
					Server = id.Substring(0, slash);
					Resource = id.Substring(slash + 1);
				}
			}
			else
			{
				if (slash == -1)
				{
					User = id.Substring(0, at);
					Server = id.Substring(at + 1);
				}
				else
				{
					if (at < slash)
					{
						User = id.Substring(0, at);
						Server = id.Substring(at + 1, slash - at - 1);
						Resource = id.Substring(slash + 1);
					}
					else
					{
						Server = id.Substring(0, slash);
						Resource = id.Substring(slash + 1);
					}
				}
			}
		}
		#endregion
		
		#region {{ XEP-0106 JID Escaping }}
		private string Escape(string user)
		{
			StringBuilder u = new StringBuilder();
			int count = 0;
			
			foreach (char c in user)
			{
				switch (c)
				{
					case ' ':
						if ((count == 0) || (count == (user.Length - 1)))
							throw new FormatException("Username cannot start or end with a space");
						u.Append(@"\20");
						break;
					case '"':
						u.Append(@"\22");
						break;
					case '&':
						u.Append(@"\26");
						break;
					case '\'':
						u.Append(@"\27");
						break;
					case '/':
						u.Append(@"\2f");
						break;
					case ':':
						u.Append(@"\3a");
						break;
					case '<':
						u.Append(@"\3c");
						break;
					case '>':
						u.Append(@"\3e");
						break;
					case '@':
						u.Append(@"\40");
						break;
					case '\\':
						u.Append(@"\5c");
						break;
					default:
						u.Append(c);
						break;
				}
				count++;
			}
			
			return u.ToString();
		}
		
		private string Unescape()
		{
			Regex re = new Regex(@"\\([2-5][0267face])");
			string u = re.Replace(_user, new MatchEvaluator(delegate(Match m) 
				{
					switch (m.Groups[1].Value)
					{
						case "20":
							return " ";
						case "22":
							return "\"";
						case "26":
							return "&";
						case "27":
							return "'";
						case "2f":
							return "/";
						case "3a":
							return ":";
						case "3c":
							return "<";
						case "3e":
							return ">";
						case "40":
							return "@";
						case "5c":
							return @"\";
						default:
							return m.Groups[0].Value;
					}
				}));
				
			return u;
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
