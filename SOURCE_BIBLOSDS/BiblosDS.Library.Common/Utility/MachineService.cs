using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Principal;
using System.Security.Permissions;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;
using BiblosDS.Library.Common.Utility;

namespace BiblosDS.Library.Common.Objects.UtilityService
{    
    public class MachineService
    {
        public static string GetServerName()
        {
            if (AzureService.IsAvailable)
                return string.Empty;
            if (ConfigurationManager.AppSettings["ServerName"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"].ToString()))
                return ConfigurationManager.AppSettings["ServerName"].ToString();
            else
                return System.Environment.MachineName;
        }
        
    }
}
