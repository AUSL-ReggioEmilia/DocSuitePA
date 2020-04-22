using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons
{
    public class ContainerModel : DomainModel, IActiveModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        public ContainerModel(Guid id, string name) : base(id)
        {
            Name = name;
        }

        #endregion

        #region [ Properties ]
        public bool? IsActive { get; set; }

        #endregion
    }
}
