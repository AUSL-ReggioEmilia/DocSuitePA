using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons
{
    public class ContactModel<TContact> : DomainModel, IContactModel, IHierarchicalModel<TContact>
        where TContact : IContactModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        public ContactModel(Guid id, string name) : base(id)
        {
            Name = name;
            Children = new HashSet<TContact>();
        }

        #endregion

        #region [ Properties ]
        public bool? IsActive { get; set; }

        public ContactType ContactType { get; set; }

        public Guid? UniqueIdFather { get; set; }

        public ICollection<TContact> Children { get; set; }

        #endregion
    }
}
