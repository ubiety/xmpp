using System;
using System.Windows.Forms;
using TestXMPP.Properties;
using Ubiety;
using Ubiety.Common;
using Ubiety.Infrastructure;

namespace TestXMPP
{
    public partial class Main : Form
    {
        private XmppState _xmppState;

		public Main()
		{
			InitializeComponent();
			slVersion.Text = Resources.Version_Label + Xmpp.Version;
		}

		private void Button1Click(object sender, EventArgs e)
		{
		    _xmppState = Xmpp.Connect(new JID(txtUsername.Text), txtPassword.Text);
		    _xmppState.Events.Error += EventsOnError;
		}

        private static void EventsOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            MessageBox.Show(errorEventArgs.Message, Resources.Error_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnExitClick(object sender, EventArgs e)
		{
			if (_xmpp != null && Xmpp.Connected)
				_xmpp.Disconnect();
			Application.Exit();
		}

		private void BtnSendClick(object sender, EventArgs e)
		{
            //UbietyMessages.SendMessage(txtMessage.Text);
		}
	}
}