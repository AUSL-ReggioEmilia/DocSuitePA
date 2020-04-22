using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;

namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator
{
    public abstract class BaseDocumentGeneratorParameter : IDocumentGeneratorParameter
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public BaseDocumentGeneratorParameter(string name)
        {
            Name = name;
            LookingTag = string.Concat("${", name, "}");
        }
        #endregion

        #region [ Properties ]
        public string Name { get; }
        public string LookingTag { get; }
        #endregion
    }
}
