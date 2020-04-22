using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols
{
    public class ProtocolSectorModel : SectorModel<ProtocolSectorModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        public ProtocolSectorModel(Guid id, string name) : base(id, name)
        {
        }
        #endregion

        #region [ Properties ]

        public bool IsAuthorized { get; set; }

        #endregion

    }
}
