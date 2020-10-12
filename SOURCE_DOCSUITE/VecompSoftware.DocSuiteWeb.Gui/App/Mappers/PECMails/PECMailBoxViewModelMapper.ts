import PECMailBoxViewModel = require('App/ViewModels/PECMails/PECMailBoxViewModel');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');
import IMapper = require('App/Mappers/IMapper');
import BaseMapper = require('App/Mappers/BaseMapper');

class PECMailBoxViewModelMapper extends BaseMapper<PECMailBoxModel> implements IMapper<PECMailBoxViewModel>{
  constructor() {
    super();
  }
  public Map(source: any): PECMailBoxViewModel {
    let toMap: PECMailBoxViewModel = <PECMailBoxViewModel>{};

    if (!source) {
      return null;
    }

    toMap.EntityShortId = source.EntityShortId;
    toMap.MailBoxRecipient = source.MailBoxRecipient;
    toMap.IncomingServer = source.IncomingServer;
    toMap.OutgoingServer = source.OutgoingServer;
    toMap.Username = source.Username;
    toMap.Password = source.Password;
    toMap.RedirectAnomaliesSMTP = source.RedirectAnomaliesSMTP;
    toMap.RedirectAnomaliesUsername = source.RedirectAnomaliesUsername;
    toMap.RedirectAnomaliesPassword = source.RedirectAnomaliesPassword;
    toMap.RedirectAnomaliesRecipient = source.RedirectAnomaliesRecipient;
    toMap.RedirectStorageSMTP = source.RedirectStorageSMTP;
    toMap.RedirectStorageUsername = source.RedirectStorageUsername;
    toMap.RedirectStoragePassword = source.RedirectStoragePassword;
    toMap.RedirectStorageRecipient = source.RedirectStorageRecipient;
    toMap.IsForInterop = source.IsForInterop;
    toMap.IsDestinationEnabled = source.IsDestinationEnabled;
    toMap.DeleteMailFromServer = source.DeleteMailFromServer;
    toMap.ReceiveDaysCap = source.ReceiveDaysCap;
    toMap.Managed = source.Managed;
    toMap.Unmanaged = source.Unmanaged;
    toMap.IdConfiguration = source.IdConfiguration;
    toMap.IncomingServerProtocol = source.IncomingServerProtocol;
    toMap.IncomingServerPort = source.IncomingServerPort;
    toMap.IncomingServerUseSsl = source.IncomingServerUseSsl;
    toMap.OutgoingServerPort = source.OutgoingServerPort;
    toMap.OutgoingServerUseSsl = source.OutgoingServerUseSsl;
    toMap.IsHandleEnabled = source.IsHandleEnabled;
    toMap.IsProtocolBox = source.IsProtocolBox;
    toMap.IsProtocolBoxExplicit = source.IsProtocolBoxExplicit;
    toMap.IdJeepServiceIncomingHost = source.IdJeepServiceIncomingHost;
    toMap.IdJeepServiceOutgoingHost = source.IdJeepServiceOutgoingHost;
    toMap.RulesetDefinition = source.RulesetDefinition;
    toMap.EntityId = source.EntityId;
    toMap.UniqueId = source.UniqueId;
    toMap.RegistrationUser = source.RegistrationUser;
    toMap.RegistrationDate = source.RegistrationDate;
    toMap.LastChangedUser = source.LastChangedUser;
    toMap.LastChangedDate = source.LastChangedDate;
    toMap.LoginError = source.LoginError;
    toMap.Location = source.Location;
    toMap.InvoiceType = source.InvoiceType;

    return toMap;
  }
}

export = PECMailBoxViewModelMapper;