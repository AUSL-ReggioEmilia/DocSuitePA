using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Services.CMVGroup;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.PDF;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;
using VecompSoftware.Services.StampaConforme;

namespace VecompSoftware.JeepService.ResolutionWorkflowManager
{
    public class ResolutionWorkflowManagerModule : JeepModuleBase<ResolutionWorkflowManagerParameters>
    {
        private const string PRIVACYLEVEL_ATTRIBUTE = "PrivacyLevel";

        private ResolutionFacade _currentResolutionFacade;
        private ResolutionActivityFacade _currentResolutionActivityFacade;
        private ResolutionLogFacade _currentResolutionLogFacade;

        private ResolutionFacade CurrentResolutionFacade
        {
            get
            {
                if (_currentResolutionFacade == null)
                {
                    _currentResolutionFacade = new ResolutionFacade();
                }
                return _currentResolutionFacade;
            }
        }

        private ResolutionLogFacade CurrentResolutionLogFacade
        {
            get
            {
                if (_currentResolutionLogFacade == null)
                {
                    _currentResolutionLogFacade = new ResolutionLogFacade();
                }
                return _currentResolutionLogFacade;
            }
        }

        private ResolutionActivityFacade CurrentResolutionActivityFacade
        {
            get
            {
                if (_currentResolutionActivityFacade == null)
                {
                    _currentResolutionActivityFacade = new ResolutionActivityFacade();
                }
                return _currentResolutionActivityFacade;
            }
        }



        public override void SingleWork()
        {
            string activityType = string.Empty;
            try
            {
                FileLogger.Info(Name, "SingleWork - Inizio elaborazione");

                if (FoldersExist())
                {
                    ICollection<ResolutionActivity> publicationActivities = CurrentResolutionActivityFacade.GetToBeProcessedByType(ResolutionActivityType.Publication);
                    if (publicationActivities != null && publicationActivities.Count > 0)
                    {
                        int counter = 0;
                        int counterOk = 0;
                        int counterError = 0;
                        activityType = "Pubblicazione Web";
                        foreach (ResolutionActivity activity in publicationActivities)
                        {
                            counter = counter + 1;
                            WebPublication(activity, ref counter, ref counterOk, ref counterError);
                        }

                        string successMessage = counterOk == 1 ? " pubblicazione avvenuta " : " pubblicazioni avvenute ";
                        string errorMessage = counterError == 1 ? " errore rilevato." : " errori rilevati.";
                        FileLogger.Info(Name, String.Concat("SingleWork - ", publicationActivities.Count, " attività da processare: ", counterOk, successMessage, "con successo, ", counterError, errorMessage));
                    }
                }

                ICollection<ResolutionActivity> effectivenessActivities = CurrentResolutionActivityFacade.GetToBeProcessedByType(ResolutionActivityType.Effectiveness);
                if (effectivenessActivities != null && effectivenessActivities.Count > 0)
                {
                    int counter = 0;
                    int counterOk = 0;
                    int counterError = 0;
                    activityType = "Esecutività";
                    foreach (ResolutionActivity activity in effectivenessActivities)
                    {
                        counter = counter + 1;
                        Effectiveness(activity, ref counterOk, ref counterError);
                    }

                    string successMessage = counterOk == 1 ? " esecutività avvenuta " : " esecutività avvenute ";
                    string errorMessage = counterError == 1 ? " errore rilevato." : " errori rilevati.";
                    FileLogger.Info(Name, String.Concat("SingleWork - ", effectivenessActivities.Count, " attività da processare: ", counterOk, successMessage, "con successo, ", counterError, errorMessage));
                }

                FileLogger.Debug(Name, "SingleWork - Fine elaborazione");
            }
            catch (Exception ex)
            {
                FileLogger.Error(this.Name, String.Format("SingleWork - Errore in elaborazione: Message: {0} - StackTrace: {1}", ex.Message, ex.StackTrace));
                string bodyMessage = String.Concat("Errore in [SingleWork] nel processo di ", activityType,
                                                   " degli atti. Non tutti le attività degli atti previste per oggi sono state processate correttamente. \nErrore: ", ex.Message, " \nStacktrace: ", ex.StackTrace);
                SendMessage(bodyMessage);
            }
        }


