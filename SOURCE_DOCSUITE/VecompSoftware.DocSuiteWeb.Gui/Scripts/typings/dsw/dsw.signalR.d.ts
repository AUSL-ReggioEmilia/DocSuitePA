
interface DSWSignalR {

    /**
     * Costruttore della classe
     * @serverAddress
     */
    new(serverAddress: string): DSWSignalR ;

    /**
     * Inizializza una nuova connessione ad un HUB specifico
     * @hubName
     * @queryString
     * @onErrorCallback
     */
    setup(hubName: string, queryString?: any, onErrorCallback?: (error: any) => void): void;

    /**
     * Avvia una nuova connessione ad un HUB specifico
     * @transport
     * @onDoneCallback
     * @onErrorCallback
     */
    startConnection(onDoneCallback?: () => void, onErrorCallback?: (error: any) => void): void;

    /**
     *
     * @hubMethodName
     * @callback
     */
    registerClientMessage(hubMethodName: string, callback?: (msg: any[]) => void): void;

    /**
     * Ferma la connessione SignalR
     */
    stopClient(): void;

    /**
     *
     * @hubMethodName
     * @data
     * @onDoneCallback
     * @onErrorCallback
     */
    sendServerMessage(hubMethodName: string, data: any, onDoneCallback: (data?: any[]) => void, onErrorCallback?: (error: any) => void): void;

    sendServerMessages(hubMethodName: string, correlationId: string, value: any, topicName: string, eventName: string, onDoneCallback: (data?: any[]) => void, onErrorCallback?: (error: any) => void): void;
}

declare var DSWSignalR: DSWSignalR;