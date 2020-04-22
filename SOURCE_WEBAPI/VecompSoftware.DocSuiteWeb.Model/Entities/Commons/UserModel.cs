
using System;
using VecompSoftware.DocSuiteWeb.Model.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class UserModel
    {
        #region [ Constructors ]

        public UserModel()
        {
        }

        #endregion

        #region [ Properties ]

        public Guid? UniqueId { get; set; }

        public string Account { get; set; }

        public AuthorizationRoleType AuthorizationType { get; set; }


        #endregion
    }
}
