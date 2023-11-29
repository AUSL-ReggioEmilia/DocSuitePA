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

class UscTemplateCollaborationSelRest {
    uscNotificationId: string;
    ddtDocumentTypeId: string;
    pnlMainContentId: string;
    ajaxLoadingPanelId: string;
    treeViewInitializationEnabled: boolean;

    private _serviceTemplateCollaboration: TemplateCollaborationService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _vTemplateTree: TreeFlat<TemplateCollaborationModel>;
    private _ddtDocumentType: Telerik.Web.UI.RadDropDownTree;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    private static TREE_NODE_ATTRIBUTE_ID = 'tree_node_attribute_id';
    public static TREE_NODE_FIXED_TEMPLATE_CLICK = "onFixedTemplateClick";
    public static TREE_NODE_FOLDER_CLICK = "onFolderClick";
    public static TREE_NODE_TEMPLATE_CLICK = "onTemplateClick";
    public static ROOT_NODE_ID_ATTRIBUTE_NAME = "RootNodeId";
    public static DISABLE_CONFIRMA_BUTTON = "disableConfirmaButton";
    private static LOADED_EVENT: string = "onLoaded";
    
    private filterStatus: TemplateCollaborationStatus | null = null;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._vTemplateTree = new TreeFlat<TemplateCollaborationModel>(r => r.UniqueId);
    }

    public SetFilterStatus(status: number) {
        if (status in TemplateCollaborationStatus) {
            this.filterStatus = status;
        }
    }

    /**
     * Mimics the selection of the node in the tree. This method will also
     * trigger the event for external subscribers : EventName : UscTemplateCollaborationRest.TREE_NODE_CLICKED
     */
    public SelectNode(id: string, expandDropdown: boolean = true): void {
        let node = this._vTemplateTree.FindNode(id);
        if (!node) return;

        let radTreeNode = this.GetRadNodeByTemplateModel(node.Model);
        if (!radTreeNode) return;

        // Update dropdown label with selected node values
        var nodeElement: HTMLElement = radTreeNode.get_element();
        $(nodeElement).find("span.rtIn")[0].click();

        this._loadingPanel.hide(this.ddtDocumentTypeId);

        if (expandDropdown) {
            this._ddtDocumentType.toggleDropDown()
        }

        this.TemplateTreeOnNodeClicked(radTreeNode);
    }

    /**
    * Will load the tree structure from root until the bracch that contains the id.
    * WIll expand the tree until the node that contains the id.
    * Will select that node
    * @param id The id of the template collaboration
    * 
    * NOTE: loads only parents, without parent childrens
    */
    public SelectAndForceLoadNodeExclusive(id: string): void {
        this._serviceTemplateCollaboration.getById(id, model => {

            this._serviceTemplateCollaboration.getAllParentsOfTemplate(id,
                models => {
                    if (models.length === 0) {
                        return;
                    }

                    if (models.findIndex(r => r.UniqueId.toLowerCase() === id.toLowerCase()) < 0) {
                        models.push(model);
                    }

                    models = models.sort((a, b) => a.TemplateCollaborationPath.localeCompare(b.TemplateCollaborationPath));

                    let parentNode = this.InjectExistingChildrenForRootNode(models[0]);

                    for (let i = 1; i < models.length; i++) {
                        parentNode = this.InjectExistingChildrenForTreeNode(parentNode, models[i]);
                    }

                    this.SelectNode(models[models.length - 1].UniqueId);
                },
                err => {
                    this.showNotificationException(err);
                });
        });
    }

    /**
     * Will load the tree structure from root until the bracch that contains the id.
     * WIll expand the tree until the node that contains the id.
     * Will select that node
     * @param id The id of the template collaboration
     * 
     * NOTE: loads all parents and childrens
     */
    public SelectAndForceLoadNode(id: string, expandDropdown: boolean = true): void {
        this._loadingPanel.show(this.ddtDocumentTypeId);
        this._serviceTemplateCollaboration.getById(id, model => {
            let path = model.TemplateCollaborationPath;

            // becuase the tree is lazy loaded, this method will try to load the tree branch up to the provided id
            // the model we received has a path and the path is of the shape /1/2/3/4/5
            // we will call a series of deferred methods, one after another that will determine 
            let pathSegments = path.split('/');

            pathSegments = pathSegments.filter(r => r.length > 0);

            let fullPaths = []

            var lastSegment = '/';
            for (let segment of pathSegments) {
                lastSegment = lastSegment + segment + '/';
                fullPaths.push(lastSegment);
            }

            if (pathSegments.length === 0) {
                return
            }

            this._serviceTemplateCollaboration.getMultipleByTemplateCollaborationPath(fullPaths, models => {
                if (models.length === 0) {
                    return
                }

                this.ReloadRoot()
                    .then(() => {
                        var deferedAll: JQueryDeferred<void>[] = [];
                        let orderedModels = models.sort((x, y) => x.TemplateCollaborationPath.localeCompare(y.TemplateCollaborationPath));

                        for (let i = 0; i < orderedModels.length; i++) {
                            deferedAll.push($.Deferred());
                        }

                        for (let i = 0; i < orderedModels.length; i++) {
                            let model = orderedModels[i];

                            if (i === 0) {
                                this.ReloadContent(model.UniqueId)
                                    .then(() => {
                                        deferedAll[i].resolve()
                                        if (i === models.length - 1) {
                                            this.SelectNode(id, expandDropdown);
                                        }
                                    })
                                    .fail(err => deferedAll[i].fail(err));

                            } else {
                                deferedAll[i - 1].then(() => {
                                    this.ReloadContent(model.UniqueId)
                                        .then(() => {
                                            deferedAll[i].resolve()

                                            if (i === models.length - 1) {
                                                this.SelectNode(id, expandDropdown);
                                            }
                                        })
                                        .fail(err => deferedAll[i].fail(err));
                                });
                            }
                        }
                    });
            });
        });
    }

    /**
    * Refetches content for root and expands the root node.
    * Acts like a refresh button of the root node
    **/
    public ReloadRoot(): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();
        this._loadingPanel.show(this.ddtDocumentTypeId);

        this.GetTreeRootNode().then(() => {
            this.TemplateTreeOnNodeExpanding(null)
                .then(() => {
                    deferred.resolve();
                })
                .always(() => {
                    this._loadingPanel.hide(this.ddtDocumentTypeId);
                })
                .fail((err) => {
                    deferred.fail(err);
                });
        })
            .fail((err) => {
            this._loadingPanel.hide(this.ddtDocumentTypeId);
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

    // called from aspx page
    public initialize(): void {
        try {
            this.initializeServices();
            this.bootstrapControls();
            this.initializeEvents();

            if (this.treeViewInitializationEnabled) {
                this.ReloadRoot();
            }

            this.bindMe();
        } catch (err) {
            this.showNotificationException(err);
        }
    }

    public clearCurrentSelection = (): void => {
        this._ddtDocumentType.get_entries().clear();
    }

    public disableTreeview = (): void => {
        this._ddtDocumentType.set_enabled(false);
    }

    private bindMe(): void {
        $(`#${this.pnlMainContentId}`).data(this);
        $(`#${this.pnlMainContentId}`).triggerHandler(UscTemplateCollaborationSelRest.LOADED_EVENT);
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
        this._ddtDocumentType = <Telerik.Web.UI.RadDropDownTree>$find(this.ddtDocumentTypeId);
        this.ensureNotNullOrUndefined(this._ddtDocumentType, '_treeViewTemplateCollaboration');
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
    }

    private initializeEvents(): void {
        let _this = this;

        this._ddtDocumentType.get_embeddedTree().add_nodeExpanding(_this.TemplateTreeOnNodeExpandingHandler);
        this._ddtDocumentType.get_embeddedTree().add_nodeClicked(_this.TemplateTreeOnNodeClickedHandler);
    }

    /**
     * TREE EXTERNAL EVENTS
     */

    /**
     * The fixed tempalte is the top level template that can host folders and tempaltes
     */
    public  OnFixedTemplateClick = (restComponentId: string, handler: { (model: TemplateCollaborationModel): void }) => {
        $(`#${restComponentId}`).on(UscTemplateCollaborationSelRest.TREE_NODE_FIXED_TEMPLATE_CLICK, (jvEvent, args) => {
            handler(args);
        });
    }

    public  OnFolderClick = (restComponentId: string, handler: { (fixedTemplate: TemplateCollaborationModel, folder: TemplateCollaborationModel): void }) => {
        $(`#${restComponentId}`).on(UscTemplateCollaborationSelRest.TREE_NODE_FOLDER_CLICK, (jvEvent, argFixedTemplate, argFolder) => {
            handler(argFixedTemplate, argFolder);
        });
    }

    public  OnTemplateClick = (restComponentId: string, handler: { (fixedTemplate: TemplateCollaborationModel, template: TemplateCollaborationModel): void }) => {
        $(`#${restComponentId}`).on(UscTemplateCollaborationSelRest.TREE_NODE_TEMPLATE_CLICK, (jvEvent, argFixedTemplate, argTemplate) => {
            handler(argFixedTemplate, argTemplate);
        });
    }

    public  OnFolderClick_DisableConfirmaButton = (restComponentId: string, handler: { (disableButton : boolean): void }) => {
        $(`#${restComponentId}`).on(UscTemplateCollaborationSelRest.DISABLE_CONFIRMA_BUTTON, (jvEvent, disableButton) => {
            handler(disableButton);
        });
    }

    /**
     * TREE EVENTS
     */

    private TemplateTreeOnNodeExpandingHandler = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let _this;
        try {
            /*
             * Get the binded element from the document. This makes it possible for external
             * calls on other methods to interact with this class because in the context of an external page calling
             * a method on this page, the "this object" is actually the binded one
             */
            _this = <UscTemplateCollaborationSelRest>$(`#${this.pnlMainContentId}`).data();
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

    private TemplateTreeOnNodeClickedHandler = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let _this;
        try {
            /*
             * Get the binded element from the document. This makes it possible for external
             * calls on other methods to interact with this class because in the context of an external page calling 
             * a method on this page, the "this object" is actually the binded one
             */
            _this = <UscTemplateCollaborationSelRest>$(`#${this.pnlMainContentId}`).data();
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

    private TemplateTreeOnNodeClicked(clikedNode: Telerik.Web.UI.RadTreeNode) {
        let templateNodeId: string = clikedNode.get_attributes().getAttribute(UscTemplateCollaborationSelRest.TREE_NODE_ATTRIBUTE_ID);
        let templateNode: TreeFlatNode<TemplateCollaborationModel> = this._vTemplateTree.FindNode(templateNodeId);

        if (templateNode.Model.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Folder)) {
            clikedNode.unselect();
            $(`#${this._ddtDocumentType.get_id()} .rddtFakeInput`).text("");
            $(`#${this.pnlMainContentId}`).triggerHandler(UscTemplateCollaborationSelRest.DISABLE_CONFIRMA_BUTTON, true);

            return;
        }

        $(`#${this.pnlMainContentId}`).triggerHandler(UscTemplateCollaborationSelRest.DISABLE_CONFIRMA_BUTTON, false);
        this.TriggerExternalEventNodeClicked(templateNode.Model);
    }

    private TemplateTreeOnNodeExpanding(expandingNode: Telerik.Web.UI.RadTreeNode | null): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();

        try {
            if (expandingNode === null) {
                let rootNodeId: string = this._ddtDocumentType.get_embeddedTree().get_attributes().getAttribute(UscTemplateCollaborationSelRest.ROOT_NODE_ID_ATTRIBUTE_NAME);
                this._ddtDocumentType.get_embeddedTree().get_nodes().clear();
                this.InjectChildrenForRootNode(rootNodeId)
                    .then(() => {
                        deferred.resolve();
                    })
                    .fail((err) => {
                        deferred.reject(err)
                    });
            } else {
                expandingNode.get_nodes().clear();
                expandingNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);

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

    private InjectExistingChildrenForRootNode(model: TemplateCollaborationModel): Telerik.Web.UI.RadTreeNode {
        this._vTemplateTree.Clear();
        this._vTemplateTree.AddFirstLevelNode(model);
        let node = this.CreateTreeTemplateNodeForTemplateType(model);
        this._ddtDocumentType.get_embeddedTree().get_nodes().add(node);
        return node;
    }

    private InjectExistingChildrenForTreeNode(parentNode: Telerik.Web.UI.RadTreeNode, childNode: TemplateCollaborationModel): Telerik.Web.UI.RadTreeNode {
        let templateNodeId: string
            = parentNode.get_attributes().getAttribute(UscTemplateCollaborationSelRest.TREE_NODE_ATTRIBUTE_ID);
        let vParentNode: TreeFlatNode<TemplateCollaborationModel> = this._vTemplateTree.FindNode(templateNodeId);


        this._vTemplateTree.AddChildNodes(vParentNode, [childNode]);
        let node = this.CreateTreeTemplateNode(childNode);
        parentNode.get_nodes().add(node);
        parentNode.set_expanded(true);
        return node;
    }

    private InjectChildrenForRootNode(rootNodeValue: string): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();

        this._serviceTemplateCollaboration.getFixedTemplates(rootNodeValue,
            fixedTemplates => {
                this.ensureNotNullOrUndefined(fixedTemplates, 'fixedTemplates');
                this._vTemplateTree.Clear();
                for (let template of fixedTemplates) {
                    this._vTemplateTree.AddFirstLevelNode(template);
                }

                var nodes = this._vTemplateTree.GetFirstLevelNodes();
                if (nodes.length === 0) {
                    return deferred.resolve();
                }

                for (let vTreeNode of nodes) {
                    let node: Telerik.Web.UI.RadTreeNode = this.CreateTreeTemplateNodeForTemplateType(vTreeNode.Model);
                    // determine if the template has children
                    this._ddtDocumentType.get_embeddedTree().get_nodes().add(node);
                    this.CreateEmptyNode(node);
                }

                return deferred.resolve();

            }, err => {
                this.showNotificationMessage('Error loading templates');
                return deferred.reject(err);
            });

        return deferred;
    }

    private InjectChildrenForTreeNode(parentNode: Telerik.Web.UI.RadTreeNode): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();

        let templateNodeId: string
            = parentNode.get_attributes().getAttribute(UscTemplateCollaborationSelRest.TREE_NODE_ATTRIBUTE_ID);

        let vParentNode: TreeFlatNode<TemplateCollaborationModel> = this._vTemplateTree.FindNode(templateNodeId);

        this._serviceTemplateCollaboration.getFixedTemplates(templateNodeId,
            children => {
                if (children === null || children === undefined) {
                    throw new Error(`Error loading child templates of node with id ${templateNodeId}`);
                }

                if (!children.length) {

                    if (!vParentNode.Model.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Template)) {
                        this.CreateEmptyNode(parentNode);
                    }

                    return deferred.resolve();
                }

                // add children to the virtual tree
                children = this.orderChildrenByRepresentationTypeAndPath(children);

                this._vTemplateTree.AddChildNodes(vParentNode, children);

                for (let template of children) {
                    let node = this.CreateTreeTemplateNode(template);
                    parentNode.get_nodes().add(node);

                    if (!template.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Template)) {
                        this.CreateEmptyNode(node);
                    }
                }

                return deferred.resolve();
            }, err => {
                this.showNotificationMessage('Error loading templates');
                return deferred.reject(err);
            }, this.filterStatus);

        return deferred;
    }

    private orderChildrenByRepresentationTypeAndPath(children: TemplateCollaborationModel[]) {
        let folders = children.filter(x => x.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Folder));
        let templates = children.filter(x => !x.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Folder));
        let orderedFolders = folders.sort((x, y) => x.TemplateCollaborationPath.localeCompare(y.TemplateCollaborationPath));
        let orderedTemplates = templates.sort((x, y) => x.TemplateCollaborationPath.localeCompare(y.TemplateCollaborationPath));
        
        return orderedFolders.concat(orderedTemplates);
    }

    private TriggerExternalEventNodeClicked(templateModel: TemplateCollaborationModel) {
        if (templateModel.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.FixedTemplates)) {
            $(`#${this.pnlMainContentId}`)
                .triggerHandler(UscTemplateCollaborationSelRest.TREE_NODE_FIXED_TEMPLATE_CLICK, [templateModel]);
        }

        if (templateModel.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Template)) {
            let fixedTemplatePath = templateModel.TemplateCollaborationPathValue.GetTopFolder();
            this._serviceTemplateCollaboration.getByTemplateCollaborationPath(
                fixedTemplatePath,
                (fixedTemplate) => {
                    $(`#${this.pnlMainContentId}`)
                        .triggerHandler(UscTemplateCollaborationSelRest.TREE_NODE_TEMPLATE_CLICK, [fixedTemplate, templateModel]);
                },
                err => {
                    this.showNotificationMessage(`Error loading the template ${fixedTemplatePath}`);
                });
        }

    }

    /**
     * NODE - CREATIONAL  
     **/

    private GetRadNodeByTemplateModel(source: TemplateCollaborationModel): Telerik.Web.UI.RadTreeNode {
        return this._ddtDocumentType.get_embeddedTree().findNodeByValue(source.UniqueId);
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
        treeNode.get_attributes().setAttribute(UscTemplateCollaborationSelRest.TREE_NODE_ATTRIBUTE_ID, source.UniqueId);
        return treeNode;
    }

    private CreateTreeTemplateNodeForFolderType(source: TemplateCollaborationModel): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        let treeNodeDescription: string = `${source.Name}`;
        let treeNodeImageUrl: string = '../App_Themes/DocSuite2008/imgset16/folder_closed.png';

        treeNode.set_text(treeNodeDescription);
        treeNode.set_value(`${source.UniqueId}`);
        treeNode.set_imageUrl(treeNodeImageUrl);
        treeNode.get_attributes().setAttribute(UscTemplateCollaborationSelRest.TREE_NODE_ATTRIBUTE_ID, source.UniqueId);

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
        emptyNode.set_text('Nessun elemento trovato');
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

    private GetTreeRootNode(): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();

        this._serviceTemplateCollaboration.getRootNode((rootNodeData: TemplateCollaborationModel) => {
            this._ddtDocumentType.get_embeddedTree().get_attributes().setAttribute(UscTemplateCollaborationSelRest.ROOT_NODE_ID_ATTRIBUTE_NAME, rootNodeData.UniqueId);
            deferred.resolve();
        }, err => {
            this.showNotificationException(err);
            deferred.reject(err);
        });
        return deferred;
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

export = UscTemplateCollaborationSelRest;