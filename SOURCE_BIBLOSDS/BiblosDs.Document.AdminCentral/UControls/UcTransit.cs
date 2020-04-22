using System;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls;
using serviceTransito = BiblosDs.Document.AdminCentral.ServiceReferenceTransit;
using serviceDocument = BiblosDs.Document.AdminCentral.ServiceReferenceDocument;
using BiblosDs.Document.AdminCentral.UAdminControls;
using System.Linq;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcTransit : BaseAdminControls
    {
        BindingList<serviceTransito.Document> documents;
        BindingList<serviceDocument.Archive> archives;
        private WaitForm waitForm = new WaitForm();
        private string archiveName;
        private const int TAKE = 20;

        public UcTransit()
        {
            InitializeComponent();
        }

        private void UcTransit_Load(object sender, System.EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            serviceDocument.DocumentsClient client = new serviceDocument.DocumentsClient("Binding_Documents");
            client.GetArchivesAsync();
            client.GetArchivesCompleted += delegate(object s, serviceDocument.GetArchivesCompletedEventArgs args)
                {
                    waitForm.Close();
                    if (args.Error == null)
                    {
                        archives = args.Result;
                        radComboBoxArchive.DisplayMember = "Name";
                        radComboBoxArchive.ValueMember = "IdArchive";
                        radComboBoxArchive.DataSource = archives;
                    }
                    else
                    {
                        RadMessageBox.Show(args.Error.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
                    }
                };           
        }       

        private void radComboBoxArchive_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            LoadDocument(0);
        }

        private void LoadDocument(int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 0;
            if (pageNumber > 0)
                pageNumber--;

            waitForm = new WaitForm();
            waitForm.Show();
            archiveName = radComboBoxArchive.Text;
            Exception error = null;

            #region NEW

            try
            {
                using (var client = new serviceTransito.TransitClient("Binding_Transit"))
                {
                    if (!string.IsNullOrEmpty(archiveName))
                    {
                        var response = client.GetTransitListDocumentsPaged(archiveName, pageNumber * TAKE, TAKE);
                        if (response.Error != null)
                        {
                            error = new Exception(response.Error.Message);
                        }
                        else
                        {
                            if (response.TotalRecords < 1)
                            {
                                this.ucPager.Initialize();
                            }
                            else
                            {
                                double numPagine = (double)response.TotalRecords / (double)TAKE;
                                this.ucPager.Initialize(1, (int)Math.Ceiling(numPagine), pageNumber + 1);
                            }
                        }
                        documents = response.Documents;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                waitForm.Close();

                if (error == null)
                {
                    radGridView1.DataSource = documents;
                }
                else
                {
                    this.ucPager.Initialize();
                    RadMessageBox.Show(error.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
                }
            }

            #endregion

            #region OLD

            //var bcw = new BackgroundWorker();
            //bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
            //{
            //    using (serviceTransito.TransitClient client = new serviceTransito.TransitClient("Binding_Transit"))
            //    {
            //        if (!string.IsNullOrEmpty(archiveName))
            //        {
            //            documents = client.GetTransitListDocumentsPaged(archiveName, pageNumber * TAKE, TAKE).Documents;
            //        }
            //    }
            //};
            //bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
            //{              
            //    waitForm.Close();
            //    if (e1.Error == null)
            //        radGridView1.DataSource = documents;
            //    else
            //        RadMessageBox.Show(e1.Error.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
            //};

            //bcw.RunWorkerAsync();   

            #endregion
        }

        private void btnProcessAll_Click(object sender, System.EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            BackgroundWorker bcw = new BackgroundWorker();
            string archiveName = archives.Where(x => x.IdArchive == new Guid(radComboBoxArchive.SelectedValue.ToString())).FirstOrDefault().Name;
            bool result = false;
            bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
            {
                using (serviceTransito.TransitClient client = new serviceTransito.TransitClient("Binding_Transit"))
                {
                    result = client.StoreTransitArchiveDocuments(archiveName);
                }
            };
            bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
            {
                waitForm.Close();
                if (e1.Error == null)
                {                    
                    RadMessageBox.Show(result ? "File in transito processato correttamente." : "Non tutti i file in transito processati.", "Risultato processo", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Info);                    
                    LoadDocument(0);
                }
                else
                    RadMessageBox.Show(e1.Error.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
            };

            bcw.RunWorkerAsync();
        }

        private void ucPager_PageChanged(object sender, int newPageNumber)
        {
            LoadDocument(newPageNumber);
        }       
    }
}
