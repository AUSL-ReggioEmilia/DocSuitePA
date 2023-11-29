//#region [ Estensioni Javascript ]

//Controllo terminazione stringa
String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

Date.prototype.defaultView = function () {
    var dd = this.getDate();
    if (dd < 10) dd = '0' + dd;
    var mm = this.getMonth() + 1;
    if (mm < 10) mm = '0' + mm;
    var yyyy = this.getFullYear();
    return String(dd + "\/" + mm + "\/" + yyyy);
};

//#endregion

//#region [ Telerik ]

// Restituisce un riferimento alla radwindow
function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) {
        oWindow = window.radWindow;
    } else if (window.frameElement.radWindow) {
        oWindow = window.frameElement.radWindow;
    }
    return oWindow;
}


// Restituisce un riferimento alla radwindow
function GetRadWindowByName(name) {
    var oWindow = GetRadWindow();
    return oWindow.get_windowManager().getWindowByName(name);
}

// Apertura finestre
function OpenWindow(url, name, width, height) {
    var oWnd = top.radopen(url, name);
    oWnd.setSize(width, height);
    oWnd.center();
    return oWnd;
}

//#endregion

//#region [ BindGrid ]

//chiude il menu dei filtri sull'evento click del mouse
function ItemMenuClickedHandler(sender, eventArgs) {
    var menu = eventArgs.get_item().get_menu();
    menu.hide();
}

function DoFilter(sender, eventArgs, gridID, columnName, filter) {
    var grid = $find(gridID);
    if (eventArgs.keyCode === 13 && grid !== null) {
        eventArgs.cancelBubble = true;
        eventArgs.returnValue = false;
        if (eventArgs.preventDefault) {
            eventArgs.preventDefault();
        }
        var masterTable = grid.get_masterTableView();
        masterTable.filter(columnName, "", filter);

        return false;
    }
}

function DoExport(gridID, exportType) {
    if (gridID !== "") {
        createCookie("ExportGridID", gridID, 0);
        createCookie("ExportGridType", exportType, 0);
        var btn = document.getElementById("<%= btnExport.ClientID %>");
        alert(btn);
        if (btn !== null) {
            btn.click();
        }
    }
    return false;
}

function createCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}

function mngRequestStarted(ajaxManager, eventArgs) {
    var array = eventArgs.EventTarget.split('$');
    var target = array[array.length - 1];
    if (target === 'excelFButton' || target === 'excelButton' || target === 'wordFButton' || target === 'wordButton' || target === 'pdfButton' || target === 'lbtViewProtocol' || target === 'lnkPratica' || target === 'lnkResolution' || target == 'excelCButton') {
        eventArgs.EnableAjax = false;
    }
}

function RedirectParent(url) {
    parent.location = url;
    return false;
}

//#endregion

//#region [ Error Handling ]

//#region Classi di errori generici

function GenericException(message) {
    if (!message || !message.length) {
        message = "Eccezione generica.";
    }
    this.message = message;
    this.name = "GenericException";
}

function UserException(message) {
    if (!message || !message.length) {
        message = "Eccezione utente.";
    }
    this.message = message;
    this.name = "UserException";
}

function DigitalSignException(message) {
    if (!message || !message.length) {
        message = "Eccezione DigitalSign generica.\nPossibili cause:\nLettore non correttamente collegato.\nCarta non inserita o non riconosciuta.";
    }
    this.message = message;
    this.name = "DigitalSignException";
}

//#endregion

//#region Aggancio alla classe Error per avere comportamenti aggiuntivi dati dal browser

GenericException.prototype = new Error();
GenericException.prototype.constructor = GenericException;

UserException.prototype = new Error();
UserException.prototype.constructor = UserException;

DigitalSignException.prototype = new Error();
DigitalSignException.prototype.constructor = DigitalSignException;

//#endregion

