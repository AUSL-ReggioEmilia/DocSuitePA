namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
{
    public class IntParameter : BaseDocumentGeneratorParameter
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public IntParameter(string name, int value)
            : base(name)
        {
            Value = value;
        }
        #endregion

        #region [ Properties ]
        public int Value { get; set; }
        #endregion
    }
}
