import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleBase = require('Fasc/FascBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleRightsViewModel = require('App/ViewModels/Fascicles/FascicleRightsViewModel');
import FascicleRights = require('App/Rules/Rights/Entities/Fascicles/FascicleRights');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');
import Environment = require('App/Models/Environment');

class FascicleClose extends FascicleBase {

    public gridId: string;
    public btnCloseFasciclesId: string;
    public btnSelectAllId: string;
    public btnDeselectAllId: string;
    public cancelFasciclesWindowId: string;
    public fasciclesTreeId: string;
    public uscNotificationId: string;

    private _grid: Telerik.Web.UI.RadGrid;
    private _btnCloseFascicles: Telerik.Web.UI.RadButton;
    private _btnSelectAll: Telerik.Web.UI.RadButton;
    private _btnDeselectAll: Telerik.Web.UI.RadButton;
    private _cancelFasciclesWindow: Telerik.Web.UI.RadWindowManager;
    private _fasciclesTreeView: Telerik.Web.UI.RadTreeView;
    private _workflowRepositoriyService: WorkflowRepositoryService;
    private _serviceConfigurations: ServiceConfiguration[];

    private static GRIDITEM_CHECKBOX_ID: string = "cbSelect";
    private static BOLD_CSSCLASS: string = "dsw-text-bold";
    private static GRIDITEM_FASCICLEID_KEY: string = "Entity.UniqueId";
    private static STEP_FAILED_IMG_URL = "../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png";
    private static ERROR_NODE_IMG_URL = "../App_Themes/DocSuite2008/imgset16/flag_red.png";
    private static STEP_SUCCEDED_IMG_URL = "../App_Themes/DocSuite2008/imgset16/accept.png";
    private static CLOSING_PROCESS_FINISHED_IMG_URL = "../App_Themes/DocSuite2008/imgset16/information.png";
    private static CLOSINGFASCICLE_PROCESS_MSG = "Chiudendo il fascicolo";
    private static CHECKINGRIGHTS_PROCESS_MSG = "Verifica se il fascicolo è chiudibile";
    private static ROOTNODE_DESCRIPTION = "Cancellazione dei fascicoli iniziata";

    private _fasciclesTreeRootNode(): Telerik.Web.UI.RadTreeNode {
        return this._fasciclesTreeView.get_nodes().getNode(0);
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;

        let workflowRepositoriyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowRepository");
        this._workflowRepositoriyService = new WorkflowRepositoryService(workflowRepositoriyConfiguration);
    }

    public initialize(): void {
        super.initialize();

        this._btnSelectAll = <Telerik.Web.UI.RadButton>$find(this.btnSelectAllId);
        this._btnCloseFascicles = <Telerik.Web.UI.RadButton>$find(this.btnCloseFasciclesId);
        this._btnDeselectAll = <Telerik.Web.UI.RadButton>$find(this.btnDeselectAllId);
        this._grid = <Telerik.Web.UI.RadGrid>$find(this.gridId);
        this._cancelFasciclesWindow = <Telerik.Web.UI.RadWindowManager>$find(this.cancelFasciclesWindowId);
        this._fasciclesTreeView = <Telerik.Web.UI.RadTreeView>$find(this.fasciclesTreeId);

        this._registerPageElementsEventHandlers();
    }

    private _registerPageElementsEventHandlers(): void {
        if (this._btnCloseFascicles) {
            this._btnCloseFascicles.add_clicking(this._startClosingSelectedFascicles);
        }

        if (this._btnSelectAll) {
            this._btnSelectAll.add_clicking(this._selectAllGridItems);
        }

        if (this._btnDeselectAll) {
            this._btnDeselectAll.add_clicking(this._deselectAllGridItems);
        }
    }

    private hasFascicolatedUD(idFascicle): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this.service.hasFascicolatedDocumentUnits(idFascicle,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private hasAuthorizedWorkflows(): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this._workflowRepositoriyService.hasAuthorizedWorkflowRepositories(Environment.Fascicle, false,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private _getFascicleRights(fascicle: FascicleModel): JQueryPromise<FascicleRightsViewModel> {
        let promise: JQueryDeferred<FascicleRightsViewModel> = $.Deferred<FascicleRightsViewModel>();
        let fascicleRights: FascicleRightsViewModel = {};
        let fascicleRule: FascicleRights = new FascicleRights(fascicle, this._serviceConfigurations);
        $.when(fascicleRule.hasViewableRight(),
            fascicleRule.hasManageableRight(),
            fascicleRule.isManager(),
            fascicleRule.isProcedureSecretary(),
            this.hasFascicolatedUD(fascicle.UniqueId),
            this.hasAuthorizedWorkflows())
            .done((view, edit, manager, secretary, ud, wf) => {
                fascicleRights.IsViewable = view;
                fascicleRights.IsEditable = edit;
                fascicleRights.IsManageable = edit;
                fascicleRights.IsManager = manager;
                fascicleRights.IsSecretary = secretary;
                fascicleRights.HasAuthorizedWorkflows = wf;
                fascicleRights.HasFascicolatedUD = ud;
                promise.resolve(fascicleRights);
            })
            .fail((exception: ExceptionDTO) => promise.reject(exception));

        return promise.promise();
    }

    private _checkFascicleClosableRights(fascicle: FascicleModel): JQueryPromise<boolean> {
        let request: JQueryDeferred<boolean> = $.Deferred<boolean>();

        this._getFascicleRights(fascicle)
            .then((fascicleRights: FascicleRightsViewModel) => {
                let isProcedureFascicle: boolean = fascicle.FascicleType == FascicleType.Procedure;
                let isPeriodicFascicle: boolean = fascicle.FascicleType == FascicleType.Period;
                let isClosed: boolean = fascicle.EndDate != null;
                let isClosable: boolean = ((fascicleRights.IsManager || fascicleRights.IsSecretary) && !isClosed && !isPeriodicFascicle);
                if (isProcedureFascicle) {
                    isClosable = isClosable && fascicleRights.HasFascicolatedUD;
                }

                request.resolve(isClosable);
            })
            .fail((exception: ExceptionDTO) => request.reject(exception));

        return request.promise();
    }

    private _closeFascicle(fascicleId: string): JQueryPromise<boolean> {
        let request: JQueryDeferred<boolean> = $.Deferred<boolean>();

        // Create tree node and set loading state
        this.service.getFascicle(fascicleId, (fascicleModel: any) => {
            fascicleModel.FascicleType = FascicleType[fascicleModel.FascicleType];

            // Create main fascicle tree node
            let currentClosingFascicleNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(`${fascicleModel.Title} ${fascicleModel.FascicleObject}`);
            currentClosingFascicleNode.set_cssClass(FascicleClose.BOLD_CSSCLASS);
            this._fasciclesTreeRootNode().get_nodes().add(currentClosingFascicleNode);
            currentClosingFascicleNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);

            // Create fascicle "Check rights" node
            let checkFascicleRightsNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(FascicleClose.CHECKINGRIGHTS_PROCESS_MSG);
            currentClosingFascicleNode.get_nodes().add(checkFascicleRightsNode);
            checkFascicleRightsNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            currentClosingFascicleNode.set_expanded(true);

            // Check if can be canceled (fascicle rights)
            this._checkFascicleClosableRights(fascicleModel)
                .then((fascicleIsCloseable: boolean) => {
                    if (!fascicleIsCloseable) {
                        this._setFinishedNodesStatus(FascicleClose.STEP_FAILED_IMG_URL, currentClosingFascicleNode, checkFascicleRightsNode);

                        request.resolve(false);
                        return;
                    }

                    this._setFinishedNodesStatus(FascicleClose.STEP_SUCCEDED_IMG_URL, checkFascicleRightsNode);

                    // Start closing the fascicle
                    let closeFascicleStatusNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(FascicleClose.CLOSINGFASCICLE_PROCESS_MSG);
                    currentClosingFascicleNode.get_nodes().add(closeFascicleStatusNode);
                    closeFascicleStatusNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);

                    fascicleModel.EndDate = moment().toDate();
                    this.service.closeFascicle(fascicleModel, () => {
                        this._setFinishedNodesStatus(FascicleClose.STEP_SUCCEDED_IMG_URL, closeFascicleStatusNode, currentClosingFascicleNode);
                        request.resolve(true);
                    }, (exception: ExceptionDTO) => {
                        this._setFinishedNodesStatus(FascicleClose.STEP_FAILED_IMG_URL, closeFascicleStatusNode, currentClosingFascicleNode);
                        this._appendErrorNodes(exception, closeFascicleStatusNode);
                        request.resolve(false);
                    });
                })
                .fail((exception: ExceptionDTO) => {
                    this._setFinishedNodesStatus(FascicleClose.STEP_FAILED_IMG_URL, checkFascicleRightsNode, currentClosingFascicleNode);
                    this._appendErrorNodes(exception, checkFascicleRightsNode);
                    request.reject(false);
                })
        }, (exception: ExceptionDTO) => request.reject(exception));

        return request.promise();
    }

    private _startClosingSelectedFascicles = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs): void => {
        let checkedFascicleIds: string[] = this._getFasciclesGridCheckedItemsIDs();

        if (!checkedFascicleIds.length) {
            this.showWarningMessage(this.uscNotificationId, "Nessun fascicolo selezionato");
            return;
        }

        if (this._fasciclesTreeRootNode().get_loadingStatusElement()) {
            this._fasciclesTreeRootNode().hideLoadingStatus();
        }
        this._fasciclesTreeRootNode().get_nodes().clear();

        this._cancelFasciclesWindow.show();
        this._fasciclesTreeRootNode().showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        this._fasciclesTreeRootNode().set_expanded(true);

        let pendingRequests: JQueryPromise<boolean>[] = [] as JQueryPromise<boolean>[];
        checkedFascicleIds.forEach((fascicleId: string) => {
            pendingRequests.push(this._closeFascicle(fascicleId));
        });

        $.when(...pendingRequests)
            .done((...responses) => {
                let successfullyClosed: number = responses.filter(status => status).length;

                this._fasciclesTreeRootNode().hideLoadingStatus();
                this._fasciclesTreeRootNode().set_text(`${FascicleClose.ROOTNODE_DESCRIPTION} (${successfullyClosed}/${checkedFascicleIds.length})`);

                if (successfullyClosed) {
                    this._grid.get_masterTableView().rebind();
                }
            })
            .fail((exception: ExceptionDTO) => {
                this._fasciclesTreeRootNode().hideLoadingStatus();
                this._fasciclesTreeRootNode().set_imageUrl(FascicleClose.STEP_FAILED_IMG_URL);
            })
            .always(() => {
                this._fasciclesTreeRootNode().set_imageUrl(FascicleClose.CLOSING_PROCESS_FINISHED_IMG_URL);
            });
    }

    private _getFasciclesGridCheckedItems(): Telerik.Web.UI.GridDataItem[] {
        let selectedGridItems: Telerik.Web.UI.GridDataItem[] = [] as Telerik.Web.UI.GridDataItem[];
        let gridItems = this._grid.get_masterTableView().get_dataItems();

        gridItems.forEach(gridItem => {
            let itemCheckbox: HTMLInputElement = <HTMLInputElement>gridItem.findElement(FascicleClose.GRIDITEM_CHECKBOX_ID);

            if (itemCheckbox.checked) {
                selectedGridItems.push(gridItem);
            }
        });

        return selectedGridItems;
    }

    private _getFasciclesGridCheckedItemsIDs(): string[] {
        let fascicleIds: string[] = new Array();
        let checkedGridItems: Telerik.Web.UI.GridDataItem[] = this._getFasciclesGridCheckedItems();

        $.each(checkedGridItems, (index, item) => {
            fascicleIds.push(item.getDataKeyValue(FascicleClose.GRIDITEM_FASCICLEID_KEY));
        });

        return fascicleIds;
    }

    private _selectAllGridItems = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs): void => {
        args.set_cancel(true);

        let gridItems: any = this._grid.get_masterTableView().get_dataItems();

        gridItems.forEach(gridItem => {
            let rowCheckbox: HTMLInputElement = <HTMLInputElement>gridItem.findElement(FascicleClose.GRIDITEM_CHECKBOX_ID);
            if (!rowCheckbox.disabled) {
                rowCheckbox.checked = true;
            }
        });
    }

    private _deselectAllGridItems = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs): void => {
        args.set_cancel(true);

        let gridItems: any = this._grid.get_masterTableView().get_dataItems();

        gridItems.forEach(gridItem => {
            let rowCheckbox: HTMLInputElement = <HTMLInputElement>gridItem.findElement(FascicleClose.GRIDITEM_CHECKBOX_ID);
            rowCheckbox.checked = false;
        });
    }

    private _appendErrorNodes(exception: ExceptionDTO, parentNode: Telerik.Web.UI.RadTreeNode) {
        let validationException: ValidationExceptionDTO = exception as ValidationExceptionDTO;
        if (validationException && validationException.validationMessages) {
            validationException.validationMessages.forEach((validationMsg: ValidationMessageDTO) => {
                if (!parentNode.get_allNodes().filter(n => n.get_text() == validationMsg.message)[0]) {
                    parentNode.get_nodes().add(this._createErrorNode(validationMsg.message));
                }
            });
        } else {
            parentNode.get_nodes().add(this._createErrorNode(exception.statusText));
        }
    }

    private _createErrorNode(errorMsg: string): Telerik.Web.UI.RadTreeNode {
        let errorNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(errorMsg);
        errorNode.set_imageUrl(FascicleClose.ERROR_NODE_IMG_URL);

        return errorNode;
    }

    private _createTreeNode(nodeText: string): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        treeNode.set_text(nodeText);

        return treeNode;
    }

    private _setFinishedNodesStatus(nodeImage: string, ...nodes: Telerik.Web.UI.RadTreeNode[]): void {
        nodes.forEach((node: Telerik.Web.UI.RadTreeNode) => {
            node.hideLoadingStatus();
            node.set_imageUrl(nodeImage);
        });
    }
}

export = FascicleClose;