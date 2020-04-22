using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions;
using VecompSoftware.JeepService.Common;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.ResolutionImporter
{
    public class ResolutionImporterModule : JeepModuleBase<ResolutionImporterParameters>
    {
        #region [ Fields ]
        private FacadeFactory _facade;
        private JsonResolutionModel _jsonResolutionModel;
        #endregion

        #region [ Properties ]
        protected virtual ISession NHibernateSession
        {
            get { return NHibernateSessionManager.Instance.GetSessionFrom("ProtDB"); }
        }
        private FacadeFactory Facade
        {
            get { return _facade ?? (_facade = new FacadeFactory()); }
        }

        public void CleanJsonModel()
        {
            _jsonResolutionModel = null;
        }

        private JsonResolutionModel JsonResolutionModel
        {
            get
            {
                if (_jsonResolutionModel == null)
                {
                    string jsonPath = GetJsonResolutionFile();
                    _jsonResolutionModel = jsonPath != null ? DeserializeResolutionModel(jsonPath) : null;
                }
                return _jsonResolutionModel;
            }
        }
        #endregion

        #region [ Methods ]
        public IList<Contact> GetProposerContacts(string description)
        {
            ICriteria criteria = NHibernateSession.CreateCriteria<Contact>();
            criteria.Add(Restrictions.Eq("Description", description));
            criteria.Add(Restrictions.Like("FullIncrementalPath", string.Format("{0}|", DocSuiteContext.Current.ResolutionEnv.ProposerContact.Value.ToString()), MatchMode.Start));
            return criteria.List<Contact>();
        }
        public string GetJsonResolutionFile()
        {
            IEnumerable<string> jsonPaths = Directory.EnumerateFiles(Parameters.ImportResolutionSourceFolder, "*.json", SearchOption.TopDirectoryOnly);
            return jsonPaths.FirstOrDefault();

        }
        private JsonResolutionModel DeserializeResolutionModel(string jsonPath)
        {
            string jsonContent = File.ReadAllText(jsonPath);
            JsonResolutionModel jsonResolutionModel = null;
            try
            {
                jsonResolutionModel = JsonConvert.DeserializeObject<JsonResolutionModel>(jsonContent);
            }
            catch (JsonSerializationException ex)
            {
                string message = String.Format("DeserializeJson - Errore in elaborazione: Message: {0} - StackTrace: {1}", ex.Message, ex.StackTrace);
                FileLogger.Error(Name, message);
                SendMessage(message);
                MoveFile(jsonPath, Parameters.ImportResolutionErrorFolder);
            }
            catch (JsonReaderException ex)
            {
                string message = String.Format("DeserializeJson - Errore in elaborazione: Message: {0} - StackTrace: {1}", ex.Message, ex.StackTrace);
                FileLogger.Error(Name, message);
                SendMessage(message);
                MoveFile(jsonPath, Parameters.ImportResolutionErrorFolder);
            }
            return jsonResolutionModel;
        }
        public override void SingleWork()
        {
            try
            {
                FileLogger.Info(this.Name, "SingleWork - Inizio elaborazione");
                if (IsJsonValid())
                {
                    UploadAndMoveJson();
                }
                FileLogger.Debug(this.Name, "SingleWork - Fine elaborazione");
            }
            catch (Exception ex)
            {
                FileLogger.Error(this.Name, String.Format("SingleWork - Errore in elaborazione: Message: {0} - StackTrace: {1}", ex.Message, ex.StackTrace));
                CleanJsonModel();
            }
        }
        private void HandleFailure(string errorMessage)
        {
            FileLogger.Error(Name, errorMessage);
            SendMessage(errorMessage);
            MoveJsonFileDocuments(Parameters.ImportResolutionErrorFolder, Parameters.ImportResolutionSourceFolder);
        }
        private bool IsJsonValid()
        {
            if (IsPathValid(Parameters.ImportResolutionSourceFolder, Parameters.ImportResolutionDropFolder))
            {
                string jsonPath = GetJsonResolutionFile();
                if (!string.IsNullOrEmpty(jsonPath))
                {
                    try
                    {
                        if (JsonResolutionModel != null && !IsAnyNullOrEmpty(JsonResolutionModel))
                        {
                            if (IsValidResolution(JsonResolutionModel))
                            {
                                FileLogger.Info(this.Name, "IsJsonValid - Il Json caricato è valido.");
                                return true;
                            }
                            else
                            {
                                HandleFailure("IsJsonValid - L'atto da importare non è valido.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleFailure(String.Concat("IsJsonValid - Il Json caricato non è corretto: ", ex.Message));
                    }
                }
            }
            return false;
        }
        private bool IsValidResolution(JsonResolutionModel jsonModel)
        {
            return (Facade.ResolutionFacade.GetByYearNumberType(jsonModel.Anno, jsonModel.Numero, jsonModel.DelDet) == null);
        }
        private bool InsertResolution(JsonResolutionModel jsonModel)
        {
            FileLogger.Info(this.Name, "Inizio inserimento Atto");
            Guid mainChainId = Guid.Empty;
            Guid attachmentsChainId = Guid.Empty;
            Resolution resolution = new Resolution();
            string typeDescription = string.Empty;
            string typeContainerDescription = string.Empty;
            resolution.Id = Facade.ParameterFacade.GetIdresolution();
            resolution.Status.Id = (short)ResolutionStatusId.Attivo;
            resolution.Year = jsonModel.Anno;
            resolution.ProposeDate = jsonModel.DataAdozione.ToLocalTime().Date;
            resolution.ProposeUser = DocSuiteContext.Current.User.FullUserName;
            resolution.AdoptionDate = jsonModel.DataAdozione.ToLocalTime().Date;
            resolution.AdoptionUser = DocSuiteContext.Current.User.FullUserName;
            resolution.Number = jsonModel.Numero;
            resolution.PublishingDate = jsonModel.DataAdozione.ToLocalTime().Date;
            resolution.PublishingUser = DocSuiteContext.Current.User.FullUserName;
            resolution.EffectivenessDate = jsonModel.DataEsecutiva.ToLocalTime().Date;
            resolution.EffectivenessUser = DocSuiteContext.Current.User.FullUserName;
            resolution.InclusiveNumber = jsonModel.Segnatura;
            resolution.ResolutionObject = jsonModel.Oggetto;
            resolution.Type.Id = jsonModel.DelDet;
            string[] splitted = jsonModel.Segnatura.Split('/');
            if (splitted.Length > 1 && splitted[1] != null)
            {
                resolution.ServiceNumber = (splitted.Length > 2 && splitted[2] != null) ? string.Concat(splitted[1], "/", splitted[2]) : splitted[1];
            }
            else
            {
                HandleFailure("InsertResolution - La segnatura dell'atto indicata nel file Json non è valida");
                return false;
            }
            if (jsonModel.DelDet == 1)
            {
                typeDescription = "Delibera";
                typeContainerDescription = "Delibere";
            }
            else
            {
                if (jsonModel.DelDet == 0)
                {
                    typeDescription = "Determina";
                    typeContainerDescription = "Determine";
                }
                else
                {
                    HandleFailure("InsertResolution - Errore in elaborazione: la tipologia di atto selezionata non esiste");
                    return false;
                }
            }
            IList<Container> containers = Facade.ContainerFacade.GetContainerByName(string.Concat(typeContainerDescription, " ", jsonModel.Proponente));
            IList<ResolutionKind> resolutionsKind = new ResolutionKindFacade(DocSuiteContext.Current.User.FullUserName).GetByName(jsonModel.TipologiaAtto);
            IList<Contact> contacts = GetProposerContacts(jsonModel.Proponente);
            IList<Role> roles = Facade.RoleFacade.GetByName(jsonModel.Proponente);
            Category category = Facade.CategoryFacade.GetById(Parameters.ImportResolutionCategoryId);

            if (containers != null && containers.Count() == 1)
            {
                resolution.Container = containers.First();
                resolution.Location = resolution.Container.ReslLocation;
            }
            else
            {
                HandleFailure(string.Concat("InsertResolution - Errore in elaborazione del contenitore: il contenitore ", typeContainerDescription, " ", jsonModel.Proponente, " non esiste o non è univoco."));
                return false;
            }
            if (contacts != null && contacts.Count() == 1)
            {
                resolution.AddProposer(contacts.First());
            }
            else
            {
                HandleFailure("InsertResolution - Errore in elaborazione del contatto selezionato: il contatto selezionato non esiste o non è univoco");
                return false;
            }
            if (category != null)
            {
                resolution.Category = category;
            }
            else
            {
                HandleFailure("InsertResolution - Errore in elaborazione del classificatore: classificatore non trovato.");
                return false;
            }

            if (resolutionsKind != null && resolutionsKind.Any() && resolutionsKind.Count() == 1)
            {
                resolution.ResolutionKind = resolutionsKind.First();
            }
            else
            {
                HandleFailure("InsertResolution - Errore in elaborazione della tipologia di atto: la tipologia di atto non esiste o non è univoca.");
                return false;
            }

            resolution.WorkflowType = Facade.TabMasterFacade.GetFieldValue("WorkflowType", DocSuiteContext.Current.ResolutionEnv.Configuration, jsonModel.DelDet);
            string SignatureDocument = String.Concat(DocSuiteContext.Current.ResolutionEnv.CorporateAcronym, " ", typeDescription, " ", jsonModel.Segnatura, " del ", jsonModel.DataAdozione.ToLocalTime().ToString("d"));
            string SignatureAttachment = String.Concat(SignatureDocument, " (Allegato)");
            try
            {
                FileLogger.Info(this.Name, "InsertResolution - Inizio caricamento documenti in Biblos.");
                string mainDocumentPath = string.Concat(Parameters.ImportResolutionSourceFolder, jsonModel.MaindocumentPath);
                FileDocumentInfo fileDocumentInfo = new FileDocumentInfo(new FileInfo(mainDocumentPath));
                fileDocumentInfo.Signature = SignatureDocument;
                BiblosDocumentInfo storedDocumentInfo = fileDocumentInfo.ArchiveInBiblos(resolution.Location.DocumentServer, resolution.Location.ReslBiblosDSDB, Guid.Empty);
                mainChainId = storedDocumentInfo.ChainId;
                FileLogger.Info(this.Name, "InsertResolution - Documento principale salvato correttamente in Biblos.");

                int integerMainChainId = storedDocumentInfo.BiblosChainId;
                int integerAttachmentChainId = -1;
                FileDocumentInfo attachment;

                string attachmentDocumentPath;
                foreach (string attachmentRelativePath in jsonModel.AttachmentsDocumentPath)
                {
                    attachmentDocumentPath = string.Concat(Parameters.ImportResolutionSourceFolder, attachmentRelativePath);
                    attachment = new FileDocumentInfo(new FileInfo(attachmentDocumentPath));
                    attachment.Signature = SignatureAttachment;
                    BiblosDocumentInfo attachmentDocumentInfo = attachment.ArchiveInBiblos(resolution.Location.DocumentServer, resolution.Location.ReslBiblosDSDB, attachmentsChainId);
                    attachmentsChainId = attachmentDocumentInfo.ChainId;
                    integerAttachmentChainId = attachmentDocumentInfo.BiblosChainId;
                    FileLogger.Info(this.Name, "InsertResolution - Allegato salvato correttamente in Biblos.");
                }

                FileResolution fileResolution = new FileResolution();
                fileResolution.Id = resolution.Id;
                fileResolution.IdResolutionFile = integerMainChainId;
                fileResolution.IdProposalFile = integerMainChainId;
                fileResolution.IdAssumedProposal = integerMainChainId;
                if (integerAttachmentChainId > 0)
                {
                    fileResolution.IdAttachements = integerAttachmentChainId;
                }
                resolution.File = fileResolution;
                FileLogger.Info(this.Name, "InsertResolution - Fine caricamento documenti in Biblos.");
            }
            catch (Exception ex)
            {
                HandleFailure(string.Concat("InsertResolution - Errore in elaborazione del salvataggio dell'atto in Biblos: ", ex.Message));
                return false;
            }

            resolution.ResolutionWorkflows = new List<ResolutionWorkflow>();
            resolution.ResolutionWorkflows.Add(CreateResolutionWorkflow(resolution, 1, 0));
            resolution.ResolutionWorkflows.Add(CreateResolutionWorkflow(resolution, 2, 0));
            resolution.ResolutionWorkflows.Add(CreateResolutionWorkflow(resolution, 3, 0));
            resolution.ResolutionWorkflows.Add(CreateResolutionWorkflow(resolution, 4, 1));

            IEnumerable<ResolutionKindDocumentSeries> resolutionDocumentSeries = resolution.ResolutionKind.ResolutionKindDocumentSeries;
            IList<BiblosChainInfo> documents;
            DocumentSeriesItem documentSeriesItem;
            ResolutionDocumentSeriesItem resolutionDocumentSeriesItem;

            using (NHibernate.ITransaction transaction = NHibernateSession.BeginTransaction())
            {
                try
                {
                    FileLogger.Info(this.Name, "InsertResolution - Inizio transaction.");
                    Facade.ResolutionFacade.SaveWithoutTransaction(ref resolution);
                    FileLogger.Info(this.Name, "InsertResolution - Inserimento autorizzazioni atto.");
                    if (contacts.First().Role != null)
                    {
                        Facade.ResolutionRoleFacade.AddRole(resolution, contacts.First().Role.Id, DocSuiteContext.Current.ResolutionEnv.AuthorizInsertType, false);
                    }
                    foreach (ResolutionKindDocumentSeries item in resolutionDocumentSeries)
                    {
                        documents = new List<BiblosChainInfo>();
                        documentSeriesItem = new DocumentSeriesItem();
                        resolutionDocumentSeriesItem = new ResolutionDocumentSeriesItem();
                        documentSeriesItem.Status = DocumentSeriesItemStatus.Draft;
                        documentSeriesItem.DocumentSeries = Facade.DocumentSeriesFacade.GetById(item.DocumentSeries.Id);
                        documentSeriesItem.Subject = resolution.ResolutionObject;
                        documentSeriesItem.Category = resolution.Category;
                        documentSeriesItem.IdMain = mainChainId;

                        Facade.DocumentSeriesItemFacade.SaveDocumentSeriesItem(documentSeriesItem, resolution.Year.Value,
                            new BiblosChainInfo(new List<DocumentInfo>()), null, null, DocSuiteContext.Current.User.FullUserName,
                            DocumentSeriesItemStatus.Draft, string.Concat("Inserimento bozza di ", item.DocumentSeries.Name), false);

                        if (roles != null && roles.Any() && roles.Count() == 1)
                        {
                            Facade.DocumentSeriesItemRoleFacade.AddOwnerRole(documentSeriesItem, roles.First(), false);
                        }
                        else
                        {
                            HandleFailure("InsertResolution - Errore in elaborazione della tipologia di atto: la tipologia di atto non esiste o non è univoca.");
                            return false;
                        }

                        resolutionDocumentSeriesItem.IdDocumentSeriesItem = documentSeriesItem.Id;
                        resolutionDocumentSeriesItem.Resolution = resolution;
                        Facade.ResolutionDocumentSeriesItemFacade.SaveWithoutTransaction(ref resolutionDocumentSeriesItem);
                    }
                    transaction.Commit();

                    //TODO:Invio comando di creazione Resolution alle WebApi
                    // facadeFactory.ResolutionFacade.SendCreateResolutionCommand(resolution);

                    FileLogger.Info(this.Name, "InsertResolution - Transaction completata con successo.");
                }
                catch (Exception ex)
                {
                    FileLogger.Error(this.Name, String.Format("InsertResolution - Errore in salvataggio di atto: Message: {0} - StackTrace: {1}", ex.Message, ex.StackTrace));
                    transaction.Rollback();
                }
            }
            Facade.ResolutionLogFacade.Log(resolution, ResolutionLogType.RI, string.Concat("Inserimento atto n.", resolution.InclusiveNumber));
            return true;
        }
        private bool IsAnyNullOrEmpty(JsonResolutionModel myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value) && !string.Equals(value, "AttachmentsDocumentPath"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private ResolutionWorkflow CreateResolutionWorkflow(Resolution resolution, short incremental, byte isActive)
        {
            ResolutionWorkflow resolutionWorkflow = new ResolutionWorkflow();
            resolutionWorkflow.Id = new ResolutionWorkflowCompositeKey() { IdResolution = resolution.Id, Incremental = incremental };
            resolutionWorkflow.IsActive = isActive;
            resolutionWorkflow.IncrementalFather = incremental > 1 ? (short)(incremental - 1) : (short?)null;
            resolutionWorkflow.RegistrationDate = resolution.AdoptionDate.Value;
            resolutionWorkflow.RegistrationUser = DocSuiteContext.Current.User.FullUserName;
            resolutionWorkflow.LastChangedUser = DocSuiteContext.Current.User.FullUserName;
            resolutionWorkflow.Document = resolution.File.IdProposalFile;
            resolutionWorkflow.Attachment = resolution.File.IdAttachements;
            resolutionWorkflow.ResStep = incremental;
            return resolutionWorkflow;
        }
        private bool IsPathValid(string pathToUpload, string pathToDownload)
        {
            if (!string.Equals(pathToUpload, pathToDownload))
            {
                if (Directory.Exists(pathToUpload) && Directory.Exists(pathToDownload))
                {
                    return true;
                }
                else
                {
                    FileLogger.Error("Le cartelle di Upload/Download non esistono.", null);
                    SendMessage("Le cartelle di Upload/Download non esistono.");
                    return false;
                }
            }
            else
            {
                FileLogger.Error("Le cartelle di Upload/Download coincidono.", null);
                SendMessage("Le cartelle di Upload/Download coincidono.");
                return false;
            }
        }
        private void MoveJsonFileDocuments(string folderUploadPath, string folderDownloadPath)
        {
            string jsonPath = GetJsonResolutionFile();
            if(!string.IsNullOrEmpty(jsonPath) && JsonResolutionModel != null)
            {
                MoveFile(jsonPath, folderUploadPath);
                string mainDocumentPath = string.Concat(Parameters.ImportResolutionSourceFolder, JsonResolutionModel.MaindocumentPath);
                MoveFile(mainDocumentPath, folderUploadPath);
                string attachmentsDocumentPath;
                foreach (string path in JsonResolutionModel.AttachmentsDocumentPath)
                {
                    attachmentsDocumentPath = string.Concat(Parameters.ImportResolutionSourceFolder, path);
                    MoveFile(attachmentsDocumentPath, folderUploadPath);
                }
            }
        }
        private void MoveFile(string oldPath, string newFolderPath)
        {
            Guid uniquePath;
            uniquePath = Guid.NewGuid();
            string fileName = Path.GetFileName(oldPath);
            fileName = string.Concat(uniquePath, fileName);
            string newPath = Path.Combine(newFolderPath, fileName);
            File.Move(oldPath, newPath);
            FileLogger.Info(this.Name, "FileMove - File spostati correttamente.");
        }
        private void UploadAndMoveJson()
        {
            if (InsertResolution(JsonResolutionModel))
            {
                MoveJsonFileDocuments(Parameters.ImportResolutionDropFolder, Parameters.ImportResolutionSourceFolder);
                FileLogger.Info(this.Name, "UploadAndMoveJson - Fine operazione con successo.");
            }
            else
            {
                FileLogger.Info(this.Name, "UploadAndMoveJson - Fine operazione con errori.");
            }
            CleanJsonModel();
        }
        #endregion
    }
}
