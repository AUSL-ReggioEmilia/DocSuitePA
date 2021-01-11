import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import uscErrorNotification = require("UserControl/uscErrorNotification");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import LocationService = require("App/Services/Commons/LocationService");
import LocationViewModel = require("App/ViewModels/Commons/LocationViewModel");
import uscLocationDetails = require("UserControl/uscLocationDetails");

class TbltLocation {
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    uscLocationDetailsId: string;
    rtvLocationId: string;
    actionToolBarId: string;
    filterToolbarId: string;
    locationViewPaneId: string;
    rwInsertId: string;
    rtbLocationNameId: string;
    rtbArchivioProtocolloId: string;
    rtbArchivioDossierId: string;
    rtbArchivioAttiId: string;
    rbConfirmId: string;
    selectedLocationId: string;

    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _uscLocationDetails: uscLocationDetails;
    private _actionToolBar: Telerik.Web.UI.RadToolBar;
    private _filterToolbar: Telerik.Web.UI.RadToolBar;
    private _rtvLocations: Telerik.Web.UI.RadTreeView;
    private _rwInsert: Telerik.Web.UI.RadWindow;
    private _rtbLocationName: Telerik.Web.UI.RadTextBox;
    private _rtbArchivioProtocollo: Telerik.Web.UI.RadTextBox;
    private _rtbArchivioDossier: Telerik.Web.UI.RadTextBox;
    private _rtbArchivioAtti: Telerik.Web.UI.RadTextBox;
    private _rbConfirm: Telerik.Web.UI.RadButton;

    private locationModel: LocationViewModel[];
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _locationService: LocationService;

