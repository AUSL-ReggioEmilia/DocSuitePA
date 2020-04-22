using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols
{

    public class ProtocolContactModel : ContactModel<ProtocolContactModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        public ProtocolContactModel(Guid id, string name) : base(id, name)
        {
        }
        #endregion

        #region [ Properties ]
        public ComunicationType ComunicationType { get; set; } //todo: passare in odata il numero

        public bool IsCC { get; set; }

        public bool IsSelected { get; set; }

        #endregion

    }
}
