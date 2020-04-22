(function () {
    var framesetElement = window.framesetElement = window.framesetElement || {};
    var timeoutID;
    var slidingZoneID;
    var slidingPaneID;
    var timeoutMS = 2000;
    var dswSignalR;

    var protocolNotReaded = "ProtocolliDaLeggere";
    var protocolInvoiceNotReaded = "ProtocolliDifatturaDaLeggere";
    var protocolToAccept = "ProtocolliDaAccettare";
    var protocolRefused = "ProtocolliRespinti";
    var protocolToDistribute = "ProtocolliDaDistribuire";
    var protocolRejected = "ProtocolliRigettati";
    var collToProtocol = "CollaborazioniDaProtocollare";
    var collToVision = "CollaborazioniDaVisionare";
    var wfUtenteCorrente = "WorkflowUtenteCorrente";
    var highlightProtocols = "ProtocolliInEvidenza";
    var pecNotReaded = "PECDaLeggere";
    var lastPagesToSign = "UltimePagineDaFirmare";

    window.OnClickedSaveCookie = function(sender, eventArgs) {
        createCookie('ItemExpanded', eventArgs.get_item().get_value(), 365);
    }

    window.SaveDockedCookie = function(sender, eventArgs) {
        createCookie('PaneDocked', 'true', 365);
    }

    window.SaveUnDockedCookie = function(sender, eventArgs) {
        createCookie('PaneDocked', 'false', 365);
    }

    window.ExpandPane = function(paneId) {
        var expandedPaneId = framesetElement.slidingZoneElement.get_expandedPaneId();
        if (expandedPaneId == null || expandedPaneId != paneId) {
            framesetElement.slidingZoneElement.expandPane('SlidingPaneMenu');
        } else {
            framesetElement.slidingZoneElement.collapsePane(paneId);
        }
    }

    window.NodeClicked = function(sender, eventArgs) {
        localStorage.setItem("CurrentPageUrl", eventArgs.get_node().get_navigateUrl());
        var node = eventArgs.get_node();
        if (node.get_nodes().get_count() > 0) {
            if (node.get_expanded()) {
                node.set_expanded(false);
            } else
                node.set_expanded(true);
        } else {
            framesetElement.slidingZoneElement.collapsePane('SlidingPaneMenu');
        }
    }

    window.CollapseMenu = function() {
        framesetElement.slidingZoneElement.collapsePane('RadPanelBarMenu');
    }

    window.SplitterLoaded = function(splitter, arg) {
        var height = framesetElement.mainElement.get_height();
        splitter.set_height(height);
        framesetElement.mainElement.set_height(height);
    }
    
    window.RadSlidingPane_OnClientExpanded = function(sender, args) {
        slidingZoneID = sender.get_parent().get_id();
        slidingPaneID = sender.get_id();
        if ($telerik.isIE8) {
            var splitterHeight = framesetElement.mainElement.get_height() - 27;
            $(".rspSlideContent").attr("style", "height: " + splitterHeight + "px !important");
        }        
        framesetElement.mainElement.onclick = body_onclick;
        ////Set the pane to close if the user doesn't "enter" the pane within the time frame.  
        timeoutID = setTimeout("CollapseOnTimeout('" + slidingPaneID + "')", timeoutMS);
        //Set up mouse enter and leave events to track auto close:
        sender.getContentContainer().onmouseleave = function () { timeoutID = setTimeout("CollapseOnTimeout('" + slidingPaneID + "')", timeoutMS); };
        sender.getContentContainer().onmouseenter = function () { clearTimeout(timeoutID); };
    }

    window.RadSlidingPane_OnClientCollapsing = function(sender, args) {
        document.body.onclick = null; //no longer need to collapse on body click as pane is closed.  
        clearTimeout(timeoutID);
    }

    //Closes the sliding pane if elements outside the pane are clicked.  
    window.body_onclick = function(e) {
        var eventIsFiredFromElement;
        if (e === null) { // I.E.
            eventIsFiredFromElement = event.srcElement;
        }
        else { // Firefox  
            eventIsFiredFromElement = e.target;
        }
        //Only close pane if the user clicked outside of the sliding pane:  
        var isChildOfSlidingPane = false;
        //Below is the only jQuery, which is used to test all the parents of our  
        //clicked control to see if it's in our sliding pane.  
        if (eventIsFiredFromElement.id === slidingZoneID)
            isChildOfSlidingPane = true;
        else if (eventIsFiredFromElement.id !== "" && $("#" + eventIsFiredFromElement.id).parents("[id=" + slidingZoneID + "]").length > 0)
            isChildOfSlidingPane = true;
        if (!isChildOfSlidingPane)
            CollapseOnTimeout(slidingPaneID);
    }

    window.CollapseOnTimeout = function(radSlidingPaneID) {
        $find(radSlidingPaneID).get_parent().collapsePane(radSlidingPaneID);
    }

    window.ToolBarClientClicking = function(button, args) {
        var pane = $find(button.get_target());
        pane.set_contentUrl(button.get_navigateUrl());
        args.set_cancel(true);
    }

    function setButtonCounter(notificationDto) {
        var btn;
        switch (notificationDto.NotificationName) {
            case protocolNotReaded:
                btn = $find(framesetElement.btnProtocolNotReader);
                if (btn){
                    btn.set_text(notificationDto.NotificationCount.toString());
                }                
                break;
            case protocolInvoiceNotReaded:
                btn = $find(framesetElement.btnProtocolInvoiceNotReaded);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case protocolToDistribute:
                btn = $find(framesetElement.btnProtocolToDistribute);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case protocolRejected:
                btn = $find(framesetElement.btnProtocolRejected);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case collToProtocol:
                btn = $find(framesetElement.btnCollToProtocol);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case collToVision:
                btn = $find(framesetElement.btnCollToVision);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case wfUtenteCorrente:
                btn = $find(framesetElement.btnWorkflow);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case highlightProtocols:
                btn = $find(framesetElement.btnHighlightProtocols);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case pecNotReaded:
                btn = $find(framesetElement.btnPECNotReader);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case protocolToAccept:
                btn = $find(framesetElement.btnProtocolToAccept);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case protocolRefused:
                btn = $find(framesetElement.btnProtocolRefused);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
            case lastPagesToSign:
                btn = $find(framesetElement.btnLastPagesToSign);
                if (btn) {
                    btn.set_text(notificationDto.NotificationCount.toString());
                }
                break;
        }
    }

    function notificationCounterMessageCallback(data) {
        $.each(data, function (i, item) {
            setButtonCounter(item);
        });
    }
    
    function signalrErrorCallback(error) {
        if (window.console) {
            console.log('SignalR error');
            console.log(error);
        }
    }

    function sendNotificationMessageRequest() {
        dswSignalR.sendServerMessage("GetNotificationCounter", "", null, signalrErrorCallback);
    }

    function tickNotificationCounter() {
        sendNotificationMessageRequest();
        //Impostato un valore safe prima di far partire i timer lato javascript
        if (framesetElement.notificationInterval > 30000) {
            var timer = setInterval(function () {
                $.ajax({
                    type: "GET",
                    url: "Frameset.aspx",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        sendNotificationMessageRequest();
                    },
                    error: function (response) {
                        clearInterval(timer);
                        dswSignalR.stopClient();
                    }
                });
            }, framesetElement.notificationInterval);
        }
    }

    window.initializeSignalR = function () {
        if (framesetElement.notificationEnabled == "True") {
            dswSignalR = new DSWSignalR(framesetElement.currentSignalRUrl);
            dswSignalR.setup("notificationTickerCount");
            dswSignalR.registerClientMessage("notificationCounterMessage", notificationCounterMessageCallback);
            dswSignalR.startConnection(tickNotificationCounter, signalrErrorCallback);
        }        
    }

    window.notifyAndRedirectToConfiguration = function (urlConfigurationPath, target) {
        var pane = $find(target);
        alert("Attenzione! E' necessario impostare l'azienda principale di lavoro. Verrà aperta la pagina di configurazione dove è possibile selezionare il menu contenente l'azienda.");
        pane.set_contentUrl(urlConfigurationPath);
    }
})();