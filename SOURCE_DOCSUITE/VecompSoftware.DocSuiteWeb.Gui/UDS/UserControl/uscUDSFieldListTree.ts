import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UDSFieldListModel = require('App/Models/UDS/UDSFieldListModel');
import UDSFieldListService = require('App/Services/UDS/UDSFieldListService');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class uscUDSFieldListTree {
    rtvReadOnlyUDSFieldListId: string;
    rddtUDSFieldListId: string;
    uscNotificationId: string;
    ajaxManagerId: string;
    idUDSRepository: string;
    udsFieldListChildren: string;
    isReadOnly: boolean;
    hiddenFieldListId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _udsFieldListService: UDSFieldListService;
    private _rtvUDSFieldList: Telerik.Web.UI.RadTreeView;
    private _rtvReadOnlyUDSFieldList: Telerik.Web.UI.RadTreeView;
    private _rddtUDSFieldList: Telerik.Web.UI.RadDropDownTree;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _udsFieldListParents: Map<string, UDSFieldListModel[]> = new Map<string, UDSFieldListModel[]>();
    private _hiddenFieldList: JQuery;
    private hiddenFieldListValues: any = {};
    private readonly _uscId: string;

    private static UDSFieldList_SERVICE_NAME = "UDSFieldList";
    private static UDSFieldList_Selected_ACTION_NAME = "UDSFieldList_Selected";
    private static FIELD_NAME_ATTRIBUTE_NAME = "FieldName";

    constructor(serviceConfigurations: ServiceConfiguration[], uscId: string) {
        this._serviceConfigurations = serviceConfigurations;
        this._uscId = uscId;

        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, uscUDSFieldListTree.UDSFieldList_SERVICE_NAME);
        this._udsFieldListService = new UDSFieldListService(serviceConfiguration);
    }

    initialize() {
        this._rddtUDSFieldList = <Telerik.Web.UI.RadDropDownTree>$find(this.rddtUDSFieldListId);
        this._rtvUDSFieldList = this._rddtUDSFieldList.get_embeddedTree();
        this._rtvUDSFieldList.add_nodeExpanded(this.rtvUDSFieldList_onExpand);
        this._rtvUDSFieldList.add_nodeClicked(this.rtvUDSFieldList_onClick);
        this._rtvReadOnlyUDSFieldList = <Telerik.Web.UI.RadTreeView>$find(this.rtvReadOnlyUDSFieldListId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._hiddenFieldList = $(`#${this.hiddenFieldListId}`);

        if (this.isReadOnly) {
            $(`#${this.rddtUDSFieldListId}`).hide();
            $(`#${this.rtvReadOnlyUDSFieldListId}`).show();
            this.loadReadOnlyTree();
        }
        else {
            $(`#${this.rddtUDSFieldListId}`).show();
            $(`#${this.rtvReadOnlyUDSFieldListId}`).hide();
            if (this.idUDSRepository && this.idUDSRepository !== "") {
                this.loadUDSFieldListTree(this.idUDSRepository);
            }
        }
    }

    loadUDSFieldListTree(idUDSRepository: string): void {
        if (idUDSRepository === "") return;
        let fieldName: string = this._rtvUDSFieldList.get_attributes().getAttribute(uscUDSFieldListTree.FIELD_NAME_ATTRIBUTE_NAME);
        this._rtvUDSFieldList.get_nodes().clear();
        this._udsFieldListService.getUDSFieldListRoot(idUDSRepository, fieldName ? fieldName : "", (data: UDSFieldListModel[]) => {
            if (!data && !this._hiddenFieldList.val()) return;
            for (let item of data) {
                this.createNode(item, true);
            }
            this.expandAllParents().done(() => {
                if (this._hiddenFieldList.val()) {
                    let fieldNameAttribute: string = this._rtvUDSFieldList.get_attributes().getAttribute(uscUDSFieldListTree.FIELD_NAME_ATTRIBUTE_NAME);
                    this.hiddenFieldListValues = JSON.parse(this._hiddenFieldList.val());
                    let selectedValue: string = this.hiddenFieldListValues[this._uscId];
                    if (!selectedValue) {
                        return;
                    }
                    if (this._rtvUDSFieldList.findNodeByValue(selectedValue)) {
                        this._rtvUDSFieldList.findNodeByValue(selectedValue).select();
                        this.setDropDownTreeText(fieldNameAttribute);
                    }
                }
            });
        }, (exception: ExceptionDTO) => {
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    createNode(udsFieldList: UDSFieldListModel, needEmptyNode: boolean, parentNode?: Telerik.Web.UI.RadTreeNode): void {
        this._rtvUDSFieldList.trackChanges();
        let childNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        childNode.set_value(udsFieldList.UniqueId);
        childNode.set_text(udsFieldList.Name);
        if (needEmptyNode) {
            this.addEmptyNode(childNode);
        }
        if (parentNode) {
            parentNode.get_nodes().add(childNode);
        }
        else {
            if (this.isReadOnly) {
                this._rtvReadOnlyUDSFieldList.get_nodes().add(childNode);
            }
            else {
                this._rtvUDSFieldList.get_nodes().add(childNode);
            }
        }
        this._rtvUDSFieldList.commitChanges();
    }

    addEmptyNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text("");
        parentNode.get_nodes().add(emptyNode);
    }

    rtvUDSFieldList_onExpand = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs): void => {
        this.rtvUDSFieldList_Expand(args.get_node(), false).done(() => { });
    }

    rtvUDSFieldList_onClick = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs): void => {
        this._rddtUDSFieldList.closeDropDown();
        this.setHiddenFieldList(args.get_node().get_value());
    }

    rtvUDSFieldList_Expand(expandedNode: Telerik.Web.UI.RadTreeNode, automaticExpandMode: boolean): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let fieldNameAttribute: string = this._rtvUDSFieldList.get_attributes().getAttribute(uscUDSFieldListTree.FIELD_NAME_ATTRIBUTE_NAME);
        this.selectFirstNode(fieldNameAttribute, expandedNode.get_level());
        this._udsFieldListService.getChildrenByParent(expandedNode.get_value(), (data: UDSFieldListModel[]) => {
            expandedNode.get_nodes().clear();
            for (let childData of data) {
                this.createNode(childData, true, expandedNode);
            }
            let existsUDSFieldListParents: boolean = this._udsFieldListParents
                && this._udsFieldListParents.get(fieldNameAttribute)
                && this._udsFieldListParents.get(fieldNameAttribute).length > 0;
            if (existsUDSFieldListParents && expandedNode.get_level() + 1 < this._udsFieldListParents.get(fieldNameAttribute).length && automaticExpandMode) {
                let nextParentModel: UDSFieldListModel = this._udsFieldListParents.get(fieldNameAttribute)[expandedNode.get_level() + 1];
                let nextParentNode: Telerik.Web.UI.RadTreeNode = this._rtvUDSFieldList.findNodeByValue(nextParentModel.UniqueId);
                if (!nextParentNode) {
                    return promise.reject();
                }
                if (nextParentNode.get_level() + 1 === this._udsFieldListParents.get(fieldNameAttribute).length) {
                    nextParentNode.select();
                    this.setDropDownTreeText(fieldNameAttribute);
                    this.setHiddenFieldList(nextParentNode.get_value());
                }
                else {
                    nextParentNode.expand();
                    this.rtvUDSFieldList_Expand(nextParentNode, automaticExpandMode).done(() => {
                        return promise.resolve();
                    });
                }
            }
            return promise.resolve();
        }, (exception: ExceptionDTO) => {
            this.showNotificationException(this.uscNotificationId, exception);
            return promise.reject();
        });
        return promise.promise();
    }

    private selectFirstNode(fieldNameAttribute: string, expandedNodeLevel: number) {
        if (!this._udsFieldListParents.get(fieldNameAttribute)) {
            return;
        }
        let currentParentModel: UDSFieldListModel = this._udsFieldListParents.get(fieldNameAttribute)[expandedNodeLevel];
        if (currentParentModel) {
            let currentParentNode: Telerik.Web.UI.RadTreeNode = this._rtvUDSFieldList.findNodeByValue(currentParentModel.UniqueId);
            if (currentParentNode && currentParentNode.get_level() + 1 === this._udsFieldListParents.get(fieldNameAttribute).length) {
                currentParentNode.select();
                this.setDropDownTreeText(fieldNameAttribute);
                this.setHiddenFieldList(currentParentNode.get_value());
            }
        }
    }

    private expandAllParents(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!this.udsFieldListChildren || !JSON.parse(this.udsFieldListChildren)) {
            return promise.resolve();
        }
        let udsFieldListChildren: any[] = JSON.parse(this.udsFieldListChildren);
        let fieldNameAttribute: string = this._rtvUDSFieldList.get_attributes().getAttribute(uscUDSFieldListTree.FIELD_NAME_ATTRIBUTE_NAME);
        let udsFieldListChild: any = udsFieldListChildren.filter(x => x.Key === fieldNameAttribute)[0];
        if (!udsFieldListChild) {
            return promise.resolve();
        }
        this._udsFieldListService.getAllParents(udsFieldListChild.Value, (data: UDSFieldListModel[]) => {
            this._udsFieldListParents.set(fieldNameAttribute, data.reverse());
            let firstParentNode: Telerik.Web.UI.RadTreeNode = this._rtvUDSFieldList.findNodeByValue(this._udsFieldListParents.get(fieldNameAttribute)[0].UniqueId);
            firstParentNode.expand();
            this.rtvUDSFieldList_Expand(firstParentNode, true).done(() => {
                return promise.resolve();
            });
        }, (exception: ExceptionDTO) => {
            this.showNotificationException(this.uscNotificationId, exception);
            return promise.reject();
        });
        return promise.promise();
    }

    private setDropDownTreeText(fieldNameAttribute: string): void {
        let dropDownText: string;
        if (this._rddtUDSFieldList.get_entries().get_count() === 0) {
            let parentTextList: string[] = [];
            if (this._rtvUDSFieldList.get_selectedNode()) {
                let parentNode: Telerik.Web.UI.RadTreeNode = this._rtvUDSFieldList.get_selectedNode();
                while (parentNode instanceof Telerik.Web.UI.RadTreeNode) {
                    parentTextList.push(parentNode.get_text());
                    parentNode = parentNode.get_parent();
                }
            }
            dropDownText = parentTextList.reverse().join(" -> ");
        }
        $(`#${this._rddtUDSFieldList.get_id()} .rddtFakeInput`).text(dropDownText);
    }

    private setHiddenFieldList(nodeValue: string): void {
        let currentPostbackState: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_UDS_FIELD_LIST_POSTBACK_STATE);
        this.hiddenFieldListValues = currentPostbackState ? JSON.parse(currentPostbackState) : {};
        this.hiddenFieldListValues[this._uscId] = nodeValue;
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_UDS_FIELD_LIST_POSTBACK_STATE, JSON.stringify(this.hiddenFieldListValues));
        this._hiddenFieldList.val(JSON.stringify(this.hiddenFieldListValues));
    }

    loadReadOnlyTree(): void {
        if (!this.udsFieldListChildren || !JSON.parse(this.udsFieldListChildren)) {
            return;
        }
        let udsFieldListChildren: any[] = JSON.parse(this.udsFieldListChildren);
        let fieldNameAttribute: string = this._rtvUDSFieldList.get_attributes().getAttribute(uscUDSFieldListTree.FIELD_NAME_ATTRIBUTE_NAME);
        let udsFieldListChild: any = udsFieldListChildren.filter(x => x.Key === fieldNameAttribute)[0];
        if (!udsFieldListChild) return;
        this._udsFieldListService.getAllParents(udsFieldListChild.Value, (data: UDSFieldListModel[]) => {
            this._udsFieldListParents.set(fieldNameAttribute, data.reverse());
            this.createNode(this._udsFieldListParents.get(fieldNameAttribute)[0], false);
            let parentNode: Telerik.Web.UI.RadTreeNode = this._rtvReadOnlyUDSFieldList.findNodeByValue(this._udsFieldListParents.get(fieldNameAttribute)[0].UniqueId);
            parentNode.expand();
            for (let parentModel of this._udsFieldListParents.get(fieldNameAttribute).filter(x => x.UDSFieldListLevel > 3)) {
                this.createNode(parentModel, false, parentNode);
                parentNode = this._rtvReadOnlyUDSFieldList.findNodeByValue(parentModel.UniqueId);
                parentNode.expand();
            }
        }, (exception: ExceptionDTO) => {
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    private showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception && exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(customMessage);
            }
        }
    }
}

export = uscUDSFieldListTree;