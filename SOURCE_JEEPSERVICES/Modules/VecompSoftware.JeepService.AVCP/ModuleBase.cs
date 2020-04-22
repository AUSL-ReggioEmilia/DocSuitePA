using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;
using VecompSoftware.DocSuiteWeb.AVCP;
using System.IO;
using VecompSoftware.DocSuiteWeb.AVCP.Import;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Biblos;

namespace VecompSoftware.JeepService.AVCP
{
    public abstract class ModuleBase : JeepModuleBase<AVCPParameters>
    {
        #region [ Fields ]
        private FacadeFactory _facade;
        private AVCPFacade _avcpfacade;
        internal AVCPFacade _avcpFacade = new AVCPFacade();

        protected const string AttributeAnno = "Anno";
        protected const string AttributeCodiceSettore = "CodiceSettore";
        protected const string AttributeNumero = "Numero";
        protected const string AttributeChiusa = "Chiusa";
        protected const string AttributeDataUltimoAggiornamento = "DataUltimoAggiornamento";
        protected const string AttributeUrlFile = "UrlFile";
        protected const string AttributeLicenza = "Licenza";
        protected const string AttributeAbstract = "Abstract";
        protected const string AttributeAnnoRiferimento = "AnnoRiferimento";
        protected const string AttributeTitolo = "Titolo";
        protected const string AttributeDataPubblicazione = "DataPubblicazione";
        protected const string AttributeEntePubblicatore = "EntePubblicatore";
        #endregion

        #region [ Properties ]
        protected FacadeFactory Facade
        {
            get { return _facade ?? (_facade = new FacadeFactory("ReslDB")); }
        }

        protected AVCPFacade AvcpFacade
        {
            get { return _avcpfacade ?? (_avcpfacade = new AVCPFacade()); }
        }

