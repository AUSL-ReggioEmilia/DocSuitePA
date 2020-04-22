/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSTypologyService = require('App/Services/UDS/UDSTypologyService');
import UDSRepositoryService = require('App/Services/UDS/UDSRepositoryService');
import ContainerService = require('App/Services/Commons/ContainerService');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import UDSTypologyModel = require('App/Models/UDS/UDSTypologyModel');
import CategoryFascicleRightModel = require('App/Models/Commons/CategoryFascicleRightModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');
import LocationTypeEnum = require('App/Models/Commons/LocationTypeEnum');
import CategoryFascicleModel = require('App/Models/Commons/CategoryFascicleModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import CategoryFascicleService = require('App/Services/Commons/CategoryFascicleService');
import CategoryFascicleRightsService = require('App/Services/Commons/CategoryFascicleRightsService');
import FascicleType = require('App/Models/Fascicles/FascicleType');

class TbltContainerGes {

    btnConfirmId: string;
    uscNotificationId: string;
    btnSearchId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    grdUDSRepositoriesId: string;
    txtNameId: string;
    idCategory: string;

    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _containerService: ContainerService;
    private _categoryFascicleService: CategoryFascicleService;
    private _categoryFascicleRightsService: CategoryFascicleRightsService;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _grdUDSRepositories: Telerik.Web.UI.RadGrid;
    private _txtName: Telerik.Web.UI.RadTextBox;

    /**
   * Costruttore
   * @param serviceConfigurations
   */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Container");
        this._containerService = new ContainerService(containerConfiguration);
        let categoryFascicleService: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "CategoryFascicle");
        this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
        let categoryFascicleRightsService: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "CategoryFascicleRight");
        this._categoryFascicleRightsService = new CategoryFascicleRightsService(categoryFascicleRightsService);
    }

    /**
   * Inizializzazione classe
   */
    initialize(): void {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicked(this.btnSearch_OnClick);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.pageContentId);
        this._grdUDSRepositories = <Telerik.Web.UI.RadGrid>$find(this.grdUDSRepositoriesId);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this.loadContainers();
    }

    /**
    *------------------------- Events -----------------------------
    */
    private btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        if (!this._grdUDSRepositories.get_selectedItems() || this._grdUDSRepositories.get_selectedItems().length < 1) {
            this.showNotificationException(this.uscNotificationId, null, "Selezionare almeno un contenitore");
            return;
        }
        this._btnConfirm.set_enabled(false);
        this._loadingPanel.show(this.pageContentId);

        this.addContainersToCategory()
            .done(() => {
                let ajaxModel: AjaxModel = <AjaxModel>{};
                ajaxModel.ActionName = 'AddContainers';
                this.closeWindow(ajaxModel);
            }).fail((exception) => {
                let ajaxModel: AjaxModel = <AjaxModel>{};
                ajaxModel.ActionName = 'AddContainers';
                this.closeWindow(ajaxModel);
            });

    }


    private addContainersToCategory(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let udsRepositories = this._grdUDSRepositories.get_selectedItems();
        let categoryFascicleRightModel: CategoryFascicleRightModel;
        this._categoryFascicleRightsService.deleteCategoryFascicleRight;
        this._categoryFascicleService.getAvailableProcedureCategoryFascicleByCategory(+this.idCategory,
            (data: CategoryFascicleModel[]) => {
                if (data) {
                    let categoryFascicle: CategoryFascicleModel = data[0];
                    $.when(udsRepositories.forEach((item) => {
                        if (item) {
                            categoryFascicleRightModel = new CategoryFascicleRightModel();
                            categoryFascicleRightModel.Container = item._dataItem;
                            categoryFascicleRightModel.CategoryFascicle = categoryFascicle;
                            let testo: string;
                            this._categoryFascicleRightsService.insertCategoryFascicleRight(categoryFascicleRightModel, (data: any) => {
                                promise.resolve();
                            });
                        }
                    }));
                }
            },
            (exception: ExceptionDTO) => {
                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
                promise.reject(exception);
            });
        return promise.promise();
    }

    private btnSearch_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        this.loadContainers();
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

    private loadContainers = () => {
        let name = this._txtName.get_value();
        this._containerService.getContainersByNameFascicleRights(name, null, (data: any) => {
            if (!data) return;
            if (data) {
                this.fillTable(data);
            }
            this._loadingPanel.hide(this.pageContentId);
            this._btnSearch.set_enabled(true);
        },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this._btnSearch.set_enabled(false);
                this.showNotificationException(this.uscNotificationId, exception);
            });
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

    private fillTable = (udsRepositories: ContainerModel[]) => {
        this._grdUDSRepositories = <Telerik.Web.UI.RadGrid>$find(this.grdUDSRepositoriesId);
        let grdUDSRepositoriesMasterTableView: Telerik.Web.UI.GridTableView = this._grdUDSRepositories.get_masterTableView();
        grdUDSRepositoriesMasterTableView.set_dataSource(udsRepositories);
        grdUDSRepositoriesMasterTableView.clearSelectedItems();
        grdUDSRepositoriesMasterTableView.dataBind();
        this._loadingPanel.hide(this.pageContentId);
    }
}
export = TbltContainerGes;