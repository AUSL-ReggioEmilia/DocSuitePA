using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityCQRSDocumentUnitDto
    {
        #region[Properties]
        public short Year { get; set; }

        public int Number { get; set; }

        public int Environment { get; set; }

        public string DocumentUnitName { get; set; }

        public string Title { get; set; }

        public UDSEntityCategoryDto Category { get; set; }
        public UDSEntityContainerDto Container { get; set; }
        #endregion

        #region[Method]

        #endregion
    }
}
