using System;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.DocumentUnits
{
    public class DocumentUnitDao : BaseNHibernateDao<DocumentUnit>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DocumentUnitDao()
            : base()
        {
        }

        public DocumentUnitDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]
        public override void Delete(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }

        public override void DeleteWithoutTransaction(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }

        public override void Save(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }

        public override void SaveWithoutTransaction(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }

        public override void Update(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateNoLastChange(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateOnly(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateOnlyWithoutTransaction(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateWithoutTransaction(ref DocumentUnit entity)
        {
            throw new NotImplementedException();
        }
        #endregion [ Methods ]
    }
}
