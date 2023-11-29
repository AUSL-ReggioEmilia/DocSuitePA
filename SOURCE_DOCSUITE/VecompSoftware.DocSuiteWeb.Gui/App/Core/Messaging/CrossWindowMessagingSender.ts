import CrossWindowMessage = require('App/Core/Messaging/CrossWindowMessage');
import ICrossWindowMessagingSender = require('App/Core/Messaging/ICrossWindowMessagingSender');

class CrossWindowMessagingSender implements ICrossWindowMessagingSender {
    constructor(private receiverLocation: Window) {
    }

    public SendMessage<T>(eventName: string, payload: T): void {
        let cwm = new CrossWindowMessage(eventName, payload);
        this.receiverLocation.postMessage(cwm, this.receiverLocation.origin);
    }
}

export = CrossWindowMessagingSender;