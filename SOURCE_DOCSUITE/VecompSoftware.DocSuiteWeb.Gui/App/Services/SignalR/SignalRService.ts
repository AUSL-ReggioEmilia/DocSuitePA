import ExceptionDTO = require('App/DTOs/ExceptionDTO');

declare var signalR: any;

class SignalRService {

    private _connection: any;

    constructor(serverUrl, queryString: string) {
        let qs = "";

        if (queryString && queryString !== "") {
            qs = `?${queryString}`;
        }

        let url = `${serverUrl}${qs}`;

        this._connection = new signalR.HubConnectionBuilder()
            .withUrl(url)
            .configureLogging(signalR.LogLevel.Information)
            .build();
    }

    isConnected(): boolean {
        return this._connection.state === signalR.HubConnectionState.Connected;
    }

    startConnection(callback?: (data: any) => any, error?: (exception: any) => any): void {
        this._connection.start()
            .then(callback)
            .catch(error);
    };

    stopConnection(callback?: (data: any) => any, error?: (exception: any) => any): void {
        this._connection.stop()
            .then(callback)
            .catch(error);
    };

    sendServerMessage(methodName: string, arg1: any, arg2?: any, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        if (arg2) {
            this._connection.invoke(methodName, arg1, arg2)
                .then(callback)
                .catch(error);
        }
        else {
            this._connection.invoke(methodName, arg1)
                .then(callback)
                .catch(error);
        }
    }

    registerClientMessage(methodName: string, callback: any) {
        this._connection.on(methodName, callback);
    };

    registerOnReconnecting(callback: (data: any) => any) {
        this._connection.onreconnecting(callback);
    };

    registerOnReconnected(callback: (data: any) => any) {
        this._connection.onreconnected(callback);
    };

    registerOnClose(callback: (data: any) => any) {
        this._connection.onclose(callback);
    }
}

export = SignalRService;