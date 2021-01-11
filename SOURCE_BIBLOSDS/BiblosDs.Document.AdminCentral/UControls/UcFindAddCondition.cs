using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceContentSearch;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcFindAddCondition : Form
    {
        public ServiceReferenceContentSearch.Condition result = null;
        
        public UcFindAddCondition()
        {
            InitializeComponent();
            this.radComboBoxCondition.Text = string.Empty;
            this.radComboBoxOperator.Text = string.Empty;
        }

        public UcFindAddCondition(ServiceReferenceContentSearch.Condition editCondition) : this()
        {
            if (editCondition != null)
            {
                this.txtName.Text = editCondition.Name;
                this.txtValue.Text = editCondition.Value.ToString();
                this.radComboBoxOperator.Text = editCondition.Operator.ToString();
                this.radComboBoxCondition.Text = editCondition.LogicalCondition.ToString();
            }
        }

        private void btnCancell_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            result = new ServiceReferenceContentSearch.Condition
            {                
                LogicalCondition = (ServiceReferenceContentSearch.FilterCondition)Enum.Parse(typeof(ServiceReferenceContentSearch.FilterCondition), radComboBoxCondition.Text),
                Operator = (ServiceReferenceContentSearch.FilterOperator)Enum.Parse(typeof(ServiceReferenceContentSearch.FilterOperator), radComboBoxOperator.Text),
                Name = txtName.Text,
                Value = txtValue.Text
            };
            DialogResult = DialogResult.OK;
        }

        private void UcFindAddCondition_Load(object sender, EventArgs e)
        {
            var op = Enum.GetNames(typeof(ServiceReferenceContentSearch.FilterOperator));
            var cond = Enum.GetNames(typeof(ServiceReferenceContentSearch.FilterCondition));

            string szOp = this.radComboBoxOperator.Text,
                szCond = this.radComboBoxCondition.Text;

            radComboBoxOperator.DataSource = op;
            radComboBoxCondition.DataSource = cond;

            if (!string.IsNullOrEmpty(szOp)) this.radComboBoxOperator.Text = szOp;
            if (!string.IsNullOrEmpty(szCond)) this.radComboBoxCondition.Text = szCond;
        }
    }
}
