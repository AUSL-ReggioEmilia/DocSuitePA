/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSTypologyService = require('App/Services/UDS/UDSTypologyService');
import UDSTypologyModel = require('App/Models/UDS/UDSTypologyModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');

class TbltUDSTypologyGes {

    btnConfirmId: string;
    txtNameId: string;
    txtOldNameId: string;
    rowOldNameId: string;
    uscNotificationId: string;
    action: string;
    currentUDSTypologyId: string;

    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _txtOldName: Telerik.Web.UI.RadTextBox;
    private _udsTypologyService: UDSTypologyService;
    private _currentUDSTypologyModel: UDSTypologyModel;    

    /**
   * Costruttore
   * @param serviceConfigurations
   */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "UDSTypology");
        this._udsTypologyService = new UDSTypologyService(serviceConfiguration);
    }

     /**
    * Inizializzazione classe
    */
    initialize(): void {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtOldName = <Telerik.Web.UI.RadTextBox>$find(this.txtOldNameId);
        this._btnConfirm.add_clicked(this.btnConfirm_OnClick);

        this._txtOldName.set_visible(false);
        $("#".concat(this.rowOldNameId)).hide();

        if (this.action === "Edit") {
            $("#".concat(this.rowOldNameId)).show();
            this._txtOldName.set_visible(true);
            this._currentUDSTypologyModel = <UDSTypologyModel>JSON.parse(sessionStorage[this.currentUDSTypologyId]);
            this._txtOldName.set_value(this._currentUDSTypologyModel.Name);
            this._txtOldName.set_textBoxValue(this._currentUDSTypologyModel.Name);
        }
    }

     /**
     *------------------------- Events -----------------------------
     */
    private btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        if (!this._txtName.get_value()) {
            return;
        }

        switch (this.action) {
            case "Add":
                let typology: UDSTypologyModel = new UDSTypologyModel();
                typology.Name = this._txtName.get_value();

                this._udsTypologyService.insertUDSTypology(typology,
                    (data: UDSTypologyModel) => {
                        if (data) {
                            let ajaxModel: AjaxModel = <AjaxModel>{};
                            ajaxModel.ActionName = 'Add';
                            ajaxModel.Value = [JSON.stringify(data)];
                            this.closeWindow(ajaxModel);
                        }
                    },
                    (exception: ExceptionDTO) => {
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
                break;
            case "Edit":
                this._currentUDSTypologyModel.Name = this._txtName.get_value();
                this._udsTypologyService.updateUDSTypology(this._currentUDSTypologyModel,
                    (data: UDSTypologyModel) => {
                        if (data) {
                            let ajaxModel: AjaxModel = <AjaxModel>{};
                            ajaxModel.ActionName = 'Edit';
                            ajaxModel.Value = [JSON.stringify(data)];
                            this.closeWindow(ajaxModel);
                        }
                    },
                    (exception: ExceptionDTO) => {
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
                break;
        }        
    }

    /**
     *------------------------- Methods -----------------------------
     */
    private showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception && exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(customMessage);
            }
        }
    }

    protected getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    private closeWindow(message?: AjaxModel): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(message);
    }
}
export = TbltUDSTypologyGes;