// Interpreta gli errori provenienti dalle varie fonti fornendo un'interfaccia per la loro visualizzazione
function ErrorHandler() {

    //#region Fields

    var unifiedException = new GenericException();

    //#endregion

    //#region Private Properties/Methods

    // Uniforma agli errori custom
    var extrudeException = function (exception) {
        // Eccezione vuota
        if (!exception) {
            return new GenericException();
        }
        // Eccezioni conosciute gestite
        if (exception instanceof GenericException || exception instanceof UserException || exception instanceof DigitalSignException) {
            return exception;
        }
        // Eccezioni conosciute non gestite
        var specificException;
        if (exception["number"] === -2147467259) { // Qui vengono accumunate moltissime eccezioni differenti
            specificException = new DigitalSignException(exception.message);
            // Controllo presenza errore specifico
            var beginCodeIndex = exception.message.indexOf(' (Cod. ', 0);
            specificException.message = beginCodeIndex !== -1 ? exception.message.substring(0, beginCodeIndex) + "." : exception.message;
            specificException.code = exception.message.substring(beginCodeIndex + 7, exception.message.indexOf(')', 0));
        } else if (exception["number"] === -2146823281) {
            // Impossibile caricare l'oggetto COM nel browser
            specificException = new DigitalSignException("Periferica non installata o driver mancanti.");
        } else {
            // Gestione generica dell'eccezione
            var debugMessage = "Errore generico, contattare amministratore di sistema.\nInformazioni:\n";
            for (var prop in exception) {
                debugMessage += prop + " value: [" + exception[prop] + "]\n";
            }
            debugMessage += "toString(): value: [" + exception.toString() + "]";
            specificException = new GenericException(debugMessage);
        }
        return specificException;
    };

    //#endregion

    //#region Public Properties/Methods

    this.setException = function (exception) {
        unifiedException = extrudeException(exception);
    };

    this.getException = function () {
        return unifiedException;
    };

    this.log = function () {
        log(unifiedException.message);
    };

    this.show = function () {
        alert(unifiedException.message);
    };

    //#endregion
}

//#region Static functions


ErrorHandler.logAndShow = function (exception) {
    var handler = new ErrorHandler();
    handler.setException(exception);
    handler.log();
    handler.show();
};

//#endregion

//#endregion

function WebCheckClicked(sender) {
    if (sender.checked) {
        return false;
    }
    try {
        var base = sender.id.substring(0, sender.id.lastIndexOf("_") + 1);
        var founded = false;
        for (var i = 0; i < 100; i++) {
            var id = base + i;
            var cb = document.getElementById(id);
            if (cb != null) {
                if (cb.checked) {
                    founded = true;
                }
            } else {
                i = 100; // Uscita forzata
            }
        }
        // Se non c'è almeno un check "fratello" attivo annullo l'uncheck
        if (!founded) {
            sender.checked = true;
        }

    } catch (e) {
        // NOOP
    }
    return false;
}

function centerUpdatePanel() {
    centerElementOnScreen(document.getElementById("<%=LoadingPanel.ClientID %>"));
}

function centerElementOnScreen(element) {
    if (element !== null) {
        var scrollTop = document.body.scrollTop;
        var scrollLeft = document.body.scrollLeft;

        var viewPortHeight = document.body.clientHeight;
        var viewPortWidth = document.body.clientWidth;

        if (document.compatMode === "CSS1Compat") {
            viewPortHeight = document.documentElement.clientHeight;
            viewPortWidth = document.documentElement.clientWidth;

            scrollTop = document.documentElement.scrollTop;
            scrollLeft = document.documentElement.scrollLeft;
        }

        var topOffset = Math.ceil(viewPortHeight / 2 - element.offsetHeight / 2);
        var leftOffset = Math.ceil(viewPortWidth / 2 - element.offsetWidth / 2);

        var top = scrollTop + topOffset;
        var left = scrollLeft + leftOffset;

        element.style.position = "absolute";
        element.style.top = top + "px";
        element.style.left = left + "px";
    }
}

function ChangeStrWithValidCharacter(oggetto) {
    var str = oggetto.value;
    //conversione su riconoscimento carattere
    var table = { '“': '\"', '”': '\"', '‘': '\'', '’': '\'', '–': '-' };
    var i = 0;
    var tmpArr = "";

    while (i < str.length) {
        var c1 = str.charAt(i);
        var temp = table[c1];

        if (temp !== null && temp !== undefined) {
            tmpArr += temp;
        } else {
            tmpArr += c1;
        }
        i++;
    }
    oggetto.value = tmpArr;
}

// Torna true se la seconda versione è maggiore della prima
function versionComparer(version1, version2) {
    var result = false;

    if (!(version1 instanceof Object)) {
        version1 = version1.toString().split(',');
    }
    if (!(version2 instanceof Object)) {
        version2 = version2.toString().split(',');
    }

    for (var i = 0; i < (Math.max(version1.length, version2.length)); i++) {

        if (version1[i] === undefined) {
            version1[i] = 0;
        }
        if (version2[i] === undefined) {
            version2[i] = 0;
        }

        if (Number(version1[i]) < Number(version2[i])) {
            result = true;
            break;
        }
        if (version1[i] !== version2[i]) {
            break;
        }
    }
    return (result);
}

//#region [ Constants ]

//Dimensioni finestra EDITING
var WIDTH_EDIT_WINDOW = 700;
var HEIGHT_EDIT_WINDOW = 400;

//Dimensioni finestra GRUPPI
var WIDTH_GROUP_WINDOW = 500;
var HEIGHT_GROUP_WINDOW = 600;

