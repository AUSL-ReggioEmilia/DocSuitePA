namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
{
    public class CharParameter : BaseDocumentGeneratorParameter
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public CharParameter(string name, char value)
            : base(name)
        {
            Value = value;
        }
        #endregion

        #region [ Properties ]
        public char Value { get; set; }
        #endregion
    }
}
