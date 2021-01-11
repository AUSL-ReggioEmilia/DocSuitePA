using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;
using System.IO;
using System.Xml.Linq;
using BiblosDs.Document.AdminCentral.UAdminControls;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcLoadFileOnStorage : BaseAdminControls
    {
        private WaitForm waitForm = new WaitForm();
        public Archive CurrentArchive { get; set; }
        BindingList<BiblosDs.Document.AdminCentral.ServiceReferenceDocument.Attribute> attributes;
        BindingList<Archive> archives;
        public UcLoadFileOnStorage()
        {
            InitializeComponent();
        }

        private void UcLoadFileOnStorage_Load(object sender, EventArgs e)
        {
            try
            {
                waitForm.Show();
                BackgroundWorker bcw = new BackgroundWorker();
                bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                {
                    using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                        archives = client.GetArchives();

                };
                bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                {
                    waitForm.Close();
                    if (e1.Error != null)
                        MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        ddlArchives.DisplayMember = "Name";
                        ddlArchives.ValueMember = "IdArchive";
                        ddlArchives.DataSource = archives;
                    }
                };
                bcw.RunWorkerAsync();    
            }
            catch (Exception ex)
            {
                waitForm.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                         
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ddlArchives_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlArchives.SelectedValue != null)
            {
                CurrentArchive = archives[ddlArchives.SelectedIndex];
                if (CurrentArchive != null)
                {
                    BackgroundWorker bcw = new BackgroundWorker();
                    bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                    {
                        using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
                        {
                            attributes = client.GetAttributesDefinition(CurrentArchive.Name);
                        }

                    };
                    bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                    {
                        waitForm.Close();
                        waitForm.Close();
                        if (e1.Error == null)
                        {
                            gwAttributes.DataSource = attributes;
                        }
                        else
                            MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    };
                    bcw.RunWorkerAsync();    
                }
            }           
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(txtPath.Text))
                {
                    MessageBox.Show("Directory not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.radGridStatus.Rows.Clear();
                DocumentsClient client = new DocumentsClient("Binding_Documents");
                string[] files = Directory.GetFiles(txtPath.Text, "*.xml");
                string pathOut = Path.Combine(txtPath.Text, "OUT");
                if (!Directory.Exists(pathOut))
                    Directory.CreateDirectory(pathOut);
                string pathErr = Path.Combine(txtPath.Text, "ERR");
                if (!Directory.Exists(pathErr))
                    Directory.CreateDirectory(pathErr);
                foreach (var file in files)
                {
                    string[] xmlRow = File.ReadAllLines(file);
                    StringBuilder xml = new StringBuilder();
                    foreach (var str in xmlRow)
                    {
                        if (!str.StartsWith("<!-- "))
                            xml.AppendLine(str);
                    }
                    XElement settingsDoc = XElement.Parse(xml.ToString());
                    var dictionary = from element in settingsDoc.Descendants("Zone")
                                     select new
                                     {
                                         Name = element.Attribute("Name").Value,
                                         Value = element.Value
                                     };
                    var tmpName = Path.GetFileNameWithoutExtension(settingsDoc.Attributes("DocumentFileName").FirstOrDefault().Value) + txtSuffix.Text + Path.GetExtension(settingsDoc.Attributes("DocumentFileName").FirstOrDefault().Value);
                    BindingList<AttributeValue> attributeValues = new BindingList<AttributeValue>();
                    foreach (var attribute in attributes)
                    {
                        if (dictionary.Where(x => x.Name == attribute.Name).FirstOrDefault() != null)
                        {
                            object val = null;
                            string obj = dictionary.Where(x => x.Name == attribute.Name).FirstOrDefault().Value;
                            switch (attribute.AttributeType)
                            {
                                case "System.String":
                                    val = obj;
                                    break;
                                case "System.Int64":
                                    val = int.Parse(obj);
                                    break;
                                case "System.Double":
                                    val = Double.Parse(obj);
                                    break;
                                case "System.DateTime":
                                    val = DateTime.Parse(obj);
                                    break;
                                default:
                                    break;
                            }
                            attributeValues.Add(new AttributeValue { Attribute = attribute, Value = val });
                        }
                    }
                    ServiceReferenceDocument.Document document = new ServiceReferenceDocument.Document();
                    document.Name = Path.GetFileName(tmpName);
                    document.Content = new Content();
                    document.Content.Blob = File.ReadAllBytes(Path.Combine(txtPath.Text, tmpName));
                    document.AttributeValues = attributeValues;
                    document.Archive = CurrentArchive;
                    try
                    {
                        client.AddDocumentToChainAsync(document, null, ContentFormat.Bynary, new string[] { txtPath.Text, file, tmpName });
                        client.AddDocumentToChainCompleted += new EventHandler<AddDocumentToChainCompletedEventArgs>(client_AddDocumentToChainCompleted);
                    }
                    catch (Exception exx)
                    {
                        this.radGridStatus.Rows.Add(new string[] { 
                        document.Name,
                        "EXC",
                        exx.Message
                    });                      
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        void client_AddDocumentToChainCompleted(object sender, AddDocumentToChainCompletedEventArgs e)
        {
            try
            {
                string[] prm = (string[])e.UserState;
                string pathOut = Path.Combine(prm[0], "OUT");
                if (!Directory.Exists(pathOut))
                    Directory.CreateDirectory(pathOut);
                string pathErr = Path.Combine(prm[0], "ERR");
                if (!Directory.Exists(pathErr))
                    Directory.CreateDirectory(pathErr);
                if (e.Error == null)
                {
                    this.radGridStatus.Rows.Add(new string[] { prm[1], "OK", "Document ID: " + e.Result.IdDocument });
                    File.Move(prm[1], Path.Combine(pathOut, Path.GetFileName(prm[1])));
                    File.Move(Path.Combine(prm[0], prm[2]), Path.Combine(pathOut, prm[2]));
                }
                else
                {
                    this.radGridStatus.Rows.Add(new string[] 
                    { 
                        e.UserState.ToString(),
                        "ERROR",
                        e.Error.Message
                    });
                    File.Move(prm[1], Path.Combine(pathErr, Path.GetFileName(prm[1])));
                    File.Move(Path.Combine(prm[0], prm[2]), Path.Combine(pathErr, prm[2]));                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
    }
}
