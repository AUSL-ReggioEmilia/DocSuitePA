using System;

namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
{
    public class GuidParameter : BaseDocumentGeneratorParameter
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public GuidParameter(string name, Guid value)
            : base(name)
        {
            Value = value;
        }
        #endregion

        #region [ Properties ]
        public Guid Value { get; set; }
        #endregion
    }
}
