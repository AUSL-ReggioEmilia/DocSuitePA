using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
{
    public class ContactListFacade : BaseProtocolFacade<ContactList, Guid, ContactListDao>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public ContactListFacade()
            : base()
        {
            
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public IList<ContactList> GetByName(string name)
        {
            return _dao.GetByName(name);
        } 

        #endregion [ Methods ]

    }
}
