using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper.Desks;
using VecompSoftware.Services.Biblos;
using VecompSoftware.Services.Biblos.DocumentsService;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
{
    public class DeskDocumentFacade : BaseProtocolFacade<DeskDocument, Guid, DeskDocumentDao>
    {
        #region [ Fields ]
        private const string checkOutComment = "l'utente {0} ha estratto il documento {1} in versione {2:0.00}";
        private const string checkInComment = "l'utente {0} ha archiviato il documento {1} in versione {2:0.00}";
        private const string renameComment = "l'utente {0} ha rinominato il documento {1} in versione {2:0.00}";
        private const string undoCheckOutComment = "l'utente {0} ha annullato l'estrazione del documento {1} in versione {2:0.00}";
        private const string ADD_DOCUMENT_LOG_FORMAT = "Inserito nuovo documento {0}";
        private const string REMOVED_DOCUMENT_LOG_FORMAT = "Rimosso documento {0}";
        private const string SIGNED_DOCUMENT_LOG_FORMAT = "l'utente {0} ha firmato il documento {1} in versione {2:0.00}";
        private readonly String _userName;
        private readonly MapperDeskDocumentResult MapperDeskDocumentResult;
        private DeskLogFacade _deskLogFacade;
        private DeskCollaborationFacade _deskCollaborationFacade;
        private DeskStoryBoardFacade _deskStoryBoardFacade;
        private DeskDocumentVersionFacade _deskDocumentVersionFacade;
        #endregion [ Fields ]

        #region [ Properties ]
        public DeskStoryBoardFacade CurrentDeskStoryBoardFacade
        {
            get { return _deskStoryBoardFacade ?? (_deskStoryBoardFacade = new DeskStoryBoardFacade(_userName)); }
        }

        public DeskDocumentVersionFacade DeskDocumentVersionFacade
        {
            get { return _deskDocumentVersionFacade ?? (_deskDocumentVersionFacade = new DeskDocumentVersionFacade(_userName)); }
        }

        private DeskLogFacade CurrentDeskLogFacade
        {
            get { return _deskLogFacade ?? (_deskLogFacade = new DeskLogFacade(_userName)); }
        }


        private DeskCollaborationFacade CurrentDeskCollaborationFacade
        {
            get { return _deskCollaborationFacade ?? (_deskCollaborationFacade = new DeskCollaborationFacade(_userName)); }
        }

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskDocumentFacade(string userName) : base()
        {
            _userName = userName;
            MapperDeskDocumentResult = new MapperDeskDocumentResult();
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        public ICollection<DeskDocument> AddNewDeskDocuments(Desk desk, ICollection<DocumentInfo> documents, Location biblosLocation)
        {
            ICollection<DeskDocument> deskDocuments = new Collection<DeskDocument>();
            foreach (DocumentInfo document in documents)
            {
                DeskDocument deskDocument = AddNewDeskDocument(desk, document, biblosLocation);
                deskDocuments.Add(deskDocument);
            }
            return deskDocuments;
        }

        public DeskDocument AddNewDeskDocument(Desk desk, DocumentInfo document, Location biblosLocation)
        {
            DeskDocument deskDocument = new DeskDocument(_userName);
            Guid chain = Guid.Empty;
            chain = DocumentInfoFactory.ArchiveDocumentsInBiblos(new List<DocumentInfo> { document }, biblosLocation.DocumentServer, biblosLocation.ProtBiblosDSDB, chain);

            deskDocument.DocumentType = DeskDocumentType.MainDocument;
            deskDocument.IdDocument = chain;
            deskDocument.Desk = desk;

            //Aggiungo il versioning
            DeskDocumentVersion version = new DeskDocumentVersion(_userName)
            {
                DeskDocument = deskDocument,
                Version = 1
            };

            deskDocument.DeskDocumentVersions.Add(version);
            this.Save(ref deskDocument);

            //Inserisco il log
            CurrentDeskLogFacade.InsertLog(DeskLogType.Modify, string.Format(ADD_DOCUMENT_LOG_FORMAT, document.Name), desk, SeverityLog.Info);

            return deskDocument;
        }

        /// <summary>
        /// Crea un DeskDocumentResult da un DeskDocument non salvato sul DB
        /// </summary>
        public DeskDocumentResult CreateDeskDocumentDto(DeskDocument document, BiblosDocumentInfo biblosDocument)
        {
            return this.CreateDeskDocumentDto(document, biblosDocument, document.Desk.Container.DeskLocation, false);
        }

        /// <summary>
        /// Crea un DeskDocumentResult da un DeskDocument
        /// </summary>
        public DeskDocumentResult CreateDeskDocumentDto(DeskDocument document, BiblosDocumentInfo biblosDocument, bool fromDb)
        {
            return this.CreateDeskDocumentDto(document, biblosDocument, document.Desk.Container.DeskLocation, fromDb);
        }

        /// <summary>
        /// Crea un DeskDocumentResult da un DeskDocument non salvato sul DB
        /// </summary>
        public DeskDocumentResult CreateDeskDocumentDto(DeskDocument document, BiblosDocumentInfo biblosDocument, Location deskLocation)
        {
            return this.CreateDeskDocumentDto(document, biblosDocument, deskLocation, false);
        }

        /// <summary>
        /// Crea un DeskDocumentResult da un DeskDocument
        /// </summary>
        public DeskDocumentResult CreateDeskDocumentDto(DeskDocument document, BiblosDocumentInfo biblosDocument, Location deskLocation, bool fromDb)
        {
            DeskDocumentVersion deskVersioning = null;
            DeskStoryBoard lastStoryBoard = null;
            DeskDocumentResult dto = MapperDeskDocumentResult.MappingDTO(document);


            // Mappo gli oggetti di Biblos
            dto.Name = biblosDocument.Name;
            dto.Size = biblosDocument.Size;
            dto.IsSavedToBiblos = true;
            dto.IsSigned = biblosDocument.IsSigned;
            dto.DocumentServer = deskLocation.DocumentServer;
            dto.BiblosSerializeKey = biblosDocument.Serialized;

            //Mappo commenti e versioning
            if (fromDb)
            {
                deskVersioning = DeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(document.Id);
                lastStoryBoard = CurrentDeskStoryBoardFacade.GetLastStoryBoard(deskVersioning);
            }

            dto.LastVersion = fromDb ? deskVersioning.Version : new decimal(1.0);
            dto.IdChainBiblos = biblosDocument.ChainId;
            dto.IdDocumentBiblos = biblosDocument.DocumentId;
            dto.IsJustInCollaboration = CurrentDeskCollaborationFacade.IsDocumentJustInCollaboration(document.Desk, dto.Name);

            dto.LastComment = fromDb && lastStoryBoard != null ? lastStoryBoard.Comment : string.Empty;

            return dto;
        }

        /// <summary>
        /// CheckIn di una specifica versione del documento. 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="content"></param>
        /// <param name="userId"></param>
        /// <param name="contentFormat"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public BiblosDocumentInfo CheckIn(Desk desk, DeskRoleUser roleUser, DeskDocumentResult document, byte[] content, string userId, ContentFormat contentFormat, decimal? version)
        {
            string docName = document.Name;
            DeskDocument deskDocument = GetById(document.IdDeskDocument.Value, false);
            return CheckIn(desk, roleUser, document.DocumentServer, document.IdDeskDocument, document.IdDocumentBiblos.Value, docName, content, userId, contentFormat, version);
        }

        /// <summary>
        /// CheckIn di una specifica versione del documento. 
        /// </summary>
        /// <param name="documentServer"></param>
        /// <param name="idDocument"></param>
        /// <param name="docName"></param>
        /// <param name="content"></param>
        /// <param name="userId"></param>
        /// <param name="contentFormat"></param>
        /// <param name="version">Ritorna la versione corrente del documento appena inserito</param>
        /// <returns></returns>
        public BiblosDocumentInfo CheckIn(Desk desk, DeskRoleUser roleUser, string documentServer, Guid? idDeskDocument, Guid idDocument, string docName, byte[] content, string userId, ContentFormat contentFormat, decimal? version)
        {
            DeskDocument deskDocument = GetById(idDeskDocument.Value, false);
            BiblosDocumentInfo newDoc = BiblosDocumentInfo.CheckInDocument(documentServer, idDocument, docName, content, userId, contentFormat, version);
            UpdateDocumentCheckIn(idDeskDocument, newDoc.DocumentId);
            decimal newversion = version ?? newDoc.Version;
            DeskDocumentVersion docVersion = DeskDocumentVersionFacade.InsertDocumentVersion(newDoc.DocumentId, newversion, deskDocument);
            AddCommentStoryBoard(newDoc, desk, roleUser, docVersion, DeskStoryBoardType.CheckInComment);
            return newDoc;
        }

        /// <summary>
        /// 1) Annulla estrazione del file da parte dell'utente.
        /// 2) Aggiorna l'ultimo utente che ha eseguito le operazioni sul file e la data.
        /// 3) Aggiunge un commento nello storyboard.
        /// </summary>
        /// <param name="desk"></param>
        /// <param name="roleUser"></param>
        /// <param name="docVersion"></param>
        /// <param name="document"></param>
        /// <param name="userId"></param>
        public void UndoCheckOut(Desk desk, DeskRoleUser roleUser, DeskDocumentVersion docVersion, DeskDocumentResult document, string userId)
        {
            UndoCheckOut(desk, roleUser, docVersion, document.DocumentServer, document.IdDeskDocument, document.IdDocumentBiblos.Value, userId);
        }


        public void RenameDoc(Desk desk, DeskRoleUser roleUser, DeskDocumentVersion docVersion, BiblosDocumentInfo document, string userId)
        {
            Service.UpdateDocument(document, DocSuiteContext.Current.User.FullUserName);
            AddCommentStoryBoard(document, desk, roleUser, docVersion, DeskStoryBoardType.RenameDocument);
        }


        /// <summary>
        /// 1) Annulla estrazione del file da parte dell'utente.
        /// 2) Aggiorna l'ultimo utente che ha eseguito le operazioni sul file e la data.
        /// 3) Aggiunge un commento nello storyboard.
        /// </summary>
        /// <param name="desk"></param>
        /// <param name="roleUser"></param>
        /// <param name="docVersion"></param>
        /// <param name="documentServer"></param>
        /// <param name="idDeskDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="userId"></param>
        public void UndoCheckOut(Desk desk, DeskRoleUser roleUser, DeskDocumentVersion docVersion, string documentServer, Guid? idDeskDocument, Guid idDocument, string userId)
        {
            BiblosDocumentInfo.UndoCheckOutDocument(documentServer, idDocument, userId);
            BiblosDocumentInfo undoDoc = BiblosDocumentInfo.GetDocumentByVersion(documentServer, idDocument, null, null);
            UpdateDocumentLastChages(idDeskDocument);
            AddCommentStoryBoard(undoDoc, desk, roleUser, docVersion, DeskStoryBoardType.UndoCheckout);
        }




        /// <summary>
        /// Esegue l'estrazione del documento con lock esclusivo
        /// </summary>
        /// <param name="document"></param>
        /// <param name="userId"></param>
        /// <param name="contentFormat"></param>
        /// <param name="returnContent"></param>
        /// <returns></returns>
        public BiblosDocumentInfo CheckOut(Desk desk, DeskRoleUser roleUser, DeskDocumentVersion docVersion, DeskDocumentResult document, string userId, ContentFormat contentFormat, bool? returnContent)
        {
            return CheckOut(desk, roleUser, docVersion, document.DocumentServer, document.IdDeskDocument, document.IdDocumentBiblos.Value, userId, contentFormat, returnContent);
        }
        /// <summary>
        /// Esegue l'estrazione del documento con lock esclusivo
        /// </summary>
        /// <param name="documentServer"></param>
        /// <param name="idDocument"></param>
        /// <param name="userId"></param>
        /// <param name="contentFormat"></param>
        /// <param name="returnContent"></param>
        /// <returns></returns>
        public BiblosDocumentInfo CheckOut(Desk desk, DeskRoleUser roleUser, DeskDocumentVersion docVersion, string documentServer, Guid? idDeskDocument, Guid idDocument, string userId, ContentFormat contentFormat, bool? returnContent)
        {
            BiblosDocumentInfo checkOutDoc = BiblosDocumentInfo.CheckOutDocument(documentServer, idDocument, userId, contentFormat, returnContent);
            UpdateDocumentLastChages(idDeskDocument);
            AddCommentStoryBoard(checkOutDoc, desk, roleUser, docVersion, DeskStoryBoardType.CheckOutComment);
            return checkOutDoc;
        }

        public BiblosDocumentInfo GetLastDocumentVersion(DeskDocumentResult document)
        {
            return BiblosDocumentInfo.GetDocumentByVersion(document.DocumentServer, document.IdDocumentBiblos.Value, null, true);
        }

        /// <summary>
        /// Aggiorna il documento firmato in BiblosDs2010
        /// </summary>
        public BiblosDocumentInfo UpdateSignedDocument(Desk desk, DeskRoleUser roleUser, DeskDocumentResult document, BiblosDocumentInfo documentToUpdate, string userId)
        {
            Guid idDeskDocument = document.IdDeskDocument.Value;
            return this.UpdateSignedDocument(desk, roleUser, idDeskDocument, documentToUpdate, userId);
        }

        /// <summary>
        /// Aggiorna il documento firmato in BiblosDs2010
        /// </summary>
        public BiblosDocumentInfo UpdateSignedDocument(Desk desk, DeskRoleUser roleUser, Guid idDeskDocument, BiblosDocumentInfo documentToUpdate, string userId)
        {
            DeskDocument deskDocument = this.GetById(idDeskDocument);
            BiblosDocumentInfo documentUpdated = documentToUpdate.Update(userId);
            DeskDocumentVersion docVersion = DeskDocumentVersionFacade.InsertDocumentVersion(documentUpdated.DocumentId, documentUpdated.Version, deskDocument);
            AddCommentStoryBoard(documentUpdated, desk, roleUser, docVersion, DeskStoryBoardType.SignedDocument);
            return documentUpdated;
        }

        /// <summary>
        /// Verifica se un documento è in checkout
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool IsCheckOut(DeskDocumentResult document)
        {
            bool result = true;
            result = IsCheckOut(document.DocumentServer, document.IdDocumentBiblos.Value);
            return result;
        }

        /// <summary>
        /// Verifica se un documento è in checkout
        /// </summary>
        /// <param name="documentServer"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public bool IsCheckOut(string documentServer, Guid idDocument)
        {
            return BiblosDocumentInfo.IsCheckOutDocument(documentServer, idDocument);
        }

        /// <summary>
        /// Verifica l'utente che ha eseguito il checkout
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public string IdUserCheckOut(DeskDocumentResult document)
        {
            string result = string.Empty;
            result = IdUserCheckOut(document.DocumentServer, document.IdDeskDocument, document.IdDocumentBiblos.Value);
            return result;
        }

        /// <summary>
        /// Verifica l'utente che ha eseguito il checkout
        /// </summary>
        /// <param name="documentServer"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public string IdUserCheckOut(string documentServer, Guid? idDeskDocument, Guid idDocument)
        {
            return BiblosDocumentInfo.IdUserCheckOutDocument(documentServer, idDocument);
        }

        /// <summary>
        /// Aggiorno l'id del documento e i last change 
        /// </summary>
        private void UpdateDocumentCheckIn(Guid? IdDeskDocument, Guid newIdBiblosDocument)
        {
            if (IdDeskDocument.HasValue)
            {
                DeskDocument deskDoc = base.GetById(IdDeskDocument.Value);
                deskDoc.LastChangedDate = DateTimeOffset.UtcNow;
                deskDoc.LastChangedUser = _userName;
                Save(ref deskDoc);
            }
            else
                throw new Exception("[DeskDocumentFacade - UpdateCheckInDocument] Nessun IdDeskDocument presente per il documento specificato");
        }

        /// <summary>
        /// Aggiorna l'utente che ha eseguito l'ultima azione sul file e l'orario
        /// </summary>
        /// <param name="IdDeskDocument"></param>
        private void UpdateDocumentLastChages(Guid? IdDeskDocument)
        {
            if (IdDeskDocument.HasValue)
            {
                DeskDocument deskDoc = base.GetById(IdDeskDocument.Value);
                deskDoc.LastChangedDate = DateTimeOffset.UtcNow;
                deskDoc.LastChangedUser = _userName;
                UpdateOnly(ref deskDoc);
            }
            else
                throw new Exception("[DeskDocumentFacade - UpdateLastChages] Nessun IdDeskDocument presente per il documento specificato");
        }

        /// <summary>
        /// Aggiunge un nuovo commento allo storyboard indicando l'azione eseguita e l'utente
        /// </summary>
        /// <param name="documentName"></param>
        /// <param name="desk"></param>
        /// <param name="roleUser"></param>
        /// <param name="docVersion"></param>
        /// <param name="boardType"></param>
        private void AddCommentStoryBoard(BiblosDocumentInfo biblosDoc, Desk desk, DeskRoleUser roleUser, DeskDocumentVersion docVersion, DeskStoryBoardType boardType)
        {
            string commentStringType = string.Empty;
            switch (boardType)
            {
                case DeskStoryBoardType.CheckInComment:
                    commentStringType = checkInComment;
                    break;
                case DeskStoryBoardType.CheckOutComment:
                    commentStringType = checkOutComment;
                    break;
                case DeskStoryBoardType.UndoCheckout:
                    commentStringType = undoCheckOutComment;
                    break;
                case DeskStoryBoardType.SignedDocument:
                    commentStringType = SIGNED_DOCUMENT_LOG_FORMAT;
                    break;
                case DeskStoryBoardType.RenameDocument:
                    commentStringType = renameComment;
                    break;
            }

            string comment = string.Format(commentStringType, _userName, biblosDoc.Name, biblosDoc.Version);
            CurrentDeskStoryBoardFacade.AddCommentToStoryBoard(comment, desk, roleUser, docVersion, boardType);
        }



        ///// <summary>
        ///// Esegue la cancellazione logica del documento 
        ///// </summary>
        public void Delete(Guid? IdDeskDocument)
        {
            if (IdDeskDocument.HasValue)
            {
                DeskDocument deskDoc = base.GetById(IdDeskDocument.Value);
                deskDoc.IsActive = 1;
                UpdateOnly(ref deskDoc);
            }
            else
                throw new Exception("[DeskDocumentFacade - LogicDeleteDocument] Nessun IdDeskDocument presente per il documento specificato");
        }
        #endregion [ Methods ]
    }
}
