/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import DocumentUnitModel = require("App/Models/DocumentUnits/DocumentUnitModel");
import UDSDocumentUnit = require("App/Models/UDS/UDSDocumentUnit");
import UDSRepositoryModel = require("App/Models/UDS/UDSRepositoryModel");
import UDSRelationType = require("App/Models/UDS/UDSRelationType");
import UDSViewBase = require("./UDSViewBase");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import UDSConnectionService = require("App/Services/UDS/UDSConnectionService");
import DocumentUnitService = require("App/Services/DocumentUnits/DocumentUnitService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import RoleService = require("App/Services/Commons/RoleService");
import RoleModel = require("App/Models/Commons/RoleModel");
import AjaxModel = require("App/Models/AjaxModel");
import UscStartWorkflow = require('UserControl/uscStartWorkflow');
import ExceptionStatusCode = require("App/DTOs/ExceptionStatusCode");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");

class UDSView extends UDSViewBase {

    btnLinkId: string;
    currentIdUDS: string;
    currentIdUDSRepository: string;
    radWindowManagerId: string;
    destinationIdUDS: string;
    destinationUDSRepositoryId: string;
    ajaxLoadingPanelId: string;
    pnlUDSViewId: string;
    btnWorkflowId: string;
    btnCompleteWorkflowId: string;
    isWorkflowEnabled: boolean;
    radNotificationInfoId: string;
    windowCompleteWorkflowId: string;
    workflowActivityId: string;

    private _windowNuovo: Telerik.Web.UI.RadWindow;
    private _radWindowManager: Telerik.Web.UI.RadWindowManager;
    private _btnLink: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel
    private _udsDocumentUnit: UDSDocumentUnit;
    private _repositoryModel: UDSRepositoryModel;
    private _documentUnitModel: DocumentUnitModel;
    private _btnWorkflow: Telerik.Web.UI.RadButton;
    private _btnCompleteWorkflow: Telerik.Web.UI.RadButton;
    private _windowCompleteWorkflow: Telerik.Web.UI.RadWindow;
    private _documentUnitService: DocumentUnitService;
    private _roleService: RoleService;
    private _notificationInfo: Telerik.Web.UI.RadNotification;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        super.initialize()
        this.cleanSessionStorage();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._notificationInfo = <Telerik.Web.UI.RadNotification>$find(this.radNotificationInfoId);
        this._radWindowManager = $find(this.radWindowManagerId) as Telerik.Web.UI.RadWindowManager;

        this._btnLink = <Telerik.Web.UI.RadButton>$find(this.btnLinkId);
        if (this._btnLink) {
            this._btnLink.add_clicked(this.btnLink_onClick);
        }
        this._btnWorkflow = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowId);
        this._btnWorkflow.set_visible(this.isWorkflowEnabled);
        if (this.isWorkflowEnabled) {
            this._btnWorkflow.add_clicking(this.btnWorkflow_onClick);
            this._radWindowManager.add_close((this.onWorkflowCloseWindow));
        }

        this._btnCompleteWorkflow = <Telerik.Web.UI.RadButton>$find(this.btnCompleteWorkflowId);
        this._btnCompleteWorkflow.set_visible(this.hasActiveWorkflowActivityWorkflow());
        if (this.hasActiveWorkflowActivityWorkflow()) {
            this._btnCompleteWorkflow.add_clicking(this.btnCompleteWorkflow_OnClick);
        }
        
        let documentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
        this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);

        this._documentUnitService.getDocumentUnitById(this.currentIdUDS, (data: DocumentUnitModel) => {
            this._documentUnitModel = data;
            this.setDocumentUnitRoles();
        });

        this._windowCompleteWorkflow = <Telerik.Web.UI.RadWindow>$find(this.windowCompleteWorkflowId);
        this._windowCompleteWorkflow.add_close((this.onWorkflowCloseWindow));
    }

    showWindow = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._windowNuovo.show();
    }

    btnLink_onClick = () => {
        let url = `../UDS/UDSLink.aspx?IdUDSRepository=${this.currentIdUDSRepository}&idUDS=${this.currentIdUDS}&fromUDSLink=True`;
        let window = this._radWindowManager.open(url, "UDSLink", null);
        window.setSize(1024, 600);
        (window as any).add_beforeClose(this.beforeCloseWindowSearch);
        window.set_modal(true);
        window.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
        window.set_visibleStatusbar(false);
        window.center();
    }

    btnWorkflow_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this.setSessionVariables();

        let url = `../Workflows/StartWorkflow.aspx?Type=UDS&ManagerID=${this.radWindowManagerId}&DSWEnvironment=${this._documentUnitModel.Environment}&Callback=${window.location.href}`;
        return this.openWindow(url, "windowStartWorkflow", 730, 550);
    }

    btnCompleteWorkflow_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        var url = `../Workflows/CompleteWorkflow.aspx?Type=Fasc&IdDocumentUnit=${this._documentUnitModel.UniqueId}&IdWorkflowActivity=${this.workflowActivityId}`;
        return this.openWindow(url, "windowCompleteWorkflow", 700, 500);
    }

    onWorkflowCloseWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            let result: AjaxModel = <AjaxModel>{};
            result = <AjaxModel>args.get_argument();
            this._notificationInfo.show();
            this._notificationInfo.set_text(result.ActionName);
            this._loadingPanel.show(this.pnlUDSViewId);
            location.reload();
        }
    }

    openWindow(url, name, width, height): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    setSessionVariables() {
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(this._documentUnitModel));
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, this._documentUnitModel.UniqueId);
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE, `${this._documentUnitModel.Title} - ${this._documentUnitModel.Subject}`);
    }

    setDocumentUnitRoles() {
        let roleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Role");
        this._roleService = new RoleService(roleConfiguration);

        if (this._documentUnitModel) {
            this._documentUnitModel.Roles = new Array<RoleModel>();
            this._documentUnitModel.DocumentUnitRoles.forEach(x => {
                this._roleService.getDocumentUnitRoles(x.UniqueIdRole, (data) => {
                    this._documentUnitModel.Roles.push(data);
                });
            })
        }
    }

    beforeCloseWindowSearch = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseCancelEventArgs) => {
        if (args.get_argument()) {
            args.set_cancel(true);
            this.closeWindowSearch(sender, args);
        }
    }

    closeWindowSearch = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        sender.remove_close(this.closeWindowSearch);

        //get arguments from callback
        if (args.get_argument() !== null) {
            this.destinationIdUDS = args.get_argument().split("|")[0];
            this.destinationUDSRepositoryId = args.get_argument().split("|")[1];
            this._loadingPanel.show(this.pnlUDSViewId);
            this.postStartUDS()
                .done(() => this.postRelatedUDS()
                    .done(() => {
                        alert("Gli archivi sono stati collegati con successo!");
                        location.reload();
                    })
                    .fail((exception: ExceptionDTO) => alert("Anomalia nel collegare gli archivi. Contattare l'assistenza."))
                )
                .fail((exception: ExceptionDTO) => alert("Anomalia nel collegare gli archivi. Contattare l'assistenza."));
        }
    }

    populateCurrentUDSModel() {
        let documentModel: DocumentUnitModel = new DocumentUnitModel();
        documentModel.UniqueId = this.destinationIdUDS;

        this._udsDocumentUnit = {
            Environment: this._repositoryModel.DSWEnvironment,
            RelationType: UDSRelationType.UDS,
            IdUDS: this.currentIdUDS,
            Relation: documentModel,
            Repository: this._repositoryModel
        };
    }

    populateDestinationUDSModel() {
        let documentModel: DocumentUnitModel = new DocumentUnitModel();
        documentModel.UniqueId = this.currentIdUDS;

        this._udsDocumentUnit = {
            Environment: this._repositoryModel.DSWEnvironment,
            RelationType: UDSRelationType.UDS,
            IdUDS: this.destinationIdUDS,
            Relation: documentModel,
            Repository: this._repositoryModel
        };
    }

    postStartUDS(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        //get the Start UDS repository, then make the POST at callback
        this.udsRepositoryService.getUDSRepositoryByID(this.currentIdUDSRepository,
            (data: UDSRepositoryModel[]) => {
                if (!data) {
                    let exc: ExceptionDTO = new ExceptionDTO();
                    exc.statusCode = ExceptionStatusCode.BadRequest;
                    exc.statusText = `Nessun repository trovato con id ${this.currentIdUDSRepository}`;
                    return promise.reject(exc);
                }
                this._repositoryModel = data[0];
                this.populateCurrentUDSModel();
                this.triggerUDSConnection()
                    .done(() => promise.resolve())
                    .fail((exception: ExceptionDTO) => promise.reject(exception));
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    postRelatedUDS() {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        //get the UDS repository related to the start UDS, then make the POST at callback
        this.udsRepositoryService.getUDSRepositoryByID(this.destinationUDSRepositoryId,
            (data: UDSRepositoryModel[]) => {
                if (!data) {
                    let exc: ExceptionDTO = new ExceptionDTO();
                    exc.statusCode = ExceptionStatusCode.BadRequest;
                    exc.statusText = `Nessun repository trovato con id ${this.destinationUDSRepositoryId}`;
                    return promise.reject(exc);
                }
                this._repositoryModel = data[0];
                this.populateDestinationUDSModel();
                this.triggerUDSConnection()
                    .done(() => promise.resolve())
                    .fail((exception: ExceptionDTO) => promise.reject(exception));
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    triggerUDSConnection(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        (this.udsConnectionService as UDSConnectionService).intitializeConnection(this._udsDocumentUnit,
            (data: any) => promise.resolve(),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private cleanSessionStorage() {
        sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
    }

    private hasActiveWorkflowActivityWorkflow = () => {
        return !String.isNullOrEmpty(this.workflowActivityId);
    }

}

export = UDSView;