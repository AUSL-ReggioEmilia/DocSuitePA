using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;
using BiblosDs.Document.AdminCentral.UControls;
using BiblosDS.UI.ConsoleTest.UControls;
using BiblosDs.Document.AdminCentral.UAdminControls;
using BiblosDs.Document.AdminCentral.UControls.AlignmentWizard;

namespace BiblosDs.Document.AdminCentral
{
    public partial class Form1 : Telerik.WinControls.UI.RadRibbonForm
    {
        private WaitForm waitForm = new WaitForm();
        public Form1()
        {            
            waitForm.Show();
            InitializeComponent();           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            waitForm.Close();
        }

        private void radImageButtonElement1_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new StoragesAdmin { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radImageButtonElement2_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new ArchivesAdmin { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radImageButtonElement3_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new UcFileUploader { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radImageButtonElement4_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new UcFileViewer { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radImageButtonElement5_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new UcTransit { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radImageButtonElement6_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new UcFindFile { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radMenuButtonItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radImageButtonElement7_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new UcLoadFileOnStorage { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radImageButtonElement8_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new LogsAdmin { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radButtonElement1_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            var step = new Step1 { Dock = DockStyle.Fill };
            panel1.Controls.Add(step);
            step.Show(panel1);
            waitForm.Close();
        }

        private void radImageButtonElement9_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new ServerAdmin { Dock = DockStyle.Fill });
            waitForm.Close();
        }

        private void radImageButtonElement10_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            panel1.Controls.Clear();
            panel1.Controls.Add(new UcConfigView { Dock = DockStyle.Fill });
            waitForm.Close();
        }        
    }
}