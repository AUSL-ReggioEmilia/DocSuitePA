using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.AVCP;
using VecompSoftware.DocSuiteWeb.AVCP.Import;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.JeepService.Common;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.AVCP
{
    public class ExcelTaskImporterModule : ModuleBase
    {
        public override void SingleWork()
        {
            try
            {
                FileLogger.Debug(base.Name, "Inizio importazione Task ACQUISTI");
                foreach (TaskHeader byTypeAndStatu in FacadeFactory.Instance.TaskHeaderFacade.GetByTypeAndStatus(TaskTypeEnum.ImportExcelToAVCPAcquisti, TaskStatusEnum.Queued))
                {
                    if (!base.Cancel)
                    {
                        TaskHeader taskHeader = byTypeAndStatu;
                        if (!this.ImportAcquisti(taskHeader))
                        {
                            taskHeader.Status = TaskStatusEnum.OnError;
                        }
                        this.CheckTaskStatus(taskHeader);
                        FacadeFactory.Instance.TaskHeaderFacade.Update(ref taskHeader);
                    }
                    else
                    {
                        FileLogger.Debug(base.Name, "Cancellazione attività per STOP servizio");
                        return;
                    }
                }
                FileLogger.Debug(base.Name, "FINE importazione Task ACQUISTI");
                FileLogger.Debug(base.Name, "Inizio importazione Task PAGAMENTI");
                foreach (TaskHeader byTypeAndStatu1 in FacadeFactory.Instance.TaskHeaderFacade.GetByTypeAndStatus(TaskTypeEnum.ImportExcelToAVCPPagamenti, TaskStatusEnum.Queued))
                {
                    if (!base.Cancel)
                    {
                        TaskHeader taskHeader1 = byTypeAndStatu1;
                        IList<TenderHeader> tenderHeaders = null;
                        try
                        {
                            tenderHeaders = this.ImportPagamenti(taskHeader1);
                        }
                        catch (Exception exception1)
                        {
                            Exception exception = exception1;
                            taskHeader1.Status = TaskStatusEnum.OnError;
                            TaskDetail taskDetail = new TaskDetail()
                            {
                                DetailType = DetailTypeEnum.ErrorType,
                                Title = "Errore in fase di elaborazione righe.",
                                ErrorDescription = exception.Message
                            };
                            taskHeader1.AddDetail(taskDetail);
                        }
                        this.CheckTaskStatus(taskHeader1);
                        FileLogger.Debug(base.Name, "Inizio aggiornamento PAGAMENTI");
                        if (taskHeader1.Status >= TaskStatusEnum.DoneWithWarnings)
                        {
                            TaskDetail taskDetail1 = new TaskDetail()
                            {
                                DetailType = DetailTypeEnum.Info,
                                Title = "Aggiornamento pagamenti."
                            };
                            taskHeader1.AddDetail(taskDetail1);
                            this.UpdatePagamenti(taskHeader1, tenderHeaders);
                        }
                        FileLogger.Debug(base.Name, "FINE aggiornamento PAGAMENTI");
                        FacadeFactory.Instance.TaskHeaderFacade.Update(ref taskHeader1);
                    }
                    else
                    {
                        FileLogger.Debug(base.Name, "Cancellazione attività per STOP servizio");
                        return;
                    }
                }
                FileLogger.Debug(base.Name, "FINE importazione Task PAGAMENTI");
            }
            catch (Exception exception2)
            {
                FileLogger.Error(base.Name, "Errore in fase di importazione TASK", exception2);
            }
            NHibernateSessionManager.Instance.CloseTransactionAndSessions();
        }

        private void CheckTaskStatus(TaskHeader _header)
        {
            if (_header.Status == TaskStatusEnum.OnError)
            {
                this.SendMessage(string.Format("Il task [{0}] ha generato errori e non è stato portato a termine correttamente.", _header.Id));
                return;
            }
            _header.Status = TaskStatusEnum.Done;
            if (_header.Details.Any<TaskDetail>((TaskDetail d) => d.DetailType == DetailTypeEnum.Warn))
            {
                _header.Status = TaskStatusEnum.DoneWithWarnings;
                this.SendMessage(string.Format("Il task [{0}] è stato portato a termine, ma presenta alcuni problemi da verificare.", _header.Id));
            }
            if (_header.Details.Any<TaskDetail>((TaskDetail d) => d.DetailType == DetailTypeEnum.ErrorType))
            {
                _header.Status = TaskStatusEnum.DoneWithErrors;
                this.SendMessage(string.Format("Il task [{0}] è stato portato a termine, ma ha generato errori.", _header.Id));
            }
        }

        private void UpdatePagamenti(TaskHeader task, IList<TenderHeader> headers)
        {
            FileLogger.Debug(base.Name, string.Concat("UpdatePagamenti ", headers.Count));
            if (headers == null)
            {
                TenderHeaderFacade tenderHeaderFacade = new TenderHeaderFacade();
                DateTime today = DateTime.Today;
                headers = tenderHeaderFacade.GetChangedHeaders(today.AddDays(-1));
            }
            foreach (TenderHeader header in headers)
            {
                if (!base.Cancel)
                {
                    FileLogger.Debug(base.Name, string.Concat("Aggiornamento ", header.Id));
                    SetDataSetResult setDataSetResult = _avcpFacade.UpdateAVCPPayments(header, base.Parameters.Username);
                    FileLogger.Debug(base.Name, string.Concat("Updated ", setDataSetResult.Updated));
                    base.AddToDetails(task, setDataSetResult.Item.Subject, setDataSetResult);
                }
                else
                {
                    FileLogger.Debug(base.Name, "Cancellazione attività per STOP servizio");
                    return;
                }
            }
        }

        private IList<TenderHeader> ImportPagamenti(TaskHeader task)
        {
            List<TenderHeader> tenderHeaders = new List<TenderHeader>();
            TaskParameter taskParameter = task.Parameters.First<TaskParameter>((TaskParameter p) => p.ParameterKey == "FilePath");
            base.CheckPath(task, taskParameter);
            TaskDetail taskDetail = new TaskDetail()
            {
                DetailType = DetailTypeEnum.Debug,
                Title = "Inizio importazione documento"
            };
            task.AddDetail(taskDetail);
            List<DocumentRow> documentRows = base.ReadDocument(task, taskParameter, "TO-TemplatePagamenti.xml");
            if (documentRows == null)
            {
                throw new DocSuiteException("Nessuna riga trovata.");
            }
            string str = Path.Combine(base.Parameters.ExcelConfigFolder, "TO-Contraenti.csv");
            foreach (ImportRowPagamento importRowPagamento in (new TorinoDocumentHandler(string.Empty, str)).AggregatePagamenti(documentRows))
            {
                try
                {
                    TenderLot byCIG = FacadeFactory.Instance.TenderLotFacade.GetByCIG(importRowPagamento.CIG);
                    if (byCIG == null)
                    {
                        byCIG = new TenderLot()
                        {
                            CIG = importRowPagamento.CIG
                        };
                        TaskDetail taskDetail1 = new TaskDetail()
                        {
                            DetailType = DetailTypeEnum.Warn,
                            Title = "Gara non trovata. Lotto orfano creato.",
                            Description = string.Format("CIG [{0}], chiave [{1}], importo [{2}]", importRowPagamento.CIG, importRowPagamento.DocumentKey, importRowPagamento.ImportoLiquidato)
                        };
                        task.AddDetail(taskDetail1);
                    }
                    FacadeFactory.Instance.TenderLotFacade.SetPayment(byCIG, importRowPagamento.CIG, importRowPagamento.DocumentKey, importRowPagamento.ImportoLiquidato);
                    FacadeFactory.Instance.TenderLotFacade.Update(ref byCIG);
                    TaskDetail taskDetail2 = new TaskDetail()
                    {
                        DetailType = DetailTypeEnum.Info,
                        Title = "Pagamento importato correttamente.",
                        Description = string.Format("CIG [{0}], chiave [{1}], importo [{2}]", importRowPagamento.CIG, importRowPagamento.DocumentKey, importRowPagamento.ImportoLiquidato)
                    };
                    task.AddDetail(taskDetail2);
                    if (byCIG.Tender != null)
                    {
                        tenderHeaders.Add(byCIG.Tender);
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    TaskDetail taskDetail3 = new TaskDetail()
                    {
                        DetailType = DetailTypeEnum.ErrorType,
                        Title = "Errore in fase di importazione riga.",
                        Description = importRowPagamento.CIG,
                        ErrorDescription = exception.Message
                    };
                    task.AddDetail(taskDetail3);
                }
            }
            return tenderHeaders;
        }

        protected bool ImportAcquisti(TaskHeader task)
        {
            List<Notification> notifications;
            bool flag;
            TaskParameter taskParameter = task.Parameters.First<TaskParameter>((TaskParameter p) => p.ParameterKey == "FilePath");
            base.CheckPath(task, taskParameter);
            TaskDetail taskDetail = new TaskDetail()
            {
                DetailType = DetailTypeEnum.Debug,
                Title = "Inizio importazione documento"
            };
            task.AddDetail(taskDetail);
            List<DocumentRow> documentRows = base.ReadDocument(task, taskParameter, "TO-TemplateAggiudicatari.xml");
            if (documentRows == null)
            {
                return false;
            }
            List<DocumentData> documentDatas = null;
            try
            {
                string str = Path.Combine(base.Parameters.ExcelConfigFolder, "TO-Contraenti.csv");
                TorinoDocumentHandler torinoDocumentHandler = new TorinoDocumentHandler(string.Empty, str);
                documentDatas = torinoDocumentHandler.BuildDocuments(documentRows, out notifications);
                if (notifications.Count <= 0)
                {
                    base.ImportRows(task, documentDatas, Parameters.FindAllResolutionTypes);
                    return true;
                }
                else
                {
                    foreach (Notification notification in notifications)
                    {
                        TaskDetail taskDetail1 = new TaskDetail()
                        {
                            DetailType = DetailTypeEnum.ErrorType,
                            Title = string.Concat(notification.ErrorID, ":", notification.Message),
                            ErrorDescription = notification.ExceptionMessage
                        };
                        task.AddDetail(taskDetail1);
                    }
                    flag = false;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                TaskDetail taskDetail2 = new TaskDetail()
                {
                    DetailType = DetailTypeEnum.ErrorType,
                    Title = "Errore in fase di elaborazione righe.",
                    ErrorDescription = exception.Message
                };
                task.AddDetail(taskDetail2);
                flag = false;
            }
            return flag;
        }

    }
}
