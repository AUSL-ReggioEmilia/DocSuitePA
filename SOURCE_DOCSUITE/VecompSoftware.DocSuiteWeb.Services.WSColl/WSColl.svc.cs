using System;
using System.Configuration;
using System.ServiceModel;
using System.Web;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Services.WSColl.ErrorHandler;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.WSColl
{
    [GlobalExceptionHandlerBehaviourAttribute(typeof(GlobalExceptionHandler))]
    public class WSColl : IWSColl
    {
        private const string LoggerName = "WSCollLog";

        private FacadeFactory _facade;
        public FacadeFactory Facade
        {
            get { return _facade ?? (_facade = new FacadeFactory("ProtDB")); }
        }

        public static string ProtocolTemplateCollaborationName
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("DocSuite.WSColl.ProtocolTemplateCollaborationName");
            }
        }

        public int Insert(string xmlColl)
        {
            try
            {
                if (string.IsNullOrEmpty(xmlColl))
                    throw new ArgumentException("xml non valorizzato", "xmlColl");

                FileLogger.Debug(LoggerName, string.Format("Insert - xmlColl: {0}", xmlColl));

                //Utente che stà utilizzando il WCF
                var result = Facade.CollaborationFacade.InsertCollaboration(xmlColl, ServiceSecurityContext.Current.WindowsIdentity.Name, ProtocolTemplateCollaborationName);
                return result;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }                        
        }

        public void AddDocument(int collNumber, byte[] stream, string documentName, bool isMain)
        {
            try
            {
                // Validazione
                if (stream.Length == 0)
                    throw new ArgumentException("Valorizzare lo stream di byte del documento");

                if (string.IsNullOrEmpty(documentName))
                    throw new ArgumentException("Inserire il nome del documento");

                FileLogger.Debug(LoggerName, string.Format("AddDocumentByte - collNumber: {0} base64DocumentStream: {1} documentName: {2} isMain: {3}", collNumber, stream.ToString(), documentName, isMain.ToString()));

                //Utente che stà utilizzando il WCF
                string username = ServiceSecurityContext.Current.WindowsIdentity.Name;
                if (isMain)
                    Facade.CollaborationFacade.BiblosInsert(collNumber, stream, documentName, username);
                else
                    Facade.CollaborationFacade.BiblosInsertAllegati(collNumber, stream, documentName, username);

            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }                       
        }

        public void AddDocument(int collNumber, string base64DocumentStream, string documentName, bool isMain)
        {
            try
            {
                // Validazione
                if (string.IsNullOrEmpty(base64DocumentStream))
                    throw new ArgumentException("Valorizzare lo stream del documento", "base64DocumentStream");

                FileLogger.Debug(LoggerName, string.Format("AddDocument - collNumber: {0} base64DocumentStream: {1} documentName: {2} isMain: {3}", collNumber, base64DocumentStream, documentName, isMain.ToString()));

                var stream = Convert.FromBase64String(base64DocumentStream);
                AddDocument(collNumber, stream, documentName, isMain);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }            
        }

        public void Start(int collNumber)
        {
            try
            {
                FileLogger.Debug(LoggerName, string.Format("Start - collNumber: {0}", collNumber));
                Facade.CollaborationFacade.StartCollaboration(collNumber);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }                        
        }

        public string GetStatus(int collNumber)
        {
            try
            {
                FileLogger.Debug(LoggerName, string.Format("GetStatus - collNumber: {0}", collNumber));
                var status = Facade.CollaborationFacade.GetStatus(collNumber);
                return status;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }            
        }
    }
}
