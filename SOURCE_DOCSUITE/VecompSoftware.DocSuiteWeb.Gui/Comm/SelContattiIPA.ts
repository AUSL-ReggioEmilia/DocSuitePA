import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import IPAAdministrationModel = require("App/Models/IPA/IPAAdministrationModel");
import IPAType = require("App/Models/IPA/IPAType");
import ImageHelper = require("../App/Helpers/ImageHelper");
import IPAAOOModel = require("../App/Models/IPA/IPAAOOModel");
import IPAOUModel = require("../App/Models/IPA/IPAOUModel");
import IPABase = require("../App/Models/IPA/IPABase");
import AjaxModel = require("../App/Models/AjaxModel");
import { ajax } from "jquery";

class SelContattiIPA {

    private treeViewId: string;
    private btnSearchId: string;
    private ajaxManagerId: string;
    private txtSearchId: string;
    private btnConfermaId: string;
    private btnConfermaNuovoId: string;
    private cmdDetailId: string;
    private ajaxLoadingPanelId: string;
    private callerId: string;
    private windowDetailsId: string;
    private windowManagerDetailId: string;

    get selectedNode(): Telerik.Web.UI.RadTreeNode {
        return this._treeView.get_selectedNode();
    }

    // Fields
    private readonly _serviceConfigurations: ServiceConfiguration[];
    private readonly _loadingDeferreds: JQueryDeferred<void>[] = [];
    private readonly _folderLoadingDeferreds: JQueryDeferred<void>[] = [];

    // Controls
    private _treeView: Telerik.Web.UI.RadTreeView;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _txtSearch: Telerik.Web.UI.RadTextBox;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _btnConfirmAndNew: Telerik.Web.UI.RadButton;
    private _btnDetails: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    // Consts
    private static readonly IPA_CONTACT_TYPE_ATTRIBUTE = "IPAContactType";
    private static readonly NODE_TYPE_ATTRIBUTE = "NodeType";
    private static readonly FOLDER_CORRELATION_ATTRIBUTE = "CodAmm";
    private static readonly IPA_DATA_ATTRIBUTE = "IPAData";

    private static readonly IPA_NODE_TYPE = "IPANodeType";
    private static readonly FOLDER_NODE_TYPE = "FolderNodeType";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    // Events
    private btnSearch_onClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        let filter: string = this._txtSearch.get_value();
        if (!filter || filter.length < 2) {
            alert("Impossibile cercare. Inserire almeno 2 caratteri nel campo di ricerca");
            args.set_cancel(true);
            return;
        }

        let promise: JQueryDeferred<void> = $.Deferred<void>();
        promise.done(() => this._loadingPanel.hide(this.treeViewId));

