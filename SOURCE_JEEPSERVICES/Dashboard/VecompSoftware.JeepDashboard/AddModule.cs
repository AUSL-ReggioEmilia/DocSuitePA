using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Module = VecompSoftware.JeepService.Common.Module;

namespace VecompSoftware.JeepDashboard
{
    public partial class AddModule : Form
    {
        #region [ Properties ]

        public Module Module { get; protected set; }

        #endregion

        #region [ Constructor ]

        public AddModule()
        {
            InitializeComponent();
        }

        #endregion

        #region [ Events ]

        private void cmdOk_Click(object sender, EventArgs e)
        {
            Module = new Module
            {
                Id = txtNome.Text,
                Assembly = txtPath.Text,
                Class = ListClasses.CheckedItems[0].ToString(),
                ParametersType = ListParameters.CheckedItems[0].ToString()
            };

            DialogResult = DialogResult.OK;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtPath.Text = openFileDialog1.FileName;

            // Cerco tutte le classi che implementano IJeepModule
            var myAssembly = Assembly.LoadFrom(txtPath.Text);
            var classes = myAssembly.GetTypes().Where(m => m.IsClass && m.GetInterface("IJeepModule") != null);

            try
            {

                foreach (var item in classes)
                {
                    ListClasses.Items.Add(item);
                }
                if (ListClasses.Items.Count == 1)
                {
                    ListClasses.SetItemChecked(0, true);
                }

                var parameters = myAssembly.GetTypes().Where(m => m.IsClass && m.GetInterface("IJeepParameter") != null);
                foreach (var item in parameters)
                {
                    ListParameters.Items.Add(item);
                }

                if (ListParameters.Items.Count == 1)
                {
                    ListParameters.SetItemChecked(0, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore in caricamento Classi automatico: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        #endregion

    }
}
