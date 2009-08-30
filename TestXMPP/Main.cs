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
			xmpp = new XMPP();
            //CompressionRegistry.Instance.AddCompression(Assembly.LoadFile(Application.StartupPath + @"\ubiety.compression.sharpziplib.dll"));
            slVersion.Text = "Ubiety Version: " + XMPP.Version;
            Errors.Instance.OnError += new EventHandler<ErrorEventArgs>(Errors_OnError);
		}

        void Errors_OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message, "Protocol Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

		private void button1_Click(object sender, EventArgs e)
		{
            XID id = new XID(txtUsername.Text);
            xmpp.SSL = btnSSL.Checked;
			xmpp.ID = id;
			xmpp.Password = txtPassword.Text;
			xmpp.Connect();
		}

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
	}
}