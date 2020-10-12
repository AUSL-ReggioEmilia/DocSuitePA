using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.PosteWeb
{
    public class PosteOnLineRequest : DSWBaseEntity
    {
        #region [ Constructor ]
        public PosteOnLineRequest() : base(Guid.NewGuid()) { }

        public PosteOnLineRequest(Guid uniqueId)
            : base(uniqueId)
        {
        }

        #endregion

        #region [ Properties ]

        public string RequestId { get; set; }

        public string GuidPoste { get; set; }

        public string IdOrdine { get; set; }

        public POLRequestStatusEnum Status { get; set; }

        public string StatusDescription { get; set; }

        public string ErrorMessage { get; set; }

        public double TotalCost { get; set; }

        public string ExtendedProperties { get; set; }

        #endregion

        #region[ Navigation Properties ]

        public virtual DocumentUnit DocumentUnit { get; set; }

        #endregion
    }
}
