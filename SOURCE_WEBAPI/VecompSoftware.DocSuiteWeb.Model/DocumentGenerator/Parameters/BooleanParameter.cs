namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
{
    public class BooleanParameter : BaseDocumentGeneratorParameter
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public BooleanParameter(string name, bool value)
            : base(name)
        {
            Value = value;
        }
        #endregion

        #region [ Properties ]
        public bool Value { get; set; }
        #endregion
    }
}
