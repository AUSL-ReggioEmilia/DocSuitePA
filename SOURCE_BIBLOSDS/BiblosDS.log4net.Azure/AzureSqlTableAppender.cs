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
using System.Configuration;
using System.Data.SqlClient;

namespace BiblosDS.Log4Net.Azure
{
    public class AzureSqlTableAppender : AppenderSkeleton
    {                
        private string tableName = "Log4Net";
        string connectionString = "";

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            try
            {
                connectionString = RoleEnvironment.GetConfigurationSettingValue("SqlLogConnectionString");
            }
            catch (Exception)
            {
                connectionString = ConfigurationManager.AppSettings["SqlLogConnectionString"];
            }
            try
            {
                this.tableName = (RoleEnvironment.CurrentRoleInstance.Role.Name.IndexOf('.') >= 0) ? RoleEnvironment.CurrentRoleInstance.Role.Name.Substring(RoleEnvironment.CurrentRoleInstance.Role.Name.IndexOf('.') + 1) : RoleEnvironment.CurrentRoleInstance.Role.Name;
                this.tableName = this.tableName.Replace(".", "");                
            }
            catch (Exception)
            {
                               
            }

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                string sql = "Select count(*) from " + this.tableName;
                try
                {
                    cnn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.ExecuteScalar();
                    }
                }
                catch (Exception)
                {
                    sql = string.Format(@"create table {0}
                                        (
                                          PartitionKey varchar(255),
                                          RowKey varchar(255),
	                                        RoleInstance varchar(400),
                                            DeploymentId varchar(400),                    
                                            [Timestamp] datetime,
                                            [Message] text,
                                            [Level] varchar(255),
	                                        LoggerName varchar(255),
                                            Domain varchar(255),
                                            ThreadName varchar(255),
	                                        [Identity] varchar(255)
                                        CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
                                        (
	                                        RowKey desc
                                        )
                                        )", this.tableName);
                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    } 
                }             
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            Action doWriteToLog = () =>
            {
                try
                {
                    string roleInstance = "ND";
                    string deploymentId = "ND";
                    try
                    {
                        roleInstance = RoleEnvironment.CurrentRoleInstance.Id;
                        deploymentId = RoleEnvironment.DeploymentId;
                    }
                    catch { }
                    LogEntry entry = new LogEntry
                    {
                        RoleInstance = roleInstance,
                        DeploymentId = deploymentId
                    };
                    entry.Timestamp = loggingEvent.TimeStamp;
                    entry.Message = loggingEvent.RenderedMessage;
                    entry.Level = loggingEvent.Level.Name;
                    entry.LoggerName = loggingEvent.LoggerName;
                    entry.Domain = loggingEvent.Domain;
                    entry.ThreadName = loggingEvent.ThreadName;
                    entry.Identity = loggingEvent.Identity;

                    using (SqlConnection cnn = new SqlConnection(connectionString))
                    {
                        string sql = string.Format(@"INSERT INTO {0}
           ([PartitionKey]
           ,[RowKey]
           ,[RoleInstance]
           ,[DeploymentId]
           ,[Timestamp]
           ,[Message]
           ,[Level]
           ,[LoggerName]
           ,[Domain]
           ,[ThreadName]
           ,[Identity])
     VALUES
           (@PartitionKey
           ,@RowKey
           ,@RoleInstance
           ,@DeploymentId
           ,@Timestamp
           ,@Message
           ,@Level
           ,@LoggerName
           ,@Domain
           ,@ThreadName
           ,@Identity)", this.tableName);
                        cnn.Open();
                        using (SqlCommand cmd = new SqlCommand(sql, cnn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@PartitionKey", entry.PartitionKey));
                            cmd.Parameters.Add(new SqlParameter("@RowKey", entry.RowKey));
                            cmd.Parameters.Add(new SqlParameter("@RoleInstance", entry.RoleInstance));
                            cmd.Parameters.Add(new SqlParameter("@DeploymentId", entry.DeploymentId));
                            cmd.Parameters.Add(new SqlParameter("@Timestamp", entry.Timestamp));
                            cmd.Parameters.Add(new SqlParameter("@Message", entry.Message));
                            cmd.Parameters.Add(new SqlParameter("@Level", entry.Level));
                            cmd.Parameters.Add(new SqlParameter("@LoggerName", entry.LoggerName));
                            cmd.Parameters.Add(new SqlParameter("@Domain", entry.Domain));
                            cmd.Parameters.Add(new SqlParameter("@ThreadName", entry.ThreadName));
                            cmd.Parameters.Add(new SqlParameter("@Identity", entry.Identity));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.ErrorHandler.Error(string.Format("{0}: Could not write log entry to {1}: {2}", this.GetType().AssemblyQualifiedName, this.tableName, exception.Message));
                }
            };
            doWriteToLog.BeginInvoke(null, null);
        }
    }
}
