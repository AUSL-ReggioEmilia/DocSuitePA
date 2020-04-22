using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class RoleFinderModel
    {
        #region [ Constructor ]

        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public short? ParentId { get; set; }
        public string ServiceCode { get; set; }
        public Guid? TenantId { get; set; }
        public int? Environment { get; set; }
        public bool? LoadOnlyRoot { get; set; }
        public bool? LoadOnlyMy { get; set; }
        public bool? LoadAlsoParent { get; set; }
        #endregion
    }
}
