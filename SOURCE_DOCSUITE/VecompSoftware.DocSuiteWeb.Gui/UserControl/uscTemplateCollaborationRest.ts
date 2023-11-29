/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');
import TemplateCollaborationRepresentationType = require('App/Models/Templates/TemplateCollaborationRepresentationType');
import TreeFlatNode = require('App/Core/TreeStructures/TreeFlatNode');
import TreeFlat = require('App/Core/TreeStructures/TreeFlat');

class UscTemplateCollaborationRest {

    uscNotificationId: string;
    rtvTemplateCollaborationId: string;
    pnlMainContentId: string;

    private _serviceTemplateCollaboration: TemplateCollaborationService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _vTemplateTree: TreeFlat<TemplateCollaborationModel>;
    private _treeViewTemplateCollaboration: Telerik.Web.UI.RadTreeView;

    private static TREE_NODE_ATTRIBUTE_ID = 'tree_node_attribute_id';
    private static TREE_NODE_TYPE_NAME = "tree_node_type";
    private static LOAD_MODE_TREE_NODE_TYPE = "load_more";
    private static LOADMORE_NODE_LABEL: string = "Carica più elementi";
    private static LOAD_MORE_NODE_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/add.png";

    private filterStatus: TemplateCollaborationStatus | null = null;

