using System;

namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
{
    public class DateTimeParameter : BaseDocumentGeneratorParameter
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public DateTimeParameter(string name, DateTime value)
            : base(name)
        {
            Value = value;
        }
        #endregion

        #region [ Properties ]
        public DateTime Value { get; set; }
        #endregion
    }
}
