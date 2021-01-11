/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import uscContactRest = require("UserControl/uscContactREST");
import ContactModel = require("App/Models/Commons/ContactModel");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import uscErrorNotification = require("UserControl/uscErrorNotification");
import ContactDSWModelMapper = require("App/Mappers/Commons/ContactDSWModelMapper");
import ContactDSWModel = require("App/Models/Commons/ContactDSWModel");

class CommonSelContactRest {
    btnConfirmId: string;
    btnConfirmAndNewId: string;
    uscContactRestId: string;
    uscNotificationId: string;
    callerId: string;
    ajaxFlatLoadingPanelId: string;
    pnlButtonsId: string;
    filterByParentId?: number;

    private _serviceConfigurations: ServiceConfiguration[];
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _btnConfirmAndNew: Telerik.Web.UI.RadButton;
    private _ajaxFlatLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;    
    private readonly _contactDSWModelMapper: ContactDSWModelMapper; 

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._contactDSWModelMapper = new ContactDSWModelMapper();
    }

    /**
    *------------------------- Events -----------------------------
    */

    confirm_onClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._ajaxFlatLoadingPanel.show(this.pnlButtonsId);
        this.createContact()
            .done((data: [string, ContactModel]) => {
                this.returnValue(data[1], data[0], (sender.get_id() == this.btnConfirmId));
            })
            .fail((exception: (string|ExceptionDTO)) => this.showNotificationException(exception))
            .always(() => this._ajaxFlatLoadingPanel.hide(this.pnlButtonsId));
    }

    /**
    *------------------------- Methods -----------------------------
    */

    initialize(): void {
        this._btnConfirm = $find(this.btnConfirmId) as Telerik.Web.UI.RadButton;
        this._btnConfirm.add_clicked(this.confirm_onClicked);
        this._btnConfirmAndNew = $find(this.btnConfirmAndNewId) as Telerik.Web.UI.RadButton;
        this._btnConfirmAndNew.add_clicked(this.confirm_onClicked);
        this._ajaxFlatLoadingPanel = $find(this.ajaxFlatLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
    }

    private createContact(): JQueryPromise<[string, ContactModel]> {
        let promise: JQueryDeferred<[string, ContactModel]> = $.Deferred<[string, ContactModel]>();
        let uscContactRest: uscContactRest = <uscContactRest>$(`#${this.uscContactRestId}`).data();
        if (!jQuery.isEmptyObject(uscContactRest)) {
            uscContactRest.createContact()
                .done((data: [string, ContactModel]) => promise.resolve(data))
                .fail((exception: (string|ExceptionDTO)) => promise.reject(exception));
        }
        return promise.promise();
    }

    getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    private returnValue(contact: ContactModel, contactType: string, toClose: boolean): void {
        let contactDswModel: ContactDSWModel = this._contactDSWModelMapper.Map(contact);
        let returnModel: any = { contactType: contactType, contact: JSON.stringify(contactDswModel) };
        if (toClose) {
            this.getRadWindow().close(returnModel);
        } else {
            this.getRadWindow().BrowserWindow[`${this.callerId}_UpdateSmart`](returnModel.contactType, returnModel.contact);
            let uscContactRest: uscContactRest = <uscContactRest>$(`#${this.uscContactRestId}`).data();
            if (!jQuery.isEmptyObject(uscContactRest)) {
                uscContactRest.clear();
            }
        }
    }

    protected showNotificationException(exception: (string|ExceptionDTO)) {
        let uscNotification: uscErrorNotification = <uscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
                return;
            }
            uscNotification.showWarningMessage(exception);
        }
    }
}

export = CommonSelContactRest;