using Microsoft.Activities;
using Microsoft.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Services.WorkflowManager
{
    [LogCategory(LogCategoryDefinition.SERVICEWF)]
    public class WorkflowInstanceManager : IWorkflowInstanceManager
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ClientSettings _settings;
        private readonly string _workflowName;
        private readonly string _worklowManagerScopeUrl;
        private readonly TimeSpan _pollingInterval;
        private readonly Timer _timer;
        private bool _isWorking = false;
        private readonly object _thisLock = new object();
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WorkflowInstanceManager));
                }
                return _logCategories;
            }
        }

        private bool IsWorking
        {
            get
            {
                lock (_thisLock)
                {
                    return _isWorking;
                }
            }
            set
            {
                lock (_thisLock)
                {
                    _isWorking = value;
                }
            }
        }
        #endregion

        #region [ Events ]
        public event EventHandler<WorkflowInstancesEventArgs> InstancesChanged;
        #endregion

        #region [ Constructor ]
        public WorkflowInstanceManager(ILogger logger, string workflowName, string worklowManagerScopeUrl, TimeSpan pollingInterval)
        {
            _logger = logger;
            _worklowManagerScopeUrl = worklowManagerScopeUrl;
            _pollingInterval = pollingInterval;
            _workflowName = workflowName;
            _timer = new Timer();
            _settings = new ClientSettings
            {
                Credentials = CredentialCache.DefaultNetworkCredentials,
                RequestTimeout = TimeSpan.FromSeconds(60)
            };
        }
        #endregion

        #region [ Methods ]
        public void Start()
        {
            _logger.WriteInfo(new LogMessage("WorkflowInstanceManager.Start -> start new timer"), LogCategories);
            _timer.Interval = _pollingInterval.TotalMilliseconds;
            _timer.Elapsed += timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsWorking)
            {
                return;
            }

            try
            {
                IsWorking = true;
                CheckCompletedInstancesAsync().Wait();
                CheckSuspededInstancesAsync().Wait();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("Check workflow instaces error"), ex, LogCategories);
            }
            finally
            {
                _logger.WriteInfo(new LogMessage("Check workflow instaces completed"), LogCategories);
                IsWorking = false;
            }
        }

        public void Stop()
        {
            _logger.WriteInfo(new LogMessage("WorkflowInstanceManager.Start -> stopping timer"), LogCategories);
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        private async Task CheckCompletedInstancesAsync()
        {
            ICollection<WorkflowInstanceInfo> instances = await GetInstancesAsync(WorkflowInstanceStatus.Completed);
            if (instances.Count == 0)
            {
                return;
            }

            InstancesChanged(this, new WorkflowInstancesEventArgs()
            {
                Instances = instances.Select(s => new Model.Workflows.WorkflowInstanceModel()
                {
                    InstanceId = Guid.Parse(s.InstanceName),
                    Status = (Model.Workflows.WorkflowInstanceStatus)s.WorkflowStatus
                }).ToList()
            });
        }

        private async Task CheckSuspededInstancesAsync()
        {
            ICollection<WorkflowInstanceInfo> instances = await GetInstancesAsync(WorkflowInstanceStatus.Suspended);
            if (instances.Count == 0)
            {
                return;
            }

            InstancesChanged(this, new WorkflowInstancesEventArgs()
            {
                Instances = instances.Select(s => new Model.Workflows.WorkflowInstanceModel()
                {
                    InstanceId = Guid.Parse(s.InstanceName),
                    Status = (Model.Workflows.WorkflowInstanceStatus)s.WorkflowStatus
                }).ToList()
            });
        }

        private async Task<ICollection<WorkflowInstanceInfo>> GetInstancesAsync(WorkflowInstanceStatus status)
        {
            return await Task.Run<ICollection<WorkflowInstanceInfo>>(() =>
            {
                try
                {
                    WorkflowManagementClient workflowClient = new WorkflowManagementClient(new Uri(new Uri(_workflowName), _workflowName.TrimStart('/')), _settings);
                    long totalInstances = workflowClient.Instances.GetCount(_workflowName, status, null);
                    int counting = 0;
                    ICollection<WorkflowInstanceInfo> instances = new List<WorkflowInstanceInfo>();
                    while (counting < totalInstances)
                    {
                        ICollection<WorkflowInstanceInfo> tmpInstances = workflowClient.Instances.Get(counting, 100, _workflowName, status);
                        instances = instances.Union(tmpInstances).ToList();
                        counting += tmpInstances.Count;
                    }
                    _logger.WriteInfo(new LogMessage(string.Format("WorkflowInstanceManager.CheckInstancesAsync -> found {0} instances - status {1}", instances.Count, status.ToString())), LogCategories);
                    return instances.Where(x => x.CreationTime > DateTime.UtcNow.AddDays(-2).Date).ToList();
                }
                catch (Exception ex)
                {
                    _logger.WriteError(new LogMessage(string.Format("WorkflowInstanceManager.CheckInstancesAsync -> failed to fetch instances for workflow named '{0}'.", _workflowName)), ex, LogCategories);
                }
                return new List<WorkflowInstanceInfo>();
            });
        }
        #endregion
    }
}
