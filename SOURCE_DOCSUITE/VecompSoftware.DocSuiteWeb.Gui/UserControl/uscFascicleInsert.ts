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
import UscContattiSel = require('UserControl/uscContattiSel');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import uscDynamicMetadataRest = require('UserControl/uscDynamicMetadataRest');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import EnumHelper = require('App/Helpers/EnumHelper');
import ContainerService = require('App/Services/Commons/ContainerService');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import uscCustomActionsRest = require('UserControl/uscCustomActionsRest');
import FascicleCustomActionModel = require('App/Models/Commons/FascicleCustomActionModel');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import uscRoleRest = require('./uscRoleRest');
import UscRoleRestEventType = require('App/Models/Commons/UscRoleRestEventType');
import AjaxModel = require('App/Models/AjaxModel');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import UscRoleRestConfiguration = require('App/Models/Commons/UscRoleRestConfiguration');
import ExternalSourceActionEnum = require('App/Helpers/ExternalSourceActionEnum');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');

declare var Page_IsValid: any;
declare var ValidatorEnable: any;
class uscFascicleInsert extends FascicleBase {
    clientId: string;
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
    isRoleRowId: string;
    uscClassificatoreId: string;
    uscRoleMasterId: string;
    uscRoleId: string;
    uscContattiRespId: string;
    fascicleTypologyRowId: string;
    txtNoteId: string;
    txtObjectId: string;
    txtConservationId: string;
    pnlConservationId: string;
    fasciclesPanelVisibilities: { [key: string]: boolean };
    rfvConservationId: string;
    rowStartDateId: string;
    radStartDateId: string;
    metadataRepositoryEnabled: boolean;
    metadataRepositoryRowId: string;
    uscMetadataRepositorySelId: string;
    uscDynamicMetadataRestId: string;
    containerRowId: string;
    ddlContainerId: string;
    fascicleContainerEnabled: boolean;
    rfvContainerId: string;
    uscCustomActionsRestId: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _serviceConfigurations: ServiceConfiguration[];
    private _rdlFascicleType: Telerik.Web.UI.RadDropDownList;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _txtConservation: Telerik.Web.UI.RadTextBox;
    private _txtObject: Telerik.Web.UI.RadTextBox;
    private _radStartDate: Telerik.Web.UI.RadDatePicker;
    private _deferredInitializeActions: JQueryDeferred<void>[] = [];
    private _deferredFascicleSelectedTypeActions: JQueryDeferred<void>[] = [];
    private _enumHelper: EnumHelper;
    private _containerService: ContainerService;
    private _ddlContainer: Telerik.Web.UI.RadDropDownList;

    private _selectedResponsibleRole: RoleModel;
    private _fascicleVisibilityType: VisibilityType;
    private _needRolesFromExternalSource_eventArgs: string[];

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

    private fascicleDataRow(): JQuery {
        return $(`#${this.fascicleDataRowId}`);
    }

    private contattiRespRow(): JQuery {
        return $(`#${this.contattiRespRowId}`);
    }

    private isMasterRow(): JQuery {
        return $(`#${this.isMasterRowId}`);
    }

    private isRoleRow(): JQuery {
        return $(`#${this.isRoleRowId}`);
    }

    private startDateRow(): JQuery {
        return $(`#${this.rowStartDateId}`);
    }

    private metadataRepositoryRow(): JQuery {
        return $(`#${this.metadataRepositoryRowId}`);
    }

    private fascicleTypologyRow(): JQuery {
        return $(`#${this.fascicleTypologyRowId}`);
    }

