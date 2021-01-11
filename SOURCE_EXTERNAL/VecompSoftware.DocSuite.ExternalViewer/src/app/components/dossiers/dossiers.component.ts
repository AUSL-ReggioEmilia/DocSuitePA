import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { LoadingSpinnerService } from '../../services/commons/loading-spinner.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ResponseModel } from '../../models/commons/response.model';
import { DossierModel } from '../../models/dossiers/dossier.model';
import { ErrorLogModel } from '../../models/commons/error-log.model';
import { AppConfigService } from '../../services/commons/app-config.service';
import { DossierFolderModel } from '../../models/dossiers/dossier-folder.model';
import { FascicleFolderModel } from '../../models/fascicles/fascicle-folder.model';
import { DocumentService } from '../../services/commons/document.service';
import { DocumentUnitModel } from '../../models/document-units/document-unit.model';
import { CommunicationService } from '../../services/commons/communication.service';
import { DocumentType } from '../../models/commons/document.type';
import { DocumentModel } from '../../models/commons/document.model';
import { ProtocolService } from '../../services/protocols/protocol.service';
import { ItemType } from '../../helpers/item-type.helper';
import { BFSTreeNode } from '../../helpers/tree-node.helper';
import { AdaptedBFSTree } from '../../helpers/adapted-bfs-tree.helper';
import { of } from 'rxjs';
import { DossierFolderService } from '../../services/dossiers/dossier-folder.service';
import { FascicleFolderService } from '../../services/fascicles/fascicle-folder.service';
import { DocumentUnitMapper } from '../../mappers/document-units/document-unit.mapper';

@Component({
    selector: 'app-dossiers',
    templateUrl: './dossiers.component.html',
    styleUrls: ['./dossiers.component.css']
})
export class DossiersComponent implements OnInit {

    //base tree
    public data: any[] = [];
    //bfs tree
    dossierTree: AdaptedBFSTree;
    dossier: DossierModel;
    //allDocumentUnits is used for view details
    allDocumentUnits: DocumentUnitModel[];
    //used for expand all nodes
    public expandedKeys: string[] = [];
    treeWidth: string;
    showDetails: string;
    baseUrl: string = this.router.url.replace("Sommario", "");
    mainDocumentLabel: string = 'Documento';
    attachmentsLabel: string = 'Allegati';
    annexedLabel: string = 'Annessi';

    constructor(private toastr: ToastrService, private spinnerService: LoadingSpinnerService, private route: ActivatedRoute,
        private documentService: DocumentService,
        private router: Router, private communcationService: CommunicationService, private protocolService: ProtocolService,
        private dossierFolderService: DossierFolderService, private fascicleFolderService: FascicleFolderService,
        private documentUnitMapper: DocumentUnitMapper) {
        this.spinnerService.stop();
    }

    onResize(event) {
        this.treeWidth = event;
    }

    ngOnInit() {
        this.allDocumentUnits = [];
        this.route.data.subscribe(response => this.successCallback(response['responseModel']), this.errorCallback);
    }

    successCallback(response: ResponseModel<DossierModel>) {
        this.dossier = response.results[0];
        this.populateTreeView(response.results[0]);
        this.router.navigate([`${this.baseUrl}/Sommario`]).then(() => {
            this.spinnerService.stop();
        });
    }

    errorCallback = (error): void => {
        //TODO: toast notification per eccezione critica
        this.toastr.clear();
        console.log(error);
        let errorMessage: any;
        if (error.constructor.name == 'ErrorLogModel') {
            let errorLog: ErrorLogModel = error as ErrorLogModel;
            errorMessage = errorLog.status.toString().concat(' - ', errorLog.errorMessages[0]);
        }
        else {
            errorMessage = error;
        }
        this.toastr.error(errorMessage, 'Errore!', { timeOut: AppConfigService.settings.toastLife });
    }

    protocolErrorCallback = (error): void => {
        console.log(error);
        this.showDetails = "none";
        this.communcationService.emitChange(this.dossier);
        this.router.navigate([`${this.baseUrl}/Sommario`]).then(() => {
            this.spinnerService.stop();
        });
        this.spinnerService.stop();
    }

