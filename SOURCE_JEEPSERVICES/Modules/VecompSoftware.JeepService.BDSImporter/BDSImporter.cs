using System;
using System.Collections.Generic;
using System.IO;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService
{
    public class BDSImporter : JeepModuleBase<BDSImporterParameters>
    {

        #region [ Properties ]

        private string LoggerName
        {
            get { return Name; }
        }

        #endregion

        #region [ Methods ]

        private string CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                FileLogger.Warn(LoggerName, "Percorso non trovato: " + path);
                FileLogger.Warn(LoggerName, "Creazione in corso...");
                Directory.CreateDirectory(path);
                FileLogger.Warn(LoggerName, "Completata.");
            }
            return path;
        }
        private void InitializeFolders()
        {
            FileLogger.Info(LoggerName, "Inizio sessione di verifica struttura cartelle");
            CreateDirectory(Parameters.WorkingFolder);
            CreateDirectory(Parameters.InputFolder);
            CreateDirectory(Parameters.OutputFolder);
            CreateDirectory(Parameters.XsdsFolder);
            FileLogger.Info(LoggerName, "Fine sessione di verifica struttura cartelle.");
        }

        public override void SingleWork()
        {
            var parser = new DocumentoSanitarioTypeParser(Parameters);
            parser.OnParsingErrorEventHandler += ParsingError;

            var i = int.MaxValue;
            if (Parameters.MaxIterationPerCycle > 0)
                i = Parameters.MaxIterationPerCycle;

            while (i > 0)
                if (parser.StackSize > 0)
                {
                    i--;
                    parser.ImportDocuments();
                }
                else if (Cancel)
                {
                    FileLogger.Info(LoggerName, "Richiesta di STOP ricevuta.");
                    LastExecution = DateTime.Now;
                }
                else
                {
                    FileLogger.Info(LoggerName, "Stack esaurito per: " + parser.GetType().ToString());
                    if (Parameters.LocalCopyEnabled)
                        parser.DisposeLocalCopies();
                    break;
                }
        }
        public override void Initialize(List<Parameter> parameters)
        {
            base.Initialize(parameters);
            InitializeFolders();
        }

        #endregion

        #region [ Events ]

        private void ParsingError(object sender, DocumentParserEventArgs args)
        {
            FileLogger.Info(LoggerName, args.Message);
            SendMessage(args.Message);
        }

        #endregion

    }
}
