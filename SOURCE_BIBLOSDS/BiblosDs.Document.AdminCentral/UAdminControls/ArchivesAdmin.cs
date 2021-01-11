using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class ArchivesAdmin : BaseAdmin
    {
        public ArchivesAdmin()
        {
            InitializeComponent();
        }

        private void ArchivesAdmin_Load(object sender, EventArgs e)
        {
            try
            {
                _panel = _pnl;
                ucShowControl("ucarchive", null);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }            
        }
    }
}
