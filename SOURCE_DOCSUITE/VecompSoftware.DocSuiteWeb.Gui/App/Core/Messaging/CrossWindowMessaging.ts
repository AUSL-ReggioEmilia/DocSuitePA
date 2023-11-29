import CrossWindowMessagingSubscriber = require('App/Core/Messaging/CrossWindowMessagingSubscriber');
import ICrossWindowMessagingListener = require('App/Core/Messaging/ICrossWindowMessagingListener');
import ICrossWindowMessagingSender = require('App/Core/Messaging/ICrossWindowMessagingSender');
import CrossWindowMessagingListener = require('App/Core/Messaging/CrossWindowMessagingListener');
import CrossWindowMessagingSender = require('App/Core/Messaging/CrossWindowMessagingSender');

class CrossWindowMessaging
    implements ICrossWindowMessagingListener, ICrossWindowMessagingSender {

    private _sender: CrossWindowMessagingSender;
    private _receiver: CrossWindowMessagingListener

    constructor(
        senderLocation: Window,
        receiverLocation: Window) {

        this._sender = new CrossWindowMessagingSender(senderLocation);
        this._receiver = new CrossWindowMessagingListener(receiverLocation);

    }

    public SendMessage<T>(eventName: string, payload: T) {
        return this._sender.SendMessage(eventName, payload);
    }

    public ListenToMessage<T>(eventName: string, handler: (payload: T) => void): CrossWindowMessagingSubscriber {
        return this._receiver.ListenToMessage(eventName, handler);
    }
}

export = CrossWindowMessaging;