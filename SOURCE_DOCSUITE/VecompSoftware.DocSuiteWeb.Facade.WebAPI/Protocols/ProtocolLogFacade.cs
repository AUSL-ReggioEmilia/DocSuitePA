using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Protocols
{
    public class ProtocolLogFacade : FacadeWebAPIBase<ProtocolLog, ProtocolLogDao>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public ProtocolLogFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<ProtocolLog, ProtocolLogDao>(s)).ToList(), currentTenant)
        {
        }
        #endregion

        #region [ Properties ]

        #endregion

        public void InsertViewDocumentLog(Guid idProtocol, string message)
        {
            ProtocolLog log = new ProtocolLog()
            {
                Entity = new Protocol(idProtocol),
                LogType = ProtocolLogEvent.PD.ToString(),
                LogDescription = message,
                RegistrationUser = Data.DocSuiteContext.Current.User.FullUserName,
                SystemComputer = Data.DocSuiteContext.Current.UserComputer,
                RegistrationDate = DateTimeOffset.UtcNow
            };
            Save(log, "ViewProtocolDocument");
        }
    }
}
