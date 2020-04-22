using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols
{
    public class ProtocolUserModel : DomainModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        public ProtocolUserModel(Guid id) : base(id)
        {
        }
        #endregion

        #region [ Properties ]

        public string Account { get; set; }

        public ProtocolUserType Type { get; set; }

        #endregion

    }
}
