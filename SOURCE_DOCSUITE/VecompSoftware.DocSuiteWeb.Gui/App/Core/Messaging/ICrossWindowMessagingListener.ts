interface ICrossWindowMessagingListener {
    ListenToMessage<T>(eventName: string, handler: (payload: T) => void);
}


export = ICrossWindowMessagingListener;