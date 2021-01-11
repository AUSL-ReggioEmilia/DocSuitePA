
interface POLRequestMetaData {
    PushCalledAt: Date,
    LastGetStatusAt: Date,
    DoneWithGetStatus: boolean,
    DocumentFromUrlCpfSaved: boolean,
    DocumentFromUrlCpfXmlSaved: boolean,
    PolRequestContactName: string,
    PolAccountName : string
}

export = POLRequestMetaData