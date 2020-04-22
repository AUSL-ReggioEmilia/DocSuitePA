using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Services.WSProt.DTO;
using VecompSoftware.DocSuiteWeb.Services.WSProt.ErrorHandler;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.WSProt
{
    [GlobalExceptionHandlerBehaviourAttribute(typeof(GlobalExceptionHandler))]
    public class WSProt : IWSProt
    {
        private const string LoggerName = "WSProtLog";

        private FacadeFactory _facade;
        public FacadeFactory Facade
        {
            get { return _facade ?? (_facade = new FacadeFactory("ProtDB")); }
        }

        public bool ValidateWindowsSecurity()
        {
            try
            {
                return string.IsNullOrEmpty(ServiceSecurityContext.Current.WindowsIdentity.Name);
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException("Utente anonimo non supportato nel servizio di protocollo");
            }
            
        }
        public string Insert(string xmlProt)
        {
            try
            {
                ValidateWindowsSecurity();
                ProtocolXML protocolXml;
                try
                {
                    protocolXml = SerializationHelper.SerializeFromString<ProtocolXML>(xmlProt);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Errore in lettura XML.", "xmlColl", ex);
                }

                FileLogger.Debug(LoggerName, string.Format("Insert - xmlProt: {0}", xmlProt));

                // Verifico se sono corretti e completi i dati
                Facade.ProtocolFacade.CheckDataForInsert(protocolXml);

                //Utente che stà utilizzando il WCF
                string username = ServiceSecurityContext.Current.WindowsIdentity.Name;

                // Inserisco il protocollo
                var protocol = Facade.ProtocolFacade.InsertProtocol(protocolXml, username);

                return string.Format("{0}/{1}", protocol.Year, protocol.Number);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public void AddDocument(int year, int number, string base64DocumentStream, string documentName, bool isMain)
        {
            try
            {
                ValidateWindowsSecurity();
                FileLogger.Debug(LoggerName, string.Format("AddDocument - year: {0} number: {1} base64DocumentStream: {2} documentName: {3} isMain: {4}", year, number, base64DocumentStream, documentName, isMain));

                Protocol protocol = Facade.ProtocolFacade.GetById((short)year, number);
                if (protocol == null)
                    throw new ArgumentException("Protocollo non presente. Verificare year e number", "year number");

                byte[] document = Convert.FromBase64String(base64DocumentStream);

                if (!protocol.IdStatus.HasValue || (protocol.IdStatus.HasValue && protocol.IdStatus.Value >= (int)ProtocolStatusId.Annullato))
                {
                    throw new ArgumentException($"Il Protocollo {year}/{number} risulta gestito e dunque è immodificabile.", "ProtocolStatusId");
                }

                if (isMain && protocol.IdDocument.HasValue && protocol.IdStatus.HasValue  && protocol.IdStatus.Value == (int)ProtocolStatusId.Attivo)
                {
                    throw new ArgumentException(
                        string.Format("Il Protocollo {0}/{1} risulta attivo. Non è possibile aggiungere documenti allegato o sostituire il documento principale.", year, number),
                        "year number isMain" );
                }

                //Utente che stà utilizzando il WCF
                string username = ServiceSecurityContext.Current.WindowsIdentity.Name;
                if (isMain)
                {
                    //Documento principale
                    Facade.ProtocolFacade.BiblosInsert(protocol, document, documentName, username);
                }
                else
                {
                    //Allegati
                    if (protocol.IdStatus.Value == (int)ProtocolStatusId.Attivo)
                    {
                        Facade.ProtocolFacade.AddAnnexes(protocol, document, documentName, username);
                    }
                    else
                    {
                        Facade.ProtocolFacade.BiblosInsertAllegati(protocol, document, documentName, username);
                    }
                }
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public void InsertCommit(int year, int number)
        {
            try
            {
                ValidateWindowsSecurity();
                FileLogger.Debug(LoggerName, string.Format("InsertCommit - year: {0} number: {1}", year, number));

                Protocol protocol = Facade.ProtocolFacade.GetById((short)year, number);
                if (protocol == null)
                {
                    throw new ArgumentException("Protocollo non presente. verificare year e number", "protocol");
                }

                if (!protocol.IdDocument.HasValue)
                {
                    throw new InvalidOperationException("Attenzione il protocollo non dispone del documento principale");
                }

                if (!protocol.IdStatus.HasValue || (protocol.IdStatus.HasValue && protocol.IdStatus.Value >= (int)ProtocolStatusId.Annullato))
                {
                    throw new ArgumentException($"Il Protocollo {year}/{number} risulta gestito e dunque è immodificabile.", "ProtocolStatusId");
                }

                if (protocol.IdDocument.HasValue && protocol.IdStatus.HasValue && protocol.IdStatus.Value == (int)ProtocolStatusId.Attivo)
                {
                    throw new ArgumentException(string.Format("Il Protocollo {0}/{1} risulta già attivo.", year, number), "year number");
                }
                //Prima attivazione del Protocollo
                Facade.ProtocolFacade.Activation(protocol);
                Facade.ProtocolFacade.SendInsertProtocolCommand(protocol);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetProtocolLink(int year, int number)
        {
            try
            {
                ValidateWindowsSecurity();
                FileLogger.Debug(LoggerName, string.Format("GetProtocolLink - year: {0} number: {1}", year, number));

                Protocol protocol = Facade.ProtocolFacade.GetById((short)year, number);

                if (protocol == null)
                    throw new ArgumentException(
                        "Attenzione protocollo non presente. Verificare gli identificativi passati come argomento",
                        "year, number");

                if (!protocol.IdDocument.HasValue)
                    throw new InvalidOperationException("Attenzione il protocollo non dispone del documento principale");

                return string.Format("Tipo=Prot&Azione=Apri&Anno={0}&Numero={1}", year, number);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetDocumentsViewerLink(int year, int number)
        {
            try
            {
                ValidateWindowsSecurity();
                FileLogger.Debug(LoggerName, string.Format("GetDocumentsViewerLink - year: {0} number: {1}", year, number));

                Protocol protocol = Facade.ProtocolFacade.GetById((short)year, number);
                if (protocol == null)
                    throw new ArgumentException(
                        "Attenzione protocollo non presente. Verificare gli identificativi passati come argomento",
                        "year, number");

                if (!protocol.IdDocument.HasValue)
                    throw new InvalidOperationException("Attenzione il protocollo non dispone del documento principale");

                return StringHelper.Substring(Facade.ProtocolFacade.GetViewerLink(protocol), '?');
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetPecs(int year, int number)
        {
            try
            {
                ValidateWindowsSecurity();
                FileLogger.Debug(LoggerName, string.Format("GetPecs - year: {0} number: {1}", year, number));

                Protocol protocol = Facade.ProtocolFacade.GetById((short)year, number);
                if (protocol == null)
                    throw new ArgumentException(
                        "Attenzione protocollo non presente. Verificare gli identificativi passati come argomento",
                        "year, number");

                return Facade.ProtocolFacade.GetPecs(protocol);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetProtocolStatuses()
        {
            try
            {
                ProtocolStatusesDto protocolStatusesDto = new ProtocolStatusesDto();
                List< ProtocolStatusDto> dtos = Facade.ProtocolStatusFacade.GetAll().Select(s => new ProtocolStatusDto().MappingFromEntity(s)).ToList();
                dtos.ForEach(protocolStatusesDto.AddProtocolStatus);
                return SerializationHelper.SerializeToStringWithoutNamespace(protocolStatusesDto);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetProtocolInfo(int year, int number)
        {
            try
            {
                ValidateWindowsSecurity();
                FileLogger.Debug(LoggerName, string.Format("GetProtocolInfo - year: {0} number: {1}", year, number));

                Protocol protocol = Facade.ProtocolFacade.GetById((short)year, number);
                if (protocol == null)
                    throw new ArgumentException(
                        "Attenzione protocollo non presente. Verificare gli identificativi passati come argomento",
                        "year, number");

                return Facade.ProtocolFacade.GetProtocolInfo(protocol);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public void SetProtocolStatus(short year, int number, string statusCode)
        {
            try
            {
                ValidateWindowsSecurity();

                if (!DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled)
                    throw new ArgumentException("Attenzione, gestione dello stato di Protocollo non abilitata.");

                Protocol protocol = Facade.ProtocolFacade.GetById(year, number);
                if (protocol == null)
                    throw new Exception("Attenzione protocollo non presente per gli identificativi passati");

                ProtocolStatus status = Facade.ProtocolStatusFacade.GetById(statusCode);
                if(status == null)
                    throw new ArgumentException("Attenzione, nessun ProtocolStatus trovato per il codice passato", "statusCode");

                //Eseguo la modifica dello stato
                protocol.Status = status;
                Facade.ProtocolFacade.Update(ref protocol);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public int GetContact(string contactMailOrCode)
        {
            if (string.IsNullOrWhiteSpace(contactMailOrCode))
            {
                throw new Exception("Ricevuto codice contatto vuoto.");
            }

            Contact contact = null;
            if (RegexHelper.IsValidEmail(contactMailOrCode))
            {
                IList<Contact> foundByMail = FacadeFactory.Instance.ContactFacade.GetByMail(contactMailOrCode);
                if (!foundByMail.IsNullOrEmpty())
                {
                    contact = foundByMail.OrderByDescending(f => f.Id).First();
                }
                if (contact != null)
                {
                    return contact.Id;
                }
            }

            IList<Contact> foundBySearchCode = FacadeFactory.Instance.ContactFacade.GetContactBySearchCode(contactMailOrCode, Convert.ToInt16(1));
            if (!foundBySearchCode.IsNullOrEmpty())
            {
                contact = foundBySearchCode.OrderByDescending(f => f.Id).First();
            }
            if (contact != null)
            {
                return contact.Id;
            }

            throw new Exception(string.Format("Contatto con codice '{0}' non trovato.", contactMailOrCode));
        }
    }
}
