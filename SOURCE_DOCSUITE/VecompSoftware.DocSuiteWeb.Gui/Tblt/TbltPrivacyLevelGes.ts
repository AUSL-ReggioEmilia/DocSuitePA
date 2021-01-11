/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import PrivacyLevelService = require('App/Services/Commons/PrivacyLevelService');
import PrivacyLevelModel = require('App/Models/Commons/PrivacyLevelModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');

declare var Page_IsValid: any;
class TbltPrivacyLevelGes {

    btnConfirmId: string;
    txtLevelId: string;
    txtDescriptionId: string;
    uscNotificationId: string;
    action: string;
    currentPrivacyLevelId: string;
    rcpColorId: string;

    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _txtLevel: Telerik.Web.UI.RadTextBox;
    private _txtDescription: Telerik.Web.UI.RadTextBox;
    private _rcpColor: Telerik.Web.UI.RadColorPicker;
    private _privacyLevelService: PrivacyLevelService;
    private _currentPrivacyLevelModel: PrivacyLevelModel;

    /**
   * Costruttore
   * @param serviceConfigurations
   */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "PrivacyLevel");
        this._privacyLevelService = new PrivacyLevelService(serviceConfiguration);
    }

    /**
   * Inizializzazione classe
   */
    initialize(): void {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._txtLevel = <Telerik.Web.UI.RadTextBox>$find(this.txtLevelId);
        this._txtDescription = <Telerik.Web.UI.RadTextBox>$find(this.txtDescriptionId);      
        this._rcpColor = <Telerik.Web.UI.RadColorPicker>$find(this.rcpColorId);   
        this._btnConfirm.add_clicked(this.btnConfirm_OnClick);

        if (this.action === "Edit") {
            this._currentPrivacyLevelModel = <PrivacyLevelModel>JSON.parse(sessionStorage[this.currentPrivacyLevelId]);
            let descr: string = this._currentPrivacyLevelModel.Description ? this._currentPrivacyLevelModel.Description : '';
            this._txtLevel.set_value(this._currentPrivacyLevelModel.Level.toString());
            this._txtLevel.set_textBoxValue(this._currentPrivacyLevelModel.Level.toString());
            this._txtDescription.set_value(this._currentPrivacyLevelModel.Description);
            this._txtDescription.set_textBoxValue(this._currentPrivacyLevelModel.Description);
            this._rcpColor.set_selectedColor(this._currentPrivacyLevelModel.Colour, false);
        }
    }

    /**
    *------------------------- Events -----------------------------
    */
    private btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {

        if (!Page_IsValid) {
            return;
        }

        let levelValue: number = Number(this._txtLevel.get_value());
        if (isNaN(levelValue)) {
            this.showNotificationException(this.uscNotificationId, null, "Il livello dev'essere un valore numerico");
            return;
        }

        switch (this.action) {
            case "Add":
                let level: PrivacyLevelModel = new PrivacyLevelModel();
                level.Level = levelValue;
                level.Description = this._txtDescription.get_value();
                if (this._rcpColor.get_selectedColor() && this._rcpColor.get_selectedColor().toLowerCase() != '#ffffff') {
                    level.Colour = this._rcpColor.get_selectedColor();
                }                
                level.IsActive = true;
                this._privacyLevelService.insertPrivacyLevel(level,
                    (data: PrivacyLevelModel) => {
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
                this._currentPrivacyLevelModel.Level = levelValue;
                this._currentPrivacyLevelModel.Description = this._txtDescription.get_value();   
                if (this._rcpColor.get_selectedColor() && this._rcpColor.get_selectedColor().toLowerCase() != '#ffffff') {
                    this._currentPrivacyLevelModel.Colour = this._rcpColor.get_selectedColor();
                } 
                this._privacyLevelService.updatePrivacyLevel(this._currentPrivacyLevelModel,
                    (data: PrivacyLevelModel) => {
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
export = TbltPrivacyLevelGes;