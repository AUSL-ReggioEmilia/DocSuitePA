using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles
{

    public class FascicleContactModel : ContactModel<FascicleContactModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        public FascicleContactModel(Guid id, string name) : base(id, name)
        {
        }
        #endregion

        #region [ Properties ]

        public bool IsSelected { get; set; }

        #endregion

    }
}
