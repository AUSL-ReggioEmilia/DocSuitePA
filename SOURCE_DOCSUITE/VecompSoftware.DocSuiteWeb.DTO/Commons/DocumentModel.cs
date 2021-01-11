using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    public class DocumentModel
    {
        #region [ Fields ]

        public string Name { get; set; }

        public Guid IdDocument { get; set; }

        public Guid IdChain { get; set; }

        public bool IsMainDocument { get; set; }

        public bool IsSigned { get; set; }

        #endregion
    }
}