    /**
     * Event Names
     */
    public static TREE_NODE_CLICKED = "onTreeNodeClicked";
    public static TREE_ROOT_NODE_CLICKED = "onTreeRootNodeClicked";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._vTemplateTree = new TreeFlat<TemplateCollaborationModel>(r => r.UniqueId);
    }

    public SetFilterStatus(status: TemplateCollaborationStatus | null) {
        this.filterStatus = status;
        this.ReloadRoot();
    }

    /**
    * Mimics the selection of the root node in the tree. This method will also
    * trigger the event for external subscribers : EventName : UscTemplateCollaborationRest.TREE_ROOT_NODE_CLICKED
    */
    public SelectRootNode(): void {
        let deferred: JQueryDeferred<void> = $.Deferred();
        this.GetTreeRootNode()
            .then((rootNode: Telerik.Web.UI.RadTreeNode) => {
                rootNode.set_selected(true);
                this.TemplateTreeOnNodeClicked(rootNode);
            })
            .fail((err) => {
                deferred.fail(err);
            });
    }

    /**
     * Mimics the selection of the node in the tree. This method will also 
     * trigger the event for external subscribers : EventName : UscTemplateCollaborationRest.TREE_NODE_CLICKED
     */
    public SelectNode(id: string): void {
        let node = this._vTemplateTree.FindNode(id);
        if (!node) return;

        let radTreeNode = this.GetRadNodeByTemplateModel(node.Model);
        if (!radTreeNode) return;

        radTreeNode.set_selected(true);

        this.TemplateTreeOnNodeClicked(radTreeNode);
    }

    /**
     * Refetches content for root and expands the root node. 
     * Acts like a refresh button of the root node
     **/
    public ReloadRoot(): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();

        this.GetTreeRootNode()
            .then((radTreeNode: Telerik.Web.UI.RadTreeNode) => {
                radTreeNode.set_expanded(true);
                this.TemplateTreeOnNodeExpanding(radTreeNode)
                    .then(() => {
                        deferred.resolve();
                    })
                    .fail((err) => {
                        deferred.fail(err);
                    });
            })
            .fail((err) => {
                deferred.fail(err);
            });

        return deferred;
    }

    /**
    * Refetches content current node and expands it.
    * Acts like a refresh button of the current node.
    **/
    public ReloadContent(id: string): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();
        // find the child
        let node = this._vTemplateTree.FindNode(id);
        if (!node) return deferred.resolve();

        let radTreeNode = this.GetRadNodeByTemplateModel(node.Model);
        if (!radTreeNode) return deferred.resolve();

        // if this is a folder and a new item was added - we need to expand it
        radTreeNode.set_expanded(true);

        this.TemplateTreeOnNodeExpanding(radTreeNode)
            .then(() => {
                deferred.resolve();
            })
            .fail((err) => {
                deferred.fail(err);
            });

        return deferred;
    }

    /**
    * Refetches content for parent node of the current node.
    * Acts like a refresh button of the parent node.
    **/
    public ReloadContentForParentOf(id: string): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();
        // find the child
        let node = this._vTemplateTree.FindNode(id);
        if (!node) return deferred.resolve();

        let parentNode = this._vTemplateTree.FindParent(node);
        if (!parentNode) return deferred.resolve();

        let radTreeNode = this.GetRadNodeByTemplateModel(parentNode.Model);
        if (!radTreeNode) return deferred.resolve();

        // if this is a folder and a new item was added - we need to expand it
        radTreeNode.set_expanded(true);

        this.TemplateTreeOnNodeExpanding(radTreeNode)
            .then(() => {
                deferred.resolve();
            })
            .fail((err) => {
                deferred.fail(err);
            });

        return deferred;
    }

    /**
     * INitialize will not load tree content.You have to also call
     * this.ReloadRoot()
     * 
     * Reason: TempalteCollaboration can start with a query string wich enforses a nonactive filter.
     * When we do that, if initialize() loads the tree and we reload again with the filter, they will be concurrently compeeting
     * and the correct values will not be loaded
     **/
    public initialize(): void {
        try {
            this.initializeServices();
            this.bootstrapControls();
            this.initializeEvents();
            //this.ReloadRoot();
            this.bindMe();
        } catch (err) {
            this.showNotificationException(err);
        }
    }

    private bindMe(): void {
        $(`#${this.pnlMainContentId}`).data(this);
    }

    /*
     * Initializers
     */
    private initializeServices(): void {

        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "TemplateCollaboration");
        if (!serviceConfiguration) {
            this.showNotificationMessage("Errore in inizializzazione. Nessun servizio configurato per il Template di collaborazione");
            return;
        }
        this._serviceTemplateCollaboration = new TemplateCollaborationService(serviceConfiguration);
    }

    private bootstrapControls(): void {
        this._treeViewTemplateCollaboration = <Telerik.Web.UI.RadTreeView>$find(this.rtvTemplateCollaborationId);
        this.ensureNotNullOrUndefined(this._treeViewTemplateCollaboration, '_treeViewTemplateCollaboration');
    }

    private initializeEvents(): void {
        let _this = this;

        this._treeViewTemplateCollaboration.add_nodeExpanding(_this.TemplateTreeOnNodeExpandingHandler);
        this._treeViewTemplateCollaboration.add_nodeClicked(_this.TemplateTreeOnNodeClickedHandler);
    }

    /**
     * TREE EXTERNAL EVENTS
     */

    /**
     * Simplifying how the called component attaches to this component event by providing a 
     * builtin model
     */
    public static OnTreeNodeClicked = (restComponentId: string, handler: { (model: TemplateCollaborationModel): void }) => {
        $(`#${restComponentId}`).on(UscTemplateCollaborationRest.TREE_NODE_CLICKED, (jvEvent, args) => {
            handler(args);
        });
    }

    public static OnTreeRootNodeClicked = (restComponentId: string, handler: { (): void }) => {
        $(`#${restComponentId}`).on(UscTemplateCollaborationRest.TREE_ROOT_NODE_CLICKED, (jvEvent) => {
            handler();
        });
    }

    /**
     * TREE EVENTS
     */

    private TemplateTreeOnNodeClickedHandler = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let _this;
        try {
            /*
            * Reason for getting "this" from component:
            * This component is referenced in TemplateCollaboration using the data() from jquery.
            * The TemplateCollaboration has a dropdown that sets filters. When the dropdown change, that method
            *   calls this component ( TbltTemplateCollaboration.UscTemplatCollaborationRest.SetFilter() )
            * Then SetFilter is called, the filterstatus property is set into the jquery.data() object
            *   because it's called from TemplateCollaboration
            *
            * If a user will then try to expand a folder/fixed template in the tree (this component)
            * the object he is working on is not the jQuery.data() and the filterStatus will have been lost
            *
            * For this reason I am first getting the data() object
            *
            * NOTE: this can be otherwise solved by removing reference of this component in the parent
            * and using only other forms of decoupled messaging (postMessage, or something else built)
            */
            _this = <UscTemplateCollaborationRest>$(`#${this.pnlMainContentId}`).data();
            if (_this === null || _this === undefined) {
                _this = this;
            }
        } catch
        {
            _this = this;
        }

        let clikedNode = args.get_node();
        _this.TemplateTreeOnNodeClicked(clikedNode);
    }

    private TemplateTreeOnNodeExpandingHandler = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let _this;
        try {
            /*
             * Reason for getting "this" from component:
             * This component is referenced in TemplateCollaboration using the data() from jquery.
             * The TemplateCollaboration has a dropdown that sets filters. When the dropdown change, that method
             *   calls this component ( TbltTemplateCollaboration.UscTemplatCollaborationRest.SetFilter() )
             * Then SetFilter is called, the filterstatus property is set into the jquery.data() object
             *   because it's called from TemplateCollaboration
             *   
             * If a user will then try to expand a folder/fixed template in the tree (this component)
             * the object he is working on is not the jQuery.data() and the filterStatus will have been lost
             * 
             * For this reason I am first getting the data() object
             * 
             * NOTE: this can be otherwise solved by removing reference of this component in the parent 
             * and using only other forms of decoupled messaging (postMessage, or something else built)
             */

            _this = <UscTemplateCollaborationRest>$(`#${this.pnlMainContentId}`).data();
            if (_this === null || _this === undefined) {
                _this = this;
            }
        } catch {
            _this = this;
        }
        let expandingNode = args.get_node();
        // the expandingNode can have a list of loaded children or a fake empty node
        // which was added to emulate the expandable functionality. In both cases we want
        // to clear children and reload content
        _this.TemplateTreeOnNodeExpanding(expandingNode);
    }

    private TemplateTreeOnNodeClicked(clikedNode: Telerik.Web.UI.RadTreeNode) {
        if (this.IsRootNode(clikedNode)) {
            this.TriggerExternalEventRootNodeClicked();
        } else if (this.IsLoadMoreNode(clikedNode)) {
            this.TemplateTreeOnNodeExpanding(clikedNode);
        } else {
            let templateNodeId: string = clikedNode.get_attributes().getAttribute(UscTemplateCollaborationRest.TREE_NODE_ATTRIBUTE_ID);
            let templateNode: TreeFlatNode<TemplateCollaborationModel> = this._vTemplateTree.FindNode(templateNodeId);
            this.TriggerExternalEventNodeClicked(templateNode);
        }
    }

    private TemplateTreeOnNodeExpanding(expandingNode: Telerik.Web.UI.RadTreeNode): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();

        try {
            expandingNode.get_nodes().clear();
            expandingNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);

            if (this.IsRootNode(expandingNode)) {
                this.InjectChildrenForRootNode(expandingNode)
                    .then(() => {
                        deferred.resolve();
                    })
                    .fail((err) => {
                        deferred.reject(err)
                    })
                    .always(() => {
                        expandingNode.hideLoadingStatus();
                    });
            } else if (this.IsLoadMoreNode(expandingNode)) {
                let parentNode: Telerik.Web.UI.RadTreeNode = expandingNode.get_parent();
                this.InjectChildrenForTreeNode(parentNode, (parentNode.get_nodes().get_count() - 1))
                    .then(() => {
                        deferred.resolve();
                    })
                    .fail((err) => {
                        deferred.reject(err)
                    });
            } else {
                this.InjectChildrenForTreeNode(expandingNode)
                    .then(() => {
                        deferred.resolve();
                    })
                    .fail((err) => {
                        deferred.reject(err)
                    })
                    .always(() => {
                        expandingNode.hideLoadingStatus();
                    });
            }

        } catch (err) {
            expandingNode.hideLoadingStatus();
            this.showNotificationException(err);
            deferred.reject(err)
        }

        return deferred;
    }

    private InjectChildrenForRootNode(rootNode: Telerik.Web.UI.RadTreeNode): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();

        this._serviceTemplateCollaboration.getFixedTemplates(rootNode.get_value(),
            fixedTemplates => {
                this.ensureNotNullOrUndefined(fixedTemplates, 'fixedTemplates');
                this._vTemplateTree.Clear();
                for (let template of fixedTemplates) {
                    this._vTemplateTree.AddFirstLevelNode(template);
                }

                var nodes = this._vTemplateTree.GetFirstLevelNodes();
                var toSolveCount = nodes.length;
                var solvedCount = 0;

                if (toSolveCount === 0) {
                    return deferred.resolve();
                }

                for (let vTreeNode of nodes) {
                    let node = this.CreateTreeTemplateNodeForTemplateType(vTreeNode.Model);
                    rootNode.get_nodes().add(node);
                    // determine if the template has children
                    this._serviceTemplateCollaboration.hasChildren(vTreeNode.Model, this.filterStatus,
                        hasDescendants => {
                            if (hasDescendants) {
                                // if the newly added child has children
                                // we will not load them, instead we add an empty fake node to 
                                // determine telerking to show the plus symbol and add the expanding evetns
                                this.CreateEmptyNode(node);
                            }
                            solvedCount++;
                            if (solvedCount == toSolveCount) {
                                return deferred.resolve();
                            }
                        },
                        err => {
                            this.showNotificationException(err);
                            return deferred.reject();
                        }
                    );
                }

            }, err => {
                this.showNotificationMessage('Error loading templates');
                return deferred.reject(err);
            });

        return deferred;
    }

    private InjectChildrenForTreeNode(parentNode: Telerik.Web.UI.RadTreeNode, skip?: number): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();

        let templateNodeId: string
            = parentNode.get_attributes().getAttribute(UscTemplateCollaborationRest.TREE_NODE_ATTRIBUTE_ID);

        let vParentNode: TreeFlatNode<TemplateCollaborationModel> = this._vTemplateTree.FindNode(templateNodeId);

        this._serviceTemplateCollaboration.countDirectDescendands(vParentNode.Model, this.filterStatus,
            descendantsCount => {
                if (descendantsCount === 0) {
                    return deferred.resolve();
                }

                this._serviceTemplateCollaboration.getDirectDescendands(vParentNode.Model, this.filterStatus, skip,
                    children => {
                        if (children === null || children === undefined) {
                            throw new Error(`Error loading child templates of node with id ${templateNodeId}`);
                        }

                        // add children to the virtual tree

                        this._vTemplateTree.AddChildNodes(vParentNode, children);

                        var toSolveCount = children.length;
                        var solvedCount = 0;

                        if (toSolveCount === 0) {
                            return deferred.resolve();
                        }

                        for (let template of children) {
                            let node = this.CreateTreeTemplateNode(template);
                            parentNode.get_nodes().add(node);

                            this._serviceTemplateCollaboration.hasChildren(template, this.filterStatus,
                                hasDescendants => {
                                    if (hasDescendants) {
                                        // the fake node is required to add the + symbol in front of the node
                                        // telerik has no real support for lazy loading, we cannot forcefully set the + symbol and attach expand events
                                        this.CreateEmptyNode(node);
                                    }

                                    solvedCount++;
                                    if (solvedCount == toSolveCount) {
                                        return deferred.resolve();
                                    }
                                }, err => {
                                    this.showNotificationException(err);
                                    return deferred.reject();
                                });
                        }

                        this.ClearLoadMoreNode(parentNode);
                        var childrenNodesCount = parentNode.get_nodes().get_count();
                        if (childrenNodesCount < descendantsCount) {
                            this.CreateLoadMoreNode(parentNode);
                        }
                    }, err => {
                        this.showNotificationMessage('Error loading templates');
                        return deferred.reject(err);
                    });
            }, err => {
                this.showNotificationMessage('Error loading templates');
                return deferred.reject(err);
            }
        );        

        return deferred;
    }

    private TriggerExternalEventNodeClicked(templateNode: TreeFlatNode<TemplateCollaborationModel>) {
        $(`#${this.pnlMainContentId}`).triggerHandler(UscTemplateCollaborationRest.TREE_NODE_CLICKED, [templateNode.Model]);
    }

    private TriggerExternalEventRootNodeClicked(): void {
        $(`#${this.pnlMainContentId}`).triggerHandler(UscTemplateCollaborationRest.TREE_ROOT_NODE_CLICKED);
    }
    /**
     * NODE - CREATIONAL  
     **/

    private GetRadNodeByTemplateModel(source: TemplateCollaborationModel): Telerik.Web.UI.RadTreeNode {
        return this._treeViewTemplateCollaboration.findNodeByValue(source.UniqueId);
    }

    private CreateTreeTemplateNode(source: TemplateCollaborationModel): Telerik.Web.UI.RadTreeNode {
        if (source.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.FixedTemplates)) {
            return this.CreateTreeTemplateNodeForTemplateType(source);
        }
        else if (source.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Folder)) {
            return this.CreateTreeTemplateNodeForFolderType(source);
        }
        else if (source.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Template)) {
            return this.CreateTreeTemplateNodeForTemplateType(source);
        } else {
            throw new Error(`The template representation type ${source.RepresentationTypeValue.ValueAsString} is not supported`);
        }
    }

    private CreateTreeTemplateNodeForTemplateType(source: TemplateCollaborationModel): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        let treeNodeDescription: string = `${source.Name}`;
        let treeNodeImageUrl: string = this.GetTemplateStatusIcon(source);

        treeNode.set_text(treeNodeDescription);
        treeNode.set_value(`${source.UniqueId}`);
        treeNode.set_imageUrl(treeNodeImageUrl);
        treeNode.get_attributes().setAttribute(UscTemplateCollaborationRest.TREE_NODE_ATTRIBUTE_ID, source.UniqueId);
        return treeNode;
    }

    private CreateTreeTemplateNodeForFolderType(source: TemplateCollaborationModel): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        let treeNodeDescription: string = `${source.Name}`;
        let treeNodeImageUrl: string = '../App_Themes/DocSuite2008/imgset16/folder_closed.png';

        treeNode.set_text(treeNodeDescription);
        treeNode.set_value(`${source.UniqueId}`);
        treeNode.set_imageUrl(treeNodeImageUrl);
        treeNode.get_attributes().setAttribute(UscTemplateCollaborationRest.TREE_NODE_ATTRIBUTE_ID, source.UniqueId);
        return treeNode;
    }

    /*
     * UTILS
     */

    /**
     *  the fake node is required to add the + symbol in front of the node
     *  telerik has no real support for lazy loading, we cannot forcefully set the + symbol and attach expand events
     */
    private CreateEmptyNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text('not loaded');
        parentNode.get_nodes().add(emptyNode);
    }

    private GetTemplateStatusIcon(template: TemplateCollaborationModel): string {
        if (template.StatusValue.Equals(TemplateCollaborationStatus.Active)) {
            return '../Comm/images/TemplateCollaboration/detail_page_item_template_active.png';
        }
        else if (template.StatusValue.Equals(TemplateCollaborationStatus.Draft)) {
            return '../Comm/images/TemplateCollaboration/detail_page_item_template_draft.png';
        } else if (template.StatusValue.Equals(TemplateCollaborationStatus.NotActive)) {
            return '../Comm/images/TemplateCollaboration/detail_page_item_template_notactive.png';
        }
        return '';
    }

    private GetTreeRootNode(): JQueryDeferred<Telerik.Web.UI.RadTreeNode> {
        let deferred: JQueryDeferred<Telerik.Web.UI.RadTreeNode> = $.Deferred();

        let rootNode: Telerik.Web.UI.RadTreeNode = this._treeViewTemplateCollaboration.get_nodes().getNode(0);
        this._serviceTemplateCollaboration.getRootNode((rootNodeData: TemplateCollaborationModel) => {
            rootNode.set_value(rootNodeData.UniqueId);
            deferred.resolve(rootNode);
        }, err => {
            this.showNotificationException(err);
            deferred.reject(err);
        });
        return deferred;
    }

    private IsRootNode(node: Telerik.Web.UI.RadTreeNode) {
        return node.get_level() == 0;
    }

    private IsLoadMoreNode(node: Telerik.Web.UI.RadTreeNode): boolean {
        let nodeType: string = node.get_attributes().getAttribute(UscTemplateCollaborationRest.TREE_NODE_TYPE_NAME);
        return nodeType && nodeType == UscTemplateCollaborationRest.LOAD_MODE_TREE_NODE_TYPE;
    }

    private CreateLoadMoreNode(parent: Telerik.Web.UI.RadTreeNode): void {        
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        treeNode.set_text(UscTemplateCollaborationRest.LOADMORE_NODE_LABEL);
        treeNode.set_imageUrl(UscTemplateCollaborationRest.LOAD_MORE_NODE_IMAGEURL);

        treeNode.get_attributes().setAttribute(UscTemplateCollaborationRest.TREE_NODE_TYPE_NAME, UscTemplateCollaborationRest.LOAD_MODE_TREE_NODE_TYPE);
        
        this.CreateEmptyNode(treeNode);

        parent.get_nodes().add(treeNode);
    }

    private ClearLoadMoreNode(parent: Telerik.Web.UI.RadTreeNode): void {
        let loadMoreNodes: Telerik.Web.UI.RadTreeNode[] = parent.get_nodes().toArray().filter(x => x.get_attributes().getAttribute(UscTemplateCollaborationRest.TREE_NODE_TYPE_NAME) != null
            && x.get_attributes().getAttribute(UscTemplateCollaborationRest.TREE_NODE_TYPE_NAME) == UscTemplateCollaborationRest.LOAD_MODE_TREE_NODE_TYPE);

        for (let loadMoreNode of loadMoreNodes) {
            parent.get_nodes().remove(loadMoreNode);
        }        
    }

    /*
     * ERROR HANDLING
     */

    private ensureNotNullOrUndefined(source: any, propertyName: string): void {
        if (source === null || source === undefined) {
            throw new Error(`Invalid property state. Property \'${propertyName}\' is null or undefined.`);
        }
    }

    private showNotificationException(exception: ExceptionDTO, customMessage?: string): void {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(customMessage);
        }
    }

    private showNotificationMessage(customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }
}

export = UscTemplateCollaborationRest;