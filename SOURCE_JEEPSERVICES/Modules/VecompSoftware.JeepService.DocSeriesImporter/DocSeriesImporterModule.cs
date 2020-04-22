using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;

using DocSuiteWebAPI = VecompSoftware.DocSuiteWeb.API;
using System.IO;

namespace VecompSoftware.JeepService.DocSeriesImporter
{
    public class DocSeriesImporterModule : JeepModuleBase<DocSeriesImporterParameters>
    {
        private Importer importer = null;

        public override void Initialize(List<Common.Parameter> parameters)
        {
            FileLogger.Info(this.Name, "Initialize");
            base.Initialize(parameters);
            this.importer = new Importer(this.Name, this.Parameters.DropFolder, this.Parameters.DoneFolderName, this.Parameters.ErrorFolder, this.Parameters.DeleteDoc_Main, this.Parameters.DeleteDoc_Annexed, this.Parameters.DeleteDoc_Unpulished, this.Parameters.MaxTimes_ReWorkError);

            if (!CreateAppFolders())
                return;
        }

        public override void SingleWork()
        {
            try
            {
                FileLogger.Info(this.Name, "SingleWork - Inizio elaborazione");

                DownloadFiles();
                ProcessFiles();

                FileLogger.Debug(this.Name, "SingleWork - Fine elaborazione");
            }
            catch (Exception ex)
            {
                FileLogger.Error(this.Name, String.Format("SingleWork - Errore in elaborazione: Message: {0} - StackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }


        private bool CreateAppFolders()
        {
            string folderName = "";
            FileLogger.Info(this.Name, "CreateAppFolders");
            try
            {
                //drop folder
                if (String.IsNullOrWhiteSpace(Parameters.DropFolder))
                    Parameters.DropFolder = DocSeriesImporterParameters.DropFolderDefault;

                folderName = Parameters.DropFolder;
                var dropFolder = new DirectoryInfo(Parameters.DropFolder);
                if (!dropFolder.Exists)
                    dropFolder.Create();

                //done folder
                if (String.IsNullOrWhiteSpace(Parameters.DoneFolderName))
                    Parameters.DoneFolderName = DocSeriesImporterParameters.DoneFolderNameDefault;

                folderName = Parameters.DoneFolderName;
                var doneFolder = new DirectoryInfo(Parameters.DoneFolderName);
                if (!doneFolder.Exists)
                    doneFolder.Create();

                //errorFolder
                if (String.IsNullOrWhiteSpace(Parameters.ErrorFolder))
                    Parameters.ErrorFolder = DocSeriesImporterParameters.ErrorFolderDefault;
                folderName = Parameters.ErrorFolder;
                var errorFolder = new DirectoryInfo(Parameters.ErrorFolder);
                if (!errorFolder.Exists)
                    errorFolder.Create();

                return true;
            }
            catch (Exception ex)
            {
                string err = String.Format("Errore in [CreateAppFolders] - Folder:{0}\nErrore: {1} \nStacktrace: {2}", folderName, ex.Message, FullStacktrace(ex));
                FileLogger.Error(Name, err, ex);
                SendMessage(err);

                return false;
            }
        }


        private void DownloadFiles()
        {
            //determina i task da eseguire
            TaskStatusEnum filter = TaskStatusEnum.Queued;

#if LOCALTEST
#warning LOCALTEST - TaskStatusEnum.Done
      filter = TaskStatusEnum.DoneWithErrors;
#endif
            //scarica files
            IEnumerable<TaskHeader> tasks = FacadeFactory.Instance.TaskHeaderFacade.GetByTypeAndStatus((TaskTypeEnum)100, filter).ToList();
            foreach (TaskHeader task in tasks)
            {
                try
                {
                    FileLogger.Debug(this.Name, String.Format("{0} {1} {2} {3}", this.importer, task, this.Parameters, this.Parameters.DropFolder));
                    this.importer.DownloadFile(task, this.Parameters.DropFolder);
                }
                catch (Exception ex)
                {
                    FileLogger.Error(this.Name, String.Format("Errore in DownloadFiles: Task.Id:{0} - Message: {1} - StackTrace: {2}", task.Id, ex.Message, ex.StackTrace));
                }
            }
        }

        private void ProcessFiles()
        {
            //elabora file scaricati
            string[] files = TaskInfo.GetInfoFiles(this.Parameters.DropFolder);

            foreach (string filename in files)
            {
                try
                {
                    int rowCount = 0;
                    int totalRow = 0;
                    bool res = this.importer.ProcessFile(filename, ref rowCount, ref totalRow);

                    //aggiorna il task
                    var tInfo = TaskInfo.Load(filename);
                    TaskHeader task = FacadeFactory.Instance.TaskHeaderFacade.GetById(tInfo.taskId);
                    DocSuiteWebAPI.DocumentSeriesItemDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<DocSuiteWebAPI.DocumentSeriesItemDTO>(task.Code);
                    if (dto.MaxTimesError <= this.Parameters.MaxTimes_ReWorkError)
                    {
                        if (res && !tInfo.HasErrors())
                        {
                            string message = string.Format("Importazione conclusa con SUCCESSO. {0} righe importate su {1} totali.", rowCount, totalRow);

                            FileLogger.Info(Name, message);
                            task.Status = TaskStatusEnum.Done;

                            tInfo.CopyStatus(Parameters.DoneFolderName);
                            tInfo.RemoveFiles();

                            TaskDetail taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.Info, Title = message, TaskHeader = task };
                            FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);
                        }
                        else
                        {
                            string message = string.Format("Importazione conclusa con ERRORI. {0} righe importate su {1} totali.", rowCount, totalRow);

                            FileLogger.Info(Name, message);
                            task.Status = TaskStatusEnum.DoneWithErrors;

                            tInfo.taskError = message;
                            tInfo.Save();

                            TaskDetail taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.ErrorType, Title = message, TaskHeader = task };
                            FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);
                        }
                        //aggiorna stato task
                        FacadeFactory.Instance.TaskHeaderFacade.Update(ref task);
                        VecompSoftware.NHibernateManager.NHibernateSessionManager.Instance.CloseTransactionAndSessions();

                        System.Threading.Thread.Sleep(this.Parameters.SleepSeconds * 1000);
                    }
                    else
                    {
                        string message = string.Format("Importazione conclusa con ERRORI perchè il file è stato riprocessato più di tre volte.", rowCount, totalRow);

                        FileLogger.Info(Name, message);
                        task.Status = TaskStatusEnum.DoneWithErrors;

                        tInfo.taskError = message;
                        tInfo.CopyStatus(Parameters.ErrorFolder);
                        tInfo.Save();

                        TaskDetail taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.ErrorType, Title = message, TaskHeader = task };
                        FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);
                        dto.MaxTimesError++;
                        task.Code = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                    }
                    FacadeFactory.Instance.TaskHeaderFacade.Update(ref task);
                    VecompSoftware.NHibernateManager.NHibernateSessionManager.Instance.CloseTransactionAndSessions();
                }
                catch (Exception ex)
                {
                    var tInfo = TaskInfo.Load(filename);
                    tInfo.taskError = ex.Message;
                    tInfo.Save();

                    TaskHeader task = FacadeFactory.Instance.TaskHeaderFacade.GetById(tInfo.taskId);
                    FileLogger.Error(this.Name, String.Format("Errore in ProcessFiles: Filename:{0} - Message: {1} - StackTrace: {2}", filename, ex.Message, ex.StackTrace));
                    TaskDetail taskDetail = new TaskDetail()
                    {
                        DetailType = DetailTypeEnum.ErrorType,
                        Title = String.Format("Errore in ProcessFiles: Filename:{0} - Message: {1}", filename, ex.Message),
                        ErrorDescription = ex.StackTrace,
                        TaskHeader = task
                    };
                    FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);

                }
            }
        }


    }
}

