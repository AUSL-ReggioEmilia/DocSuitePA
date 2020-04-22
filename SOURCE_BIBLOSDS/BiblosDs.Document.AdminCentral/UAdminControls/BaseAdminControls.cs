using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using Telerik.WinControls.UI;
using BiblosDs.Document.AdminCentral.ServiceReferenceAdministration;

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class BaseAdminControls: UserControl
    {
        private WaitForm dlg = new WaitForm();
        protected AdministrationClient Client;
        public String ControlName { get; set; }
        public Hashtable OutputParameters { get; set; }
        public Hashtable InputParameters { get; set; }        
        protected action Action;

        public event EventHandler CloseClick;

        protected enum action
        {
            nothing,
            create,
            update,
            delete
        }      

        public BaseAdminControls()
        {
            Debug.WriteLine("BaseAdminControls");
            try
            {
                Client = new AdministrationClient("Binding_Administration");              
            }
            catch (Exception ex)
            {
                TrapError(ex);
            }            
        }

        public BaseAdminControls(Hashtable parameters): this()
        {
            Debug.WriteLine("BaseAdminControls init");
            try
            {
                InputParameters = parameters;
            }
            catch (Exception ex)
            {
                TrapError(ex);
            }
        }

        protected string FormatTitle(List<String> args)
        {
            String FormatTitle = String.Empty;
            for (int i = 0; i < args.Count; i++)
            {
                if (i != args.Count - 1)
                {
                    FormatTitle += "[" + args[i] + "].";
                }
                else
                {
                    FormatTitle = FormatTitle.TrimEnd('.') + " - " + args[i];
                }
            }
            return FormatTitle;
        }

        protected void TrapError(Exception e)
        {
            if (e.Message.Contains("provider: Provider Named Pipes, error: 40") )
                MessageBox.Show("Al momento non è possibile effettuare la connessione al server. Siete pregati di tentare più tardi." +
                Environment.NewLine + Client.Endpoint.Address.ToString(), "Errorore di connessione, BiblosDs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(e.Message, "BiblosDs", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }        
        /// <summary>
        /// Apre la wait dialog e disabilita il controllo
        /// </summary>
        protected virtual void CreateWaitDialog()
        {
            Cursor = Cursors.WaitCursor;
            if (dlg == null)
            {
                dlg = new WaitForm();
                dlg.TabIndex = 100;                
                dlg.Dock = DockStyle.Fill;
                dlg.Show();
            }            
            this.Enabled = false;
        }

        /// <summary>
        /// Chiude la wait dialog e riabilita il controllo
        /// </summary>
        protected virtual void CloseWaitDialog()
        {            
            if (dlg != null)
            {                                
                dlg.Close();
                dlg = null;
            }
            Cursor = Cursors.Default;
            this.Enabled = true;
        }

        /// <summary>
        /// Metodo per validare l'inizializzazione del controllo
        /// </summary>
        /// <param name="requiredParameters"></param>
        protected virtual void VerifyInputParameters(List<string> requiredParameters)
        {
            foreach (String s in requiredParameters)
            {
                if (InputParameters[s] == null)
                {
                    throw new Exception("Il parametro obbligatorio [" + s + "] ha valore NULL");
                }
            }
        }

        protected virtual void BackToSenderControl(object sender, EventArgs e)
        {
        }
        
        protected void Close_Click(object sender, EventArgs e)
        {
            if (CloseClick != null)
            {
                CloseClick(this, new EventArgs());
            }
        }

        protected void OnError(Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

        protected void OnEndSubmit(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseWaitDialog();
            if (e.Error == null)
            {
                MessageBox.Show("Operation completed successfully");
                BackToSenderControl(null, new EventArgs());
            }
            else
            {
                TrapError(e.Error);
            }
        }

        /// <summary>
        /// Mostra o Nasconde le Colonne del GridView Passato contenenti i Guid
        /// </summary>
        /// <param name="radGridView">GridView da esaminare</param>
        /// <param name="bShow">Flag che indica se mostrare - True o nascondere - false le colonne con i Guid</param>
        protected void ShowGuid(RadGridView radGridView, Boolean bShow)
        {
            try
            {
                if (radGridView.Rows.Count > 0)
                {
                    GridViewRowInfo rw = radGridView.Rows[0];
                    foreach (GridViewDataColumn col in radGridView.Columns)
                    {
                        if (rw.Cells[col.FieldName].Value != null && rw.Cells[col.FieldName].Value.GetType() == typeof(System.Guid))
                        {
                            col.IsVisible = bShow;
                        }
                    }
                }
            }
            catch (Exception)
            {                
                //throw;
            }            
        }
    }
}
