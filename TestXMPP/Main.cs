using System;
using System.Windows.Forms;
using ubiety;
using ubiety.common;
using ubiety.registries;
using System.Reflection;

namespace TestXMPP
{
	public partial class Main : Form
	{
		XMPP xmpp;

		public Main()
		{
			InitializeComponent();
            //CompressionRegistry.Instance.AddCompression(Assembly.LoadFile(Application.StartupPath + @"\ubiety.compression.sharpziplib.dll"));
            Errors.Instance.OnError += new EventHandler<ErrorEventArgs>(Errors_OnError);
			slVersion.Text = "Ubiety Version: " + XMPP.Version;
		}

        void Errors_OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message, "Protocol Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

		private void button1_Click(object sender, EventArgs e)
		{
            XID id = new XID(txtUsername.Text);
			xmpp = XMPP.Connect(id, txtPassword.Text, null, 5222, btnSSL.Checked);
		}

        private void btnExit_Click(object sender, EventArgs e)
        {
			xmpp.Disconnect();
            Application.Exit();
        }
	}
}