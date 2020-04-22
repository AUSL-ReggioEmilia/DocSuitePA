/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Dossiers/DossierService", "App/Models/DocumentUnits/ChainType", "App/DTOs/ExceptionDTO"], function (require, exports, DossierService, ChainType, ExceptionDTO) {
    var DossierBase = /** @class */ (function () {
        function DossierBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        DossierBase.prototype.initialize = function () {
            this.service = new DossierService(this._serviceConfiguration);
        };
        DossierBase.prototype.addContainers = function (containers, rdlContainer) {
            var item;
            for (var _i = 0, containers_1 = containers; _i < containers_1.length; _i++) {
                var container = containers_1[_i];
                item = new Telerik.Web.UI.DropDownListItem();
                item.set_text(container.Name);
                item.set_value(container.EntityShortId.toString());
                rdlContainer.get_items().add(item);
                if (!container.isActive) {
                    $(item.get_element()).attr('style', 'color: grey');
                }
            }
            rdlContainer.trackChanges();
        };
        DossierBase.prototype.fillContacts = function (contact, dossierModel) {
            if (!!contact) {
                var contactModel = void 0;
                var contacts = new Array();
                var contactId = JSON.parse(contact);
                for (var _i = 0, contactId_1 = contactId; _i < contactId_1.length; _i++) {
                    var id = contactId_1[_i];
                    contactModel = {};
                    contactModel.EntityId = id;
                    contacts.push(contactModel);
                }
                dossierModel.Contacts = contacts;
            }
            return dossierModel;
        };
        DossierBase.prototype.fillRoles = function (role, dossierModel) {
            if (!!role) {
                var roleModel = void 0;
                var dossierRoleModel = {};
                var dossierRoles = new Array();
                var roleId = JSON.parse(role);
                for (var _i = 0, roleId_1 = roleId; _i < roleId_1.length; _i++) {
                    var id = roleId_1[_i];
                    roleModel = {};
                    roleModel.EntityShortId = id;
                    dossierRoleModel.Role = roleModel;
                    dossierRoles.push(dossierRoleModel);
                }
                dossierModel.DossierRoles = dossierRoles;
            }
            return dossierModel;
        };
        DossierBase.prototype.fillInserts = function (archiveChain, dossierModel) {
            if (archiveChain && archiveChain != "00000000-0000-0000-0000-000000000000") {
                var dossierDocumentModel = {};
                var dossierDocuments = new Array();
                dossierDocumentModel.ChainType = ChainType.Miscellanea;
                dossierDocumentModel.IdArchiveChain = archiveChain;
                dossierDocuments.push(dossierDocumentModel);
                dossierModel.DossierDocuments = dossierDocuments;
            }
            return dossierModel;
        };
        DossierBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        DossierBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        /**
        * Recupera una RadWindow dalla pagina
        */
        DossierBase.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        /**
        * Chiude la RadWindow
        */
        DossierBase.prototype.closeWindow = function (message) {
            var wnd = this.getRadWindow();
            wnd.close(message);
        };
        DossierBase.DOSSIER_TYPE_NAME = "Dossier";
        DossierBase.DOSSIERFOLDER_TYPE_NAME = "DossierFolder";
        DossierBase.DOSSIERDOCUMENT_TYPE_NAME = "DossierDocument";
        DossierBase.DOSSIERLOG_TYPE_NAME = "DossierLog";
        DossierBase.FASCICLE_TYPE_NAME = "Fascicle";
        DossierBase.ROLE_TYPE_NAME = "Role";
        return DossierBase;
    }());
    return DossierBase;
});
//# sourceMappingURL=DossierBase.js.map