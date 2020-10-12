using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade.Interfaces;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Facade.Observers.TopMedia
{

    class TopMediaObserver : IFacadeObserver<ProtocolFacade>
    {

        #region [ Fields ]

        private FacadeFactory _facade;

        #endregion

        #region [ Properties ]

        public FacadeFactory Facade
        {
            get { return _facade ?? (_facade = new FacadeFactory("ProtDB")); }
        }

        #endregion

        void IFacadeObserver<ProtocolFacade>.Observe(ProtocolFacade target)
        {
            target.AfterInsert += target_AfterInsert;
        }
        void IFacadeObserver<ProtocolFacade>.Disregard(ProtocolFacade target)
        {
            target.AfterInsert -= target_AfterInsert;
        }

        private void logToEntity(Protocol protocol, Exception ex)
        {
            protocol.TDError = ex.Message;
            FileLogger.Warn("FileLog", "TopMedia LogToEntity TDError: " + protocol.TDError);
            FacadeFactory.Instance.ProtocolFacade.UpdateNoLastChange(ref protocol);
            // Commentata perchè genera "System.ObjectDisposedException: Safe handle has been closed" su WindowsIdentity recuperando il nome utente.
            //FacadeFactory.Instance.ProtocolLogFacade.Log(protocol, ProtocolLogEvent.PX, protocol.TDError);
        }
        private void clearEntityLogging(Protocol protocol)
        {
            protocol.TDError = string.Empty;
            Facade.ProtocolFacade.UpdateNoLastChange(ref protocol);
        }

        /// <summary>
        /// Recupera il codice prodotto dal nome del file. 
        /// </summary>
        /// <param name="fileName">Nome del file.</param>
        /// <returns></returns>
        private int getProductId(string fileName)
        {
            // Questo dizionario andrebbe tabellato... - FG
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case FileHelper.DOC:
                    return 3;
                case FileHelper.EML:
                    return 118;
                case FileHelper.GIF:
                case FileHelper.JPEG:
                case FileHelper.JPG:
                case FileHelper.TIF:
                    return 114;
                case FileHelper.HTM:
                case FileHelper.HTML:
                    return 44;
                case FileHelper.MSG:
                    return 116;
                case FileHelper.P7M:
                    return getProductId(Path.GetFileNameWithoutExtension(fileName)); // Chissà se va... - FG
                case FileHelper.PDF:
                    return 14;
                case FileHelper.PPT:
                    return 74;
                case FileHelper.TXT:
                    return 24;
                case FileHelper.XLS:
                    return 34;
                case FileHelper.ZIP:
                    return 94;
                default:
                    string message = "Nessun ProductId trovato per il file {0}.";
                    message = string.Format(message, fileName);
                    throw new Exception(message);
            }
        }
        private int countAttachments(Protocol protocol)
        {
            if (protocol.IdAttachments.GetValueOrDefault(0) > 0)
                try
                {
                    var chain = new UIDChain(protocol.Location.ProtBiblosDSDB, protocol.IdAttachments.Value);
                    var result = BiblosDocumentInfo.GetDocuments(chain);

                    if (!result.IsNullOrEmpty())
                        return result.Count;
                }
                catch (Exception ex)
                {
                    string message = "countAttachments: " + ex.Message;
                    throw new Exception(message, ex);
                }
            return 0;
        }
        private IDictionary<string, string> getDocumentAttributes(Protocol protocol)
        {
            var attributes = new Dictionary<string, string>();

            attributes.Add("Registro protocollo", protocol.Container.Name);
            attributes.Add("Numero protocollo", protocol.Number.ToString());
            attributes.Add("Anno registro", protocol.Year.ToString());
            attributes.Add("Data protocollo", string.Format("{0:dd/MM/yyyy}", protocol.RegistrationDate.ToLocalTime()));
            attributes.Add("Oggetto", protocol.ProtocolObject);
            attributes.Add("Tipo movimento", protocol.Type.Description);
            attributes.Add("Tipo posta", protocol.ServiceCategory);
            attributes.Add("Numero documento", protocol.DocumentProtocol);
            attributes.Add("Data documento", string.Format("{0:dd/MM/yyyy}", protocol.DocumentDate));

            attributes.Add("Annullato", "N");
            if (protocol.IdStatus == (int)ProtocolStatusId.Annullato)
            {
                attributes["Annullato"] = "S";
                attributes.Add("Data di annullamento", string.Format("{0:dd/MM/yyyy}", protocol.LastChangedDate));
                attributes.Add("Motivazione annullamento", protocol.LastChangedReason);
            }

            attributes.Add("Note", protocol.Note);
            attributes.Add("Disponibile", "S");

            Category iterator =  protocol.Category;
            var categories = new List<string>();
            while (iterator.Parent != null)
            {
                categories.Add(iterator.GetFullName());
                iterator = iterator.Parent;
            }
            categories.Reverse(); // Riordino dal generale al particolare.

            var areaDS = categories.Count > 0 ? categories[0] : null;
            var categoriaDS = categories.Count > 1 ? categories[1] : null;
            var classeDS = categories.Count > 2 ? categories[2] : null;
            var sottoclasseDS = categories.Count > 3 ? categories[3] : null;

            attributes.Add("Area DS", areaDS);
            attributes.Add("Categoria DS", categoriaDS);
            attributes.Add("Classe DS", classeDS);
            attributes.Add("Sottoclasse DS", sottoclasseDS);

            // Recupero i contatti da rubrica.
            var senders = protocol.Contacts
                .Where(c => c.ComunicationType.Eq("M"))
                .Select(c => c.Contact.Description);
            var recipients = protocol.Contacts
                .Where(c => c.ComunicationType.Eq("D"))
                .Select(c => c.Contact.Description);

            // Recupero i contatti manuali.
            var contactManuals = Facade.ProtocolContactManualFacade.GetByProtocol(protocol);
            var manualSenders = contactManuals
                .Where(c => c.ComunicationType.Eq("M"))
                .Select(c => c.Contact.Description);
            var manualRecipients = contactManuals
                .Where(c => c.ComunicationType.Eq("D"))
                .Select(c => c.Contact.Description);

            senders = senders.Concat(manualSenders);
            recipients = recipients.Concat(manualRecipients);

            attributes.Add("Ragione sociale mittente", null);
            if (!senders.IsNullOrEmpty())
                attributes["Ragione sociale mittente"] = string.Join(", ", senders);

            attributes.Add("Ragione sociale destinataria", null);
            if (!recipients.IsNullOrEmpty())
                attributes["Ragione sociale destinataria"] = string.Join(", ", recipients);

            if (!string.IsNullOrEmpty(protocol.Subject))
                attributes.Add(
                    protocol.Type.Id == -1 ? "Ufficio Destinatario" : "Ufficio Mittente",
                    protocol.Subject);
            attributes.Add("Numero allegati", countAttachments(protocol).ToString());

            // Vincoli TopMedia.
            var validated = new Dictionary<string, string>(attributes);
            foreach (var item in attributes.Keys)
            {
                if (string.IsNullOrEmpty(validated[item]))
                    validated[item] = string.Empty;
                if (validated[item].Length > 250)
                    validated[item] = validated[item].Substring(0, 250);
            }

            return validated;
        }
        private IDictionary<string, string> getAttachmentAttributes(Protocol protocol)
        {
            IDictionary<string, string> attributes = new Dictionary<string, string>();

            attributes.Add("Numero Protocollo", protocol.Number.ToString());
            attributes.Add("Data Protocollo", string.Format("{0:dd/MM/yyyy}", protocol.RegistrationDate.ToLocalTime()));
            attributes.Add("Registro Protocollo", protocol.Container.Name);
            attributes.Add("Anno Registro", protocol.Year.ToString());
            attributes.Add("Oggetto Allegato", protocol.ProtocolObject);
            attributes.Add("Numero Allegato", null); // ATTENZIONE! Viene inizializzato successivamente.

            // Vincoli TopMedia.
            var validated = new Dictionary<string, string>(attributes);
            foreach (var item in attributes.Keys)
            {
                if (string.IsNullOrEmpty(validated[item]))
                    validated[item] = string.Empty;
                if (validated[item].Length > 250)
                    validated[item] = validated[item].Substring(0, 250);
            }

            return validated;
        }

        private string[] attributesToArray(IDictionary<string, string> attributes)
        {
            IList<string> result = new List<string>();
            foreach (KeyValuePair<string, string> item in attributes)
                if (!string.IsNullOrEmpty(item.Value))
                    result.Add(string.Format("{0};{1}", item.Key, item.Value));
            return result.ToArray<string>();
        }

        /// <summary>
        /// Verifica la validità dei parametri di connessione al servizio TopMedia.
        /// </summary>
        private void verifyParameters()
        {
            if (string.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.UserName))
                throw new Exception("verifyParameters: TopMediaParameters.UserName mancante o non valido.");
            if (string.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.Password))
                throw new Exception("verifyParameters: TopMediaParameters.Password mancante o non valido.");
            if (string.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.DocumentType))
                throw new Exception("verifyParameters: TopMediaParameters.DocumentType mancante o non valido.");
            if (string.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType))
                throw new Exception("verifyParameters: TopMediaParameters.AttachmentType mancante o non valido.");
            if (string.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.Language))
                throw new Exception("verifyParameters: TopMediaParameters.Language mancante o non valido.");
        }
        private bool HasArchivableContainer(Protocol protocol)
        {
            return DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.ContainerIdentifiers.Contains(protocol.Container.Id);
        }
        private bool HasArchivableRole(Protocol protocol)
        {
            foreach (int archivableRole in DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.RoleIdentifiers)
                if (protocol.ContainsRole(archivableRole))
                    return true;
            return false;
        }
        private bool IsArchivable(Protocol protocol)
        {
            return HasArchivableContainer(protocol) || HasArchivableRole(protocol);
        }

        private string login()
        {
            try
            {

                string result = null;
                var message = string.Format("TopMedia Login - username: {0}, password: {1}, language: {2}",
                    DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.UserName,
                    DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.Password,
                    DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.Language);
                FileLogger.Info("FileLog", message);
#if DEBUG
                result = Guid.NewGuid().ToString("N");
#else
                result =  Services.TopMedia.Service.Login(
                    DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.UserName,
                    DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.Password,
                    DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.Language);
#endif
                FileLogger.Info("FileLog", "TopMedia - sessionid: " + result);
                return result;
            }
            catch (Exception ex)
            {
                string message = "Service.Login: {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }
        }

        /// <summary>
        /// Archivia un documento.
        /// </summary>
        /// <param name="sessionId">Identificativo della sessione di lavoro.</param>
        /// <param name="documentType">Tipologia di documento (documento/allegato).</param>
        /// <param name="attributes">Dizionario degli attributi del documento.</param>
        /// <param name="fileName">Nome del file da archiviare.</param>
        /// <param name="blob">Blob del file da archiviare.</param>
        /// <returns></returns>
        //        private int archive(string sessionId, string documentType, IDictionary<string, string> attributes, string fileName, byte[] blob)
        //        {
        //            string[] keys = attributes.Keys.ToArray<string>();
        //            string[] values = attributes.Values.ToArray<string>();
        //            int? productId = getProductId(fileName);

        //#if DEBUG
        //            return 1;
        //#else
        //            // return Services.TopMedia.Service.Archive(sessionId, documentType, keys, values, fileName, blob, productId);
        //#endif
        //        }
        //private int archive(string sessionId, string documentType, IDictionary<string, string> attributes, DocumentInfo documentInfo)
        //{
        //    return archive(sessionId, documentType, attributes, documentInfo.Name, documentInfo.Stream);
        //}
        //private int archive(string sessionId, string documentType, IDictionary<string, string> attributes, FileInfo fileInfo)
        //{
        //    byte[] blob = FileHelper.StreamFile(fileInfo);
        //    return archive(sessionId, documentType, attributes, fileInfo.Name, blob);
        //}

        /// <summary>
        /// Elimina l'archiviazione di uno specifico identificativo di documento.
        /// </summary>
        /// <param name="sessionId">Identificativo della sessione di lavoro.</param>
        /// <param name="documentType">Tipologia di documento (documento/allegato).</param>
        /// <param name="identifier">Identificativo del documento da eliminare.</param>
        private void purge(string sessionId, string documentType, int identifier)
        {
            try
            {
                var message = string.Format("TopMedia Purge - sessionid: {0}, documenttype: {1}, identifier: {2}",
                    sessionId, documentType, identifier);
                FileLogger.Info("FileLog", message);
#if DEBUG
#else
                Services.TopMedia.Service.Purge(sessionId, true, documentType, identifier);
#endif
            }
            catch { }
        }
        private void purge(string sessionId, string documentType, IList<int> identifiers)
        {
            foreach (int id in identifiers)
                purge(sessionId, documentType, id);
        }
        /// <summary>
        /// Aggiorna l'archiviazione di un documento.
        /// </summary>
        /// <param name="sessionId">Identificativo della sessione di lavoro.</param>
        /// <param name="documentType">Tipologia di documento (documento/allegato).</param>
        /// <param name="attributes">Dizionario con gli attributi da aggiornare.</param>
        /// <returns></returns>
        private int update(string sessionId, string documentType, IDictionary<string, string> attributes)
        {
            if (!attributes.ContainsKey("Anno registro"))
                throw new Exception("update: il dizionario non contiene la chiave \"Anno registro\".");
            if (!attributes.ContainsKey("Numero protocollo"))
                throw new Exception("update: il dizionario non contiene la chiave \"Numero protocollo\".");

            string[] fields = attributesToArray(attributes);
            string annoRegistro = string.Format("(;Anno registro;15;{0};;);AND", attributes["Anno registro"]);
            string numeroProtocollo = string.Format("(;Numero protocollo;15;{0};;);", attributes["Numero protocollo"]);
            string[] filters = { annoRegistro, numeroProtocollo };

            var message = string.Format("TopMedia Update - sessionid: {0}, documenttype: {1}, attributes: {2}",
                    sessionId, documentType, attributes.Count);
            FileLogger.Info("FileLog", message);

            int result = 0;
#if DEBUG
            result = 1;
#else
            result = Services.TopMedia.Service.Update(sessionId, documentType, fields, filters);
#endif
            FileLogger.Info("FileLog", "result: " + result.ToString());
            return result;
        }

        /// <summary>
        /// Archivia gli allegati del protocollo.
        /// </summary>
        /// <param name="sessionId">Identificativo della sessione di lavoro.</param>
        /// <param name="protocol">Protocollo di riferimento.</param>
        /// <returns></returns>
        private IList<int> archiveAttachments(string sessionId, Protocol protocol)
        {
            if (!protocol.IdAttachments.HasPositiveValue())
                return null;

            // Recupero gli allegati da Biblos.
            UIDLocation location = new UIDLocation(protocol.Location.ProtBiblosDSDB);
            IList<BiblosDocumentInfo> attachments = null;
            try
            {
                UIDChain attachmentsUid = new UIDChain(location, protocol.IdAttachments.Value);
                attachments = BiblosDocumentInfo.GetDocuments(attachmentsUid);
            }
            catch (Exception ex)
            {
                string message = "archiveAttachments: errore in recupero documenti da Biblos. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }

            if (attachments.IsNullOrEmpty())
                return null;

            // Recupero il dizionario degli attributi.
            IDictionary<string, string> attributes = null;
            try
            {
                attributes = getAttachmentAttributes(protocol);
            }
            catch (Exception ex)
            {
                string message = "archiveAttachments: errore in recupero dizionario degli attributi. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }

            // Archivio gli allegati.
            IList<int> archived = new List<int>();
            int counter = 1;
            try
            {
                foreach (var item in attachments)
                {
                    attributes["Numero Allegato"] = counter.ToString();
                    var pid = getProductId(item.Name);
                    var fields = attributes.Keys.ToArray();
                    var values = attributes.Values.ToArray();
                    int result = -1;

                    var message = string.Format("TopMedia Archive - sessionid: {0}, attachmenttype: {1}, fields: {2}, values: {3}, name: {4}, stream: {5}, pid: {6}",
                            sessionId,
                            DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType,
                            fields.Length, values.Length,
                            item.Name, item.Stream.Length,
                            pid);
                    FileLogger.Info("FileLog", message);
#if DEBUG
                    result = counter;
#else
                    result = Services.TopMedia.Service.Archive(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType, fields, values, item.Name, item.Stream, pid);
#endif
                    FileLogger.Info("FileLog", "result: " + result.ToString());
                    archived.Add(result);
                    counter++;
                }
            }
            catch (Exception ex)
            {
                string message = "archiveAttachments: errore in archiviazione documento. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }
            return archived;
        }
        private IList<int> archiveAttachments(string sessionId, Protocol protocol, IList<DocumentInfo> attachments)
        {
            if (attachments.IsNullOrEmpty())
                return null;

            // Recupero il dizionario degli attributi.
            IDictionary<string, string> attributes = null;
            try
            {
                attributes = getAttachmentAttributes(protocol);
            }
            catch (Exception ex)
            {
                string message = "archiveAttachments List<FileInfo>: errore in recupero dizionario degli attributi. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }

            // Archivio gli allegati.
            IList<int> archived = new List<int>();
            int counter = 1;
            try
            {
                foreach (var item in attachments)
                {
                    attributes["Numero Allegato"] = counter.ToString();
                    var pid = getProductId(item.Name);
                    var fields = attributes.Keys.ToArray();
                    var values = attributes.Values.ToArray();
                    int result = -1;


                    var message = string.Format("TopMedia Archive - sessionid: {0}, attachmenttype: {1}, fields: {2}, values: {3}, name: {4}, stream: {5}, pid: {6}",
                            sessionId,
                            DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType,
                            fields.Length, values.Length,
                            item.Name, item.Stream.Length,
                            pid);
                    FileLogger.Info("FileLog", message);
#if DEBUG
                    result = counter;
#else
                    result = Services.TopMedia.Service.Archive(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType, fields, values, item.Name, item.Stream, pid);
#endif
                    FileLogger.Info("FileLog", "result: " + result.ToString());
                    archived.Add(result);
                    counter++;
                }
            }
            catch (Exception ex)
            {
                string message = "archiveAttachments List<FileInfo>: errore in archiviazione documento. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }
            return archived;
        }
        /// <summary>
        /// Archivia il documento di un protocollo.
        /// </summary>
        /// <param name="sessionId">Identificativo della sessione di lavoro.</param>
        /// <param name="protocol">Protocollo di riferimento.</param>
        /// <returns></returns>
        private int archiveDocument(string sessionId, Protocol protocol)
        {
            if (!protocol.IdDocument.HasValue || protocol.IdDocument < 1)
                throw new Exception("archiveDocument: IdDocument del protocollo mancante o non valido.");

            // Recupero il documento da Biblos.
            DocumentInfo document = null;
            try
            {
                document = new BiblosDocumentInfo(protocol.Location.ProtBiblosDSDB, protocol.IdDocument.Value);
            }
            catch (Exception ex)
            {
                string message = "archiveDocument: errore in recupero documento da Biblos. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }

            // Recupero il dizionario degli attributi.
            IDictionary<string, string> attributes = null;
            try
            {
                attributes = getDocumentAttributes(protocol);
            }
            catch (Exception ex)
            {
                string message = "archiveDocument: : errore in recupero dizionario degli attributi. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }

            // Archivio il documento.
            int archived = -1;
            try
            {
                var pid = getProductId(document.Name);
                var fields = attributes.Keys.ToArray();
                var values = attributes.Values.ToArray();

                var message = string.Format("TopMedia Archive - sessionid: {0}, attachmenttype: {1}, fields: {2}, values: {3}, name: {4}, stream: {5}, pid: {6}",
                    sessionId,
                    DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType,
                    fields.Length, values.Length,
                    document.Name, document.Stream.Length,
                    pid);
                FileLogger.Info("FileLog", message);
#if DEBUG
                archived = 123;
#else
                archived = Services.TopMedia.Service.Archive(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.DocumentType, fields, values, document.Name, document.Stream, pid);
                Services.TopMedia.Service.TryLogout(sessionId);
#endif
                FileLogger.Info("FileLog", "archived: " + archived.ToString());
            }
            catch (Exception ex)
            {
                string message = "archiveDocument: errore in archiviazione documento. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }
            return archived;
        }
        /// <summary>
        /// Aggiorna il dizionario di un protocollo.
        /// </summary>
        /// <param name="sessionId">Identificativo della sessione di lavoro.</param>
        /// <param name="protocol">Protocollo di riferimento.</param>
        /// <returns></returns>
        private int updateDocument(string sessionId, Protocol protocol)
        {
            if (!protocol.IdDocument.HasValue || protocol.IdDocument < 1)
                throw new Exception("updateDocument: IdDocument del protocollo mancante o non valido.");

            // Recupero il dizionario degli attributi.
            IDictionary<string, string> attributes = null;
            try
            {
                attributes = getDocumentAttributes(protocol);
                attributes.Remove("Data protocollo"); // Perchè viene rimosso? In teoria dovrei aggiornarlo con la data di modifica... - FG
            }
            catch (Exception ex)
            {
                string message = "updateDocument: errore in recupero dizionario degli attributi. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }

            // Aggiorno il dizionario del documento legato al protocollo modificato.
            int count = -1;
            try
            {
                count = update(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.DocumentType, attributes);
                FileLogger.Info("FileLog", "Logout: " + sessionId);
#if DEBUG
#else
                Services.TopMedia.Service.TryLogout(sessionId);
#endif
            }
            catch (Exception ex)
            {
                string message = "updateDocument: errore in archiviazione documento. {0} - {1}";
                message = string.Format(message, ex.Source, ex.Message);
                throw new Exception(message, ex);
            }
            return count;
        }

        /// <summary>
        /// Archivia documento e allegati del protocollo.
        /// </summary>
        /// <param name="protocol">Protocollo di riferimento.</param>
        public void ArchiveProtocol(Protocol protocol)
        {
            if (protocol.TDIdDocument.HasValue && protocol.TDIdDocument > 0)
                throw new Exception("ArchiveProtocol: il protocollo specificato risulta essere già stato archiviato in TopMedia.");

            // Verifico siano corretti i parametri di connessione al servizio.
            verifyParameters();

            // Inizializzo la sessione di lavoro.
            string sessionId = string.Empty;
            try
            {
                sessionId = login();
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);
                return;
            }

            clearEntityLogging(protocol);

            // Archivio gli allegati.
            IList<int> attachmentIdentifiers = null;
            try
            {
                attachmentIdentifiers = archiveAttachments(sessionId, protocol);
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);
                if (!attachmentIdentifiers.IsNullOrEmpty())
                    purge(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType, attachmentIdentifiers);
                FileLogger.Info("FileLog", "Logout: " + sessionId);
#if DEBUG
#else
                Services.TopMedia.Service.TryLogout(sessionId);
#endif
                return;
            }

            // Archivio il documento.
            int? documentIdentifier = null;
            try
            {
                documentIdentifier = archiveDocument(sessionId, protocol);
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);

                // Eseguo il rollback dei documenti archiviati.
                if (documentIdentifier.HasValue)
                    purge(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.DocumentType, documentIdentifier.Value);
                if (!attachmentIdentifiers.IsNullOrEmpty())
                    purge(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType, attachmentIdentifiers);

                return;
            }
            finally
            {
                FileLogger.Info("FileLog", "Logout: " + sessionId);
#if DEBUG
#else
                Services.TopMedia.Service.TryLogout(sessionId);
#endif
            }

            // Se tutte le operazioni precedenti sono andate a buon fine finalizzo l'archiviazione.
            protocol.TDIdDocument = documentIdentifier.Value;
            protocol.TDError = string.Empty;
            Facade.ProtocolFacade.UpdateNoLastChange(ref protocol);
        }
        /// <summary>
        /// Archivia una lista di FileInfo come allegati del protocollo.
        /// </summary>
        /// <param name="protocol">Protocollo di riferimento.</param>
        /// <param name="documentsAdded">Lista di FileInfo da archiviare.</param>
        public void ArchiveProtocolAttachments(Protocol protocol, IList<DocumentInfo> documentsAdded)
        {
            if (documentsAdded.IsNullOrEmpty())
                return;

            // Verifico siano corretti i parametri di connessione al servizio.
            verifyParameters();

            // Inizializzo la sessione di lavoro.
            string sessionId = string.Empty;
            try
            {
                sessionId = login();
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);
                return;
            }

            clearEntityLogging(protocol);

            // Archivio gli allegati.
            IList<int> attachmentIdentifiers = null;
            try
            {
                attachmentIdentifiers = archiveAttachments(sessionId, protocol, documentsAdded);
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);

                // Eseguo il rollback dei documenti archiviati.
                if (attachmentIdentifiers != null && attachmentIdentifiers.Count > 0)
                    purge(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.AttachmentType, attachmentIdentifiers);
                return;
            }
            finally
            {
                FileLogger.Info("FileLog", "Logout: " + sessionId);
#if DEBUG
#else
                Services.TopMedia.Service.TryLogout(sessionId);
#endif
            }
        }
        /// <summary>
        /// Aggiorna un'archiviazione con le modifiche apportate al protocollo.
        /// </summary>
        /// <param name="protocol">Protocollo di riferimento.</param>
        public void ModifyProtocol(Protocol protocol)
        {
            // Verifico siano corretti i parametri di connessione al servizio.
            verifyParameters();

            // Inizializzo la sessione di lavoro.
            string sessionId = string.Empty;
            try
            {
                sessionId = login();
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);
                return;
            }

            clearEntityLogging(protocol);

            // Archivio il documento.
            int? count = null;
            try
            {
                count = updateDocument(sessionId, protocol);
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);
                return;
            }
            finally
            {
                FileLogger.Info("FileLog", "Logout: " + sessionId);
#if DEBUG
#else
                Services.TopMedia.Service.TryLogout(sessionId);
#endif
            }

        }
        /// <summary>
        /// Aggiorna un'archiviazione con le modifiche apportate dall'annullamento del protocollo.
        /// </summary>
        /// <param name="protocol">Protocollo di riferimento.</param>
        public void CancelProtocol(Protocol protocol)
        {
            // Verifico siano corretti i parametri di connessione al servizio.
            verifyParameters();

            // Inizializzo la sessione di lavoro.
            string sessionId = string.Empty;
            try
            {
                sessionId = login();
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);
                return;
            }

            // Genero un dizionario di attributi specifico per l'annullamento.
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("Annullato", "S");
            attributes.Add("Data di annullamento", string.Format("{0:dd/MM/yyyy}", DateTime.Today));
            attributes.Add("Motivazione annullamento", "Cambio Contenitore"); // E' corretta questa cosa? - FG
            // Mancano comunque le chiavi "Anno registro" e "Numero protocollo" necessarie per "update", come fa a funzionare questa cosa? - FG

            clearEntityLogging(protocol);

            // Aggiorno il dizionario del documento legato al protocollo annullato.
            int? count = null;
            try
            {
                count = update(sessionId, DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.DocumentType, attributes);
            }
            catch (Exception ex)
            {
                logToEntity(protocol, ex);
                return;
            }
            finally
            {
                FileLogger.Info("FileLog", "Logout: " + sessionId);
#if DEBUG
#else
                Services.TopMedia.Service.TryLogout(sessionId);
#endif
            }
        }


        void target_AfterUpdate(object sender, ProtocolEventArgs args)
        {
            //if (!IsArchivable(args.Protocol))
            //{
            //    // Se il protocollo è già stato processato e non è più archiviabile annullo la precedente lavorazione.
            //    if (args.Protocol.TDIdDocument.HasValue)
            //        CancelProtocol(args.Protocol);
            //    return;
            //}

            //// Se il protocollo è già stato processato ne aggiorno l'archiviazione.
            //if (args.Protocol.TDIdDocument.HasValue)
            //{
            //    ModifyProtocol(args.Protocol);
            //    ArchiveProtocolAttachments(args.Protocol, args.Tag as List<FileInfo>);
            //    return;
            //}

            //// Altrimenti ne processo l'archiviazione.
            //ArchiveProtocol(args.Protocol);
        }

        void target_AfterInsert(object sender, ProtocolEventArgs args)
        {
            if (!IsArchivable(args.Protocol))
                return;

            // Processo l'archiviazione del protocollo.
            ArchiveProtocol(args.Protocol);
        }
        void target_AfterEdit(object sender, ProtocolEventArgs args)
        {
            if (!IsArchivable(args.Protocol))
                return;

            // Processo l'archiviazione del protocollo.
            ModifyProtocol(args.Protocol);
            ArchiveProtocolAttachments(args.Protocol, args.Tag as List<DocumentInfo>);
        }
        void target_AfterCancel(object sender, ProtocolEventArgs args)
        {
            if (!IsArchivable(args.Protocol))
                return;

            // Processo l'archiviazione del protocollo.
            CancelProtocol(args.Protocol);
        }

    }

}
