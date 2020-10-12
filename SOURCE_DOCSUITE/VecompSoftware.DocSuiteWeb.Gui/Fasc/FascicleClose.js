var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Rules/Rights/Entities/Fascicles/FascicleRights", "App/Models/Fascicles/FascicleType", "App/Services/Workflows/WorkflowRepositoryService", "App/Models/Environment"], function (require, exports, FascicleBase, ServiceConfigurationHelper, FascicleRights, FascicleType, WorkflowRepositoryService, Environment) {
    var FascicleClose = /** @class */ (function (_super) {
        __extends(FascicleClose, _super);
        function FascicleClose(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            _this._startClosingSelectedFascicles = function (sender, args) {
                var checkedFascicleIds = _this._getFasciclesGridCheckedItemsIDs();
                if (!checkedFascicleIds.length) {
                    _this.showWarningMessage(_this.uscNotificationId, "Nessun fascicolo selezionato");
                    return;
                }
                if (_this._fasciclesTreeRootNode().get_loadingStatusElement()) {
                    _this._fasciclesTreeRootNode().hideLoadingStatus();
                }
                _this._fasciclesTreeRootNode().get_nodes().clear();
                _this._cancelFasciclesWindow.show();
                _this._fasciclesTreeRootNode().showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                _this._fasciclesTreeRootNode().set_expanded(true);
                var pendingRequests = [];
                checkedFascicleIds.forEach(function (fascicleId) {
                    pendingRequests.push(_this._closeFascicle(fascicleId));
                });
                $.when.apply($, pendingRequests).done(function () {
                    var responses = [];
                    for (var _i = 0; _i < arguments.length; _i++) {
                        responses[_i] = arguments[_i];
                    }
                    var successfullyClosed = responses.filter(function (status) { return status; }).length;
                    _this._fasciclesTreeRootNode().hideLoadingStatus();
                    _this._fasciclesTreeRootNode().set_text(FascicleClose.ROOTNODE_DESCRIPTION + " (" + successfullyClosed + "/" + checkedFascicleIds.length + ")");
                    if (successfullyClosed) {
                        _this._grid.get_masterTableView().rebind();
                    }
                })
                    .fail(function (exception) {
                    _this._fasciclesTreeRootNode().hideLoadingStatus();
                    _this._fasciclesTreeRootNode().set_imageUrl(FascicleClose.STEP_FAILED_IMG_URL);
                })
                    .always(function () {
                    _this._fasciclesTreeRootNode().set_imageUrl(FascicleClose.CLOSING_PROCESS_FINISHED_IMG_URL);
                });
            };
            _this._selectAllGridItems = function (sender, args) {
                args.set_cancel(true);
                var gridItems = _this._grid.get_masterTableView().get_dataItems();
                gridItems.forEach(function (gridItem) {
                    var rowCheckbox = gridItem.findElement(FascicleClose.GRIDITEM_CHECKBOX_ID);
                    if (!rowCheckbox.disabled) {
                        rowCheckbox.checked = true;
                    }
                });
            };
            _this._deselectAllGridItems = function (sender, args) {
                args.set_cancel(true);
                var gridItems = _this._grid.get_masterTableView().get_dataItems();
                gridItems.forEach(function (gridItem) {
                    var rowCheckbox = gridItem.findElement(FascicleClose.GRIDITEM_CHECKBOX_ID);
                    rowCheckbox.checked = false;
                });
            };
            _this._serviceConfigurations = serviceConfigurations;
            var workflowRepositoriyConfiguration = ServiceConfigurationHelper.getService(_this._serviceConfigurations, "WorkflowRepository");
            _this._workflowRepositoriyService = new WorkflowRepositoryService(workflowRepositoriyConfiguration);
            return _this;
        }
        FascicleClose.prototype._fasciclesTreeRootNode = function () {
            return this._fasciclesTreeView.get_nodes().getNode(0);
        };
        FascicleClose.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._btnSelectAll = $find(this.btnSelectAllId);
            this._btnCloseFascicles = $find(this.btnCloseFasciclesId);
            this._btnDeselectAll = $find(this.btnDeselectAllId);
            this._grid = $find(this.gridId);
            this._cancelFasciclesWindow = $find(this.cancelFasciclesWindowId);
            this._fasciclesTreeView = $find(this.fasciclesTreeId);
            this._registerPageElementsEventHandlers();
        };
        FascicleClose.prototype._registerPageElementsEventHandlers = function () {
            if (this._btnCloseFascicles) {
                this._btnCloseFascicles.add_clicking(this._startClosingSelectedFascicles);
            }
            if (this._btnSelectAll) {
                this._btnSelectAll.add_clicking(this._selectAllGridItems);
            }
            if (this._btnDeselectAll) {
                this._btnDeselectAll.add_clicking(this._deselectAllGridItems);
            }
        };
        FascicleClose.prototype.hasFascicolatedUD = function (idFascicle) {
            var promise = $.Deferred();
            this.service.hasFascicolatedDocumentUnits(idFascicle, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascicleClose.prototype.hasAuthorizedWorkflows = function () {
            var promise = $.Deferred();
            this._workflowRepositoriyService.hasAuthorizedWorkflowRepositories(Environment.Fascicle, false, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascicleClose.prototype._getFascicleRights = function (fascicle) {
            var promise = $.Deferred();
            var fascicleRights = {};
            var fascicleRule = new FascicleRights(fascicle, this._serviceConfigurations);
            $.when(fascicleRule.hasViewableRight(), fascicleRule.hasManageableRight(), fascicleRule.isManager(), fascicleRule.isProcedureSecretary(), this.hasFascicolatedUD(fascicle.UniqueId), this.hasAuthorizedWorkflows())
                .done(function (view, edit, manager, secretary, ud, wf) {
                fascicleRights.IsViewable = view;
                fascicleRights.IsEditable = edit;
                fascicleRights.IsManageable = edit;
                fascicleRights.IsManager = manager;
                fascicleRights.IsSecretary = secretary;
                fascicleRights.HasAuthorizedWorkflows = wf;
                fascicleRights.HasFascicolatedUD = ud;
                promise.resolve(fascicleRights);
            })
                .fail(function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascicleClose.prototype._checkFascicleClosableRights = function (fascicle) {
            var request = $.Deferred();
            this._getFascicleRights(fascicle)
                .then(function (fascicleRights) {
                var isProcedureFascicle = fascicle.FascicleType == FascicleType.Procedure;
                var isPeriodicFascicle = fascicle.FascicleType == FascicleType.Period;
                var isClosed = fascicle.EndDate != null;
                var isClosable = ((fascicleRights.IsManager || fascicleRights.IsSecretary) && !isClosed && !isPeriodicFascicle);
                if (isProcedureFascicle) {
                    isClosable = isClosable && fascicleRights.HasFascicolatedUD;
                }
                request.resolve(isClosable);
            })
                .fail(function (exception) { return request.reject(exception); });
            return request.promise();
        };
        FascicleClose.prototype._closeFascicle = function (fascicleId) {
            var _this = this;
            var request = $.Deferred();
            // Create tree node and set loading state
            this.service.getFascicle(fascicleId, function (fascicleModel) {
                fascicleModel.FascicleType = FascicleType[fascicleModel.FascicleType];
                // Create main fascicle tree node
                var currentClosingFascicleNode = _this._createTreeNode(fascicleModel.Title + " " + fascicleModel.FascicleObject);
                currentClosingFascicleNode.set_cssClass(FascicleClose.BOLD_CSSCLASS);
                _this._fasciclesTreeRootNode().get_nodes().add(currentClosingFascicleNode);
                currentClosingFascicleNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                // Create fascicle "Check rights" node
                var checkFascicleRightsNode = _this._createTreeNode(FascicleClose.CHECKINGRIGHTS_PROCESS_MSG);
                currentClosingFascicleNode.get_nodes().add(checkFascicleRightsNode);
                checkFascicleRightsNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                currentClosingFascicleNode.set_expanded(true);
                // Check if can be canceled (fascicle rights)
                _this._checkFascicleClosableRights(fascicleModel)
                    .then(function (fascicleIsCloseable) {
                    if (!fascicleIsCloseable) {
                        _this._setFinishedNodesStatus(FascicleClose.STEP_FAILED_IMG_URL, currentClosingFascicleNode, checkFascicleRightsNode);
                        request.resolve(false);
                        return;
                    }
                    _this._setFinishedNodesStatus(FascicleClose.STEP_SUCCEDED_IMG_URL, checkFascicleRightsNode);
                    // Start closing the fascicle
                    var closeFascicleStatusNode = _this._createTreeNode(FascicleClose.CLOSINGFASCICLE_PROCESS_MSG);
                    currentClosingFascicleNode.get_nodes().add(closeFascicleStatusNode);
                    closeFascicleStatusNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                    fascicleModel.EndDate = moment().toDate();
                    _this.service.closeFascicle(fascicleModel, function () {
                        _this._setFinishedNodesStatus(FascicleClose.STEP_SUCCEDED_IMG_URL, closeFascicleStatusNode, currentClosingFascicleNode);
                        request.resolve(true);
                    }, function (exception) {
                        _this._setFinishedNodesStatus(FascicleClose.STEP_FAILED_IMG_URL, closeFascicleStatusNode, currentClosingFascicleNode);
                        _this._appendErrorNodes(exception, closeFascicleStatusNode);
                        request.resolve(false);
                    });
                })
                    .fail(function (exception) {
                    _this._setFinishedNodesStatus(FascicleClose.STEP_FAILED_IMG_URL, checkFascicleRightsNode, currentClosingFascicleNode);
                    _this._appendErrorNodes(exception, checkFascicleRightsNode);
                    request.reject(false);
                });
            }, function (exception) { return request.reject(exception); });
            return request.promise();
        };
        FascicleClose.prototype._getFasciclesGridCheckedItems = function () {
            var selectedGridItems = [];
            var gridItems = this._grid.get_masterTableView().get_dataItems();
            gridItems.forEach(function (gridItem) {
                var itemCheckbox = gridItem.findElement(FascicleClose.GRIDITEM_CHECKBOX_ID);
                if (itemCheckbox.checked) {
                    selectedGridItems.push(gridItem);
                }
            });
            return selectedGridItems;
        };
        FascicleClose.prototype._getFasciclesGridCheckedItemsIDs = function () {
            var fascicleIds = new Array();
            var checkedGridItems = this._getFasciclesGridCheckedItems();
            $.each(checkedGridItems, function (index, item) {
                fascicleIds.push(item.getDataKeyValue(FascicleClose.GRIDITEM_FASCICLEID_KEY));
            });
            return fascicleIds;
        };
        FascicleClose.prototype._appendErrorNodes = function (exception, parentNode) {
            var _this = this;
            var validationException = exception;
            if (validationException && validationException.validationMessages) {
                validationException.validationMessages.forEach(function (validationMsg) {
                    if (!parentNode.get_allNodes().filter(function (n) { return n.get_text() == validationMsg.message; })[0]) {
                        parentNode.get_nodes().add(_this._createErrorNode(validationMsg.message));
                    }
                });
            }
            else {
                parentNode.get_nodes().add(this._createErrorNode(exception.statusText));
            }
        };
        FascicleClose.prototype._createErrorNode = function (errorMsg) {
            var errorNode = this._createTreeNode(errorMsg);
            errorNode.set_imageUrl(FascicleClose.ERROR_NODE_IMG_URL);
            return errorNode;
        };
        FascicleClose.prototype._createTreeNode = function (nodeText) {
            var treeNode = new Telerik.Web.UI.RadTreeNode();
            treeNode.set_text(nodeText);
            return treeNode;
        };
        FascicleClose.prototype._setFinishedNodesStatus = function (nodeImage) {
            var nodes = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                nodes[_i - 1] = arguments[_i];
            }
            nodes.forEach(function (node) {
                node.hideLoadingStatus();
                node.set_imageUrl(nodeImage);
            });
        };
        FascicleClose.GRIDITEM_CHECKBOX_ID = "cbSelect";
        FascicleClose.BOLD_CSSCLASS = "dsw-text-bold";
        FascicleClose.GRIDITEM_FASCICLEID_KEY = "Entity.UniqueId";
        FascicleClose.STEP_FAILED_IMG_URL = "../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png";
        FascicleClose.ERROR_NODE_IMG_URL = "../App_Themes/DocSuite2008/imgset16/flag_red.png";
        FascicleClose.STEP_SUCCEDED_IMG_URL = "../App_Themes/DocSuite2008/imgset16/accept.png";
        FascicleClose.CLOSING_PROCESS_FINISHED_IMG_URL = "../App_Themes/DocSuite2008/imgset16/information.png";
        FascicleClose.CLOSINGFASCICLE_PROCESS_MSG = "Chiudendo il fascicolo";
        FascicleClose.CHECKINGRIGHTS_PROCESS_MSG = "Verifica se il fascicolo è chiudibile";
        FascicleClose.ROOTNODE_DESCRIPTION = "Cancellazione dei fascicoli iniziata";
        return FascicleClose;
    }(FascicleBase));
    return FascicleClose;
});
//# sourceMappingURL=FascicleClose.js.map