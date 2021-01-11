/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSTypologyService = require('App/Services/UDS/UDSTypologyService');
import UDSRepositoryService = require('App/Services/UDS/UDSRepositoryService');
import ContainerService = require('App/Services/Commons/ContainerService');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import UDSTypologyModel = require('App/Models/UDS/UDSTypologyModel');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');
import LocationTypeEnum = require('App/Models/Commons/LocationTypeEnum');

class TbltUDSRepositoriesTypologyGes {

    btnConfirmId: string;
    uscNotificationId: string;
    currentUDSTypologyId: string;
    rdlContainerId: string;
    btnSearchId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    grdUDSRepositoriesId: string;
    txtNameId: string;
    txtAliasId: string;    

    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _udsTypologyService: UDSTypologyService;
    private _udsRepositoryService: UDSRepositoryService;    
    private _containerService: ContainerService;
    private _rdlContainer: Telerik.Web.UI.RadDropDownList;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _grdUDSRepositories: Telerik.Web.UI.RadGrid;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _txtAlias: Telerik.Web.UI.RadTextBox;    

    protected static UDSREPOSITORY_TYPE_NAME = "UDSRepository";
    /**
   * Costruttore
   * @param serviceConfigurations
   */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "UDSTypology");
        this._udsTypologyService = new UDSTypologyService(serviceConfiguration);
        let udsRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, TbltUDSRepositoriesTypologyGes.UDSREPOSITORY_TYPE_NAME);
        this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
        let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Container");
        this._containerService = new ContainerService(containerConfiguration);
    }

     /**
    * Inizializzazione classe
    */
    initialize(): void {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
        this._rdlContainer = <Telerik.Web.UI.RadDropDownList>$find(this.rdlContainerId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicked(this.btnSearch_OnClick);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.pageContentId);
        this._grdUDSRepositories = <Telerik.Web.UI.RadGrid>$find(this.grdUDSRepositoriesId);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtAlias = <Telerik.Web.UI.RadTextBox>$find(this.txtAliasId);
        this.loadContainers();
        this.loadAvailableUDSRepositories();
    }

     /**
     *------------------------- Events -----------------------------
     */
    private btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        if (!this._grdUDSRepositories.get_selectedItems() || this._grdUDSRepositories.get_selectedItems().length<1){
            this.showNotificationException(this.uscNotificationId, null, "Selezionare almeno un archivio");
            return;
        }

        this._btnConfirm.set_enabled(false);
        this._loadingPanel.show(this.pageContentId);        
        let udsRepositories = this._grdUDSRepositories.get_selectedItems();

        let typology = <UDSTypologyModel>JSON.parse(sessionStorage[this.currentUDSTypologyId]);
        udsRepositories.forEach((item) => { if (item) typology.UDSRepositories.push(item._dataItem)});
        
        this._udsTypologyService.updateUDSTypology(typology,
            (data: UDSTypologyModel) => {
                if (data) {
                    let ajaxModel: AjaxModel = <AjaxModel>{};
                    ajaxModel.ActionName = 'AddUDSRepositories';
                    ajaxModel.Value = [JSON.stringify(data)];
                    this.closeWindow(ajaxModel);
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);                
                this._btnConfirm.set_enabled(true);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private btnSearch_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this.loadAvailableUDSRepositories();
    }

    private loadAvailableUDSRepositories = () => {
        let name = this._txtName.get_value();
        let alias = this._txtAlias.get_value();
        let idContainer: string = "";
        if (this._rdlContainer.get_selectedItem()) {
            idContainer = this._rdlContainer.get_selectedItem().get_value();
        }

        this._loadingPanel.show(this.pageContentId);
        this._udsRepositoryService.getAvailableCQRSPublishedUDSRepositories(this.currentUDSTypologyId, name, alias, idContainer,
            (data: any) => {
                if (data) {
                    this.fillTable(data);
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
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
        this._containerService.getContainers(LocationTypeEnum.UDSLocation,(data: any) => {
            if (!data) return;
            let containers: ContainerModel[] = <ContainerModel[]>data;
            this.addContainers(containers, this._rdlContainer);
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

    private fillTable = (udsRepositories: UDSRepositoryModel[]) => {
        this._grdUDSRepositories = <Telerik.Web.UI.RadGrid>$find(this.grdUDSRepositoriesId);
        let grdUDSRepositoriesMasterTableView: Telerik.Web.UI.GridTableView = this._grdUDSRepositories.get_masterTableView();
        grdUDSRepositoriesMasterTableView.set_dataSource(udsRepositories);
        grdUDSRepositoriesMasterTableView.clearSelectedItems();
        grdUDSRepositoriesMasterTableView.dataBind();
        this._loadingPanel.hide(this.pageContentId);        
    }
}
export = TbltUDSRepositoriesTypologyGes;