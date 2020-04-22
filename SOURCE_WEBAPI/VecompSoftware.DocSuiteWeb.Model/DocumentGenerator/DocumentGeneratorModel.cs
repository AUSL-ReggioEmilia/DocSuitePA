using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;

namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator
{
    public class DocumentGeneratorModel : IDocumentGeneratorModel
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public DocumentGeneratorModel()
        {
            DocumentGeneratorParameters = new List<IDocumentGeneratorParameter>();
        }
        #endregion

        #region [ Properties ]
        public ICollection<IDocumentGeneratorParameter> DocumentGeneratorParameters { get; set; }
        #endregion
    }
}
