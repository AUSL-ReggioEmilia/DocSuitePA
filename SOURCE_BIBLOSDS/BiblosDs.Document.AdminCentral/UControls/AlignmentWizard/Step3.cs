using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    public partial class Step3 : WizardStepBase
    {
        private const string TITOLO = "Migrazione documenti";
        private const int TAKE = 20;

        private FormCaricamento frmWait;
        private int skip;
        private bool caricamentoAnnullato;

        private List<ServiceReferenceContentSearch.Document> documents;

        [Obsolete("Solo per designer", true)]
        public Step3()
            : this(null)
        {
        }

        public Step3(Step2 prevStep)
        {
            this.InitializeComponent();
            base.Initialize(TITOLO, prevStep, WizardStepBase.CreateEmptyWizardStep());
        }

        public override void Show(Control parentControl)
        {
            base.Show(parentControl, this);

            this.caricamentoAnnullato = false;

            this.Enabled = false;
            this.UseWaitCursor = true;

            this.frmWait = new FormCaricamento("Caricamento documenti", string.Empty, true, 0, 100, TAKE);
            this.frmWait.OperazioneAnnullata += (sender, args) => { this.caricamentoAnnullato = true; };
            this.frmWait.Show(this);

            try
            {
                #region MAPPING ATTRIBUTI

                this.gridAttrs.Rows.Clear();

                foreach (var attr in Step2.AttributesMappings)
                {
                    this.gridAttrs.Rows.Add(attr.Key, attr.Key.Name, attr.Value, attr.Value.Name);
                }

                #endregion MAPPING ATTRIBUTI

                #region RECUPERO DOCUMENTI

                this.skip = 0;
                this.documents = new List<ServiceReferenceContentSearch.Document>();

                this.gridDocs.DataSource = null;

                Step1.ContentSearchServiceSourceClient.GetAllDocumentsCompleted += Documenti_Recuperati;
                Step1.ContentSearchServiceSourceClient.GetAllDocumentsAsync(Step2.SourceArchive.Name, true, this.skip, TAKE);

                #endregion RECUPERO DOCUMENTI
            }
            catch (Exception ex)
            {
                try { this.frmWait.Hide(); }
                catch { }

                MessageBox.Show(this, "Si e' verificato un errore. Dettagli: " + ex.Message, TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Enabled = true;
                this.UseWaitCursor = false;
            }
        }

        public override void GotoNextStep()
        {
            if (this.caricamentoAnnullato && MessageBox.Show(this, "Non tutti i documenti sono stati caricati. Si vuole comunque procedere con la migrazione di quelli attualmente caricati?", TITOLO, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                MessageBox.Show(this, "Eseguire un nuovo allineamento degli attributi per ri-caricare i documenti.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (MessageBox.Show(this, "Si vuole procedere con la migrazione dei documenti?", TITOLO, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DataGridViewCheckBoxCell cella;
                var hasErrors = false;

                var documentiDaMigrare = new List<ServiceReferenceContentSearch.Document>();
                bool almostOneDocumentToMigrate = false;

                this.UseWaitCursor = true;
                this.Enabled = false;

                this.frmWait.Show(this);

                try
                {
                    foreach (DataGridViewRow riga in this.gridDocs.Rows)
                    {
                        cella = riga.Cells["colSelect"] as DataGridViewCheckBoxCell;
                        if ((bool)cella.Value)
                        {
                            almostOneDocumentToMigrate = true;
                            documentiDaMigrare.Add(riga.DataBoundItem as ServiceReferenceContentSearch.Document);
                        }
                    }
                    //Controlla se non c'è neppure un documento da migrare.
                    if (!almostOneDocumentToMigrate)
                    {
                        MessageBox.Show(this, "Non è stato selezionato alcun documento da migrare. Impossibile procedere.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        Guid destDocumentId;
                        ServiceReferenceContentSearch.Document doc;
                        ServiceReferenceDocument.Document destDoc;
                        //Cicla su tutti i documenti che debbono essere migrati.
                        for (int idxDoc = 0; idxDoc < documentiDaMigrare.Count; idxDoc++)
                        {
                            doc = documentiDaMigrare[idxDoc];
                            //Prima di tutto: occhio all'IdBiblos ed all'Hash.
                            if (doc.IdBiblos.HasValue)
                            {
                                try
                                {
                                    destDocumentId = Step1.DocumentServiceDestinationClient.GetDocumentId(Step2.DestinationArchive.Name, doc.IdBiblos.Value);

                                    destDoc = Step1.DocumentServiceDestinationClient.GetDocumentInfo(destDocumentId, null, null)
                                        .SingleOrDefault();

                                    if (destDoc == null || destDoc.DocumentHash != doc.DocumentHash)
                                        throw new ApplicationException(string.Format("Il documento sul server di destinazione con id {0} non permette di migrare il documento con id {1} presente sul server sorgente.", destDocumentId, doc.IdDocument));
                                }
                                catch (/*FaultException*/ Exception ex)
                                {
                                    //throw new ApplicationException(string.Format("Id Biblos presente sul server di destinazione per il documento \"{0}\", id {1}.", doc.Name, doc.IdDocument));
                                }
                            }
                            //Fa fuori gli attributi superflui che non debbono essere migrati: prima li seleziona...
                            var attrToDelete = doc.AttributeValues
                                .Where(x => !Step2.AttributesMappings.Keys
                                    .Select(key => key.IdAttribute)
                                    .Contains(x.IdAttribute)); //Tutti gli attributi che NON sono compresi (non sono stati selezionati) fra quelli da migrare.
                            //...poi li rimuove dalla lista.
                            ServiceReferenceContentSearch.AttributeValue toDelete;
                            while ((toDelete = attrToDelete.FirstOrDefault()) != null)
                            {
                                doc.AttributeValues.Remove(toDelete);
                            }
                            //Scorre l'elenco degli attributi mappati.
                            foreach (var mapping in Step2.AttributesMappings)
                            {
                                //Recupera, dal documento da migrare, tutti gli attributi che debbono essere migrati.
                                var attrsDaMigrare = doc.AttributeValues
                                    .Where(x => x.IdAttribute == mapping.Key.IdAttribute);
                                //Scorre tutti gli attributi che debbono essere migrati.
                                for (int idxAttr = 0; idxAttr < attrsDaMigrare.Count(); idxAttr++)
                                {
                                    var attr = attrsDaMigrare.ElementAt(idxAttr);
                                    //Assegna l'id corretto dell'attributo (id di destinazione)
                                    attr.IdAttribute = mapping.Value.IdAttribute;
                                    //Anche il nome dell'attributo deve essere coerente.
                                    attr.Attribute.Name = mapping.Value.Name;
                                    //Assegna il corretto ID attributo.
                                    attr.Attribute.IdAttribute = mapping.Value.IdAttribute;
                                    //Assegna il corretto archivio cui deve appartenere l'attributo (archivio di destinazione).
                                    attr.Attribute.Archive.IdArchive = Step2.DestinationArchive.IdArchive;
                                    attr.Attribute.Archive.Name = Step2.DestinationArchive.Name;
                                    //Assegna l'id archivio corretto al gruppo attributi da migrare.
                                    attr.Attribute.AttributeGroup = Step3.createAttributeGroupFromDocument(mapping.Value.AttributeGroup);
                                }
                            }
                            //Assegna l'archivio corretto al documento che sarà migrato.
                            doc.Archive.IdArchive = Step2.DestinationArchive.IdArchive;
                            doc.Archive.Name = Step2.DestinationArchive.Name;
                            //AddToChain
                            Step1.DocumentServiceDestinationClient.AddDocumentToChain(Step3.createDocumentFromContentSearch(doc), null, ServiceReferenceDocument.ContentFormat.Base64);
                            //via dalla lista di quelli ancora da migrare.
                            this.documents.Remove(doc);
                        }

                        this.frmWait.Hide();
                        MessageBox.Show(this, "Migrazione completata con successo.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (this.documents.Count < 1)
                        {
                            this.gridDocs.DataSource = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    try { this.frmWait.Hide(); }
                    catch { }

                    MessageBox.Show(this, "Si e' verificato un errore durante la migrazione. Messaggio: " + ex.Message, TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    hasErrors = true;
                }
                finally
                {
                    try { this.frmWait.Hide(); }
                    catch { }

                    this.UseWaitCursor = false;
                    this.Enabled = true;
                }

                if (hasErrors)
                {
                    MessageBox.Show(this, "Verranno nuovamente recuperati i documenti da elaborare.", TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Show(this.ParentControl);
                }
            }
        }

        private static ServiceReferenceDocument.Document createDocumentFromContentSearch(ServiceReferenceContentSearch.Document sourceDoc)
        {
            ServiceReferenceDocument.Document retval = null;

            if (sourceDoc != null)
            {
                retval = new ServiceReferenceDocument.Document
                {
                    ChainOrder = sourceDoc.ChainOrder,
                    DateCreated = sourceDoc.DateCreated,
                    DateExpire = sourceDoc.DateExpire,
                    DateMain = sourceDoc.DateMain,
                    DocumentHash = sourceDoc.DocumentHash,
                    ExtensionData = sourceDoc.ExtensionData,
                    FullSign = sourceDoc.FullSign,
                    IdBiblos = sourceDoc.IdBiblos,
                    IdDocument = sourceDoc.IdDocument,
                    IdPreservation = sourceDoc.IdPreservation,
                    IdUserCheckOut = sourceDoc.IdUserCheckOut,
                    IsCheckOut = sourceDoc.IsCheckOut,
                    IsConservated = sourceDoc.IsConservated,
                    IsLinked = sourceDoc.IsLinked,
                    IsVisible = sourceDoc.IsVisible,
                    Name = sourceDoc.Name,
                    PreservationName = sourceDoc.PreservationName,
                    PrimaryKeyValue = sourceDoc.PrimaryKeyValue,
                    SignHeader = sourceDoc.SignHeader,
                    Size = sourceDoc.Size,
                    Version = sourceDoc.Version,
                };


                if (sourceDoc.Archive != null)
                {
                    retval.Archive = new ServiceReferenceDocument.Archive { IdArchive = sourceDoc.Archive.IdArchive, Name = sourceDoc.Archive.Name };
                };

                if (sourceDoc.AttributeValues != null)
                {
                    retval.AttributeValues = new BindingList<ServiceReferenceDocument.AttributeValue>();
                    foreach (var values in sourceDoc.AttributeValues.Where(x => x.Attribute != null))
                    {
                        retval.AttributeValues.Add(new ServiceReferenceDocument.AttributeValue
                        {
                            IdAttribute = values.IdAttribute,
                            Value = values.Value,
                            Attribute = new ServiceReferenceDocument.Attribute
                            {
                                IdAttribute = values.Attribute.IdAttribute,
                                Name = values.Attribute.Name,
                            },
                        });
                    }
                }
            }

            return retval;
        }

        private static ServiceReferenceContentSearch.AttributeGroup createAttributeGroupFromDocument(ServiceReferenceDocument.AttributeGroup sourceAttrGroup)
        {
            ServiceReferenceContentSearch.AttributeGroup retval = null;

            if (sourceAttrGroup != null)
            {
                retval = new ServiceReferenceContentSearch.AttributeGroup
                {
                    Description = sourceAttrGroup.Description,
                    ExtensionData = sourceAttrGroup.ExtensionData,
                    IdArchive = sourceAttrGroup.IdArchive,
                    IdAttributeGroup = sourceAttrGroup.IdAttributeGroup,
                    IsVisible = sourceAttrGroup.IsVisible,
                    GroupType = (ServiceReferenceContentSearch.AttributeGroupType)Enum.Parse(typeof(ServiceReferenceContentSearch.AttributeGroupType), sourceAttrGroup.GroupType.ToString(), true),
                };
            }

            return retval;
        }

        private void Documenti_Recuperati(object sender, ServiceReferenceContentSearch.GetAllDocumentsCompletedEventArgs e)
        {
            if (this.skip < 1 /*|| this.caricamentoAnnullato*/)
                this.documents.Clear();

            if (this.caricamentoAnnullato)
            {
                MessageBox.Show(this,
                    "Il caricamento dei documenti e' stato interrotto.",
                    TITOLO,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                try { Step1.ContentSearchServiceSourceClient.GetAllDocumentsCompleted -= this.Documenti_Recuperati; }
                catch { }

                //Aggiunge i documenti alla griglia.
                this.gridDocs.AutoGenerateColumns = false;
                this.gridDocs.DataSource = this.documents;

                this.UseWaitCursor = false;
                this.Enabled = true;
            }
            else if (e.Cancelled || e.Error != null)
            {
                try { Step1.ContentSearchServiceSourceClient.GetAllDocumentsCompleted -= this.Documenti_Recuperati; }
                catch { }

                try { this.frmWait.Hide(); }
                catch { }

                MessageBox.Show(this,
                    "Si e' verificato un errore. Dettaglio: " + ((e.Error != null) ? e.Error.Message : string.Empty),
                    TITOLO,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.documents.Clear();

                this.UseWaitCursor = false;
                this.Enabled = true;
            }
            else
            {
                //Si tiene in memoria i documenti recuperati.
                this.documents.AddRange(e.Result);
                //Aggiorna il progresso di caricamento.
                this.frmWait.ValoreMassimo = e.docunentsInArchiveCount;
                this.frmWait.Incrementa();
                //Controlla se sono stati recuperati tutti i documenti in archivio.
                if ((this.skip + TAKE) >= e.docunentsInArchiveCount)
                {
                    try { Step1.ContentSearchServiceSourceClient.GetAllDocumentsCompleted -= this.Documenti_Recuperati; }
                    catch { }

                    //Aggiunge i documenti alla griglia.
                    this.gridDocs.AutoGenerateColumns = false;
                    this.gridDocs.DataSource = this.documents;

                    try { this.frmWait.Hide(); }
                    catch { }

                    MessageBox.Show(this, "Tutti i documenti sono stati recuperati. Totale documenti = " + e.docunentsInArchiveCount, TITOLO, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.UseWaitCursor = false;
                    this.Enabled = true;
                }
                else
                {
                    this.skip += TAKE;
                    Step1.ContentSearchServiceSourceClient.GetAllDocumentsAsync(Step2.SourceArchive.Name, true, this.skip, TAKE);
                }
            }
        }
    }
}
