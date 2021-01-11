/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import ContactModel = require('App/Models/Commons/ContactModel');

declare var ValidatorEnable: any;
class uscContattiSel {
    validationId: string;
    contentId: string;
    currentUserContact: ContactModel;

    public static LOADED_EVENT: string = "onLoaded";

    private _sessionStorageKey: string;
    private static SESSION_NAME_SELECTED_CONTACTS: string = "SessionContacts";

    constructor() {
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
    initialize(): void {
        this._sessionStorageKey = `${this.contentId}_${uscContattiSel.SESSION_NAME_SELECTED_CONTACTS}`;
        this.bindLoaded();
    }

    /**
* Scateno l'evento di "Load Completed" del controllo
*/
    private bindLoaded(): void {
        $("#".concat(this.contentId)).data(this);
        $("#".concat(this.contentId)).triggerHandler(uscContattiSel.LOADED_EVENT);

    }

    enableValidators = (state: boolean) => {
        ValidatorEnable(document.getElementById(this.validationId), state)
    }

    setContacts = (contact: ContactModel) => {
        //set new object if getContacts() returns null
        let contactsList: ContactModel[] = JSON.parse(this.getContacts()) || [] as ContactModel[];
        contactsList.push(contact);
        sessionStorage[this._sessionStorageKey] = JSON.stringify(contactsList);
    }

    deleteContacts = (contactsToRemove: ContactModel[]) => {
        let contactsList: ContactModel[] = JSON.parse(this.getContacts()) || [] as ContactModel[];

        //searching for items to remove (not using findIndex() or filter() for IE compatibility)
        for (let i = 0; i < contactsList.length; i++) {
            for (let contactToRemove of contactsToRemove) {
                if (contactsList[i].UniqueId === contactToRemove.UniqueId) {
                    contactsList.splice(i, 1);
                }
            }
        }

        sessionStorage[this._sessionStorageKey] = JSON.stringify(contactsList);
    }

    getContacts = (): string => {
        let result: string = sessionStorage[this._sessionStorageKey];
        if (result == null) {
            return null;
        }
        return result;
    }

    getCurrentUser = (): ContactModel => {
        if (this.currentUserContact) {
            return this.currentUserContact;
        }
    }

    public clearSessionStorage() {
        if (sessionStorage[this._sessionStorageKey] != null) {
            sessionStorage.removeItem(this._sessionStorageKey);
        }
    }
}

export = uscContattiSel;