        private void WebPublication(ResolutionActivity activity, ref int counter, ref int counterOk, ref int counterError)
        {
            string result = string.Empty;

            try
            {
                FileLogger.Info(Name, String.Concat("WebPublication - Inizio elaborazione activity '", activity.Description, "' con Id ", activity.Id));

                string fileName = string.Concat(DocSuiteContext.Current.ResolutionEnv.WebPublishHTMLFile, ".pdf");

                string biblosDsServer = activity.Resolution.Location.DocumentServer;
                if (!string.IsNullOrEmpty(activity.JsonDocuments))
                {
                    ResolutionActivityDocumentModel documents = JsonConvert.DeserializeObject<ResolutionActivityDocumentModel>(activity.JsonDocuments);

                    if (documents != null && documents.Ids.Count > 0)
                    {
                        IList<string> files = new List<string>();
                        BiblosDocumentInfo doc;
                        FileInfo file;
                        foreach (Guid idDocument in documents.Ids)
                        {
                            doc = new BiblosDocumentInfo(biblosDsServer, idDocument);
                            if (!doc.Attributes.Any(f=> PRIVACYLEVEL_ATTRIBUTE.Equals(f.Key)))
                            {
                                doc.Attributes.Add(PRIVACYLEVEL_ATTRIBUTE, "0");
                            }
                            file = BiblosFacade.SaveUniquePdfToTempNoSignature(doc, "WebPubTemp.pdf", Parameters.DocumentFolderPath);
                            files.Add(file.FullName);
                        }

                        string tempfileName = FileHelper.UniqueFileNameFormat(fileName, DocSuiteContext.Current.User.UserName);
                        string fileDirectory = Path.Combine(Parameters.DocumentFolderPath, tempfileName);
                        try
                        {
                            if (files.Count == 1)
                            {
                                FileLogger.Info(Name, "WebPublication - Fusione non necessaria, singolo documento.");
                                File.Copy(files.First(), fileDirectory);
                            }

                            if (files.Count > 1)
                            {
                                FileLogger.Info(Name, "WebPublication - Inizio fusione documenti.");
                                PdfMerge managerPDF = new PdfMerge();
                                foreach (string item in files)
                                {
                                    managerPDF.AddDocument(item);
                                }
                                managerPDF.Merge(fileDirectory);
                                FileLogger.Info(Name, string.Concat("WebPublication - Documenti fusi in ", fileDirectory));
                            }

                        }
                        catch (Exception ex)
                        {
                            FileLogger.Error(Name, String.Concat("WebPublication - Si è verificato un errore in fase di fusione documenti.", ex));
                            MoveErrorFile(fileDirectory);
                            throw new Exception("Errore in fasi di fusione dei documenti.", ex);
                        }

                        string signature = ParseString(DocSuiteContext.Current.ResolutionEnv.WebPublishSign, activity.Resolution, string.Empty, counter, documents.Ids.Count);
                        string label = string.Format(DocSuiteContext.Current.ResolutionEnv.WebPublishSignTag, signature);
                        FileInfo info = new FileInfo(fileDirectory);
                        byte[] content;
                        using (FileStream stream = new FileStream(fileDirectory, FileMode.Open, FileAccess.Read))
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            content = reader.ReadBytes(Convert.ToInt32(info.Length));
                        }

                        try
                        {
                            FileLogger.Info(Name, "WebPublication - Conversione documento per pubblicazione Web");
                            content = Service.ConvertToPdf(content, "pdf", label);
                        }
                        catch (Exception ex)
                        {
                            FileLogger.Error(Name, String.Concat("WebPublication - Errore in conversione documento: ", ex.Message));
                            MoveErrorFile(fileDirectory);
                            throw new Exception("Errore in conversione documento.", ex);
                        }

                        try
                        {

                            MemoryDocumentInfo tempdocument = new MemoryDocumentInfo(content, fileName);
                            CmvGroup cmvGroup = new CmvGroup();

                            if (!cmvGroup.Publish(activity.Resolution, tempdocument, out result))
                            {
                                throw new InvalidOperationException(result);
                            }
                            FileLogger.Info(Name, "WebPublication - Documento inviato al portale correttamente.");

                        }
                        catch (Exception ex)
                        {
                            string message = string.Concat("Errore in fase di invio documento al portale: ", tempfileName, " - ", result);
                            FileLogger.Error(Name, String.Concat("WebPublication - ", message, ". ErrorMessage: ", ex.Message));
                            MoveErrorFile(fileDirectory);
                            throw new Exception(message, ex);
                        }


                        //Salvo i dati sul db
                        Resolution resolution = activity.Resolution;
                        resolution.WebState = Resolution.WebStateEnum.Published;
                        resolution.WebPublicationDate = DateTime.Today;
                        resolution.WebPublicatedDocuments = documents.IsPrivacy ? "1" : "0";
                        resolution.WebPublicatedDocuments = string.Concat(resolution.WebPublicatedDocuments, "|");

                        CurrentResolutionFacade.Update(ref resolution);
                        CurrentResolutionLogFacade.Insert(resolution, ResolutionLogType.RP, String.Concat("JeepService - Pubblicazione atto n. ", resolution.Id, " avvenuta con successo: ", result));
                        FileLogger.Info(Name, String.Concat("WebPublication - Pubblicazione atto n.", resolution.Id, " avvenuta con successo."));
                        activity.Status = ResolutionActivityStatus.Processed;
                        CurrentResolutionActivityFacade.Update(ref activity);
                        counterOk = counterOk + 1;
                    }

                }

            }
            catch (Exception ex)
            {
                activity.Status = ResolutionActivityStatus.ProcessedWithErrors;
                CurrentResolutionActivityFacade.Update(ref activity);
                CurrentResolutionLogFacade.Log(activity.Resolution, ResolutionLogType.RE, String.Concat("JeepService - Errore in fase di pubblicazione atto n. ", activity.Resolution.Id, ": ", ex.Message));
                FileLogger.Error(Name, string.Concat("WebPublication - ", ex.Message), ex);
                string bodyMessage = string.Concat("Errore in [WebPublication] nella pubblicazione web dell'Atto ", activity.Resolution.InclusiveNumber, " n. ", activity.Resolution.Id,
                                                   ". \nErrore: ", ex.Message, " \nStacktrace: ", ex.StackTrace);
                SendMessage(bodyMessage);
                counterError = counterError + 1;
            }

        }

