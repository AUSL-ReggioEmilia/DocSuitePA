export enum ProtocolStatusType {
     
    Incomplete = -10,
    Rejected = -20, 
    Temporary = -9,
    Wrong = -5,
    Booked = -4,
    Suspended = -3,
    Annulled = -2,
    //ErratoE
    Invalid = -1,
    Active = 0,
    Container = 1,
    Senders = 2,
    Recipients = 3,
    Recovered = 4,
    Used = 5,

    //Fattura PA inviata
    PAInvoiceSent = 6,
    //Fattura PA notificata
    PAInvoiceNotified = 7,
    //Fattura PA accettata - Notificata da SDI, non ancora da Destinatario
    PAInvoiceAccepted = 8,
    //Fattura PA rifiutata da SDI
    PAInvoiceSdiRefused = 9,
    //Fattura rifiutata
    PAInvoiceRefused = 10

}