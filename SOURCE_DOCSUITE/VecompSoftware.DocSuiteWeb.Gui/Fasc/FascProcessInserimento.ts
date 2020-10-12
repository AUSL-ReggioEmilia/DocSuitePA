import FascBase = require("./FascBase");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import FascicleService = require("App/Services/Fascicles/FascicleService");
import AjaxModel = require("App/Models/AjaxModel");
import MetadataRepositoryModel = require("App/Models/Commons/MetadataRepositoryModel");
import Guid = require("App/Helpers/GuidHelper");
import WorkflowActionFascicleModel = require("App/Models/Workflows/WorkflowActionFascicleModel");
import InsertActionType = require("App/Models/InsertActionType");
import ContentBase = require("App/Models/ContentBase");
import PageClassHelper = require("App/Helpers/PageClassHelper");
import uscFascicleProcessInsert = require("UserControl/uscFascicleProcessInsert");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import FascicleType = require("App/Models/Fascicles/FascicleType");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");

declare var Page_IsValid: any;
class FascProcessInserimento extends FascBase {
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnInsert: Telerik.Web.UI.RadButton;

    pnlContentId: string;
    ajaxLoadingPanelId: string;
    btnInsertId: string;
    uscNotificationId: string;
    flatList: DossierFolderModel[];
    idCategory?: number;
    idDocumentUnit?: string;
    uscFascicleInsertId: string;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME));
    }

    initialize(): void {
        super.initialize();

        this._btnInsert = $find(this.btnInsertId) as Telerik.Web.UI.RadButton;
        this._btnInsert.add_clicking(this._btnInsert_OnClick);
        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;

        if (this.idCategory) {
            this.loadCategory(this.idCategory);
        }
    }

    _btnInsert_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        if (!Page_IsValid) {
            args.set_cancel(true);
            return;
        }

        PageClassHelper.callUserControlFunctionSafe<uscFascicleProcessInsert>(this.uscFascicleInsertId)
            .done((instance) => {
                try {
                    instance.getFascicle().done((currentFascicleToInsert) => {
                        instance.fillMetadataModel().done((metadatas: [string, string]) => {
                            if (!metadatas) {
                                this._btnInsert.set_enabled(true);
                                return;
                            }
                            this.finalizeInsert(currentFascicleToInsert, metadatas[0], metadatas[1]);
                        });                        
                    });
                } catch (error) {
                    console.error(error.message);
                    let exception: ExceptionDTO = new ExceptionDTO();
                    exception.statusText = "E' avvenuto un errore durante la creazione del fascicolo";
                    this.showNotificationException(this.uscNotificationId, exception);
                }                
            });
    }

    finalizeInsert(fascicle: FascicleModel, metadataDesigner: string, metadataValues: string): void {
        fascicle.MetadataValues = metadataValues;
        fascicle.MetadataDesigner = metadataDesigner;
        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY)) {
            let metadataRepository: MetadataRepositoryModel = new MetadataRepositoryModel();
            metadataRepository.UniqueId = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY);
            fascicle.MetadataRepository = metadataRepository;
        }

        if (this.idDocumentUnit) {
            fascicle.UniqueId = Guid.newGuid();
            let workflowAction: WorkflowActionFascicleModel = new WorkflowActionFascicleModel();
            workflowAction.Fascicle = {} as ContentBase;
            workflowAction.Fascicle.$type = "VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles.FascicleModel, VecompSoftware.DocSuiteWeb.Model"
            workflowAction.Fascicle.UniqueId = fascicle.UniqueId;
            workflowAction.Referenced = {} as ContentBase;
            workflowAction.Referenced.$type = "VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits.DocumentUnitModel, VecompSoftware.DocSuiteWeb.Model";
            workflowAction.Referenced.UniqueId = this.idDocumentUnit;
            fascicle.WorkflowActions.push(workflowAction);
        }

        let insertActionType: InsertActionType = null;
        if (fascicle.FascicleType == FascicleType.Procedure) {
            insertActionType = InsertActionType.InsertProcedureFascicle;
        }

        this._loadingPanel.show(this.pnlContentId);
        this.service.insertFascicle(fascicle, insertActionType,
            (data: FascicleModel) => {
                window.location.href = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${data.UniqueId}`;
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlContentId);
                this._btnInsert.set_enabled(true);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private loadCategory(idCategory: number): void {
        PageClassHelper.callUserControlFunctionSafe<uscFascicleProcessInsert>(this.uscFascicleInsertId)
            .done((instance) => {
                instance.loadDefaultCategory(idCategory);
            });
    }
}

export = FascProcessInserimento;