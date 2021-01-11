namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
{
    public class StringParameter : BaseDocumentGeneratorParameter
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public StringParameter(string name, string value)
            : base(name)
        {
            Value = value;
        }
        #endregion

        #region [ Properties ]
        public string Value { get; set; }
        public bool HasHtmlValue { get; set; }

        #endregion
    }
}
