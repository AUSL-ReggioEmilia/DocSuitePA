using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolContactModel
    {
        #region [ Constructor ]
        public ProtocolContactModel()
        {

        }
        #endregion

        #region [ Properties ]
        public ComunicationType ComunicationType { get; set; }
        public ProtocolContactType ContactType { get; set; }
        public int IdContact { get; set; }
        public string Description { get; set; }

        #endregion

    }
}