import CrossWindowMessage = require('App/Core/Messaging/CrossWindowMessage');
import CrossWindowMessagingSubscriber = require('App/Core/Messaging/CrossWindowMessagingSubscriber');
import ICrossWindowMessagingListener = require('App/Core/Messaging/ICrossWindowMessagingListener');

class CrossWindowMessagingListener implements ICrossWindowMessagingListener {
    private subscribers: CrossWindowMessagingSubscriber[] = [];

    constructor(private receiverLocation: Window) {
    }

    public ListenToMessage<T>(eventName: string, handler: (payload: T) => void): CrossWindowMessagingSubscriber {
        // we need to keep a reference of the exact function for removing the handler
        // to avoid memory leakage
        let messageHandler = (event: MessageEvent<any>) => {
            if (event.data.hasOwnProperty('Marker') && event.data['Marker'] === 'CrossWindowMessage') {
                let cwm = event.data as CrossWindowMessage;

                if (cwm.EventName === eventName) {
                    handler(cwm.Payload);
                }
            }
        };

        this.receiverLocation.addEventListener('message', messageHandler);

        // create a subscriber which contains the exact unsubscriber logic using
        // the local scope function reference
        let subscriber = new CrossWindowMessagingSubscriber(() => {
            this.receiverLocation.removeEventListener('message', messageHandler);
        });

        this.subscribers.push(subscriber);

        return subscriber;
    }

    public UnsubscribeAll(): void {
        for (let subscriber of this.subscribers) {
            if (subscriber) {
                subscriber.Unsubscribe();
            }
        }
    }
}

export = CrossWindowMessagingListener;