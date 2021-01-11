namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
{
    public class FloatParameter : BaseDocumentGeneratorParameter
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public FloatParameter(string name, float value)
            : base(name)
        {
            Value = value;
        }
        #endregion

        #region [ Properties ]
        public float Value { get; set; }
        #endregion
    }
}
