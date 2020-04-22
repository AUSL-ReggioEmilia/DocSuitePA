using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Desks
{
    public class DeskRightsUtil
    {
        #region [Fields]

        private bool? _isDocumentsViewable;
        private bool? _isSummaryViewable;
        private bool? _isInsertable;
        private bool? _isEditable;
        private bool? _isClosable;
        private bool? _isDeletable;
        private bool? _currentUserIsOwner;
        private bool? _currentUserIsManager;
        private bool? _currentUserIsContributor;
        private bool? _isProtocollable;
        private bool? _isDocumentMaganer;
        private bool? _canApproveDocument;
        private bool? _canRefuseDocument;
        private bool? _canCheckInDocument;
        private bool? _canCheckOutDocument;
        private bool? _canLogicDeleteDocument;
        private bool? _isDocumentButtonDeleteVisible;
        private bool? _isUserButtonDeleteVisible;
        private bool? _canSignDocument;
        private bool? _currentUserIsReader;
        private readonly string _userName;

        #endregion

        #region [Constructor]

        public DeskRightsUtil(Desk itemDesk, string userName)
        {
            _userName = userName;
            CurrentDesk = itemDesk;
            CurrentDeskFacade = new DeskFacade(_userName);
            CurrentDeskDocumentFacade = new DeskDocumentFacade(_userName);
            CurrentDeskEndorsmentFacade = new DeskDocumentEndorsementFacade(_userName);
            CurrentDeskCollaborationFacade = new DeskCollaborationFacade(_userName);
            CurrentCollaborationVersioningFacade = new CollaborationVersioningFacade();
        }
        #endregion

        #region [Properties]

        public Desk CurrentDesk { get; private set; }

        private DeskFacade CurrentDeskFacade { get; set; }

        private DeskCollaborationFacade CurrentDeskCollaborationFacade { get; set; }

        private DeskDocumentEndorsementFacade CurrentDeskEndorsmentFacade { get; set; }
        private DeskDocumentFacade CurrentDeskDocumentFacade { get; set; }

        private CollaborationVersioningFacade CurrentCollaborationVersioningFacade { get; set; }

        private DeskPermissionType CurrentUserPermissionType
        {
            get
            {
                return CurrentDesk.DeskRoleUsers.Where(x => x.AccountName.Eq(DocSuiteContext.Current.User.FullUserName)).Select(s => s.PermissionType).SingleOrDefault();
            }
        }

        private bool CurrentUserIsAuthorized
        {
            get
            {
                return CurrentDesk.DeskRoleUsers.Any(x => x.AccountName.Eq(DocSuiteContext.Current.User.FullUserName));
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente è owner del Tavolo
        /// </summary>
        public bool CurrentUserIsOwner
        {
            get
            {
                if (_currentUserIsOwner.HasValue)
                    return _currentUserIsOwner.Value;

                _currentUserIsOwner = CurrentUserPermissionType.Equals(DeskPermissionType.Admin);
                return _currentUserIsOwner.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente è gestore del Tavolo
        /// </summary>
        public bool CurrentUserIsManager
        {
            get
            {
                if (_currentUserIsManager.HasValue)
                    return _currentUserIsManager.Value;

                _currentUserIsManager = CurrentUserPermissionType.Equals(DeskPermissionType.Manage);
                return _currentUserIsManager.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente è contributore del Tavolo
        /// </summary>
        public bool CurrentUserIsContributor
        {
            get
            {
                if (_currentUserIsContributor.HasValue)
                    return _currentUserIsContributor.Value;
                _currentUserIsContributor = CurrentUserPermissionType.Equals(DeskPermissionType.Contributor);
                return _currentUserIsContributor.Value;
            }
        }
        /// <summary>
        /// Verifica se l'utente corrente è lettore del Tavolo
        /// </summary>
        public bool CurrentUserIsReader
        {
            get
            {
                if (_currentUserIsReader.HasValue)
                    return _currentUserIsReader.Value;

                _currentUserIsReader = CurrentUserPermissionType.Equals(DeskPermissionType.Reader);
                return _currentUserIsReader.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha diritti di visualizzazione sommario del Tavolo
        /// </summary>
        public bool IsSummaryViewable
        {
            get
            {
                if (_isSummaryViewable.HasValue)
                    return _isSummaryViewable.Value;

                if (CurrentUserIsManager || CurrentUserIsOwner || CurrentUserIsAuthorized)
                {
                    _isSummaryViewable = true;
                    return _isSummaryViewable.Value;
                }

                _isSummaryViewable = CheckDeskRights(DeskRightPositions.Read);
                return _isSummaryViewable.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha diritti di visualizzazione documenti del Tavolo
        /// </summary>
        public bool IsDocumentsViewable
        {
            get
            {
                if (_isDocumentsViewable.HasValue)
                    return _isDocumentsViewable.Value;

                if (CurrentUserIsManager || CurrentUserIsOwner || CurrentUserIsAuthorized)
                {
                    _isDocumentsViewable = true;
                    return _isDocumentsViewable.Value;
                }

                _isDocumentsViewable = CheckDeskRights(DeskRightPositions.ViewDocuments);
                return _isDocumentsViewable.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha diritti di inserimento
        /// </summary>
        public bool IsInsertable
        {
            get
            {
                if (_isInsertable.HasValue)
                    return _isInsertable.Value;

                if (CurrentUserIsManager || CurrentUserIsOwner)
                {
                    _isInsertable = true;
                    return _isInsertable.Value;
                }

                _isInsertable = CheckDeskRights(DeskRightPositions.Insert);
                return _isInsertable.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha diritti di modifica del Tavolo
        /// </summary>
        public bool IsEditable
        {
            get
            {
                if (_isEditable.HasValue)
                    return _isEditable.Value;

                if (IsClosed)
                    return false;

                if (CurrentUserIsManager || CurrentUserIsOwner)
                {
                    _isEditable = true;
                    return _isEditable.Value;
                }

                _isEditable = CheckDeskRights(DeskRightPositions.Modify);
                return _isEditable.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha diritti di cancellazione del Tavolo
        /// </summary>
        public bool IsDeletable
        {
            get
            {
                if (_isDeletable.HasValue)
                    return _isDeletable.Value;

                if (CurrentUserIsManager || CurrentUserIsOwner)
                {
                    _isDeletable = true;
                    return _isDeletable.Value;
                }

                _isDeletable = CheckDeskRights(DeskRightPositions.Modify);
                return _isDeletable.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha diritti di chiusura del Tavolo
        /// </summary>
        public bool IsCloseable
        {
            get
            {
                if (_isClosable.HasValue)
                    return _isClosable.Value;

                if (IsClosed)
                    return false;

                _isClosable = false;
                if (CurrentUserIsManager || CurrentUserIsOwner || CheckDeskRights(DeskRightPositions.Close))
                    _isClosable = true;

                return _isClosable.Value;
            }
        }

        /// <summary>
        /// Verifica se è possibile e si hanno diritti per creare una collaborazione dal Tavolo corrente
        /// </summary>
        public bool IsProtocollable
        {
            get
            {
                if (_isProtocollable.HasValue)
                    return _isProtocollable.Value;

                _isProtocollable = false;


                if (CurrentUserIsManager || CurrentUserIsOwner || CheckDeskRights(DeskRightPositions.Collaboration))
                    _isProtocollable = true;

                return _isProtocollable.Value;
            }
        }

        /// <summary>
        /// Verifica se è possibile e si hanno diritti per check out/in documento
        /// </summary>
        public bool IsStoryBoardViewable
        {
            get
            {
                if (_isDocumentMaganer.HasValue)
                    return _isDocumentMaganer.Value;

                _isDocumentMaganer = false;
                if (CurrentUserIsManager || CurrentUserIsOwner || CurrentUserIsAuthorized || CheckDeskRights(DeskRightPositions.Modify))
                    _isDocumentMaganer = true;

                return _isDocumentMaganer.Value;
            }
        }

        /// <summary>
        /// Verifica se è possibile e si hanno diritti per check out/in documento
        /// </summary>
        public bool CanLogicDeleteDocument
        {
            get
            {
                if (_canLogicDeleteDocument.HasValue)
                    return _canLogicDeleteDocument.Value;

                _canLogicDeleteDocument = false;
                if (CurrentUserIsManager || CurrentUserIsOwner)
                    _canLogicDeleteDocument = true;

                return _canLogicDeleteDocument.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente può approvare un documento
        /// </summary>
        public bool CanApproveDocument
        {
            get
            {
                if (_canApproveDocument.HasValue)
                    return _canApproveDocument.Value;

                if (!IsApproval)
                    return false;

                _canApproveDocument = false;
                if (CurrentUserIsManager || CurrentUserIsOwner || CheckDeskRights(DeskRightPositions.Modify) || CurrentUserPermissionType.Equals(DeskPermissionType.Approval))
                    _canApproveDocument = true;

                return _canApproveDocument.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente può rifiutare un documento.
        /// </summary>
        public bool CanRefuseDocument
        {
            get
            {
                if (_canRefuseDocument.HasValue)
                    return _canRefuseDocument.Value;

                if (!IsApproval)
                    return false;

                _canRefuseDocument = false;
                if (CurrentUserIsManager || CurrentUserIsOwner || CheckDeskRights(DeskRightPositions.Modify) || CurrentUserPermissionType.Equals(DeskPermissionType.Approval))
                    _canRefuseDocument = true;

                return _canRefuseDocument.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha i diritti per eseguire il checkin del documento
        /// </summary>
        public bool CanCheckInDocument
        {
            get
            {
                if (this._canCheckInDocument.HasValue)
                    return this._canCheckInDocument.Value;

                this._canCheckInDocument = false;
                if (CurrentUserIsManager || CurrentUserIsOwner || (CheckDeskRights(DeskRightPositions.Modify) || !CurrentUserPermissionType.Equals(DeskPermissionType.Reader)) && IsOpen)
                    this._canCheckInDocument = true;

                return this._canCheckInDocument.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha i diritti per eseguire il checkOut del documento
        /// </summary>
        public bool CanCheckOutDocument
        {
            get
            {
                if (this._canCheckOutDocument.HasValue)
                    return this._canCheckOutDocument.Value;

                this._canCheckOutDocument = false;
                if (CurrentUserIsManager || CurrentUserIsOwner || (CheckDeskRights(DeskRightPositions.Modify) || !CurrentUserPermissionType.Equals(DeskPermissionType.Reader)) && IsOpen)
                    this._canCheckOutDocument = true;

                return this._canCheckOutDocument.Value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente ha i diritti per firmare i documenti
        /// </summary>
        public bool CanSignDocument
        {
            get
            {
                if (this._canSignDocument.HasValue)
                    return this._canSignDocument.Value;

                this._canSignDocument = false;
                if (CurrentUserIsManager || CurrentUserIsOwner || (CheckDeskRights(DeskRightPositions.Modify) || !CurrentUserPermissionType.Equals(DeskPermissionType.Reader)) && IsOpen)
                    this._canSignDocument = true;

                return this._canSignDocument.Value;
            }
        }


        /// <summary>
        /// Verifica se lo stato del Tavolo è Aperto
        /// </summary>
        public bool IsOpen
        {
            get { return CurrentDesk.Status.Equals(DeskState.Open); }
        }

        public bool IsApproval
        {
            get { return CurrentDesk.Status.Equals(DeskState.Approve); }
        }

        /// <summary>
        /// Verifica se lo stato del Tavolo è Chiuso
        /// </summary>
        public bool IsClosed
        {
            get { return CurrentDesk.Status.Equals(DeskState.Closed); }
        }

        #endregion

        #region [Methods]

        private bool CheckDeskRights(DeskRightPositions rightPosition)
        {
            return CurrentDeskFacade.CheckRight(CurrentDesk, CommonShared.GetArrayUserFromString(), rightPosition);
        }

        public bool IsDocumentButtonDeleteVisible
        {
            get
            {
                if (_isDocumentButtonDeleteVisible.HasValue)
                    return _isDocumentButtonDeleteVisible.Value;

                _isDocumentButtonDeleteVisible = false;
                if (!CurrentDesk.Status.Equals(DeskState.Approve) && !CurrentDesk.Status.Equals(DeskState.Closed) && !CurrentUserIsReader)
                    _isDocumentButtonDeleteVisible = true;

                return _isDocumentButtonDeleteVisible.Value;
            }
        }

        public bool IsUserButtonDeleteVisible
        {
            get
            {
                if (_isUserButtonDeleteVisible.HasValue)
                    return _isUserButtonDeleteVisible.Value;

                _isUserButtonDeleteVisible = false;
                if (!CurrentDesk.Status.Equals(DeskState.Approve))
                    _isUserButtonDeleteVisible = true;

                return _isUserButtonDeleteVisible.Value;
            }
        }
        #endregion
    }
}
