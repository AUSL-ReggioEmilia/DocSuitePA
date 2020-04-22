using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses
{
    public class InvoicePreviewModel
    {
        #region [ Constructor ]
        public InvoicePreviewModel()
        {
            Selectable = true;
        }
        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public string InvoiceFilename { get; set; }
        public string InvoiceMetadataFilename { get; set; }
        public string Description { get; set; }
        public string Result { get; set; }
        public bool Selectable { get; set; }

        #endregion

    }
}
