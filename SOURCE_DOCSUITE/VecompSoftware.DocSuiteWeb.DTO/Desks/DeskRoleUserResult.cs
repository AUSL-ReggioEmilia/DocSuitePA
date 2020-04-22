using System;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.DTO.Desks
{
    [Serializable]
    public class DeskRoleUserResult
    {
        #region [ Constructor ]

        public DeskRoleUserResult() { }

        public DeskRoleUserResult(Contact contact)
        {
            this.UserName = contact.Description;
            this.Description = contact.Description;
        }

        public DeskRoleUserResult(Contact contact, string key)
        {
            this.SerializeKey = key;
            this.UserName = contact.Code;
            this.Description = contact.Description;
        }

        #endregion

        #region [ Properties ]

        public string SerializeKey { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

        public bool IsSavedOnDesk { get; set; }

        public bool IsChanged { get; set; }

        public DeskPermissionType? PermissionType { get; set; }

        #endregion
    }
}
