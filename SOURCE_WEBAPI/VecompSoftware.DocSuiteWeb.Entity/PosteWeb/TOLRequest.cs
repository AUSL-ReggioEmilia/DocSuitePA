using System;

namespace VecompSoftware.DocSuiteWeb.Entity.PosteWeb
{
    public class TOLRequest : PosteOnLineRequest
    {
        #region [ Constructor ]

        public TOLRequest() : base(Guid.NewGuid()) { }

        public TOLRequest(Guid uniqueId) : base(uniqueId)
        {
        }

        #endregion

        #region [ Properties ]

        public string Testo { get; set; }

        #endregion

        #region [ Navigation Properties ]
        #endregion
    }
}
