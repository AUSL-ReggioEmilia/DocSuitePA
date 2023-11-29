using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.UDS;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.PEC
{
    public class PECMailRightsUtil
    {
        #region [ Fields ]
        private readonly UDSRepositoryFacade _currentUDSRepositoryFacade;
        private readonly string _userName;
        private readonly Guid _idTenantAOO;
        private bool? _isActive;
        private bool? _hasAnomalies;
        private bool? _isHandler;
        private bool? _isRestorable;
        private bool? _isProtocollable;
        private bool? _isAttachable;
        private bool? _isDeletable;
        private bool? _isForwardable;
        private bool? _isDestinable;
        private bool? _hasError;
        private bool? _isDelivered;
        private bool? _canHandle;
        private bool? _currentUserHasProtocolPECHandleRights;
        private bool? _currentUserHasUDSPECHandleRights;
        private bool? _currentUserHasPECHandleRights;
        private bool? _isResendable;
        #endregion

        #region [ Constructor ]

        public PECMailRightsUtil(PECMail pecMail, string userName, Guid idTenantAOO)
        {
            CurrentPecMail = pecMail;
            _userName = userName;
            _currentUDSRepositoryFacade = new UDSRepositoryFacade(userName);
            _idTenantAOO = idTenantAOO;
        }

        #endregion        

        #region [ Properties ]

        private PECMail CurrentPecMail
        {
            get; set;
        }

        public bool IsActive
        {
            get
            {
                if (_isActive.HasValue)
                {
                    return _isActive.Value;
                }

                _isActive = CurrentPecMail.IsActive == ActiveType.Cast(ActiveType.PECMailActiveType.Active);
                return _isActive.Value;
            }
        }

        public bool HasAnomalies
        {
            get
            {
                if (_hasAnomalies.HasValue)
                {
                    return _hasAnomalies.Value;
                }

                _hasAnomalies = CurrentPecMail.IsActive == ActiveType.Cast(ActiveType.PECMailActiveType.Error)
                    || CurrentPecMail.IsActive == ActiveType.Cast(ActiveType.PECMailActiveType.Processing);
                return _hasAnomalies.Value;
            }
        }

        public bool IsHandler
        {
            get
            {
                if (_isHandler.HasValue)
                {
                    return _isHandler.Value;
                }

                _isHandler = true;
                // Se è attiva la presa in carico della pec verifico sia posta in ingresso.
                if (PecHandleEnabled(CurrentPecMail.MailBox) && CurrentPecMail.Direction == (short)PECMailDirection.Ingoing)
                {
                    _isHandler = CurrentPecMail.Handler.Eq(DocSuiteContext.Current.User.FullUserName);
                }
                return _isHandler.Value;
            }
        }

        public bool IsRestorable
        {
            get
            {
                if (_isRestorable.HasValue)
                {
                    return _isRestorable.Value;
                }

                _isRestorable = IsHandler && CurrentPecMail.IsActive.Equals(ActiveType.Cast(ActiveType.PECMailActiveType.Delete));
                return _isRestorable.Value;
            }
        }

        public bool IsProtocollable
        {
            get
            {
                if (_isProtocollable.HasValue)
                {
                    return _isProtocollable.Value;
                }

                bool isProtocollableDirection = (IsHandler && CurrentPecMail.Direction == (short)PECMailDirection.Ingoing)
                    || (DocSuiteContext.Current.ProtocolEnv.ProtocollableOutgoingPEC && CurrentPecMail.Direction == (short)PECMailDirection.Outgoing);

                _isProtocollable = isProtocollableDirection && CurrentPecMail.IsActive == (short)ActiveType.PECMailActiveType.Active
                    && !CurrentPecMail.HasDocumentUnit();

                // Verifico se per questa particolare mailbox è attiva la gestiona della destinazione.
                if (_isProtocollable.Value && DocSuiteContext.Current.ProtocolEnv.IsPECDestinationEnabled && CurrentPecMail.MailBox.IsDestinationEnabled.Value)
                {
                    // Verifico se la destinazione è obbligatoria.
                    if (!DocSuiteContext.Current.ProtocolEnv.IsPECDestinationOptional)
                    {
                        // Se la pec è stata destinata oppure è destinabile da me la rendo protocollabile.
                        bool isDestinated = CurrentPecMail.IsDestinated.HasValue && CurrentPecMail.IsDestinated.Value;
                        _isProtocollable = _isProtocollable.Value && (isDestinated || IsDestinable);
                    }
                }
                return _isProtocollable.Value;
            }
        }

        public bool IsAttachable
        {
            get
            {
                if (_isAttachable.HasValue)
                {
                    return _isAttachable.Value;
                }

                if (CurrentPecMail.Direction == (short)PECMailDirection.Ingoing)
                {
                    _isAttachable = IsProtocollable;
                    return _isAttachable.Value;
                }

                _isAttachable = CurrentPecMail.IsActive == (short)ActiveType.PECMailActiveType.Active && !CurrentPecMail.HasDocumentUnit();
                return _isAttachable.Value;
            }
        }

        public bool IsDeletable
        {
            get
            {
                if (_isDeletable.HasValue)
                {
                    return _isDeletable.Value;
                }

                // Rendo cancellabile quando ho la mail correntemente in gestione, è attiva e non è stata protocollata.
                _isDeletable = !IsPostaCertifica && (string.IsNullOrEmpty(CurrentPecMail.Handler) || IsHandler) && CurrentPecMail.IsActive == ActiveType.Cast(ActiveType.PECMailActiveType.Active)
                    && !CurrentPecMail.HasDocumentUnit();

                // Rendo cancellabile la posta in uscita quando è attiva la gestione esterna del cestino.
                if (CurrentPecMail.Direction == (short)PECMailDirection.Outgoing)
                {
                    _isDeletable = _isDeletable.Value && DocSuiteContext.Current.ProtocolEnv.PECExternalTrashbin;
                }

                // Se abilito da parametro, rendo cancellabili anche le PEC che non sono state protocollate, sono attive e in gestione
                if (DocSuiteContext.Current.ProtocolEnv.PECEnableDeleteIfNotRecorded)
                {
                    _isDeletable = _isDeletable.Value || (IsPostaCertifica && (string.IsNullOrEmpty(CurrentPecMail.Handler) || IsHandler)
                        && CurrentPecMail.IsActive == (short)ActiveType.PECMailActiveType.Active && !CurrentPecMail.HasDocumentUnit());
                }
                return _isDeletable.Value;
            }
        }

        public bool IsForwardable
        {
            get
            {
                if (_isForwardable.HasValue)
                {
                    return _isForwardable.Value;
                }

                _isForwardable = IsHandler && CurrentPecMail.IsActive == (short)ActiveType.PECMailActiveType.Active;
                return _isForwardable.Value;
            }
        }

        public bool IsDestinable
        {
            // Verifico di essere Direttore o Vice di almeno un settore abbinato alla mailbox della pec corrente.
            get
            {
                if (_isDestinable.HasValue)
                {
                    return _isDestinable.Value;
                }

                _isDestinable = IsHandler && CurrentPecMail.IsActive == ActiveType.Cast(ActiveType.PECMailActiveType.Active) && !CurrentPecMail.HasDocumentUnit()
                    && CurrentPecMail.MailBox.IsDestinationEnabled.GetValueOrDefault(false) && CurrentPecMail.Direction == (short)PECMailDirection.Ingoing
                    && FacadeFactory.Instance.PECMailboxFacade.IsRoleUserManager(CurrentPecMail.MailBox);
                return _isDestinable.Value;
            }
        }

        public bool HasError
        {
            get
            {
                if (_hasError.HasValue)
                {
                    return _hasError.Value;
                }

                List<string> errorTypes = new List<string>() {
                    PECMailTypes.PreavvisoErroreConsegna,
                    PECMailTypes.NonAccettazione,
                    PECMailTypes.ErroreConsegna
                };
                _hasError = CurrentPecMail.Receipts.Any(r => errorTypes.Any(e => e.Eq(r.ReceiptType)));
                return _hasError.Value;
            }
        }

        public bool IsDelivered
        {
            get
            {
                if (_isDelivered.HasValue)
                {
                    return _isDelivered.Value;
                }

                _isDelivered = !CurrentPecMail.Receipts.IsNullOrEmpty() && CurrentPecMail.Receipts.Any(r => r.ReceiptType.Eq(PECMailTypes.AvvenutaConsegna));
                return _isDelivered.Value;
            }
        }

        public bool IsSendable
        {
            get
            {
                return !HasAnomalies;
            }
        }

        public bool IsResendable
        {
            get
            {
                if (_isResendable.HasValue)
                {
                    return _isResendable.Value;
                }

                _isResendable = (!IsDelivered || HasError) && CurrentPecMail.HasDocumentUnit() && !HasAnomalies;

                if (_isResendable.Value && CurrentPecMail.DocumentUnit != null)
                {
                    switch (CurrentPecMail.DocumentUnit.Environment)
                    {
                        case (int)DSWEnvironment.Protocol:
                            _isResendable = _isResendable.Value && new ProtocolRights(FacadeFactory.Instance.ProtocolFacade.GetById(CurrentPecMail.DocumentUnit.Id)).IsPECSendable;
                            break;
                        default:
                            {
                                if (CurrentPecMail.DocumentUnit.Environment >= 100)
                                {
                                    _isResendable = _isResendable.Value && new UDSRepositoryRightsUtil(_currentUDSRepositoryFacade.GetById(CurrentPecMail.DocumentUnit.IdUDSRepository.Value),
                                _userName, new UDSDto()).IsPECSendable;
                                }
                            }
                            break;
                    }
                }
                return _isResendable.Value;
            }
        }

        public bool HasNoReceipts
        {
            get
            {
                return CurrentPecMail.Receipts.IsNullOrEmpty();
            }
        }

        public bool IsPostaCertifica
        {
            get
            {
                return "posta-certificata".Eq(CurrentPecMail.XTrasporto);
            }
        }

        public bool IsReceiptViewable
        {
            // Permetto la visualizzazione della ricevuta solo se la mail risulta spedita.
            get
            {
                return CurrentPecMail.MailDate.HasValue && !HasAnomalies;
            }
        }

        public bool CanHandle
        {
            get
            {
                if (_canHandle.HasValue)
                {
                    return _canHandle.Value;
                }
                //o sono un vigario * firmatario di collaborazione
                _canHandle = PecHandleEnabled(CurrentPecMail.MailBox) && IsActive && !CurrentPecMail.HasDocumentUnit()
                    && CurrentPecMail.Direction == (short)PECMailDirection.Ingoing && !IsHandler && FacadeFactory.Instance.PECMailFacade.CanChangeHandler(CurrentPecMail)
                    && CurrentUserHasPECHandleRights;
                return _canHandle.Value;
            }
        }

        public bool CurrentUserHasProtocolPECHandleRights
        {
            get
            {
                if (_currentUserHasProtocolPECHandleRights.HasValue)
                {
                    return _currentUserHasProtocolPECHandleRights.Value;
                }
                _currentUserHasProtocolPECHandleRights = CommonShared.UserProtocolCheckRight((ProtocolContainerRightPositions)DocSuiteContext.Current.ProtocolEnv.PECHandleContainerRight);
                return _currentUserHasProtocolPECHandleRights.Value;
            }
        }

        public bool CurrentUserHasUDSPECHandleRights
        {
            get
            {
                if (_currentUserHasUDSPECHandleRights.HasValue)
                {
                    return _currentUserHasUDSPECHandleRights.Value;
                }
                _currentUserHasUDSPECHandleRights = CommonShared.UserUDSCheckRight((ProtocolContainerRightPositions)DocSuiteContext.Current.ProtocolEnv.PECHandleContainerRight);
                return _currentUserHasUDSPECHandleRights.Value;
            }
        }

        public bool CurrentUserHasPECHandleRights
        {
            get
            {
                if (_currentUserHasPECHandleRights.HasValue)
                {
                    return _currentUserHasPECHandleRights.Value;
                }

                _currentUserHasPECHandleRights = CurrentUserHasProtocolPECHandleRights || CurrentUserHasUDSPECHandleRights;
                return _currentUserHasPECHandleRights.Value;
            }
        }

        #endregion

        #region [ Methods ]
        private bool PecHandleEnabled(PECMailBox mailBox)
        {
            return DocSuiteContext.Current.ProtocolEnv.PECHandlerEnabled
                && mailBox.IsHandleEnabled.HasValue && mailBox.IsHandleEnabled.Value;
        }
        #endregion
    }
}
