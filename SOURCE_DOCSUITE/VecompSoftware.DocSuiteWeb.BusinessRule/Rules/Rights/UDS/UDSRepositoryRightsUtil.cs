using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.DTO.UDS;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS
{
    public class UDSRepositoryRightsUtil
    {
        #region [ Fields ]
        private readonly UDSDto _UDSDto;
        private readonly string _userName;
        private bool? _isCurrentUserAuthorized;
        private Guid? _currentIdWorkflowActivity;
        private bool? _isPECSendable;
        private bool? _isActive;
        private bool? _isProtocollable;
        private bool? _isWorkflowEnabled;
        private bool? _isEditable;
        private bool? _isDeletable;
        private bool? _isClonable;
        private bool? _isMailSendable;
        private bool? _isMailRoleSendable;
        private bool? _isAuthorizable;
        private bool? _cancelMotivationRequired;
        private UDSModel _UDSmodel = null;
        private bool? _isSummaryViewable;
        private bool? _isDocumentsViewable;
        private bool? _isInsertable;
        private bool? _isArchiveableByPEC;
        private bool _viewLog;
        private bool? _hasWorkflowInProgress;
        private UDSUserFinder _currentUDSUserFinder;
        private WorkflowActivityFinder _currentWorkflowActivityFinder;
        #endregion

        #region [ Constructor ]
        private UDSRepositoryRightsUtil(string userName, UDSDto udsDto)
        {
            _UDSDto = udsDto;
            _userName = userName;
        }

        public UDSRepositoryRightsUtil(Data.Entity.UDS.UDSRepository udsRepository, string userName, UDSDto udsDto)
            : this(userName, udsDto)
        {
            CurrentUDSRepository = udsRepository;
        }
        #endregion

        #region [ Properties ]
        public Data.Entity.UDS.UDSRepository CurrentUDSRepository { get; private set; }

        private UDSUserFinder CurrentUDSUserFinder
        {
            get
            {
                if (_currentUDSUserFinder == null)
                {
                    _currentUDSUserFinder = new UDSUserFinder(DocSuiteContext.Current.Tenants);
                }

                return _currentUDSUserFinder;
            }
        }

        private WorkflowActivityFinder CurrentWorkflowActivityFinder
        {
            get
            {
                if (_currentWorkflowActivityFinder == null)
                {
                    _currentWorkflowActivityFinder = new WorkflowActivityFinder(DocSuiteContext.Current.Tenants);
                }

                return _currentWorkflowActivityFinder;
            }
        }

        private UDSModel CurrentUDSModel
        {
            get
            {
                if (_UDSmodel != null)
                {
                    return _UDSmodel;
                }

                _UDSmodel = UDSModel.LoadXml(CurrentUDSRepository.ModuleXML);
                return _UDSmodel;
            }
        }

        public bool IsActive
        {
            get
            {
                if (!_isActive.HasValue)
                {
                    _isActive = _UDSDto.Status == 1;
                }
                return _isActive.Value;
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

                bool protocolEnabled = CurrentUDSModel.Model.ProtocolEnabled;
                _isProtocollable = IsActive && protocolEnabled && IsEditable && !HasWorkflowInProgress;
                return _isProtocollable.Value;
            }
        }

        public bool IsWorkflowEnabled
        {
            get
            {
                if (_isWorkflowEnabled.HasValue)
                {
                    return _isWorkflowEnabled.Value;
                }

                bool workflowEnabled = DocSuiteContext.Current.ProtocolEnv.WorkflowManagerEnabled && CurrentUDSModel.Model.WorkflowEnabled;
                _isWorkflowEnabled = workflowEnabled && IsEditable && !HasWorkflowInProgress;
                return _isWorkflowEnabled.Value;
            }
        }

        public bool IsInsertable
        {
            get
            {
                if (_isInsertable.HasValue)
                {
                    return _isInsertable.Value;
                }

                _isInsertable = FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentUDSRepository.Container.Id,
                    DSWEnvironment.UDS, (int)UDSRightPositions.Insert, true);
                return _isInsertable.Value;
            }
        }

        public bool IsEditable
        {
            get
            {
                if (_isEditable.HasValue)
                {
                    return _isEditable.Value;
                }

                _isEditable = IsActive && FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentUDSRepository.Container.Id,
                    DSWEnvironment.UDS, (int)UDSRightPositions.Modify, true) && !HasWorkflowInProgress;
                if (_isEditable.HasValue && !_isEditable.Value && _UDSDto.UDSModel.Model.Authorizations.Instances != null && _UDSDto.UDSModel.Model.Authorizations.Instances.Any())
                {
                    IList<Role> roles = FacadeFactory.Instance.RoleFacade.GetByIds(_UDSDto.UDSModel.Model.Authorizations.Instances.Where(f=> f.AuthorizationType == AuthorizationType.Responsible).Select(t => t.IdAuthorization).ToList());
                    _isEditable = FacadeFactory.Instance.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.DocumentSeries, roles);
                }
                if (_isEditable.HasValue && !_isEditable.Value)
                {
                    _isEditable = CurrentUserWorkflowActivity.HasValue;
                }

                return _isEditable.Value;
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

                _isDeletable = IsActive && FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentUDSRepository.Container.Id,
                    DSWEnvironment.UDS, (int)UDSRightPositions.Cancel, true) && !HasWorkflowInProgress;
                return _isDeletable.Value;
            }
        }

        public bool IsClonable
        {
            get
            {
                if (_isClonable.HasValue)
                {
                    return _isClonable.Value;
                }

                _isClonable = IsActive && IsEditable && !HasWorkflowInProgress;
                if (_isClonable.HasValue && !_isClonable.Value)
                {
                    _isClonable = CurrentUserWorkflowActivity.HasValue;
                }

                return _isClonable.Value;
            }
        }

        public bool IsPECSendable
        {
            get
            {
                if (_isPECSendable.HasValue)
                {
                    return _isPECSendable.Value;
                }

                bool pecEnabled = CurrentUDSModel.Model.PECButtonEnabled;
                _isPECSendable = IsActive && pecEnabled && FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentUDSRepository.Container.Id,
                    DSWEnvironment.UDS, (int)UDSRightPositions.PECOutgoing, true) && !HasWorkflowInProgress;
                return _isPECSendable.Value;
            }
        }

        public bool IsMailSendable
        {
            get
            {
                if (_isMailSendable.HasValue)
                {
                    return _isMailSendable.Value;
                }

                _isMailSendable = IsActive && CurrentUDSModel.Model.MailButtonEnabled;
                return _isMailSendable.Value;
            }
        }

        public bool IsMailRoleSendable
        {
            get
            {
                if (_isMailRoleSendable.HasValue)
                {
                    return _isMailRoleSendable.Value;
                }

                _isMailRoleSendable = IsActive && CurrentUDSModel.Model.MailRoleButtonEnabled && _UDSDto != null && _UDSDto.Authorizations != null && _UDSDto.Authorizations.Any();
                return _isMailRoleSendable.Value;
            }
        }

        public bool IsAuthorizable
        {
            get
            {
                if (_isAuthorizable.HasValue)
                {
                    return _isAuthorizable.Value;
                }

                _isAuthorizable = FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentUDSRepository.Container.Id,
                        DSWEnvironment.UDS, DocSuiteContext.Current.ProtocolEnv.AuthorizedSecurity, true);
                if (_isAuthorizable.HasValue && !_isAuthorizable.Value && _UDSDto.UDSModel.Model.Authorizations.Instances != null && _UDSDto.UDSModel.Model.Authorizations.Instances.Any())
                {
                    IList<Role> roles = FacadeFactory.Instance.RoleFacade.GetByIds(_UDSDto.UDSModel.Model.Authorizations.Instances.Select(t => t.IdAuthorization).ToList());
                    _isAuthorizable = FacadeFactory.Instance.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.DocumentSeries, roles);
                }

                return _isAuthorizable.Value;
            }
        }

        public bool CancelMotivationRequired
        {
            get
            {
                if (_cancelMotivationRequired.HasValue)
                {
                    return _cancelMotivationRequired.Value;
                }

                _cancelMotivationRequired = CurrentUDSModel.Model.CancelMotivationRequired && !HasWorkflowInProgress;
                return _cancelMotivationRequired.Value;
            }
        }

        public bool IsSummaryViewable
        {
            get
            {
                if (_isSummaryViewable.HasValue)
                {
                    return _isSummaryViewable.Value;
                }

                _isSummaryViewable = FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentUDSRepository.Container.Id, DSWEnvironment.UDS, (int)UDSRightPositions.Read, true);
                if (!_isSummaryViewable.Value)
                {
                    if (_UDSDto.UDSModel.Model.Authorizations != null && _UDSDto.UDSModel.Model.Authorizations.Instances != null)
                    {
                        IList<Role> roles = FacadeFactory.Instance.RoleFacade.GetByIds(_UDSDto.UDSModel.Model.Authorizations.Instances.Select(s => s.IdAuthorization).ToList());
                        _isSummaryViewable = FacadeFactory.Instance.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.DocumentSeries, roles);
                    }
                }

                return _isSummaryViewable.Value;
            }
        }

        public bool IsDocumentsViewable
        {
            get
            {
                if (_isDocumentsViewable.HasValue)
                {
                    return _isDocumentsViewable.Value;
                }

                _isDocumentsViewable = FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentUDSRepository.Container.Id, DSWEnvironment.UDS, (int)UDSRightPositions.ViewDocuments, true);
                if (!_isDocumentsViewable.Value)
                {
                    if (_UDSDto.UDSModel.Model.Authorizations != null && _UDSDto.UDSModel.Model.Authorizations.Instances != null)
                    {
                        IList<Role> roles = FacadeFactory.Instance.RoleFacade.GetByIds(_UDSDto.UDSModel.Model.Authorizations.Instances.Select(s => s.IdAuthorization).ToList());
                        _isDocumentsViewable = FacadeFactory.Instance.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.DocumentSeries, roles);
                    }
                }

                return _isDocumentsViewable.Value;
            }
        }

        public bool IsArchiveableByPEC
        {
            get
            {
                if (_isArchiveableByPEC.HasValue)
                {
                    return _isArchiveableByPEC.Value;
                }

                _isArchiveableByPEC = _UDSDto.UDSModel.Model.PECEnabled && IsInsertable && FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentUDSRepository.Container.Id,
                    DSWEnvironment.UDS, (int)UDSRightPositions.PECIngoing, true) && !HasWorkflowInProgress;
                return _isArchiveableByPEC.Value;
            }
        }

        public bool IsCurrentUserAuthorized
        {
            get
            {
                if (_isCurrentUserAuthorized.HasValue)
                {
                    return _isCurrentUserAuthorized.Value;
                }
                _isCurrentUserAuthorized = false;
                CurrentUDSUserFinder.ResetDecoration();
                CurrentUDSUserFinder.IdUDS = _UDSDto.Id;
                CurrentUDSUserFinder.EnablePaging = false;
                CurrentUDSUserFinder.Domain = DocSuiteContext.Current.User.Domain;
                CurrentUDSUserFinder.Username = DocSuiteContext.Current.User.UserName;
                CurrentUDSUserFinder.CheckUserAuthorization = true;
                ICollection<WebAPIDto<UDSUser>> result = CurrentUDSUserFinder.DoSearch();

                if (result != null && result.Count() > 0)
                {
                    ICollection<UDSUser> authorizedUsers = result.Select(x => x.Entity).ToList();
                    if (authorizedUsers.Count() > 0)
                    {
                        _isCurrentUserAuthorized = true;
                    }
                }
                return _isCurrentUserAuthorized.Value;
            }
        }


        public bool EnableViewLog
        {
            get
            {
                if (CommonShared.HasGroupAdministratorRight)
                {
                    return _viewLog = true;
                }

                if (!string.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.EnvGroupLogView))
                {
                    return _viewLog = CommonShared.HasGroupLogViewRight;
                }
                return _viewLog;
            }
        }

        public bool HasWorkflowInProgress
        {
            get
            {
                if (!_hasWorkflowInProgress.HasValue)
                {
                    CurrentWorkflowActivityFinder.ResetDecoration();
                    CurrentWorkflowActivityFinder.EnablePaging = false;
                    CurrentWorkflowActivityFinder.DocumentUnitReferenced = _UDSDto.Id;
                    CurrentWorkflowActivityFinder.Top1 = true;
                    CurrentWorkflowActivityFinder.WorkflowInstanceInProgress = true;
                    ICollection<WebAPIDto<WorkflowActivity>> result = CurrentWorkflowActivityFinder.DoSearch();

                    _hasWorkflowInProgress = result != null && result.Count() > 0;
                }
                return _hasWorkflowInProgress.Value;
            }
        }

        public Guid? CurrentUserWorkflowActivity
        {
            get
            {
                if (!_currentIdWorkflowActivity.HasValue)
                {
                    CurrentWorkflowActivityFinder.ResetDecoration();
                    CurrentWorkflowActivityFinder.EnablePaging = false;
                    CurrentWorkflowActivityFinder.DocumentUnitReferenced = _UDSDto.Id;
                    CurrentWorkflowActivityFinder.IsAuthorized = true;
                    CurrentWorkflowActivityFinder.Account = _userName;
                    CurrentWorkflowActivityFinder.Statuses = new List<WorkflowStatus>() { WorkflowStatus.Todo, WorkflowStatus.Progress };
                    CurrentWorkflowActivityFinder.Top1 = true;
                    CurrentWorkflowActivityFinder.WorkflowInstanceInProgress = true;
                    ICollection<WebAPIDto<WorkflowActivity>> result = CurrentWorkflowActivityFinder.DoSearch();

                    _currentIdWorkflowActivity = null;
                    if (result.Any())
                    {
                        _currentIdWorkflowActivity = result.OrderByDescending(f => f.Entity.RegistrationDate).First().Entity.UniqueId;
                    }
                }
                return _currentIdWorkflowActivity;
            }
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
