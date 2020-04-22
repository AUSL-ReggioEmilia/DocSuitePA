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
                if (nodeType && nodeType == 'Root') {
                    _this._btnCopyFromUser.set_visible(false);
                    _this._btnGroupsAdd.set_visible(false);
                    _this._btnGuidedGroupsAdd.set_visible(false);
                    _this._btnDelete.set_visible(false);
                    return;
                }
                _this._btnCopyFromUser.set_visible(true);
                _this._btnGroupsAdd.set_visible(true);
                _this._btnGuidedGroupsAdd.set_visible(true);
                _this._btnDelete.set_visible(true);
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
            this.btnCopyFromUser_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this.openCopyFromUserWindow();
            };
            /**
             * Evento scatenato al click del bottone aggiungi utente
             * @param sender
             * @param args
             */
            this.btnAddUser_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this.openUserWindow();
            };
            /**
             * Evento scatenato al click del bottone elimina utente
             * @param sender
             * @param args
             */
            this.btnDeleteUser_OnClick = function (sender, args) {
                args.set_cancel(true);
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
            this.btnGroupsAdd_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this.openGroupsWindow();
            };
            /**
            * Evento scatenato al click del bottone Rimuovi dai gruppi
            * @param sender
            * @param args
            */
            this.btnDelete_OnClick = function (sender, args) {
                args.set_cancel(true);
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
            this.btnGuidedGroupsAdd_OnClick = function (sender, args) {
                args.set_cancel(true);
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
                //args.set_cancel(true);
            };
            $(document).ready(function () {
            });
        }
        /**
       * Inizializzazione classe
       */
        TbltSecurityUsers.prototype.initialize = function () {
            this._wndManager = $find(this.radWindowManagerGroupsId);
            this._wndManager.getWindowByName('windowGroupsAdd').add_close(this.onCloseWindowGroups);
            this._wndManager.getWindowByName('windowUserAdd').add_close(this.onCloseWindowUser);
            this._wndManager.getWindowByName('windowCopyFromUser').add_close(this.onCloseWindowCopyFromUser);
            this._rtvUsers = $find(this.rtvUsersId);
            this._rtvGroups = $find(this.rtvGroupsId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnGuidedGroupsAdd = $find(this.btnGuidedGroupsAddId);
            this._btnGuidedGroupsAdd.add_clicking(this.btnGuidedGroupsAdd_OnClick);
            this._btnGroupsAdd = $find(this.btnGroupsAddId);
            this._btnGroupsAdd.add_clicking(this.btnGroupsAdd_OnClick);
            this._btnCopyFromUser = $find(this.btnCopyFromUserId);
            this._btnCopyFromUser.add_clicking(this.btnCopyFromUser_OnClick);
            this._btnDelete = $find(this.btnDeleteId);
            this._btnDelete.add_clicking(this.btnDelete_OnClick);
            this._btnAddUser = $find(this.btnAddUserId);
            this._btnAddUser.add_clicking(this.btnAddUser_OnClick);
            this._btnDeleteUser = $find(this.btnDeleteUserId);
            this._btnDeleteUser.add_clicking(this.btnDeleteUser_OnClick);
            this._btnGroupsAdd.set_visible(false);
            this._btnCopyFromUser.set_visible(false);
            this._btnGuidedGroupsAdd.set_visible(false);
            this._btnDelete.set_visible(false);
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
        TbltSecurityUsers.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        return TbltSecurityUsers;
    }());
    return TbltSecurityUsers;
});
//# sourceMappingURL=TbltSecurityUsers.js.map