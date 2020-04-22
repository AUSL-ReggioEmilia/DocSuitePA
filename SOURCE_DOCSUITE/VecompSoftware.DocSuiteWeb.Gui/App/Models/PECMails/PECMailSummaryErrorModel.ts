interface PECMailSummaryErrorModel {
    $id: string;
    $type: string;
    Body: string;
    CorrelatedId: string;
    Priority: number;
    ProcessedErrorMessages: string;
    ReceivedDate: string;
    Recipients: string;
    Sender: string;
    Subject: string;
}

export = PECMailSummaryErrorModel