    populateTreeView(result: DossierModel): void {
        this.populateDossier(result);
    }

    populateDossier(dossier: DossierModel): void {
        //build dossier tree
        this.dossierTree = new AdaptedBFSTree(`${dossier.title} - ${dossier.subject}`, dossier.uniqueId, [1], ItemType.Dossier);
        //expand dossier tree
        this.expandedKeys.push("0");
        //load second tree level
        let dossierNode: BFSTreeNode = new BFSTreeNode("", dossier.uniqueId, [1], -1);
        this.loadDossierFolders(dossierNode);
        //add dossier tree to base tree
        this.data.push(this.dossierTree);
    }

    loadDossierFolders(parent: BFSTreeNode): void {
        this.dossierFolderService.getNextDossierFolders(parent.value).subscribe(response => {
            this.populateDossierFolders(response.results, parent.path, parent);
            this.collapseChildren();
        }, this.errorCallback);
    }

    populateDossierFolders(dossierFolders: DossierFolderModel[], parentPath: number[], parent: BFSTreeNode): void {
        for (let dossierFolder of dossierFolders) {
            //get dossierFolder path
            let dossierFolderPath = parentPath.concat([dossierFolders.indexOf(dossierFolder) + 1]);
            //build dossier folder node
            let dossierFolderItemType: ItemType = dossierFolder.idFascicle != null
                ? ItemType.Fascicle : ItemType.DossierFolder;
            let dossierFolderNode: BFSTreeNode = new BFSTreeNode(dossierFolder.name, dossierFolder.id, dossierFolderPath, dossierFolderItemType, dossierFolder.idFascicle);
            //add dossier folder node to tree
            this.dossierTree.traverseBF(dossierFolderNode, parentPath);
            //verify if dossierFolder has children to allow expanding
            this.dossierFolderHasChildren(dossierFolder.id, dossierFolderPath, parent);
            //allow expanding for fascicle node
            if (dossierFolder.idFascicle !== null) {
                this.addEmptyNode(dossierFolderPath);
            }
        }
    }

    dossierFolderHasChildren(idDossierFolder: string, dossierFolderPath: number[], parent: BFSTreeNode): void {
        this.dossierFolderService.hasChildren(idDossierFolder).subscribe(response => {
            if (response) {
                this.addEmptyNode(dossierFolderPath);
            }
            parent.loadingNode = false;
            this.collapseChildren();
        }, this.errorCallback);
    }

    loadFascicleFolders(parent: BFSTreeNode, isFascicle: boolean = false): void {
        let id: string = isFascicle ? parent.fascicleId : parent.value;
        this.fascicleFolderService.getNextFascicleFolders(id).subscribe(response => {
            this.populateFascicleFolders(response.results, parent.path, parent);
            this.loadFascicleDocumentUnits(id, parent, response.results.length);
            this.collapseChildren();
        }, this.errorCallback);
    }

    populateFascicleFolders(fascicleFolders: FascicleFolderModel[], parentPath: number[], parent: BFSTreeNode): void {
        for (let fascicleFolder of fascicleFolders) {
            //get fascicleFolder path
            let fascicleFolderPath = parentPath.concat([fascicleFolders.indexOf(fascicleFolder) + 1]);
            //build fascicle folder node
            let fascicleFolderNode: BFSTreeNode = new BFSTreeNode(fascicleFolder.name, fascicleFolder.id, fascicleFolderPath, ItemType.FascicleFolder);
            //add fascicle folder node to tree
            this.dossierTree.traverseBF(fascicleFolderNode, parentPath);
            //verify if fascicleFolder has children to allow expanding
            if (fascicleFolder.hasChildren || fascicleFolder.hasDocuments) {
                this.addEmptyNode(fascicleFolderPath);
            }
        }
        parent.loadingNode = false;
    }

