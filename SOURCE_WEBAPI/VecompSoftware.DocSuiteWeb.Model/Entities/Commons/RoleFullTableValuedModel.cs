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
        public byte? IsActive { get; set; }
        public string FullIncrementalPath { get; set; }
        public Guid? TenantId { get; set; }
        public short? IdRoleTenant { get; set; }
        public Guid? UniqueId { get; set; }
        public string ServiceCode { get; set; }
        public DateTime? ActiveFrom { get; set; }

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
