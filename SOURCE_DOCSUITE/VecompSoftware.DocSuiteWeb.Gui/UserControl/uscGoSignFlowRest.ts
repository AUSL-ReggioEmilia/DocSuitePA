
class uscGoSignFlowRest {
    private static IFRAME_NAME: string = "goSignFrame";
    private static MAX_ATTEMPTS: number = 20;

    private readonly _currentClientId: string;
    private readonly _proxySignUrl: string;
    private _currentAttempt = 0;

    constructor(currentClientId: string, proxySignUrl: string) {
        this._currentClientId = currentClientId;
        this._proxySignUrl = proxySignUrl;
    }

    initialize(): void {

    }

    private createProxySignFrame(sessionId: string): void {
        const path = `${this._proxySignUrl}/local/get-html/${sessionId}/IT`;
        $(`[name=${this._currentClientId}${uscGoSignFlowRest.IFRAME_NAME}]`).remove();
        let iframe = document.createElement("iframe");
        iframe.src = path;
        iframe.name = `${this._currentClientId}${uscGoSignFlowRest.IFRAME_NAME}`;
        iframe.style.display = 'none';
        document.body.appendChild(iframe);
    }

    private getGoSignStatus(sessionId: string): JQueryPromise<any> {
        const path = `${this._proxySignUrl}/api/web/getStatus?ioSessionID=${sessionId}`;
        return $.ajax({ url: path, dataType: 'jsonp', crossDomain: true });
    }

    private waitGoSignStatus(sessionId: string, deferred: JQueryDeferred<any>): void {
        this.getGoSignStatus(sessionId)
            .done((data) => {
            const status: string = data.currentStatus;
            switch (status.toUpperCase()) {
                case "PENDING":
                case "CERTS_READY":
                case "HASH_READY":
                case "HASH_SENT": {
                    if (this._currentAttempt < uscGoSignFlowRest.MAX_ATTEMPTS) {
                        this._currentAttempt++;
                        setTimeout(() => this.waitGoSignStatus(sessionId, deferred), 3000);
                    } else {
                        deferred.reject("Superati il numero massimo di tentativi di verifica stato firma");
                    }
                    break;
                }
                case "SIGN_READY":
                case "CLOSED": {
                    deferred.resolve(status);
                    break;
                }
                default: {
                    deferred.reject(status == "CANCELED" ? "Transazione cancellata dall'utente" : "Server error");
                    break;
                }
            }
        }).fail((error) => deferred.reject(error));
    }

    startGoSignFlow(sessionId: string): JQueryPromise<any> {
        const promise: JQueryDeferred<void> = $.Deferred<void>();
        this._currentAttempt = 0;
        this.createProxySignFrame(sessionId);
        const deferred = $.Deferred<any>();
        this.waitGoSignStatus(sessionId, deferred);
        deferred.done((result) => promise.resolve(result))
            .fail((error) => promise.reject(error))
            .always(() => $(`[name=${this._currentClientId}${uscGoSignFlowRest.IFRAME_NAME}]`).remove());
        return promise.promise();
    }
}

export = uscGoSignFlowRest;