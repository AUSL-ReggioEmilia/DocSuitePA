using VecompSoftware.DocSuiteWeb.DTO.Desks;
using VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Desks
{
    /// <summary>
    /// Verifica lo stato dei documenti su biblos. La verifica viene fatta all'inizializzazione e poi le informazioni vengono mantenute.
    /// Per verificare nuovamente le informazioni è necessario eseguire un UpdateData()
    /// </summary>
    public class DeskDocumentRightsUtil
    {
        #region [Fields]
        private bool? _isDocumentCheckOut;
        private bool? _isDocumentCheckOutFromCurrentUser;
        private string _currentUserDocumentCheckOut = string.Empty;
        private bool? _isCheckInButtonVisible;
        private bool? _isCheckOutButtonVisible;
        private bool? _isCheckInButtonEnable;
        private bool? _isUndoCheckOutButtonVisible;
        private bool? _canDeleteDocument;
        private bool? _isRenameButtonEnable;
        private readonly string _userName;
        #endregion

        #region [Constructor]

        public DeskDocumentRightsUtil(DeskDocumentResult itemDeskDocument, DeskRightsUtil currentDeskRight, string userName)
        {
            _userName = userName;
            this.CurrentDocument = itemDeskDocument;
            this.CurrentDeskDocumentFacade = new DeskDocumentFacade(_userName);
            this.CurrentDeskRight = currentDeskRight;
        }
        #endregion

        #region [Properties]

        public DeskDocumentResult CurrentDocument { get; private set; }
        public DeskDocumentFacade CurrentDeskDocumentFacade { get; private set; }
        public DeskRightsUtil CurrentDeskRight { get; private set; }


        public bool IsDocumentCheckOut
        {
            get
            {
                if (_isDocumentCheckOut.HasValue)
                {
                    return _isDocumentCheckOut.Value;
                }

                _isDocumentCheckOut = false;
                if (!CurrentDocument.IdDocumentBiblos.HasValue)
                {
                    return _isDocumentCheckOut.Value;
                }

                _isDocumentCheckOut = CurrentDeskDocumentFacade.IsCheckOut(CurrentDocument.IdDocumentBiblos.Value);
                return _isDocumentCheckOut.Value;
            }
        }

        public bool IsDocumentCheckOutFromCurrentUser
        {
            get
            {
                if (_isDocumentCheckOutFromCurrentUser.HasValue)
                {
                    return _isDocumentCheckOutFromCurrentUser.Value;
                }

                _isDocumentCheckOutFromCurrentUser = false;
                if (!CurrentDocument.IdDocumentBiblos.HasValue)
                {
                    return _isDocumentCheckOutFromCurrentUser.Value;
                }

                _isDocumentCheckOutFromCurrentUser = (IsDocumentCheckOut && CurrentUserDocumentCheckOut.Eq(_userName));
                return _isDocumentCheckOutFromCurrentUser.Value;
            }
        }

        public string CurrentUserDocumentCheckOut
        {
            get
            {
                if (!string.IsNullOrEmpty(_currentUserDocumentCheckOut))
                {
                    return _currentUserDocumentCheckOut;
                }

                if (!CurrentDocument.IdDocumentBiblos.HasValue)
                {
                    return string.Empty;
                }

                _currentUserDocumentCheckOut = CurrentDeskDocumentFacade.IdUserCheckOut(CurrentDocument.IdDeskDocument, CurrentDocument.IdDocumentBiblos.Value);
                return _currentUserDocumentCheckOut;
            }
        }

        /// <summary>
        /// Visualizzo il bottone del checkin solo se il documento è estratto da me e ho diritti sul documento.
        /// Il bottone dovrà anche essere visibile ma disabilitato se sarà verificata l'opzione <see cref="IsCheckInButtonEnable"/>
        /// </summary>
        public bool IsCheckInButtonVisible
        {
            get
            {
                if (_isCheckInButtonVisible.HasValue)
                {
                    return _isCheckInButtonVisible.Value;
                }

                _isCheckInButtonVisible = false;
                if (!CurrentDocument.IdDocumentBiblos.HasValue || CurrentDocument.IsSigned)
                {
                    return _isCheckInButtonVisible.Value;
                }

                _isCheckInButtonVisible = ((IsDocumentCheckOutFromCurrentUser && CurrentDeskRight.CanCheckInDocument) || (!IsCheckInButtonEnable));
                return _isCheckInButtonVisible.Value;
            }
        }


        /// <summary>
        /// Visualizzo il bottone del checkout solo se il documento è archiviato e ho diritti sul documento
        /// </summary>
        public bool IsCheckOutButtonVisible
        {
            get
            {
                if (_isCheckOutButtonVisible.HasValue)
                {
                    return _isCheckOutButtonVisible.Value;
                }

                _isCheckOutButtonVisible = false;
                if (!CurrentDocument.IdDocumentBiblos.HasValue || CurrentDocument.IsSigned)
                {
                    return _isCheckOutButtonVisible.Value;
                }

                _isCheckOutButtonVisible = (!IsDocumentCheckOut && CurrentDeskRight.CanCheckOutDocument);
                return _isCheckOutButtonVisible.Value;
            }
        }

        /// <summary>
        /// Abilitazione del bottone di checkin solo se le seguenti condizioni sono vere:
        /// 1) documento in check out per l'utente corrente
        /// 2) l'utente corrente ha diritti di checkin sul documento
        /// </summary>
        public bool IsCheckInButtonEnable
        {
            get
            {
                if (_isCheckInButtonEnable.HasValue)
                {
                    return _isCheckInButtonEnable.Value;
                }

                _isCheckInButtonEnable = false;
                if (!CurrentDocument.IdDocumentBiblos.HasValue || CurrentDocument.IsSigned)
                {
                    return _isCheckInButtonEnable.Value;
                }

                _isCheckInButtonEnable = (IsDocumentCheckOut && IsDocumentCheckOutFromCurrentUser && CurrentDeskRight.CanCheckInDocument);
                return _isCheckInButtonEnable.Value;
            }
        }

        /// <summary>
        /// Rendo visibile il bottone di annulla checkout solo se sono vere le seguenti condizioni:
        /// 1) Documento in checkout dall'utente corrente
        /// 2) diritti di checkout
        /// </summary>
        public bool IsUndoCheckOutButtonVisible
        {
            get
            {
                if (_isUndoCheckOutButtonVisible.HasValue)
                {
                    return _isUndoCheckOutButtonVisible.Value;
                }

                _isUndoCheckOutButtonVisible = false;
                if (!CurrentDocument.IdDocumentBiblos.HasValue && CurrentDocument.IsSigned)
                {
                    return _isUndoCheckOutButtonVisible.Value;
                }

                _isUndoCheckOutButtonVisible = (IsDocumentCheckOutFromCurrentUser && CurrentDeskRight.CanCheckOutDocument);
                return _isUndoCheckOutButtonVisible.Value;
            }
        }

        public bool CanDeleteDocument
        {
            get
            {
                if (_canDeleteDocument.HasValue)
                    return _canDeleteDocument.Value;

                _canDeleteDocument = (!IsDocumentCheckOut || CurrentDeskRight.CurrentUserIsOwner || CurrentDeskRight.CurrentUserIsManager) && !CurrentDocument.IsSigned;
                return _canDeleteDocument.Value;
            }
        }


        public bool IsRanameButtonEnable
        {
            get
            {
                if (_isRenameButtonEnable.HasValue)
                {
                    return _isRenameButtonEnable.Value;
                }
                _isRenameButtonEnable = false;
                if (!CurrentDocument.IdDocumentBiblos.HasValue && CurrentDocument.IsSigned)
                {
                    return _isRenameButtonEnable.Value;
                }
                _isRenameButtonEnable = (!IsDocumentCheckOut) && (CurrentDeskRight.CurrentUserIsOwner || CurrentDeskRight.CurrentUserIsManager || CurrentDeskRight.CurrentUserIsContributor) && !CurrentDocument.IsSigned;
                return _isRenameButtonEnable.Value;
            }
        }
        #endregion

        #region [Methods]

        #endregion
    }
}