        this._loadingPanel.show(this.treeViewId);
        this._loadingDeferreds.push(promise);

        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(filter);
        ajaxModel.ActionName = "SearchAdministrations";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        args.set_cancel(true);
    }

    private btnConfirm_onClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        if (this.selectedNode == null) {
            alert("E' necessario selezionare un contatto");
            args.set_cancel(true);
            return;
        }

        let nodeType: string = this.selectedNode.get_attributes().getAttribute(SelContattiIPA.NODE_TYPE_ATTRIBUTE);
        if (nodeType == SelContattiIPA.FOLDER_NODE_TYPE) {
            alert("Elemento selezionato non valido, selezionare un contatto valido dalla lista");
            args.set_cancel(true);
            return;
        }

        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(IPAType[this.selectedNode.get_attributes().getAttribute(SelContattiIPA.IPA_CONTACT_TYPE_ATTRIBUTE)]);
        ajaxModel.Value.push(JSON.stringify(this.selectedNode.get_attributes().getAttribute(SelContattiIPA.IPA_DATA_ATTRIBUTE)));
        ajaxModel.Value.push(`${(sender.get_commandArgument() == "confirm")}`);
        ajaxModel.ActionName = "Confirm";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        args.set_cancel(true);
    }

    private btnDetails_onClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        if (this.selectedNode == null) {
            alert("E' necessario selezionare un contatto");
            return;
        }

        let nodeType: string = this.selectedNode.get_attributes().getAttribute(SelContattiIPA.NODE_TYPE_ATTRIBUTE);
        if (nodeType == SelContattiIPA.FOLDER_NODE_TYPE) {
            alert("Elemento selezionato non valido, selezionare un contatto valido dalla lista");
            return;
        }

        let promise: JQueryDeferred<void> = $.Deferred<void>();
        promise.done(() => this._loadingPanel.hide(this.treeViewId));

        this._loadingPanel.show(this.treeViewId);
        this._loadingDeferreds.push(promise);

        let ipaType: IPAType = this.selectedNode.get_attributes().getAttribute(SelContattiIPA.IPA_CONTACT_TYPE_ATTRIBUTE) as IPAType;
        let ipaEntitySerialized: string = JSON.stringify(this.selectedNode.get_attributes().getAttribute(SelContattiIPA.IPA_DATA_ATTRIBUTE));
        if (ipaType == IPAType.Administration) {
            let ajaxModel: AjaxModel = <AjaxModel>{};
            ajaxModel.Value = new Array<string>();
            ajaxModel.Value.push(ipaEntitySerialized);
            ajaxModel.ActionName = "ShowAdministrationDetails";
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        } else {
            this.showDetails(ipaEntitySerialized, ipaType);
        }
        
        args.set_cancel(true);
    }

    private treeView_onNodeExpanding = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let nodeType: string = args.get_node().get_attributes().getAttribute(SelContattiIPA.NODE_TYPE_ATTRIBUTE);
        switch (nodeType) {
            case SelContattiIPA.FOLDER_NODE_TYPE: {
                this.initializeNodeLoadingBehaviour(args.get_node());
                let codAmm: string = args.get_node().get_attributes().getAttribute(SelContattiIPA.FOLDER_CORRELATION_ATTRIBUTE);
                let folderType: IPAType = args.get_node().get_value();
                switch (folderType) {
                    case IPAType.AOO: {
                        this.searchAOOs(codAmm);
                        break;
                    }
                    case IPAType.OU: {
                        this.searchOUs(codAmm);
                        break;
                    }
                    default: {
                        throw new Error(`folder of type ${folderType} is not valid in this context`);
                    }
                }
                break;
            }
            case SelContattiIPA.IPA_NODE_TYPE: {
                let ipaContactType: IPAType = args.get_node().get_attributes().getAttribute(SelContattiIPA.IPA_CONTACT_TYPE_ATTRIBUTE);
                console.info(`expanding IPA contact node ${args.get_node().get_value()} of type ${ipaContactType}`);
                break;
            }
            default: {
                throw new Error(`node of type ${nodeType} is not valid in this context`);
            }
        }
    }

    private treeView_onNodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let nodeType: string = args.get_node().get_attributes().getAttribute(SelContattiIPA.NODE_TYPE_ATTRIBUTE);
        this._btnDetails.set_enabled(nodeType != SelContattiIPA.FOLDER_NODE_TYPE);
    }

    // Methods
    initialize(): void {
        this.initializeControls();
        this.initializeBehaviours();
    }

    initializeControls(): void {
        this._treeView = $find(this.treeViewId) as Telerik.Web.UI.RadTreeView;
        this._btnSearch = $find(this.btnSearchId) as Telerik.Web.UI.RadButton;
        this._ajaxManager = $find(this.ajaxManagerId) as Telerik.Web.UI.RadAjaxManager;
        this._txtSearch = $find(this.txtSearchId) as Telerik.Web.UI.RadTextBox;
        this._btnConfirm = $find(this.btnConfermaId) as Telerik.Web.UI.RadButton;
        this._btnConfirmAndNew = $find(this.btnConfermaNuovoId) as Telerik.Web.UI.RadButton;
        this._btnDetails = $find(this.cmdDetailId) as Telerik.Web.UI.RadButton;
        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
    }

    initializeBehaviours(): void {
        this._treeView.add_nodeExpanding(this.treeView_onNodeExpanding);
        this._treeView.add_nodeClicked(this.treeView_onNodeClicked);
        this._btnSearch.add_clicking(this.btnSearch_onClicked);
        this._btnConfirm.add_clicking(this.btnConfirm_onClicked);
        this._btnConfirmAndNew.add_clicking(this.btnConfirm_onClicked);
        this._btnDetails.add_clicking(this.btnDetails_onClicked);

        this._txtSearch.focus();
        this._btnConfirm.set_commandArgument("confirm");
        this._btnConfirmAndNew.set_commandArgument("confirmAndNew");
    }

    searchAdministrationsCallback(serialized: string): void {
        let ipaAdministrations: IPAAdministrationModel[] = JSON.parse(serialized);
        this._treeView.get_nodes().clear();

        let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        rootNode.set_text(`Pubbliche amministrazioni (${ipaAdministrations.length})`);
        this.appendNode(rootNode);

        if (ipaAdministrations.length == 0) {
            this._loadingDeferreds.forEach((promise: JQueryDeferred<void>) => promise.resolve());
            return;
        }

        ipaAdministrations = ipaAdministrations.sort((a, b) => a.des_amm < b.des_amm ? -1 : 1);
        let ipaNode: Telerik.Web.UI.RadTreeNode;
        for (let ipaAdministration of ipaAdministrations) {
            ipaNode = this.createIPANode(IPAType.Administration, ipaAdministration.des_amm, ipaAdministration.cod_amm);
            ipaNode.get_attributes().setAttribute(SelContattiIPA.IPA_DATA_ATTRIBUTE, ipaAdministration);
            this.appendNode(ipaNode, rootNode);
        }

        rootNode.expand();
        this._loadingDeferreds.forEach((promise: JQueryDeferred<void>) => promise.resolve());
    }

    searchAOOCallback(serialized: string, codiceAmministrazione: string): void {
        let ipaAOOs: IPAAOOModel[] = JSON.parse(serialized);
        let ipaAdministrationNode: Telerik.Web.UI.RadTreeNode = this._treeView.findNodeByValue(codiceAmministrazione);
        if (ipaAdministrationNode == null) {
            throw new Error(`TreeView node with value "${codiceAmministrazione}" has not found`);
        }

        let aooFolderNode: Telerik.Web.UI.RadTreeNode = ipaAdministrationNode.get_nodes().toArray().find(x => x.get_value() == IPAType.AOO);
        aooFolderNode.get_nodes().clear();

        ipaAOOs = ipaAOOs.sort((a, b) => a.des_aoo < b.des_aoo ? -1 : 1);
        let ipaNode: Telerik.Web.UI.RadTreeNode;
        for (let ipaAOO of ipaAOOs) {
            ipaNode = this.createIPANode(IPAType.AOO, ipaAOO.des_aoo, ipaAOO.cod_aoo);
            ipaNode.get_attributes().setAttribute(SelContattiIPA.IPA_DATA_ATTRIBUTE, ipaAOO);
            this.appendNode(ipaNode, aooFolderNode);
        }

        this._folderLoadingDeferreds.forEach((promise: JQueryDeferred<void>) => promise.resolve());
    }

    searchOUCallback(serialized: string, codiceAmministrazione: string): void {
        let ipaOUs: IPAOUModel[] = JSON.parse(serialized);
        let ipaAdministrationNode: Telerik.Web.UI.RadTreeNode = this._treeView.findNodeByValue(codiceAmministrazione);
        if (ipaAdministrationNode == null) {
            throw new Error(`TreeView node with value "${codiceAmministrazione}" has not found`);
        }

        let ouFolderNode: Telerik.Web.UI.RadTreeNode = ipaAdministrationNode.get_nodes().toArray().find(x => x.get_value() == IPAType.OU);
        ouFolderNode.get_nodes().clear();

        ipaOUs = ipaOUs.sort((a, b) => a.des_ou < b.des_ou ? -1 : 1);
        let ipaNode: Telerik.Web.UI.RadTreeNode;
        for (let ipaOU of ipaOUs) {
            ipaNode = this.createIPANode(IPAType.OU, ipaOU.des_ou, ipaOU.cod_uni_ou);
            ipaNode.get_attributes().setAttribute(SelContattiIPA.IPA_DATA_ATTRIBUTE, ipaOU);
            this.appendNode(ipaNode, ouFolderNode);
        }

        this._folderLoadingDeferreds.forEach((promise: JQueryDeferred<void>) => promise.resolve());
    }

    confirmCallback(serialized: string, toClose: boolean) {
        let contactIpa: any = {};
        contactIpa.Action = "Ins";
        contactIpa.Contact = serialized;
        if (toClose) {
            this.getRadWindow().close(contactIpa);
        } else {
            this.getRadWindow().BrowserWindow[`${this.callerId}_UpdateManual`](serialized, "Ins");
        }
    }

    searchAOOs(codiceAmministrazione: string): void {
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(codiceAmministrazione);
        ajaxModel.ActionName = "SearchAOOs";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    searchOUs(codiceAmministrazione: string): void {
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(codiceAmministrazione);
        ajaxModel.ActionName = "SearchOUs";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    showAdministrationDetailsCallback(serialized: string): void {
        this.showDetails(serialized, IPAType.Administration);
    }

    private showDetails(serialized: string, ipaType: IPAType): void {
        let ipaEntity: IPABase = JSON.parse(serialized);

        this.writeDetailsModel(ipaEntity, ipaType);

        this._loadingDeferreds.forEach((promise: JQueryDeferred<void>) => promise.resolve());

        this.openWindow(this.windowDetailsId);               
    }

    private createIPANode(ipaType: IPAType, description: string, value: string): Telerik.Web.UI.RadTreeNode {
        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(description);
        node.set_value(value);

        node.set_imageUrl(ImageHelper.getContactTypeImageUrl(IPAType[ipaType]));
        node.get_attributes().setAttribute(SelContattiIPA.NODE_TYPE_ATTRIBUTE, SelContattiIPA.IPA_NODE_TYPE);
        node.get_attributes().setAttribute(SelContattiIPA.IPA_CONTACT_TYPE_ATTRIBUTE, ipaType);

        if (ipaType == IPAType.Administration) {
            let aooFolderNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            aooFolderNode.set_text(IPAType[IPAType.AOO]);
            aooFolderNode.set_value(IPAType.AOO);
            aooFolderNode.set_imageUrl(ImageHelper.getFolderImageUrl());
            aooFolderNode.get_attributes().setAttribute(SelContattiIPA.FOLDER_CORRELATION_ATTRIBUTE, value);
            aooFolderNode.get_attributes().setAttribute(SelContattiIPA.NODE_TYPE_ATTRIBUTE, SelContattiIPA.FOLDER_NODE_TYPE);
            aooFolderNode.get_nodes().add(this.createEmptyNode());
            node.get_nodes().add(aooFolderNode);

            let ouFolderNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            ouFolderNode.set_text(IPAType[IPAType.OU]);
            ouFolderNode.set_value(IPAType.OU);
            ouFolderNode.set_imageUrl(ImageHelper.getFolderImageUrl());
            ouFolderNode.get_attributes().setAttribute(SelContattiIPA.FOLDER_CORRELATION_ATTRIBUTE, value);
            ouFolderNode.get_attributes().setAttribute(SelContattiIPA.NODE_TYPE_ATTRIBUTE, SelContattiIPA.FOLDER_NODE_TYPE);
            ouFolderNode.get_nodes().add(this.createEmptyNode());
            node.get_nodes().add(ouFolderNode);
        }

        return node;
    }

    private appendNode(node: Telerik.Web.UI.RadTreeNode): void
    private appendNode(node: Telerik.Web.UI.RadTreeNode, parentNode: Telerik.Web.UI.RadTreeNode): void
    private appendNode(node: Telerik.Web.UI.RadTreeNode, parentNode?: Telerik.Web.UI.RadTreeNode): void {
        if (parentNode != null) {
            parentNode.get_nodes().add(node);
            return;
        }

        this._treeView.get_nodes().add(node);
    }

    private createEmptyNode(): Telerik.Web.UI.RadTreeNode {
        return new Telerik.Web.UI.RadTreeNode();
    }

    private initializeNodeLoadingBehaviour(node: Telerik.Web.UI.RadTreeNode): void {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        promise.done(() => node.hideLoadingStatus());

        node.get_nodes().clear();
        node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        this._folderLoadingDeferreds.push(promise);
    }

    private writeDetailsModel(ipaModel: IPABase, ipaType: IPAType): void {
        let description: string = null;
        let telephoneNamber: string = null;
        let website: string = null;
        let responsible: string = `${ipaModel.cogn_resp ?? ""} ${ipaModel.nome_resp ?? ""}`;
        let administrationType: string = null;
        switch (ipaType) {
            case IPAType.Administration: {
                let ipaAdministration: IPAAdministrationModel = ipaModel as IPAAdministrationModel;
                description = ipaAdministration.des_amm;
                website = ipaAdministration.sito_istituzionale;
                responsible = `${ipaAdministration.titolo_resp} ${responsible}`;
                administrationType = ipaAdministration.tipologia;
                break;
            }
            case IPAType.AOO: {
                let ipaAOO: IPAAOOModel = ipaModel as IPAAOOModel;
                description = ipaAOO.des_aoo;
                telephoneNamber = ipaAOO.tel;
                break;
            }
            case IPAType.OU: {
                let ipaOU: IPAOUModel = ipaModel as IPAOUModel;
                description = ipaOU.des_ou;
                telephoneNamber = ipaOU.tel;
                break;
            }
        }

        $("#lblDescrizione").html(description);
        $("#lblIndirizzo").html(ipaModel.indirizzo);
        $("#lblCAP").html(ipaModel.cap);
        $("#lblProvincia").html(ipaModel.provincia);
        $("#lblRegione").html(ipaModel.regione);
        $("#lblTelefono").html(telephoneNamber);
        $("#lblEmail").html(ipaModel.mail1);
        $("#lblWebSite").html(website);
        $("#lblResponsabile").html(responsible);
        $("#lblCodAmm").html(ipaModel.cod_amm);
        $("#lblTypeAmm").html(administrationType);
    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    private openWindow(windowName: string): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.windowManagerDetailId);
        let wnd: Telerik.Web.UI.RadWindow = manager.getWindowById(windowName);
        wnd.show();
        wnd.set_modal(true);
        wnd.center();
        return false;
    }
}

export = SelContattiIPA;