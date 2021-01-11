using System;
using System.Linq;
using System.ServiceModel.Activation;
using System.ServiceModel;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Preservation.Services;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Enums;
using System.Collections.Generic;
using BiblosDS.Library.Common.Utility;
using VecompSoftware.ServiceContract.BiblosDS.Preservations;

[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Preservation : IPreservation
{
    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Preservation));  
    #region IPreservation Members

    /// <summary>
    /// Esegue la verifica della conservazione, ritorna true o false a seconda dell'esito della verifica
    /// Sempre salva il file con l'esito nella directory della conservazione 
    /// </summary>
    /// <param name="IdPreservation"></param>
    /// <returns></returns>
    public bool VerifyPreservation(Guid IdPreservation)
    {
        try
        {
            PreservationService info = new PreservationService();
            return info.VerifyExistingPreservation(IdPreservation); 
        }
        catch (Exception ex)
        {            
            logger.Error(ex);
            throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }       
    }

    public BiblosDS.Library.Common.Objects.PreservationEvents[] PreservationHistory(Guid IdPreservation)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Ritorna se il task di conservazione è stato eseguito
    /// </summary>
    /// <param name="IdTask"></param>
    /// <returns></returns>
    public int GetPreservationTaskStatus(Guid IdTask)
    {
        PreservationTask thisTask = GetPreservationTask(IdTask);
        if (thisTask.Executed == true)
            return 1;
        else
            return 0; 
    }

    public bool PreparePreservationTask(Guid IdTask)
    {
        throw new NotImplementedException();
    }

    public Guid GetIdPreservation(Guid IdTask)
    {
        throw new NotImplementedException();
    }

    public string GetClosingPreservationHash(Guid IdPreservation)
    {
        try
        {
            var info = new PreservationService().GetPreservation(IdPreservation, false);
            if (info == null)
                throw new Exception("Nessuna conservazione presente con l'id passato.");
            var dirInfo = new DirectoryInfo(info.Path);
            var files = dirInfo.GetFiles("*" + info.Label + ".txt.*");
            if (files.Any(x => x.Extension.Contains("p7m") || x.Extension.Contains("x7m")))
            {
                return UtilityService.GetHash(File.ReadAllBytes(files.Where(x => x.Extension.Contains("p7m") || x.Extension.Contains("x7m")).FirstOrDefault().FullName), new PreservationService().MustUseSHA256Mark());
            }else
            {
                return "File di chiusura firmato non ancora presente.";
            }
        }
        catch (Exception ex)
        {            
            logger.Error(ex);
            throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }       
    }

    public bool ConfirmSignedPreservation()
    {
        throw new NotImplementedException();
    }



    #endregion
    
    public BiblosDS.Library.Common.Objects.DocumentContent GetPreservationAdEMark(Guid idPreservation, BiblosDS.Library.Common.Enums.DocumentContentFormat outputFormat)
    {
        try
        {
            throw new NotImplementedException();     
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }           
    }
    
    public BiblosDS.Library.Common.Objects.DocumentContent GetPreservationCloseFile(Guid idPreservation, BiblosDS.Library.Common.Enums.DocumentContentFormat outputFormat)
    {
        logger.InfoFormat("GetPreservationCloseFile - id preservation {0}", idPreservation);
        try
        {        
            var info = new PreservationService().GetPreservationClosingFileInfo(idPreservation);
            if (info == null)
                throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception("File di chiusura non trovato");           
            switch (outputFormat)
            {
                case BiblosDS.Library.Common.Enums.DocumentContentFormat.Binary:
                    return new BiblosDS.Library.Common.Objects.DocumentContent { Blob = info.File, Description = info.FileName };
                case BiblosDS.Library.Common.Enums.DocumentContentFormat.ConformBinary:
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BoblosDSConvWs.BiblosDSConv"]))
                    {
                        BiblosDS.WCF.WCFServices.BoblosDSConvWs.BiblosDSConv wsStampaConforme = new BiblosDS.WCF.WCFServices.BoblosDSConvWs.BiblosDSConv();
                        wsStampaConforme.Url = ConfigurationManager.AppSettings["BoblosDSConvWs.BiblosDSConv"].ToString();
                        var conformDoc = wsStampaConforme.ToRaster(new BiblosDS.WCF.WCFServices.BoblosDSConvWs.stDoc { Blob = Convert.ToBase64String(info.File), FileExtension = info.FileName });
                        return new BiblosDS.Library.Common.Objects.DocumentContent { Description = ".pdf", Blob = Convert.FromBase64String(conformDoc.Blob) };
                    }
                    else
                    {
                        return new BiblosDS.Library.Common.Objects.DocumentContent { Blob = info.File, Description = info.FileName };
                    }
                default:
                    throw new FormatException("Formato di output non supportato.");
            }
        }
        catch (Exception ex)
        {            
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.Info("GetPreservationCloseFile - END");
        }     
    }

    public BindingList<BiblosDS.Library.Common.Objects.PreservationTask> CreatePreservationTask(BindingList<BiblosDS.Library.Common.Objects.PreservationTask> tasks)
    {
        logger.Info("CreatePreservationTask - START");
        try
        {
            return new PreservationService().CreatePreservationTask(tasks);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.Info("CreatePreservationTask - END");
        }
    }

    public BindingList<BiblosDS.Library.Common.Objects.PreservationTask> UpdatePreservationTask(BindingList<BiblosDS.Library.Common.Objects.PreservationTask> tasks, bool updateCorrelatedTasks)
    {
        logger.Info("UpdatePreservationTask - START");
        try
        {
            return new PreservationService().UpdatePreservationTask(tasks, updateCorrelatedTasks);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.Info("UpdatePreservationTask - END");
        }
    }

    public PreservationTask GetPreservationTask(Guid idTask)
    {
        logger.InfoFormat("GetPreservationTask - id task {0}", idTask);
        try
        {
            var ret = new PreservationService().GetPreservationTask(idTask);
            logger.InfoFormat("GetPreservationTask - il task {0} stato trovato.", (ret != null) ? "E'" : "NON E'");
            return ret;
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
    }

    public PreservationTaskResponse GetPreservationTasksByArchive(BindingList<BiblosDS.Library.Common.Objects.DocumentArchive> archives, int skip, int take)
    {
        logger.Info("GetPreservationTasksByArchive - START");
        try
        {
            var ret = new PreservationService().GetPreservationTasks(archives, skip, take);
            logger.Info("GetPreservationTasksByArchive - END");
            return ret;
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
    }

    public BindingList<PreservationTaskDatesResponse> GetNextPreservationTaskDatesForArchive(Guid idArchive, BindingList<PreservationTaskTypes> types)
    {
        logger.InfoFormat("GetNextPreservationTaskDatesForArchive - id archive {0}", idArchive);
        try
        {
            return new PreservationService().GetNextPreservationTaskDatesForArchive(idArchive, types);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.Info("GetNextPreservationTaskDatesForArchive - END");
        }
    }

    public PreservationInfoResponse ExecutePreservationTask(Guid idTask)
    {
        try
        {
            logger.InfoFormat("ExecutePreservationTask - id task {0}", idTask);
            var svc = new PreservationService();
            //Check if idTask exists
            var taskToExecute = svc.GetPreservationTask(idTask);
            PreservationTask taskPreservation = null;
            if (taskToExecute == null)
                throw new Exception("Nessun task trovato con l'id passato");

            if (taskToExecute.TaskType.Type != PreservationTaskTypes.Verify) //Anche il tipo Unknown viene trattato come se fosse il task di conservazione.
            {
                taskPreservation = taskToExecute;
                if (taskToExecute.CorrelatedTasks != null)
                {
                    taskToExecute = taskToExecute
                        .CorrelatedTasks
                        .FirstOrDefault(x => x.TaskType != null && x.TaskType.Type == PreservationTaskTypes.Verify);

                    if (taskToExecute == null)
                        taskToExecute = taskPreservation;
                }
            }

            try
            {
                if (!taskToExecute.Enabled)
                    throw new Exception("Il task di {0} non risulta abilitato.");
                if (taskToExecute.HasError)
                    throw new Exception("Il task di {0} è stato eseguito con errori.");
                if (taskToExecute.Executed)
                    throw new Exception("Il task di {0} non è stato ancora eseguito.");

                if (taskPreservation != null)
                {
                    taskToExecute = taskPreservation;

                    if (!taskToExecute.Enabled)
                        throw new Exception("Il task di {0} non risulta abilitato.");
                    if (taskToExecute.HasError)
                        throw new Exception("Il task di {0} è stato eseguito con errori.");
                    if (!taskToExecute.Executed)
                        throw new Exception("Il task di {0} non è stato ancora eseguito.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ex.Message, taskToExecute.TaskType.Type == PreservationTaskTypes.Verify ? "verifica" : "conservazione"));
            }

            return new BiblosDS.WCF.WCFServices.PreservationServiceInstances().ExecutePreservation(taskPreservation);                
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.InfoFormat("ExecutePreservationTask END {0}", idTask);
        }        
    }

    public bool EnablePreservationTaskByActivationPin(Guid idTaskToEnable, Guid activationPin, short mininumDaysOffset)
    {
        var ret = false;
        try
        {
            ret = new PreservationService().EnablePreservationTaskByActivationPin(idTaskToEnable, activationPin, mininumDaysOffset);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.InfoFormat("EnablePreservationTaskByActivationPin - END {0}", (ret) ? "Ok" : "Ko");
        }
        return ret;
    }


    public IDictionary<Guid, BiblosDsException> UpdateDocumentMetadata(Guid idArchive, IDictionary<Guid, BindingList<DocumentAttributeValue>> documentAttributes)
    {
        IDictionary<Guid, BiblosDsException> ret;
        try
        {
            ret = new PreservationService().UpdateDocumentMetadata(idArchive, documentAttributes);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.InfoFormat("EnablePreservationTaskByActivationPin - END");
        }
        return ret;
    }


    public BiblosDS.Library.Common.Objects.Preservation GetPreservationFromTask(Guid idTask)
    {
        try
        {
            logger.DebugFormat("GetPreservationFromTask {0}", idTask);
            return new PreservationService().GetPreservationsByIdTask(idTask);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.InfoFormat("GetPreservationFromTask - END");
        }
    }


    public void RemovePendigPreservation(Guid idArchive)
    {
        try
        {
            logger.DebugFormat("RemovePendigPreservation {0}", idArchive);
            new PreservationService().RemovePendigPreservation(idArchive);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            if (ex is FaultException)
                throw ex;
            else
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
        finally
        {
            logger.InfoFormat("RemovePendigPreservation - END");
        }
    }
}
