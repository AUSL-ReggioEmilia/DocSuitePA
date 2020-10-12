import PECBase = require('PEC/PECBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import WorkflowActivityLogService = require('App/Services/Workflows/WorkflowActivityLogService');
import WorkflowActivityLogModel = require('App/Models/Workflows/WorkflowActivityLogModel');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import FascicleDocumentService = require('App/Services/Fascicles/FascicleDocumentService');
import uscFascicleSearch = require('UserControl/uscFascicleSearch');
import AjaxModel = require('App/Models/AjaxModel');
import FascicleFolderService = require('App/Services/Fascicles/FascicleFolderService');
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');

class PECToDocumentUnit extends PECBase {
    uscNotificationId: string;
    maxNumberElements: string;
    managerWindowsId: string;
    searchFasciclesId: string;
    ajaxLoadingPanelId: string;
    ajaxManagerId: string;
    pageContentId: string;
    pnlTemplateProtocolId: string;
    pnlUDSSelectId: string;
    pnlFascicleSelectId: string;
    rblDocumentUnitId: string;
    cmdInitId: string;
    cmdInitAndCloneId: string;
    isPecClone: boolean;
    templateProtocolEnabled: boolean;
    uscFascicleSearchId: string;
    documentListGridId: string;
    pnlButtonsId: string;

    private _rblDocumentUnit: JQuery;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _serviceConfigurations: ServiceConfiguration[];
    private _fascicleDocumentService: FascicleDocumentService;
    private _fascicleFolderService: FascicleFolderService;
    private _service: WorkflowActivityLogService;
    private _documentListGrid: Telerik.Web.UI.RadGrid;

    private static INSERT_MISCELLANEA: string = "InsertMiscellanea";

    private cmdInit(): JQuery {
        return $(`#${this.cmdInitId}`);
    }

    private cmdInitAndClone(): JQuery {
        return $(`#${this.cmdInitAndCloneId}`);
    }

    private pnlTemplateProtocol(): JQuery {
        return $(`#${this.pnlTemplateProtocolId}`);
    }

    private pnlUDS(): JQuery {
        return $(`#${this.pnlUDSSelectId}`);
    }

    private pnlFascicle(): JQuery {
        return $(`#${this.pnlFascicleSelectId}`);
    }

    private pnlButtons(): JQuery {
        return $(`#${this.pnlButtonsId}`);
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, PECBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize(): void {
        super.initialize();

        let serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivityLog");
        this._service = new WorkflowActivityLogService(serviceConfiguration);

        let fascicleDocumentServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocument");
        this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentServiceConfiguration);

        let fascicleFolderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleFolder");
        this._fascicleFolderService = new FascicleFolderService(fascicleFolderServiceConfiguration);

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._documentListGrid = $find(this.documentListGridId) as Telerik.Web.UI.RadGrid;

        this.cmdInitAndClone().hide();
        if (this.isPecClone) {
            this.cmdInitAndClone().show();
        }

        this.pnlTemplateProtocol().hide();
        if (this.templateProtocolEnabled) {
            this.pnlTemplateProtocol().show();
        }

        this._rblDocumentUnit = $("#".concat(this.rblDocumentUnitId));
    }

    insertWorkflowActivity(fascicleModel: FascicleModel, workflowActivityModel: WorkflowActivityModel, url: string): void {
        let workflowActivityLogModel: WorkflowActivityLogModel = {
            UniqueId: "",
            LogDate: new Date(),
            SystemComputer: "",
            LogType: "",
            LogDescription: "",
            Severity: null,
            RegistrationDate: new Date(),
            RegistrationUser: "",
            LastChangedDate: new Date(),
            LastChangedUser: "",
            Entity: workflowActivityModel

        };
        workflowActivityLogModel.LogDescription = `Fascicolo:  ${fascicleModel.Year}/${fascicleModel.Number}=> ${fascicleModel.FascicleObject}`;
        (<WorkflowActivityLogService>this._service).insertWorkflowActivityLog(workflowActivityLogModel,
            (data: any) => {
                if (data == null) return;
                alert("I documenti sono stati allegati con successo");
                window.location.href = url;
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    radioButton_OnClick = () => {
        var selected = $(':radio:checked').val();
        this._documentListGrid.get_masterTableView().showColumn(1);
        switch (selected) {
            case "1": {
                this.cmdInitAndClone().hide();
                if (this.isPecClone) {
                    this.cmdInitAndClone().show();
                }

                this.pnlTemplateProtocol().hide();
                if (this.templateProtocolEnabled) {
                    this.pnlTemplateProtocol().show();
                }

                this.pnlUDS().hide();
                this.pnlFascicle().hide();
                break;
            }                

            case "7": {
                this.cmdInitAndClone().hide();
                if (this.isPecClone) {
                    this.cmdInitAndClone().show();
                }

                this.pnlTemplateProtocol().hide();
                this.pnlUDS().show();
                this.pnlFascicle().hide();
                break;
            }                

            case "8": {
                this._documentListGrid.get_masterTableView().hideColumn(1);
                this.cmdInitAndClone().hide();
                this.pnlTemplateProtocol().hide();
                this.pnlUDS().hide();
                this.pnlFascicle().show();
                break;
            }                
        }
    }

    hasSelectedFascicle() {
        let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
        if (!jQuery.isEmptyObject(uscFascicleSearch)) {
            let selectedFascicle: FascicleModel = uscFascicleSearch.getSelectedFascicle();
            return selectedFascicle != null;
        }
    }

    cmdFascMiscellaneaInsert_Click = (sender: any, args: any) => {
        let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
        if (!jQuery.isEmptyObject(uscFascicleSearch)) {
            let selectedFascicle: FascicleModel = uscFascicleSearch.getSelectedFascicle();
            if (selectedFascicle) {
                let selectedFascicleFolder: FascicleSummaryFolderViewModel = uscFascicleSearch.getSelectedFascicleFolder();
                let ajaxModel: AjaxModel = <AjaxModel>{};
                ajaxModel.Value = new Array<string>();
                ajaxModel.Value.push(selectedFascicle.UniqueId);
                if (selectedFascicleFolder) {
                    ajaxModel.Value.push(selectedFascicleFolder.UniqueId);
                }                
                ajaxModel.ActionName = PECToDocumentUnit.INSERT_MISCELLANEA;
                (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
            } else {
                alert("Nessun fascicolo selezionato");
            }
        }
    }

    confirmCallback(idChain: string, idFascicle: string, isNewArchiveChain: boolean, errorMessage: string, idFascicleFolder: string) {
        if (errorMessage) {
            alert(errorMessage);
            this._loadingPanel.hide(this.documentListGridId);
            this.pnlButtons().show();
            return;
        }

        if (isNewArchiveChain) {
            let fascicleDocumentModel: FascicleDocumentModel = <FascicleDocumentModel>{};
            fascicleDocumentModel.ChainType = ChainType.Miscellanea;
            fascicleDocumentModel.IdArchiveChain = idChain;
            fascicleDocumentModel.Fascicle = new FascicleModel();
            fascicleDocumentModel.Fascicle.UniqueId = idFascicle;
            this._fascicleFolderService.getById(idFascicleFolder,
                (data: any) => {
                    if (!data) {
                        this._loadingPanel.hide(this.pageContentId);
                        this.showNotificationException(this.uscNotificationId, null, "E' avvenuto un errore durante il processo di fascicolazione");
                        return;
                    }

                    fascicleDocumentModel.FascicleFolder = data as FascicleFolderModel;
                    this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel,
                        (data: any) => window.location.href = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${idFascicle}`,
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.pageContentId);
                            this.showNotificationException(this.uscNotificationId, exception);
                        }
                    );
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.pageContentId);
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );            
        } else {
            window.location.href = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${idFascicle}`;
        }       
    }        
}

export = PECToDocumentUnit;