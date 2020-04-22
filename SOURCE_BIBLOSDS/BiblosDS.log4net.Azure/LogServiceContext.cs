﻿/*
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
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace BiblosDS.Log4Net.Azure
{
    internal class LogServiceContext : TableServiceContext
    {
        public LogServiceContext(string baseAddress, StorageCredentials credentials) : base(baseAddress, credentials) { }
        internal void Log(LogEntry logEntry)
        {
            AddObject("LogEntries", logEntry);
            SaveChanges();
        }

        public IQueryable<LogEntry> LogEntries
        {
            get
            {
                return CreateQuery<LogEntry>("LogEntries");
            }
        }
    }
}
