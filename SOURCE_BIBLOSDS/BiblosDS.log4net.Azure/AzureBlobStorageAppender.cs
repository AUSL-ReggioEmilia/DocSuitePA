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
using System.IO;
using System.Diagnostics;

namespace BiblosDS.Log4Net.Azure
{
    public class AzureBlobStorageAppender : RollingFileAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            base.Append(loggingEvent);
        }
        public override void ActivateOptions()
        {
            base.ActivateOptions();
        }
        public override string File
        {
            set
            {
                base.File = RoleEnvironment.GetLocalResource("Log4Net").RootPath + @"\"
                    + new FileInfo(value).Name + "_"
                    + Process.GetCurrentProcess().ProcessName;
            }
        }
    }
}
