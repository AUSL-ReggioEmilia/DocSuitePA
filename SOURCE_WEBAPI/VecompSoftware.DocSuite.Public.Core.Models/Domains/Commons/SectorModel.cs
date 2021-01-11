using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons
{
    public class SectorModel<TSector> : DomainModel, ISectorModel, IHierarchicalModel<TSector>
        where TSector : ISectorModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        public SectorModel(Guid id, string name) : base(id)
        {
            Name = name;
            Children = new HashSet<TSector>();
        }

        #endregion

        #region [ Properties ]
        public string ArchiveSection { get; set; }

        public bool? IsActive { get; set; }

        public Guid? UniqueIdFather { get; set; }

        public ICollection<TSector> Children { get; set; }

        #endregion
    }
}
