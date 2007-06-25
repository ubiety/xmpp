using System;
using System.Windows.Forms;
using xmpp;
using xmpp.common;

namespace TestXMPP
{
	public partial class Main : Form
	{
		XMPP xmpp;

		public Main()
		{
			InitializeComponent();
			xmpp = new XMPP();
		}

		private void button1_Click(object sender, EventArgs e)
		{
            XID id = new XID("coder2000@127.0.0.1/roller");
            xmpp.SSL = cbSSL.Checked;
			xmpp.ID = id;
			xmpp.Password = "loki";
			xmpp.Connect();
		}
	}
}