using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;

namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    public partial class Step2 : WizardStepBase
    {
        private const string TITOLO = "Selezione archivi";
        private const string EMPTY_MAPPING_TEXT = "NESSUNA ASSOCIAZIONE";
        private WaitForm frmWait = new WaitForm();

        private List<ServiceReferenceDocument.Attribute> srcAttributes, destAttributes;
        private bool srcAttributesLoaded, destAttributesLoaded, canProceed;

        private static Dictionary<ServiceReferenceDocument.Attribute, ServiceReferenceDocument.Attribute> attributesMappings;
        public static Dictionary<ServiceReferenceDocument.Attribute, ServiceReferenceDocument.Attribute> AttributesMappings
        {
            get
            {
                return new Dictionary<ServiceReferenceDocument.Attribute, ServiceReferenceDocument.Attribute>(Step2.attributesMappings ?? new Dictionary<ServiceReferenceDocument.Attribute, ServiceReferenceDocument.Attribute>());
            }
        }

        public static ServiceReferenceDocument.Archive SourceArchive { get; private set; }
        public static ServiceReferenceDocument.Archive DestinationArchive { get; private set; }

        [Obsolete("Solo per designer", true)]
        public Step2()
            : this(null)
        {
        }

        public Step2(Step1 prevStep)
        {
            this.InitializeComponent();
            base.Initialize(TITOLO, prevStep, new Step3(this));
        }

        public override void Show(Control parentControl)
        {
            base.Show(parentControl, this);

            this.UseWaitCursor = true;
            this.Enabled = false;

            this.gridAttributi.Rows.Clear();
            this.destAttributesLoaded = false;
            this.srcAttributesLoaded = false;
            this.canProceed = false;

            this.frmWait = new WaitForm();
            this.frmWait.Show(this);

            try
            {
                this.cbArchiviSorgente.DataSource = Step1.DocumentServiceSourceClient.GetArchives();
                this.cbArchiviDestinazione.DataSource = Step1.DocumentServiceDestinationClient.GetArchives();

                this.UseWaitCursor = false;
                this.Enabled = true;
                this.frmWait.Hide();
            }
            catch (Exception ex)
            {
                this.frmWait.Hide();
                MessageBox.Show(this, "Impossibile proseguire: non si può recuperare l'elenco degli archivi. Messaggio d'errore: " + ex.Message, TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.GotoPreviousStep();
            }
        }

        public override void GotoNextStep()
        {
            if (this.canProceed)
            {
                Step2.attributesMappings = new Dictionary<ServiceReferenceDocument.Attribute, ServiceReferenceDocument.Attribute>();
                string destAttrName;
                bool almostOneAttributeWithoutMapping = false;
                ServiceReferenceDocument.Attribute attributoSorgente, attributoDestinazione;
                DataGridViewComboBoxCell cellaDestinazione;

                foreach (DataGridViewRow riga in this.gridAttributi.Rows)
                {
                    attributoSorgente = riga.Cells["colSorgente"].Value as ServiceReferenceDocument.Attribute;
                    cellaDestinazione = riga.Cells["colDestinazione"] as DataGridViewComboBoxCell;
                    destAttrName = cellaDestinazione.Value.ToString();
                    
                    if (!destAttrName.Equals(EMPTY_MAPPING_TEXT, StringComparison.InvariantCultureIgnoreCase))
                    {
                        attributoDestinazione = null;
                        foreach (var item in cellaDestinazione.Items.OfType<ServiceReferenceDocument.Attribute>())
                        {
                            attributoDestinazione = item as ServiceReferenceDocument.Attribute;
                            if (attributoDestinazione.Name == destAttrName)
                            {
                                break;
                            }
                        }
                        Step2.attributesMappings.Add(attributoSorgente, attributoDestinazione as ServiceReferenceDocument.Attribute);
                    }
                    else
                    {
                        almostOneAttributeWithoutMapping = true;
                        if (attributoSorgente.IsRequired)
                        {
                            MessageBox.Show(this, string.Format("ATTENZIONE: il mapping dell'attributo {0} e' obbligatorio.", attributoSorgente.Name), TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                }

                if (!almostOneAttributeWithoutMapping
                    || MessageBox.Show(this, "Uno o piu' attributi non sono stati associati. Si desidera procedere ugualmente?", TITOLO, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    base.GotoNextStep();
                }
            }
            else
            {
                MessageBox.Show(this, "Controllare l'allineamento degli attributi per proseguire.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (this.cbArchiviSorgente.SelectedItem != null && this.cbArchiviDestinazione.SelectedItem != null)
            {
                this.canProceed = false;
                this.Enabled = false;
                this.UseWaitCursor = true;

                this.srcAttributesLoaded = false;
                this.destAttributesLoaded = false;

                this.gridAttributi.Rows.Clear();

                Step1.DocumentServiceSourceClient.GetAttributesDefinitionCompleted += new EventHandler<GetAttributesDefinitionCompletedEventArgs>(SourceConnection_GetAttributesDefinitionCompleted);

                Step1.DocumentServiceDestinationClient.GetAttributesDefinitionCompleted += new EventHandler<GetAttributesDefinitionCompletedEventArgs>(DestinationConnection_GetAttributesDefinitionCompleted);

                this.frmWait.Show(this);

                try
                {
                    Step1.DocumentServiceSourceClient.GetAttributesDefinitionAsync((this.cbArchiviSorgente.SelectedItem as Archive).Name);

                    Step1.DocumentServiceDestinationClient.GetAttributesDefinitionAsync((this.cbArchiviDestinazione.SelectedItem as Archive).Name);

                    while (!this.srcAttributesLoaded || !this.destAttributesLoaded)
                    {
                        Application.DoEvents();
                    }

                    foreach (var attr in this.srcAttributes)
                    {
                        this.gridAttributi.Rows.Add(attr, attr.Name);

                        var cellaElementi = this.gridAttributi.Rows[this.gridAttributi.Rows.Count - 1].Cells["colDestinazione"] as DataGridViewComboBoxCell;
                        //Elemento vuoto.
                        cellaElementi.Items.Add(EMPTY_MAPPING_TEXT);
                        //Elenco attributi archivio di destinazione.
                        cellaElementi.Items.AddRange(this.destAttributes.ToArray());
                        //Bisogna sempre specificarlo.
                        cellaElementi.DisplayMember = "Name";
                        //Controlla se negli attributi dell'archivio di destinazione è già presente un attributo con l'identico nome.
                        var match = this.destAttributes
                            .Where(x => x.Name.Equals(attr.Name, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefault();

                        if (match != null)
                        {
                            cellaElementi.Value = match;
                            cellaElementi.ReadOnly = true;
                        }
                        else
                        {
                            cellaElementi.Value = EMPTY_MAPPING_TEXT;
                        }
                    }

                    try { this.frmWait.Hide(); }
                    catch { }

                    Step2.SourceArchive = this.cbArchiviSorgente.SelectedItem as ServiceReferenceDocument.Archive;
                    Step2.DestinationArchive = this.cbArchiviDestinazione.SelectedItem as ServiceReferenceDocument.Archive;

                    this.canProceed = true;
                }
                catch (Exception ex)
                {
                    this.frmWait.Hide();
                    MessageBox.Show(this, "Attenzione, si è verificato il seguente errore: " + ex.Message, TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Enabled = true;
                    this.UseWaitCursor = false;
                }
            }
        }

        private void DestinationConnection_GetAttributesDefinitionCompleted(object sender, GetAttributesDefinitionCompletedEventArgs e)
        {
            try { Step1.DocumentServiceDestinationClient.GetAttributesDefinitionCompleted -= DestinationConnection_GetAttributesDefinitionCompleted; }
            catch { }

            if (e.Cancelled || e.Error != null)
                throw new ApplicationException("impossibile recuperare gli attributi dell'archivio.");

            this.destAttributes = new List<ServiceReferenceDocument.Attribute>(e.Result.OrderBy(x => x.Name));

            this.destAttributesLoaded = true;
        }

        private void SourceConnection_GetAttributesDefinitionCompleted(object sender, GetAttributesDefinitionCompletedEventArgs e)
        {
            try { Step1.DocumentServiceSourceClient.GetAttributesDefinitionCompleted -= SourceConnection_GetAttributesDefinitionCompleted; }
            catch { }

            if (e.Cancelled || e.Error != null)
                throw new ApplicationException("impossibile recuperare gli attributi dell'archivio.");

            this.srcAttributes = new List<ServiceReferenceDocument.Attribute>(e.Result.OrderBy(x => x.Name));

            this.srcAttributesLoaded = true;
        }
    }
}
