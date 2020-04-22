using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.UDS;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.UDS
{
    public class UDSLogFacade : FacadeWebAPIBase<UDSLog, UDSLogDao>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public UDSLogFacade(ICollection<TenantModel> model)
            : base(model.Select(s => new WebAPITenantConfiguration<UDSLog, UDSLogDao>(s)).ToList())
        {
        }
        #endregion

        #region [ Properties ]
        
        #endregion

        public void InsertLog(Guid idUDS, Guid idUDSRepository, int environment, string message, UDSLogType type)
        {
            UDSLog log = new UDSLog()
            {
                IdUDS = idUDS,
                Entity = new UDSRepository(idUDSRepository),
                Environment = environment,
                LogType = type,
                LogDescription = message,
                RegistrationUser = DocSuiteContext.Current.User.FullUserName,
                SystemComputer = DocSuiteContext.Current.UserComputer,
                RegistrationDate = DateTimeOffset.UtcNow
            };
            Save(log);
        }
    }
}
