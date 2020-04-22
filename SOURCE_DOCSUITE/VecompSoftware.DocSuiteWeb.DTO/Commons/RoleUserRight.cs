using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    [Serializable]
    public class RoleUserRight
    {
        public RoleUserRight()
        {
        }
        public string Name { get; set; }
        public string GroupName { get; set; }

        public bool IsProtocolEnabled { get; set; }
        public bool AreProtocolPECEnabled { get; set; }
        public bool IsDocumentEnabled { get; set; }
        public bool IsDocumentWorkflowEnabled { get; set; }
        public bool IsDocumentManagerEnabled { get; set; }
        public bool IsResolutionEnabled { get; set; }
        public bool IsDocumentSeriesEnabled { get; set; }
    }
}
