using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Telerik.WinControls.UI;

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public class BaseAdmin: UserControl
    {
        protected Panel _panel;
        public delegate void ShowControlDelegate(String controlName, Hashtable parameters);

        public void ucShowControl(String controlName, Hashtable parameters)
        {
            _panel.Controls.Clear();
            BaseAdminControls control = null;
            switch (controlName.ToLower())
            {
                // STORAGES
                case "ucstorages":
                    control = new UCStorages(parameters);
                    break;
                case "ucstoragesdetail":
                    control = new UCStoragesDetail(parameters);
                    break;
                // ARCHIVE
                case "ucarchive":
                    control = new UCArchive(parameters);
                    break;
                case "ucarchivedetail":
                    control = new UCArchiveDetail(parameters);
                    break;
                // ARCHIVE-STORAGE
                case "ucarchivestoragerelation":
                    control = new UcArchiveStorageRelation(parameters);
                    break;
                // ATTRIBUTE
                case "ucattribute":
                    control = new UcAttribute(parameters);
                    break;
                case "ucattributedetail":
                    control = new UcAttributeDetail(parameters);
                    break;
                // STORAGE-AREA
                case "ucstoragearea":
                    control = new UcStorageArea(parameters);
                    break;
                case "ucstorageareadetail":
                    control = new UcStorageAreaDetail(parameters);
                    break;
                // STORAGE-RULE
                case "ucstoragerule":
                    control = new UcStorageRule(parameters);
                    break;
                case "ucstorageruledetail":
                    control = new UcStorageRuleDetail(parameters);
                    break;
                case "ucstoragearearule":
                    control = new UcStorageAreaRule(parameters);
                    break;
                case "ucstoragearearuledetail":
                    control = new UcStorageAreaRuleDetail(parameters);
                    break;
                case "ucattributegroup":
                    control = new UcAttributeGroup(parameters);
                    break;
                case "ucattributegroupdetail":
                    control = new UcAttributeGroupDetail(parameters);
                    break;
                case "uclog":
                    control = new UcLog(parameters);
                    break;
                case "ucserver":
                    control = new UcServer(parameters);
                    break;
                case "ucserverdetail":
                    control = new UcServerDetail(parameters);
                    break;
                default:
                    MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Il controllo richiesto non è stato trovato. Nome: [" + controlName + "]");
                    break;
            }
            if (control != null)
            {
                control.CloseClick += new EventHandler(control_CloseClick);
                control.Dock = DockStyle.Fill;
                _panel.Controls.Add(control);
            }
        }
        
        void control_CloseClick(object sender, EventArgs e)
        {
            BaseAdminControls bac = sender as BaseAdminControls;
            if (bac != null)
            {
                if (bac.ControlName != null)
                {
                    ShowControlDelegate del = new ShowControlDelegate(ucShowControl);
                    Invoke(del, new object[] { bac.ControlName, bac.OutputParameters });
                }
            }
        }

    }
}
