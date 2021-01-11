using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNet.SignalR;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.DTO.Commons;
using VecompSoftware.DocSuiteWeb.EntityMapper.Workflow;
using VecompSoftware.DocSuiteWeb.Facade.Common.Hubs;
using VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI;
using VecompSoftware.Services.Logging;
using WebAPIFinder = VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Commons
{
    public class NotificationCounter
    {
        #region [ Fields ]
        private readonly NotificationHub _notificationHub = null;
        private readonly Guid _idTenantAOO;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public NotificationCounter(NotificationHub notificationHub, Guid idTenantAOO)
        {
            _notificationHub = notificationHub;
            _idTenantAOO = idTenantAOO;
        }

        #endregion

        public void UpdateNotification(string connectionId, ICollection<Notification> notifications)
        {
            _notificationHub.Clients.Client(connectionId).notificationCounterMessage(notifications);
        }

        public void GetNotifications(string connectionId)
        {
            ICollection<Notification> notifications = new List<Notification>();
            foreach (NotificationType notification in DocSuiteContext.Current.ProtocolEnv.NotificationTypes)
            {
                switch (notification)
                {
                    case NotificationType.ProtocolliDaLeggere:
                        notifications.Add(GetProtocolNotRead());
                        break;
                    case NotificationType.ProtocolliDaDistribuire:
                        if (DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled)
                        {
                            notifications.Add(GetProtocolToDistribute());
                        }
                        break;
                    case NotificationType.ProtocolliInEvidenza:
                        if (DocSuiteContext.Current.ProtocolEnv.ProtocolHighlightEnabled)
                        {
                            notifications.Add(GetHighlightProtocols());
                        }
                        break;
                    case NotificationType.ProtocolliRigettati:
                        if (DocSuiteContext.Current.ProtocolEnv.ProtocolRejectionEnabled)
                        {
                            notifications.Add(GetProtocolRejected());
                        }
                        break;
                    case NotificationType.CollaborazioniDaProtocollare:
                        notifications.Add(GetCollaborationToProtocol());
                        break;
                    case NotificationType.CollaborazioniDaVisionare:
                        notifications.Add(GetCollaborationToVision());
                        break;
                    case NotificationType.WorkflowUtenteCorrente:
                        notifications.Add(GetWorkflowByUser());
                        break;
                    case NotificationType.PECDaLeggere:
                        notifications.Add(GetPECNotRead());
                        break;
                    case NotificationType.ProtocolliDaAccettare:
                        if (DocSuiteContext.Current.ProtocolEnv.RefusedProtocolAuthorizationEnabled)
                        {
                            notifications.Add(GetProtocolToAccept());
                        }                                
                        break;
                    case NotificationType.ProtocolliRespinti:
                        if(DocSuiteContext.Current.ProtocolEnv.RefusedProtocolAuthorizationEnabled && CommonShared.HasRefusedProtocolGroupsRight)
                        {
                            notifications.Add(GetProtocolRefused());
                        }                                
                        break;
                    case NotificationType.UltimePagineDaFirmare:
                        if (DocSuiteContext.Current.IsResolutionEnabled && DocSuiteContext.Current.ResolutionEnv.ShowMassiveResolutionSearchPageEnabled && CommonShared.HasGroupDigitalLastPageRight)
                        {
                            notifications.Add(GetLastPagesToSign()); 
                        }
                        break;
                    case NotificationType.ProtocolliDifatturaDaLeggere:
                        {
                            if (DocSuiteContext.Current.ProtocolEnv.InvoiceSDIEnabled)
                            {
                                notifications.Add(GetProtocolInvoiceNotRead());
                            }
                            break;
                        }

                }
            }

            UpdateNotification(connectionId, notifications);
        }

        private Notification GetProtocolNotRead()
        {
            try
            {
                Notification model = new Notification();
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.LoadFetchModeFascicleEnabled = false;
                _protocolFinder.LoadFetchModeProtocolLogs = false;
                _protocolFinder.IdStatus = (int)ProtocolStatusId.Attivo;               
                _protocolFinder.TopMaxRecords = DocSuiteContext.Current.ProtocolEnv.DesktopMaxRecords;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read);
                cutil.ExcludeInvoiceContainer(ref _protocolFinder);
                _protocolFinder.ProtocolNotReaded = true;            

                model.NotificationName = NotificationType.ProtocolliDaLeggere.ToString();
                model.NotificationCount = _protocolFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }
        
        private Notification GetProtocolInvoiceNotRead()
        {
            try
            {
                Notification model = new Notification();
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.LoadFetchModeFascicleEnabled = false;
                _protocolFinder.LoadFetchModeProtocolLogs = false;
                _protocolFinder.IdStatus = (int)ProtocolStatusId.Attivo;
                _protocolFinder.TopMaxRecords = DocSuiteContext.Current.ProtocolEnv.DesktopMaxRecords;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read);
                cutil.OnlyInvoiceContainer(ref _protocolFinder);
                _protocolFinder.ProtocolNotReaded = true;

                model.NotificationName = NotificationType.ProtocolliDifatturaDaLeggere.ToString();
                model.NotificationCount = _protocolFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetProtocolToAccept()
        {
            try
            {
                Notification model = new Notification();
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.RestrictionOnlyRoles = true;
                _protocolFinder.OnlyExplicitRoles = true;
                _protocolFinder.ProtocolRoleStatus = ProtocolRoleStatus.ToEvaluate;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read);

                model.NotificationName = NotificationType.ProtocolliDaAccettare.ToString();
                model.NotificationCount = _protocolFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetProtocolRefused()
        {
            try
            {
                Notification model = new Notification();
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.ProtocolRoleStatus = ProtocolRoleStatus.Refused;
                _protocolFinder.IsInRefusedProtocolRoleGroup = true;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read);

                model.NotificationName = NotificationType.ProtocolliRespinti.ToString();
                model.NotificationCount = _protocolFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetProtocolToDistribute()
        {
            try
            {
                Notification model = new Notification();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                CommonUtil cutil = new CommonUtil();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.LoadFetchModeFascicleEnabled = false;
                _protocolFinder.LoadFetchModeProtocolLogs = false;
                _protocolFinder.NotDistributed = true;
                _protocolFinder.IdTypes = DocSuiteContext.Current.ProtocolEnv.ProtocolDistributionTypologies;
                _protocolFinder.IdStatus = (int)ProtocolStatusId.Attivo;
                int dayDiff = DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff;
                _protocolFinder.RegistrationDateFrom = DateTime.Today.AddDays(-dayDiff);
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Distribute);

                model.NotificationName = NotificationType.ProtocolliDaDistribuire.ToString();
                model.NotificationCount = _protocolFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetHighlightProtocols()
        {
            try
            {
                Notification model = new Notification();
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.LoadFetchModeFascicleEnabled = false;
                _protocolFinder.LoadFetchModeProtocolLogs = false;
                _protocolFinder.IdStatus = (int)ProtocolStatusId.Attivo;
                _protocolFinder.TopMaxRecords = DocSuiteContext.Current.ProtocolEnv.DesktopMaxRecords;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                _protocolFinder.ProtocolHighlightToMe = true;

                model.NotificationName = NotificationType.ProtocolliInEvidenza.ToString();
                model.NotificationCount = _protocolFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetProtocolRejected()
        {
            try
            {
                Notification model = new Notification();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                CommonUtil cutil = new CommonUtil();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.LoadFetchModeFascicleEnabled = false;
                _protocolFinder.LoadFetchModeProtocolLogs = false;
                _protocolFinder.IdStatus = (int)ProtocolStatusId.Rejected;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read);

                model.NotificationName = NotificationType.ProtocolliRigettati.ToString();
                model.NotificationCount = _protocolFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private WebAPIFinder.Collaborations.CollaborationFinder InitCollaborationFinder()
        {
            try
            {
                WebAPIFinder.Collaborations.CollaborationFinder currentCollaborationFinder = null;
                if (DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled)
                {
                    currentCollaborationFinder = new WebAPIFinder.Collaborations.CollaborationFinder(DocSuiteContext.Current.Tenants);
                }
                else
                {
                    currentCollaborationFinder = new WebAPIFinder.Collaborations.CollaborationFinder(DocSuiteContext.Current.CurrentTenant);
                }
                return currentCollaborationFinder;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetCollaborationToProtocol()
        {
            try
            {
                Notification model = new Notification();
                WebAPIFinder.Collaborations.CollaborationFinder _collaborationFinder = InitCollaborationFinder();

                WebAPIImpersonatorFacade.ImpersonateFinder(_collaborationFinder,
                    (impersonationType, finder) =>
                    {
                        finder.UserName = DocSuiteContext.Current.User.UserName;
                        finder.Domain = DocSuiteContext.Current.User.Domain;
                        finder.CollaborationFinderActionType = WebAPIFinder.Collaborations.CollaborationFinderActionType.ToManage;

                        model.NotificationName = NotificationType.CollaborazioniDaProtocollare.ToString();
                        finder.DoSearchHeader();
                        model.NotificationCount = finder.Count();
                    });
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetCollaborationToVision()
        {
            try
            {
                Notification model = new Notification();
                WebAPIFinder.Collaborations.CollaborationFinder _collaborationFinder = InitCollaborationFinder();

                WebAPIImpersonatorFacade.ImpersonateFinder(_collaborationFinder,
                    (impersonationType, finder) =>
                    {
                        finder.UserName = DocSuiteContext.Current.User.UserName;
                        finder.Domain = DocSuiteContext.Current.User.Domain;
                        finder.CollaborationFinderActionType = WebAPIFinder.Collaborations.CollaborationFinderActionType.ToVisionSign;
                        finder.CollaborationFinderFilterType = WebAPIFinder.Collaborations.CollaborationFinderFilterType.AllCollaborations;

                        model.NotificationName = NotificationType.CollaborazioniDaVisionare.ToString();
                        finder.DoSearchHeader();
                        model.NotificationCount = finder.Count();
                    });
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetWorkflowByUser()
        {
            try
            {
                Notification model = new Notification();
                WorkflowActivityFinder _wfFinder = new WorkflowActivityFinder(new MapperWorkflowActivity(), DocSuiteContext.Current.User.FullUserName);
                _wfFinder.WorkflowActivityStatus = new Collection<WorkflowStatus> { WorkflowStatus.Active, WorkflowStatus.Progress };
                _wfFinder.IdTenant = FacadeFactory.Instance.UserLogFacade.GetByUser(DocSuiteContext.Current.User.FullUserName).CurrentTenantId;
                model.NotificationName = NotificationType.WorkflowUtenteCorrente.ToString();
                model.NotificationCount = _wfFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }
        private Notification GetPECNotRead()
        {
            try
            {
                Notification model = new Notification();
                CommonUtil cutil = new CommonUtil();
                NHibernatePECMailFinder _pecMailFinder = new NHibernatePECMailFinder();
                _pecMailFinder.Actives = true;
                _pecMailFinder.Direction = (short) PECMailDirection.Ingoing;
                _pecMailFinder.NotRead = true;
                _pecMailFinder.CheckSecurityRight = true;
                cutil.ApplyPecMailFinderSecurity(ref _pecMailFinder);                                

                model.NotificationName = NotificationType.PECDaLeggere.ToString();
                model.NotificationCount = _pecMailFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }

        private Notification GetLastPagesToSign()
        {
            try
            {
                Notification model = new Notification();                

                model.NotificationName = NotificationType.UltimePagineDaFirmare.ToString();
                model.NotificationCount = new ResolutionFacade().GetPublicated(null, DateTime.Now, null, null, null, null).Count;
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw;
            }
        }
    }
}