    loadFascicleDocumentUnits(idFascicleFolder: string, parent: BFSTreeNode, lastChildPosition: number): void {
        this.fascicleFolderService.getFascicleDocumentUnitFromFolder(idFascicleFolder).subscribe(response => {
            let documentUnits: DocumentUnitModel[] = [];
            for (let result of response) {
                documentUnits.push(this.documentUnitMapper.mapFromJson(result));
            }
            this.allDocumentUnits = this.allDocumentUnits.concat(documentUnits);
            this.populateDocumentUnits(documentUnits, parent.path, lastChildPosition, parent);
            this.loadFascicleDocuments(idFascicleFolder, parent, documentUnits.length);
            this.collapseChildren();
        }, this.errorCallback);
    }

    populateDocumentUnits(documentUnits: DocumentUnitModel[], parentPath: number[], lastChildPosition: number, parent: BFSTreeNode): void {
        for (let documentUnit of documentUnits) {
            //get documentUnit path
            let documentUnitPath = parentPath.concat([documentUnits.indexOf(documentUnit) + lastChildPosition]);
            //build document unit node
            let documentUnitNode: BFSTreeNode = new BFSTreeNode(`${documentUnit.title} - ${documentUnit.subject}`, documentUnit.id, documentUnitPath, ItemType.DocumentUnit);
            //add document unit node to tree
            this.dossierTree.traverseBF(documentUnitNode, parentPath);
            //allow expanding
            this.addEmptyNode(documentUnitPath);
        }
        parent.loadingNode = false;
    }

    loadFascicleDocuments(idFascicleFolder: string, parent: BFSTreeNode, lastChildPosition: number): void {
        this.fascicleFolderService.getFascicleDocumentsFromFolder(idFascicleFolder).subscribe(response => {
            let documentUnits: DocumentUnitModel[] = [];
            for (let result of response) {
                documentUnits.push(this.documentUnitMapper.mapFromJson(result));
            }
            if (documentUnits.length > 0) {
                //get miscellaneous folder path
                let miscellaneousFolderPath = parent.path.concat([lastChildPosition]);
                //build miscellaneous folder node
                let miscellaneousFolderNode: BFSTreeNode = new BFSTreeNode(this.getChainTypeText(DocumentType.Miscellanea), documentUnits[0].subject, miscellaneousFolderPath, ItemType.MiscellaneousFolder);
                //add miscellaneous fodler node to tree
                this.dossierTree.traverseBF(miscellaneousFolderNode, parent.path);
                //allow expanding
                this.addEmptyNode(miscellaneousFolderPath);
            }
        }, this.errorCallback);
    }

    loadDocumentsByArchiveChain(parent: BFSTreeNode): void {
        this.documentService.getDocumentsByArchiveChain(parent.value).subscribe(response => {
            this.populateDocuments(response.results, parent.path, this.mainDocumentLabel, parent);
        }, this.errorCallback);
    }

    loadDocuments(parent: BFSTreeNode, isDocumentUnitExpanded: boolean = true, chainTypeLabel: string = ""): void {
        this.documentService.getDocuments(parent.value).subscribe(response => {
            if (isDocumentUnitExpanded) {
                this.populateDocumentChainTypes(response.results, parent.path, parent.value, parent);
            }
            else {
                this.populateDocuments(response.results, parent.path, chainTypeLabel, parent);
            }
            this.collapseChildren();
        }, this.errorCallback);
    }

    populateDocumentChainTypes(documents: DocumentModel[], parentPath: number[], parentId: string, parent: BFSTreeNode) {
        //get distinct document chain types (group documents by chain types)
        let chainTypes: DocumentType[] = documents.map(x => x.documentType)
            .filter((value, index, self) => self.indexOf(value) === index).sort((x, y) => { return x - y; });
        for (let chainTypeIndex: number = 0; chainTypeIndex < chainTypes.length; chainTypeIndex++) {
            //get chainType path
            let chainTypePath: number[] = parentPath.concat([chainTypeIndex + 1]);
            //build chain type node
            let chainType: DocumentType = chainTypes[chainTypeIndex];
            let documentTypeNode: BFSTreeNode = new BFSTreeNode(this.getChainTypeText(chainType), parentId, chainTypePath, ItemType.ChainType);
            //add chain type node to tree
            this.dossierTree.traverseBF(documentTypeNode, parentPath);
            //allow expanding
            this.addEmptyNode(chainTypePath);
        }
        parent.loadingNode = false;
    }

