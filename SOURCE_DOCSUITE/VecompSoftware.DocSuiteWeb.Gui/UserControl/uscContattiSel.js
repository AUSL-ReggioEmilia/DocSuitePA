/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
define(["require", "exports", "../app/core/extensions/string"], function (require, exports) {
    var uscContattiSel = /** @class */ (function () {
        function uscContattiSel() {
            var _this = this;
            this.enableValidators = function (state) {
                ValidatorEnable(document.getElementById(_this.validationId), state);
            };
            this.setContacts = function (contact) {
                //set new object if getContacts() returns null
                var contactsList = JSON.parse(_this.getContacts()) || [];
                contactsList.push(contact);
                sessionStorage[_this._sessionStorageKey] = JSON.stringify(contactsList);
            };
            this.deleteContacts = function (contactsToRemove) {
                var contactsList = JSON.parse(_this.getContacts()) || [];
                //searching for items to remove (not using findIndex() or filter() for IE compatibility)
                for (var i = 0; i < contactsList.length; i++) {
                    for (var _i = 0, contactsToRemove_1 = contactsToRemove; _i < contactsToRemove_1.length; _i++) {
                        var contactToRemove = contactsToRemove_1[_i];
                        if (contactsList[i].UniqueId === contactToRemove.UniqueId) {
                            contactsList.splice(i, 1);
                        }
                    }
                }
                sessionStorage[_this._sessionStorageKey] = JSON.stringify(contactsList);
            };
            this.getContacts = function () {
                var result = sessionStorage[_this._sessionStorageKey];
                if (result == null) {
                    return null;
                }
                return result;
            };
            this.getCurrentUser = function () {
                if (_this.currentUserContact) {
                    return _this.currentUserContact;
                }
            };
        }
        /**
         *------------------------- Events -----------------------------
         */
        /**
         *------------------------- Methods -----------------------------
         */
        /**
        * Metodo di inizializzazione
        */
        uscContattiSel.prototype.initialize = function () {
            this._sessionStorageKey = this.contentId + "_" + uscContattiSel.SESSION_NAME_SELECTED_CONTACTS;
            this.bindLoaded();
        };
        /**
    * Scateno l'evento di "Load Completed" del controllo
    */
        uscContattiSel.prototype.bindLoaded = function () {
            $("#".concat(this.contentId)).data(this);
            $("#".concat(this.contentId)).triggerHandler(uscContattiSel.LOADED_EVENT);
        };
        uscContattiSel.prototype.clearSessionStorage = function () {
            if (sessionStorage[this._sessionStorageKey] != null) {
                sessionStorage.removeItem(this._sessionStorageKey);
            }
        };
        uscContattiSel.LOADED_EVENT = "onLoaded";
        uscContattiSel.SESSION_NAME_SELECTED_CONTACTS = "SessionContacts";
        return uscContattiSel;
    }());
    return uscContattiSel;
});
//# sourceMappingURL=uscContattiSel.js.map