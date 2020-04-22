/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import CategoryFascicleService = require('App/Services/Commons/CategoryFascicleService')
import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import CategoryFascicleModel = require('App/Models/Commons/CategoryFascicleModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import FasciclePeriod = require('App/Models/Commons/FasciclePeriod');
import CategoryFascicleViewModel = require('App/ViewModels/Commons/CategoryFascicleViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');
import CategoryFascicleRightsService = require('App/Services/Commons/CategoryFascicleRightsService');
import CategoryFascicleRightModel = require('App/Models/Commons/CategoryFascicleRightModel');
import RoleModel = require('../App/Models/Commons/RoleModel');

class TbltCreaFascicolo {

    ddlPeriodsId: string;
    radBtnProcedimentoId: string;
    radBtnPeriodicoId: string;
    ddlUDSId: string;
    ajaxManagerId: string;
    btnSaveId: string;
    rowPeriodId: string;
    ajaxLoadingPanelId: string;
    pageLayoutId: string;
    uscNotificationId: string;
    environment: number;
    idCategory: string;
    fascicleContainerEnabled: boolean;

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _btnSave: Telerik.Web.UI.RadButton;
    private _serviceConfigurations: ServiceConfiguration[];
    private _categoryFascicleService: CategoryFascicleService;
    private _ddlPeriods: Telerik.Web.UI.RadComboBox;
    private _ddlUDSs: Telerik.Web.UI.RadComboBox;
    private _rowPeriod: JQuery;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _categoryFascicleRightService: CategoryFascicleRightsService;

    /**
    * Costruttore
    * @param webApiConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }


    /**
     *------------------------- Events -----------------------------
     */


    /**
     * Inizializza la classe
     */
    initialize() {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnSave = <Telerik.Web.UI.RadButton>$find(this.btnSaveId);
        this._btnSave.add_clicking(this.btnSave_OnClick);
        this._ddlPeriods = <Telerik.Web.UI.RadComboBox>$find(this.ddlPeriodsId);
        this._ddlUDSs = <Telerik.Web.UI.RadComboBox>$find(this.ddlUDSId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rowPeriod = $("#".concat(this.rowPeriodId));
        this._rowPeriod.show();
        let categoryFascicleService: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicle");
        this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
        let categoryFascicleRightService: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicleRight");
        this._categoryFascicleRightService = new CategoryFascicleRightsService(categoryFascicleRightService);
    }

    btnSave_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        this._loadingPanel.show(this.pageLayoutId);
        let item = <CategoryFascicleModel>{};
        item.FascicleType = FascicleType['Period'];
        item.DSWEnvironment = parseInt(this._ddlUDSs.get_selectedItem().get_value());

        let fasciclePeriod = <FasciclePeriod>{};
        fasciclePeriod.UniqueId = this._ddlPeriods.get_selectedItem().get_value();
        item.FasciclePeriod = fasciclePeriod;

        let category = <CategoryModel>{};
        category.EntityShortId = Number(this.idCategory);
        item.Category = category;

        this._categoryFascicleService.insertCategoryFascicle(item,
            (data: any) => {
                let insertedCategoryFascicle: CategoryFascicleModel = data;
                this._loadingPanel.hide(this.pageLayoutId);
                this.closeWindow(JSON.stringify(insertedCategoryFascicle));
            },
            (exception: ExceptionDTO) => {
                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            });
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Recupera una RadWindow dalla pagina
     */
    getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    /**
     * Chiude la RadWindow
     */
    closeWindow(idCategory: string): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(idCategory);
    }

}

export = TbltCreaFascicolo;