    private containerRow(): JQuery {
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
        PageClassHelper.callUserControlFunctionSafe<UscCategoryRest>(this.uscClassificatoreId)
            .done((instance) => instance.setFascicleTypeParam(FascicleType.Procedure));

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rdlFascicleType = <Telerik.Web.UI.RadDropDownList>$find(this.rdlFascicleTypeId);
        this._rdlFascicleType.add_selectedIndexChanged(this.rdlFascicleType_onSelectedIndexChanged);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._txtConservation = $find(this.txtConservationId) as Telerik.Web.UI.RadTextBox;
        this._txtObject = $find(this.txtObjectId) as Telerik.Web.UI.RadTextBox;
        this._radStartDate = <Telerik.Web.UI.RadDatePicker>$find(this.radStartDateId);
        this._radStartDate.set_selectedDate(moment().toDate());
        this._enumHelper = new EnumHelper();
        this._ddlContainer = $find(this.ddlContainerId) as Telerik.Web.UI.RadDropDownList;
        this._ddlContainer.add_selectedIndexChanged(this.ddlContainer_onSelectedIndexChanged);
        this._rdlFascicleType.findItemByValue(String(FascicleType.Procedure))?.select();

        let containerServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
        this._containerService = new ContainerService(containerServiceConfiguration);

        if (this.pnlConservationId) {
            let txtConservation: Telerik.Web.UI.RadNumericTextBox = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtConservationId);
            txtConservation.set_value("0");
        }

        this.metadataRepositoryRow().hide();

        if (this.selectedFascicleType && this._rdlFascicleType.findItemByValue(String(FascicleType.Period))) {
            this.initializeFasciclePeriodic();
        }

        if (this.activityFascicleEnabled && !this.selectedFascicleType) {
            this.initializeEmptyFascicleTypeSelected();
        }

