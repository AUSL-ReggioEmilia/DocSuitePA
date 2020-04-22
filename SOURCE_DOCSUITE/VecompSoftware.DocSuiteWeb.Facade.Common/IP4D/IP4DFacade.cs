using System;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.ExternalViewer;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Commands.Models.ExternalViewer;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.IP4D
{
    public class IP4DFacade
    {
        #region [ Fields ]
        private readonly CommandFacade<ICommandProtocolExternalViewer> _commandFacade = null;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public IP4DFacade()
        {
            _commandFacade = new CommandFacade<ICommandProtocolExternalViewer>();
        }
        #endregion

        #region [ Methods ]
        /// <summary>
        /// Invia un comando alle WebAPI per spedire un modello di tipo <typeparamref name="ExternalViewerModel"/>
        /// </summary>
        /// <param name="model"></param>
        public void SendIP4D(ExternalViewerModel model)
        {
            IdentityContext identity = new IdentityContext(DocSuiteContext.Current.User.FullUserName);
            string tenantName = DocSuiteContext.Current.CurrentTenant.TenantName;
            Guid tenantId = DocSuiteContext.Current.CurrentTenant.TenantId;            

            ICommandProtocolExternalViewer commandSend = new CommandProtocolExternalViewer(tenantName, tenantId, string.Empty, null, null, null, identity, model);
            _commandFacade.Push(commandSend);
        }
        #endregion
    }
}
