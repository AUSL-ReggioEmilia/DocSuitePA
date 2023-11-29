using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.DTO.Commons;
using VecompSoftware.DocSuiteWeb.EntityMapper.Workflow;
using VecompSoftware.DocSuiteWeb.Facade.Common.Hubs;
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
            Notification model = new Notification
            {
                NotificationName = NotificationType.ProtocolliDaLeggere.ToString()
            };
            try
            {
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.LoadFetchModeFascicleEnabled = false;
                _protocolFinder.LoadFetchModeProtocolLogs = false;
                _protocolFinder.IdStatus = (int)ProtocolStatusId.Attivo;               
                _protocolFinder.TopMaxRecords = DocSuiteContext.Current.ProtocolEnv.DesktopMaxRecords;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read, _idTenantAOO);
                cutil.ExcludeInvoiceContainer(ref _protocolFinder);
                _protocolFinder.ProtocolNotReaded = true;            

                model.NotificationCount = _protocolFinder.Count();
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetProtocolInvoiceNotRead()
        {
            Notification model = new Notification
            {
                NotificationName = NotificationType.ProtocolliDifatturaDaLeggere.ToString()
            };
            try
            {
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.LoadFetchModeFascicleEnabled = false;
                _protocolFinder.LoadFetchModeProtocolLogs = false;
                _protocolFinder.IdStatus = (int)ProtocolStatusId.Attivo;
                _protocolFinder.TopMaxRecords = DocSuiteContext.Current.ProtocolEnv.DesktopMaxRecords;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read, _idTenantAOO);
                cutil.OnlyInvoiceContainer(ref _protocolFinder);
                _protocolFinder.ProtocolNotReaded = true;

                model.NotificationCount = _protocolFinder.Count();
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetProtocolToAccept()
        {
            Notification model = new Notification();
            model.NotificationName = NotificationType.ProtocolliDaAccettare.ToString();
            try
            {
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.RestrictionOnlyRoles = true;
                _protocolFinder.OnlyExplicitRoles = true;
                _protocolFinder.ProtocolRoleStatus = ProtocolRoleStatus.ToEvaluate;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read, _idTenantAOO);

                model.NotificationCount = _protocolFinder.Count();
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetProtocolRefused()
        {
            Notification model = new Notification();
            model.NotificationName = NotificationType.ProtocolliRespinti.ToString();
            try
            {
                CommonUtil cutil = new CommonUtil();
                NHibernateProtocolFinder _protocolFinder = new NHibernateProtocolFinder();
                _protocolFinder.IdTenantAOO = _idTenantAOO;
                _protocolFinder.ProtocolRoleStatus = ProtocolRoleStatus.Refused;
                _protocolFinder.IsInRefusedProtocolRoleGroup = true;
                DateTime dateFrom = DateTime.Today.AddDays(-DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff);
                _protocolFinder.RegistrationDateFrom = dateFrom;
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read, _idTenantAOO);

                model.NotificationCount = _protocolFinder.Count();
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetProtocolToDistribute()
        {
            Notification model = new Notification
            {
                NotificationName = NotificationType.ProtocolliDaDistribuire.ToString()
            };
            try
            {
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
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Distribute, _idTenantAOO);

                model.NotificationCount = _protocolFinder.Count();
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetHighlightProtocols()
        {
            Notification model = new Notification
            {
                NotificationName = NotificationType.ProtocolliInEvidenza.ToString()
            };
            try
            {
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

                model.NotificationCount = _protocolFinder.Count();
                return model;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
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
                cutil.ApplyProtocolFinderSecurity(ref _protocolFinder, SecurityType.Read, _idTenantAOO);

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
            Notification model = new Notification
            {
                NotificationName = NotificationType.CollaborazioniDaProtocollare.ToString()
            };
            try
            {
                WebAPIFinder.Collaborations.CollaborationFinder _collaborationFinder = InitCollaborationFinder();

                WebAPIImpersonatorFacade.ImpersonateFinder(_collaborationFinder,
                    (impersonationType, finder) =>
                    {
                        finder.UserName = DocSuiteContext.Current.User.UserName;
                        finder.Domain = DocSuiteContext.Current.User.Domain;
                        finder.CollaborationFinderActionType = WebAPIFinder.Collaborations.CollaborationFinderActionType.ToManage;

                        finder.DoSearchHeader();
                        model.NotificationCount = finder.Count();
                    });
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetCollaborationToVision()
        {
            Notification model = new Notification
            {
                NotificationName = NotificationType.CollaborazioniDaVisionare.ToString()
            };
            try
            {
                WebAPIFinder.Collaborations.CollaborationFinder _collaborationFinder = InitCollaborationFinder();

                WebAPIImpersonatorFacade.ImpersonateFinder(_collaborationFinder,
                    (impersonationType, finder) =>
                    {
                        finder.UserName = DocSuiteContext.Current.User.UserName;
                        finder.Domain = DocSuiteContext.Current.User.Domain;
                        finder.CollaborationFinderActionType = WebAPIFinder.Collaborations.CollaborationFinderActionType.ToVisionSign;
                        finder.CollaborationFinderFilterType = WebAPIFinder.Collaborations.CollaborationFinderFilterType.AllCollaborations;

                        finder.DoSearchHeader();
                        model.NotificationCount = finder.Count();
                    });
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetWorkflowByUser()
        {
            Notification model = new Notification
            {
                NotificationName = NotificationType.WorkflowUtenteCorrente.ToString()
            };
            try
            {
                WorkflowActivityFinder _wfFinder = new WorkflowActivityFinder(new MapperWorkflowActivity(), DocSuiteContext.Current.User.FullUserName);
                _wfFinder.WorkflowActivityStatus = new Collection<WorkflowStatus> { WorkflowStatus.Active, WorkflowStatus.Progress };
                _wfFinder.IdTenant = FacadeFactory.Instance.UserLogFacade.GetByUser(DocSuiteContext.Current.User.FullUserName).CurrentTenantId;
                _wfFinder.IsVisible = true;
                model.NotificationCount = _wfFinder.Count();
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetPECNotRead()
        {
            Notification model = new Notification
            {
                NotificationName = NotificationType.PECDaLeggere.ToString()
            };
            try
            {
                CommonUtil cutil = new CommonUtil();
                NHibernatePECMailFinder _pecMailFinder = new NHibernatePECMailFinder
                {
                    Actives = true,
                    Direction = (short)PECMailDirection.Ingoing,
                    NotRead = true,
                    CheckSecurityRight = true,
                    OnlyProtocolBox = false

                };
                cutil.ApplyPecMailFinderSecurity(ref _pecMailFinder, _idTenantAOO);
                model.NotificationCount = _pecMailFinder.Count();
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }

        private Notification GetLastPagesToSign()
        {
            Notification model = new Notification
            {
                NotificationName = NotificationType.UltimePagineDaFirmare.ToString()
            };
            try
            {
                model.NotificationCount = new ResolutionFacade().GetPublicated(null, DateTime.Now, null, null, null, null).Count;
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                model.NotificationCount = -1;
            }
            return model;
        }
    }
}