    private static TOOLBAR_CREATE = "create";
    private static TOOLBAR_MODIFY = "modify";
    private static TOOLBAR_SEARCH = "search";
    private static TOOLBAR_SEARCH_INPUTS = "searchInput";
    private static LOCATION_SERVICE = "Location";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        let locationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltLocation.LOCATION_SERVICE);
        this._locationService = new LocationService(locationConfiguration);

        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxLoadingPanel.show(this.locationViewPaneId);
        this._rtvLocations = <Telerik.Web.UI.RadTreeView>$find(this.rtvLocationId);
        this._rtvLocations.add_nodeClicked(this.rtvLocations_nodeClicked);
        this._uscLocationDetails = <uscLocationDetails>$(`#${this.uscLocationDetailsId}`).data();
        this._filterToolbar = <Telerik.Web.UI.RadToolBar>$find(this.filterToolbarId);
        this._filterToolbar.add_buttonClicked(this.filterToolbar_onClick);
        this._actionToolBar = <Telerik.Web.UI.RadToolBar>$find(this.actionToolBarId);
        this._actionToolBar.add_buttonClicked(this.actionToolBar_onClick);

        this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_CREATE).enable();
        this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_MODIFY).disable();

        this._rwInsert = <Telerik.Web.UI.RadWindow>$find(this.rwInsertId);

        this._rtbLocationName = <Telerik.Web.UI.RadTextBox>$find(this.rtbLocationNameId);
        this._rtbArchivioProtocollo = <Telerik.Web.UI.RadTextBox>$find(this.rtbArchivioProtocolloId);
        this._rtbArchivioDossier = <Telerik.Web.UI.RadTextBox>$find(this.rtbArchivioDossierId);
        this._rtbArchivioAtti = <Telerik.Web.UI.RadTextBox>$find(this.rtbArchivioAttiId);

        this._rbConfirm = <Telerik.Web.UI.RadButton>$find(this.rbConfirmId);
        this._rbConfirm.add_clicked(this.rbConfirmInsert_onCLick);

        this.loadTreeView();
    };

    rbConfirmInsert_onCLick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        if ($("#insertLocation").is(":visible")) {
            return this.insertOrUpdateLocation();
        }
    }

    rtvLocations_nodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let selectedNode = args.get_node();

        if (selectedNode.get_level() === 0) {
            $(`#${this._uscLocationDetails.pnlDetailsId}`).hide();
            this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_CREATE).enable();
            this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_MODIFY).disable();
        } else {
            $(`#${this._uscLocationDetails.pnlDetailsId}`).show();
            this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_CREATE).disable();
            this._actionToolBar.findItemByValue(TbltLocation.TOOLBAR_MODIFY).enable();

            uscLocationDetails.selectedLocationUniqueId = selectedNode.get_value();
            this._uscLocationDetails.setLocationDetails();
        }
    }

    filterToolbar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let name: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>this._filterToolbar.findItemByValue(TbltLocation.TOOLBAR_SEARCH_INPUTS).findControl("txtSearch");
        let archive: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>this._filterToolbar.findItemByValue(TbltLocation.TOOLBAR_SEARCH_INPUTS).findControl("txtSearchArchive");
        switch (args.get_item().get_value()) {
            case TbltLocation.TOOLBAR_SEARCH: {
                this._rtvLocations.get_nodes().getNode(0).get_nodes().clear();
                this.loadTreeViewFiltered(name.get_textBoxValue(), archive.get_textBoxValue());
                break;
            }
        }
    }

    actionToolBar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvLocations.get_selectedNode();
        switch (args.get_item().get_value()) {
            case TbltLocation.TOOLBAR_CREATE: {
                this.clearInputs();
                this.selectedLocationId = "";
                this._rwInsert.show();
                this._rwInsert.set_title("Aggiungi deposito documentale");
                break;
            }
            case TbltLocation.TOOLBAR_MODIFY: {
                this.selectedLocationId = selectedNode.get_value();
                this.populateLocationInputs(this.selectedLocationId);
                this._rwInsert.show();
                this._rwInsert.set_title("Modifica deposito documentale");
                break;
            }
        }
    }

    private loadTreeViewFiltered(name: string, archive: string) {
        this._ajaxLoadingPanel.show(this.locationViewPaneId);
        this._locationService.filterLocationByNameAndArchive(name, archive, (data) => {
            this.locationModel = data;
            for (let location of this.locationModel) {
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_value(location.UniqueId);
                node.set_text(location.Name);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/location.png");
                this._rtvLocations.get_nodes().getNode(0).get_nodes().add(node);
                this._rtvLocations.commitChanges();
            }
            this._rtvLocations.get_nodes().getNode(0).expand();
            this._ajaxLoadingPanel.hide(this.locationViewPaneId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.locationViewPaneId);
            this.showNotificationException(error);
        });
    }

    private loadTreeView(): void {
        this._locationService.getLocations((data) => {
            this.locationModel = data;
            for (let location of this.locationModel) {
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_value(location.UniqueId);
                node.set_text(location.Name);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/location.png");
                this._rtvLocations.get_nodes().getNode(0).get_nodes().add(node);
                this._rtvLocations.commitChanges();
            }
            this._rtvLocations.get_nodes().getNode(0).expand();
            this._ajaxLoadingPanel.hide(this.locationViewPaneId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.locationViewPaneId);
            this.showNotificationException(error);
        });
    }

    private clearInputs() {
        this._rtbLocationName.clear();
        this._rtbArchivioProtocollo.clear();
        this._rtbArchivioDossier.clear();
        this._rtbArchivioAtti.clear();
    }

    private populateLocationInputs(locationId: string) {
        let location: LocationViewModel = <LocationViewModel>{};
        for (let locationToFind of this.locationModel) {
            if (locationToFind.UniqueId === locationId) {
                location = locationToFind;
                break;
            }
        }

        this._rtbLocationName.set_value(location.Name);
        this._rtbArchivioProtocollo.set_value(location.ProtocolArchive);
        this._rtbArchivioDossier.set_value(location.DossierArchive);
        this._rtbArchivioAtti.set_value(location.ResolutionArchive);
    }

    private insertOrUpdateLocation() {
        let exists: boolean = this.selectedLocationId !== "";

        if (this._rtbLocationName.get_value() == "") {
            alert("Il campo nome e obbligatorio");
            return;
        }


        let location: LocationViewModel = <LocationViewModel>{};
        location.Name = this._rtbLocationName.get_value();
        location.ProtocolArchive = this._rtbArchivioProtocollo.get_value();
        location.DossierArchive = this._rtbArchivioDossier.get_value();
        location.ResolutionArchive = this._rtbArchivioAtti.get_value();

        if (exists) {
            this.updateLocation(location);
        }
        else {
            this.insertLocation(location);
        }
    }

    private updateLocation(location: LocationViewModel) {
        this._ajaxLoadingPanel.show(this.rtvLocationId);
        location.UniqueId = this.selectedLocationId;
        this._locationService.update(location, (data) => {
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_value(data.UniqueId);
            node.set_text(data.Name);
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/location.png");
            this._rtvLocations.get_nodes().getNode(0).get_nodes().add(node);
            this._rtvLocations.commitChanges();
            
            let locationModelToUpdate: LocationViewModel = this.locationModel.filter(x => x.UniqueId === data.UniqueId)[0];
            let locationModelToUpdateIndex = this.locationModel.indexOf(locationModelToUpdate);
            this.locationModel[locationModelToUpdateIndex] = data;

            this._rwInsert.close();

            this._rtvLocations.get_selectedNode().set_text(data.Name);
            this._uscLocationDetails.clearLocationDetails();
            this._uscLocationDetails.setLocationDetails();

            this._ajaxLoadingPanel.hide(this.rtvLocationId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvLocationId);
            this.showNotificationException(error);
        });
    }

    private insertLocation(location: LocationViewModel) {
        this._ajaxLoadingPanel.show(this.rtvLocationId);
        this._locationService.insert(location, (data) => {
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_value(data.UniqueId);
            node.set_text(data.Name);
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/location.png");
            this._rtvLocations.get_nodes().getNode(0).get_nodes().add(node);
            this._rtvLocations.commitChanges();

            if (!this.locationModel) {
                this.locationModel = [];
            }

            this.locationModel.push(data);
            this._rwInsert.close();
            this._ajaxLoadingPanel.hide(this.rtvLocationId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvLocationId);
            this.showNotificationException(error);
        });
    }

    private showNotificationException(exception: ExceptionDTO, customMessage?: string): void {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: uscErrorNotification = <uscErrorNotification>$(`#${this.uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(customMessage)
        }
    }

    private showNotificationMessage(customMessage: string): void {
        let uscNotification: uscErrorNotification = <uscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }
}

export = TbltLocation;