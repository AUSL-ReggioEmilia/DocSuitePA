interface ServiceBusTopicContentModel {
    $id: string;
    $type: string;
    Contents: string;
    CorrelatedMessages: object;
    CorrelationId: string;
    CreationTime: Date;
    CustomProperties: object;
    EventNAme: string;
    ExecutedTime: Date;
    Id: string;
    Identity: object;
    Name: string;
    TenantId: string;
    TenantName: string;
}

export = ServiceBusTopicContentModel