        private bool FoldersExist()
        {
            if (!Directory.Exists(Parameters.DocumentFolderPath))
            {
                FileLogger.Error(string.Concat("SingleWork - La cartella al percorso ", Parameters.DocumentFolderPath, " non esiste."), null);
                SendMessage(string.Concat("Errore in [SingleWork] - La cartella al percorso ", Parameters.DocumentFolderPath, " non esiste."));
                return false;
            }

            if (!Directory.Exists(Parameters.ErrorFolderPath))
            {
                FileLogger.Error(string.Concat("SingleWork - La cartella al percorso ", Parameters.ErrorFolderPath, " non esiste."), null);
                SendMessage(string.Concat("Errore in [SingleWork] - La cartella al percorso ", Parameters.ErrorFolderPath, " non esiste."));
                return false;
            }

            return true;
        }

        private void MoveErrorFile(string oldPath)
        {
            string newFolderPath = Parameters.ErrorFolderPath;
            MoveFile(oldPath, newFolderPath);
        }

        private void MoveFile(string oldPath, string newFolderPath)
        {
            string fileName = Path.GetFileName(oldPath); ;
            string newPath = Path.Combine(newFolderPath, fileName);
            File.Move(oldPath, newPath);
            FileLogger.Info(this.Name, "MoveFile - File spostati correttamente.");
        }

