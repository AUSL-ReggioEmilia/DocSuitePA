using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
{
    public class FascicleLogFacade : FacadeWebAPIBase<FascicleLog, FascicleLogDao>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public FascicleLogFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<FascicleLog, FascicleLogDao>(s)).ToList(), currentTenant)
        {
        }
        #endregion

        #region [ Properties ]

        #endregion

        public void InsertFascicleDocumentLog(Guid idFascicle, FascicleLogType logType, string message)
        {
            FascicleLog log = new FascicleLog()
            {
                Entity = new Fascicle(idFascicle),
                LogType = logType,
                LogDescription = message,
                RegistrationUser = Data.DocSuiteContext.Current.User.FullUserName,
                SystemComputer = Data.DocSuiteContext.Current.UserComputer,
                RegistrationDate = DateTimeOffset.UtcNow
            };
            Save(log, null);
        }
    }
}
