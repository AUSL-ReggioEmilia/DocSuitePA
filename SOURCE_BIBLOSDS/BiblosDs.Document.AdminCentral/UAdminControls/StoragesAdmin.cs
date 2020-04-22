using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using BiblosDs.Document.AdminCentral.UAdminControls;

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class StoragesAdmin : BaseAdmin
    {
        public StoragesAdmin()
        {
            InitializeComponent();
        }

        private void StoragesAdmin_Load(object sender, EventArgs e)
        {
            try
            {
                _panel = _pnl;
                ucShowControl("ucstorages", null);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }            
        }
    }
}
