using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.Models
{
    public class LogModel
    {
        public string RowKey { get; set;}
      public string RoleInstance{ get; set;}
      public string DeploymentId{ get; set;}
      public DateTime Timestamp{ get; set;}
      public string Message{ get; set;}
      public string Level{ get; set;}
      public string LoggerName{ get; set;}
      public string Domain{ get; set;}
      public string ThreadName{ get; set;}
      public string Identity{ get; set;}
    }
}