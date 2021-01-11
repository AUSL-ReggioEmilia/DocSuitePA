using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;
using Telerik.WinControls.UI;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcAttributeEdit : UserControl
    {

        BindingList<BiblosDs.Document.AdminCentral.ServiceReferenceDocument.Attribute> attributes;
        public string archiveName;
        public BindingList<AttributeValue> OldAttribute { get; set; }

        public UcAttributeEdit()
        {
            InitializeComponent();
        }

        public void Load()
        {
            backgroundWorker1.RunWorkerAsync();            
        }

        public BindingList<AttributeValue> GetAttributeValue()
        {
            BindingList<AttributeValue> attributeValues = new BindingList<AttributeValue>();
            AttributeValue attributeValue;
            try
            {
                for (int i = 0; i < gwAttributes.Rows.Count; i++)
                {
                    attributeValue = new AttributeValue();
                    attributeValue.Attribute = attributes.Where(x => x.IdAttribute.ToString() == gwAttributes.Rows[i].Cells["IdAttribute"].Value.ToString()).Single();
                    attributeValue.Value = gwAttributes.Rows[i].Cells["colValue"].Value.ToString();
                    attributeValues.Add(attributeValue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString()); 
            }
            return attributeValues;
        }       

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (DocumentsClient client = new DocumentsClient("Binding_Documents"))
            {
                attributes = client.GetAttributesDefinition(archiveName);
            }  
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                gwAttributes.DataSource = attributes;
                for (int i = 0; i < gwAttributes.Rows.Count; i++)
                {
                    try
                    {
                        gwAttributes.Rows[i].Cells["colValue"].Value = OldAttribute.Where(x => x.Attribute.IdAttribute.ToString() == gwAttributes.Rows[i].Cells["IdAttribute"].Value.ToString()).Single().Value;                                        
                    }
                    catch (ArgumentNullException){}
                    catch (InvalidOperationException){}
                }
            }
            else
                MessageBox.Show(e.Error.ToString());            
            
        }

        /// <summary>
        /// Mostra o Nasconde le Colonne del GridView Passato contenenti i Guid
        /// </summary>
        /// <param name="radGridView">GridView da esaminare</param>
        /// <param name="bShow">Flag che indica se mostrare - True o nascondere - false le colonne con i Guid</param>
        private void ShowGuid(RadGridView radGridView, Boolean bShow)
        {
            if (radGridView.Rows.Count > 0)
            {
                GridViewRowInfo rw = radGridView.Rows[0];
                foreach (GridViewDataColumn col in radGridView.Columns)
                {
                    if (rw.Cells[col.FieldName].Value.GetType() == typeof(System.Guid))
                    {
                        col.IsVisible = bShow;
                    }
                }
            }
        }
    }
}
