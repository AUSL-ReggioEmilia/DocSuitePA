import PECMailViewModel = require('App/ViewModels/PECMails/PECMailViewModel');
import IMapper = require('App/Mappers/IMapper');
import EnumHelper = require("App/Helpers/EnumHelper");

class PECMailViewModelMapper implements IMapper<PECMailViewModel>{
    private _enumHelper: EnumHelper;

    constructor() {
        this._enumHelper = new EnumHelper();
    }

    public Map(source: any): PECMailViewModel {
        let toMap: PECMailViewModel = <PECMailViewModel>{};
        if (!source) {
            return null;
        }

        toMap.EntityShortId = source.EntityShortId;
        toMap.Year = source.Year;
        toMap.Number = source.Number;
        toMap.MailUID = source.MailUID;
        toMap.MailContent = source.MailContent;
        toMap.MailSubject = source.MailSubject;
        toMap.MailSenders = source.MailSenders;
        toMap.MailRecipients = source.MailRecipients;
        toMap.MailDate = (source.MailDate !== null && source.MailDate !== undefined) ? moment(source.MailDate).format("DD/MM/YYYY") : "";
        toMap.MailType = source.MailType;
        toMap.MailError = source.MailError;
        toMap.MailPriority = source.MailPriority;
        toMap.XTrasporto = source.XTrasporto;
        toMap.MessageID = source.MessageID;
        toMap.XRiferimentoMessageID = source.XRiferimentoMessageID;
        toMap.Segnatura = source.Segnatura;
        toMap.MessaggioRitornoName = source.MessaggioRitornoName;
        toMap.MessaggioRitornoStream = source.MessaggioRitornoStream;
        toMap.MailBody = source.MailBody;
        toMap.RecordedInDocSuite = source.RecordedInDocSuite;
        toMap.ContentLength = source.ContentLength;
        toMap.IsToForward = source.IsToForward;
        toMap.IsValidForInterop = source.IsValidForInterop;
        toMap.IsActive = source.IsActive;
        toMap.MailStatus = source.MailStatus;
        toMap.IsDestinated = source.IsDestinated;
        toMap.DestinationNote = source.DestinationNote;
        toMap.Handler = source.Handler;
        toMap.IDAttachments = source.IDAttachments;
        toMap.IDDaticert = source.IDDaticert;
        toMap.IDEnvelope = source.IDEnvelope;
        toMap.IDMailContent = source.IDMailContent;
        toMap.IDPostacert = source.IDPostacert;
        toMap.IDSegnatura = source.IDSegnatura;
        toMap.IDSmime = source.IDSmime;
        toMap.PECType = source.PECType;
        toMap.Checksum = source.Checksum;
        toMap.Multiple = source.Multiple;
        toMap.SplittedFrom = source.SplittedFrom;
        toMap.OriginalRecipient = source.OriginalRecipient;
        toMap.HeaderChecksum = source.HeaderChecksum;
        toMap.ProcessStatus = source.ProcessStatus;
        toMap.MailRecipientsCc = source.MailRecipientsCc;
        toMap.ReceivedAsCc = source.ReceivedAsCc;
        toMap.Size = source.Size;
        toMap.MultipleType = source.MultipleType;
        toMap.IdUDS = source.IdUDS;
        toMap.DocumentUnitType = source.DocumentUnitType;
        toMap.EntityId = source.EntityId;
        toMap.UniqueId = source.UniqueId;
        toMap.InvoiceStatus = source.InvoiceStatus;
        toMap.InvoiceStatusDescription = source.InvoiceStatus !== null && source.InvoiceStatus !== undefined
            ? this._enumHelper.getInvoiceStatusDescription(source.InvoiceStatus)
            : "";
        toMap.InvoiceTypeDescription = (source.PECMailBox !== null && source.PECMailBox !== undefined)
            ? this._enumHelper.getInvoiceTypeDescription(source.PECMailBox.InvoiceType, source.Direction)
            : "";
        toMap.Direction = source.Direction;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.PECMailReceipts = source.PECMailReceipts;

        return toMap;
    }
}

export = PECMailViewModelMapper;