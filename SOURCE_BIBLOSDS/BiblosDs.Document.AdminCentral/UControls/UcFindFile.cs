using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceContentSearch;
using Telerik.WinControls.UI;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;
using BiblosDS.UI.ConsoleTest.UControls;
using System.IO;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcFindFile : UserControl
    {
        ServiceReferenceContentSearch.ContentSearchClient client;
        ServiceReferenceDocument.DocumentsClient clientDocument = new DocumentsClient("Binding_Documents");
        WaitForm waitForm = new WaitForm();
        RadContextMenu menu;
        BindingList<Condition> conditions = new BindingList<Condition>();
        Dictionary<RadTreeNode, Condition> conditionsDictionary = new Dictionary<RadTreeNode, Condition>();

        private BindingList<ServiceReferenceContentSearch.Condition> conditionsToFind = new BindingList<Condition>();
        private int skip = 0;
        private const int TAKE = 10;

        public UcFindFile()
        {
            InitializeComponent();

            client = new ServiceReferenceContentSearch.ContentSearchClient("ContentSearch");

            client.SearchQueryPagedCompleted += (object s, SearchQueryPagedCompletedEventArgs args) =>
            {
                if (args.Error != null || args.Result.HasErros)
                {
                    waitForm.Close();
                    this.UseWaitCursor = false;
                    this.Enabled = true;
                    MessageBox.Show(args.Error == null ? args.Result.Error.Message : args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ServiceReferenceContentSearch.Document currDoc;
                    for (int i = 0; i < args.Result.Documents.Count; i++)
                    {
                        currDoc = args.Result.Documents[i];
                        
                        var metadataDetails = currDoc.AttributeValues
                            .Select(x => x.Attribute.Name + ": " + x.Value)
                            .ToArray();

                        this.radGridFindResults.Rows.Add(new string[] {
                            (skip + i + 1).ToString(),
                            currDoc.Name,
                            (currDoc.Status != null && !string.IsNullOrEmpty(currDoc.Status.Description))? currDoc.Status.Description : "N/A",
                            string.Join(" ", metadataDetails),
                            currDoc.IdDocument.ToString(),                       
                        });
                    }

                    if ((skip + TAKE) >= args.Result.TotalRecords)
                    {
                        waitForm.Close();
                        this.UseWaitCursor = false;
                        this.Enabled = true;
                        MessageBox.Show("Retrieved " + args.Result.TotalRecords + " record.", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.btnFind.Enabled = true;
                    }
                    else
                    {
                        skip += TAKE;
                        ricercaDocumenti();
                    }
                }
            };
        }

        private void UcFindFile_Load(object sender, EventArgs e)
        {
            //this.radTreeView1.SelectedNode = null;
            //this.radTreeView1.DisplayMember = "DisplayName";
            //this.radTreeView1.ValueMember = "Id";
            //this.radTreeView1.ParentIDMember = "IdParent";
            //this.radTreeView1.DataSource = conditions;
            menu = new RadContextMenu();
            RadMenuItem documentoView = new RadMenuItem();
            documentoView.Text = "Add child condition";
            RadContextMenuManager manager = new RadContextMenuManager();
            manager.SetRadContextMenu(this.radTreeView1, radContextMenu1);
            
            addChildCond.Click += (object sender1, EventArgs e1) =>
            {
                if (this.radTreeView1.SelectedNode != null)
                {
                    UcFindAddCondition cond = new UcFindAddCondition();
                    if (cond.ShowDialog() == DialogResult.OK)
                    {                       

                        var conditionResult = cond.result;
                        ////conditionResult.Id = conditions.Count + 1;
                        ////conditionResult.IdParent = condition.Id;                        
                        conditions.Add(conditionResult);

                        RadTreeNode newNode = new RadTreeNode(conditionResult.Name + " " + conditionResult.Operator + " " + conditionResult.Value);                        

                        this.radTreeView1.SelectedNode.Nodes.Add(newNode);
                        this.conditionsDictionary.Add(newNode, conditionResult);
                    }
                }
                else
                {
                    btnAdd_Click(null, null);
                }
            };

            remCond.Click += (object sender1, EventArgs e1) =>
            {
                if (this.radTreeView1.SelectedNode != null)
                {                    
                    conditions.Remove(conditionsDictionary[radTreeView1.SelectedNode]);
                    conditionsDictionary.Remove(this.radTreeView1.SelectedNode);
                    foreach (RadTreeNode node in this.radTreeView1.SelectedNode.Nodes)
                    {
                        var nodeCondition = conditionsDictionary[node];
                        if (conditions.Contains(nodeCondition)) conditions.Remove(nodeCondition);
                        if (conditionsDictionary.ContainsKey(node)) conditionsDictionary.Remove(node);
                    }
                    this.radTreeView1.SelectedNode.Nodes.Clear();
                    this.radTreeView1.SelectedNode.Remove();
                }
            };

            editCond.Click += (object sender1, EventArgs e1) =>
            {
                if (this.radTreeView1.SelectedNode != null)
                {
                    var condition = conditionsDictionary[radTreeView1.SelectedNode];
                    Condition oldCondition = condition;

                    UcFindAddCondition ucDlg = new UcFindAddCondition(condition);
                    if (ucDlg.ShowDialog(this) == DialogResult.OK)
                    {
                        condition = ucDlg.result;
//                        this.radTreeView1.SelectedNode.DataBoundItem = condition;
                        this.radTreeView1.SelectedNode.Text = condition.Name + " " + condition.Operator + " " + condition.Value;
                        this.conditionsDictionary[this.radTreeView1.SelectedNode] = condition;
                        this.conditions.Remove(oldCondition);
                        this.conditions.Add(condition);
                    }
                }
            };

            manager.SetRadContextMenu(this.radGridFindResults, this.radContextMenu2);
            this.showDocument.Click += (object sender1, EventArgs e1) =>
            {
                try
                {
                    if (this.radGridFindResults.SelectedRows.Count > 0)
                    {
                        BiblosDs.Document.AdminCentral.ServiceReferenceDocument.Content content = null;
                        //e.CurrentRow.Cells["IdChain"].Value, e.CurrentRow.Cells["IdBiblos"].Value });            
                        Guid id = Guid.Parse(radGridFindResults.SelectedRows[0].Cells["iddocument"].Value.ToString());
                        // Metodo nuovo
                        waitForm = new WaitForm();
                        waitForm.Show();
                        BackgroundWorker bcw = new BackgroundWorker();
                        bcw.DoWork += delegate(object sender2, DoWorkEventArgs e2)
                        {
                            using (var client = new DocumentsClient("Binding_Documents"))
                            {
                                content = client.GetDocumentContent(id, null, ContentFormat.Bynary, null);
                            }
                        };
                        bcw.RunWorkerCompleted += delegate(object sender2, RunWorkerCompletedEventArgs e2)
                        {
                            waitForm.Close();
                            if (e2.Error != null)
                                MessageBox.Show(e2.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                            {
                                if (content != null)
                                {
                                    DialogResult res = MessageBox.Show("Yes Open, No Save file", "Open/Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                    if (res == DialogResult.Yes)
                                        new UcDocumentViewer(content.Blob).Show();
                                    else if (res == DialogResult.No)
                                    {
                                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                                            File.WriteAllBytes(saveFileDialog1.FileName, content.Blob);
                                    }
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
            };

            this.showMetadata.Click += (object sender1, EventArgs e1) =>
                {
                    if (this.radGridFindResults.SelectedRows.Count > 0)
                    {
                        Guid id = Guid.Parse(radGridFindResults.SelectedRows[0].Cells["iddocument"].Value.ToString());
                        clientDocument.GetDocumentInfoCompleted += delegate(object senderObj, GetDocumentInfoCompletedEventArgs eRes)
                        {
                            waitForm.Close();
                            if (eRes.Error != null)
                            {
                                MessageBox.Show(eRes.Error.Message);
                                return;
                            }
                            Forms.MetaDataView metadata = new Forms.MetaDataView();
                            metadata.Attributes = eRes.Result.Where(x => x.IdDocument == Guid.Parse(radGridFindResults.SelectedRows[0].Cells["iddocument"].Value.ToString())).Single().AttributeValues;
                            metadata.ShowDialog();
                        };
                        clientDocument.GetDocumentInfoAsync(id, null, null);
                        waitForm = new WaitForm();
                        waitForm.Show();
                    }
                };
            menu.Items.Add(documentoView);
        }        

        private void btnInsert_Click(object sender, EventArgs e)
        {
            //conditionBindingSource.Add(new ServiceReferenceContentSearch.Condition
            //{
            //    LogicalCondition = (ServiceReferenceContentSearch.FilterCondition)Enum.Parse(typeof(ServiceReferenceContentSearch.FilterCondition), radComboBoxConditions.Text),
            //    Operator = (ServiceReferenceContentSearch.FilterOperator)Enum.Parse(typeof(ServiceReferenceContentSearch.FilterOperator), radComboBoxOperators.Text),
            //    Name = txtName.Text,
            //    Value = txtValue.Text
            //});
        }

        private void AddNode(RadTreeNode node, ServiceReferenceContentSearch.Condition condition)
        {
            if (node.Nodes.Count > 0)
            {
                condition.Conditions = new BindingList<Condition>();
                foreach (var item in node.Nodes)             
                    AddNode(item, condition);                    
            }
            else
            {
                
                //ServiceReferenceContentSearch.Condition cond = new Condition();
                //cond = node.DataBoundItem as ServiceReferenceContentSearch.Condition;
                if (condition.Conditions != null)
                    condition.Conditions.Add(conditionsDictionary[node]);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (this.radTreeView1.Nodes.Count <= 0 || !this.btnFind.Enabled)
            {
                return;
            }
            conditionsToFind = new BindingList<ServiceReferenceContentSearch.Condition>();
            //conditionsToFind.Add(
            //    new Condition
            //    {
            //        LogicalCondition = FilterCondition.Or,
            //        Conditions = new BindingList<Condition> 
            //        { 
            //            new Condition { Name="Nome", Value = "Gianni", LogicalCondition = FilterCondition.And, Operator = FilterOperator.IsEqualTo }, 
            //            new Condition { Name="Nome", Value = "Gianni", LogicalCondition = FilterCondition.And, Operator = FilterOperator.IsEqualTo} 
            //        }
            //    });
            //conditionsToFind.Add(
            //    new Condition
            //    {
            //        LogicalCondition = FilterCondition.And,
            //        Conditions = new BindingList<Condition> 
            //        { 
            //            new Condition { Name="Nome", Value = "Andrea", LogicalCondition = FilterCondition.And, Operator = FilterOperator.IsEqualTo }, 
            //            new Condition { Name="Nome", Value = "Piccoli", LogicalCondition = FilterCondition.And, Operator = FilterOperator.IsEqualTo} 
            //        }
            //    });
            
            foreach (var item in radTreeView1.Nodes)
            {
                var cond = conditionsDictionary[item];// item.DataBoundItem as ServiceReferenceContentSearch.Condition;
                AddNode(item, cond);
                conditionsToFind.Add(cond);
            }         
            this.radGridFindResults.Rows.Clear();

            skip = 0;

            ricercaDocumenti();
        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {
            UcFindAddCondition cond = new UcFindAddCondition();
            if (cond.ShowDialog() == DialogResult.OK)
            {
                Condition condition = cond.result;
                ////condition.Id = conditions.Count+1;
                ////condition.IdParent = 0;
                conditions.Add(condition);

                RadTreeNode newNode = new RadTreeNode(condition.Name + " " + condition.Operator + " " + condition.Value);                
                //newNode.DataBoundItem = condition;
                this.radTreeView1.Nodes.Add(newNode);
                this.conditionsDictionary.Add(newNode, condition);
            }
        }

        private void ricercaDocumenti()
        {
            if (conditionsToFind != null)
            {
                if (skip < 1)
                {
                    skip = 0;
                    try { radGridFindResults.Rows.Clear(); }
                    catch { }
                }

                if (waitForm == null || !waitForm.Visible)
                {
                    this.UseWaitCursor = true;
                    this.Enabled = false;
                    waitForm = new WaitForm();
                    waitForm.Show();
                }

                client.SearchQueryPagedAsync(conditionsToFind, null, null, skip, TAKE);
            }                
        }
    }
}
