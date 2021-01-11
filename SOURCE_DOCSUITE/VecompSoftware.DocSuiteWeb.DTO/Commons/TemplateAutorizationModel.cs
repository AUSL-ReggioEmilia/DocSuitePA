using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    public class TemplateAuthorizationModel
    {
        public string TemplateName { get; set; }
        public bool ManagerDeletable { get; set; }
        public bool AddSignersOnTop { get; set; }
        public IList<String> LockedSigners { get; set; }
    }
}
