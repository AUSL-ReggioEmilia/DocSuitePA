define(["require", "exports"], function (require, exports) {
    var TbltSecurityUsers = /** @class */ (function () {
        /**
    * Costruttore
    * @param serviceConfigurations
    */
        function TbltSecurityUsers() {
            var _this = this;
            /**
           *------------------------- Events -----------------------------
           */
            this.initializeDetailsCallback = function (nodeType) {
                _this.initializeSpecialToolbarAction();
                if (nodeType && nodeType == 'Root') {
                    _this._updateActionButtonsVisibility(false);
                    return;
                }
                _this._updateActionButtonsVisibility(true);
            };
            /**
             * Evento alla chiusura della finestra di copia da utente
             * @param sender
             * @param args
             */
            this.onCloseWindowCopyFromUser = function (sender, args) {
                if (args.get_argument()) {
                    _this._ajaxManager.ajaxRequest('copyFromUser|' + args.get_argument());
                }
            };
            /**
             * Evento alla chiusura della finestra di inserimento utente
             * @param sender
             * @param args
             */
            this.onCloseWindowUser = function (sender, args) {
                if (args.get_argument()) {
                    _this._ajaxManager.ajaxRequest('users|' + args.get_argument());
                }
            };
            /**
           * Evento alla chiusura della finestra di assegnazione gruppi
           * @param sender
           * @param args
           */
            this.onCloseWindowGroups = function (sender, args) {
                if (args.get_argument()) {
                    _this._ajaxManager.ajaxRequest('groups|' + args.get_argument());
                }
            };
            /**
             * Evento scatenato al click del bottone copia da utente
             * @param sender
             * @param args
             */
            this.btnCopyFromUser_OnClick = function () {
                _this.openCopyFromUserWindow();
            };
            /**
             * Evento scatenato al click del bottone aggiungi utente
             * @param sender
             * @param args
             */
            this.btnAddUser_OnClick = function () {
                _this.openUserWindow();
            };
            /**
             * Evento scatenato al click del bottone elimina utente
             * @param sender
             * @param args
             */
            this.btnDeleteUser_OnClick = function () {
                _this._rtvUsers = $find(_this.rtvUsersId);
                if (!_this._rtvUsers.get_selectedNode() || !_this._rtvUsers.get_selectedNode().get_value()) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare un utente.");
                    return;
                }
                _this._wndManager.radconfirm("Sei sicuro di voler rimuovere l'utente da tutti i gruppi?", function (arg) {
                    if (arg) {
                        _this._ajaxManager.ajaxRequest('deleteuser');
                    }
                }, 300, 160);
            };
            /**
            * Evento scatenato al click del bottone Assegna gruppi
            * @param sender
            * @param args
            */
            this.btnGroupsAdd_OnClick = function () {
                _this.openGroupsWindow();
            };
            /**
            * Evento scatenato al click del bottone Rimuovi dai gruppi
            * @param sender
            * @param args
            */
            this.btnDelete_OnClick = function () {
                _this._rtvUsers = $find(_this.rtvUsersId);
                if (!_this._rtvUsers.get_selectedNode() || !_this._rtvUsers.get_selectedNode().get_value()) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare un utente.");
                    return;
                }
                _this._rtvGroups = $find(_this.rtvGroupsId);
                if (!_this._rtvGroups.get_checkedNodes() || _this._rtvGroups.get_checkedNodes().length == 0) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare almeno un gruppo.");
                    return;
                }
                _this._wndManager.radconfirm("Sei sicuro di voler rimuovere l'utente?", function (arg) {
                    if (arg) {
                        _this._ajaxManager.ajaxRequest('delete');
                    }
                }, 300, 160);
            };
            /**
            * Evento scatenato al click del bottone Configurazione guidata
            * @param sender
            * @param args
            */
            this.btnGuidedGroupsAdd_OnClick = function () {
                _this._rtvUsers = $find(_this.rtvUsersId);
                if (!_this._rtvUsers.get_selectedNode() || !_this._rtvUsers.get_selectedNode().get_value()) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare un utente.");
                    return;
                }
                var selectedUser = _this._rtvUsers.get_selectedNode();
                if (selectedUser) {
                    window.location.href = "../Tblt/TbltSecurityGroupWizard.aspx?Type=Comm&DomainAD=".concat(selectedUser.get_attributes().getAttribute('Domain'), "&AccountAD=", selectedUser.get_attributes().getAttribute('Account'));
                }
            };
            /**
             * Metodo di apertura finestra aggiunta utente
             */
            this.openUserWindow = function () {
                var url = "../Comm/SelUsers.aspx?Type=Comm";
                _this.openWindow(url, "windowUserAdd", 700, 600);
            };
            /**
             * Metodo di apertura finestra aggiunta utente
             */
            this.openCopyFromUserWindow = function () {
                var url = "../Comm/SelUsers.aspx?Type=Comm";
                _this.openWindow(url, "windowCopyFromUser", 700, 600);
            };
            /**
             * Metodo di apertura finestra assegnazione gruppi
             */
            this.openGroupsWindow = function () {
                _this._rtvUsers = $find(_this.rtvUsersId);
                if (!_this._rtvUsers.get_selectedNode() || !_this._rtvUsers.get_selectedNode().get_value()) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare un utente.");
                    return;
                }
                var url = "../UserControl/uscMultiSelGroups.aspx?Type=Comm";
                _this.openWindow(url, 'windowGroupsAdd', 500, 600);
            };
            this.actionToolbar_ButtonClicked = function (sender, args) {
                var currentActionButtonItem = args.get_item();
                var currentAction = _this.toolbarActions().filter(function (item) { return item[0] == currentActionButtonItem.get_commandName(); })
                    .map(function (item) { return item[1]; })[0];
                currentAction();
            };
            this.specialToolbarActions_ButtonClicked = function (sender, args) {
                var currentActionButtonItem = args.get_item();
                var currentAction = _this.specialToolbarActions().filter(function (item) { return item[0] == currentActionButtonItem.get_commandName(); })
                    .map(function (item) { return item[1]; })[0];
                currentAction();
            };
            $(document).ready(function () {
            });
        }
        TbltSecurityUsers.prototype.toolbarActions = function () {
            var _this = this;
            var items = [
                [TbltSecurityUsers.ADD_COMMANDNAME, function () { return _this.btnAddUser_OnClick(); }],
                [TbltSecurityUsers.DELETEUSER_COMMANDNAME, function () { return _this.btnDeleteUser_OnClick(); }]
            ];
            return items;
        };
        TbltSecurityUsers.prototype.specialToolbarActions = function () {
            var _this = this;
            var items = [
                [TbltSecurityUsers.COPYFROMUSER_COMMANDNAME, function () { return _this.btnCopyFromUser_OnClick(); }],
                [TbltSecurityUsers.GROUPSADD_COMMANDNAME, function () { return _this.btnGroupsAdd_OnClick(); }],
                [TbltSecurityUsers.GUIDEDGROUPSADD_COMMANDNAME, function () { return _this.btnGuidedGroupsAdd_OnClick(); }],
                [TbltSecurityUsers.DELETE_COMMANDNAME, function () { return _this.btnDelete_OnClick(); }]
            ];
            return items;
        };
        /**
       * Inizializzazione classe
       */
        TbltSecurityUsers.prototype.initialize = function () {
            this._actionToolbar = $find(this.actionsToolbarId);
            this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);
            this._wndManager = $find(this.radWindowManagerGroupsId);
            this._wndManager.getWindowByName('windowGroupsAdd').add_close(this.onCloseWindowGroups);
            this._wndManager.getWindowByName('windowUserAdd').add_close(this.onCloseWindowUser);
            this._wndManager.getWindowByName('windowCopyFromUser').add_close(this.onCloseWindowCopyFromUser);
            this._rtvUsers = $find(this.rtvUsersId);
            this._rtvGroups = $find(this.rtvGroupsId);
            this._ajaxManager = $find(this.ajaxManagerId);
        };
        TbltSecurityUsers.prototype.initializeSpecialToolbarAction = function () {
            this._toolbarSpecialAction = $find(this.toolbarSpecialActionId);
            if (this._toolbarSpecialAction) {
                this._toolbarSpecialAction.add_buttonClicked(this.specialToolbarActions_ButtonClicked);
            }
        };
        /*
         * ------------------------------- Methods ----------------------------------
         */
        /**
         * Metodo di apertura nuova finestra
         * @param url
         * @param name
         * @param width
         * @param height
         */
        TbltSecurityUsers.prototype.openWindow = function (url, name, width, height) {
            var wnd = this._wndManager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.center();
            return false;
        };
        TbltSecurityUsers.prototype._updateActionButtonsVisibility = function (isVisible) {
            var toolbarBtnDisplayMode = isVisible ? "inline-block" : "none";
            this._toolbarSpecialAction.get_items().forEach(function (item) {
                var btnCommandName = item.get_commandName();
                if (btnCommandName === TbltSecurityUsers.ADD_COMMANDNAME || btnCommandName === TbltSecurityUsers.DELETEUSER_COMMANDNAME) {
                    return;
                }
                item.get_element().style.display = toolbarBtnDisplayMode;
            });
        };
        TbltSecurityUsers.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        TbltSecurityUsers.ADD_COMMANDNAME = "AddUser";
        TbltSecurityUsers.DELETEUSER_COMMANDNAME = "DeleteUser";
        TbltSecurityUsers.COPYFROMUSER_COMMANDNAME = "CopyFromUser";
        TbltSecurityUsers.GROUPSADD_COMMANDNAME = "AddGroups";
        TbltSecurityUsers.GUIDEDGROUPSADD_COMMANDNAME = "GuidedGroupsAdd";
        TbltSecurityUsers.DELETE_COMMANDNAME = "Delete";
        return TbltSecurityUsers;
    }());
    return TbltSecurityUsers;
});
//# sourceMappingURL=TbltSecurityUsers.js.map