using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using BiblosDs.Document.AdminCentral.ServiceReferenceAdministration;
using Telerik.WinControls;

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class UcAttributeGroupDetail : BaseAdminControls
    {
        private Guid IdArchive, IdAttributeGroupModify;
        private AttributeGroup agAttribute;
        private action CurrentAction;
        private string ArchiveName;

        public UcAttributeGroupDetail(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();
            this.IdArchive = this.IdAttributeGroupModify = Guid.Empty;
            // Caricamento dati da DB.
            this.Load += new EventHandler(UcAttributeGroupDetail_Load);
        }

        void UcAttributeGroupDetail_Load(object sender, EventArgs e)
        {
            base.VerifyInputParameters(new List<string> { "Action", "IdArchive" });
            this.IdArchive = (Guid)InputParameters["IdArchive"];

            base.CreateWaitDialog();
            try
            {
                this.comboBoxType.Items.AddRange(Enum.GetNames(typeof(AttributeGroupType)));
                this.CurrentAction = (action)InputParameters["Action"];
                this.ArchiveName = InputParameters.ContainsKey("ArchiveName") ? InputParameters["ArchiveName"].ToString() 
                    : string.Empty;

                switch (this.CurrentAction)
                {
                    #region Creazione nuovo AttributeGroup
                    case action.create:
                        this.btUpdate.Text = "Insert";
                        this.btUpdate.Click += this.InsertAttributeGroup;
                        this.radPanelTitle.Text = "Insert Attribute Group for archive " +
                            this.ArchiveName;
                        break;
                    #endregion
                    #region Modifica di un AttributeGroup già esistente
                    case action.update:
                        VerifyInputParameters(new List<string> { "IdAttributeGroup" });

                        this.IdAttributeGroupModify = (Guid)InputParameters["IdAttributeGroup"];

                        this.btUpdate.Click += this.UpdateAttributeGroup;
                        this.radPanelTitle.Text = "Updating Attribute Group";
                        this.LoadAttributeGroupFromClient();

                        break;
                    #endregion
                }

                base.CloseWaitDialog();
            }
            catch (Exception exx)
            {
                //Propaga l'eccezione.
                throw exx;
            }
        }

        private void LoadAttributeGroupFromClient()
        {
            this.agAttribute = Client.GetAttributeGroup(this.IdArchive).Where(x => x.IdAttributeGroup == this.IdAttributeGroupModify).First();

            if (this.agAttribute != null)
            {
                this.txtDescription.Text = agAttribute.Description;
                this.comboBoxType.Text = agAttribute.GroupType.ToString();
            }
            else
            {
                base.TrapError(new Exception("Invalid IdAttributeGroup"));
            }
        }

        private void InsertAttributeGroup(object sender, EventArgs e)
        {
            string descrizione = this.txtDescription.Text;
            string tipo = this.comboBoxType.Text;
            if (!string.IsNullOrEmpty(descrizione) && !string.IsNullOrEmpty(tipo))
            {
                base.CreateWaitDialog();
                try
                {
                    AttributeGroup ag = new AttributeGroup();
                    AttributeGroupType grpType;
                    if (!Enum.TryParse<AttributeGroupType>(tipo, true, out grpType))
                    {
                        RadMessageBox.Show(this,
                        "Required informations (description, group type) not present.",
                        "Error",
                        MessageBoxButtons.OK,
                        RadMessageIcon.Error);
                        return;
                    }
                    ag.GroupType = grpType;
                    ag.Description = descrizione;
                    ag.IdAttributeGroup = Guid.NewGuid();
                    ag.IdArchive = this.IdArchive;
                    ag.IsVisible = this.ckVisible.Checked;
                    Client.AddDocumentAttributeGroup(ag);
                }
                finally
                {
                    base.OnEndSubmit(sender, new RunWorkerCompletedEventArgs(null, null, false));
                }
            }
            else 
            {
                RadMessageBox.Show(this,
                    "Required informations aren't present. Please Retry.",
                    "Error", 
                    MessageBoxButtons.OK,
                    RadMessageIcon.Error);
            }
        }

        private void UpdateAttributeGroup(object sender, EventArgs e)
        {
            if (this.agAttribute != null)
            {
                if (string.IsNullOrEmpty(this.comboBoxType.Text)
                    || string.IsNullOrEmpty(this.txtDescription.Text))
                {
                    RadMessageBox.Show(this,
                        "Some required informations (description, group type) aren't present.",
                        "Error",
                        MessageBoxButtons.OK,
                        RadMessageIcon.Error);
                    return;
                }

                AttributeGroupType grpType;
                if (!Enum.TryParse<AttributeGroupType>(this.comboBoxType.Text, true, out grpType))
                {
                    RadMessageBox.Show(this,
                        "Group type isn't valid.",
                        "Error",
                        MessageBoxButtons.OK,
                        RadMessageIcon.Error);
                    return;
                }

                if (this.agAttribute.Description != this.txtDescription.Text
                    || this.agAttribute.GroupType != grpType
                    || !this.ckVisible.Checked)
                {
                    base.CreateWaitDialog();
                    this.agAttribute.Description = this.txtDescription.Text;
                    this.agAttribute.GroupType = grpType;
                    this.agAttribute.IsVisible = this.ckVisible.Checked;
                    Client.UpdateAttributeGroup(this.agAttribute);
                }
                base.OnEndSubmit(sender, new RunWorkerCompletedEventArgs(null, null, false));
            }
        }

        protected override void BackToSenderControl(object sender, EventArgs e)
        {
            ControlName = "UcAttributeGroup";
            OutputParameters = new Hashtable();
            OutputParameters.Add("IdArchive", this.IdArchive);
            OutputParameters.Add("ArchiveName", this.ArchiveName);
            base.Close_Click(sender, e);
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.BackToSenderControl(sender, e);
        }
    }
}
