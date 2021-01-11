/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import MassimarioScartoModel = require('App/Models/MassimariScarto/MassimarioScartoModel');
import MassimarioScartoStatusType = require('App/Models/MassimariScarto/MassimarioScartoStatusType');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MassimarioScartoService = require('App/Services/MassimariScarto/MassimarioScartoService');
import CategoryService = require('App/Services/Commons/CategoryService');
import uscMassimarioScarto = require('UserControl/uscMassimarioScarto');
import RadTreeNodeViewModel = require('App/ViewModels/Telerik/RadTreeNodeViewModel');
import WindowHelper = require('App/Helpers/WindowHelper');
import StringHelper = require('App/Helpers/StringHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

declare var Page_IsValid: any;
declare var ValidatorEnable: any;
class TbltMassimarioScartoGes {
    uscMassimarioScartoId: string;
    ajaxLoadingPanelId: string;
    splitterPageId: string;
    lblConservationPeriodId: string;
    pnlDetailsId: string;
    rwEditMassimarioId: string;
    txtNameId: string;
    txtCodeId: string;
    txtNoteId: string;
    txtPeriodId: string;
    btnInfiniteId: string;
    btnSaveMassimarioId: string;
    uscNotificationId: string;
    rgvCategoriesId: string;
    lblNoteId: string;
    lblStartDateId: string;
    lblEndDateId: string;
    rdpStartDateId: string;
    rdpEndDateId: string;
    pnlMetadataId: string;
    rfvNameId: string;
    rfvCodeId: string;
    rfvEndDateId: string;
    managerId: string;
    folderToolBarId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _massimarioScartoService: MassimarioScartoService;
    private _categoryService: CategoryService;
    private _uscMassimarioScarto: uscMassimarioScarto;
    private _folderToolBar: Telerik.Web.UI.RadToolBar;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _txtCode: Telerik.Web.UI.RadNumericTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _btnInfinite: Telerik.Web.UI.RadButton;
    private _txtPeriod: Telerik.Web.UI.RadNumericTextBox;
    private _btnSaveMassimario: Telerik.Web.UI.RadButton;
    private _rgvCategories: Telerik.Web.UI.RadGrid;
    private _rdpStartDate: Telerik.Web.UI.RadDatePicker;
    private _rdpEndDate: Telerik.Web.UI.RadDatePicker;
    private _manager: Telerik.Web.UI.RadWindowManager;

    private static CREATE_OPTION = "create";
    private static MODIFY_OPTION = "modify";
    private static DELETE_OPTION = "delete";
    private static RECOVER_OPTION = "recover";
    /**
     * Costruttore
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(function () {
        });
    }

    /**
     *------------------------- Events -----------------------------
     */
    folderToolBar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case TbltMassimarioScartoGes.CREATE_OPTION: {
                this.initializeInsertForm();
                this._btnSaveMassimario.set_commandArgument("Insert");
                this.openWindow();
                break;
            }
            case TbltMassimarioScartoGes.MODIFY_OPTION: {
                this._uscMassimarioScarto = <uscMassimarioScarto>$("#".concat(this.uscMassimarioScartoId)).data();
                let selectedModel: MassimarioScartoModel = this._uscMassimarioScarto.getSelectedMassimario();
                this.initializeEditForm(selectedModel);
                this._btnSaveMassimario.set_commandArgument("Edit");
                this.openWindow();
                break;
            }
            case TbltMassimarioScartoGes.DELETE_OPTION: {
                this.initializeCancelForm();
                this._btnSaveMassimario.set_commandArgument("Cancel");
                this.openWindow();
                break;
            }
            case TbltMassimarioScartoGes.RECOVER_OPTION: {
                this._uscMassimarioScarto = <uscMassimarioScarto>$("#".concat(this.uscMassimarioScartoId)).data();
                let selectedModel: MassimarioScartoModel = this._uscMassimarioScarto.getSelectedMassimario();
                this.initializeRecoverForm(selectedModel);
                this._btnSaveMassimario.set_commandArgument("Recover");
                this.openWindow();
                break;
            }
        }
    }

    /**
     * Evento scatenato al click del pulsante Conferma
     * @method
     * @param sender
     * @param eventArgs
     * @returns
     */
    btnSaveMassimario_Clicking = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonCancelEventArgs) => {
        eventArgs.set_cancel(true);
        if (!Page_IsValid) {
            return false;
        }

        this._btnSaveMassimario.set_enabled(false);
        this._uscMassimarioScarto = <uscMassimarioScarto>$("#".concat(this.uscMassimarioScartoId)).data();
        let selectedModel: MassimarioScartoModel = this._uscMassimarioScarto.getSelectedMassimario();
        let model: MassimarioScartoModel = undefined;
        let argument: string = this._btnSaveMassimario.get_commandArgument();
        if (argument != "Cancel") {
            model = this.fillModelFromPage();
        }

        this.showLoadingPanel(this.pnlMetadataId);
        //Salvo l'elemento tramite le web api        
        switch (argument) {
            case "Insert":
                model.FakeInsertId = selectedModel.UniqueId;
                this._massimarioScartoService.insertMassimario(model, this.insertMassimarioCallback,
                    (exception: ExceptionDTO) => {
                        this.showNotificationException(this.uscNotificationId, exception);
                        this._btnSaveMassimario.set_enabled(true);
                        this.closeLoadingPanel(this.pnlMetadataId);
                    });
                break;
            case "Edit":
                model.FakeInsertId = selectedModel.FakeInsertId;
                model.UniqueId = selectedModel.UniqueId;
                this._massimarioScartoService.updateMassimario(model, this.updateMassimarioCallback,
                    (exception: ExceptionDTO) => {
                        this.showNotificationException(this.uscNotificationId, exception);
                        this._btnSaveMassimario.set_enabled(true);
                        this.closeLoadingPanel(this.pnlMetadataId);
                    });
                break;
            case "Recover":
                model.FakeInsertId = selectedModel.FakeInsertId;
                model.UniqueId = selectedModel.UniqueId;
                model.EndDate = null;
                model.Status = MassimarioScartoStatusType.Active;
                this._massimarioScartoService.updateMassimario(model, this.updateMassimarioCallback,
                    (exception: ExceptionDTO) => {
                        this.showNotificationException(this.uscNotificationId, exception);
                        this._btnSaveMassimario.set_enabled(true);
                        this.closeLoadingPanel(this.pnlMetadataId);
                    });
                break;
            case "Cancel":
                this._uscMassimarioScarto = <uscMassimarioScarto>$("#".concat(this.uscMassimarioScartoId)).data();
                selectedModel.Status = MassimarioScartoStatusType.LogicalDelete;
                let tmp = this._rdpEndDate.get_selectedDate();
                selectedModel.EndDate = moment.utc(this._rdpEndDate.get_selectedDate()).hours(24).minutes(0).seconds(0).milliseconds(0).toDate();
                this._massimarioScartoService.updateMassimario(selectedModel, this.updateMassimarioCallback,
                    (exception: ExceptionDTO) => {
                        this.showNotificationException(this.uscNotificationId, exception);
                        this._btnSaveMassimario.set_enabled(true);
                        this.closeLoadingPanel(this.pnlMetadataId);
                    });
                break;
        }
    }

    /**
     * Evento scatenato all'inserimento di un valore nella radnumerictextbox
     * @method
     * @param sender
     * @param eventArgs
     * @returns
     */
    txtPeriod_KeyPress = (sender: Telerik.Web.UI.RadNumericTextBox, eventArgs: Telerik.Web.UI.InputValueChangedEventArgs) => {
        this._btnInfinite.set_checked(String.isNullOrEmpty(sender.get_value()));
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
 * Metodo di Gestione dell'errore
 */
    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    /**
 * Metodi di Gestione dell'errore
 */
    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }


    /**
     * Callback da inserimento nuovo massimario
     */
    insertMassimarioCallback = (data: any) => {
        this.closeLoadingPanel(this.pnlMetadataId);
        this._uscMassimarioScarto.updateSelectedNodeChildren();
        let selectedModel: MassimarioScartoModel = this._uscMassimarioScarto.getSelectedMassimario();
        if (selectedModel.UniqueId) {
            this.setButtonVisibility(selectedModel);
        }
        else {
            this.setButtonVisibility(0, true);
        }
        this._btnSaveMassimario.set_enabled(true);
        this.closeWindow();
    }

    /**
     * Callback da modifica massimario
     */
    updateMassimarioCallback = (data: any) => {
        this.closeLoadingPanel(this.pnlMetadataId);
        this._uscMassimarioScarto.updateParentNode(() => {
            this.hideDetailsPanel();
            let selectedModel: MassimarioScartoModel = this._uscMassimarioScarto.getSelectedMassimario();
            if (selectedModel.UniqueId) {
                this.setButtonVisibility(selectedModel);
            }
            else {
                this.setButtonVisibility(0, true);
            }
            this._btnSaveMassimario.set_enabled(true);
            this.closeWindow();
        });
    }

    /**
     * Metodo di inizializzazione pagina
     */
    private initialize(): void {
        this._folderToolBar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolBarId);
        this._folderToolBar.add_buttonClicked(this.folderToolBar_onClick);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtCode = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtCodeId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._btnInfinite = <Telerik.Web.UI.RadButton>$find(this.btnInfiniteId);
        this._txtPeriod = <Telerik.Web.UI.RadNumericTextBox>$find(this.txtPeriodId);
        this._txtPeriod.add_valueChanged(this.txtPeriod_KeyPress);
        this._btnSaveMassimario = <Telerik.Web.UI.RadButton>$find(this.btnSaveMassimarioId);
        this._btnSaveMassimario.add_clicking(this.btnSaveMassimario_Clicking);
        this._rgvCategories = <Telerik.Web.UI.RadGrid>$find(this.rgvCategoriesId);
        this._rdpStartDate = <Telerik.Web.UI.RadDatePicker>$find(this.rdpStartDateId);
        this._rdpEndDate = <Telerik.Web.UI.RadDatePicker>$find(this.rdpEndDateId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);

        let massimarioScartoConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MassimarioScarto");
        this._massimarioScartoService = new MassimarioScartoService(massimarioScartoConfiguration);

        let categoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Category");
        this._categoryService = new CategoryService(categoryConfiguration);

        this.hideDetailsPanel()
        $("#".concat(this.uscMassimarioScartoId)).on(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, (args, data) => {
            if (data != undefined) {
                let node: RadTreeNodeViewModel = new RadTreeNodeViewModel();
                node.fromJson(data);
                if (node.value != 0) {
                    this.loadMassimarioDetails(node.attributes.UniqueId);
                    this.setButtonVisibility(node.attributes.MassimarioScartoLevel, node.attributes.IsActive);
                } else {
                    this.hideDetailsPanel();
                    this.setButtonVisibility(0, true);
                }
            }
        });

        $("#".concat(this.uscMassimarioScartoId)).on(uscMassimarioScarto.ON_START_LOAD_EVENT, (args) => {
            this.showLoadingPanel(this.splitterPageId);
        });

        this.showLoadingPanel(this.splitterPageId);
        $("#".concat(this.uscMassimarioScartoId)).on(uscMassimarioScarto.ON_END_LOAD_EVENT, (args) => {
            this.closeLoadingPanel(this.splitterPageId);
        });

        $("#".concat(this.uscMassimarioScartoId)).on(uscMassimarioScarto.ON_ERROR_EVENT, (args, exception) => {
            this.showNotificationException(this.uscNotificationId, exception);
            this.closeLoadingPanel(this.splitterPageId);
        });
    }

    /**
      * Visualizza un nuovo loading panel nella pagina
      */
    private showLoadingPanel(updatedElementId: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.show(updatedElementId);
    }

    /**
     * Nasconde il loading panel nella pagina
     */
    private closeLoadingPanel(updatedElementId: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.hide(updatedElementId);
    }

    /**
     * Metodo che recupera i metadati di un massimario e li imposta nella pagina.
     * Gestisce anche le logiche di visualizzazione dei pulsanti e pannelli nella pagina.
     * @param massimarioId
     */
    private loadMassimarioDetails(massimarioId: string) {
        this.showLoadingPanel(this.pnlDetailsId);
        this._massimarioScartoService.getMassimarioById(massimarioId,
            (data: any) => {
                if (data == null) return;
                let massimario: MassimarioScartoModel = <MassimarioScartoModel>data;
                this._categoryService.getByIdMassimarioScarto(massimarioId,
                    (data: any) => {
                        let categories: CategoryModel[] = <CategoryModel[]>data;
                        if (categories == undefined) {
                            categories = new Array<CategoryModel>();
                        }

                        this.setDetailPanelControls(massimario, categories);
                        this.showDetailsPanel();
                        this.closeLoadingPanel(this.pnlDetailsId);
                    },
                    (exception: ExceptionDTO) => {
                        this.closeLoadingPanel(this.pnlDetailsId);
                        this.showNotificationException(this.uscNotificationId, exception);
                    }
                );
            },
            (exception: ExceptionDTO) => {
                this.closeLoadingPanel(this.pnlDetailsId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * Imposta i valori per i controlli del pannello dei dettagli di un massimario
     * @param massimario
     * @param categories
     */
    private setDetailPanelControls(massimario: MassimarioScartoModel, categories: CategoryModel[]) {
        $("#".concat(this.lblConservationPeriodId)).html(massimario.getPeriodLabel());
        massimario.MassimarioScartoLevel != 2 ? $('#detailPeriodSection').hide() : $('#detailPeriodSection').show();
        $("#".concat(this.lblNoteId)).html(massimario.Note);
        $("#".concat(this.lblStartDateId)).html(moment(massimario.StartDate).format("DD/MM/YYYY"));
        $("#".concat(this.lblEndDateId)).html('');
        if (massimario.EndDate != undefined) {
            $("#".concat(this.lblEndDateId)).html(moment(massimario.EndDate).format("DD/MM/YYYY"));
        }

        let masterTable = this._rgvCategories.get_masterTableView();
        masterTable.set_dataSource(categories);
        masterTable.dataBind();
    }

    /**
     * Metodo che setta la visibilità dei pulsanti
     * @param massimarioLevel
     * @param status
     * @param massimario
     */
    private setButtonVisibility(massimario: MassimarioScartoModel)
    private setButtonVisibility(massimarioLevel: number, active: boolean)
    private setButtonVisibility(massimarioModelOrLevel: any, active?: boolean) {
        let massimarioLevel: number = 0;
        if (massimarioModelOrLevel instanceof MassimarioScartoModel) {
            let model: MassimarioScartoModel = <MassimarioScartoModel>massimarioModelOrLevel;
            if (model.MassimarioScartoLevel != undefined && model.Status != undefined) {
                massimarioLevel = model.MassimarioScartoLevel;
                active = model.isActive();
            }
        } else {
            massimarioLevel = <number>massimarioModelOrLevel;
        }

        this._uscMassimarioScarto = <uscMassimarioScarto>$("#".concat(this.uscMassimarioScartoId)).data();

        this._folderToolBar.findItemByValue(TbltMassimarioScartoGes.CREATE_OPTION).set_enabled(massimarioLevel < 2 && active);
        this._folderToolBar.findItemByValue(TbltMassimarioScartoGes.MODIFY_OPTION).set_enabled(massimarioLevel > 0 && active);
        this._folderToolBar.findItemByValue(TbltMassimarioScartoGes.DELETE_OPTION).set_enabled(massimarioLevel > 0 && active && this._uscMassimarioScarto.allSelectedChildrenIsCancel());
        this._folderToolBar.findItemByValue(TbltMassimarioScartoGes.RECOVER_OPTION).set_enabled(!active);
    }

    /**               
     * Apre una nuova radwindow con dati personalizzati
     * @method
     * @param url
     * @param name
     * @param width
     * @param height
     * @returns
     */
    openWindow(): boolean {
        let wnd: Telerik.Web.UI.RadWindow = <Telerik.Web.UI.RadWindow>$find(this.rwEditMassimarioId);
        wnd.setSize(WindowHelper.WIDTH_EDIT_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
        wnd.show();
        return false;
    }

    /**
     * Inizializza la form per l'inserimento di un massimario
     */
    private initializeInsertForm(): void {
        let wnd: Telerik.Web.UI.RadWindow = <Telerik.Web.UI.RadWindow>$find(this.rwEditMassimarioId);
        wnd.set_title("Inserisci massimario di scarto");

        this._txtName.clear();
        this._txtCode.clear();
        (<any>this._txtCode.get_element()).readOnly = false;
        this._txtNote.clear();
        this._txtPeriod.clear();
        this._btnInfinite.set_checked(true);
        this._rdpStartDate.set_selectedDate(new Date());
        this._rdpEndDate.set_enabled(false);
        this._rdpEndDate.clear();

        this._uscMassimarioScarto = <uscMassimarioScarto>$("#".concat(this.uscMassimarioScartoId)).data();
        let selectedModel: MassimarioScartoModel = this._uscMassimarioScarto.getSelectedMassimario();
        this.displayRowsForm("Insert", selectedModel.MassimarioScartoLevel != 1);

        //Reset validator
        ValidatorEnable($get(this.rfvCodeId), true);
        ValidatorEnable($get(this.rfvNameId), true);
        $("#".concat(this.rfvNameId)).hide();
        $("#".concat(this.rfvCodeId)).hide();
        ValidatorEnable($get(this.rfvEndDateId), false);
    }

    /**
     * Inizializza la form per la modifica di un massimario
     * @param model
     */
    private initializeEditForm(model: MassimarioScartoModel): void {
        let wnd: Telerik.Web.UI.RadWindow = <Telerik.Web.UI.RadWindow>$find(this.rwEditMassimarioId);
        wnd.set_title("Modifica massimario di scarto");

        this._txtName.set_value(model.Name);
        this._txtCode.set_value(model.Code.toString());
        (<any>this._txtCode.get_element()).readOnly = true;
        this._txtNote.set_value(model.Note);
        this._rdpStartDate.set_selectedDate(moment(model.StartDate).startOf('day').toDate());
        this.displayRowsForm("Edit", model.MassimarioScartoLevel != 2);

        let minDate: Date = moment().startOf('day').toDate();
        this._rdpEndDate.set_enabled(true);
        if (model.EndDate != undefined)
            this._rdpEndDate.set_selectedDate(moment(model.EndDate).startOf('day').toDate());
        else
            this._rdpEndDate.set_selectedDate(undefined);

        ValidatorEnable($get(this.rfvCodeId), true);
        ValidatorEnable($get(this.rfvNameId), true);
        ValidatorEnable($get(this.rfvEndDateId), false);

        if (model.ConservationPeriod != undefined) {
            this._btnInfinite.set_checked(model.ConservationPeriod == -1);
            this._txtPeriod.set_value(model.ConservationPeriod != -1 ? model.getPeriodLabel() : undefined);
        } else {
            this._txtPeriod.clear();
            this._btnInfinite.set_checked(true);
        }
    }

    /**
     * Inizializza la form per la cancellazione di un Massimario
     */
    private initializeCancelForm(): void {
        let wnd: Telerik.Web.UI.RadWindow = <Telerik.Web.UI.RadWindow>$find(this.rwEditMassimarioId);
        wnd.set_title("Elimina massimario di scarto");

        let minDate: Date = moment().startOf('day').toDate();
        this._rdpEndDate.set_enabled(true);
        this._rdpEndDate.set_selectedDate(minDate);
        this._rdpEndDate.set_minDate(minDate);
        this.displayRowsForm("Cancel");

        ValidatorEnable($get(this.rfvCodeId), false);
        ValidatorEnable($get(this.rfvNameId), false);
        ValidatorEnable($get(this.rfvEndDateId), true);
    }

    private initializeRecoverForm(model: MassimarioScartoModel): void {
        let wnd: Telerik.Web.UI.RadWindow = <Telerik.Web.UI.RadWindow>$find(this.rwEditMassimarioId);
        wnd.set_title("Recupera massimario di scarto");

        if (model) {
            this._txtName.set_value(model.Name);
            (<any>this._txtName.get_element()).readOnly = true;
            this._txtCode.set_value(model.Code.toString());
            (<any>this._txtCode.get_element()).readOnly = true;
            this._txtNote.set_value(model.Note);
            (<any>this._txtNote.get_element()).readOnly = true;
            this._rdpStartDate.set_selectedDate(moment(model.StartDate).startOf('day').toDate());
            this._rdpStartDate.set_enabled(false);

            (<any>this._txtPeriod.get_element()).readOnly = true;
            if (model.ConservationPeriod != undefined) {
                this._btnInfinite.set_checked(model.ConservationPeriod == -1);
                this._txtPeriod.set_value(model.ConservationPeriod != -1 ? model.getPeriodLabel() : undefined);
            } else {
                this._txtPeriod.clear();
                this._btnInfinite.set_checked(true);
            }

            this.displayRowsForm("Recover", model.MassimarioScartoLevel != 2);

            ValidatorEnable($get(this.rfvCodeId), false);
            ValidatorEnable($get(this.rfvNameId), false);
            ValidatorEnable($get(this.rfvEndDateId), false);
        }
    }

    private displayRowsForm(commandName: string): void
    private displayRowsForm(commandName: string, hidePeriods: boolean): void
    private displayRowsForm(commandName: string, hidePeriods?: boolean): void {
        switch (commandName) {
            case "Insert":
            case "Edit":
                $('#rowEndDate').show();
                $('#rowName').show();
                $('#rowCode').show();
                $('#rowNote').show();
                hidePeriods ? $('#rowConservationPeriod').hide() : $('#rowConservationPeriod').show();
                $('#rowStartDate').show();
                break;
            case "Recover":
                $('#rowEndDate').hide();
                $('#rowName').show();
                $('#rowCode').show();
                $('#rowNote').show();
                $('#rowConservationPeriod').show();
                $('#rowStartDate').show();
                break;
            case "Cancel":
                $('#rowEndDate').show();
                $('#rowName').hide();
                $('#rowCode').hide();
                $('#rowNote').hide();
                $('#rowConservationPeriod').hide();
                $('#rowStartDate').hide();
                break;
        }
    }

    /**
     * Chiude la radwindow attualmente aperta
     */
    private closeWindow(): void {
        let wnd: Telerik.Web.UI.RadWindow = <Telerik.Web.UI.RadWindow>$find(this.rwEditMassimarioId);
        wnd.close();
    }

    /**
     * Nasconde il pannello dei dettagli
     */
    private hideDetailsPanel(): void {
        $('#'.concat(this.pnlDetailsId)).hide();
    }

    /**
     * Visualizza il pannello dei dettagli
     */
    private showDetailsPanel(): void {
        $('#'.concat(this.pnlDetailsId)).show();
    }

    /**
     * Esegue il fill dei controlli della pagina in un nuovo modello MassimarioScartoModel
     */
    private fillModelFromPage(): MassimarioScartoModel {
        let model: MassimarioScartoModel = new MassimarioScartoModel();
        model.Code = Number(this._txtCode.get_value());
        model.Name = this._txtName.get_value();
        model.Note = this._txtNote.get_value();

        if (!String.isNullOrEmpty(this._txtPeriod.get_value())) {
            model.ConservationPeriod = Number(this._txtPeriod.get_value());
        } else {
            model.ConservationPeriod = -1;
        }

        model.StartDate = moment.utc(this._rdpStartDate.get_selectedDate()).hours(24).minutes(0).seconds(0).milliseconds(0).toDate();
        model.EndDate = moment.utc(this._rdpEndDate.get_selectedDate()).hours(24).minutes(0).seconds(0).milliseconds(0).toDate();
        model.Status = MassimarioScartoStatusType.Active;
        return model;
    }
}

export = TbltMassimarioScartoGes;