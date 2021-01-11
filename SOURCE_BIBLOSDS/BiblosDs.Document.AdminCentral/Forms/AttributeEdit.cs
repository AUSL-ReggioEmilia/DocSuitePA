using System;
using System.ComponentModel;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;

namespace BiblosDs.Document.AdminCentral.Forms
{
    public partial class AttributeEdit : Form
    {        
        public AttributeEdit(string archiveName, BindingList<AttributeValue> Attributes)
        {
            InitializeComponent();
            ucAttributeEdit1.archiveName = archiveName;
            ucAttributeEdit1.OldAttribute = Attributes;
        }

        private void AttributeEdit_Load(object sender, EventArgs e)
        {            
            ucAttributeEdit1.Load();
        }

        public BindingList<AttributeValue> GetAttributeValue()
        {
            return ucAttributeEdit1.GetAttributeValue();
        }
    }
}
