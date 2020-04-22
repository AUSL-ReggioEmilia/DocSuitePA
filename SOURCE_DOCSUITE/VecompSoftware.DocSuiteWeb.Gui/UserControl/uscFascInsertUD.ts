/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import DocumentUnitService = require('App/Services/DocumentUnits/DocumentUnitService');
import FascicleDocumentUnitService = require('App/Services/Fascicles/FascicleDocumentUnitService');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import FascicleFoundViewModel = require('App/ViewModels/Fascicles/FascicleFoundViewModel');
import ResolutionModel = require('App/Models/Resolutions/ResolutionModel');
import ProtocolModel = require('App/Models/Protocols/ProtocolModel');
import Environment = require('App/Models/Environment');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleBase = require('Fasc/FascBase');
import IFascicolableBaseModel = require('App/Models/Fascicles/IFascicolableBaseModel');
import IFascicolableBaseService = require('App/Services/Fascicles/IFascicolableBaseService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import FascicleFinderViewModel = require('App/ViewModels/Fascicles/FascicleFinderViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContainerService = require('App/Services/Commons/ContainerService');
import FascicolableActionType = require("App/Models/FascicolableActionType");
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import LocationTypeEnum = require('App/Models/Commons/LocationTypeEnum');
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import FascicleDocumentUnitModel = require('App/Models/Fascicles/FascicleDocumentUnitModel');


declare var Page_IsValid: any;
class uscFascInsertUD extends FascicleBase {
    currentFascicleId: string;
    panelFilterId: string;
    labelTitoloId: string;
    txtYearId: string;
    ajaxLoadingPanelId: string;
    txtNumberId: string;
    btnReferenceId: string;
    btnSearchId: string;
    pageContentId: string;
    ajaxManagerId: string;
    btnUDLinkId: string;
    lblUDObjectId: string;
    lblCategoryId: string;
    lblUDTypeId: string;
    rowResolutionTypeId: string;
    grdUdDocSelectedId: string;
    rcbUDDocId: string;
    txtSubjectId: string;
    treeCategoryId: string;
    uscNotificationId: string;
    rcbContainersId: string;
    maxNumberElements: string;
    radWindowManagerId: string;
    chbCategoryChildId: string;
    idFascicleFolder: string;
    miscellaneaLocationEnabled: boolean;

    private _panelFilter: JQuery;
    private _lblUDObject: JQuery;
    private _lblCategory: JQuery;
    private _lblUDType: JQuery;
    private _chbCategoryChild: HTMLInputElement;
    private _btnUDLink: Telerik.Web.UI.RadButton;
    private _txtYear: Telerik.Web.UI.RadNumericTextBox;
    private _txtNumber: Telerik.Web.UI.RadTextBox;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnReference: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _grdUdDocSelected: Telerik.Web.UI.RadGrid;
    private _rcbUDDoc: Telerik.Web.UI.RadComboBox;
    private _txtSubject: Telerik.Web.UI.RadTextBox;
    private _treeCategory: Telerik.Web.UI.RadTreeView;
    private _rcbContainers: Telerik.Web.UI.RadComboBox;
    private _radWindowManager: Telerik.Web.UI.RadWindowManager;
    private _fascicleModel: FascicleModel;
    private _containerService: ContainerService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _documentUnitService: DocumentUnitService;
    private _documentUnitsFound: DocumentUnitModel[];
    private _fascicleDocumentUnitService: FascicleDocumentUnitService;

    private get DocumentUnitRedirectUrls(): Array<[Environment, (documentUnit: DocumentUnitModel) => string]> {
        let items: Array<[Environment, (documentUnit: DocumentUnitModel) => string]> = [
            [Environment.Protocol, (d) => `../Prot/ProtVisualizza.aspx?Year=${d.Year.toString()}&Number=${d.Number.toString()}&Type=Prot`],
            [Environment.Resolution, (d) => `../Resl/ReslVisualizza.aspx?IdResolution=${d.EntityId.toString()}&Type=Resl`],
            [Environment.DocumentSeries, (d) => `../Series/Item.aspx?IdDocumentSeriesItem=${d.EntityId.toString()}&Action=View&Type=Series`],
            [Environment.UDS, (d) => `../UDS/UDSView.aspx?Type=UDS&IdUDS=${d.UniqueId.toString()}&IdUDSRepository=${d.UDSRepository.UniqueId.toString()}`]
        ];
        return items;
    }

    private get DocumentUnitInsertActions(): Array<[Environment, (documentUnit: DocumentUnitModel) => JQueryPromise<void>]> {
        let items: Array<[Environment, (documentUnit: DocumentUnitModel) => JQueryPromise<void>]> = [
            [Environment.Protocol, (d) => this.insertFascicleDocumentUnit(d)],
            [Environment.Resolution, (d) => this.insertFascicleDocumentUnit(d)],
            [Environment.UDS, (d) => this.insertFascicleDocumentUnit(d)]
        ];
        return items;
    }

    /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(function () {
        });
    }

    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();
        this._panelFilter = <JQuery>$("#".concat(this.panelFilterId));
        this._lblUDObject = <JQuery>$("#".concat(this.lblUDObjectId));
        this._lblCategory = <JQuery>$("#".concat(this.lblCategoryId));
        this._lblUDType = <JQuery>$("#".concat(this.lblUDTypeId));
        this._chbCategoryChild = <HTMLInputElement>($("#".concat(this.chbCategoryChildId))[0]);
        this._btnUDLink = <Telerik.Web.UI.RadButton>$find(this.btnUDLinkId);;
        this._btnReference = <Telerik.Web.UI.RadButton>$find(this.btnReferenceId);
        this._btnReference.add_clicking(this.btnReference_OnClick);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._btnReference.set_enabled(false);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_OnClick);
        this._txtNumber = <Telerik.Web.UI.RadTextBox>$find(this.txtNumberId);
        this._txtYear = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtYearId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._radWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._grdUdDocSelected = <Telerik.Web.UI.RadGrid>$find(this.grdUdDocSelectedId);
        this._txtSubject = <Telerik.Web.UI.RadTextBox>$find(this.txtSubjectId);
        this._treeCategory = <Telerik.Web.UI.RadTreeView>$find(this.treeCategoryId);
        this._grdUdDocSelected.add_rowSelected(this.grdUdDocSelected_OnRowSelectChanged);
        this._grdUdDocSelected.add_rowDeselected(this.grdUdDocSelected_OnRowSelectChanged);
        this._rcbUDDoc = <Telerik.Web.UI.RadComboBox>$find(this.rcbUDDocId);
        this._rcbUDDoc.add_selectedIndexChanged(this.rcbUDDoc_OnSelectedIndexChanged);
        this._rcbContainers = <Telerik.Web.UI.RadComboBox>$find(this.rcbContainersId);
        this._rcbContainers.add_selectedIndexChanged(this.rcbContainers_OnSelectedIndexChanged);
        this._rcbContainers.add_itemsRequested(this.rcbContainers_OnClientItemsRequested);

        let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
        this._containerService = new ContainerService(containerConfiguration);

        let scrollContainer: JQuery = $(this._rcbContainers.get_dropDownElement()).find('div.rcbScroll');
        $(scrollContainer).scroll(this.rcbOtherFascicles_onScroll);

        $("#".concat(this.rowResolutionTypeId)).hide();

        let documentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOCUMENT_UNIT_TYPE_NAME);
        this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);

        let fascicleDocumentUnitService: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
        this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitService);

        if (this.currentFascicleId == "") {
            this._panelFilter.hide();
        }

        this._loadingPanel.show(this.pageContentId);
        $("#panelContent").hide();

        this.service.getFascicle(this.currentFascicleId,
            (data: FascicleModel) => {
                if (data == null) return;
                this._fascicleModel = data;
                $("#".concat(this.labelTitoloId)).text("Fascicolo: ".concat(" (", this._fascicleModel.Title, ")"));

                this.enableButtons(false);
                this._panelFilter.show();
                this._txtYear.set_value(this._fascicleModel.Year.toString());
                this._txtNumber.focus();
                this._loadingPanel.hide(this.pageContentId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato alla selezione/deselezione di un record della griglia
     * @param sender
     * @param args
     */
    grdUdDocSelected_OnRowSelectChanged = (sender: Telerik.Web.UI.RadGrid, args: Telerik.Web.UI.GridDataItemEventArgs) => {
        this.enableButtons(this._grdUdDocSelected.get_selectedItems().length > 0);
    }

    /**
     * Evento scatenato al click del pulsante Inserisci
     * @param sender
     * @param args
     */
    btnReference_OnClick = (sender: any, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (!Page_IsValid) {
            return;
        }

        this.enableButtons(false);
        this.addFascicleUD();
    }

    /**
     * Evento scatenato al click del pulsante di cerca UD
     * @param sender
     * @param args
     */
    btnSearch_OnClick = (sender: any, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (!Page_IsValid) {
            return;
        }

        if (this._grdUdDocSelected.get_selectedItems().length > 0) {
            this._grdUdDocSelected.clearSelectedItems();
        }
        this.enableButtons(false);

        $("#panelContent").hide();

        this.findDocumentUnits(0);
    }

    onPageChanged() {
        let masterTable: Telerik.Web.UI.GridTableView = this._grdUdDocSelected.get_masterTableView();
        let skip = masterTable.get_currentPageIndex() * masterTable.get_pageSize();
        this.findDocumentUnits(skip);
    }


    /**
    * Evento al cambio di classificatore
    */
    onCategoryChanged = (idCategory: number) => {
        if (idCategory != 0) {
            $("#".concat(this.chbCategoryChildId)).show();
            $("label[for=".concat(this.chbCategoryChildId, "]")).show();
        }

        if (idCategory == 0) {
            $("#".concat(this.chbCategoryChildId)).hide();
            $("label[for=".concat(this.chbCategoryChildId, "]")).hide();
        }
    }


    /**
     *------------------------- Methods -----------------------------
     */



    /**
     * Metodo che esegue l'inserimento della UD selezionata
     * @param model
     */
    addFascicleUD(): void {
        let selectedDocumentUnits: DocumentUnitModel[] = this.getSelectedDocumentUnits();
        if (selectedDocumentUnits.length == 0) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare un documento per l'inserimento.");
            return;
        }

        let deferredActions: JQueryPromise<void>[] = [];
        let errorMessages: string[] = [];
        for (let documentUnit of selectedDocumentUnits) {
            const deferredInsertAction = () => {
                let promise = $.Deferred<void>();
                this.insertFascicleDocumentUnit(documentUnit)
                    .fail((exception: ExceptionDTO) => {
                        if (exception) {
                            if (exception instanceof ValidationExceptionDTO) {
                                for (let validationMessage of exception.validationMessages) {
                                    errorMessages.push(`Il documento ${documentUnit.Title} ha restituito il seguente errore: ${validationMessage.message}`);
                                }
                            } else {
                                errorMessages.push(`Il documento ${documentUnit.Title} ha restituito il seguente errore: ${exception.statusText}`);
                            }
                        }
                    })
                    .always(() => promise.resolve());
                return promise.promise();
            };
            deferredActions.push(deferredInsertAction());
        }

        this._loadingPanel.show(this.pageContentId);
        $.when.apply(null, deferredActions)
            .then(() => {
                if (errorMessages && errorMessages.length > 0) {
                    this.enableButtons(true);
                    let validationException: ValidationExceptionDTO = new ValidationExceptionDTO();
                    validationException.statusText = `Non tutti i documenti selezionati sono stati inseriti correttamente. Per maggiori informazioni verificare i messaggi di errore ottenuti ed eventualmente conttattare l'assistenza.`;
                    validationException.validationMessages = [];
                    let validationMessage: ValidationMessageDTO;
                    for (let errorMessage of errorMessages) {
                        validationMessage = new ValidationMessageDTO();
                        validationMessage.message = errorMessage;
                        validationException.validationMessages.push(validationMessage);
                    }
                    this.showNotificationException(this.uscNotificationId, validationException);
                } else {
                    this.closeWindow();
                }
            })
            .always(() => this._loadingPanel.hide(this.pageContentId));
    }

    getSelectedDocumentUnits(): DocumentUnitModel[] {
        let masterTable: Telerik.Web.UI.GridTableView = this._grdUdDocSelected.get_masterTableView();
        let selectedItems: Telerik.Web.UI.GridDataItem[] = <Telerik.Web.UI.GridDataItem[]>masterTable.get_selectedItems();
        let selectedIds: string[] = selectedItems.map((item: Telerik.Web.UI.GridDataItem) => item.get_dataItem().UniqueId);
        return this._documentUnitsFound.filter((item: DocumentUnitModel) => selectedIds.some(e => e == item.UniqueId));
    }

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
    closeWindow(): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(true);
    }

    /**
     * Apre una nuova nuova RadWindow
     * @param url
     * @param name
     * @param width
     * @param height
     */
    openWindow(url, width, height): boolean {
        let wnd: Telerik.Web.UI.RadWindow = this._radWindowManager.open(url, null, null);
        wnd.set_modal(true);
        wnd.setSize(width, height);
        wnd.center();
        return false;
    }

    /**
     * Recupera l'url per la visualizzazione della UD
     * @param documentUnit
     */
    getDocumentUnitUrl(documentUnit: DocumentUnitModel): string {
        let env: Environment = this._documentUnitService.getDocumentUnitEnvironment(documentUnit);
        let foundItems: string[] = this.DocumentUnitRedirectUrls.filter((item: [Environment, (documentUnit: DocumentUnitModel) => string]) => item[0] == env)
            .map((item: [Environment, (documentUnit: DocumentUnitModel) => string]) => item[1](documentUnit));

        if (!foundItems || foundItems.length == 0) {
            return "#";
        }

        return foundItems[0];
    }

    private getFinderModel(): FascicleFinderViewModel {
        let finderModel: FascicleFinderViewModel = <FascicleFinderViewModel>{};
        finderModel.Year = Number(this._txtYear.get_value());
        if (this._txtNumber.get_value() != "")
            finderModel.Number = this._txtNumber.get_value().replace("/", "|");

        if (this._rcbUDDoc.get_selectedItem() != undefined && this._rcbUDDoc.get_selectedItem().get_value() != "") {
            finderModel.UdType = this._rcbUDDoc.get_selectedItem().get_value();
        }

        if (this._txtSubject.get_value() != "") {
            finderModel.Subject = this._txtSubject.get_value();
        }

        if (this._rcbContainers.get_selectedItem() && this._rcbContainers.get_selectedItem().get_value()) {
            finderModel.ContainerId = Number(this._rcbContainers.get_selectedItem().get_value());
        }

        this._treeCategory = <Telerik.Web.UI.RadTreeView>$find(this.treeCategoryId);
        if (this._treeCategory.get_allNodes().length > 0) {
            let lastNodeIndex = this._treeCategory.get_allNodes().length - 1;
            finderModel.CategoryId = this._treeCategory.get_allNodes()[lastNodeIndex].get_value();
            finderModel.CategoryFullPath = [];
            this._treeCategory.get_allNodes().forEach(function (item: Telerik.Web.UI.RadTreeNode) {
                finderModel.CategoryFullPath.push(+item.get_value());
            });
        }

        finderModel.IncludeChildClassification = this._chbCategoryChild.checked;

        return finderModel;
    }

    /**
     * Esegue una ricerca delle documentunits corrispondenti all'anno/numero impostati
     * senza SecurityGroup attiva
     * @param protocolYear
     * @param protocolNumber
     */
    private findDocumentUnits(skip: number): void {
        this._loadingPanel.show(this.pageContentId);

        let finderModel: FascicleFinderViewModel = this.getFinderModel();

        let masterTableView: Telerik.Web.UI.GridTableView = this._grdUdDocSelected.get_masterTableView();
        let top: number = skip + masterTableView.get_pageSize();

        this._documentUnitService.findDocumentUnits(skip, top, finderModel, this._fascicleModel,
            (data: DocumentUnitModel[]) => {
                if (data == null) {
                    this._loadingPanel.hide(this.pageContentId);
                    this.showWarningMessage(this.uscNotificationId, "Nessun elemento trovato con i parametri passati");
                    return;
                }

                this.bindDocumentUnitsGrid(data);
                this._documentUnitService.countDocumentUnits(finderModel, this._fascicleModel,
                    (data) => {
                        if (data == null) {
                            this._loadingPanel.hide(this.pageContentId);
                            this.showWarningMessage(this.uscNotificationId, "Nessun elemento trovato con i parametri passati");
                            return;
                        }
                        masterTableView.set_virtualItemCount(data);
                        masterTableView.dataBind();
                        this._loadingPanel.hide(this.pageContentId);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        this.enableButtons(false);
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.enableButtons(false);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    /**
     * Esegue il binding della griglia dei risultati di ricerca UD
     * @param documentUnits
     */
    private bindDocumentUnitsGrid(documentUnits: DocumentUnitModel[]) {
        let models: Array<FascicleFoundViewModel> = new Array<FascicleFoundViewModel>();

        $.each(documentUnits, (index: number, documentUnit: DocumentUnitModel) => {
            let model: FascicleFoundViewModel = <FascicleFoundViewModel>{};
            if (documentUnit.Category) {
                model.Category = documentUnit.Category.Name;
            }
            if (documentUnit.Container) {
                model.Container = documentUnit.Container.Name;
            }
            model.UDLink = this.getDocumentUnitUrl(documentUnit);
            model.Object = documentUnit.Subject;
            model.UDType = documentUnit.DocumentUnitName;
            model.UDTitle = documentUnit.Title;
            model.UniqueId = documentUnit.UniqueId;
            model.IdFascicle = documentUnit.IdFascicle;
            model.RegistrationDate = moment(documentUnit.RegistrationDate).format("DD/MM/YYYY");
            model.IsFascicolable = documentUnit.IsFascicolable;
            models.push(model);
        });

        this._documentUnitsFound = documentUnits;
        $("#panelContent").show();

        let masterTableView: Telerik.Web.UI.GridTableView = this._grdUdDocSelected.get_masterTableView();
        masterTableView.set_dataSource(models);
        masterTableView.dataBind();
    }




    insertFascicleDocumentUnit(documentUnit: DocumentUnitModel): JQueryPromise<void> {
        let fascicleDocumentUnitModel: FascicleDocumentUnitModel = new FascicleDocumentUnitModel(this._fascicleModel.UniqueId);
        fascicleDocumentUnitModel.DocumentUnit = documentUnit;
        return this.insertFascicleUD(fascicleDocumentUnitModel, this._fascicleDocumentUnitService);
    }

    /**
     * Metodo generico che esegue la chiamata ajax per l'inserimento di una nuova UD nel fascicolo
     * @param model
     * @param service
     */
    private insertFascicleUD(model: IFascicolableBaseModel, service: IFascicolableBaseService<IFascicolableBaseModel>): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        if (this.idFascicleFolder) {
            model.FascicleFolder = {} as FascicleFolderModel;
            model.FascicleFolder.UniqueId = this.idFascicleFolder;
        }

        service.insertFascicleUD(model, FascicolableActionType.AutomaticDetection,
            (data: any) => {
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        );
        return promise.promise();
    }

    /**
    * Metodo che imposta l'abilitazione dei pulsanti Fascicola e Inserisci
    * @param value
    */
    private enableButtons(value: boolean) {
        this._btnReference.set_enabled(value);
    }

    /**
   * Evento scatenato dalla RadComboBox per inizializzare i dati da visualizzare
   * @param sender
   * @param args
   */
    rcbContainers_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {

        let numberOfItems: number = sender.get_items().get_count();
        if (numberOfItems > 0) {
            //Decremento di 1 perchè la combo visualizza anche un item vuoto
            numberOfItems -= 1;
        }

        let currentOtherContainerItems: number = numberOfItems;
        let currentComboFilter: string = sender.get_attributes().getAttribute('currentFilter');
        let otherContainerCount: number = Number(sender.get_attributes().getAttribute('otherContainerCount'));
        let updating: boolean = sender.get_attributes().getAttribute('updating') == 'true';
        if (isNaN(otherContainerCount) || currentComboFilter != args.get_text()) {
            //Se il valore del filtro è variato re-inizializzo la radcombobox per chiamare le WebAPI
            otherContainerCount = undefined;
        }
        sender.get_attributes().setAttribute('currentFilter', args.get_text());

        this.setMoreResultBoxText('Caricamento...');

        if ((otherContainerCount == undefined || currentOtherContainerItems < otherContainerCount) && !updating) {
            sender.get_attributes().setAttribute('updating', 'true');
            let location: LocationTypeEnum = null;
            if (this._rcbUDDoc.get_selectedItem() != undefined && this._rcbUDDoc.get_selectedItem().get_value() != "") {
                let env: string = this._rcbUDDoc.get_selectedItem().get_value();
                if (env == "Protocollo") {
                    location = LocationTypeEnum.ProtLocation;
                }
                if (env == "Serie documentale") {
                    location = LocationTypeEnum.DocumentSeriesLocation;
                }
                if (env == "Archivio") {
                    location = LocationTypeEnum.UDSLocation;
                }
                if (location == null) {
                    location = LocationTypeEnum.ReslLocation;
                }
            }
            this._containerService.getContainersByEnvironment(args.get_text(), this.maxNumberElements, currentOtherContainerItems, location,
                (data: ODATAResponseModel<ContainerModel>) => {
                    try {
                        this.refreshContainers(data.value);
                        let scrollToPosition: boolean = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                let scrollContainer: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollContainer.scrollTop($(sender.get_items().getItem(currentOtherContainerItems + 1).get_element()).position().top);
                            }
                        }

                        sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        if (sender.get_items().get_count() > 0) {
                            currentOtherContainerItems = sender.get_items().get_count() - 1;
                        }

                        this.setMoreResultBoxText('Visualizzati '.concat(currentOtherContainerItems.toString(), ' di ', data.count.toString()));
                    }
                    catch (error) {
                        this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                        console.log(JSON.stringify(error));
                    }
                },
                (exception: ExceptionDTO) => {
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
    }

    /**
 * Metodo che setta la label MoreResultBoxText della RadComboBox
 * @param message
 */
    private setMoreResultBoxText(message: string) {
        this._rcbContainers.get_moreResultsBoxMessageElement().innerText = message;
    }


    /**
 * Metodo per popolare la RadComboBox di selezione fascicoli
 * @param data
 */
    refreshContainers = (data: ContainerModel[]) => {
        if (data.length > 0) {
            this._rcbContainers.beginUpdate();
            if (this._rcbContainers.get_items().get_count() == 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._rcbContainers.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, container) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(container.Name);
                item.set_value(container.EntityShortId.toString());
                this._rcbContainers.get_items().add(item);
            });
            this._rcbContainers.showDropDown();
            this._rcbContainers.endUpdate();
        }
        else {
            if (this._rcbContainers.get_items().get_count() == 0) {
            }

        }
    }

    /**
 * Evento scatenato allo scrolling della RadComboBox di selezione fascicoli
 * @param args
 */
    rcbOtherFascicles_onScroll = (args: JQueryEventObject) => {
        var element = args.target;
        if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
            let filter: string = this._rcbContainers.get_text();
            this.rcbContainers_OnClientItemsRequested(this._rcbContainers, new (<any>Telerik.Web.UI.RadComboBoxRequestEventArgs)(filter, args));
        }
    }

    rcbContainers_OnSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let selectedItem: Telerik.Web.UI.RadComboBoxItem = sender.get_selectedItem();
        let domEvent: Sys.UI.DomEvent = args.get_domEvent();
        if (domEvent.type == 'mousedown') {
            return;
        }

        if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
            let emptyItem: Telerik.Web.UI.RadComboBoxItem = sender.findItemByText('');
            sender.clearItems();
            sender.get_items().add(emptyItem);
            sender.get_items().add(selectedItem);
            sender.get_attributes().setAttribute('currentFilter', selectedItem.get_text());
            sender.get_attributes().setAttribute('otherContainerCount', '1');
            this.setMoreResultBoxText("Visualizzati 1 di 1");
        }
    }

    rcbUDDoc_OnSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let domEvent: Sys.UI.DomEvent = args.get_domEvent();
        if (domEvent.type == 'mousedown') {
            return;
        }

        if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
            this._rcbContainers.clearItems();
        }
    }
}

export = uscFascInsertUD;