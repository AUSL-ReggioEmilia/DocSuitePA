using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;
using BiblosDs.Document.AdminCentral.ServiceReferenceContentSearch;
using System.ServiceModel;

namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    public partial class Step1 : WizardStepBase
    {
        private const string TITOLO = "Selezione connessioni";

        public static DocumentsClient DocumentServiceSourceClient { get; private set; }
        public static DocumentsClient DocumentServiceDestinationClient { get; private set; }

        public static ContentSearchClient ContentSearchServiceSourceClient { get; private set; }
        public static ContentSearchClient ContentSearchServiceDestinationClient { get; private set; }

        private const string DOCUMENT_SERVICE_NAME = "Binding_Documents", CONTENT_SEARCH_NAME = "ContentSearch";

        public Step1()
        {
            this.InitializeComponent();
            base.Initialize(TITOLO, null, new Step2(this));
        }

        public override void Show(Control parentControl)
        {
            base.Show(parentControl, this);
        }

        public override void GotoNextStep()
        {
            this.UseWaitCursor = true;
            this.Enabled = false;

            Application.DoEvents();

            try
            {
                if (this.canProceed())
                {
                    Step1.DocumentServiceSourceClient = new DocumentsClient(DOCUMENT_SERVICE_NAME, this.getServiceEndpointAddress(txtUrlSource.Text, true));
                    Step1.DocumentServiceDestinationClient = new DocumentsClient(DOCUMENT_SERVICE_NAME, this.getServiceEndpointAddress(txtUrlDest.Text, true));

                    Step1.ContentSearchServiceSourceClient = new ContentSearchClient(CONTENT_SEARCH_NAME, this.getServiceEndpointAddress(txtUrlSource.Text, false));
                    Step1.ContentSearchServiceDestinationClient = new ContentSearchClient(CONTENT_SEARCH_NAME, this.getServiceEndpointAddress(txtUrlDest.Text, false));

                    this.UseWaitCursor = false;
                    this.Enabled = true;

                    base.GotoNextStep();
                }
            }
            catch (Exception ex)
            {
                this.UseWaitCursor = false;
                this.Enabled = true;
                throw;
            }
        }

        private EndpointAddress getServiceEndpointAddress(string partialUrl, bool isDocumentService)
        {
            partialUrl = (partialUrl ?? string.Empty).Trim().TrimEnd('/');
            partialUrl = string.Format("{0}/{1}", partialUrl, (isDocumentService) ? "Documents.svc" : "ContentSearch.svc");
            return new EndpointAddress(partialUrl);
        }

        private void btnVerifySource_Click(object sender, EventArgs e)
        {
            if (this.verifyConnection(this.txtUrlSource.Text))
            {
                MessageBox.Show(this, "Connessione stabilita con successo.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnVerifyDest_Click(object sender, EventArgs e)
        {
            if (this.verifyConnection(this.txtUrlDest.Text))
            {
                MessageBox.Show(this, "Connessione stabilita con successo.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool verifyConnection(string url)
        {
            bool isOk = false;
            try
            {
                using (var svc = new DocumentsClient(DOCUMENT_SERVICE_NAME, this.getServiceEndpointAddress(url, true)))
                {
                    isOk = svc.IsAlive();
                }

                if (isOk)
                {
                    using (var svc = new ContentSearchClient(CONTENT_SEARCH_NAME, this.getServiceEndpointAddress(url, false)))
                    {
                        isOk = svc.IsAlive();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Impossibile stabilire una connessione.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isOk;
        }

        private bool canProceed()
        {
            bool ok = false;
            if (string.IsNullOrWhiteSpace(txtUrlSource.Text))
            {
                MessageBox.Show(this, "URL sorgente non impostato.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else if (string.IsNullOrEmpty(txtUrlDest.Text))
            {
                MessageBox.Show(this, "URL di destinazione non impostato.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                ok = this.verifyConnection(this.txtUrlSource.Text) && this.verifyConnection(this.txtUrlDest.Text);
            }

            return ok;
        }
    }
}