        this._fascicleVisibilityType = VisibilityType.Confidential;
        this.registerUscRoleRestEventHandlers();
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId).done((instance) => {
            instance.setToolbarVisibility(true);
            instance.renderRolesTree([]);
        });
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId).done((instance) => {
            instance.setToolbarVisibility(true);
            instance.renderRolesTree([]);
        });
        sessionStorage.removeItem(`${this.clientId}_FascicleRolesToAdd`);

        if (this.metadataRepositoryEnabled) {
            this.metadataRepositoryRow().show();
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
                            let ajaxModel: AjaxModel = <AjaxModel>{
                                ActionName: "Initialize",
                                Value: []
                            };
                            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
                        })
                        .fail((exception: ExceptionDTO) => promise.reject(exception));
                    return promise.promise();
                }

                deferredInitializeAction()
                    .always(() => {
                        if (!this.activityFascicleEnabled && (!this.selectedFascicleType || this.selectedFascicleType != FascicleType.Period)) {
                            this.setProcedureTypeSelected();
                        }
                        this._loadingPanel.hide(this.fasciclePageContentId);
                    });
            })
            .fail((exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.fasciclePageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            });

        this.bindLoaded();
    }

    /**
 * Callback da code-behind per l'inizializzazione
 * @param isInitialized
 */
    initializeCallback = (): void => {
        if (this.selectedFascicleType) {
            let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
            if (!jQuery.isEmptyObject(uscClassificatore)) {
                uscClassificatore.setFascicleTypeParam(this.selectedFascicleType);
            }
        }

        this._deferredInitializeActions.forEach(x => x.resolve());
    }
    /**
     *------------------------- Events -----------------------------
     */


    /**
     * Evento scatenato al cambio di selezione del classificatore
     * @param conservationYear
     */
    onCategoryChanged = (conservationYear: string, idMetadataRepository: string, customActionsJson: string) => {
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


        if (customActionsJson !== "") {
            PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
                .done((instance) => {
                    instance.loadItems(<FascicleCustomActionModel>JSON.parse(customActionsJson));
                });
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

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId)
            .done((instance) => {
                instance.forceBehaviourValidationState(true);
                instance.enableValidators(true);
            });

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                instance.forceBehaviourValidationState(false);
                instance.enableValidators(false);
            });

        PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
            .done((instance) => {
                instance.loadItems(<FascicleCustomActionModel>{
                    AutoCloseAndClone: false
                });
            });
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
 * Metodo di validazione della pagina
 */
    isPageValid(): boolean {


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
                                let ajaxModel: AjaxModel = <AjaxModel>{
                                    ActionName: "FascicleTypeSelected",
                                    Value: []
                                };
                                if (this._selectedResponsibleRole) {
                                    ajaxModel.Value.push(this._selectedResponsibleRole.EntityShortId.toString());
                                }
                                (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
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
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                instance.multipleRoles = "true";
                instance.onlyMyRoles = false;
                instance.setConfiguration(<UscRoleRestConfiguration>{
                    isReadOnlyMode: true
                });
            });
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId)
            .done((instance) => {
                instance.multipleRoles = "false";
                instance.onlyMyRoles = true;
            });
        this._deferredFascicleSelectedTypeActions.forEach((item: JQueryDeferred<void>) => item.resolve());
    }

    printCategoryNotFascicolable(): void {
        this.showWarningMessage(this.uscNotificationId, "Attenzione! Il classificatore selezionato non ha nessuna tipologia di fascicolazione associata");
    }

    private initializeFascicleProcedure(): void {
        if (this.fascicleContainerEnabled) {
            this.containerRow().show();
            ValidatorEnable($get(this.rfvContainerId), true);
            $(`#${this.rfvContainerId}`).show();
        } else {
            this.containerRow().hide();
            ValidatorEnable($get(this.rfvContainerId), false);
            $(`#${this.rfvContainerId}`).hide();
        }

        this.fascicleDataRow().show();
        this.contattiRespRow().show();
        this.isMasterRow().show();
        this.isRoleRow().show();
        this.startDateRow().hide();

        if (!String.isNullOrEmpty(this.pnlConservationId)) {
            $("#".concat(this.pnlConservationId)).show();
        }
    }

    private initializeFascicleActivity(): void {
        ValidatorEnable($get(this.rfvContainerId), false);
        $(`#${this.rfvContainerId}`).hide();
        this.containerRow().hide();
        this.fascicleDataRow().show();
        this.contattiRespRow().hide();
        this.startDateRow().hide();
        this.isRoleRow().hide();

        if (!String.isNullOrEmpty(this.pnlConservationId)) {
            $("#".concat(this.pnlConservationId)).hide();
        }
    }

    private initializeFasciclePeriodic(): void {
        this._rdlFascicleType.findItemByValue(String(FascicleType.Period)).select();
        if (this.fascicleContainerEnabled) {
            this.containerRow().show();
            ValidatorEnable($get(this.rfvContainerId), true);
            $(`#${this.rfvContainerId}`).hide();
        } else {
            this.containerRow().hide();
            ValidatorEnable($get(this.rfvContainerId), false);
            $(`#${this.rfvContainerId}`).hide();
            this.isMasterRow().show();
        }
        this.isRoleRow().show();
        this.fascicleDataRow().show();
        this.contattiRespRow().hide();
        this.fascicleTypologyRow().hide();
    }

    private initializeEmptyFascicleTypeSelected(): void {
        ValidatorEnable($get(this.rfvContainerId), false);
        $(`#${this.rfvContainerId}`).hide();
        this.fascicleDataRow().hide();
    }

    //contact: number
    getFascicle = (): FascicleModel => {
        let fascicleModel: FascicleModel = new FascicleModel();
        fascicleModel.Conservation = null;

        if (this.selectedFascicleType == FascicleType.Procedure) {
            let txtConservation: Telerik.Web.UI.RadNumericTextBox = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtConservationId);
            fascicleModel.Conservation = Number(txtConservation.get_value());

        } else if (this.selectedFascicleType == FascicleType.Activity) {
            fascicleModel.VisibilityType = VisibilityType.Confidential;
        }
        fascicleModel.FascicleType = this.selectedFascicleType;
        fascicleModel.FascicleObject = this._txtObject.get_textBoxValue();
        fascicleModel.StartDate = this._radStartDate.get_selectedDate();
        fascicleModel.Note = this._txtNote.get_value();

        if (this.fascicleContainerEnabled && (this.selectedFascicleType == FascicleType.Period
            || this.selectedFascicleType == FascicleType.Procedure)) {
            fascicleModel.Container = {} as ContainerModel;
            fascicleModel.Container.EntityShortId = this.selectedContainer;
        }

        let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
        if (!jQuery.isEmptyObject(uscClassificatore)) {
            let selectedCategory: CategoryModel = uscClassificatore.getSelectedCategory();
            if (selectedCategory) {
                fascicleModel.Category = selectedCategory;
            }
        }

        fascicleModel.VisibilityType = this._fascicleVisibilityType;
        fascicleModel.FascicleRoles = this.populateFascicleRoles();

        PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
            .done((instance) => {
                let customActions: FascicleCustomActionModel = instance.getCustomActions<FascicleCustomActionModel>();
                fascicleModel.CustomActions = JSON.stringify(customActions);
            });
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

    fillMetadataModel(): JQueryPromise<[string, string]> {
        let promise: JQueryDeferred<[string, string]> = $.Deferred<[string, string]>();
        PageClassHelper.callUserControlFunctionSafe<uscDynamicMetadataRest>(this.uscDynamicMetadataRestId)
            .done((instance) => {
                let metadataRepository = instance.getMetadataRepository();
                let setiIntegrationEnabledField: boolean = false;
                if (metadataRepository && metadataRepository.JsonMetadata) {
                    let metadataJson: MetadataDesignerViewModel = JSON.parse(metadataRepository.JsonMetadata);
                    setiIntegrationEnabledField = metadataJson.SETIFieldEnabled;
                }

                promise.resolve(instance.bindModelFormPage(setiIntegrationEnabledField));
            });
        return promise.promise();
    }

    enableValidators = (enabled: boolean) => {
        ValidatorEnable($get(this.rfvConservationId), enabled);

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

        let isProcedure: boolean = false;
        if (selectedType == FascicleType.Procedure.toString()) {
            isProcedure = true;
        }

        ValidatorEnable($get(this.rfvConservationId), isProcedure);

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
        $(`#${this.uscMetadataRepositorySelId}`).off(UscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT);
        $(`#${this.uscMetadataRepositorySelId}`).on(UscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, (args, data) => {
            PageClassHelper.callUserControlFunctionSafe<uscDynamicMetadataRest>(this.uscDynamicMetadataRestId)
                .done((instance) => {
                    if (data) {
                        instance.loadMetadataRepository(data);
                    } else {
                        instance.clearPage();
                    }
                });
        });
    }

    setCategoryRole(idRole: number): void {
        let uscClassificatore: UscCategoryRest = <UscCategoryRest>$("#".concat(this.uscClassificatoreId)).data();
        if (!jQuery.isEmptyObject(uscClassificatore)) {
            uscClassificatore.setShowRoleParam(idRole);
        }
    }

    getCustomActions(): JQueryPromise<FascicleCustomActionModel> {
        let promise: JQueryDeferred<FascicleCustomActionModel> = $.Deferred<FascicleCustomActionModel>();
        PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
            .done((instance) => {
                promise.resolve(instance.getCustomActions<FascicleCustomActionModel>());
            });
        return promise.promise();
    }

    private registerUscRoleRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    this.deleteRoleFromModel(roleId);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    this.addRoleToModel(this.uscRoleMasterId, newAddedRoles, (role) => {
                        existedRole = role;
                    });
                    return $.Deferred<RoleModel>().resolve(existedRole);
                });
                instance.registerEventHandler(UscRoleRestEventType.SetFascicleVisibilityType, (visibilityType: VisibilityType) => {
                    this._fascicleVisibilityType = visibilityType;
                    return $.Deferred<void>().resolve();
                });
            });

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    this.deleteRoleFromModel(roleId);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    this.addRoleToModel(this.uscRoleId, newAddedRoles, (role) => {
                        existedRole = role;
                    });
                    if (!existedRole) {
                        this._selectedResponsibleRole = newAddedRoles[0];
                    }
                    return $.Deferred<RoleModel>().resolve(existedRole, true);
                });
            });
    }

    private addRoleToModel(toCheckControlId: string, newAddedRoles: RoleModel[], existedRoleCallback?: (RoleModel) => void): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(toCheckControlId)
            .done((instance) => {
                let existedRole: RoleModel = instance.existsRole(newAddedRoles);
                if (existedRole) {
                    alert(`Non è possibile selezionare il settore ${existedRole.Name} in quanto già presente come settore ${toCheckControlId == this.uscRoleMasterId ? "responsabile" : "autorizzato"} del fascicolo`);
                    existedRoleCallback(existedRole);
                    newAddedRoles = newAddedRoles.filter(x => x.IdRole !== existedRole.IdRole);
                }
                if (toCheckControlId === this.uscRoleMasterId) {
                    this.setCategoryRole(newAddedRoles[0].EntityShortId);
                    return this.addRole(newAddedRoles, false);
                }
            });
    }

    private addRole(newAddedRoles: RoleModel[], isMaster: boolean): void {
        if (!newAddedRoles.length)
            return;

        let fascicleRoles: FascicleRoleModel[] = [];
        if (this.getFascicleRolesToAdd()) {
            fascicleRoles = this.getFascicleRolesToAdd();
        }

        for (let newAddedRole of newAddedRoles) {
            let fascicleRole = new FascicleRoleModel();
            fascicleRole.IsMaster = isMaster;
            fascicleRole.Role = newAddedRole;
            fascicleRoles.push(fascicleRole);
        }
        this.setFascicleRolesToSession(fascicleRoles);
    }

    private getFascicleRolesToAdd(): FascicleRoleModel[] {
        let itemsFromSession: string = sessionStorage.getItem(`${this.clientId}_FascicleRolesToAdd`);
        if (itemsFromSession) {
            return JSON.parse(itemsFromSession) as FascicleRoleModel[];
        }
        return null;
    }

    private setFascicleRolesToSession(fascicleRoles: FascicleRoleModel[]): void {
        if (!fascicleRoles) {
            sessionStorage.removeItem(`${this.clientId}_FascicleRolesToAdd`);
        }

        sessionStorage[`${this.clientId}_FascicleRolesToAdd`] = JSON.stringify(fascicleRoles);
    }

    private deleteRoleFromModel(roleIdToDelete: number): void {
        if (!roleIdToDelete)
            return;

        let fascicleRoles: FascicleRoleModel[] = [];
        if (this.getFascicleRolesToAdd()) {
            fascicleRoles = this.getFascicleRolesToAdd();
        }

        fascicleRoles = fascicleRoles.filter(x => x.Role.IdRole !== roleIdToDelete && x.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
        this.setFascicleRolesToSession(fascicleRoles);
    }

    private populateFascicleRoles(): FascicleRoleModel[] {
        let fascicleRoles: FascicleRoleModel[] = this.getFascicleRolesToAdd();
        if (this._selectedResponsibleRole) {
            if (!fascicleRoles) {
                fascicleRoles = [];
            }
            fascicleRoles.push(<FascicleRoleModel>{
                Role: this._selectedResponsibleRole,
                IsMaster: true
            });
        }
        let uscRole: uscRoleRest = $(`#${this.uscRoleId}`).data();
        let raciRoles: RoleModel[] = uscRole.getRaciRoles();
        for (let fascicleRole of fascicleRoles) {
            fascicleRole.AuthorizationRoleType = (raciRoles && raciRoles.some(x => x.EntityShortId === fascicleRole.Role.EntityShortId)) || fascicleRole.IsMaster
                ? AuthorizationRoleType.Responsible
                : AuthorizationRoleType.Accounted;
        }
        return fascicleRoles;
    }

    private loadMetadataRepository(id: string, generateMetadataInputs: boolean) {
        PageClassHelper.callUserControlFunctionSafe<UscMetadataRepositorySel>(this.uscMetadataRepositorySelId).done((instance) => instance.setComboboxText(id, generateMetadataInputs));
    }

    private loadRoles(items: FascicleRoleModel[]) {
        let rolesModel: RoleModel[] = [];
        let masterRolesModel: RoleModel[] = [];
        let raciRoles: RoleModel[] = [];

        let fascicleRoles: FascicleRoleModel[] = this.getFascicleRolesToAdd() || [];

        for (let fascicleRole of items) {
            if (fascicleRole.IsMaster) {
                masterRolesModel.push(fascicleRole.Role);
            }
            else {
                if (fascicleRole.AuthorizationRoleType === AuthorizationRoleType.Responsible) {
                    raciRoles.push(fascicleRole.Role);
                }
                rolesModel.push(fascicleRole.Role);
            }
            fascicleRoles.push(fascicleRole);
        }

        this.setFascicleRolesToSession(fascicleRoles);
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId).done((instance) => {
            if (raciRoles) {
                instance.setRaciRoles(raciRoles);
            }
            instance.renderRolesTree(rolesModel);
        });
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId).done((instance) => instance.renderRolesTree(masterRolesModel));
    }

    loadActivityFascicleFields(fascicleTemplateModel: FascicleModel, generateMetadataInputs: boolean = true): void {
        this._txtObject.set_value(fascicleTemplateModel.FascicleObject);

        if (fascicleTemplateModel.MetadataRepository && fascicleTemplateModel.MetadataRepository.UniqueId) {
            this.loadMetadataRepository(fascicleTemplateModel.MetadataRepository.UniqueId, generateMetadataInputs);
        }

        else {
            PageClassHelper.callUserControlFunctionSafe<UscMetadataRepositorySel>(this.uscMetadataRepositorySelId).done((instance) => instance.clearComboboxText());
        }

        this.loadRoles(fascicleTemplateModel.FascicleRoles);

        if (fascicleTemplateModel.CustomActions) {
            PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
                .done((instance) => {
                    instance.loadItems(<FascicleCustomActionModel>JSON.parse(fascicleTemplateModel.CustomActions));
                });
        }
    }

    loadProcedureFascicleFields(fascicleTemplateModel: FascicleModel, generateMetadataInputs: boolean = true): void {
        this._txtObject.set_value(fascicleTemplateModel.FascicleObject);

        if (fascicleTemplateModel.MetadataRepository && fascicleTemplateModel.MetadataRepository.UniqueId) {
            this.loadMetadataRepository(fascicleTemplateModel.MetadataRepository.UniqueId, generateMetadataInputs);
        }
        else {
            PageClassHelper.callUserControlFunctionSafe<UscMetadataRepositorySel>(this.uscMetadataRepositorySelId).done((instance) => instance.clearComboboxText());
        }

        if (fascicleTemplateModel.FascicleRoles.filter(x => x.IsMaster)[0]) {
            PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId).done((instance) => instance.setToolbarVisibility(false));
        }
        else {
            PageClassHelper.callUserControlFunctionSafe<UscCategoryRest>(this.uscClassificatoreId).done((instance) => {
                //set popup roles source
                this._needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.DossierFolder.toString(), instance.getProcessFascicleTemplateFolderId()];
                $(`#${this.uscRoleMasterId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, this._needRolesFromExternalSource_eventArgs);
            });
        }

        if (fascicleTemplateModel.Contacts.length > 0) {
            let ajaxModel: AjaxModel = <AjaxModel>{
                ActionName: "SetDefaultContact",
                Value: [fascicleTemplateModel.Contacts[0].EntityId.toString()]
            };

            const ajaxManager: Telerik.Web.UI.RadAjaxManager = $find(this.ajaxManagerId) as Telerik.Web.UI.RadAjaxManager;
            if (ajaxManager) {
                // Workaround solution for updating VB contact user control
                setTimeout(() => {
                    console.log(JSON.stringify(ajaxModel));
                    ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                }, 500);
            }
        }

        this.loadRoles(fascicleTemplateModel.FascicleRoles);

        if (fascicleTemplateModel.CustomActions) {
            PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
                .done((instance) => {
                    instance.loadItems(<FascicleCustomActionModel>JSON.parse(fascicleTemplateModel.CustomActions));
                });
        }
    }

    populateInputs(fascicleTemplateModel: FascicleModel) {
        PageClassHelper.callUserControlFunctionSafe<UscCategoryRest>(this.uscClassificatoreId).done((instance) => {
            instance.addDefaultCategory(fascicleTemplateModel.Category.EntityShortId, false, false)
                .done(() => {
                    if (fascicleTemplateModel.FascicleType === FascicleType.Activity) {
                        this.loadActivityFascicleFields(fascicleTemplateModel, false);
                    }
                    else {
                        this.loadProcedureFascicleFields(fascicleTemplateModel, false);
                    }
                })
        });

        if (fascicleTemplateModel.FascicleType) {
            this._rdlFascicleType.findItemByValue(String(fascicleTemplateModel.FascicleType))?.select();
        }

        if (fascicleTemplateModel.Note) {
            this._txtNote.set_value(fascicleTemplateModel.Note);
        }

        if (fascicleTemplateModel.Conservation) {
            this._txtConservation.set_value(fascicleTemplateModel.Conservation.toString());
        }

        if (fascicleTemplateModel.MetadataDesigner && fascicleTemplateModel.MetadataValues) {
            PageClassHelper.callUserControlFunctionSafe<uscDynamicMetadataRest>(this.uscDynamicMetadataRestId)
                .done((instance) => {
                    instance.loadPageItems(fascicleTemplateModel.MetadataDesigner, fascicleTemplateModel.MetadataValues);
                });
        }
    }

}
export = uscFascicleInsert;
