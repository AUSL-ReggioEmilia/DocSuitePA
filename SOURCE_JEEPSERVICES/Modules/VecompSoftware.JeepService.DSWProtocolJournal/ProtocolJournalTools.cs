using System;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.JeepService.SignEngine;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService
{
    class ProtocolJournalTools
    {
        #region [ Fields. ]

        private static string _loggerName;
        private static SignEngineFacade _signEngine;

        #endregion

        #region [ Properties. ]

        public static string LoggerName
        {
            get
            {
                return string.IsNullOrEmpty(_loggerName) ? "Application" : _loggerName;
            }
            set { _loggerName = value; }
        }

        public static SignEngineFacade SignEngine
        {
            get
            {
                if (_signEngine == null)
                {
                    throw new InvalidOperationException("SignEngine non inizializzato");
                }

                return _signEngine;
            }
            set
            {
                _signEngine = value;
            }
        }

        public static void InitializeSignEngine()
        {
            _signEngine = new SignEngineFacade(SignEngineUrl, SignEngineUser, SignEnginePassword, CertificateName, InfocamereFormat);
        }

        public static string SignEngineUrl { get; set; }
        public static string SignEngineUser { get; set; }
        public static string SignEnginePassword { get; set; }
        public static string CertificateName { get; set; }
        public static int InfocamereFormat { get; set; }

        #endregion

        #region [ Methods. ]

        public static int SaveJournalToBiblos(Location location, TempFileDocumentInfo document)
        {
            var uidDocument = document.ArchiveInBiblos(location.DocumentServer, location.ProtBiblosDSDB);
            // Questa chiamata potrebbe essere lenta con DB Biblos troppo grande (ENPACL)
            const int attemps = 10;
            Exception lastException = null;
            for (var i = 0; i < attemps; i++)
            {
                try
                {
                    var bdi = new BiblosDocumentInfo(location.DocumentServer, uidDocument.DocumentId);
                    return bdi.BiblosChainId;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    FileLogger.Error(LoggerName, string.Format("Tentativo di caricamento idBiblos #{0}/10 fallito per {1}", i, ex.Message), ex);
                    // Riposo il thread 1 minuto
                    Thread.Sleep(20000 * i);
                }
            }
            // Se sono qui significa che non sono riuscito a caricare il numero per 10 volte
            throw new Exception(String.Format("Impossibile caricare l'id Biblos per il registro corrente. GuidDocument = {0}", uidDocument), lastException);
        }
        #endregion
    }
}