//Dimensioni finestra LOG
var WIDTH_LOG_WINDOW = 700;
var HEIGHT_LOG_WINDOW = 450;

//Dimensioni finestra STAMPA
var WIDTH_PRINT_WINDOW = 700;
var HEIGHT_PRINT_WINDOW = 500;

//#endregion

function ShowPopupAbove(sender, args) {
    var picker = $find(sender);
    var textBox = picker.get_textBox();
    var popupElement = picker.get_popupContainer();
    var dimensions = picker.getElementDimensions(popupElement);
    var position = picker.getElementPosition(textBox);
    picker.showPopup(position.x + 5, position.y - dimensions.height + 20);
}

function ShowPopupRight(sender, args) {
    var picker = $find(sender);
    var textBox = picker.get_textBox();
    var button = picker.get_popupButton();
    var popupElement = picker.get_popupContainer();
    var dimensions = picker.getElementDimensions(popupElement);
    var position = picker.getElementPosition(textBox);
    picker.showPopup(position.x + textBox.clientWidth + button.clientWidth + 5, position.y - dimensions.height / 2 + 50);
}

function OnClientNodeClickedExpand(sender, eventArgs) {
    var node = eventArgs.get_node();
    if (node) {
        node.expand();
    }
}

function SetButtonVisibility(buttonControl, visible) {
    if (buttonControl !== null) {
        buttonControl.style.display = visible ? "inline" : "none";
    }
}

function EnableButton(buttonControl, enable) {
    if (buttonControl !== null) {
        buttonControl.disabled = !enable;
    }
}

function EnableItemByValue(menu, itemValue) {
    var menuItem = menu.findItemByValue(itemValue);
    if (menuItem !== null) {
        menuItem.enable();
    }
}

function DisableItemByValue(menu, itemValue) {
    var menuItem = menu.findItemByValue(itemValue);
    if (menuItem !== null) {
        menuItem.disable();
    }
}

function SetTextBoxValue(textBoxId, value) {
    var txt = document.getElementById(textBoxId);
    if (txt !== null) {
        txt.value = value;
    }
}

function AdjustButtonWithItem(buttonId, menu, itemValue) {
    var btn = document.getElementById(buttonId);
    if (btn !== null) {
        var item = menu.findItemByValue(itemValue);
        if (item !== null) {
            btn.disabled = (!item.get_enabled());
        }
    }
}

function anyNodeButRootCheck(sender, args) {
    var root = $find(sender.controltovalidate).get_nodes().getNode(0);
    if (!root || root.get_allNodes().length === 0) {
        args.IsValid = false;
        return;
    }
    args.IsValid = true;
}

function anyNodeCheck(sender, args) {
    var nodes = $find(sender.controltovalidate).get_allNodes();
    if (!nodes || nodes.length === 0) {
        args.IsValid = false;
        return;
    }
    args.IsValid = true;
}

function anySelectedNode(sender, args) {
    var selectedNode = $find(sender.controltovalidate).get_selectedNode();
    args.IsValid = !!selectedNode;
}

function RadConfirm(sender, args) {
    var callBackFunction = Function.createDelegate(sender, function (shouldSubmit) {
        if (shouldSubmit) {
            this.click();
        }
    });

    radconfirm(sender.get_commandArgument(), callBackFunction, 300, 160, null, "Conferma");
    args.set_cancel(true);
}

function getDocumentIconURL(documents) {
    if (documents == undefined) {
        return '../App_Themes/DocSuite2008/imgset16/document_empty.png';
    }
    switch (getExtension(documents.DocumentName)) {
        case 'pdf':
            return '../App_Themes/DocSuite2008/imgset16/file_extension_pdf.png';
            break;
        case 'doc': case 'docx':
            return '../App_Themes/DocSuite2008/imgset16/file_extension_doc.png';
            break;
        case 'xls': case 'xlsx':
            return '../App_Themes/DocSuite2008/imgset16/file_extension_xls.png';
            break;
        case 'zip':
            return '../App_Themes/DocSuite2008/imgset16/file_extension_zip.png';
            break;
        case 'rar':
            return '../App_Themes/DocSuite2008/imgset16/file_extension_rar.png';
            break;
        case 'txt':
            return '../App_Themes/DocSuite2008/imgset16/file_extension_txt.png';
            break;
        case 'xml':
            return '../App_Themes/DocSuite2008/imgset16/tag.png';
            break;
        case 'log':
            return '../App_Themes/DocSuite2008/imgset16/file_extension_log.png';
            break;
        case 'p7m': case 'p7x': case 'p7s':
            return '../App_Themes/DocSuite2008/imgset16/card_chip_gold.png';
            break;
        default:
            return '../App_Themes/DocSuite2008/imgset16/document_empty.png';
    }
}