using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Resolutions
{
    public class ResolutionDocument
    {
        #region [ Constructor ]
        public ResolutionDocument() { }
        #endregion

        #region [ Properties ]
        public string DocumentName { get; set; }
        public string DocumentGroup { get; set; }
        public string BiblosSerializeKey { get; set; }
        public Guid? IdBiblosDocument { get; set; }
        public int? IdResolution { get; set; }
        #endregion
    }
}
