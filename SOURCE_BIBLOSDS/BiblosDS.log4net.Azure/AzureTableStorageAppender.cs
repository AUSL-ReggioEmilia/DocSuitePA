/*
Copyright 2011 Vidar Kongsli

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0
	
Unless required by applicable law or agreed to in writing, software 
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. 
 */
using System;
using System.Data.Services.Client;
using log4net.Appender;
using log4net.Core;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace BiblosDS.Log4Net.Azure
{
    public class AzureTableStorageAppender : AppenderSkeleton
    {        
        private TableHelper tableHelper;
        private string tableName = "Log";


        public override void ActivateOptions()
        {
            base.ActivateOptions();
            this.tableHelper = new TableHelper("TableStorageConnectionString", RoleEnvironment.IsAvailable);
            this.tableName = (RoleEnvironment.CurrentRoleInstance.Role.Name.IndexOf('.') >= 0) ? RoleEnvironment.CurrentRoleInstance.Role.Name.Substring(RoleEnvironment.CurrentRoleInstance.Role.Name.IndexOf('.') + 1) : RoleEnvironment.CurrentRoleInstance.Role.Name;
            this.tableName = this.tableName.Replace(".", "");
            this.tableHelper.CreateTable(this.tableName);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            Action doWriteToLog = () =>
            {
                try
                {
                    LogEntry entry = new LogEntry
                    {
                        RoleInstance = RoleEnvironment.CurrentRoleInstance.Id,
                        DeploymentId = RoleEnvironment.DeploymentId
                    };
                    entry.Timestamp = loggingEvent.TimeStamp;
                    entry.Message = loggingEvent.RenderedMessage;
                    entry.Level = loggingEvent.Level.Name;
                    entry.LoggerName = loggingEvent.LoggerName;
                    entry.Domain = loggingEvent.Domain;
                    entry.ThreadName = loggingEvent.ThreadName;
                    entry.Identity = loggingEvent.Identity;
                    this.tableHelper.InsertEntity(this.tableName, entry);
                }
                catch (DataServiceRequestException exception)
                {
                    this.ErrorHandler.Error(string.Format("{0}: Could not write log entry to {1}: {2}", this.GetType().AssemblyQualifiedName, this.tableName, exception.Message));
                }

            };
            doWriteToLog.BeginInvoke(null, null);
        }
    }
}
