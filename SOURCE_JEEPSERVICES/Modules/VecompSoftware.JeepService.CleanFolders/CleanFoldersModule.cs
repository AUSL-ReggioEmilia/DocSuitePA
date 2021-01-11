using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.CleanFolders
{
    public class CleanFoldersModule : JeepModuleBase<CleanFoldersParameters>
    {
        private FolderSimpleType _simpleTypeFolder;
        private FolderPecType _pecTypeFolder;

        public override void Initialize(List<Parameter> parameters)
        {
            base.Initialize(parameters);

            if (!Directory.Exists(Parameters.TempFolder))
            {
                Directory.CreateDirectory(Parameters.TempFolder);
            }

            if (!Parameters.FoldersList.Any())
            {
                FileLogger.Error(Name, "Errore in [Initialize] - E' avvenuto un errore nella lettura delle Directory da processare.");
                throw new ArgumentOutOfRangeException();
            }

            var chkFolders = CheckFoldersExist();

            if (chkFolders) return;
            FileLogger.Error(Name, "Errore in [Initialize] - Alcune directory specificate non esistono.");
            throw new DirectoryNotFoundException();
        }

        private bool CancelRequest()
        {
            return Cancel;
        }

        public override void SingleWork()
        {
            FileLogger.Info(Name, "Inizio procedura di pulizia Directory selezionate.");
            switch (Parameters.FolderType)
            {
                case FolderType.Simple:
                    FileLogger.Info(Name, "Tipologia di directory da processare: Simple.");
                    foreach (var item in Parameters.FoldersList)
                    {
                        FileLogger.Debug(Name, String.Format("Processo la directory: {0}", item));
                        if (CancelRequest())
                        {
                            FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                            return;
                        }
                        _simpleTypeFolder = new FolderSimpleType(CancelRequest, SendMessage, Name, item, Parameters);
                        _simpleTypeFolder.Process();
                    }
                    _simpleTypeFolder.MailErrorMessage();
                    break;
                case FolderType.Pec:
                    FileLogger.Info(Name, "Tipologia di directory da processare: Pec.");
                    foreach (var item in Parameters.FoldersList)
                    {
                        FileLogger.Debug(Name, String.Format("Processo la directory: {0}", item));
                        if (CancelRequest())
                        {
                            FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                            return;
                        }
                        _pecTypeFolder = new FolderPecType(CancelRequest, SendMessage, Name, item, Parameters);
                        _pecTypeFolder.Process();
                    }
                    _pecTypeFolder.MailErrorMessage();
                    break;
                default:
                    throw new NotImplementedException();
            }
            FileLogger.Info(Name, "Fine procedura di pulizia Directory selezionate");
        }

        private bool CheckFoldersExist()
        {
            return Parameters.FoldersList.All(Directory.Exists);
        }
    }
}
