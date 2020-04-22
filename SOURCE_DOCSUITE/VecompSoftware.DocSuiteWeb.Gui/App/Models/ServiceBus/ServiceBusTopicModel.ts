interface ServiceBusTopicModel {
    ChannelName: string;
    Content: string;
    ContentType: string;
    CorrelationId: string;
    ExpiresAtUtc: Date;
    Label: string;
    MessageId: string;
    ReplyTo: string;
    ReplyToSessionId: string;
    ScheduledEnqueueTimeUtc: Date;
    SequenceNumber: number;
    SessionId: string;
    Size: number;
    State: string;
    SubscriptionName: string;
    TimeToLive: string;
}

export = ServiceBusTopicModel