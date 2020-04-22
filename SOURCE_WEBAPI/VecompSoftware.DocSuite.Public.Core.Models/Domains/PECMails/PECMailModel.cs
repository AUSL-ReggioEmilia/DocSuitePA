using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.PECMails
{
    public class PECMailModel : DomainModel
    {
        #region [ Contructors ]
        public PECMailModel(Guid id)
            : base(id)
        {
            Receipts = new HashSet<PECMailReceiptModel>();
        }
        #endregion

        #region [ Properties ]

        public short? Year { get; set; }

        public int? Number { get; set; }

        public string Subject { get; set; }

        public PECDirection Direction { get; set; }

        public string Senders { get; set; }

        public string Recipients { get; set; }

        public PECActiveType IsActive { get; set; }

        public DateTime? Date { get; set; }

        public string Body { get; set; }

        public string Step { get; set; }

        public PECType? PECType { get; set; }

        public ICollection<PECMailReceiptModel> Receipts { get; set; }

        #endregion
    }
}
