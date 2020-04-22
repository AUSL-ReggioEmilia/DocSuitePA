using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityCategoryDto
    {
        #region[Properties]
        public int EntityShortId { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }

        public string FullCode { get; set; }

        public string FullSearchComputed { get; set; }

        public string FullIncrementalPath { get; set; }
        #endregion

        #region[Method]
        public string GetCodeDotted()
        {
            List<string> listOfNewCodes = new List<string>();
            IEnumerable<string> listOfCode = this.FullCode.Split('|');
            foreach (string elem in listOfCode)
            {
                listOfNewCodes.Add(elem.TrimStart('0'));
            }
            return string.Join(".", listOfNewCodes);
        }
        #endregion
    }
}
