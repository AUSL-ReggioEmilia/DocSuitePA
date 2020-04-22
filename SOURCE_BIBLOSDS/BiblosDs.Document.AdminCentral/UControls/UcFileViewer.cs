using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using BiblosDs.Document.AdminCentral.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;
using BiblosDS.UI.ConsoleTest.UControls;
using BiblosDs.Document.AdminCentral.UAdminControls;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcFileViewer : BaseAdminControls
    {
        BindingList<ServiceReferenceDocument.Document> documents;
        RadContextMenu menu;
        RadContextMenu menuParent;
        WaitForm waitForm = new WaitForm();
        int documentInArchive;
        int documentInPage = 20;
        int currentPage = 0;
        public UcFileViewer()
        {
            InitializeComponent();
        }

        private void UcFileViewer_Load(object sender, EventArgs e)
        {
            menu = new RadContextMenu();
            RadMenuItem documentoView = new RadMenuItem();
            documentoView.Text = "Visualizza documento";
            documentoView.Click += new EventHandler(documentoView_Click);
            menu.Items.Add(documentoView);
            RadMenuItem documentoCheckOut = new RadMenuItem();
            documentoCheckOut.Text = "Check Out";
            documentoCheckOut.Click += new EventHandler(documentoCheckOut_Click);
            menu.Items.Add(documentoCheckOut);
            RadMenuItem documentoCheckIn = new RadMenuItem();
            documentoCheckIn.Text = "Check In";
            documentoCheckIn.Click += new EventHandler(documentoCheckIn_Click);
            menu.Items.Add(documentoCheckIn);
            RadMenuItem metadataView = new RadMenuItem();
            metadataView.Text = "Visualizza i metadati";
            metadataView.Click += new EventHandler(metadataView_Click);
            menu.Items.Add(metadataView);
            RadMenuItem signDocument = new RadMenuItem();
            signDocument.Text = "Sign";
            signDocument.Enabled = false;
            signDocument.Click += new EventHandler(signDocument_Click);
            menu.Items.Add(signDocument);
            RadMenuItem signExpireDate = new RadMenuItem();
            signExpireDate.Text = "Visualizza date scadenze certificati";            
            signExpireDate.Click += new EventHandler(signExpireDate_Click);
            menu.Items.Add(signExpireDate);
            menuParent = new RadContextMenu();
            RadMenuItem showHistory = new RadMenuItem();
            showHistory.Text = "Visualizza versioni";
            showHistory.Click += new EventHandler(showHistory_Click);
            menuParent.Items.Add(showHistory);

            this.ucPager.Initialize(1, 1);

            try
            {
                waitForm = new WaitForm();
                waitForm.Show();
                ServiceReferenceDocument.DocumentsClient client = new ServiceReferenceDocument.DocumentsClient("Binding_Documents");
                client.GetArchivesAsync();
                client.GetArchivesCompleted += delegate(object senderObj, GetArchivesCompletedEventArgs eRes)
                    {
                        waitForm.Close();

                        if (eRes.Error == null)
                        {
                            var result = (eRes.Result ?? new BindingList<Archive>())
                            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                            .OrderBy(x => x.Name)
                            .ToList();

                            radComboBoxArchive.DataSource = result;
                        }
                        else
                        {
                            MessageBox.Show(eRes.Error.Message);
                        }
                    };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        

        #region radGridView1 Events
     

        private void radGridView1_CurrentRowChanged()
        {
            try
            {
                waitForm = new WaitForm();
                waitForm.Show();
                ServiceReferenceContentSearch.ContentSearchClient client = new ServiceReferenceContentSearch.ContentSearchClient("ContentSearch");
                client.GetAllDocumentChainsCompleted += new EventHandler<ServiceReferenceContentSearch.GetAllDocumentChainsCompletedEventArgs>(client_GetAllDocumentChainsCompleted);
                client.GetAllDocumentChainsAsync(radComboBoxArchive.SelectedValue.ToString(), currentPage * documentInPage, documentInPage);                
            }
            catch (Exception ex)
            {
                waitForm.Close();
                RadMessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);                
            }            
            
        }

        void client_GetAllDocumentChainsCompleted(object sender, ServiceReferenceContentSearch.GetAllDocumentChainsCompletedEventArgs e)
        {
            waitForm.Close();
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LblMessage.Text = "Documents in archive: " + e.docunentsInArchiveCount + ", Current record from "+ currentPage * documentInPage + " to "+ ((currentPage * documentInPage) + documentInPage) + ".";

            double numPag = (double)e.docunentsInArchiveCount / (double)documentInPage;

            this.ucPager.Initialize(1, (int)Math.Ceiling(numPag), currentPage + 1);

            var result = e.Result;

            if (result != null)
                result = new BindingList<ServiceReferenceContentSearch.Document>(result.OrderBy(x => x.Name).ThenByDescending(x => x.Version).ThenByDescending(x => x.DateCreated).ToList());

            radGridView1.DataSource = result;
        }       

        #endregion

        #region radGridView2 Events

        private void radGridView2_ContextMenuOpening(object sender, Telerik.WinControls.UI.ContextMenuOpeningEventArgs e)
        {
            GridDataCellElement cell = e.ContextMenuProvider as GridDataCellElement;
            if ((bool)radGridView2.Rows[cell.RowIndex].Cells["IsCheckOut"].Value)
            {
                menu.Items[2].Enabled = true;
                menu.Items[1].Enabled = false;
            }
            else
            {
                menu.Items[2].Enabled = false;
                menu.Items[1].Enabled = true;
            }
            e.ContextMenu = menu.DropDown;
        }

        private void radGridView1_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            GridDataCellElement cell = e.ContextMenuProvider as GridDataCellElement;           
            e.ContextMenu = menuParent.DropDown;
        }

        void documentoView_Click(object sender, EventArgs e)
        {
            try
            {
                Content content = null;
                //e.CurrentRow.Cells["IdChain"].Value, e.CurrentRow.Cells["IdBiblos"].Value });            
                Guid id = (Guid)radGridView2.SelectedRows[0].Cells["IdDocument"].Value;
                // Metodo nuovo
                waitForm = new WaitForm();
                waitForm.Show();
                var client = new DocumentsClient("Binding_Documents");
                client.GetDocumentConformContentCompleted += (object o, GetDocumentConformContentCompletedEventArgs arg) =>
                    {
                        waitForm.Close();
                        if (arg.Error != null)
                            MessageBox.Show(arg.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            if (arg.Result.Blob == null)
                            {
                                MessageBox.Show("Nessun blob associato al documento.");
                                return;
                            }
                            var percorsoPdf = Path.Combine(Path.GetTempPath(), arg.Result.Description);
                            File.WriteAllBytes(percorsoPdf, arg.Result.Blob);
                            System.Diagnostics.Process.Start(percorsoPdf);    
                            //if (content != null)
                            //{
                            //    DialogResult res = MessageBox.Show("Yes Open, No Save file", "Open/Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            //    if (res == DialogResult.Yes)
                            //        new UcDocumentViewer(content.Blob).Show();
                            //    else if (res == DialogResult.No)
                            //    {
                            //        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            //            File.WriteAllBytes(saveFileDialog1.FileName, content.Blob);
                            //    }
                            //}
                        }
                    };
                string label = "<Label>"
                       + "  <Text>Protocollo 123 (pagina) di (pagine)</Text><Footer>Protocollo 123 (pagina) di (pagine)</Footer>"
                       + "  <Font Face=\"Arial\" Size=\"18\" Style=\"Bold,Italic\" />"
                       + "</Label>";
                client.GetDocumentConformContentAsync(id, ContentFormat.Binary, label, client);

                //BackgroundWorker bcw = new BackgroundWorker();
                //bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                //{
                //    using (var client = new DocumentsClient("Binding_Documents"))
                //    {
                //        //content = client.GetDocumentContent(id, null, ContentFormat.Bynary, null);
                //    }
                //};
                //bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                //{
                    
                //};
                //bcw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                waitForm.Close();
                MessageBox.Show(ex.Message);
            }
        }

        void storageChange_Click(object sender, EventArgs e)
        {
            
        }

        void signExpireDate_Click(object sender, EventArgs e)
        {
            try
            {
                string result = null;

                //    Content content;
                //    // Metodo nuovo
                BindingList<Certificate> certificates;
                using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                {
                    certificates = client.GetDocumentSigned((Guid)radGridView2.SelectedRows[0].Cells["IdDocument"].Value);
                }
                if (certificates != null)
                {

                    result = "<signatures>";
                    foreach (var crtlo in certificates)
                    {
                        result += "<signature level=\"" + crtlo.Level + "\" type=\"" + crtlo.Type
                            + "\" expiry=\"" + crtlo.DateExpiration
                            + "\" fiscalcode=\"" + crtlo.FiscalCode
                            + "\" validfrom=\"" + crtlo.DateValidFrom
                            + "\" name=\"" + "-" + "\""
                            + " issuer=\"" + crtlo.Issuer + "\""
                            + " email=\"" + crtlo.Email + "\""
                            + " role=\"" + crtlo.Role + "\"><![CDATA[" + crtlo.Description + "]]></signature>";

                    }
                    result += "</signatures>";
                }
                new UcDocumentViewer(result).Show();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
            }
        }

        void signDocument_Click(object sender, EventArgs e)
        {

           



        }


        void metadataView_Click(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            try
            {
                DocumentsClient client = new DocumentsClient("Binding_Documents");
                client.GetDocumentInfoCompleted += (object o, GetDocumentInfoCompletedEventArgs args) =>
                {
                    waitForm.Close();
                    if (args.Error != null)
                        MessageBox.Show(args.Error.ToString());
                    else
                    {

                        var doc = args.Result.Where(x => x.IdDocument == (Guid)args.UserState).FirstOrDefault();
                        if (doc != null)
                        {
                            Forms.MetaDataView metadata = new Forms.MetaDataView();
                            metadata.Attributes = doc.AttributeValues;
                            metadata.ShowDialog();
                        }
                        else
                            MessageBox.Show("Nessun documento presente con l'id cercato.");
                    }
                };
                client.GetDocumentInfoAsync((Guid)radGridView2.SelectedRows[0].Cells["IdDocument"].Value, null, true, (Guid)radGridView2.SelectedRows[0].Cells["IdDocument"].Value);
            }
            catch (Exception ex)
            {
                waitForm.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        void archiveSel_Click(object sender, EventArgs e)
        {
           
        }

        void documentoCheckOut_Click(object sender, EventArgs e)
        {
            try
            {
                var currentDoc = radGridView2.SelectedRows.First().DataBoundItem as ServiceReferenceDocument.Document;

                if (currentDoc != null && !currentDoc.IsLatestVersion)
                {
                    RadMessageBox.Show(this, "Impossibile mettere in check-out una versione precedente del documento.", "", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                    return;
                }

                if (RadMessageBox.Show("Sei sicuro di volere estrarre il documento?", "Check-Out", MessageBoxButtons.YesNo, Telerik.WinControls.RadMessageIcon.Question) == DialogResult.Yes)
                {
                    ServiceReferenceDocument.Document document = new BiblosDs.Document.AdminCentral.ServiceReferenceDocument.Document();
                    Guid id = (Guid)radGridView2.SelectedRows[0].Cells["IdDocument"].Value;
                    string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    BackgroundWorker bcw = new BackgroundWorker();
                    waitForm = new WaitForm();
                    waitForm.Show();
                    bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                    {
                        using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                        {
                            document = client.CheckOutDocument(id, user, ContentFormat.Bynary, null);
                        }
                    };
                    bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                    {
                        waitForm.Close();
                        if (e1.Error != null)
                            MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            saveFileDialog1 = new SaveFileDialog();
                            radGridView2.SelectedRows[0].Cells["IsCheckOut"].Value = true;
                            radGridView2.SelectedRows[0].Cells["IdUserCheckOut"].Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            if (document.Content != null && saveFileDialog1.ShowDialog() == DialogResult.OK)
                                File.WriteAllBytes(saveFileDialog1.FileName, document.Content.Blob);
                        }
                    };
                    bcw.RunWorkerAsync();                  
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
            }
            
        }

        void documentoCheckIn_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceReferenceDocument.Document document = (ServiceReferenceDocument.Document)radGridView2.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show("Si vuole modificare il contenuto del file?", "Modificare il file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {

                        document.Content = new Content();
                        document.Content.Blob = File.ReadAllBytes(openFileDialog1.FileName);                       
                    }    
                }
                Forms.AttributeEdit attrEdit = new AttributeEdit(document.Archive.Name, document.AttributeValues);
                if (attrEdit.ShowDialog() == DialogResult.OK)
                {
                    document.AttributeValues = attrEdit.GetAttributeValue();
                    BackgroundWorker bcw = new BackgroundWorker();
                    waitForm = new WaitForm();
                    waitForm.Show();
                    string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                    {
                        using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                        {
                            client.CheckInDocument(document, user, ContentFormat.Bynary, null);
                        }
                    };
                    bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                    {
                        waitForm.Close();
                        if (e1.Error != null)
                            MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            radGridView2.SelectedRows[0].Cells["IsCheckOut"].Value = true;
                            radGridView2.SelectedRows[0].Cells["IdUserCheckOut"].Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            radGridView1_CurrentRowChanged();
                        }
                    };
                    bcw.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
            }
            
        }

        void showHistory_Click(object sender, EventArgs e)
        {
            try
            {
                //Chain document = (Chain)radGridView1.SelectedRows[0].DataBoundItem;
                //if (document.IsCompatibility.HasValue && document.IsCompatibility.Value)
                //{
                //    MessageBox.Show("History not supported with compatibility");
                //}
                //else
                //{
                //    ViewHistory history = new ViewHistory(document.IdChain);
                //    history.ShowDialog();
                //}
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);                
            }            
        } 
        #endregion        

        private void radGridView2_DataBindingComplete(object sender, GridViewBindingCompleteEventArgs e)
        {
            this.ShowGuid(radGridView2, ConfigurationManager.AppSettings["ShowGUID"] == null ? false :Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGUID"]));
        }

        private void radGridView1_RowsChanged(object sender, GridViewCollectionChangedEventArgs e)
        {

        }       

        private void radComboBoxArchive_SelectedValueChanged(object sender, EventArgs e)
        {
            if (radComboBoxArchive.SelectedValue != null)
            {
                ucPager_PageChanged(sender, 0);
            }
        }

        private void radGridView2_CurrentRowChanged(object sender, CurrentRowChangedEventArgs e)
        {
            using (var client = new ServiceReferenceDocument.DocumentsClient("Binding_Documents"))
            {
                var doc = (e.CurrentRow.DataBoundItem as ServiceReferenceDocument.Document);
                doc = client.GetDocumentInServer(doc.IdDocument);
                gvServers.DataSource = doc.DocumentInServer;
            }
        }

        private void parentDocument_CurrentRowChanged(object sender, CurrentRowChangedEventArgs e)
        {
            ServiceReferenceDocument.DocumentsClient client = new ServiceReferenceDocument.DocumentsClient("Binding_Documents");
            client.GetChainInfoAsync((Guid)e.CurrentRow.Cells["IdDocument"].Value, null, null);
            client.GetChainInfoCompleted += new EventHandler<GetChainInfoCompletedEventArgs>(client_GetChainInfoCompleted);
        }

        void client_GetChainInfoCompleted(object sender, GetChainInfoCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                RadMessageBox.Show(e.Error.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
                return;
            }

            var result = e.Result;

            if (result != null)
                result = new BindingList<ServiceReferenceDocument.Document>(result.OrderBy(x => x.Name).ThenByDescending(x => x.Version).ThenByDescending(x => x.DateCreated).ToList());

            radGridView2.DataSource = result;
        }

        private void ucPager_PageChanged(object sender, int newPageNumber)
        {
            currentPage = newPageNumber;

            if (currentPage < 0)
                currentPage = 0;

            if (currentPage > 0)
                currentPage--;

            radGridView1_CurrentRowChanged();
        }
    }
}
