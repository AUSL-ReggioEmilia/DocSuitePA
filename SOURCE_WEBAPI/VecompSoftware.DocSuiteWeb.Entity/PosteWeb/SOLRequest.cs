using System;

namespace VecompSoftware.DocSuiteWeb.Entity.PosteWeb
{

    public class SOLRequest : LOLRequest
    {
        #region [ Constructor ]

        public SOLRequest() : base(Guid.NewGuid()) { }
        
        public SOLRequest(Guid uniqueId) : base(uniqueId)
        {
        }

        #endregion

        #region [ Properties ]
        #endregion

        #region[ Navigation Properties ]
        #endregion
    }
}
