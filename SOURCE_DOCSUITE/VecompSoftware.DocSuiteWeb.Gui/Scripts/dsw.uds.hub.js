var DSWUDSHub = (function () {

    var currentSignalRServerAddress;
    var currentUDSRadNotification;
    var currentUDSErrorRadNotification;
    var currentUDSScripts;
    var currentAjaxManager;
    var currentBtnAction;
    var currentBtnActionUniqueId;
    var currentActionMode;
    var currentHiddenField;
    var currentOnSuccessCallback;
    var currentOnErrorCallback;

    var correlationId = null;
    var tentativeCount = 0;
    var maxDeliveryCountOnQueue = 1;
    var dswSignalR;

    function DSWUDSHub(signalRServerAddress, udsRadNotification, udsErrorRadNotification,
        ajaxManager, btnAction, hiddenField, btnActionUniqueId, udsScripts) {

        currentSignalRServerAddress = signalRServerAddress;
        currentUDSRadNotification = udsRadNotification;
        currentUDSErrorRadNotification = udsErrorRadNotification;
        currentAjaxManager = ajaxManager;
        currentBtnAction = btnAction;
        currentHiddenField = hiddenField;
        currentBtnActionUniqueId = btnActionUniqueId;
        currentUDSScripts = udsScripts;
    }

    DSWUDSHub.prototype.start = function (actionMode, onSuccessCallback, onErrorCallback) {
        if (!actionMode || !currentSignalRServerAddress || !currentUDSRadNotification ||
            !currentUDSErrorRadNotification || !currentAjaxManager || !currentBtnAction ||
            !currentHiddenField || !currentBtnActionUniqueId || !currentUDSScripts) {
            onError("Unable to start hub manager.");
            return;
        }

        currentActionMode = actionMode;
        currentOnSuccessCallback = onSuccessCallback;
        currentOnErrorCallback = onErrorCallback;
        currentBtnAction.set_enabled(false);
        currentUDSRadNotification.hide();
        currentUDSErrorRadNotification.hide();
        tentativeCount = 0;
        dswSignalR = new DSWSignalR(currentSignalRServerAddress);
        correlationId = dswSignalR.newGuid();

        dswSignalR.setup("UDSEventHub", {
            'correlationId': correlationId
        });
        dswSignalR.registerClientMessage("udsDataEvent", actionHubSuccess);
        dswSignalR.registerClientMessage("udsDataError", actionHubError);
        dswSignalR.startConnection(onDoneSignalRConnectionCallback, onErrorSignalRCallback);
        udsScripts.showLoadingPanel();
    };

    function onDoneSignalRConnectionCallback() {
        var serverFunction = "SubscribeUDSInsertEvents";
        switch (currentActionMode) {
            case "Insert":
                serverFunction = "SubscribeUDSInsertEvents";
                break;
            case "Update":
                serverFunction = "SubscribeUDSUpdateEvents";
                break;
            case "Delete":
                serverFunction = "SubscribeUDSDeleteEvents";
                break;
        }
        dswSignalR.sendServerMessage(serverFunction, correlationId, onDoneSignalRSubscriptionCallback, onErrorSignalRCallback);
    }

    function onDoneSignalRSubscriptionCallback() {
        currentHiddenField.value = correlationId;
        currentAjaxManager.ajaxRequestWithTarget(currentBtnActionUniqueId, '');
        currentBtnAction.set_enabled(false);
        currentUDSRadNotification.set_text("Attendere prego, operazione in corso ...");
        currentUDSRadNotification.show();
    }

    function onErrorSignalRCallback(error) {
        console.log(error);
        tentativeCount = 0;
        completeError("Impossibile procedere con la gestione dell'archivio corrente. Contattare l'assistenza : Errore di comunicazione con le WebAPI.");
    }

    function actionHubSuccess(model) {
        if (correlationId !== null) {
            $.each(model.CorrelatedCommands, function (i, command) {
                if (command.CorrelationId === correlationId) {
                    if (currentOnSuccessCallback) {
                        currentOnSuccessCallback(model.Content.ContentTypeValue);
                    }
                    return;
                }
            });
        }
    }

    function actionHubError(model) {
        if (correlationId !== null) {
            $.each(model.CorrelatedCommands, function (i, command) {
                if (command.CorrelationId === correlationId) {
                    tentativeCount++;
                    completeError(model.Content.ContentTypeValue);
                }
            });
        }
    }

    function completeError(message) {
        console.log(message);
        currentUDSRadNotification.hide();
        currentUDSErrorRadNotification.set_updateInterval(0);
        if (tentativeCount < maxDeliveryCountOnQueue) {
            message = "Avvenuta anomalia durante l'inserimento dell'archivio. Nuovo tentativo in corso, attendere prego...";
        }    
        else {
            message = "L'errore non si è risolto automaticamente durante i successivi tetentativi. Contattare assistenza.<br/>"+message;
            currentUDSScripts.hideLoadingPanel();
            if (currentOnErrorCallback) {
                currentOnErrorCallback();
                currentBtnAction.set_enabled(true);
            }
        }
        currentUDSErrorRadNotification.show();
        currentUDSErrorRadNotification.set_text(message);
    }

    return DSWUDSHub;
})();