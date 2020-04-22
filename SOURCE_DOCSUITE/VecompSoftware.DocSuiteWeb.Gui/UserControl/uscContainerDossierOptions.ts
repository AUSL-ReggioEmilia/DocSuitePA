/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import DossierFolderInsertViewModel = require('App/ViewModels/Dossiers/DossierFolderInsertViewModel');
import DossierInsertViewModel = require('App/ViewModels/Dossiers/DossierInsertViewModel');
import UscDossierFolders = require('UserControl/uscDossierFolders');
import DossierSummaryFolderViewModelMapper = require('App/Mappers/Dossiers/DossierFolderSummaryModelMapper');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierModel = require('App/Models/Dossiers/DossierModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import Guid = require('App/Helpers/GuidHelper');
import ContainerPropertyService = require('App/Services/Commons/ContainerPropertyService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ContainerPropertyModel = require('App/Models/Commons/ContainerPropertyModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContainerPropertyType = require('App/Models/Commons/ContainerPropertyType');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');

class UscContainerDossierOptions {
    uscDossierFoldersId: string;
    uscNotificationId: string;
    rtvMetadataId: string;
    uscMetadataSelId: string;
    chkMetadataReadonlyId: string;
    tlbMetadataId: string;
    splPageContentId: string;

    static DOSSIERFOLDERS_SESSIONNAME: string = "dossierfoldersessionname";

    private _rtvMetadata: Telerik.Web.UI.RadTreeView;
    private _serviceConfigurations: ServiceConfiguration[];
    private _containerPropertyService: ContainerPropertyService;
    private _currentContainerProperty: ContainerPropertyModel;
    private _currentContainerId: number;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _tlbMetadata: Telerik.Web.UI.RadToolBar;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */

    chkMetadataReadonly_onCheckedChanged = () => {
        let isChecked: boolean = $("#".concat(this.chkMetadataReadonlyId)).is(":checked");
        if (isChecked) {

        }
    }

    /**
     * Evento scatenato alla selezione di un metdata repository
     */
    private uscMetadataSel_onSelectedIndexChanged = (args: any, data: any) => {
        if (data) {
            let uscRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataSelId)).data();
            $.when(uscRepositorySel.getSelectedMetadata())
                .done((repository) => {
                    try {
                        this._rtvMetadata = $find(this.rtvMetadataId) as Telerik.Web.UI.RadTreeView;
                        let metadataRepository: MetadataRepositoryModel = repository as MetadataRepositoryModel;
                        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                        node.set_value(data);
                        node.set_text(metadataRepository.Name);
                        node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/EnumDesigner_16x_green.png")
                        let isChecked: boolean = $("#".concat(this.chkMetadataReadonlyId)).is(":checked");
                        if (isChecked) {
                            this._rtvMetadata.get_nodes().getNode(0).get_nodes().clear();
                        }
                        this._rtvMetadata.get_nodes().getNode(0).get_nodes().add(node);
                        $("#".concat(this.chkMetadataReadonlyId)).prop("disabled", false);
                        if (this._rtvMetadata.get_nodes().getNode(0).get_nodes().get_count() > 1) {
                            $("#".concat(this.chkMetadataReadonlyId)).prop("disabled", true);
                        }
                    } catch (e) {
                        console.error(e);
                        this.showNotificationException("Errore nella gestione del metadata selezionato");
                    }
                })
                .fail(() => {
                    this.showNotificationException("Errore nel caricamento del metadata selezionato");
                })
        }
    }

    /**
     * Evento scatenato al click di un pulsante della toolbar relativa ai metadata repository selezionati
     */
    tlbMetadata_OnButtonClicked = (source: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        this._rtvMetadata = $find(this.rtvMetadataId) as Telerik.Web.UI.RadTreeView;
        let item: Telerik.Web.UI.RadToolBarItem = args.get_item();
        if (item) {
            let command: string = item.get_value();
            switch (command) {
                case "removeMetadata": {
                    try {
                        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvMetadata.get_selectedNode();
                        if (selectedNode && selectedNode.get_value()) {
                            this._rtvMetadata.get_nodes().getNode(0).get_nodes().remove(selectedNode);
                            $("#".concat(this.chkMetadataReadonlyId)).prop("disabled", false);
                            if (this._rtvMetadata.get_nodes().getNode(0).get_nodes().get_count() > 1) {
                                $("#".concat(this.chkMetadataReadonlyId)).prop("disabled", true);
                            }
                        }
                    } catch (e) {
                        console.error(e);
                        this.showNotificationException("Errore nella rimozione del metadata selezionato");
                    }
                    break;
                }
            }
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */

    initialize(): void {
        this._rtvMetadata = $find(this.rtvMetadataId) as Telerik.Web.UI.RadTreeView;
        this._tlbMetadata = $find(this.tlbMetadataId) as Telerik.Web.UI.RadToolBar;

        let containerPropertyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ContainerProperty");
        this._containerPropertyService = new ContainerPropertyService(containerPropertyConfiguration);

        let metadataRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
        this._metadataRepositoryService = new MetadataRepositoryService(metadataRepositoryConfiguration);

        this.bindLoaded();
    }

    private bindLoaded(): void {
        $("#".concat(this.splPageContentId)).data(this);
    }

    /**
     * Carica l'alberatura di un dossier per uno specifico contenitore
     * @param idContainer
     */
    loadFolders(idContainer: number) {
        $("#".concat(this.uscMetadataSelId)).on(UscMetadataRepositorySel.SELECTED_INDEX_EVENT, this.uscMetadataSel_onSelectedIndexChanged);
        $("#".concat(this.chkMetadataReadonlyId)).on("click", this.chkMetadataReadonly_onCheckedChanged);
        this._tlbMetadata = $find(this.tlbMetadataId) as Telerik.Web.UI.RadToolBar;
        this._tlbMetadata.add_buttonClicked(this.tlbMetadata_OnButtonClicked);
        let uscRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataSelId)).data();
        this._rtvMetadata = $find(this.rtvMetadataId) as Telerik.Web.UI.RadTreeView;
        if (!jQuery.isEmptyObject(uscRepositorySel)) {
            uscRepositorySel.clearComboboxText();
            this._rtvMetadata.get_nodes().getNode(0).get_nodes().clear();
        }

        sessionStorage[UscContainerDossierOptions.DOSSIERFOLDERS_SESSIONNAME] = [];
        this._currentContainerProperty = undefined;
        this._currentContainerId = idContainer;
        $("#".concat(this.chkMetadataReadonlyId)).prop('checked', false);
        let uscDossierFolders: UscDossierFolders = <UscDossierFolders>$("#".concat(this.uscDossierFoldersId)).data();
        if (!jQuery.isEmptyObject(uscDossierFolders)) {
            let tmpDossierId: string = Guid.newGuid();
            uscDossierFolders.setRootNode('', tmpDossierId);
            uscDossierFolders.loadNodes([]);
            uscDossierFolders.showLoadingPanel();
            this._containerPropertyService.getByContainer(idContainer, "DossierFoldersModel",
                (data: any) => {
                    if (!data || data.length == 0) {
                        uscDossierFolders.hideLoadingPanel();
                        return;
                    }

                    try {
                        let containerProperty: ContainerPropertyModel = data[0] as ContainerPropertyModel;
                        this._currentContainerProperty = containerProperty;
                        if (containerProperty.ValueString) {
                            let insertModel: DossierInsertViewModel = JSON.parse(containerProperty.ValueString) as DossierInsertViewModel;
                            let folders: DossierFolderModel[] = [];
                            this.loadFolderModelRecursive(insertModel.Folders, folders, tmpDossierId, tmpDossierId);
                            this.loadMetadata(insertModel.MetadataRestrictions);
                            this._rtvMetadata.get_nodes().getNode(0).expand();
                            $("#".concat(this.chkMetadataReadonlyId)).prop('checked', insertModel.SetMetadataReadOnly);
                            sessionStorage[UscContainerDossierOptions.DOSSIERFOLDERS_SESSIONNAME] = JSON.stringify(folders);

                            let rootFolders: DossierFolderModel[] = folders.filter(x => {
                                for (let f of folders) {
                                    if (f.UniqueId == x.ParentInsertId) {
                                        return false;
                                    }
                                }
                                return true;
                            });

                            let instance = new DossierSummaryFolderViewModelMapper();
                            let toLoadFolders: DossierSummaryFolderViewModel[] = [];
                            for (let folder of rootFolders) {
                                toLoadFolders.push(instance.Map(folder));
                            }
                            uscDossierFolders.loadNodes(toLoadFolders);
                        }
                        uscDossierFolders.hideLoadingPanel();
                    } catch (e) {
                        uscDossierFolders.hideLoadingPanel();
                        console.error(e);
                        this.showNotificationException("Errore nella lettura dei valori per il contenitore selezionato");
                    }
                },
                (exception: ExceptionDTO) => {
                    uscDossierFolders.hideLoadingPanel();
                    this.showNotificationException(exception);
                });
        }
    }

    /**
     * Salva le informazioni del dossier per il contenitore selezionato
     */
    saveDossierFolders = (): boolean => {
        let instance = new DossierSummaryFolderViewModelMapper();
        let folders: DossierFolderModel[] = this.getStorageFolders();
        let rootFolders: DossierFolderModel[] = folders.filter(x => {
            for (let f of folders) {
                if (f.UniqueId == x.ParentInsertId) {
                    return false;
                }
            }
            return true;
        });

        let uscDossierFolders: UscDossierFolders = <UscDossierFolders>$("#".concat(this.uscDossierFoldersId)).data();
        try {
            let action: any = (this._currentContainerProperty) ? (m, c, e) => this._containerPropertyService.updateContainerProperty(m, c, e) : (m, c, e) => this._containerPropertyService.insertContainerProperty(m, c, e);
            uscDossierFolders.showLoadingPanel();
            let saveModel: DossierInsertViewModel = new DossierInsertViewModel();
            let toSaveFolders: DossierFolderInsertViewModel[] = [];
            this.fillFoldersRecursive(rootFolders, folders, toSaveFolders);
            saveModel.Folders = toSaveFolders;
            if (!this._currentContainerProperty) {
                this._currentContainerProperty = {} as ContainerPropertyModel;
                this._currentContainerProperty.Container = {} as ContainerModel;
                this._currentContainerProperty.Container.EntityShortId = this._currentContainerId;
                this._currentContainerProperty.Name = "DossierFoldersModel";
                this._currentContainerProperty.ContainerType = ContainerPropertyType.Json;
            }

            this._rtvMetadata = $find(this.rtvMetadataId) as Telerik.Web.UI.RadTreeView;
            let nodes: Telerik.Web.UI.RadTreeNode[] = this._rtvMetadata.get_nodes().getNode(0).get_nodes().toArray();
            for (let node of nodes) {
                saveModel.MetadataRestrictions.push(node.get_value());
            }
            saveModel.SetMetadataReadOnly = $("#".concat(this.chkMetadataReadonlyId)).is(":checked");
            this._currentContainerProperty.ValueString = JSON.stringify(saveModel);

            action(this._currentContainerProperty,
                (data: any) => {
                    uscDossierFolders.hideLoadingPanel();
                },
                (exception: ExceptionDTO) => {
                    uscDossierFolders.hideLoadingPanel();
                    this.showNotificationException(exception);
                });
        } catch (e) {
            uscDossierFolders.hideLoadingPanel();
            console.error(e);
            this.showNotificationException("Errore nel salvataggio del modello di dossier per il contenitore selezionato.");
        }
        return false;
    }

    private loadMetadata(metadatas: string[]): void {
        if (!metadatas || metadatas.length == 0) {
            return;
        }

        let metadata: string = metadatas.shift();
        this._metadataRepositoryService.getFullModelById(metadata.toString(),
            (data: any) => {
                if (!data) {
                    return;
                }

                let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                node.set_text(data.Name);
                node.set_value(metadata);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/EnumDesigner_16x_green.png")
                this._rtvMetadata = $find(this.rtvMetadataId) as Telerik.Web.UI.RadTreeView;
                this._rtvMetadata.get_nodes().getNode(0).get_nodes().add(node);
                $("#".concat(this.chkMetadataReadonlyId)).prop("disabled", false);
                if (this._rtvMetadata.get_nodes().getNode(0).get_nodes().get_count() > 1) {
                    $("#".concat(this.chkMetadataReadonlyId)).prop("disabled", true);
                }
                this.loadMetadata(metadatas);
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(exception);
            });
    }

    private getStorageFolders(): DossierFolderModel[] {
        let jsFolders: string = sessionStorage[UscContainerDossierOptions.DOSSIERFOLDERS_SESSIONNAME];
        if (!jsFolders) {
            return [];
        }
        return JSON.parse(jsFolders);
    }

    private loadFolderModelRecursive(models: DossierFolderInsertViewModel[], folders: DossierFolderModel[], dossierId: string, parentId: string): void {
        if (!models || models.length == 0) {
            return;
        }

        for (let model of models) {
            let folder: DossierFolderModel = {} as DossierFolderModel;
            folder.Dossier = {} as DossierModel;
            folder.Dossier.UniqueId = dossierId;
            folder.Name = model.Name;
            folder.ParentInsertId = parentId;
            folder.Status = DossierFolderStatus.InProgress;
            folder.UniqueId = Guid.newGuid();

            if (model.Roles && model.Roles.length > 0) {
                folder.DossierFolderRoles = [];
                for (let role of model.Roles) {
                    let dossierRole: DossierFolderRoleModel = {} as DossierFolderRoleModel;
                    dossierRole.Role = {} as RoleModel;
                    dossierRole.Role.EntityShortId = role;
                    folder.DossierFolderRoles.push(dossierRole);
                }
            }

            if (model.Fascicle) {
                model.Fascicle.UniqueId = Guid.newGuid();
                folder.Fascicle = model.Fascicle;
                folder.Status = DossierFolderStatus.Fascicle;
            }

            if (model.Children && model.Children.length > 0) {
                folder.Status = DossierFolderStatus.Folder;
                this.loadFolderModelRecursive(model.Children, folders, dossierId, folder.UniqueId);
            }
            folders.push(folder);
        }
    }

    private fillFoldersRecursive(toFillFolders: DossierFolderModel[], folders: Readonly<DossierFolderModel[]>, models: DossierFolderInsertViewModel[]): void {
        if (!folders || folders.length == 0) {
            return;
        }

        for (let folder of toFillFolders) {
            let model: DossierFolderInsertViewModel = this.createDossierInsertModel(folder);
            let folderChildren: DossierFolderModel[] = folders.filter(x => x.ParentInsertId == folder.UniqueId);
            let hasChildren: boolean = folderChildren && folderChildren.length > 0;
            if (hasChildren) {
                model.Children = [];
                this.fillFoldersRecursive(folderChildren, folders, model.Children);
            }
            models.push(model);
        }
    }

    private createDossierInsertModel(folder: DossierFolderModel): DossierFolderInsertViewModel {
        let model: DossierFolderInsertViewModel = new DossierFolderInsertViewModel();
        model.Name = folder.Name;
        model.Roles = (folder.DossierFolderRoles) ? folder.DossierFolderRoles.map<number>(x => x.Role.EntityShortId) : [];
        model.Fascicle = folder.Fascicle;
        return model;
    }

    private showNotificationException(exception: ExceptionDTO)
    private showNotificationException(message: string)
    private showNotificationException(exception: any) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(exception);
            }
        }
    }
}

export = UscContainerDossierOptions;