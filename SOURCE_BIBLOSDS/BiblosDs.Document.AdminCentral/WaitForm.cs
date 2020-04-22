using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BiblosDs.Document.AdminCentral
{
    public partial class WaitForm : Form
    {
        public WaitForm()
        {
            InitializeComponent();
        }

        private void WaitForm_Load(object sender, EventArgs e)
        {
            radWaitingBar1.StartWaiting();
        }

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			radWaitingBar1.EndWaiting();
		}
    }
}