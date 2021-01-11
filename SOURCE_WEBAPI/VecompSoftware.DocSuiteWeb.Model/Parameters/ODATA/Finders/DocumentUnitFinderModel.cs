using System;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
{
    public class DocumentUnitFinderModel
    {
        #region [ Constructor ]

        #endregion

        #region [ Properties ]
        public int? Skip { get; set; }
        public int? Top { get; set; }
        public Guid? IdFascicle { get; set; }
        public int? Year { get; set; }
        public string Number { get; set; }
        public string DocumentUnitName { get; set; }
        public int? IdCategory { get; set; }
        public int? IdContainer { get; set; }
        public string Subject { get; set; }
        public bool? IncludeChildClassification { get; set; }
        public Guid? IdTenantAOO { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IncludeThreshold { get; set; }
        public string Threshold { get; set; }
        #endregion
    }
}
