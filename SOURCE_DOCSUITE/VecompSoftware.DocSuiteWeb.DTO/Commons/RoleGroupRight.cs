using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    [Serializable]
    public class RoleGroupRight
    {
        public RoleGroupRight()
        {
            this.Users = new List<RoleUserRight>();
        }

        public RoleGroupRight(int idRole)
           : this()
        {
            this.IdRole = idRole;
        }

        public int IdRole { get; set; }
        public string GroupName { get; set; }
        public IList<RoleUserRight> Users { get; set; }


        public string Authorization { get; set; }
        public string Location { get; set; }



    }
}
