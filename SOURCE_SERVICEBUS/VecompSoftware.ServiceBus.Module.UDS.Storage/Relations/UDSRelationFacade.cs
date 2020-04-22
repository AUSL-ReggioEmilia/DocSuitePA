using Newtonsoft.Json;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.UDS.Exceptions;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSRelationFacade
    {

        private const string UDS_LOG_ROLES_MODIFY = "Modifica autorizzazioni";
        private const string UDS_INFO = " Archivio: {0} n. {1}";
        private readonly UDSContactEqualityComparer _udsContactEqualityComparer = new UDSContactEqualityComparer();
        private readonly UDSModel _uds;
        private readonly UDSTableBuilder _builder;
        public UDSModel UDS => _uds;
        public UDSTableBuilder Builder => _builder;

        public UDSRelationFacade(string xml, string xmlSchema, string dbSchema = "dbo")
        {
            List<string> validationErrors;
            bool validate = UDSModel.ValidateXml(xml, xmlSchema, out validationErrors);
            if (!validate)
            {
                throw new UDSRelationException(string.Format("UDSRelationFacade - Errori di validazione Xml: {0}", string.Join("\n", validationErrors)));
            }
            _uds = UDSModel.LoadXml(xml);
            _builder = new UDSTableBuilder(UDS, dbSchema);
        }

        public UDSRelationFacade(UDSModel uds, string dbSchema = "dbo")
        {
            _uds = uds;
            _builder = new UDSTableBuilder(UDS, dbSchema);
        }

        public UDSRelations GetRelations(Database db, Guid udsId)
        {
            return new UDSRelations
            {
                Documents = db.Fetch<UDSDocument>(SQLFetchString(_builder.UDSDocumentsTableName), udsId)
            };
        }

        public void DeleteRelations(Database db, Guid udsId)
        {
            db.Execute(SQLDeleteRelationsString(_builder.UDSDocumentsTableName), udsId);
        }

        internal ICollection<UDSDocument> AddDocuments(Database db, Guid udsId)
        {
            List<UDSDocument> documents = new List<UDSDocument>();
            //elimina tutti i documenti collegati
            if (_uds.Model.Documents == null)
            {
                return new List<UDSDocument>();
            }

            documents.AddRange(AddDocumentsByType(db, udsId, _uds.Model.Documents.Document, UDSDocumentType.Document));
            documents.AddRange(AddDocumentsByType(db, udsId, _uds.Model.Documents.DocumentAnnexed, UDSDocumentType.DocumentAnnexed));
            documents.AddRange(AddDocumentsByType(db, udsId, _uds.Model.Documents.DocumentAttachment, UDSDocumentType.DocumentAttachment));
            documents.AddRange(AddDocumentsByType(db, udsId, _uds.Model.Documents.DocumentDematerialisation, UDSDocumentType.Dematerialisation));

            return documents;
        }

        private ICollection<UDSDocument> AddDocumentsByType(Database db, Guid udsId, Document model, UDSDocumentType docType)
        {
            IList<UDSDocument> documents = new List<UDSDocument>();
            if (model != null && model.Instances != null)
            {
                DocumentInstance instance = model.Instances.First();
                UDSDocument doc = new UDSDocument()
                {
                    UDSDocumentId = Guid.NewGuid(),
                    UDSId = udsId,
                    DocumentName = !model.AllowMultiFile ? instance.DocumentName : string.Empty,
                    IdDocument = Guid.Parse(instance.IdDocument),
                    DocumentType = (short)docType,
                    DocumentLabel = model.Label
                };

                doc.Insert(db, _builder.DbSchema, _builder.UDSDocumentsTableName);
                documents.Add(doc);
            }
            return documents;
        }

        private UDSAuthorization CreateUDSAuthorization(Guid udsId, AuthorizationInstance instance)
        {
            UDSAuthorization udsAuthorization = new UDSAuthorization()
            {
                UDSAuthorizationId = Guid.NewGuid(),
                UDSId = udsId,
                IdRole = instance.IdAuthorization,
                RoleLabel = _uds.Model.Authorizations.Label,
                AuthorizationType = instance.AuthorizationType
            };

            Guid uniqueIdRole;
            if (Guid.TryParse(instance.UniqueId, out uniqueIdRole))
            {
                udsAuthorization.UniqueIdRole = uniqueIdRole;
            }
            return udsAuthorization;
        }

        internal ICollection<UDSAuthorization> AddAuthorizations(Guid udsId, IEnumerable<AuthorizationInstance> buildModelRoles)
        {
            if (_uds.Model.Authorizations == null || _uds.Model.Authorizations.Instances == null)
            {
                return new List<UDSAuthorization>();
            }

            IList<UDSAuthorization> authorizations = new List<UDSAuthorization>();
            foreach (AuthorizationInstance instance in _uds.Model.Authorizations.Instances)
            {
                authorizations.Add(CreateUDSAuthorization(udsId, instance));
            }
            foreach (AuthorizationInstance instance in buildModelRoles.Where(f => !authorizations.Any(r => r.IdRole == f.IdAuthorization && r.AuthorizationType == f.AuthorizationType)))
            {
                authorizations.Add(CreateUDSAuthorization(udsId, instance));
            }
            return authorizations;
        }

        internal ICollection<UDSContact> AddContacts(Guid udsId)
        {
            if (_uds.Model.Contacts == null || !_uds.Model.Contacts.Any())
            {
                return new List<UDSContact>();
            }

            ICollection<UDSContact> contacts = new List<UDSContact>();
            foreach (Contacts contactItem in _uds.Model.Contacts.Where(x => x.ContactInstances != null || x.ContactManualInstances != null))
            {
                if (contactItem.ContactInstances != null)
                {
                    foreach (ContactInstance instance in contactItem.ContactInstances)
                    {
                        UDSContact item = new UDSContact()
                        {
                            UDSContactId = Guid.NewGuid(),
                            UDSId = udsId,
                            ContactLabel = contactItem.Label
                        };

                        if (instance.GetType() == typeof(ContactInstance))
                        {
                            item.IdContact = instance.IdContact;
                            item.ContactType = (short)UDSContactType.Normal;
                        }

                        contacts.Add(item);
                    }
                }

                if (contactItem.ContactManualInstances != null)
                {
                    foreach (ContactManualInstance instance in contactItem.ContactManualInstances)
                    {
                        UDSContact item = new UDSContact()
                        {
                            UDSContactId = Guid.NewGuid(),
                            UDSId = udsId,
                            ContactManual = instance.ContactDescription,
                            ContactType = (short)UDSContactType.Manual,
                            ContactLabel = contactItem.Label
                        };

                        contacts.Add(item);
                    }
                }
            }
            return contacts;
        }

        internal ICollection<UDSMessage> AddMessages(Guid udsId)
        {
            if (_uds.Model.Messages == null || _uds.Model.Messages.Instances == null)
            {
                return new List<UDSMessage>();
            }

            ICollection<UDSMessage> messages = new List<UDSMessage>();
            UDSMessage item;
            foreach (MessageInstance instance in _uds.Model.Messages.Instances)
            {
                item = new UDSMessage()
                {
                    UDSMessageId = Guid.NewGuid(),
                    UDSId = udsId,
                    IdMessage = instance.IdMessage
                };

                Guid uniqueIdMessage;
                if (Guid.TryParse(instance.UniqueId, out uniqueIdMessage))
                {
                    item.UniqueIdMessage = uniqueIdMessage;
                }

                messages.Add(item);
            }

            return messages;
        }

        internal ICollection<UDSPECMail> AddPECMails(Guid udsId)
        {
            if (_uds.Model.PECMails == null || _uds.Model.PECMails.Instances == null)
            {
                return new List<UDSPECMail>();
            }

            ICollection<UDSPECMail> pecs = new List<UDSPECMail>();
            UDSPECMail item;
            foreach (PECInstance instance in _uds.Model.PECMails.Instances)
            {
                item = new UDSPECMail()
                {
                    UDSPECMailId = Guid.NewGuid(),
                    UDSId = udsId,
                    IdPECMail = instance.IdPECMail
                };

                Guid uniqueIdPecMail;
                if (Guid.TryParse(instance.UniqueId, out uniqueIdPecMail))
                {
                    item.UniqueIdPECMail = uniqueIdPecMail;
                }

                pecs.Add(item);
            }

            return pecs;
        }

        internal ICollection<UDSProtocol> AddProtocols(Guid udsId)
        {
            if (_uds.Model.Protocols == null || _uds.Model.Protocols.Instances == null)
            {
                return new List<UDSProtocol>();
            }

            ICollection<UDSProtocol> protocols = new List<UDSProtocol>();
            UDSProtocol item;
            foreach (ProtocolInstance instance in _uds.Model.Protocols.Instances)
            {
                item = new UDSProtocol()
                {
                    UDSProtocolId = Guid.NewGuid(),
                    UDSId = udsId,
                    IdProtocol = Guid.Parse(instance.IdProtocol)
                };

                protocols.Add(item);
            }

            return protocols;
        }

        internal ICollection<UDSResolution> AddResolutions(Guid udsId)
        {
            if (_uds.Model.Resolutions == null || _uds.Model.Resolutions.Instances == null)
            {
                return new List<UDSResolution>();
            }

            ICollection<UDSResolution> resolutions = new List<UDSResolution>();
            UDSResolution item;
            foreach (ResolutionInstance instance in _uds.Model.Resolutions.Instances)
            {
                item = new UDSResolution()
                {
                    UDSResolutionId = Guid.NewGuid(),
                    UDSId = udsId,
                    IdResolution = instance.IdResolution
                };

                Guid uniqueIdResolution;
                if (Guid.TryParse(instance.UniqueId, out uniqueIdResolution))
                {
                    item.UniqueIdResolution = uniqueIdResolution;
                }

                resolutions.Add(item);
            }

            return resolutions;
        }

        internal ICollection<UDSCollaboration> AddCollaborations(Guid udsId)
        {
            if (_uds.Model.Collaborations == null || _uds.Model.Collaborations.Instances == null)
            {
                return new List<UDSCollaboration>();
            }

            ICollection<UDSCollaboration> collaborations = new List<UDSCollaboration>();
            UDSCollaboration item;
            foreach (CollaborationInstance instance in _uds.Model.Collaborations.Instances.Where(x => x.IdCollaboration != 0))
            {
                item = new UDSCollaboration()
                {
                    UDSCollaborationId = Guid.NewGuid(),
                    UDSId = udsId,
                    IdCollaboration = instance.IdCollaboration,
                    CollaborationTemplateName = instance.TemplateName,
                    CollaborationUniqueId = string.IsNullOrEmpty(instance.UniqueId) ? default(Guid?) : Guid.Parse(instance.UniqueId),
                };

                collaborations.Add(item);
            }

            return collaborations;
        }

        internal ICollection<UDSUserModel> AddUsers(Guid udsId)
        {
            ICollection<UDSUserModel> users = new List<UDSUserModel>();
            if (_uds.Model.Contacts != null)
            {
                IEnumerable<Contacts> contacts = _uds.Model.Contacts.Where(f => f.ContactManualInstances != null && f.ContactType == ContactType.AccountAuthorization);
                foreach (ContactManualInstance instance in contacts.SelectMany(s => s.ContactManualInstances).Where(x => !string.IsNullOrEmpty(x.ContactDescription)))
                {
                    users.Add(JsonConvert.DeserializeObject<UDSContactModel>(instance.ContactDescription).Contact);
                }
            }
            return users;
        }

        internal ICollection<UDSDocument> UpdateDocuments(Database db, Guid udsId, UDSRelations existingRels, Action<Guid> detachAction)
        {
            List<UDSDocument> documents = new List<UDSDocument>();
            //non ci sono documenti elimino tutti quelli collegati
            if (_uds.Model.Documents == null)
            {
                db.Execute(SQLDeleteRelationsString(_builder.UDSDocumentsTableName), udsId);
                return documents;
            }

            documents.AddRange(UpdateDocumentsByType(db, udsId, _uds.Model.Documents.Document, existingRels, UDSDocumentType.Document, detachAction));
            documents.AddRange(UpdateDocumentsByType(db, udsId, _uds.Model.Documents.DocumentAnnexed, existingRels, UDSDocumentType.DocumentAnnexed, detachAction));
            documents.AddRange(UpdateDocumentsByType(db, udsId, _uds.Model.Documents.DocumentAttachment, existingRels, UDSDocumentType.DocumentAttachment, detachAction));
            documents.AddRange(UpdateDocumentsByType(db, udsId, _uds.Model.Documents.DocumentDematerialisation, existingRels, UDSDocumentType.Dematerialisation, detachAction));

            return documents;
        }

        private ICollection<UDSDocument> UpdateDocumentsByType(Database db, Guid udsId, Document model, UDSRelations existingRels, UDSDocumentType docType, Action<Guid> detachAction)
        {
            string deleteAllSQL = string.Format(@"delete from {0}.{1} where {2}=@0 and DocumentType=@1", _builder.DbSchema, _builder.UDSDocumentsTableName, UDSTableBuilder.UDSFK);
            string deleteItemSQL = string.Format(@"delete from {0}.{1} where {2}=@0 and DocumentType=@1 and {3}=@2", _builder.DbSchema, _builder.UDSDocumentsTableName,
                UDSTableBuilder.UDSFK, UDSTableBuilder.UDSDocumentsFK);

            List<UDSDocument> documents = new List<UDSDocument>();
            if (model != null && model.Instances != null)
            {
                //situazione db
                IEnumerable<Guid> existings = existingRels.Documents.Where(p => p.DocumentType == (short)docType).Select(p => p.IdDocument);

                //elimino gli elementi per l'aggiornamento
                db.Execute(deleteAllSQL, udsId, docType);

                //elementi da aggiungere
                DocumentInstance instance = model.Instances.First();
                Guid idDocument = Guid.Parse(instance.IdDocument);
                string documentName = instance.DocumentName;
                if (string.IsNullOrEmpty(documentName) && docType == UDSDocumentType.Document)
                {
                    documentName = existingRels.Documents.SingleOrDefault(f => f.IdDocument == idDocument)?.DocumentName;
                }
                UDSDocument doc = new UDSDocument()
                {
                    UDSDocumentId = Guid.NewGuid(),
                    UDSId = udsId,
                    IdDocument = idDocument,
                    DocumentName = !model.AllowMultiFile ? documentName : existingRels.Documents.FirstOrDefault(f => f.DocumentType == (short)docType)?.DocumentName,
                    DocumentType = (short)docType
                };

                doc.Insert(db, _builder.DbSchema, _builder.UDSDocumentsTableName);
                doc.DocumentLabel = model.Label;
                documents.Add(doc);
            }
            else
            {
                db.Execute(deleteAllSQL, udsId, docType);
                //elimina le relazioni tipo Document
                Guid? chainId = existingRels.Documents.Where(p => p.DocumentType == (short)docType).Select(s => s.IdDocument).FirstOrDefault();
                if (chainId.HasValue && chainId.Value != Guid.Empty)
                {
                    detachAction(chainId.Value);
                }
            }
            return documents;
        }

        internal void AddUDSAuthorizationtLog(Guid udsId, string title, string fullNumber, string userName, DateTimeOffset creationTime, ref UDSLogType logType, ICollection<UDSLogModel> logs)
        {
            string logSubjectMessage = string.Concat(UDS_LOG_ROLES_MODIFY, string.Format(UDS_INFO, title, fullNumber));
            logType = UDSLogType.AuthorizationModify;
            logs.Add(CreateNewLog(udsId, logType, logSubjectMessage, userName, creationTime));
        }

        private string SQLFetchString(string relationTable)
        {
            return string.Format(@"select * from {0}.{1} where {2}=@0", _builder.DbSchema, relationTable, UDSTableBuilder.UDSFK);
        }

        private string SQLDeleteRelationsString(string relationTable)
        {
            return string.Format(@"delete from {0}.{1} where {2}=@0", _builder.DbSchema, relationTable, UDSTableBuilder.UDSFK);
        }

        private UDSLogModel CreateNewLog(Guid udsId, UDSLogType logType, string logDescription, string username, DateTimeOffset creationTime)
        {
            UDSLogModel log = new UDSLogModel
            {
                UDSLogId = Guid.NewGuid(),
                UDSId = udsId,
                LogDate = creationTime,
                LogDescription = logDescription,
                SystemComputer = Environment.MachineName,
                SystemUser = username,
                LogType = logType
            };

            return log;
        }
    }
}
