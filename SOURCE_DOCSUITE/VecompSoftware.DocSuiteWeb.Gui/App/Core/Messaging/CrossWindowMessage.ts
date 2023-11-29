 class CrossWindowMessage {
    public Marker = 'CrossWindowMessage';

    constructor(public EventName: string, public Payload: any) {
    }
}

export = CrossWindowMessage;
