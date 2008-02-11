// project created on 12/10/2007 at 12:14 AM
using System;
using Common;
using xmpp;
using xmpp.common;
using System.Collections;
using System.Collections.Specialized;

namespace testconsole
{
	class MainClass
	{
		[AutoSetMember("id")]
		private static string _id;
		
		[AutoSetMember("password")]
		private static string _password;
		
		[AutoSetMember("ssl")]
		private static bool _ssl = false;
		
		[STAThread]
		public static void Main(string[] args)
		{
			ArgumentParser parser = new ArgumentParser(ArgumentFormats.NamedValue, true);
			StringDictionary sargs = parser.Parse(args);
			
			parser.AutoSetMembers(typeof(MainClass));
			
			XID id = new XID(_id);
			XMPP xm = new XMPP();
			
			xm.ID = id;
			xm.Password = _password;
			xm.SSL = _ssl;
			xm.Connect();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
		}
	}
}