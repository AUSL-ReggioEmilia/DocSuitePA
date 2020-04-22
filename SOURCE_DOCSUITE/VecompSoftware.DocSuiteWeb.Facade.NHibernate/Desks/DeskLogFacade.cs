using System;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
{
    public class DeskLogFacade : BaseProtocolFacade<DeskLog, Guid, DeskLogDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskLogFacade(string userName) : base()
        {
            _userName = userName;
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public void InsertLog(DeskLogType logType, string logDescription, Desk desk, SeverityLog severity)
        {
            DeskLog log = new DeskLog
            {
                Desk = desk,
                LogDate = DateTime.Now,
                LogDescription = logDescription,
                LogType = logType,
                Severity = severity,
                SystemComputer = DocSuiteContext.Current.UserComputer,
                SystemUser = _userName
            };

            Save(ref log);
        }
        #endregion [ Methods ]
    }
}
