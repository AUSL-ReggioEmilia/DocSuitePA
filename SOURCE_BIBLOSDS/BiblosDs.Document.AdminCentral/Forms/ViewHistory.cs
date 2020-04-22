using System;
using System.ComponentModel;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;

namespace BiblosDs.Document.AdminCentral.Forms
{
    public partial class ViewHistory : Form
    {
        public Guid IdDocument { get; set; }
        BindingList<ServiceReferenceDocument.Document> documents;
        public ViewHistory()
        {
            InitializeComponent();
        }

        public ViewHistory(Guid IdDocument)
        {
            InitializeComponent();
            this.IdDocument = IdDocument;
        }

        private void ViewHistory_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync(IdDocument);
            this.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (DocumentsClient client = new DocumentsClient())
            {
                documents = client.GetChainInfoDetails((Guid)e.Argument, null,  null, null);
            }  
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else
                radGridView1.DataSource = documents;
            if (documents.Count <= 0)
                MessageBox.Show("Nessuna catena trovata con l'id specificato.");
            //
            this.Enabled = true;
        }        
    }
}
