/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import TemplateReportService = require('App/Services/Templates/TemplateReportService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateReportModel = require('App/Models/Templates/TemplateReportModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ReportBuilderModel = require('App/Models/Reports/ReportBuilderModel');
import UscReportDesigner = require('UserControl/uscReportDesigner');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class ReportDesignerList {
    rtvReportsId: string;
    uscReportDesignerId: string;
    uscNotificationId: string;
    btnNewId: string;
    btnEditId: string;
    ajaxLoadingPanelId: string;
    splPageId: string;
    toolBarSearchId: string;

    private _service: TemplateReportService;
    private _rtvReports: Telerik.Web.UI.RadTreeView;
    private _btnNew: Telerik.Web.UI.RadButton;
    private _btnEdit: Telerik.Web.UI.RadButton;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _toolBarSearch: Telerik.Web.UI.RadToolBar;

    static TEMPLATE_REPORT_NAME: string = "TemplateReport";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let service: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, ReportDesignerList.TEMPLATE_REPORT_NAME);
        if (!service) {
            this.showNotification("Nessun servizio configurato per la gestione dei report.");
        }

        this._service = new TemplateReportService(service);
    }

    /**
     *------------------------- Events -----------------------------
     */
    rtvReport_OnNodeClicked = (source: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        if (!selectedNode || !selectedNode.get_value()) {
            $("#".concat(this.uscReportDesignerId)).hide();
            this._btnNew.set_enabled(true);
            this._btnEdit.set_enabled(false);
            return;
        }

        this.showLoading();
        $.when(this.loadDesigner(args.get_node().get_value()))
            .done(() => {
                this._btnNew.set_enabled(false);
                this._btnEdit.set_enabled(true);
            })
            .fail((err) => this.showNotification(err))
            .always(() => this.hideLoading());
    }

    btnNew_OnClick = (source: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        location.href = "ReportDesigner.aspx";
    }

    btnEdit_OnClick = (source: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvReports.get_selectedNode();
        if (!selectedNode || !selectedNode.get_value()) {
            return;
        }

        location.href = "ReportDesigner.aspx?ReportUniqueId=".concat(selectedNode.get_value());
    }

    toolBarSearch_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, eventArgs: Telerik.Web.UI.RadToolBarEventArgs) => {
        let txtSearchDescription: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>this._toolBarSearch.findItemByValue('searchDescription').findControl('txtReportName');
        this.showLoading();
        $.when(this.loadReports(txtSearchDescription.get_value()))
            .done(() => {
                this._btnNew.set_enabled(true);
                this._btnEdit.set_enabled(false);
                $("#".concat(this.uscReportDesignerId)).hide();
            })
            .fail((err) => this.showNotification(err))
            .always(() => this.hideLoading());
    }

    /**
     *------------------------- Methods -----------------------------
     */
    initialize(): void {
        this._rtvReports = $find(this.rtvReportsId) as Telerik.Web.UI.RadTreeView;
        this._rtvReports.add_nodeClicked(this.rtvReport_OnNodeClicked);
        this._btnNew = $find(this.btnNewId) as Telerik.Web.UI.RadButton;
        this._btnNew.add_clicked(this.btnNew_OnClick);
        this._btnEdit = $find(this.btnEditId) as Telerik.Web.UI.RadButton;
        this._btnEdit.add_clicked(this.btnEdit_OnClick);
        this._toolBarSearch = $find(this.toolBarSearchId) as Telerik.Web.UI.RadToolBar;
        this._toolBarSearch.add_buttonClicked(this.toolBarSearch_ButtonClicked);
        this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;

        $("#".concat(this.uscReportDesignerId)).hide();
        this._btnNew.set_enabled(true);
        this._btnEdit.set_enabled(false);

        this.showLoading();
        $.when(this.loadReports())
            .fail((err) => this.showNotification(err))
            .always(() => this.hideLoading());
    }

    private loadReports(name: string): JQueryPromise<void>
    private loadReports(): JQueryPromise<void>
    private loadReports(name?: any): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvReports.get_nodes().getNode(0);
        rootNode.get_nodes().clear();
        this._service.find(name,
            (data: any) => {
                try {
                    let templateReports: TemplateReportModel[] = data as TemplateReportModel[];                    
                    let node: Telerik.Web.UI.RadTreeNode;
                    for (let template of templateReports) {
                        node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(template.Name);
                        node.set_value(template.UniqueId);
                        rootNode.get_nodes().add(node);
                    }
                    rootNode.expand();
                    promise.resolve();
                } catch (e) {
                    console.error(e);
                    promise.reject("Errore nel recupero dei report disponibili.");
                }
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            });
        return promise.promise();
    }

    private loadDesigner(reportId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let uscReportDesigner: UscReportDesigner = <UscReportDesigner>$("#".concat(this.uscReportDesignerId)).data();
        this._service.getById(reportId,
            (data: any) => {
                try {                    
                    let templateReport: TemplateReportModel = data as TemplateReportModel;
                    let reportBuilderModel: ReportBuilderModel = JSON.parse(templateReport.ReportBuilderJsonModel);
                    uscReportDesigner.loadDesignerModel(reportBuilderModel);
                    $("#".concat(this.uscReportDesignerId)).show();

                    promise.resolve();
                } catch (e) {
                    console.error(e);
                    promise.reject("Errore nel caricamento dei dati del designer selezionato.");
                }
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            });
        return promise.promise();
    }

    private showLoading(): void {
        this._ajaxLoadingPanel.show(this.splPageId);
    }

    private hideLoading(): void {
        this._ajaxLoadingPanel.hide(this.splPageId);
    }

    private showNotification(error: string): void
    private showNotification(error: ExceptionDTO): void
    private showNotification(error?: any): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (error instanceof ExceptionDTO) {
                uscNotification.showNotification(error);
            } else {
                uscNotification.showNotificationMessage(error);
            }            
        }
    }
}

export = ReportDesignerList;