        private string ParseString(string signature, Resolution resolution, string filename, int counter, int total)
        {
            signature = signature.Replace("{filename}", filename);
            signature = signature.Replace("{year}", resolution.Year.ToString());
            signature = signature.Replace("{number}", resolution.Number.ToString().PadLeft(5, '0'));
            signature = signature.Replace("{fullnumber}", CurrentResolutionFacade.CalculateFullNumber(resolution, resolution.Type.Id, false));
            signature = signature.Replace("{type}", resolution.Type.Description);

            if (total > 1)
            {
                signature = signature.Replace("{counter}", String.Concat("(Documento ", counter, ")"));
            }
            else
            {
                signature = signature.Replace("{counter}", String.Empty);
            }

            if (resolution.AdoptionDate.HasValue)
            {
                signature = signature.Replace("{AdoptionDate}", resolution.AdoptionDate.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                signature = signature.Replace("{AdoptionDate}", String.Empty);
            }

            if (resolution.PublishingDate.HasValue)
            {
                signature = signature.Replace("{PublishingDate}", resolution.PublishingDate.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                signature = signature.Replace("{PublishingDate}", String.Empty);
            }


            if (resolution.EffectivenessDate.HasValue)
            {
                signature = signature.Replace("{EffectivenessDate}", resolution.EffectivenessDate.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                signature = signature.Replace("{EffectivenessDate}", String.Empty).Replace("{EffectivenessDate}", String.Empty);
            }
            signature = signature.Replace("{object}", resolution.ResolutionObject);

            return signature;
        }

        private void Effectiveness(ResolutionActivity activity, ref int counterOk, ref int counterError)
        {
            string result = string.Empty;

            try
            {
                FileLogger.Info(Name, String.Concat("Esecutività - Inizio elaborazione activity '", activity.Description, "' con Id ", activity.Id));


                if (activity.Resolution.WebState == Resolution.WebStateEnum.Published)
                {

                    Resolution resolution = activity.Resolution;

                    if (CurrentResolutionFacade.UpdateEffectivenessDate(resolution.Id.ToString(), activity.ActivityDate.Date, DocSuiteContext.Current.User.FullUserName))
                    {
                        CurrentResolutionFacade.SendUpdateResolutionCommand(resolution);

                        CurrentResolutionLogFacade.Insert(resolution, ResolutionLogType.RP, String.Concat("Esecutività atto n.", resolution.Id, " avvenuta con successo: ", result));
                        activity.Status = ResolutionActivityStatus.Processed;
                        CurrentResolutionActivityFacade.Update(ref activity);
                        counterOk = counterOk + 1;
                    }
                    else
                    {
                        activity.Status = ResolutionActivityStatus.ProcessedWithErrors;
                        CurrentResolutionLogFacade.Log(activity.Resolution, ResolutionLogType.RE, String.Concat("Errore aggioramento dello stato in esecutività dell'atto n.", activity.Resolution.Id));
                        string message = string.Concat("Errore in [Effectiveness] - L'atto ", activity.Resolution.Id, " non è stato correttamente portato in esecutività.");
                        SendMessage(message);
                        throw new Exception(message);
                    }

                }
                else
                {
                    activity.Status = ResolutionActivityStatus.ProcessedWithErrors;
                    FileLogger.Error(string.Concat("SingleWork - L'atto ", activity.Resolution.Id, " non è stato ancora pubblicato, impossibile portare in esecutività."), null);
                    string message = string.Concat("Errore in [Effectiveness] - L'atto ", activity.Resolution.Id, " non è stato ancora pubblicato, impossibile portare in esecutività.");
                    SendMessage(message);
                    throw new Exception(message);
                }

            }
            catch (Exception ex)
            {
                activity.Status = ResolutionActivityStatus.ProcessedWithErrors;
                CurrentResolutionActivityFacade.Update(ref activity);
                CurrentResolutionLogFacade.Log(activity.Resolution, ResolutionLogType.RE, String.Concat("Errore in fase di esecutività atto n.", activity.Resolution.Id, ": ", ex.Message));
                FileLogger.Error(Name, string.Concat("Effectiveness - ", ex.Message), ex);
                string bodyMessage = string.Concat("Errore in [Effectiveness] nell'esecutività dell'Atto ", activity.Resolution.InclusiveNumber, " n. ", activity.Resolution.Id,
                                                   ". \nErrore: ", ex.Message, " \nStacktrace: ", ex.StackTrace);
                SendMessage(bodyMessage);
                counterError = counterError + 1;
            }
        }
    }
}

