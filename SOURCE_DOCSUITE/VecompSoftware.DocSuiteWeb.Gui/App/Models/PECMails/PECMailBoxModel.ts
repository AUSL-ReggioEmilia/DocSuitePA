import LocationViewModel = require('App/ViewModels/Commons/LocationViewModel');

interface PECMailBoxModel {
  EntityShortId: number;
  MailBoxRecipient: string;
  IncomingServer: string;
  OutgoingServer: string;
  Username: string;
  Password: string;
  RedirectAnomaliesSMTP: string;
  RedirectAnomaliesUsername: string;
  RedirectAnomaliesPassword: string;
  RedirectAnomaliesRecipient: string;
  RedirectStorageSMTP: string;
  RedirectStorageUsername: string;
  RedirectStoragePassword: string;
  RedirectStorageRecipient: string;
  IsForInterop: boolean;
  IsDestinationEnabled: boolean;
  DeleteMailFromServer: boolean;
  ReceiveDaysCap: number;
  Managed: boolean;
  Unmanaged: boolean;
  IdConfiguration: number;
  IncomingServerProtocol: number;
  IncomingServerPort: number;
  IncomingServerUseSsl: boolean;
  OutgoingServerPort: number;
  OutgoingServerUseSsl: boolean;
  IsHandleEnabled: boolean;
  IsProtocolBox: boolean;
  IsProtocolBoxExplicit: boolean;
  IdJeepServiceIncomingHost: string;
  IdJeepServiceOutgoingHost: string;
  RulesetDefinition: string;
  EntityId: number;
  UniqueId: string;
  RegistrationUser: string;
  RegistrationDate?: Date;
  LastChangedUser: string;
  LastChangedDate?: Date;
  HumanEnabled: boolean;
  Location: LocationViewModel;
  InvoiceType: string;
}

export = PECMailBoxModel;