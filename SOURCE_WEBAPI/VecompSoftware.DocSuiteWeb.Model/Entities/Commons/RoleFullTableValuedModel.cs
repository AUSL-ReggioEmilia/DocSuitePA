using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class RoleFullTableValuedModel
    {
        #region [ Constructor ]
        public RoleFullTableValuedModel()
        {

        }
        #endregion

        #region [ Properties ]

        public short? IdRole { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string FullIncrementalPath { get; set; }
        public Guid? UniqueId { get; set; }
        public string ServiceCode { get; set; }
        public RoleTypology RoleTypology { get; set; }
        public bool Collapsed { get; set; }
        public string EMailAddress { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public Guid IdTenantAOO { get; set; }
        public bool IsRealResult { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #region [ Parent ]
        public short? RoleParent_IdRole { get; set; }
        #endregion

        #endregion

        #region [ Methods ]

        #endregion
    }
}