    populateDocuments(documents: DocumentModel[], parentPath: number[], chainTypeLabel: string, parent: BFSTreeNode) {
        let chainType: DocumentType = this.getChainTypeValue(chainTypeLabel);
        //get documents groupped by chain type
        let documentsByChainType: DocumentModel[] = documents.filter(x => x.documentType === chainType);
        for (let documentIndex: number = 0; documentIndex < documentsByChainType.length; documentIndex++) {
            //link document path to chain type path
            let documentPath: number[] = parentPath.concat([documentIndex + 1]);
            //build document node
            let documentByChainType: DocumentModel = documentsByChainType[documentIndex];
            let documentNode: BFSTreeNode = new BFSTreeNode(documentByChainType.documentName, documentByChainType.id, documentPath, ItemType.Document);
            documentNode.imageUrl = documentByChainType.imageUrl;
            //add document node to tree
            this.dossierTree.traverseBF(documentNode, parentPath);
        }
        parent.loadingNode = false;
    }

    getChainTypeText(chainType: DocumentType): string {
        return chainType === DocumentType.Annexed
            ? this.annexedLabel
            : chainType === DocumentType.Attachment
                ? this.attachmentsLabel
                : chainType === DocumentType.Main
                    ? this.mainDocumentLabel
                    : DocumentType[chainType];
    }

    getChainTypeValue(chainTypeLabel: string): DocumentType {
        return chainTypeLabel === this.annexedLabel
            ? DocumentType.Annexed
            : chainTypeLabel === this.attachmentsLabel
                ? DocumentType.Attachment
                : chainTypeLabel === this.mainDocumentLabel
                    ? DocumentType.Main
                    : DocumentType.Miscellanea;
    }

    addEmptyNode(parentPath: number[]): void {
        let emptyNode: BFSTreeNode = new BFSTreeNode("", "", parentPath.concat([1]), -1);
        this.dossierTree.traverseBF(emptyNode, parentPath);
    }

    //set tree icons
    public iconClass(dataItem: BFSTreeNode): any {
        return {
            'k-i-folder-more': dataItem.itemType === ItemType.Dossier,
            'k-i-aggregate-fields': dataItem.itemType === ItemType.Fascicle,
            'k-i-paste-plain-text': dataItem.itemType === ItemType.DocumentUnit,
            'k-i-link-vertical': dataItem.itemType === ItemType.ChainType,
            'k-i-folder': dataItem.itemType === ItemType.DossierFolder || dataItem.itemType === ItemType.FascicleFolder || dataItem.itemType === ItemType.MiscellaneousFolder,
            'k-icon': true
        };
    }

    //tree search implementation
    public keys: string[] = [];

    public isExpanded = (dataItem: any, index: string) => {
        return this.keys.indexOf(index) > -1;
    }

    public handleCollapse(node) {
        this.keys = this.keys.filter(k => k !== node.index);
    }

    public handleExpand(node) {
        this.keys = this.keys.concat(node.index);
    }

    public children = (dataitem: any): any[] => <any>of(dataitem.items);
    public hasChildren = (dataitem: any): boolean => !!dataitem.items;

    public parsedData: any[] = this.data;
    public searchTerm = '';

    public parsedExpandedKeys: string[];
    public searchedItems: any[];

    public onkeyup(value: string): void {
        this.parsedExpandedKeys = [];
        this.searchedItems = [];
        this.parsedExpandedKeys.push("0");
        this.parsedData = this.search(this.data, value);
        this.expandParsedTree(this.parsedData);
        this.expandedKeys = this.parsedExpandedKeys;
    }

    public expandParsedTree(items: any[]) {
        for (let item of items) {
            item.searchedNodeFont = "";
            if (this.searchedItems.filter(x => x.text === item.text).length > 0) {
                item.searchedNodeFont = "bold";
            }
            if (item.items && item.items.length > 0) {
                for (let i = 0; i < item.items.length; i++) {
                    item.items[i].path = item.path.concat([i]);
                    item.items[i].path[0] = 0;
                    if (item.items[i].items.length > 0 && item.items[i].items[0].text !== "") {
                        this.parsedExpandedKeys.push(`${item.items[i].path.join('_')}`);
                    }
                }
                this.expandParsedTree(item.items);
            }
        }
    }

