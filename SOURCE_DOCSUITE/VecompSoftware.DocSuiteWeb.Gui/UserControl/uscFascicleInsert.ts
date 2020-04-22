/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import VisibilityType = require('App/Models/Fascicles/VisibilityType');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleBase = require('Fasc/FascBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscCategoryRest = require('UserControl/uscCategoryRest');
import UscSettori = require('UserControl/uscSettori');
import UscOggetto = require('UserControl/uscOggetto');
import UscContattiSel = require('UserControl/uscContattiSel');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import UscDynamicMetadata = require('UserControl/uscDynamicMetadata');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import EnumHelper = require('App/Helpers/EnumHelper');
import ContainerService = require('App/Services/Commons/ContainerService');
import ContainerModel = require('App/Models/Commons/ContainerModel');

declare var Page_IsValid: any;
declare var ValidatorEnable: any;
class uscFascicleInsert extends FascicleBase {
    ajaxLoadingPanelId: string;
    ajaxManagerId: string;
    uscNotificationId: string;
    fasciclePageContentId: string;
    currentUser: string;
    fascicleDataRowId: string;
    rdlFascicleTypeId: string;
    contattiRespRowId: string;
    activityFascicleEnabled: boolean;
    isMasterRowId: string;
    uscClassificatoreId: string;
    uscSettoriId: string;
    uscMasterRolesId: string;
    uscContattiRespId: string;
    fascicleTypologyRowId: string;
    txtNoteId: string;
    uscOggettoId: string;
    txtConservationId: string;
    pnlConservationId: string;
    fasciclesPanelVisibilities: { [key: string]: boolean };
    rfvConservationId: string;
    rowStartDateId: string;
    radStartDateId: string;
    metadataRepositoryEnabled: boolean;
    metadataRepositoryRowId: string;
    uscMetadataRepositorySelId: string;
    uscDynamicMetadataId: string;
    containerRowId: string;
    ddlContainerId: string;
    fascicleContainerEnabled: boolean;
    rfvContainerId: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _serviceConfigurations: ServiceConfiguration[];
    private _rdlFascicleType: Telerik.Web.UI.RadDropDownList;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _radStartDate: Telerik.Web.UI.RadDatePicker;
    private _deferredInitializeActions: JQueryDeferred<void>[] = [];
    private _deferredFascicleSelectedTypeActions: JQueryDeferred<void>[] = [];
    private _enumHelper: EnumHelper;
    private _containerService: ContainerService;
    private _ddlContainer: Telerik.Web.UI.RadDropDownList;


    public static LOADED_EVENT: string = "onLoaded";
    public static FASCICLE_TYPE_CHANGED_EVENT: string = "onFascicleTypeChanged";

    get selectedFascicleType(): FascicleType {
        if (this._rdlFascicleType.get_selectedItem().get_value()) {
            return Number(this._rdlFascicleType.get_selectedItem().get_value());
        }
        return null;
    }

    get selectedContainer(): number {
        if (this._ddlContainer.get_selectedItem() && this._ddlContainer.get_selectedItem().get_value()) {
            return Number(this._ddlContainer.get_selectedItem().get_value());
        }
        return null;
    }

    private get fascicleDataRow(): JQuery {
        return $(`#${this.fascicleDataRowId}`);
    }

    private get contattiRespRow(): JQuery {
        return $(`#${this.contattiRespRowId}`);
    }

    private get isMasterRow(): JQuery {
        return $(`#${this.isMasterRowId}`);
    }

    private get startDateRow(): JQuery {
        return $(`#${this.rowStartDateId}`);
    }

    private get metadataRepositoryRow(): JQuery {
        return $(`#${this.metadataRepositoryRowId}`);
    }

    private get fascicleTypologyRow(): JQuery {
        return $(`#${this.fascicleTypologyRowId}`);
    }

    private get containerRow(): JQuery {
        return $(`#${this.containerRowId}`);
    }

    /**
     * Costruttore
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    /**
     * Initialize
     */
    initialize(): void {
        super.initialize();

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rdlFascicleType = <Telerik.Web.UI.RadDropDownList>$find(this.rdlFascicleTypeId);
        this._rdlFascicleType.add_selectedIndexChanged(this.rdlFascicleType_onSelectedIndexChanged);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._radStartDate = <Telerik.Web.UI.RadDatePicker>$find(this.radStartDateId);
        this._radStartDate.set_selectedDate(moment().toDate());
        this._enumHelper = new EnumHelper();
        this._ddlContainer = $find(this.ddlContainerId) as Telerik.Web.UI.RadDropDownList;
        this._ddlContainer.add_selectedIndexChanged(this.ddlContainer_onSelectedIndexChanged);

        let containerServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
        this._containerService = new ContainerService(containerServiceConfiguration);

        if (this.pnlConservationId) {
            let txtConservation: Telerik.Web.UI.RadNumericTextBox = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtConservationId);
            txtConservation.set_value("0");
        }

        this.metadataRepositoryRow.hide();

        if (this.selectedFascicleType && this.selectedFascicleType == FascicleType.Period) {
            this.initializeFasciclePeriodic();
        }

        if (this.activityFascicleEnabled && !this.selectedFascicleType) {
            this.initializeEmptyFascicleTypeSelected();
        }

        if (this.metadataRepositoryEnabled) {
            this.metadataRepositoryRow.show();
            this.setMetadataRepositorySelectedIndexEvent();
        }

        this._loadingPanel.show(this.fasciclePageContentId);
        this.checkAutorizations()
            .done((result: boolean) => {
                if (!result) {
                    this._loadingPanel.hide(this.fasciclePageContentId);
                    this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione pagina.<br \> Utente non autorizzato all'inserimento di un nuovo fascicolo");
                    return;
                }

                const deferredInitializeAction = () => {
                    let promise: JQueryDeferred<void> = $.Deferred<void>();
                    this._deferredInitializeActions.push(promise);
                    let initializeContainersAction = () => $.Deferred<void>().resolve().promise();
                    if (this.fascicleContainerEnabled && (this.selectedFascicleType == FascicleType.Period
                        || this.selectedFascicleType == FascicleType.Procedure)) {
                        initializeContainersAction = () => this.initializeContainers();
                    }

                    initializeContainersAction()
                        .done(() => {
                            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest("Initialize");
                        })
                        .fail((exception: ExceptionDTO) => promise.reject(exception));
                    return promise.promise();
                }

                deferredInitializeAction()
                    .always(() => {
                        if (!this.activityFascicleEnabled && (!this.selectedFascicleType || this.selectedFascicleType != FascicleType.Period)) {
                            this.setProcedureTypeSelected();
                        }
                        this.bindLoaded();
                        this._loadingPanel.hide(this.fasciclePageContentId);
                    });
            })
            .fail((exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.fasciclePageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    /**
     *------------------------- Events -----------------------------
     */


    /**
     * Evento scatenato al cambio di selezione del classificatore
     * @param conservationYear
     */
    onCategoryChanged = (conservationYear: string, idMetadataRepository: string) => {
        if (this.selectedFascicleType == FascicleType.Procedure) {
            let txtConservation: Telerik.Web.UI.RadNumericTextBox = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtConservationId);
            if (conservationYear && Number(conservationYear) != -1)
                txtConservation.set_value(conservationYear);
            else
                txtConservation.set_value("0");
        }

        if (this.fascicleContainerEnabled && this.selectedFascicleType == FascicleType.Procedure) {
            this.initializeContainers()
                .fail((exception: ExceptionDTO) => this.showNotificationException(this.uscNotificationId, exception));
        }

        if (this.metadataRepositoryEnabled) {
            let uscMetadataRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataRepositorySelId)).data();
            if (!jQuery.isEmptyObject(uscMetadataRepositorySel)) {
                this.setMetadataRepositorySelectedIndexEvent();
                uscMetadataRepositorySel.clearComboboxText();
                if (!!idMetadataRepository) {
                    uscMetadataRepositorySel.setComboboxText(idMetadataRepository);
                    let uscDynamicMetadata: UscDynamicMetadata = <UscDynamicMetadata>$("#".concat(this.uscDynamicMetadataId)).data();
                    if (!jQuery.isEmptyObject(uscDynamicMetadata)) {
                        uscDynamicMetadata.loadDynamicMetadata(idMetadataRepository);
                    }
                }
            }
        }
    }

    /**
     * Evento scatenato al cambio di selezione di una tipologia di Fascicolo
     * @param sender
     * @param args
     */
    rdlFascicleType_onSelectedIndexChanged = (sender: Telerik.Web.UI.RadDropDownList, args: Telerik.Web.UI.DropDownListIndexChangedEventArgs) => {
        this.enableValidatorsByFasciceType();
        let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
        if (!jQuery.isEmptyObject(uscClassificatore)) {
            uscClassificatore.setFascicleTypeParam(this.selectedFascicleType);
            uscClassificatore.setShowAuthorizedParam(this.selectedFascicleType != FascicleType.Activity);
        }

        if (this.activityFascicleEnabled && !this.selectedFascicleType) {
            this.initializeEmptyFascicleTypeSelected();
            return;
        }
        this.setPageBehaviourAfterFascicleTypeChanged();
    }

    ddlContainer_onSelectedIndexChanged = (sender: Telerik.Web.UI.RadDropDownList, args: Telerik.Web.UI.DropDownListIndexChangedEventArgs) => {
        let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
        if (!jQuery.isEmptyObject(uscClassificatore)) {
            uscClassificatore.setShowContainerParam(this.selectedContainer);
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */

    private checkAutorizations(): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        if (!this.selectedFascicleType) {
            return promise.resolve(true);
        }

        this.service.hasInsertRight(this.selectedFascicleType,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    initializeContainers(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let selectedIdCategory: number = null;
        let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
        if (!jQuery.isEmptyObject(uscClassificatore)) {
            let selectedCategory: CategoryModel = uscClassificatore.getSelectedCategory();
            if (selectedCategory) {
                selectedIdCategory = selectedCategory.EntityShortId;
            }
        }
        this._containerService.getFascicleInsertAuthorizedContainers(selectedIdCategory, this.selectedFascicleType,
            (data: any) => {
                let containers: ContainerModel[] = data as ContainerModel[];
                let lastSelectedContainer: number = this.selectedContainer;
                if (this._ddlContainer.get_selectedItem()) {
                    this._ddlContainer.get_selectedItem().set_selected(false);
                }
                this._ddlContainer.get_items().clear();
                if (containers.length != 1) {
                    let emptyItem: Telerik.Web.UI.DropDownListItem = new Telerik.Web.UI.DropDownListItem();
                    this._ddlContainer.get_items().add(emptyItem);
                }
                let containerItem: Telerik.Web.UI.DropDownListItem;
                for (let container of containers) {
                    containerItem = new Telerik.Web.UI.DropDownListItem();
                    containerItem.set_text(container.Name);
                    containerItem.set_value(container.EntityShortId.toString());
                    this._ddlContainer.get_items().add(containerItem);
                    if (containers.length == 1) { containerItem.set_selected(true); };
                }
                if (lastSelectedContainer) {
                    let itemToSelect: Telerik.Web.UI.DropDownListItem = this._ddlContainer.findItemByValue(lastSelectedContainer.toString());
                    if (itemToSelect) {
                        itemToSelect.set_selected(true);
                    }
                }
                promise.resolve();
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    /**
     * Callback da code-behind per l'inizializzazione
     * @param isInitialized
     */
    initializeCallback(): void {
        if (this.selectedFascicleType) {
            let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
            if (!jQuery.isEmptyObject(uscClassificatore)) {
                uscClassificatore.setFascicleTypeParam(this.selectedFascicleType);
            }
        }

        this._deferredInitializeActions.forEach((item: JQueryDeferred<void>) => item.resolve());
    }

    /**
 * Metodo di validazione della pagina
 */
    isPageValid(): boolean {

        let uscOggetto: UscOggetto = <UscOggetto>$("#".concat(this.uscOggettoId)).data();
        if (!jQuery.isEmptyObject(uscOggetto)) {
            let txtOggetto: string = uscOggetto.getText();

            if (!uscOggetto.isValid()) {
                this.showNotificationMessage(this.uscNotificationId, "Impossibile salvare.\nIl campo Oggetto ha superato i caratteri disponibili.<br />(Caratteri ".concat(txtOggetto.length.toString(),
                    " Disponibili ", uscOggetto.getMaxLength().toString(), ")"));
                return false;
            }
        }

        let fascicleTypeSelected: number = Number(this._rdlFascicleType.get_selectedItem().get_value());
        if (fascicleTypeSelected == FascicleType.Procedure) {
            let txtConservation: Telerik.Web.UI.RadNumericTextBox = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtConservationId);
            if (txtConservation.get_value() == null || isNaN(Number(txtConservation.get_value()))) {
                this.showNotificationMessage(this.uscNotificationId, "Impossibile salvare. Il campo Conservazione Anni è mancante o non valido.");
                return false;
            }
        }

        return Page_IsValid;
    }

    setPageBehaviourAfterFascicleTypeChanged(): void {
        this._loadingPanel.show(this.fasciclePageContentId);
        this.checkAutorizations()
            .done((result) => {
                if (!result) {
                    let fascicleTypeDescription: string = this._enumHelper.getFascicleTypeDescription(this.selectedFascicleType);
                    this.initializeEmptyFascicleTypeSelected();
                    this.showNotificationMessage(this.uscNotificationId, `Errore in inizializzazione pagina.<br \> Utente non autorizzato all'inserimento di un nuovo fascicolo di tipo ${fascicleTypeDescription}`);
                    this._loadingPanel.hide(this.fasciclePageContentId);
                    return;
                }

                this.initializePageByFascicleType(this.selectedFascicleType);
                const deferredAction = () => {
                    let promise: JQueryDeferred<void> = $.Deferred<void>();
                    this._deferredFascicleSelectedTypeActions.push(promise);
                    let initializeContainersAction = () => $.Deferred<void>().resolve().promise();
                    if (this.fascicleContainerEnabled && (this.selectedFascicleType == FascicleType.Period
                        || this.selectedFascicleType == FascicleType.Procedure)) {
                        initializeContainersAction = () => this.initializeContainers();
                    }
                    initializeContainersAction()
                        .done(() => {
                            setTimeout(() => {
                                (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest("FascicleTypeSelected");
                            }, 500);
                        })
                        .fail((exception: ExceptionDTO) => promise.reject(exception));
                    this._loadingPanel.hide(this.fasciclePageContentId);
                    return promise.promise();
                }

                deferredAction()
                    .always(() => {
                        $("#".concat(this.fasciclePageContentId)).triggerHandler(uscFascicleInsert.FASCICLE_TYPE_CHANGED_EVENT);
                        this._loadingPanel.hide(this.fasciclePageContentId);
                    });
            })
            .fail((exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.fasciclePageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    initializePageByFascicleType(fascicleType: FascicleType): void {
        switch (fascicleType) {
            case FascicleType.Procedure: {
                this.initializeFascicleProcedure();
            }
                break;
            case FascicleType.Activity: {
                this.initializeFascicleActivity();
            }
                break;

            case FascicleType.Period: {
                this.initializeFasciclePeriodic();
            }
                break;
        }
    }

    fascicleTypeSelectedCallback(): void {
        this._deferredFascicleSelectedTypeActions.forEach((item: JQueryDeferred<void>) => item.resolve());
    }

    printCategoryNotFascicolable(): void {
        this.showWarningMessage(this.uscNotificationId, "Attenzione! Il classificatore selezionato non ha nessuna tipologia di fascicolazione associata");
    }

    private initializeFascicleProcedure(): void {
        if (this.fascicleContainerEnabled) {
            this.containerRow.show();
            ValidatorEnable($get(this.rfvContainerId), true);
            $(`#${this.rfvContainerId}`).show();
        } else {
            this.containerRow.hide();
            ValidatorEnable($get(this.rfvContainerId), false);
            $(`#${this.rfvContainerId}`).hide();
        }

        this.fascicleDataRow.show();
        this.contattiRespRow.show();
        this.isMasterRow.show();
        this.startDateRow.hide();

        if (!String.isNullOrEmpty(this.pnlConservationId)) {
            $("#".concat(this.pnlConservationId)).show();
        }
    }

    private initializeFascicleActivity(): void {
        ValidatorEnable($get(this.rfvContainerId), false);
        $(`#${this.rfvContainerId}`).hide();
        this.containerRow.hide();
        this.fascicleDataRow.show();
        this.contattiRespRow.hide();
        this.isMasterRow.hide();
        this.startDateRow.hide();

        if (!String.isNullOrEmpty(this.pnlConservationId)) {
            $("#".concat(this.pnlConservationId)).hide();
        }
    }

    private initializeFasciclePeriodic(): void {
        if (this.fascicleContainerEnabled) {
            this.containerRow.show();
            ValidatorEnable($get(this.rfvContainerId), true);
            $(`#${this.rfvContainerId}`).hide();
        } else {
            this.containerRow.hide();
            ValidatorEnable($get(this.rfvContainerId), false);
            $(`#${this.rfvContainerId}`).hide();
            this.isMasterRow.show();
        }
        this.fascicleDataRow.show();
        this.contattiRespRow.hide();
        this.fascicleTypologyRow.hide();
    }

    private initializeEmptyFascicleTypeSelected(): void {
        ValidatorEnable($get(this.rfvContainerId), false);
        $(`#${this.rfvContainerId}`).hide();
        this.fascicleDataRow.hide();
    }

    //contact: number
    getFascicle = () => {

        let fascicleModel: FascicleModel = new FascicleModel();
        fascicleModel.Conservation = null;

        if (this.selectedFascicleType == FascicleType.Procedure) {
            let txtConservation: Telerik.Web.UI.RadNumericTextBox = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtConservationId);
            fascicleModel.Conservation = Number(txtConservation.get_value());

        } else if (this.selectedFascicleType == FascicleType.Activity) {
            fascicleModel.VisibilityType = VisibilityType.Confidential;
        }
        fascicleModel.FascicleType = this.selectedFascicleType;
        fascicleModel.StartDate = this._radStartDate.get_selectedDate();
        fascicleModel.Note = this._txtNote.get_value();

        if (this.fascicleContainerEnabled && (this.selectedFascicleType == FascicleType.Period
            || this.selectedFascicleType == FascicleType.Procedure)) {
            fascicleModel.Container = {} as ContainerModel;
            fascicleModel.Container.EntityShortId = this.selectedContainer;
        }

        let uscOggetto: UscOggetto = <UscOggetto>$("#".concat(this.uscOggettoId)).data();
        if (!jQuery.isEmptyObject(uscOggetto)) {
            fascicleModel.FascicleObject = uscOggetto.getText();
        }

        let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
        if (!jQuery.isEmptyObject(uscClassificatore)) {
            let selectedCategory: CategoryModel = uscClassificatore.getSelectedCategory();
            if (selectedCategory) {
                fascicleModel.Category = selectedCategory;
            }
        }

        //TO DO: quando lo user control dei contatti sarà client side si potrà chiamare come i settori ed il classificatore
        //if (fascicleModel.FascicleType != FascicleType.Activity) {
        //    let contactModel: ContactModel = <ContactModel>{};
        //    contactModel.EntityId = contact;
        //    fascicleModel.Contacts.push(contactModel);
        //}

        let uscRoles: UscSettori = <UscSettori>$("#".concat(this.uscSettoriId)).data();
        if (!jQuery.isEmptyObject(uscRoles)) {
            if (this.selectedFascicleType == FascicleType.Procedure) {
                fascicleModel.VisibilityType = uscRoles.getFascicleVisibilityType();
            }

            let source: any = JSON.parse(uscRoles.getRoles());
            if (source != null) {
                let role: RoleModel;
                let fascicleRole: FascicleRoleModel;
                for (let s of source) {
                    role = <RoleModel>{};
                    fascicleRole = new FascicleRoleModel();
                    role.IdRole = s.EntityShortId;
                    role.EntityShortId = s.EntityShortId;
                    role.TenantId = s.TenantId;
                    fascicleRole.Role = role;
                    fascicleModel.FascicleRoles.push(fascicleRole);
                }
            }
        }

        if (fascicleModel.FascicleType != FascicleType.Activity) {
            let uscMasterRoles: UscSettori = <UscSettori>$("#".concat(this.uscMasterRolesId)).data();
            if (!jQuery.isEmptyObject(uscMasterRoles)) {
                let source: any = JSON.parse(uscMasterRoles.getRoles());
                if (source != null) {
                    let role: RoleModel;
                    let fascicleRole: FascicleRoleModel;
                    for (let s of source) {
                        role = <RoleModel>{};
                        fascicleRole = new FascicleRoleModel();
                        role.IdRole = s.EntityShortId;
                        role.EntityShortId = s.EntityShortId;
                        role.TenantId = s.TenantId;
                        fascicleRole.Role = role;
                        fascicleRole.IsMaster = true;
                        fascicleModel.FascicleRoles.push(fascicleRole);

                    }
                }
            }
        }
        return fascicleModel;
    }
    /**
* Scateno l'evento di "Load Completed" del controllo
*/
    private bindLoaded(): void {
        $("#".concat(this.fasciclePageContentId)).data(this);
        $("#".concat(this.fasciclePageContentId)).triggerHandler(uscFascicleInsert.LOADED_EVENT);
    }

    getSelectedFascicleType = () => {
        return this._rdlFascicleType.get_selectedItem().get_value();
    }

    enableValidators = (enabled: boolean) => {
        ValidatorEnable($get(this.rfvConservationId), enabled);

        let uscMasterRoles: UscSettori = <UscSettori>$("#".concat(this.uscMasterRolesId)).data();
        if (!jQuery.isEmptyObject(uscMasterRoles)) {
            uscMasterRoles.enableValidators(enabled);
        }

        let uscRoles: UscSettori = <UscSettori>$("#".concat(this.uscSettoriId)).data();
        if (!jQuery.isEmptyObject(uscRoles)) {
            uscRoles.enableValidators(enabled);
        }

        let uscOggetto: UscOggetto = <UscOggetto>$("#".concat(this.uscOggettoId)).data();
        if (!jQuery.isEmptyObject(uscOggetto)) {
            uscOggetto.enableVaidators(enabled);
        }

        let uscContattiResp: UscContattiSel = <UscContattiSel>$("#".concat(this.uscContattiRespId)).data();
        if (!jQuery.isEmptyObject(uscContattiResp)) {
            uscContattiResp.enableValidators(enabled);
        }
    }

    private enableValidatorsByFasciceType = () => {
        let selectedType: string = this._rdlFascicleType.get_selectedItem().get_value();

        if (String.isNullOrEmpty(selectedType)) {
            this.enableValidators(true);
            return;
        }

        let uscOggetto: UscOggetto = <UscOggetto>$("#".concat(this.uscOggettoId)).data();
        if (!jQuery.isEmptyObject(uscOggetto)) {
            uscOggetto.enableVaidators(true);
        }

        let isProcedure: boolean = false;
        if (selectedType == FascicleType.Procedure.toString()) {
            isProcedure = true;
        }

        ValidatorEnable($get(this.rfvConservationId), isProcedure);

        let uscMasterRoles: UscSettori = <UscSettori>$("#".concat(this.uscMasterRolesId)).data();
        if (!jQuery.isEmptyObject(uscMasterRoles)) {
            uscMasterRoles.enableValidators(isProcedure);
        }

        let uscRoles: UscSettori = <UscSettori>$("#".concat(this.uscSettoriId)).data();
        if (!jQuery.isEmptyObject(uscRoles)) {
            uscRoles.enableValidators(!isProcedure);
        }

        let uscContattiResp: UscContattiSel = <UscContattiSel>$("#".concat(this.uscContattiRespId)).data();
        if (!jQuery.isEmptyObject(uscContattiResp)) {
            uscContattiResp.enableValidators(isProcedure);
        }

    }

    setProcedureTypeSelected() {
        let selectedItem: Telerik.Web.UI.DropDownListItem = this._rdlFascicleType.findItemByValue(FascicleType.Procedure.toString());
        selectedItem.set_selected(true);
        this._rdlFascicleType.set_enabled(false);
        $("#".concat(this.fascicleDataRowId)).show();
    }

    private setMetadataRepositorySelectedIndexEvent() {
        $("#".concat(this.uscMetadataRepositorySelId)).off(UscMetadataRepositorySel.SELECTED_INDEX_EVENT);
        $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_INDEX_EVENT, (args, data) => {
            let uscDynamicMetadata: UscDynamicMetadata = <UscDynamicMetadata>$("#".concat(this.uscDynamicMetadataId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadata)) {
                setTimeout(() => {
                    uscDynamicMetadata.loadDynamicMetadata(data);
                }, 500);
            }
        });
    }

    setCategoryRole(idRole: number): void {
        let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
        if (!jQuery.isEmptyObject(uscClassificatore)) {
            uscClassificatore.setShowRoleParam(idRole);
        }
    }
}
export = uscFascicleInsert;
