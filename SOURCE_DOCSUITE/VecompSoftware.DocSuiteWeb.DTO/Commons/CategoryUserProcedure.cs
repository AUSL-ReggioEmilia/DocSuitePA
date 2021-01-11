using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    [Serializable]
    public class CategoryUserProcedure
    {
        public CategoryUserProcedure()
        {

        }

        public string RoleName { get; set; }

        public string ProcedureUserName { get; set; }
    }
}
