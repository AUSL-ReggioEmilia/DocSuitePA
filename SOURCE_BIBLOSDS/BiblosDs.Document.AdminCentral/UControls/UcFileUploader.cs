using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;
using BiblosDs.Document.AdminCentral.UAdminControls;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcFileUploader : BaseAdminControls
    {
        public Archive CurrentArchive { get; set; }
        BindingList<Archive> archives;
        BindingList<BiblosDs.Document.AdminCentral.ServiceReferenceDocument.Attribute> attributes;
        private WaitForm waitForm = new WaitForm();

        public UcFileUploader()
        {
            InitializeComponent();            
        }        


        private void UcFileUploader_Load(object sender, EventArgs e)
        {
            waitForm.Show();
            backgroundWorker2.RunWorkerAsync();
            this.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                {
                    attributes = client.GetAttributesDefinition(radComboBoxArchive.Text);
                }
            }
            catch (Exception)
            {                
                throw;
            }            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            waitForm.Close();
            if (e.Error == null)
            {
                gwAttributes.DataSource = attributes;                                               
            }
            else
                MessageBox.Show(e.Error.Message, "Errore",  MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Enabled = true;            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {                      
            if (radComboBoxArchive.SelectedValue == null)
            {
                MessageBox.Show("Selezionare un Archive per proseguire.", "Incomplete data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            BindingList<AttributeValue> attributeValues = new BindingList<AttributeValue>();
            AttributeValue attributeValue;           
            ServiceReferenceDocument.Document document = new ServiceReferenceDocument.Document();
            document.Archive = archives[radComboBoxArchive.SelectedIndex];
            document.AttributeValues = attributeValues;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < gwAttributes.Rows.Count; i++)
                    {
                        if (gwAttributes.Rows[i].Cells["colValue"].Value == null || string.IsNullOrEmpty(gwAttributes.Rows[i].Cells["colValue"].Value.ToString()))
                            continue;
                        attributeValue = new AttributeValue();
                        attributeValue.Attribute = attributes.Where(x => x.IdAttribute.ToString() == gwAttributes.Rows[i].Cells["IdAttribute"].Value.ToString()).Single();
                        switch (attributeValue.Attribute.AttributeType)
                        {
                            case "System.String":
                                attributeValue.Value = gwAttributes.Rows[i].Cells["colValue"].Value.ToString();
                                break;
                            case "System.Int64":
                                attributeValue.Value = Int64.Parse(gwAttributes.Rows[i].Cells["colValue"].Value.ToString());
                                break;
                            case "System.Double":
                                attributeValue.Value = Double.Parse(gwAttributes.Rows[i].Cells["colValue"].Value.ToString());
                                break;
                            case "System.DateTime":
                                attributeValue.Value = DateTime.Parse(gwAttributes.Rows[i].Cells["colValue"].Value.ToString());
                                break;
                            default:
                                break;
                        }
                        attributeValues.Add(attributeValue);
                    }

                    document.Name = Path.GetFileName(openFileDialog1.FileName);
                    document.Content = new Content();
                    document.Content.Blob = File.ReadAllBytes(openFileDialog1.FileName);


                    Nullable<Guid> parentId = null;
                 
                    waitForm = new WaitForm();
                    waitForm.Show();
                    BackgroundWorker bcw = new BackgroundWorker();
                    bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                    {
                        using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                            document = client.AddDocumentToChain(document, parentId, ContentFormat.Bynary);

                    };
                    bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                    {
                        waitForm.Close();
                        if (e1.Error != null)
                            MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            if (!parentId.HasValue)
                                WriteChainToFile(document.DocumentParent, false);
                            for (int i = 0; i < gwAttributes.Rows.Count; i++)
                            {
                                gwAttributes.Rows[i].Cells["colValue"].Value = string.Empty;
                            }                          
                        }
                    };
                    bcw.RunWorkerAsync();                    
                }
            }
            catch (Exception ex)
            {
                waitForm.Close();
                MessageBox.Show(ex.Message);
            }            
        }

        private void btnAddChain_Click(object sender, EventArgs e)
        {          
           
            ServiceReferenceDocument.Document document = new ServiceReferenceDocument.Document();
            document.Archive = CurrentArchive;            

            try
            {
                waitForm = new WaitForm();
                waitForm.Show();
                BackgroundWorker bcw = new BackgroundWorker();                
                bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                {
                    using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                    {
                        document.IdDocument = client.CreateDocumentChain(CurrentArchive.Name, new BindingList<AttributeValue>());
                    }                    
                };
                bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                {
                    waitForm.Close();
                    if (e1.Error != null)
                        MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        WriteChainToFile(document, false);
                };
                bcw.RunWorkerAsync();                              
            }
            catch (Exception ex)
            {
                waitForm.Close();
                MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WriteChainToFile(ServiceReferenceDocument.Document document, bool compatybility)
        {            

            //Chain chain = new Chain();
            //chain.IdChain = document.IdDocument;
            //chain.IdArchive = document.Archive.IdArchive;
            //chain.ArchiveName = document.Archive.Name;
            //chain.IdBiblos = document.IdBiblos.HasValue ? document.IdBiblos.Value : 0;
            //chain.IsCompatibility = compatybility;
            //chain.Name = txtName.Text;
            //chain.Description = txtDesc.Text;
            //chain.DateCreated = DateTime.Now;
            //using (ChainContexDataContext cn = new ChainContexDataContext(ConfigurationManager.ConnectionStrings["DocumentConnectionString"].ConnectionString))
            //{
            //    cn.Chains.InsertOnSubmit(chain);
            //    cn.SubmitChanges();
            //}            
            //BindChain(CurrentArchive.IdArchive);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            DocumentsClient client = null;
            try
            {
                client = new DocumentsClient("Binding_Documents");  
                archives = client.GetArchives();
                if (archives != null)
                    archives = new BindingList<Archive>(archives.OrderBy(x => x.Name).ToList());
            }           
            finally
            {
                try
                {
                    client.Close();
                }
                catch { }
            }            
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            waitForm.Close();
            if (e.Error == null)
            {
                radComboBoxArchive.DisplayMember = "Name";
                radComboBoxArchive.ValueMember = "IdArchive";
                radComboBoxArchive.DataSource = archives;                                
            }
            else
                MessageBox.Show(e.Error.Message);
            this.Enabled = true;            
        }

        private void radComboBoxArchive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radComboBoxArchive.SelectedValue != null)
            {
                CurrentArchive = archives[radComboBoxArchive.SelectedIndex];
                if (CurrentArchive != null)
                {
                    BindChain(CurrentArchive.IdArchive);
                    waitForm = new WaitForm();
                    waitForm.Show();
                    backgroundWorker1.RunWorkerAsync();
                    this.Enabled = false;                    
                }
            }else                
                BindChain(Guid.Empty);
        }        

        private void BindChain(Guid idArchive)
        {
            //try
            //{
            //    using (ChainContexDataContext cn = new ChainContexDataContext(ConfigurationManager.ConnectionStrings["DocumentConnectionString"].ConnectionString))
            //    {
            //        var query = from m in cn.Chains
            //                    where m.IdArchive == idArchive
            //                    select m;
            //        BindingList<Chain> chains = new BindingList<Chain>(query.ToList());                    
            //        radComboBoxChain.DisplayMember = "Name";
            //        radComboBoxChain.ValueMember = "IdChain";
            //        radComboBoxChain.DataSource = chains;                    
            //        radComboBoxChain.SelectedItem = null;
            //    }                
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error",  MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            
        }             

        private void button1_Click(object sender, EventArgs e)
        {            
            if (radComboBoxArchive.SelectedValue == null)
            {
                MessageBox.Show("Selezionare un Archive per proseguire.", "Incomplete data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            BindingList<AttributeValue> attributeValues = new BindingList<AttributeValue>();
            AttributeValue attributeValue;
            ServiceReferenceDocument.Document document = new ServiceReferenceDocument.Document();
            document.Archive = archives[radComboBoxArchive.SelectedIndex];
            document.AttributeValues = attributeValues;

            UcStorageExplorer storage = new UcStorageExplorer();
            if (storage.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    document.Storage = storage.Storage;
                    document.StorageArea = storage.StorageArea;
                    for (int i = 0; i < gwAttributes.Rows.Count; i++)
                    {
                        attributeValue = new AttributeValue();
                        attributeValue.Attribute = attributes.Where(x => x.IdAttribute.ToString() == gwAttributes.Rows[i].Cells["IdAttribute"].Value.ToString()).Single();
                        switch (attributeValue.Attribute.AttributeType)
                        {
                            case "System.String":
                                attributeValue.Value = gwAttributes.Rows[i].Cells["colValue"].Value.ToString();
                                break;
                            case "System.Int64":
                                attributeValue.Value = Int64.Parse(gwAttributes.Rows[i].Cells["colValue"].Value.ToString());
                                break;
                            case "System.Double":
                                attributeValue.Value = Double.Parse(gwAttributes.Rows[i].Cells["colValue"].Value.ToString());
                                break;
                            case "System.DateTime":
                                attributeValue.Value = DateTime.Parse(gwAttributes.Rows[i].Cells["colValue"].Value.ToString());
                                break;
                            default:
                                break;
                        }
                        attributeValues.Add(attributeValue);
                    }

                    document.Name = Path.GetFileName(openFileDialog1.FileName);                    

                    Nullable<Guid> parentId = null;               
                    waitForm = new WaitForm();
                    waitForm.Show();
                    BackgroundWorker bcw = new BackgroundWorker();
                    bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                    {
                        using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                            document = client.AddDocumentToChain(document, parentId, ContentFormat.Bynary);

                    };
                    bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                    {
                        waitForm.Close();
                        if (e1.Error != null)
                            MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            if (!parentId.HasValue)
                                WriteChainToFile(document.DocumentParent, false);
                            for (int i = 0; i < gwAttributes.Rows.Count; i++)
                            {
                                gwAttributes.Rows[i].Cells["colValue"].Value = string.Empty;
                            }                          
                        }
                    };
                    bcw.RunWorkerAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
