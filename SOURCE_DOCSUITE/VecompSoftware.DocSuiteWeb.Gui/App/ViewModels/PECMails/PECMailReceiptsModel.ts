class PECMailReceiptsModel {
    EntityId: number;
    ReceiptType: string;
    ErrorShort: string;
    ErrorDescription: string;
    ReceiptDate: Date;
    Sender: string;
    Receiver: string;
    ReceiverType: string;
    Subject: string;
    Provider: string;
    Identification: string;
    MSGID: string;
    EntityShortId: number;
    UniqueId: string;
    RegistrationUser: string;
    RegistrationDate: Date;
}

export = PECMailReceiptsModel;