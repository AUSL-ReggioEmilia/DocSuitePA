using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolUserModel
    {
        #region [ Constructor ]
        public ProtocolUserModel()
        {

        }
        #endregion

        #region [ Properties ]
        public Guid? UniqueId { get; set; }

        public string Account { get; set; }

        public ProtocolUserType Type { get; set; }
        #endregion

    }
}