    public search(items: any[], term: string): any[] {
        return items.reduce((acc, item) => {
            if (this.contains(item.text, term)) {
                acc.push(item);
                this.searchedItems.push(item);
            } else if (item.items && item.items.length > 0) {
                const newItems = this.search(item.items, term);
                if (newItems.length > 0) {
                    let newItem: BFSTreeNode = new BFSTreeNode(item.text, item.value, item.path, item.itemType);
                    newItem.items = newItems;
                    acc.push(newItem);
                }
            }

            return acc;
        }, []);
    }

    public contains(text: string, term: string): boolean {
        return text.toLowerCase().indexOf(term.toLowerCase()) >= 0;
    }

    //load node children
    expand(event) {
        let expandedItem: BFSTreeNode = event.dataItem;
        expandedItem.items = [];
        expandedItem.loadingNode = true;
        switch (expandedItem.itemType) {
            case ItemType.Dossier: {
                this.loadDossierFolders(expandedItem);
                break;
            }
            case ItemType.DossierFolder: {
                this.loadDossierFolders(expandedItem);
                break;
            }
            case ItemType.Fascicle: {
                this.loadFascicleFolders(expandedItem, true);
                break;
            }
            case ItemType.FascicleFolder: {
                this.loadFascicleFolders(expandedItem);
                break;
            }
            case ItemType.DocumentUnit: {
                this.loadDocuments(expandedItem);
                break;
            }
            case ItemType.ChainType: {
                this.loadDocuments(expandedItem, false, expandedItem.text);
                break;
            }
            case ItemType.MiscellaneousFolder: {
                this.loadDocumentsByArchiveChain(expandedItem);
                break;
            }
        }
    }

    collapsedItemKey: string;

    collapse(event) {
        let collapsedItem: BFSTreeNode = event.dataItem;
        this.collapsedItemKey = collapsedItem.path.map(x => --x).join("_");
    }

    collapseChildren() {
        if (this.collapsedItemKey) {
            this.expandedKeys = this.expandedKeys.filter(x => !x.startsWith(`${this.collapsedItemKey}_`));
            this.collapsedItemKey = undefined;
        }
    }

    //load node details
    select(event) {
        let itemType: ItemType = event.item.dataItem.itemType;
        switch (itemType) {
            case ItemType.Dossier: {
                this.showDetails = "block";
                if (`${this.baseUrl}Sommario` !== this.router.url) {
                    this.communcationService.emitChange(this.dossier);
                    this.router.navigate([`${this.baseUrl}/Sommario`]).then(() => {
                        this.spinnerService.stop();
                    });
                }
                break;
            }
            case ItemType.Fascicle: {
                this.showDetails = "block";
                this.router.navigate([`${this.baseUrl}/Fascicolo/${event.item.dataItem.fascicleId}`]).then(() => {
                    this.spinnerService.stop();
                });
                break;
            }
            case ItemType.DocumentUnit: {
                this.showDetails = "block";
                let selectedDocumentUnit: DocumentUnitModel = this.allDocumentUnits.filter(x => x.id === event.item.dataItem.value)[0];
                this.protocolService.getProtocolSummary(selectedDocumentUnit.year, selectedDocumentUnit.number).subscribe((response) => {
                    this.router.navigate([`${this.baseUrl}/Protocollo/Anno/${response.results[0].year}/Numero/${response.results[0].number}`]).then(() => {
                        this.spinnerService.stop();
                    });
                }, this.protocolErrorCallback);
                break;
            }
            case ItemType.Document: {
                this.showDetails = "block";
                this.router.navigate([`${this.baseUrl}/Documento/${event.item.dataItem.value}`]).then(() => {
                    this.spinnerService.stop();
                });
                break;
            }
            default: {
                this.showDetails = "none";
                break;
            }
        }
    }
}