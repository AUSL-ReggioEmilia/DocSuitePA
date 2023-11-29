/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ProtocolService = require('App/Services/Protocols/ProtocolService');
import ResolutionService = require('App/Services/Resolutions/ResolutionService');
import UDSRepositoryService = require('App/Services/UDS/UDSRepositoryService');
import UDSService = require('App/Services/UDS/UDSService');
import FascicleFolderService = require('App/Services/Fascicles/FascicleFolderService');
import FascicleDocumentUnitService = require('App/Services/Fascicles/FascicleDocumentUnitService');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import ProtocolModel = require('App/Models/Protocols/ProtocolModel');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import ResolutionModel = require('App/Models/Resolutions/ResolutionModel');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import UDSModel = require('App/Models/UDS/UDSModel');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicolableBaseModel = require('App/Models/Fascicles/FascicolableBaseModel');
import RadGridHelper = require('App/Helpers/RadGridHelper');
import FascicleDocumentUnitModel = require('App/Models/Fascicles/FascicleDocumentUnitModel');
import ReferenceModel = require('App/Models/Fascicles/ReferenceModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import StringHelper = require('App/Helpers/StringHelper');
import FascicleReferenceType = require('App/Models/Fascicles/FascicleReferenceType');
import AssociatedFasciclesViewModel = require('App/ViewModels/Fascicles/AssociatedFasciclesViewModel');
import FascicleBase = require('Fasc/FascBase');
import Environment = require('App/Models/Environment');
import IFascicolableBaseModel = require('App/Models/Fascicles/IFascicolableBaseModel');
import IFascicolableBaseService = require('App/Services/Fascicles/IFascicolableBaseService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import AjaxModel = require('App/Models/AjaxModel');
import FascicolableActionType = require('App/Models/FascicolableActionType');
import CategoryTreeViewModel = require('App/ViewModels/Commons/CategoryTreeViewModel');
import uscFascicleSearch = require('UserControl/uscFascicleSearch');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');

class FascUDManager extends FascicleBase {
    documentUnitUniqueId: string;
    documentUnitRepositoryName: string;
    documentUnitType: number;
    ajaxLoadingPanelId: string;
    radWindowManagerId: string;
    rowContainerId: string;
    pageContentId: string;
    lblUDSelectedId: string;
    lblUDTitleId: string;
    lblDocumentUnitTypeId: string;
    lblContainerId: string;
    lblObjectId: string;
    lblCategoryId: string;
    signalRServerAddress: string;
    rgvAssociatedFasciclesId: string;
    btnInsertId: string;
    btnRemoveId: string;
    btnNewFascicleId: string;
    uscNotificationId: string;
    validationModel: any;
    currentIdUDSRepository: string;
    btnCategoryChangeId: string;
    ajaxManagerId: string;
    managerFascicleLinkId: string;
    availableFascicleRowId: string;
    maxNumberElements: string;
    uscFascicleSearchId: string;
    processEnabled: boolean;
    folderSelectionEnabled: boolean;
    authorizedFasciclesEnabled: boolean;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rgvAssociatedFascicles: Telerik.Web.UI.RadGrid;
    private _btnInsert: Telerik.Web.UI.RadButton;
    private _btnRemove: Telerik.Web.UI.RadButton;
    private _btnNewFascicle: Telerik.Web.UI.RadButton;
    private _btnCategoryChange: Telerik.Web.UI.RadButton;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _lblUDSelected: JQuery;
    private _lblUDTitle: JQuery;
    private _lblContainer: JQuery;
    private _lblObject: JQuery;
    private _lblCategory: JQuery;
    private _lblDocumentUnitType: JQuery;
    private _signalR: DSWSignalR;
    private _currentUD: any;
    private _udservice: any;
    private _fascicleDocumentUnitService: FascicleDocumentUnitService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _domainUserService: DomainUserService;
    private _udsRepositoryService: UDSRepositoryService;
    private _selectedAssociatedFascicle: AssociatedFasciclesViewModel;
    private _managerAddFascLink: Telerik.Web.UI.RadWindowManager;
    private _fascicleFolderService: FascicleFolderService;
    private _uscFascicleSearch: uscFascicleSearch;
    /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
     * Inizializzazione
     */
    initialize(): void {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rgvAssociatedFascicles = <Telerik.Web.UI.RadGrid>$find(this.rgvAssociatedFasciclesId);
        this._btnInsert = <Telerik.Web.UI.RadButton>$find(this.btnInsertId);
        this._btnInsert.add_clicking(this.btnInsert_OnClick);
        this._btnNewFascicle = <Telerik.Web.UI.RadButton>$find(this.btnNewFascicleId);
        this._btnNewFascicle.add_clicking(this.btnNewFascicle_OnClick);
        this._btnRemove = <Telerik.Web.UI.RadButton>$find(this.btnRemoveId);
        this._btnRemove.add_clicking(this.btnRemoveFascicle_OnClick);
        this._btnCategoryChange = <Telerik.Web.UI.RadButton>$find(this.btnCategoryChangeId);
        this._btnCategoryChange.add_clicking(this.btnCategoryChange_OnClick);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._lblUDSelected = $("#".concat(this.lblUDSelectedId));
        this._lblUDTitle = $("#".concat(this.lblUDTitleId));
        this._lblDocumentUnitType = $("#".concat(this.lblDocumentUnitTypeId));
        this._lblContainer = $("#".concat(this.lblContainerId));
        this._lblObject = $("#".concat(this.lblObjectId));
        this._lblCategory = $("#".concat(this.lblCategoryId));
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._uscFascicleSearch = <uscFascicleSearch>$(`#${this.uscFascicleSearchId}`).data();

        try {
            let fascicleDocumentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
            this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitConfiguration);

            let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOMAIN_TYPE_NAME);
            this._domainUserService = new DomainUserService(domainUserConfiguration);

            let fascicleFolderServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLEFOLDER_TYPE_NAME);
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderServiceConfiguration);

            this._btnCategoryChange.set_visible(false);
            this._btnInsert.set_enabled(false);
            this._btnNewFascicle.set_enabled(false);
            this._btnRemove.set_enabled(false);


            switch (<Environment>this.documentUnitType) {
                case Environment.Protocol:
                    let protocolConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.PROTOCOL_TYPE_NAME);
                    this._udservice = new ProtocolService(protocolConfiguration);
                    this.loadProtocol();
                    break;
                case Environment.Resolution:
                    let resolutionConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.RESOLUTION_TYPE_NAME);
                    this._udservice = new ResolutionService(resolutionConfiguration);
                    this.loadResolution();
                    break;
                case Environment.DocumentSeries:
                    break;
                case Environment.UDS:
                    let udsConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, this.documentUnitRepositoryName);
                    let udsRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.UDSREPOSITORY_TYPE_NAME);
                    this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
                    this._udservice = new UDSService(udsConfiguration);
                    this.loadUDSRepository();
                    break;
                default:
                    break;
            }
        }
        catch (error) {
            this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
            console.log(JSON.stringify(error));
        }
    }

    /**
     *------------------------- Events -----------------------------
     */    

    /**
     * Evento scatenato al click del pulsante "Nuovo fascicolo"
     * @param sender
     * @param args
     */
    btnNewFascicle_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this._loadingPanel.show(this.pageContentId);
        this._btnInsert.set_enabled(false);
        this._btnRemove.set_enabled(false);
        if ((<any>this._btnNewFascicle).get_visible()) {
            this._btnNewFascicle.set_enabled(false);
        }
        if ((<any>this._btnCategoryChange).get_visible()) {
            this._btnCategoryChange.set_enabled(false);
        }

        let params = "Titolo=Nuovo Fascicolo&Type=Fasc";
        switch (<Environment>this.documentUnitType) {
            case Environment.Protocol:
                let protocol: ProtocolModel = <ProtocolModel>this._currentUD;
                params = params.concat("&IdCategory=", protocol.Category.EntityShortId.toString(), "&IdDocumentUnit=", protocol.UniqueId, "&Environment=", Environment.Protocol.toString());
                break;
            case Environment.Resolution:
                let resolution: ResolutionModel = <ResolutionModel>this._currentUD;
                params = params.concat("&IdCategory=", resolution.Category.EntityShortId.toString(), "&IdDocumentUnit=", resolution.UniqueId, "&Environment=", Environment.Resolution.toString());
                break;
            case Environment.UDS:
                let UDS: UDSModel = <UDSModel>this._currentUD;
                params = params.concat("&IdCategory=", UDS.Category.EntityShortId.toString(), "&IdDocumentUnit=", UDS.UniqueId, "&Environment=", Environment.UDS.toString(), "&IdUDSRepository=", this.currentIdUDSRepository);
                break;
        }

        let locationPath = "../Fasc/FascInserimento.aspx";
        if (this.processEnabled) {
            locationPath = "../Fasc/FascProcessInserimento.aspx";
        }
        window.location.href = `${locationPath}?${params}`;
    }

    /**
     * Evento scatenato al click del pulsante "Rimuovi"
     * @param sender
     * @param args
     */
    btnRemoveFascicle_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        let dataItems: any = this._rgvAssociatedFascicles.get_selectedItems();
        if (dataItems.length == 0) {
            this.showNotificationMessage(this.uscNotificationId, "Nessun Fascicolo selezionato");
            return;
        }

        this._selectedAssociatedFascicle = <AssociatedFasciclesViewModel>dataItems[0].get_dataItem();

        if (this._selectedAssociatedFascicle.FascicleType == FascicleReferenceType.Fascicle && !this.validationModel.CanManageFascicle) {
            this.showNotificationMessage(this.uscNotificationId, "Non si hanno i diritti di rimozione del Fascicolo selezionato");
            return;
        }

        this._manager.radconfirm("Sei sicuro di voler eliminare il documento dal fascicolo selezionato?", (arg) => {
            if (arg) {
                this.removeFascicleDocumentUnit(this._selectedAssociatedFascicle);
            }
        }, 300, 160);
    }

    /**
     * Evento scatenato al click del pulsante "Fascicola"
     * @param sender
     * @param args
     */
    btnInsert_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);

        let selectedFascicle: FascicleModel;
        let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
        if (!jQuery.isEmptyObject(uscFascicleSearch)) {
            selectedFascicle = uscFascicleSearch.getSelectedFascicle();
        }

        if (!selectedFascicle) {
            this.showNotificationMessage(this.uscNotificationId, "Nessun Fascicolo selezionato");
            return;
        }

        let selectedFascicleFolder: FascicleSummaryFolderViewModel = this._uscFascicleSearch.getSelectedFascicleFolder();

        if (this.folderSelectionEnabled && !selectedFascicleFolder) {
            this.showNotificationMessage(this.uscNotificationId, "Nessuna cartella fascicolo selezionata");
            return;
        }

        this.insertFascicleDocumentUnit(selectedFascicle, selectedFascicleFolder);
    }    


    /**
     * Evento scatenato al click del pulsante "Cambio classificazione"
     * @param sender
     * @param args
     */
    btnCategoryChange_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this._manager.add_close(this.closeChangeCategoryWindow);
        let protocol: ProtocolModel = <ProtocolModel>this._currentUD;
        let url: string = `../UserControl/CommonSelCategoryRest.aspx?Type=Fasc&FascicleBehavioursEnabled=true&FascicleType=1&IncludeParentDescendants=True&ParentId=${protocol.Category.EntityShortId}`;
        this._manager.open(url, "windowInsertProtocollo", null);
        this._manager.center();
    }

    /**
    * Evento di chiusura finestra di Cambio Classificazione
    * @param sender
     * @param args
    */
    closeChangeCategoryWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            this._loadingPanel.show(this.pageContentId);
            this._btnInsert.set_enabled(false);
            this._btnRemove.set_enabled(false);
            if ((<any>this._btnNewFascicle).get_visible()) {
                this._btnNewFascicle.set_enabled(false);
            }
            if ((<any>this._btnCategoryChange).get_visible()) {
                this._btnCategoryChange.set_enabled(false);
            }

            let category: CategoryTreeViewModel = args.get_argument();
            let ajaxModel: AjaxModel = <AjaxModel>{};
            ajaxModel.ActionName = "ChangeCategory";
            ajaxModel.Value = new Array<string>();
            ajaxModel.Value.push(category.IdCategory.toString());
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        }
    }


    /**
    *------------------------- Methods -----------------------------
    */

    /**
     * Metodo che aggiorna la tabella dei fascicoli associati
     * @param data
     */
    refreshAssociatedFascicles = (data: FascicleModel[]) => {
        let models: Array<AssociatedFasciclesViewModel> = new Array<AssociatedFasciclesViewModel>();
        let cmbOpenCloseItems: Telerik.Web.UI.RadComboBoxItem[];
        let isAlreadyFascicolate: boolean = false;
        if (data.length > 0) {
            try {
                $.each(data, (index, fascicle) => {
                    let model: AssociatedFasciclesViewModel;
                    let imageUrl: string = "";
                    let referenceImageUrl: string = "";
                    let refTooltip: string = "";
                    let openCloseTooltip: string = "";
                    if (fascicle.EndDate == null) {
                        imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                        openCloseTooltip = "Fascicolo aperto";
                    } else {
                        imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                        openCloseTooltip = "Fascicolo chiuso";
                    }


                    let referenceType: FascicleReferenceType = FascicleReferenceType.Fascicle;
                    let fascicleUDId: string;
                    let refModel: ReferenceModel;

                    refModel = this.getDocumentUnitUDReferenceModel(fascicle, <Environment>this.documentUnitType);

                    if (refModel != null) {
                        referenceType = refModel.ReferenceType;
                        fascicleUDId = refModel.UniqueId;
                    }

                    referenceImageUrl = (referenceType == FascicleReferenceType.Fascicle) ? "../App_Themes/DocSuite2008/imgset16/folder_document.png" : "../App_Themes/DocSuite2008/imgset16/link.png";
                    refTooltip = (referenceType == FascicleReferenceType.Fascicle) ? "Fascicolato" : "Per riferimento";
                    let tileText: string = fascicle.Title.concat(" ", fascicle.FascicleObject);

                    model = <AssociatedFasciclesViewModel>{
                        Name: tileText,
                        UniqueId: fascicle.UniqueId,
                        ImageUrl: imageUrl,
                        OpenCloseTooltip: openCloseTooltip,
                        ReferenceImageUrl: referenceImageUrl,
                        ReferenceTooltip: refTooltip,
                        FascicleType: referenceType,
                        UDUniqueId: fascicleUDId
                    };
                    models.push(model);
                    if (!isAlreadyFascicolate) {
                        isAlreadyFascicolate = (referenceType == FascicleReferenceType.Fascicle);
                    }
                });
                this._btnInsert.set_enabled(this.validationModel.CanManageFascicle);
                this._btnNewFascicle.set_visible(!isAlreadyFascicolate);
                if ((<any>this._btnNewFascicle).get_visible()) {
                    this._btnNewFascicle.set_enabled(this.validationModel.CanInsertFascicle);
                }
                this._btnRemove.set_enabled(this.validationModel.CanManageFascicle);
                this._btnCategoryChange.set_visible(this.validationModel.CanChangeCategory);
                if ((<any>this._btnCategoryChange).get_visible()) {
                    this._btnCategoryChange.set_enabled(!isAlreadyFascicolate);
                }
            }
            catch (error) {
                this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                console.log(JSON.stringify(error));
                return;
            }
        }
        else {
            this.setButtonEnable();
        }

        $("#".concat(this.availableFascicleRowId)).show();
        if (!isAlreadyFascicolate && !this.validationModel.CanManageFascicle) {
            $("#".concat(this.availableFascicleRowId)).hide();
        }

        let tableView: Telerik.Web.UI.GridTableView = this._rgvAssociatedFascicles.get_masterTableView();

        tableView.clearSelectedItems();
        tableView.set_dataSource(models);
        tableView.dataBind();
    }

    private getDocumentUnitUDReferenceModel(fascicle: FascicleModel, dswEnvironment: Environment): ReferenceModel {
        let protocol: ProtocolModel = <ProtocolModel>this._currentUD;
        let model: ReferenceModel = new ReferenceModel();
        $.each(fascicle.FascicleDocumentUnits, (index: number, fascicleDocumentUnit: FascicleDocumentUnitModel) => {
            if (((dswEnvironment == Environment.UDS && fascicleDocumentUnit.DocumentUnit.Environment >= 100) || (dswEnvironment != Environment.UDS && fascicleDocumentUnit.DocumentUnit.Environment == dswEnvironment))
                && fascicleDocumentUnit.DocumentUnit.UniqueId == protocol.UniqueId) {
                if ($.type(fascicleDocumentUnit.ReferenceType) === "string") {
                    fascicleDocumentUnit.ReferenceType = FascicleReferenceType[fascicleDocumentUnit.ReferenceType.toString()];
                }

                if (fascicleDocumentUnit.ReferenceType == FascicleReferenceType.Fascicle) {
                    this._btnInsert.set_enabled(false);
                    this._btnNewFascicle.set_visible(false);
                    this._btnRemove.set_enabled(this.validationModel.CanManageFascicle);
                    if ((<any>this._btnCategoryChange).get_visible()) {
                        this._btnCategoryChange.set_enabled(false);
                    }
                    let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
                    if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                        uscFascicleSearch.loadFascicle(fascicle.UniqueId);
                    }
                }
                model.ReferenceType = fascicleDocumentUnit.ReferenceType;
                model.UniqueId = fascicleDocumentUnit.UniqueId;
            }
        });
        return model;
    }

    /**
     * Carica da UD di tipo protocollo
     */
    private loadProtocol() {
        this._loadingPanel.show(this.pageContentId);
        (<ProtocolService>this._udservice).getProtocolByUniqueId(this.documentUnitUniqueId,
            (data: any) => {
                if (data == null) {
                    $("#".concat(this.pageContentId)).hide();
                    this._btnRemove.set_enabled(false);
                    this._btnNewFascicle.set_enabled(false);
                    this._loadingPanel.hide(this.pageContentId);
                    return;
                }

                if (!this.validationModel.CanManageFascicle && !this.validationModel.CanChangeCategory) {
                    this.showNotificationMessage(this.uscNotificationId, "Mancano i diritti di gestione dei fascicoli relativi al documento");
                    $("#".concat(this.pageContentId)).hide();
                    this._btnRemove.set_enabled(false);
                    this._btnNewFascicle.set_enabled(false);
                    this._loadingPanel.hide(this.pageContentId);
                    return;
                }

                let model: ProtocolModel = <ProtocolModel>data;
                this._lblUDSelected.html("Protocollo selezionato");
                this._lblDocumentUnitType.html("Protocollo:");
                this._lblUDTitle.html(model.Year.toString().concat("/", String("0000000" + model.Number.toString()).slice(-7), " del ", moment(model.RegistrationDate).format("DD/MM/YYYY")));
                this._lblObject.html(model.Object);
                this._lblCategory.html(model.Category.Name);
                this._lblContainer.html(model.Container.Name);
                this._currentUD = model;

                this.loadUDdata(model, () => {
                    this._loadingPanel.hide(this.pageContentId);
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * Carica da UD di tipo Resolution
     */
    private loadResolution(): void {
        this._loadingPanel.show(this.pageContentId);
        (<ResolutionService>this._udservice).getResolutionByUniqueId(this.documentUnitUniqueId,
            (data: any) => {
                if (data == null) {
                    $("#".concat(this.pageContentId)).hide();
                    this._btnRemove.set_enabled(false);
                    this._btnNewFascicle.set_enabled(false);
                    this._loadingPanel.hide(this.pageContentId);
                    return;
                }

                if (!this.validationModel.CanManageFascicle) {
                    this.showNotificationMessage(this.uscNotificationId, "Mancano i diritti di gestione dei fascicoli relativi al documento");
                    $("#".concat(this.pageContentId)).hide();
                    this._btnRemove.set_enabled(false);
                    this._btnNewFascicle.set_enabled(false);
                    this._loadingPanel.hide(this.pageContentId);
                    return;
                }

                let model: ResolutionModel = <ResolutionModel>data;
                this._lblUDSelected.html("Atto selezionato");
                this._lblDocumentUnitType.html("Atto:");
                this._lblUDTitle.html(model.InclusiveNumber);
                this._lblObject.html(model.Object);
                this._lblCategory.html(model.Category.Name);
                $("#".concat(this.rowContainerId)).hide();
                this._currentUD = model;

                this.loadUDdata(model, () => {
                    this._loadingPanel.hide(this.pageContentId);
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
 * Carica da UD di tipo UDS
 */
    private loadUDSRepository(): void {
        this._loadingPanel.show(this.pageContentId);
        (<UDSService>this._udservice).getUDSByUniqueId(this.documentUnitUniqueId,
            (data: any) => {
                if (data == null) {
                    $("#".concat(this.pageContentId)).hide();
                    this._btnRemove.set_enabled(false);
                    this._btnNewFascicle.set_enabled(false);
                    this._loadingPanel.hide(this.pageContentId);
                    return;
                }

                if (!this.validationModel.CanManageFascicle) {
                    this.showNotificationMessage(this.uscNotificationId, "Mancano i diritti di gestione dei fascicoli relativi al documento");
                    $("#".concat(this.pageContentId)).hide();
                    this._btnRemove.set_enabled(false);
                    this._btnNewFascicle.set_enabled(false);
                    this._loadingPanel.hide(this.pageContentId);
                    return;
                }

                let model: UDSModel = <UDSModel>data;
                this._lblUDSelected.html("Archivio selezionato");
                this._lblDocumentUnitType.html(this.documentUnitRepositoryName.concat(":"));
                this._lblUDTitle.html(model.Year.toString().concat("/", String("0000000" + model.Number.toString()).slice(-7), " del ", moment(model.RegistrationDate).format("DD/MM/YYYY")));
                this._lblObject.html(model.Subject);
                this._lblCategory.html(model.Category.Name);
                $("#".concat(this.rowContainerId)).hide();
                this._currentUD = model;

                this.loadUDdata(model, () => {
                    this._loadingPanel.hide(this.pageContentId);
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
 * Rimuove un riferimento per document unit
 * @param model
 */
    removeFascicleDocumentUnit(model: AssociatedFasciclesViewModel): void {
        let fascicleDocumentUnit: FascicleDocumentUnitModel = new FascicleDocumentUnitModel(model.UniqueId);
        fascicleDocumentUnit.ReferenceType = model.FascicleType;
        fascicleDocumentUnit.UniqueId = model.UDUniqueId;
        let documentUnit: DocumentUnitModel = <DocumentUnitModel>this._currentUD;
        fascicleDocumentUnit.DocumentUnit = documentUnit;
        this.removeFascicleUD(fascicleDocumentUnit, documentUnit, this._fascicleDocumentUnitService);
    }


    /**
     * Rimuove un riferimento per l'UD  corrente
     * @param model
     * @param reference
     * @param service
     */
    private removeFascicleUD(model: IFascicolableBaseModel, reference: any, service: IFascicolableBaseService<IFascicolableBaseModel>): void {
        this._loadingPanel.show(this.pageContentId);
        this._btnInsert.set_enabled(false);
        this._btnRemove.set_enabled(false);
        if ((<any>this._btnNewFascicle).get_visible()) {
            this._btnNewFascicle.set_enabled(false);
        }
        if ((<any>this._btnCategoryChange).get_visible()) {
            this._btnCategoryChange.set_enabled(false);
        }

        service.deleteFascicleUD(model,
            (data: any) => {
                this.loadUDdata(reference, () => {
                    if ((<any>this._btnNewFascicle).get_visible()) {
                        this._btnNewFascicle.set_enabled(this.validationModel.CanInsertFascicle);
                    }
                    this._loadingPanel.hide(this.pageContentId);
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.setButtonEnable();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * Inserisce un riferimento per la documentunit
     * @param selectedFascicle
     **/
    insertFascicleDocumentUnit(selectedFascicle: FascicleModel, selectedFascicleFolder: FascicleSummaryFolderViewModel): void {
        let model: FascicleDocumentUnitModel = new FascicleDocumentUnitModel(selectedFascicle.UniqueId);
        model.ReferenceType = FascicleReferenceType.Fascicle;
        let documentUnit: DocumentUnitModel = <DocumentUnitModel>this._currentUD;
        model.DocumentUnit = documentUnit;

        model.FascicleFolder = {} as FascicleFolderModel;
        model.FascicleFolder.UniqueId = selectedFascicleFolder.UniqueId;

        this.insertFascicleUD(model, documentUnit, this._fascicleDocumentUnitService);
    }

    /**
     * Inserisce un riferimento per la UD selezionata
     * @param model
     * @param reference
     * @param service
     */
    private insertFascicleUD(model: IFascicolableBaseModel, reference: any, service: IFascicolableBaseService<IFascicolableBaseModel>): void {
        this._loadingPanel.show(this.pageContentId);
        this._btnInsert.set_enabled(false);
        this._btnRemove.set_enabled(false);
        this._btnInsert.removeCssClass("rbHovered");
        if ((<any>this._btnNewFascicle).get_visible()) {
            this._btnNewFascicle.set_enabled(false);
        }
        if ((<any>this._btnCategoryChange).get_visible()) {
            this._btnCategoryChange.set_enabled(false);
        }

        service.insertFascicleUD(model, FascicolableActionType.AutomaticDetection,
            (data: any) => {
                this.loadUDdata(reference, () => {
                    if ((<any>this._btnCategoryChange).get_visible()) {
                        this._btnCategoryChange.set_enabled(false);
                    }
                    this._loadingPanel.hide(this.pageContentId);
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.setButtonEnable();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * Inizializza i dati della pagina
     * TODO: da togliere a favore di Signalr
     */
    private loadUDdata(model: any, done?: Function): void {
        if (this.authorizedFasciclesEnabled) {
            this.service.getAuthorizedFasciclesFromDocumentUnit(model.UniqueId,
                (data: FascicleModel[]) => this.loadUDDataSuccessCallback(data, done),
                (exception: ExceptionDTO) => this.loadUDDataErrorCallback(exception, done)
            );
        }
        else {
            this.service.getAssociatedFascicles(model.UniqueId, this.documentUnitType, null,
                (data: FascicleModel[]) => this.loadUDDataSuccessCallback(data, done),
                (exception: ExceptionDTO) => this.loadUDDataErrorCallback(exception, done)
            );
        }
    }

    /**
    * Metodo di callback di cambio classficazione
    * @param categoryName
    * @param validationModel
    */
    changeCategoryCallback(categoryName: string, validationModel: string) {
        this.validationModel = JSON.parse(validationModel);
        this.loadProtocol();
        this._loadingPanel.hide(this.pageContentId);
    }

    /**
    * Metodo di callback di errore
    * @param message
    */
    errorCallback(message: string) {
        if (message) {
            this._loadingPanel.hide(this.pageContentId);
            this.showNotificationMessage(this.uscNotificationId, message);
        }
    }

    /**
    * Metodo abilitare/disabilitare pulsanti in base ai diritti
    */
    setButtonEnable() {
        this._btnInsert.set_enabled(this.validationModel.CanManageFascicle);
        this._btnRemove.set_enabled(this.validationModel.CanManageFascicle);
        if ((<any>this._btnNewFascicle).get_visible()) {
            this._btnNewFascicle.set_enabled(this.validationModel.CanInsertFascicle);
        }
        else {
            this._btnNewFascicle.set_visible(this.validationModel.CanInsertFascicle);
        }
        this._btnCategoryChange.set_visible(this.validationModel.CanChangeCategory);
        if ((<any>this._btnCategoryChange).get_visible()) {
            this._btnCategoryChange.set_enabled(true);
        }
    }

    loadUDDataSuccessCallback(data: FascicleModel[], done?: Function) {
        let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
        if (!jQuery.isEmptyObject(uscFascicleSearch)) {
            uscFascicleSearch.clearSelections();
        }

        this.refreshAssociatedFascicles(data);
        if (done) {
            done();
        }
    }

    loadUDDataErrorCallback(exception: ExceptionDTO, done?: Function) {
        this.showNotificationMessage(this.uscNotificationId, 'Errore nella richiesta. Non è stato possibile recuperare i Fascicoli associati.');
        this.setButtonEnable();
        if (done) {
            done();
        }
    }
}

export = FascUDManager;
