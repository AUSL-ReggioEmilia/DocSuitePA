using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.PECMails
{
    public class PECMailBoxTableValuedModelMapper : BaseModelMapper<PECMailBoxTableValuedModel, PECMailBoxModel>, IPECMailBoxTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public PECMailBoxTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override PECMailBoxModel Map(PECMailBoxTableValuedModel model, PECMailBoxModel modelTransformed)
        {
            modelTransformed.MailBoxRecipient = model.MailBoxRecipient;
            modelTransformed.IncomingServer = model.IncomingServer;
            modelTransformed.IncomingServerProtocol = model.IncomingServerProtocol;
            modelTransformed.IncomingServerPort = model.IncomingServerPort;
            modelTransformed.IncomingServerUseSsl = model.IncomingServerUseSsl;
            modelTransformed.OutgoingServer = model.OutgoingServer;
            modelTransformed.OutgoingServerPort = model.OutgoingServerPort;
            modelTransformed.OutgoingServerUseSsl = model.OutgoingServerUseSsl;
            modelTransformed.Username = model.Username;
            modelTransformed.Password = model.Password;
            modelTransformed.Managed = model.Managed;
            modelTransformed.Unmanaged = model.Unmanaged;
            modelTransformed.IsDestinationEnabled = model.IsDestinationEnabled;
            modelTransformed.IsForInterop = model.IsForInterop;
            modelTransformed.IdConfiguration = model.IdConfiguration;
            modelTransformed.DeleteMailFromServer = model.DeleteMailFromServer;
            modelTransformed.ReceiveDaysCap = model.ReceiveDaysCap;
            modelTransformed.RedirectAnomaliesSMTP = model.RedirectAnomaliesSMTP;
            modelTransformed.RedirectAnomaliesUsername = model.RedirectAnomaliesUsername;
            modelTransformed.RedirectAnomaliesPassword = model.RedirectStoragePassword;
            modelTransformed.RedirectAnomaliesRecipient = model.RedirectAnomaliesRecipient;
            modelTransformed.RedirectStoragePassword = model.RedirectStoragePassword;
            modelTransformed.RedirectStorageUsername = model.RedirectStorageUsername;
            modelTransformed.RedirectStorageSMTP = model.RedirectStorageSMTP;
            modelTransformed.RedirectStorageRecipient = model.RedirectStorageRecipient;
            modelTransformed.IsHandleEnabled = model.IsHandleEnabled;
            modelTransformed.IsProtocolBox = model.IsProtocolBox;
            modelTransformed.IsProtocolBoxExplicit = model.IsProtocolBoxExplicit;
            modelTransformed.IdJeepServiceIncomingHost = model.IdJeepServiceIncomingHost;
            modelTransformed.IdJeepServiceOutgoingHost = model.IdJeepServiceOutgoingHost;
            modelTransformed.RulesetDefinition = model.RulesetDefinition;

            return modelTransformed;
        }

        public override ICollection<PECMailBoxModel> MapCollection(ICollection<PECMailBoxTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<PECMailBoxModel>();
            }
            List<PECMailBoxModel> modelsTransformed = new List<PECMailBoxModel>();
            PECMailBoxModel modelTransformed = null;
            foreach (IGrouping<int, PECMailBoxTableValuedModel> pecMailBoxLookup in model.ToLookup(x => x.UniqueId))
            {
                modelTransformed = Map(pecMailBoxLookup.First(), new PECMailBoxModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
