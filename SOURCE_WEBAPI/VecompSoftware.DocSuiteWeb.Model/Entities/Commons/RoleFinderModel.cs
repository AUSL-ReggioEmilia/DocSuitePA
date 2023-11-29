using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class RoleFinderModel
    {
        #region [ Constructor ]

        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public Guid? UniqueId { get; set; }
        public short? ParentId { get; set; }
        public string ServiceCode { get; set; }
        public Guid? IdTenantAOO { get; set; }
        public int? Environment { get; set; }
        public bool? LoadOnlyRoot { get; set; }
        public bool? LoadOnlyMy { get; set; }
        public bool? LoadAlsoParent { get; set; }
        public RoleTypology? RoleTypology { get; set; }
        public short? IdCategory { get; set; }
        public Guid? IdDossierFolder { get; set; }
        #endregion
    }
}
