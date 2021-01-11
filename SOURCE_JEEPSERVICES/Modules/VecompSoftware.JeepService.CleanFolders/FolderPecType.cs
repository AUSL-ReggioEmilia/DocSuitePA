using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.CleanFolders
{
    public class FolderPecType : FolderBaseType
    {
        #region [ Variables ]
        private const string Searchpattern = "*mail.eml";
        private List<EmlItem> _emlItems = new List<EmlItem>();
        private Func<bool> _cancelFunc;
        private string _loggerName;
        private FacadeFactory _facadeFactory;
        private Location _location;
        #endregion

        #region [ Properties ]

        public FacadeFactory FacadeFactory
        {
            get { return _facadeFactory ?? (_facadeFactory = new FacadeFactory()); }
        }

        public Location Location
        {
            get { return _location ?? (_location = new Location()); }
        }
        #endregion

        #region [ Constructor ]
        public FolderPecType(Func<bool> cancelFunc, Action<string> sendMessage, string loggerName, string path, CleanFoldersParameters parameters)
            : base(path, sendMessage, parameters)
        {
            _emlItems = new List<EmlItem>();
            _cancelFunc = cancelFunc;
            _loggerName = loggerName;
        }
        #endregion

        #region [ Methods ]
        private IEnumerable<EmlItem> RetriveListFilesByParams(string path)
        {
            var fileList = new DirectoryInfo(path).GetFiles(Searchpattern, SearchOption.TopDirectoryOnly);
            //Verifico se è impostato il parametro per la gestione del numero giorni massimo da mantenere
            if (Parameters.MaxNumberDaysToKeep > 0)
                return fileList.Where(x => x.CreationTime < DateTime.Now.Subtract(TimeSpan.FromDays(Parameters.MaxNumberDaysToKeep))).Select(s => new EmlItem(s));

            return fileList.Select(s => new EmlItem(s));
        }

        public void Process()
        {
            _emlItems.AddRange(RetriveListFilesByParams(FolderPath));
            var counter = _emlItems.Count();
            FileLogger.Debug(_loggerName, String.Format("[FolderPecType - Process]. Inizio processo {0} file.", counter));
            foreach (var ei in _emlItems)
            {
                if (_cancelFunc())
                {
                    FileLogger.Info(_loggerName, "Chiusura modulo invocata dall'utente.");
                    return;
                }
                if (CheckExistPec(ei))
                {
                    FileLogger.Debug(_loggerName, String.Format(
                        "[FolderPecType - Process]. La PEC con UID {0} e OriginalRecipient '{1}' è correttamente salvata in DB, elimino il file {2} dalla directory di backup.",
                        ei.MailUid, ei.Recipient, ei.Name));
                    DeleteFile(Path.Combine(FolderPath, ei.Name));
                }
                else
                {
                    var message = String.Format("Errore in [FolderPecType - Process]. Nessuna PEC trovata con UID {0} e OriginalRecipient '{1}'.", ei.MailUid, ei.Recipient);
                    FileLogger.Error(_loggerName, message);
                    UpdateCleanFoldersException(message, FolderType.Pec);
                }
            }
            FileLogger.Debug(_loggerName, String.Format("[FolderPecType - Process]. Fine processo {0} file.", counter));
        }

        private bool CheckExistPec(EmlItem item)
        {
            //Verifico se la PEC esiste nel sistema
            return FacadeFactory.PECMailFacade.HeaderChecksumExists(item.HeaderHash, item.Recipient);            
        }

        public override void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    FileLogger.Debug(_loggerName,
                        String.Format(
                            "[FolderPecType - DeleteFile]. Cancellazione del file {0} avvenuta correttamente.",
                            Path.GetFileName(filePath)));
                }
                else
                {
                    throw new FileNotFoundException("File non trovato", filePath);
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(_loggerName, String.Format(
                    "Errore in [FolderPecType - DeleteFile]. E' avvenuto un errore nella cancellazione del file {0}. {1}{2}",
                    filePath, Environment.NewLine, ex.Message));
                UpdateCleanFoldersException(String.Format(
                    "Errore in [FolderPecType - DeleteFile]. E' avvenuto un errore nella cancellazione del file {0}.",
                    filePath), FolderType.Pec);
            }
        }

        #endregion
    }
}
