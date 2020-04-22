/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import DossierService = require('App/Services/Dossiers/DossierService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import DossierModel = require('App/Models/Dossiers/DossierModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import DossierRoleModel = require('App/Models/Dossiers/DossierRoleModel');
import DossierDocumentModel = require('App/Models/Dossiers/DossierDocumentModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');

abstract class DossierBase {
    protected static DOSSIER_TYPE_NAME = "Dossier";
    protected static DOSSIERFOLDER_TYPE_NAME = "DossierFolder";
    protected static DOSSIERDOCUMENT_TYPE_NAME = "DossierDocument";
    protected static DOSSIERLOG_TYPE_NAME = "DossierLog";
    protected static FASCICLE_TYPE_NAME = "Fascicle";
    protected static ROLE_TYPE_NAME = "Role";

    private _serviceConfiguration: ServiceConfiguration;
    protected service: DossierService;

    constructor(serviceConfiguration: ServiceConfiguration) {
        this._serviceConfiguration = serviceConfiguration;
    }

    initialize() {
        this.service = new DossierService(this._serviceConfiguration);
    }

    protected addContainers(containers: ContainerModel[], rdlContainer: Telerik.Web.UI.RadDropDownList) {
        let item: Telerik.Web.UI.DropDownListItem;
        for (let container of containers) {
            item = new Telerik.Web.UI.DropDownListItem();
            item.set_text(container.Name);
            item.set_value(container.EntityShortId.toString());

            rdlContainer.get_items().add(item);

            if (!container.isActive) {
                $(item.get_element()).attr('style', 'color: grey');
            }
        }
        rdlContainer.trackChanges();
    }

    protected fillContacts(contact: string, dossierModel: DossierModel): DossierModel {
        if (!!contact) {
            let contactModel: ContactModel;
            let contacts = new Array<ContactModel>();
            let contactId: number[] = JSON.parse(contact);
            for (let id of contactId) {
                contactModel = <ContactModel>{};
                contactModel.EntityId = id;
                contacts.push(contactModel);
            }
            dossierModel.Contacts = contacts;
        }
        return dossierModel
    }

    protected fillRoles(role: string, dossierModel: DossierModel): DossierModel {
        if (!!role) {
            let roleModel: RoleModel;
            let dossierRoleModel: DossierRoleModel = <DossierRoleModel>{};
            let dossierRoles = new Array<DossierRoleModel>();
            let roleId: number[] = JSON.parse(role);
            for (let id of roleId) {
                roleModel = <RoleModel>{}
                roleModel.EntityShortId = id;
                dossierRoleModel.Role = roleModel;
                dossierRoles.push(dossierRoleModel);
            }
            dossierModel.DossierRoles = dossierRoles;
        }
        return dossierModel
    }

    protected fillInserts(archiveChain: string, dossierModel: DossierModel): DossierModel {
        if (archiveChain && archiveChain != "00000000-0000-0000-0000-000000000000") {
            let dossierDocumentModel: DossierDocumentModel = <DossierDocumentModel>{};
            let dossierDocuments = new Array<DossierDocumentModel>();
            dossierDocumentModel.ChainType = ChainType.Miscellanea;
            dossierDocumentModel.IdArchiveChain = archiveChain;
            dossierDocuments.push(dossierDocumentModel);
            dossierModel.DossierDocuments = dossierDocuments;
        }
        return dossierModel
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {        
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    /**
    * Recupera una RadWindow dalla pagina
    */
    protected getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }


    /**
    * Chiude la RadWindow
    */
    protected closeWindow(message?: AjaxModel): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(message);
    }

}

export = DossierBase;