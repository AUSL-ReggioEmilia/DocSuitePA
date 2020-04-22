using System;
using System.Collections.Generic;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    [Serializable]
    public class MergeDocumentResult
    {
        #region [ Constructor ]
        public MergeDocumentResult()
        {
            Errors = new List<string>();
        }
        #endregion

        #region [ Properties ]
        public DocumentInfo MergedDocument { get; set; }

        public ICollection<string> Errors { get; set; }

        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }
        #endregion
    }
}
