using System;
using System.ComponentModel;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;

namespace BiblosDs.Document.AdminCentral.Forms
{
    public partial class MetaDataView : Form
    {
        
        public BindingList<AttributeValue> Attributes { get; set; }
        public MetaDataView()
        {
            InitializeComponent();
        }

        private void MetaDataView_Load(object sender, EventArgs e)
        {
            foreach (var item in Attributes)
            {
                gwAttributes.Rows.Add(new object[5]{
                        item.Attribute.IdAttribute,
                        item.Attribute.Name,
                        item.Attribute.AttributeType,
                        item.Attribute.IsRequired,
                        item.Value
                    });
            }          
        }

        
    }
}