        protected string ModulePath
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(ModuleBase));
                return System.IO.Path.GetDirectoryName(assembly.Location);
            }
        }
        #endregion

        #region [ Virtual ]
        protected virtual void OnSingleWork() { }
        protected virtual bool UpdateDocumentItem(DocumentSeriesItem item)
        {
            return false;
        }
        #endregion

        #region [ Methods ]

        public override void SingleWork()
        {
            try
            {
                FileLogger.Info(Name, string.Format("ModulePath:{0}", this.ModulePath));
                OnSingleWork();
            }
            catch (Exception ex)
            {
                OnError("", ex);
            }
        }

        protected void OnError(string errorMessage, Exception ex)
        {
            var message = string.Format("Error:{0}.", errorMessage);
            FileLogger.Error(Name, message, ex);
            SendMessage(message);
        }

        protected string UpdateDocumentsAvcp()
        {
            int num = 0;
            int num1 = 0;
            string str = "UpdateDocumentsAvcp";
            DocumentSeriesItemFinder documentSeriesItemFinder = new DocumentSeriesItemFinder(base.Parameters.Username)
            {
                EnablePaging = false,
                ItemStatusIn = new List<DocumentSeriesItemStatus>()
                {
                    DocumentSeriesItemStatus.Active
                }
            };
            DocumentSeriesItemFinder documentSeriesItemFinder1 = documentSeriesItemFinder;
            List<int> nums = new List<int>()
            {
                DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.Value
            };
            documentSeriesItemFinder1.IdDocumentSeriesIn = nums;
            documentSeriesItemFinder.IsRetired = new bool?(false);
            List<DocumentQueryItem> documentQueryItems = ModuleBase.LoadTestList(Path.Combine(this.ModulePath, "doclist.csv"));
            if (documentQueryItems != null && documentQueryItems.Count > 0)
            {
                FileLogger.Debug(base.Name, string.Format("Verifica {0} documenti", documentQueryItems.Count));
                documentQueryItems.ForEach((DocumentQueryItem p) => {
                    List<SearchCondition> searchConditions = new List<SearchCondition>()
                    {
                        new SearchCondition()
                        {
                            AttributeName = "CodiceSettore",
                            AttributeValue = p.CodiceServizio,
                            Operator = SearchConditionOperator.IsEqualTo
                        },
                        new SearchCondition()
                        {
                            AttributeName = "Anno",
                            AttributeValue = p.Anno,
                            Operator = SearchConditionOperator.IsEqualTo
                        },
                        new SearchCondition()
                        {
                            AttributeName = "Numero",
                            AttributeValue = p.Numero,
                            Operator = SearchConditionOperator.IsEqualTo
                        }
                    };
                    this.Facade.DocumentSeriesFacade.FillFinder(documentSeriesItemFinder, searchConditions);
                });
            }
            List<DocumentSeriesItem> list = documentSeriesItemFinder.DoSearch().ToList<DocumentSeriesItem>();
            string name = base.Name;
            object count = list.Count;
            int? avcpDocumentSeriesId = DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId;
            FileLogger.Info(name, string.Format("Trovati {0} registrazioni in Serie Documentale {1}.", count, avcpDocumentSeriesId.Value));
            foreach (DocumentSeriesItem documentSeriesItem in list)
            {
                if (!base.Cancel)
                {
                    num1++;
                    if (this.UpdateDocumentItem(documentSeriesItem))
                    {
                        continue;
                    }
                    num++;
                }
                else
                {
                    FileLogger.Info(base.Name, "Modulo interrotto da STOP Servizio");
                    break;
                }
            }
            NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            string str1 = string.Format("{0} - Controllati {1} documenti - {2} Errori.", str, num1, num);
            FileLogger.Info(base.Name, str1);
            return str1;
        }


        protected void SaveToBiblos(Resolution resolution)
        {
            Category byId;
            int num;
            DocumentSeriesFacade documentSeriesFacade = this.Facade.DocumentSeriesFacade;
            int? avcpDocumentSeriesId = DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId;
            DocumentSeries documentSeries = documentSeriesFacade.GetById(avcpDocumentSeriesId.Value, false);
            BiblosChainInfo biblosChainInfo = new BiblosChainInfo();
            biblosChainInfo.AddAttribute("Anno", resolution.Year.ToString());
            biblosChainInfo.AddAttribute("CodiceSettore", resolution.CodiceServizio());
            int num1 = resolution.NumeroAtto();
            biblosChainInfo.AddAttribute("Numero", num1.ToString());
            biblosChainInfo.AddAttribute("Chiusa", "0");
            DocumentSeriesItem documentSeriesItem = new DocumentSeriesItem();
            if (base.Parameters.DefaultCategory <= 0)
            {
                num = (resolution.SubCategory != null ? resolution.SubCategory.Id : resolution.Category.Id);
                byId = this.Facade.CategoryFacade.GetById(num, false);
            }
            else
            {
                byId = this.Facade.CategoryFacade.GetById(base.Parameters.DefaultCategory, false);
            }
            if (byId.Root != byId)
            {
                documentSeriesItem.Category = byId.Root;
                documentSeriesItem.SubCategory = byId;
            }
            else
            {
                documentSeriesItem.Category = byId;
            }
            documentSeriesItem.DocumentSeries = documentSeries;
            documentSeriesItem.Subject = resolution.ResolutionObject;
            this.Facade.DocumentSeriesItemFacade.SaveDocumentSeriesItem(documentSeriesItem, biblosChainInfo, null, null, DocumentSeriesItemStatus.Active, string.Format("Importato da [{0}].", base.Name));
            this.Facade.ResolutionDocumentSeriesItemFacade.LinkResolutionToDocumentSeriesItem(resolution, documentSeriesItem);
            ResolutionLogFacade resolutionLogFacade = this.Facade.ResolutionLogFacade;
            object[] name = new object[] { documentSeriesItem.DocumentSeries.Container.Name, documentSeriesItem.Year, documentSeriesItem.Number, base.Name };
            resolutionLogFacade.Log(resolution, ResolutionLogType.SD, string.Format("Inserimento in Serie Documentale [{0}] da [{3}] : {1}/{2:000000}", name));
        }

        protected static List<DocumentQueryItem> LoadTestList(string csvFile)
        {
            List<DocumentQueryItem> items = new List<DocumentQueryItem>();
            if (!System.IO.File.Exists(csvFile))
            {
                return items;
            }

            HashSet<string> hash = new HashSet<string>();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(csvFile))
            {
                while (sr.Peek() > 0)
                {
                    string line = sr.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(line))
                    {
                        line = line.Trim();
                        if (!hash.Contains(line))
                        {
                            hash.Add(line);

                            string[] fields = line.Split(';');
                            if (fields.Length != 3)
                                continue;

                            items.Add(new DocumentQueryItem
                            {
                                CodiceServizio = fields[0].Trim(),
                                Anno = Int32.Parse(fields[1].Trim()),
                                Numero = Int32.Parse(fields[2].Trim())
                            });
                        }
                    }
                }
                return items;
            }
        }

        protected void CheckPath(TaskHeader task, TaskParameter path)
        {
            if (path == null || string.IsNullOrEmpty(path.Value))
            {
                TaskDetail taskDetail = new TaskDetail()
                {
                    DetailType = DetailTypeEnum.ErrorType,
                    Title = "Parametro FilePath mancante."
                };
                task.AddDetail(taskDetail);
                FileLogger.Error(base.Name, "Parametro FilePath mancante.");
                throw new DocSuiteException("Parametro FilePath mancante");
            }
            if (!File.Exists(path.Value))
            {
                TaskDetail taskDetail1 = new TaskDetail()
                {
                    DetailType = DetailTypeEnum.ErrorType,
                    Title = "Documento non trovato.",
                    Description = path.Value
                };
                task.AddDetail(taskDetail1);
                FileLogger.Error(base.Name, string.Concat("Documento non trovato: ", path.Value));
                throw new DocSuiteException(string.Concat("Documento non trovato: ", path.Value));
            }
        }

        protected List<DocumentRow> ReadDocument(TaskHeader task, TaskParameter path, string templateName)
        {
            List<DocumentRow> documentRows;
            List<DocumentRow> documentRows1 = new List<DocumentRow>();
            try
            {
                string str = Path.Combine(base.Parameters.ExcelConfigFolder, templateName);
                ExcelReader excelReader = new ExcelReader(path.Value, str);
                List<Notification> notifications = new List<Notification>();
                if (excelReader.Fetch(out documentRows1, out notifications))
                {
                    return documentRows1;
                }
                else
                {
                    foreach (Notification notification in notifications)
                    {
                        TaskDetail taskDetail = new TaskDetail()
                        {
                            DetailType = DetailTypeEnum.ErrorType,
                            Title = string.Concat(notification.ErrorID, ":", notification.Message),
                            ErrorDescription = notification.ExceptionMessage
                        };
                        task.AddDetail(taskDetail);
                    }
                    documentRows = null;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                TaskDetail taskDetail1 = new TaskDetail()
                {
                    DetailType = DetailTypeEnum.ErrorType,
                    Title = "Errore in fase di lettura documento.",
                    ErrorDescription = exception.Message
                };
                task.AddDetail(taskDetail1);
                documentRows = null;
            }
            return documentRows;
        }

        protected void AddToDetails(TaskHeader task, string key, SetDataSetResult res)
        {
            object[] step = new object[] { res.Step, res.Updated, res.Flushed, res.LastUpdate, res.Saved, res.Chain.ID };
            string str = string.Format("Ultimo step eseguito {0}; DataSet Aggiornato {1}; DataSet Flushed {2}; DataSet LastUpdate {3}; DataSet Saved {4}; DataSet Chain.ID {5}", step);
            FileLogger.Debug(base.Name, str);
            if (!res.Updated)
            {
                TaskDetail taskDetail = new TaskDetail()
                {
                    DetailType = DetailTypeEnum.Warn,
                    Title = string.Format("[{0}] - Aggiornamento non eseguito.", key),
                    Description = string.Format("Data ultimo aggiornamento dataset in archivio [{0:dd/MM/yyyy}].", res.LastUpdate)
                };
                task.AddDetail(taskDetail);
                return;
            }
            TaskDetail taskDetail1 = new TaskDetail()
            {
                DetailType = DetailTypeEnum.Info,
                Title = string.Format("DataSet [{0}] aggiornato correttamente.", key),
                Description = str
            };
            task.AddDetail(taskDetail1);
            FileLogger.Info(base.Name, string.Format("DataSet [{0}] aggiornato correttamente.", key));
        }

        protected virtual void ImportRows(TaskHeader task, List<DocumentData> rows)
        {
            ImportRows(task, rows, false);
        }

        protected virtual void ImportRows(TaskHeader task, List<DocumentData> rows, bool findAllResolutionTypes)
        {
            foreach (DocumentData row in rows)
            {
                if (!base.Cancel)
                {
                    try
                    {
                        FileLogger.Info(base.Name, string.Format("Importazione {0}", row.DocumentKey));
                        FileLogger.Debug(base.Name, string.Format("AVCPDefaultCategoryId {0}", DocSuiteContext.Current.ProtocolEnv.AVCPDefaultCategoryId));
                        DocumentSeriesItem itemOrCreate = _avcpFacade.GetItemOrCreate(row.DocumentKey, row.Pubblicazione.metadata.titolo, DocSuiteContext.Current.ProtocolEnv.AVCPDefaultCategoryId, base.Parameters.Username);
                        FileLogger.Info(base.Name, string.Format("DocumentSeriesItem {0}", itemOrCreate.Id));
                        row.Pubblicazione.metadata.urlFile = string.Format(DocSuiteContext.Current.ProtocolEnv.AVCPDatasetUrlMask, itemOrCreate.Id);
                        row.Pubblicazione.metadata.entePubblicatore = DocSuiteContext.Current.ProtocolEnv.AVCPEntePubblicatore;
                        row.Pubblicazione.metadata.licenza = DocSuiteContext.Current.ProtocolEnv.AVCPLicenza;
                        Resolution provvedimento = null;
                        if (DocSuiteContext.Current.ProtocolEnv.AVCPLinkToResolution)
                        {
                            FileLogger.Info(base.Name, "LinkToResolution ATTIVO");
                            if(findAllResolutionTypes)
                            {
                                provvedimento = _avcpFacade.GetProvvedimento(row.DocumentKey, false, true);
                            }
                            else
                            {
                                provvedimento = _avcpFacade.GetProvvedimento(row.Anno, row.CodiceServizio, row.Numero, false);
                            }                            
                            FileLogger.Info(base.Name, string.Format("Resolution {0}", provvedimento.Id));
                            _avcpFacade.LinkToResolution(itemOrCreate, provvedimento);
                        }
                        TenderHeader tender = null;
                        try
                        {
                            tender = _avcpFacade.LinkToTender(row.Pubblicazione, provvedimento, itemOrCreate);
                            FileLogger.Info(base.Name, string.Format("Collegato a Tender {0}", tender.Id));
                        }
                        catch (InvalidOperationException invalidOperationException1)
                        {
                            InvalidOperationException invalidOperationException = invalidOperationException1;
                            FileLogger.Error(base.Name, "Errore in LinkToTender", invalidOperationException);
                            TaskDetail taskDetail = new TaskDetail()
                            {
                                DetailType = DetailTypeEnum.ErrorType,
                                Title = string.Format("[{0}] - Errore in collegamento a Gara.", row.DocumentKey),
                                Description = invalidOperationException.Message.ToString()
                            };
                            task.AddDetail(taskDetail);
                            continue;
                        }
                        row.Pubblicazione = _avcpFacade.SetAVCPPayments(itemOrCreate, row.Pubblicazione, tender);
                        FileLogger.Debug(base.Name, "SetAVCPPayments eseguito");
                        SetDataSetResult setDataSetResult = _avcpFacade.SetDataSetPub(row.Pubblicazione, itemOrCreate);
                        FileLogger.Debug(base.Name, "SetDataSetPub eseguito");
                        this.AddToDetails(task, row.DocumentKey, setDataSetResult);
                        FileLogger.Debug(base.Name, "AddToDetails eseguito");
                        if (itemOrCreate.PublishingDate.HasValue)
                        {
                            string name = base.Name;
                            DateTime? publishingDate = itemOrCreate.PublishingDate;
                            FileLogger.Debug(name, string.Format("DocumentSeriesItem già pubblicata {0}", publishingDate.Value));
                            TaskDetail taskDetail1 = new TaskDetail()
                            {
                                DetailType = DetailTypeEnum.Info,
                                Title = string.Format("[{0}] - Pubblicazione non necessaria.", row.DocumentKey)
                            };
                            DateTime? nullable = itemOrCreate.PublishingDate;
                            taskDetail1.Description = string.Format("Data pubblicazione [{0:dd/MM/yyyy}]", nullable.Value);
                            task.AddDetail(taskDetail1);
                        }
                        else
                        {
                            FileLogger.Debug(base.Name, "!item.PublishingDate.HasValue");
                            List<string> strs = new List<string>();
                            if (!_avcpFacade.ValidateAVCP(row.Pubblicazione, out strs))
                            {
                                string str = string.Join("|", strs);
                                TaskDetail taskDetail2 = new TaskDetail()
                                {
                                    DetailType = DetailTypeEnum.ErrorType,
                                    Title = string.Format("[{0}] - Pubblicazione non eseguita.", row.DocumentKey),
                                    Description = str
                                };
                                task.AddDetail(taskDetail2);
                                FileLogger.Warn(base.Name, string.Format("DocumentSeriesItem NON pubblicata: {0}", str));
                            }
                            else
                            {
                                itemOrCreate.PublishingDate = new DateTime?(DateTime.Today);
                                FacadeFactory.Instance.DocumentSeriesItemFacade.Update(ref itemOrCreate);
                                TaskDetail taskDetail3 = new TaskDetail()
                                {
                                    DetailType = DetailTypeEnum.Info,
                                    Title = string.Format("[{0}] - Pubblicazione eseguita.", row.DocumentKey)
                                };
                                DateTime? publishingDate1 = itemOrCreate.PublishingDate;
                                taskDetail3.Description = string.Format("Data pubblicazione [{0:dd/MM/yyyy}]", publishingDate1.Value);
                                task.AddDetail(taskDetail3);
                                string name1 = base.Name;
                                DateTime? nullable1 = itemOrCreate.PublishingDate;
                                FileLogger.Info(name1, string.Format("DocumentSeriesItem pubblicata {0}", nullable1.Value));
                            }
                        }
                    }
                    catch (SetDataSetResultException setDataSetResultException1)
                    {
                        SetDataSetResultException setDataSetResultException = setDataSetResultException1;
                        object[] step = new object[] { setDataSetResultException.PartialResult.Step, setDataSetResultException.PartialResult.Updated, setDataSetResultException.PartialResult.Flushed, setDataSetResultException.PartialResult.LastUpdate, setDataSetResultException.PartialResult.Saved, setDataSetResultException.PartialResult.Chain.ID };
                        string str1 = string.Format("Ultimo step eseguito {0}; DataSet Aggiornato {1}; DataSet Flushed {2}; DataSet LastUpdate {3}; DataSet Saved {4}; DataSet Chain.ID {5}", step);
                        FileLogger.Error(base.Name, str1, setDataSetResultException);
                        TaskDetail taskDetail4 = new TaskDetail()
                        {
                            DetailType = DetailTypeEnum.ErrorType,
                            Title = string.Format("Errore in elaborazione riga {0}", row.DocumentKey),
                            Description = str1,
                            ErrorDescription = setDataSetResultException.Message
                        };
                        task.AddDetail(taskDetail4);
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        FileLogger.Error(base.Name, string.Format("Errore generico in elaborazione riga {0}", row.DocumentKey), exception);
                        TaskDetail taskDetail5 = new TaskDetail()
                        {
                            DetailType = DetailTypeEnum.ErrorType,
                            Title = string.Format("Errore generico in elaborazione riga {0}", row.DocumentKey),
                            ErrorDescription = exception.Message
                        };
                        task.AddDetail(taskDetail5);
                        return;
                    }
                }
                else
                {
                    FileLogger.Debug(base.Name, "Cancellazione attività per STOP servizio");
                    return;
                }
            }
        }


        /// <summary>
        /// Importo i pagamenti AVCP nella Tender Lot.
        /// Sono importati solo i pagamenti estratti dai nuovi file XML generati.
        /// 
        /// Non viene attualmente utilizzato poichè le sommatorie sono già correntemente calcolate nel file XML e aggiornate con il metodo SetAVCPPayments.
        /// </summary>
        /// <param name="docs">Lista dei documenti generati</param>
        /// <returns></returns>
        public List<TaskHeader> ImportPayments(List<DocumentData> docs)
        {
            List<TaskHeader> tasks = new List<TaskHeader>();
            TaskHeader task = new TaskHeader();
            List<TenderHeader> tor = new List<TenderHeader>();
            List<ImportRowPagamento> pagamenti = new List<ImportRowPagamento>();
            try
            {
                // Per ogni documento creato sommo gli importi dei singoli CIG e li persisto nel DB
                foreach (DocumentData doc in docs)
                {
                    // Per ogni CIG presente nel documento creo un row per i pagamenti.
                    foreach (string cig in doc.CigList.Distinct())
                    {
                        ImportRowPagamento pagamento = new ImportRowPagamento();
                        pagamento.DocumentKey = doc.DocumentKey;
                        pagamento.CIG = cig;
                        pagamento.DataAggiornamento = DateTime.Now;
                        pagamento.ImportoLiquidato = doc.Pubblicazione.data.Where(x => x.cig == cig).Sum(x => x.importoSommeLiquidate);
                        pagamenti.Add(pagamento);
                    }
                }
            }
            catch (Exception ex)
            {
                task.AddDetail(new TaskDetail()
                {
                    DetailType = DetailTypeEnum.ErrorType,
                    Title = "Errore in fase di conversione dei dati di importazione nella riga.",
                    ErrorDescription = ex.Message
                });
            }

            foreach (ImportRowPagamento row in pagamenti)
            {
                try
                {
                    TenderLot lot = Facade.TenderLotFacade.GetByCIG(row.CIG);

                    if (lot == null)
                    {
                        lot = new TenderLot();
                        lot.CIG = row.CIG;
                        task.AddDetail(new TaskDetail()
                        {
                            DetailType = DetailTypeEnum.Warn,
                            Title = "Gara non trovata. Lotto orfano creato.",
                            Description = string.Format("CIG [{0}], chiave [{1}], importo [{2}]", row.CIG, row.DocumentKey, row.ImportoLiquidato)
                        });
                    }

                    Facade.TenderLotFacade.SetPayment(lot, row.CIG, row.DocumentKey, row.ImportoLiquidato);
                    Facade.TenderLotFacade.Update(ref lot);
                    task.AddDetail(new TaskDetail()
                    {
                        DetailType = DetailTypeEnum.Info,
                        Title = "Pagamento importato correttamente.",
                        Description = string.Format("CIG [{0}], chiave [{1}], importo [{2}]", row.CIG, row.DocumentKey, row.ImportoLiquidato)
                    });

                    if (lot.Tender != null)
                        tor.Add(lot.Tender);
                }
                catch (Exception ex1)
                {
                    task.AddDetail(new TaskDetail()
                    {
                        DetailType = DetailTypeEnum.ErrorType,
                        Title = "Errore in fase di importazione riga.",
                        Description = row.CIG,
                        ErrorDescription = ex1.Message
                    });
                }
            }
            return tasks;
        }

        #endregion
    }


    public class DocumentQueryItem
    {
        public string CodiceServizio { get; set; }
        public int Anno { get; set; }
        public int Numero { get; set; }
    }

}


