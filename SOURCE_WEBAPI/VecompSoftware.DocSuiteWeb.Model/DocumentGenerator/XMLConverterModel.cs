using System;

namespace VecompSoftware.DocSuiteWeb.Model.DocumentGenerator
{
    public class XMLConverterModel
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public XMLConverterModel()
        {
            UniqueId = Guid.NewGuid();
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public XMLModelKind ModelKind { get; set; }
        public object Model { get; set; }
        public string Version { get; set; }
        public string Xsl { get; set; }
        #endregion
    }
}
