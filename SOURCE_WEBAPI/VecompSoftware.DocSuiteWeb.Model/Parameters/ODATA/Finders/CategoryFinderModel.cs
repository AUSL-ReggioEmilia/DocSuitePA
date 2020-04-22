using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
{
    public class CategoryFinderModel
    {
        #region [ Constructor ]

        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public FascicleType? FascicleType { get; set; }
        public bool? HasFascicleInsertRights { get; set; }
        public string Manager { get; set; }
        public string Secretary { get; set; }
        public short? IdRole { get; set; }
        public bool? LoadRoot { get; set; }
        public short? ParentId { get; set; }
        public bool? ParentAllDescendants { get; set; }
        public string FullCode { get; set; }
        public bool? FascicleFilterEnabled { get; set; }
        public short? IdContainer { get; set; }
        #endregion
    }
}
