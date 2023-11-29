using System;
using System.Collections.Generic;
using System.Drawing;

namespace VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
{
    public class PreparePECMailDocumentsWorkflow
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public Guid UniqueId
        {
            get; set;
        }
        public Guid PECMailId
        {
            get; set;
        }

        public List<DocumentInfoModel> DocumentInfos
        {
            get; set;
        }
        #endregion

        #region [ Constructor ]
        public PreparePECMailDocumentsWorkflow()
        {
            DocumentInfos = new List<DocumentInfoModel>();
        }
        #endregion
    }

}
