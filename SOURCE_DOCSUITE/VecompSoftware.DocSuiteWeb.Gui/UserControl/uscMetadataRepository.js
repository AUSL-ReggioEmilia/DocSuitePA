/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "App/Models/Commons/MetadataRepositoryStatus"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, MetadataRepositoryStatus) {
    var uscMetadatarepository = /** @class */ (function (_super) {
        __extends(uscMetadatarepository, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function uscMetadatarepository(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME)) || this;
            /**
             * --------------------------- Events -------------------------------
             */
            /**
             * Evento scatenato al click di un RadButton nella toolbar di ricerca
             * @param sender
             * @param eventArgs
             */
            _this.toolBar_ButtonClicked = function (sender, eventArgs) {
                var txtSearchDescription = _this._toolBarSearch.findItemByValue('searchDescription').findControl('txtRepositoryName');
                _this.loadRepositories();
            };
            /**
             * Evento scatenato al click di un nodo
             * @param sender
             * @param eventArgs
             */
            _this.treeView_ClientNodeClicked = function (sender, eventArgs) {
                var selectedNode = eventArgs.get_node();
                if (selectedNode == _this._rtvMetadataRepository.get_nodes().getNode(0)) {
                    $("#".concat(_this.pageId)).triggerHandler(uscMetadatarepository.ON_ROOT_NODE_CLICKED);
                }
                else {
                    $("#".concat(_this.pageId)).triggerHandler(uscMetadatarepository.ON_NODE_CLICKED, selectedNode.get_value());
                }
            };
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /**
         * --------------------------- Methods -------------------------------
         */
        /**
         * Funzione chiamata in inizializzazione
         */
        uscMetadatarepository.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._rtvMetadataRepository = $find(this.rtvMetadataRepositoryId);
            this._rtvMetadataRepository.add_nodeClicked(this.treeView_ClientNodeClicked);
            this._toolBarSearch = $find(this.toolBarSearchId);
            this._toolBarSearch.add_buttonClicked(this.toolBar_ButtonClicked);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this.loadRepositories();
            $("#".concat(this.pageId)).data(this);
        };
        /**
         * Carico i nodi della RadTreeView
         */
        uscMetadatarepository.prototype.loadRepositories = function () {
            var _this = this;
            //se ripetuto mettere nell'initialize
            var txtSearchDescription = this._toolBarSearch.findItemByValue('searchDescription').findControl('txtRepositoryName');
            this._loadingPanel.show(this.pageId);
            this._service.findMetadataRepositories(txtSearchDescription.get_value(), function (data) {
                _this.loadNodes(data);
            }, function (exception) {
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                    _this._loadingPanel.hide(_this.pageId);
                }
            });
        };
        /**
         * Popolo i nodi della treeview con i vari metadati
         * @param repositories
         */
        uscMetadatarepository.prototype.loadNodes = function (repositories) {
            if (repositories == null)
                return;
            var rootNode = this._rtvMetadataRepository.get_nodes().getNode(0);
            rootNode.get_nodes().clear();
            var newNode;
            $.each(repositories, function (index, repository) {
                newNode = new Telerik.Web.UI.RadTreeNode();
                newNode.set_text(repository.Name);
                newNode.set_value(repository.UniqueId);
                newNode.get_attributes().setAttribute("Status", repository.Status);
                switch (Number(MetadataRepositoryStatus[repository.Status])) {
                    case MetadataRepositoryStatus.Confirmed:
                        newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/EnumDesigner_16x_green.png");
                        break;
                    case MetadataRepositoryStatus.Draft:
                        newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/EnumDesigner_16x.png");
                        break;
                }
                rootNode.get_nodes().add(newNode);
            });
            rootNode.set_expanded(true);
            this._rtvMetadataRepository.commitChanges();
            this._loadingPanel.hide(this.pageId);
            $("#".concat(this.pageId)).triggerHandler(uscMetadatarepository.ON_TREEVIEW_LOADED);
        };
        uscMetadatarepository.prototype.getSelectedNode = function () {
            if (!this._rtvMetadataRepository.get_selectedNode()) {
                return null;
            }
            return this._rtvMetadataRepository.get_selectedNode();
        };
        uscMetadatarepository.prototype.findNodeByValue = function (id) {
            if (id) {
                return this._rtvMetadataRepository.findNodeByValue(id);
            }
        };
        uscMetadatarepository.ON_ROOT_NODE_CLICKED = "onRootNodeClick";
        uscMetadatarepository.ON_NODE_CLICKED = "onNodeClicked";
        uscMetadatarepository.ON_TREEVIEW_LOADED = "onTreeViewLoaded";
        return uscMetadatarepository;
    }(MetadataRepositoryBase));
    return uscMetadatarepository;
});
//# sourceMappingURL=uscMetadataRepository.js.map