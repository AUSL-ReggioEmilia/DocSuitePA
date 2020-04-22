/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import UscReportDesignerInformation = require('UserControl/uscReportDesignerInformation');
import UscReportDesigner = require('UserControl/uscReportDesigner');
import UscReportDesignerToolbox = require('UserControl/uscReportDesignerToolbox');
import Environment = require('App/Models/Environment');
import ReportInformationViewModel = require('App/ViewModels/Reports/ReportInformationViewModel');
import ReportBuilderModel = require('App/Models/Reports/ReportBuilderModel');
import ReportBuilderProjectionModel = require('App/Models/Reports/ReportBuilderProjectionModel');
import ReportBuilderConditionModel = require('App/Models/Reports/ReportBuilderConditionModel');
import ReportBuilderSortModel = require('App/Models/Reports/ReportBuilderSortModel');
import ReportBuilderPropertyModel = require('App/Models/Reports/ReportBuilderPropertyModel');
import ReportToolboxItemViewModel = require('App/ViewModels/Reports/ReportToolboxItemViewModel');
import ReportToolboxItemType = require('App/ViewModels/Reports/ReportToolboxItemType');
import ReportBuilderPropertyModelMapper = require('App/Mappers/Reports/ReportBuilderPropertyModelMapper');
import ReportBuilderConditionModelMapper = require('App/Mappers/Reports/ReportBuilderConditionModelMapper');
import TemplateReportService = require('App/Services/Templates/TemplateReportService');
import TemplateReportModel = require('App/Models/Templates/TemplateReportModel');
import TemplateReportStatus = require('App/Models/Templates/TemplateReportStatus');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ReportInformationViewModelMapper = require('App/Mappers/Reports/ReportInformationViewModelMapper');

declare var Page_ClientValidate: any;
class ReportDesigner {
    uscNotificationId: string;
    uscReportInformationId: string;
    uscReportDesignerId: string;
    uscReportToolboxId: string;
    reportUniqueId: string;
    btnDraftId: string;
    btnSaveId: string;
    splPageId: string;
    ajaxLoadingPanelId: string;
    toolboxItems: ReportToolboxItemViewModel[];

    private _propertyMapper: ReportBuilderPropertyModelMapper;
    private _conditionMapper: ReportBuilderConditionModelMapper;
    private _informationViewModelMapper: ReportInformationViewModelMapper;
    private _btnDraft: Telerik.Web.UI.RadButton;
    private _btnSave: Telerik.Web.UI.RadButton;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _service: TemplateReportService;

    static TEMPLATE_REPORT_NAME: string = "TemplateReport";

    private get action(): string {
        return (!this.reportUniqueId) ? ReportDesigner.INSERT_ACTION : ReportDesigner.EDIT_ACTION;
    }

