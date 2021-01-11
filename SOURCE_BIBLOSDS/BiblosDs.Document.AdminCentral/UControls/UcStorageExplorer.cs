using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcStorageExplorer : Form
    {
        private WaitForm waitForm = new WaitForm();
        BindingList<Storage> storages = new BindingList<Storage>();
        BindingList<StorageArea> storageAreas = new BindingList<StorageArea>();
        public Storage Storage
        {
            get
            {
                return storages.Where(x => x.IdStorage == new Guid(radComboBoxStorage.SelectedValue.ToString())).FirstOrDefault();
            }
        }

        public StorageArea StorageArea
        {
            get
            {
                return storageAreas.Where(x => x.IdStorageArea == new Guid(radComboBoxStorageArea.SelectedValue.ToString())).FirstOrDefault();
            }
        }

        public UcStorageExplorer()
        {
            InitializeComponent();
        }

        private void UcStorageExplorer_Load(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            BackgroundWorker bcw = new BackgroundWorker();            
            bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
            {
                using (DocumentsClient client = new DocumentsClient())
                    storages = client.GetStorages();

            };
            bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
            {
                waitForm.Close();
                if (e1.Error != null)
                    MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    radComboBoxStorage.DisplayMember = "Name";
                    radComboBoxStorage.ValueMember = "IdStorage";
                    radComboBoxStorage.DataSource = storages;
                }
            };
            bcw.RunWorkerAsync();
        }

        private void radComboBoxStorage_SelectedIndexChanged(object sender, EventArgs e)
        {
            waitForm = new WaitForm();
            waitForm.Show();
            BackgroundWorker bcw = new BackgroundWorker();
            bcw.DoWork += delegate(object sender1, DoWorkEventArgs e1)
            {
                using (DocumentsClient client = new DocumentsClient())
                    if (Storage != null) storageAreas = client.GetStorageAreas(Storage);
            };
            bcw.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
            {
                waitForm.Close();
                if (e1.Error != null)
                    MessageBox.Show(e1.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    radComboBoxStorageArea.DisplayMember = "Name";
                    radComboBoxStorageArea.ValueMember = "IdStorageArea";
                    radComboBoxStorageArea.DataSource = storageAreas;
                }
            };
            bcw.RunWorkerAsync();
        }
    }
}
