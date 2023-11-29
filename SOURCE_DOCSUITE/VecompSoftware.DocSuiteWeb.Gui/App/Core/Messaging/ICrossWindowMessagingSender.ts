interface ICrossWindowMessagingSender {
    SendMessage<T>(eventName: string, payload: T);
}

export = ICrossWindowMessagingSender;