    static INSERT_ACTION: string = "Insert";
    static EDIT_ACTION: string = "Edit";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let service: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, ReportDesigner.TEMPLATE_REPORT_NAME);
        if (!service) {
            this.showNotification("Nessun servizio configurato per la gestione dei report.");
        }

        this._service = new TemplateReportService(service);
        this._propertyMapper = new ReportBuilderPropertyModelMapper();
        this._conditionMapper = new ReportBuilderConditionModelMapper();
        this._informationViewModelMapper = new ReportInformationViewModelMapper();
    }

    /**
     *------------------------- Events -----------------------------
     */
    private BtnDraft_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (Page_ClientValidate("ReportData")) {
            this.showLoading();
            $.when(this.saveTemplate(true))
                .done(() => {
                    
                })
                .fail((ex: ExceptionDTO) => {
                    this.showNotification(ex);
                })
                .always(() => {
                    this.hideLoading();
                });
        }
    }

    private BtnSave_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (Page_ClientValidate("ReportData")) {
            this.showLoading();
            $.when(this.saveTemplate(false))
                .done(() => {

                })
                .fail((ex: ExceptionDTO) => {
                    this.showNotification(ex);
                })
                .always(() => {
                    this.hideLoading();
                });
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */
    initialize() {
        this._btnDraft = $find(this.btnDraftId) as Telerik.Web.UI.RadButton;
        this._btnDraft.add_clicked(this.BtnDraft_OnClick);
        this._btnSave = $find(this.btnSaveId) as Telerik.Web.UI.RadButton;
        this._btnSave.add_clicked(this.BtnSave_OnClick);
        this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;

        this.initializeControls();
        this.checkCompletedLoad();
    }

    private checkCompletedLoad(): void {
        this.showLoading();
        $.when(this.checkLoadDesigner(), this.checkLoadInformations(), this.checkLoadToolbox())
            .done(() => {
                if (!this.reportUniqueId) {
                    this.hideLoading();
                    return;
                }
                $.when(this.loadCurrentReportTemplate(this.reportUniqueId))
                    .always(() => {
                        this.hideLoading();
                    });
            })
            .fail(() => {
                this.hideLoading();
            });
    }

    private initializeControls(): void {
        this.initializeDesignerControl();
        this.initializeInformationControl();
        this.initializeToolboxControl();
    }

    private initializeDesignerControl(): void {
        $("#".concat(this.uscReportDesignerId)).bind(UscReportDesigner.ON_CHANGED_VIEW, (args) => {
            this.loadToolbox();
        });
    }

    private initializeInformationControl(): void {
        $("#".concat(this.uscReportInformationId)).bind(UscReportDesignerInformation.ON_EXECUTE_LOAD_EVENT, (args) => {
            //TODO: Gestire caricamento Tag e informazioni
            this.loadDesigner();
            this.loadToolbox();
        });
    }

    private initializeToolboxControl(): void {
        $("#".concat(this.uscReportToolboxId)).on(UscReportDesignerToolbox.ON_NODE_DROPPING, (evt, sender, args, item) => {
            let uscReportDesigner: UscReportDesigner = <UscReportDesigner>$("#".concat(this.uscReportDesignerId)).data();
            uscReportDesigner.Designer_DroppingItems(sender, args, item);
        });
    }

    private initializeInformations(model: ReportInformationViewModel)
    private initializeInformations()
    private initializeInformations(model?: any) {
        let uscReportInformations: UscReportDesignerInformation = <UscReportDesignerInformation>$("#".concat(this.uscReportInformationId)).data();
        const environments: Environment[] = [Environment.Fascicle];
        const documentUnits: Environment[] = [Environment.Protocol];

        if (!model) {
            model = new ReportInformationViewModel();
        }        
        model.Environments = environments;
        model.DocumentUnits = documentUnits;

        uscReportInformations.loadInformations(model);
    }

    private checkLoadInformations(): JQueryPromise<any> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let uscReportInformations: UscReportDesignerInformation = <UscReportDesignerInformation>$("#".concat(this.uscReportInformationId)).data();
        if (!jQuery.isEmptyObject(uscReportInformations)) {
            this.initializeInformations();
            promise.resolve();
        }

        $("#".concat(this.uscReportInformationId)).bind(UscReportDesignerInformation.ON_END_LOAD_EVENT, (args) => {
            this.initializeInformations();
            promise.resolve();
        });
        return promise.promise();
    }

    private checkLoadToolbox(): JQueryPromise<any> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let uscReportToolbox: UscReportDesignerToolbox = <UscReportDesignerToolbox>$("#".concat(this.uscReportToolboxId)).data();
        if (!jQuery.isEmptyObject(uscReportToolbox)) {
            promise.resolve();
        }

        $("#".concat(this.uscReportToolboxId)).bind(UscReportDesignerToolbox.ON_END_LOAD_EVENT, (args) => {
            promise.resolve();
        })
        return promise.promise();
    }

    private checkLoadDesigner(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let uscReportDesigner: UscReportDesigner = <UscReportDesigner>$("#".concat(this.uscReportDesignerId)).data();
        if (!jQuery.isEmptyObject(uscReportDesigner)) {
            promise.resolve();
        }

        $("#".concat(this.uscReportDesignerId)).bind(UscReportDesigner.ON_END_LOAD_EVENT, (args) => {
            promise.resolve();
        })
        return promise.promise();
    }

    private loadCurrentReportTemplate(uniqueId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._service.getById(uniqueId,
            (data: any) => {
                try {
                    let templateReport: TemplateReportModel = data as TemplateReportModel;
                    let reportInformationModel: ReportInformationViewModel = this._informationViewModelMapper.Map(templateReport);
                    this.initializeInformations(reportInformationModel);
                    this.loadToolbox();

                    let reportBuilderModel: ReportBuilderModel = JSON.parse(templateReport.ReportBuilderJsonModel);
                    this.loadDesigner(reportBuilderModel);

                    promise.resolve();
                } catch (e) {
                    console.error(e);
                    promise.reject();
                }                
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            });
        return promise.promise();
    }

    private loadDesigner(model: ReportBuilderModel)
    private loadDesigner()
    private loadDesigner(model?: any) {
        let uscReportDesigner: UscReportDesigner = <UscReportDesigner>$("#".concat(this.uscReportDesignerId)).data();
        if (!model) {
            model = new ReportBuilderModel();
            let condition = new ReportBuilderConditionModel();
            condition.ConditionName = "HasPecMails";
            let condition2 = new ReportBuilderConditionModel();
            condition2.ConditionName = "HasPecMails2";
            //model.Conditions = [condition, condition2];

            let projection: ReportBuilderProjectionModel = new ReportBuilderProjectionModel();
            let projection1: ReportBuilderProjectionModel = new ReportBuilderProjectionModel();
            let property1 = new ReportBuilderPropertyModel();
            property1.Name = "Prop1";
            property1.DisplayName = "Proprietà";

            let property2 = new ReportBuilderPropertyModel();
            property2.Name = "Prop2";
            property2.DisplayName = "Proprietà 2";

            let property3 = new ReportBuilderPropertyModel();
            property3.Name = "Prop3";
            property3.DisplayName = "Proprietà 3";

            //projection.ReportProperties = [property1, property2];
            projection.Alias = "Verifica";
            projection.TagName = "Test";
            //projection1.ReportProperties = [property3];
            projection1.TagName = "Test1";

            model.Projections = [projection, projection1];
        }
        uscReportDesigner.loadDesignerModel(model);
    }

    private loadToolbox(): void {
        let uscReportDesigner: UscReportDesigner = <UscReportDesigner>$("#".concat(this.uscReportDesignerId)).data();
        let uscReportToolbox: UscReportDesignerToolbox = <UscReportDesignerToolbox>$("#".concat(this.uscReportToolboxId)).data();
        let uscReportInformations: UscReportDesignerInformation = <UscReportDesignerInformation>$("#".concat(this.uscReportInformationId)).data();
        $.when(uscReportInformations.getInformations())
            .done((informationModel: ReportInformationViewModel) => {
                try {
                    if (!informationModel || !informationModel.SelectedEnvironment) {
                        return;
                    }

                    let selectedView: string = uscReportDesigner.currentActiveView();
                    let toolboxItemType: ReportToolboxItemType = (selectedView == UscReportDesigner.PROJECTIONS_VIEW) ? ReportToolboxItemType.Projection : ReportToolboxItemType.Condition;
                    let toolboxItemViewModels: ReportToolboxItemViewModel[] = JSON.parse(JSON.stringify(this.toolboxItems));

                    toolboxItemViewModels = toolboxItemViewModels.filter(m => m.Environment == informationModel.SelectedEnvironment && m.ItemType == toolboxItemType);
                    if (informationModel.SelectedEnvironment == Environment.Fascicle && selectedView == UscReportDesigner.PROJECTIONS_VIEW) {
                        for (let toolboxItemViewModel of toolboxItemViewModels) {
                            toolboxItemViewModel.ReportItems = toolboxItemViewModel.ReportItems.concat(informationModel.MetadataProperties);
                        }
                    }

                    if (informationModel.SelectedDocumentUnit) {
                        let documentUnitToolboxItems: ReportToolboxItemViewModel[] = this.toolboxItems.filter(m => m.Environment == informationModel.SelectedDocumentUnit && m.ItemType == toolboxItemType)
                        for (let documentUnitToolboxItem of documentUnitToolboxItems) {
                            toolboxItemViewModels.push(documentUnitToolboxItem);
                        }
                    }

                    for (let toolboxItem of toolboxItemViewModels) {
                        if (toolboxItemType == ReportToolboxItemType.Projection) {
                            toolboxItem.ReportItems = this._propertyMapper.MapCollection(toolboxItem.ReportItems);
                        } else if (toolboxItemType == ReportToolboxItemType.Condition) {
                            toolboxItem.ReportItems = this._conditionMapper.MapCollection(toolboxItem.ReportItems);
                        }
                    }
                    uscReportToolbox.loadToolbox(toolboxItemViewModels);
                } catch (e) {
                    console.error(e);
                    this.showNotification("E' avvenuto un errore durante la fase di caricamento elementi nella toolbox.");
                }
            })
            .fail(() => {
                this.showNotification("E' avvenuto un errore nel recupero delle informazioni del report corrente.");
            });
    }

    private showNotification(message: string): void
    private showNotification(error: ExceptionDTO): void
    private showNotification(error: any): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (error instanceof ExceptionDTO) {
                uscNotification.showNotification(error);
            } else {
                uscNotification.showNotificationMessage(error);
            }
        }
    }

    private saveTemplate(draft: boolean): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let saveAction: Function = (!this.reportUniqueId) ? (m, c, e) => this._service.insertTemplateReport(m, c, e) : (m, c, e) => this._service.updateTemplateReport(m, c, e);
        let uscReportDesigner: UscReportDesigner = <UscReportDesigner>$("#".concat(this.uscReportDesignerId)).data();
        let uscReportInformations: UscReportDesignerInformation = <UscReportDesignerInformation>$("#".concat(this.uscReportInformationId)).data();
        $.when(uscReportInformations.getInformations())
            .done((informationModel: ReportInformationViewModel) => {
                if (!informationModel.Name) {
                    let ex: ExceptionDTO = new ExceptionDTO();
                    ex.statusText = "Specificare il nome del report.";
                    promise.reject(ex);
                    return;
                }

                if (!informationModel.SelectedEnvironment) {
                    let ex: ExceptionDTO = new ExceptionDTO();
                    ex.statusText = "Specificare almeno una tipologia.";
                    promise.reject(ex);
                    return;
                }

                let builderModel: ReportBuilderModel = uscReportDesigner.getDesignerModel();
                builderModel.Entity = informationModel.SelectedEnvironment;
                builderModel.UDType = informationModel.SelectedDocumentUnit;
                let metadataRepository: MetadataRepositoryModel = new MetadataRepositoryModel();
                metadataRepository.UniqueId = informationModel.SelectedMetadata;
                builderModel.MetadataRepository = metadataRepository;

                let toSave: TemplateReportModel = new TemplateReportModel();
                if (this.action == ReportDesigner.EDIT_ACTION) {
                    toSave.UniqueId = this.reportUniqueId;
                }
                toSave.Environment = informationModel.SelectedEnvironment;
                toSave.Name = informationModel.Name;
                toSave.Status = (draft) ? TemplateReportStatus.Draft : TemplateReportStatus.Active;
                toSave.ReportBuilderJsonModel = JSON.stringify(builderModel);

                saveAction(toSave,
                    (data: any) => {
                        this.reportUniqueId = data.UniqueId;
                        this.loadCurrentReportTemplate(data.UniqueId);
                        promise.resolve();
                    },
                    (exception: ExceptionDTO) => {
                        promise.reject(exception);
                    });
            })
            .fail(() => {
                let ex: ExceptionDTO = new ExceptionDTO();
                ex.statusText = "Errore nel recupero delle informazioni del report corrente.";
                promise.reject(ex);
            });
        return promise.promise();
    }

    private showLoading(): void {
        this._ajaxLoadingPanel.show(this.splPageId);
    }

    private hideLoading(): void {
        this._ajaxLoadingPanel.hide(this.splPageId);
    }
}

export = ReportDesigner;