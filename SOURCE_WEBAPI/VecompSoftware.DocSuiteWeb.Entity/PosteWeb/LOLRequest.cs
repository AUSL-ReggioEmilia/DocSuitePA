using System;

namespace VecompSoftware.DocSuiteWeb.Entity.PosteWeb
{

    public class LOLRequest : PosteOnLineRequest
    {
        #region [ Constructor ]

        public LOLRequest() : base(Guid.NewGuid())
        {
        }

        public LOLRequest(Guid uniqueId) : base(uniqueId)
        {

        }

        #endregion

        #region [ Properties ]

        public string DocumentName { get; set; }
        public string DocumentMD5 { get; set; }
        public string DocumentPosteMD5 { get; set; }
        public string DocumentPosteFileType { get; set; }

        public Guid? IdArchiveChain { get; set; }
        public Guid? IdArchiveChainPoste { get; set; }

        #endregion

        #region[ Navigation Properties ]
        #endregion
    }
}
