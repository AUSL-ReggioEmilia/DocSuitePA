using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Integrations.GenericProcesses;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Dematerialisation
{
    public class DematerialisationFacade
    {
        #region [ Fields ]
        private readonly CommandFacade<ICommandDematerialisationRequest> _commandFacade = null;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public DematerialisationFacade()
        {
            _commandFacade = new CommandFacade<ICommandDematerialisationRequest>();
        }
        #endregion

        #region [ Methods ]
        /// <summary>
        /// Invia un comando alle WebAPI per spedire un modello di tipo <typeparamref name="DematerialisationRequestModel"/>
        /// </summary>
        /// <param name="model"></param>
        public void SendDematerialisationRequest(DocumentManagementRequestModel model)
        {
            IdentityContext identity = new IdentityContext(DocSuiteContext.Current.User.FullUserName);
            string tenantName = DocSuiteContext.Current.CurrentTenant.TenantName;
            Guid tenantId = DocSuiteContext.Current.CurrentTenant.TenantId;

            ICommandDematerialisationRequest commandSend = new CommandDematerialisationRequest(tenantName, tenantId, identity, model);
            _commandFacade.Push(commandSend);
        }
        #endregion
    }
}
