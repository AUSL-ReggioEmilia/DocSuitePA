//Gestione senza proxy automatico di SignalR
var DSWSignalR = (function () {

    var currentHub;
    var currentHubProxy;
    var currentServerAddress;

    function DSWSignalR(serverAddress) {
        currentHub = null;
        currentHubProxy = null;
        if (serverAddress) {
            currentServerAddress = serverAddress;
        }
    }

    function showWarningNotification(message) {
        console.log(message);
    }

    function initializeCallback(invokedMethod, onDoneCallback, onErrorCallback) {
        if (onDoneCallback) {
            invokedMethod.done(onDoneCallback);
        }

        if (onErrorCallback) {
            invokedMethod.fail(onErrorCallback);
        }
    }

    DSWSignalR.prototype.setup = function (hubName, queryString, onErrorCallback) {
        currentHub = jQuery.hubConnection(currentServerAddress);

        if (queryString && queryString !== "") {
            currentHub.qs = queryString;
        }

        if (onErrorCallback) {
            currentHub.error(onErrorCallback);
        }
        currentHubProxy = currentHub.createHubProxy(hubName);
    };

    DSWSignalR.prototype.startConnection = function (onDoneCallback, onErrorCallback) {
        if (!onDoneCallback) {
            onDoneCallback = function () { };
        }
        if (!onErrorCallback) {
            onErrorCallback = function () { };
        }
        currentHub.start()
            .done(onDoneCallback)
            .fail(onErrorCallback);
    };

    DSWSignalR.prototype.registerClientMessage = function (hubMethodName, callback) {
        currentHubProxy.on(hubMethodName, callback);
    };

    DSWSignalR.prototype.stopClient = function () {
        currentHub.stop();
    };

    DSWSignalR.prototype.sendServerMessage = function (hubMethodName, correlationId, onDoneCallback, onErrorCallback) {
        var invokedMethod = currentHubProxy.invoke(hubMethodName, correlationId);
        initializeCallback(invokedMethod, onDoneCallback, onErrorCallback);
    };

    DSWSignalR.prototype.sendServerMessages = function (hubMethodName, correlationId, value, onDoneCallback, onErrorCallback) {
        var invokedMethod = currentHubProxy.invoke(hubMethodName, correlationId, value);
        initializeCallback(invokedMethod, onDoneCallback, onErrorCallback);
    };

    DSWSignalR.prototype.newGuid = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    };

    return DSWSignalR;
})();