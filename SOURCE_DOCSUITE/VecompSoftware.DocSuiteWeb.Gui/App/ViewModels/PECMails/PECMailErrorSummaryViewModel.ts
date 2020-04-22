class PECMailErrorSummaryViewModel {
/*
                        CorrelatedId = correlationId,
                        ProcessedErrorMessages = string.Join(Environment.NewLine, mailInfo.Errors),
                        Subject = mailInfo.Subject,
                        Body = mailInfo.Body,
                        Sender = mailInfo.Sender,
                        Recipients = mailInfo.Recipients,
                        ReceivedDate = mailInfo.Date,
                        Priority = mailInfo.Priority,
                        */
    CorrelationId: string;
    ProcessedErrorMessages: string;
    Subject: string;
    Body: string;
    Sender: string;
    Recipients: string;
    ReceivedDate: Date;
    Priority: string;
}

export = PECMailErrorSummaryViewModel