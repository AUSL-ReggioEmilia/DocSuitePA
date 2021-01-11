using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public enum WorkflowValidationRulesType : short
    {
        [Description("Esiste cartella di fascicolo")]
        IsExist,
        [Description("La cartella contiene almeno un file/inserto")]
        HasFile,
        [Description("La cartella contiene almeno una unità documentale")]
        HasDocumentUnit,
        [Description("La cartella contiene almeno un file/inserto firmato digitalmente")]
        HasSignedFile
    }
}
