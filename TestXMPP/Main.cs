using System;
using System.Windows.Forms;
using xmpp;
using xmpp.common;
using xmpp.registries;
using System.Reflection;

namespace TestXMPP
{
	public partial class Main : Form
	{
		XMPP xmpp;

		public Main()
		{
			InitializeComponent();
			xmpp = new XMPP();
            CompressionRegistry.Instance.AddCompression(Assembly.LoadFile(Application.StartupPath + @"\xmpp.compression.sharpziplib.dll"));
		}

		private void button1_Click(object sender, EventArgs e)
		{
            XID id = new XID(txtID.Text);
            xmpp.SSL = cbSSL.Checked;
			xmpp.ID = id;
			xmpp.Password = txtPassword.Text;
			xmpp.Connect();
		}
	}
}