import DossierModel = require('App/Models/Dossiers/DossierModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import DossierRoleModel = require('App/Models/Dossiers/DossierRoleModel');
import DossierBase = require('Dossiers/DossierBase');
import WindowHelper = require('App/Helpers/WindowHelper');
import DossierService = require('App/Services/Dossiers/DossierService');
import ContainerService = require('App/Services/Commons/ContainerService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscDynamicMetadataClient = require('UserControl/uscDynamicMetadataClient');
import MetadataViewModel = require('App/ViewModels/Metadata/MetadataViewModel');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ContainerPropertyService = require('App/Services/Commons/ContainerPropertyService');
import ContainerPropertyModel = require('App/Models/Commons/ContainerPropertyModel');
import DossierInsertViewModel = require('App/ViewModels/Dossiers/DossierInsertViewModel');
import DossierFolderInsertViewModel = require('App/ViewModels/Dossiers/DossierFolderInsertViewModel');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import InsertActionType = require('App/Models/InsertActionType');
import BuildActionModel = require('App/Models/Commons/BuildActionModel');
import BuildActionType = require('App/Models/Commons/BuildActionType');
import Guid = require('App/Helpers/GuidHelper');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');

declare var Page_IsValid: any;

class DossierInserimento extends DossierBase {

    dossierPageContentId: string;
    btnConfirmId: string;
    btnConfirmUniqueId: string;
    txtObjectId: string;
    txtNoteId: string;
    rdpStartDateId: string;
    rfvStartDateId: string;
    rdlContainerId: string;
    rfvContainerId: string;
    ajaxLoadingPanelId: string;
    ajaxManagerId: string;
    radWindowManagerId: string;
    uscNotificationId: string;
    uscDynamicMetadataId: string;
    metadataRepositoryEnabled: boolean;
    uscMetadataRepositorySelId: string;
    rowMetadataId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _dossierService: DossierService;
    private _containerService: ContainerService;
    private _rdlContainer: Telerik.Web.UI.RadDropDownList;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _rdpStartDate: Telerik.Web.UI.RadDatePicker;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _rowMetadataRepository: JQuery;
    private _selectedMetadataRepository: string;
    private _containerPropertyService: ContainerPropertyService;
    private _selectedDossierFoldersModel: DossierInsertViewModel;
    private _dossierFolderService: DossierFolderService;

    /**
* Costruttore
* @param serviceConfiguration
*/
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }


    /**
*------------------------- Events -----------------------------
*/

    /**
   * Evento scatenato al click del pulsante ConfermaInserimento
   * @method
   * @param sender
   * @param eventArgs
   * @returns
   */
    btnConfirm_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        if (Page_IsValid) {
            this._loadingPanel.show(this.dossierPageContentId);
            this._btnConfirm.set_enabled(false);
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequestWithTarget(this.btnConfirmUniqueId, '');
        }
    }

    rdlContainer_OnSelectedIndexChanged = (sender: Telerik.Web.UI.RadDropDownList, eventArgs: Telerik.Web.UI.DropDownListIndexChangedEventArgs) => {
        if (sender.get_selectedItem()) {
            let uscMetadataRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataRepositorySelId)).data();
            let uscDynamicMetadataClient: UscDynamicMetadataClient = <UscDynamicMetadataClient>$("#".concat(this.uscDynamicMetadataId)).data();
            uscMetadataRepositorySel.setRepositoryRestrictions([]);
            uscMetadataRepositorySel.clearComboboxText();
            uscMetadataRepositorySel.enableSelection();
            uscDynamicMetadataClient.clearPage();
            this._selectedDossierFoldersModel = undefined;
            this._loadingPanel.show(this.dossierPageContentId);
            this._containerPropertyService.getByContainer(Number(sender.get_selectedItem().get_value()), "DossierFoldersModel",
                (data: any) => {
                    if (!data || data.length == 0) {
                        this._loadingPanel.hide(this.dossierPageContentId);
                        return;
                    }

                    try {
                        let containerProperty: ContainerPropertyModel = data[0] as ContainerPropertyModel;
                        if (containerProperty.ValueString) {
                            this._selectedDossierFoldersModel = JSON.parse(containerProperty.ValueString);
                            uscMetadataRepositorySel.setRepositoryRestrictions(this._selectedDossierFoldersModel.MetadataRestrictions);
                            if (this._selectedDossierFoldersModel.SetMetadataReadOnly && this._selectedDossierFoldersModel.MetadataRestrictions.length > 0) {
                                let repositoryId: string = this._selectedDossierFoldersModel.MetadataRestrictions[0];
                                uscMetadataRepositorySel.setComboboxText(repositoryId);
                                this._selectedMetadataRepository = repositoryId;
                                uscDynamicMetadataClient.loadMetadataRepository(repositoryId);
                                uscMetadataRepositorySel.disableSelection();
                            }
                        }
                        this._loadingPanel.hide(this.dossierPageContentId);
                    } catch (e) {
                        console.error(e);
                        this._loadingPanel.hide(this.dossierPageContentId);
                        this.showNotificationMessage(this.uscNotificationId, "Errore nella lettura degli automatismi associati al contenitore.");
                    }                    
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.dossierPageContentId);
                    this.showNotificationException(this.uscNotificationId, exception);
                });
        }
    }

    /**
   *------------------------- Methods -----------------------------
   */

    /**
    * Metodo di inizializzazione
    */
    initialize() {
        super.initialize();
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_OnClicked);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._rdpStartDate = <Telerik.Web.UI.RadDatePicker>$find(this.rdpStartDateId);
        this._rdlContainer = <Telerik.Web.UI.RadDropDownList>$find(this.rdlContainerId);
        this._rdlContainer.add_selectedIndexChanged(this.rdlContainer_OnSelectedIndexChanged);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.ajaxManagerId);
        this._rowMetadataRepository = $("#".concat(this.rowMetadataId));
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
        this._containerService = new ContainerService(containerConfiguration);

        let containerPropertyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ContainerProperty");
        this._containerPropertyService = new ContainerPropertyService(containerPropertyConfiguration);

        let dossierFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME);
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);

        this._rdpStartDate.set_selectedDate(this._rdpStartDate.get_focusedDate());
        this._btnConfirm.set_enabled(false);
        this._rowMetadataRepository.hide();
        this._loadingPanel.show(this.dossierPageContentId);
        this.checkInsertRight();

        if (this.metadataRepositoryEnabled) {
            this._rowMetadataRepository.show();

            $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_INDEX_EVENT, (args, data) => {
                let uscDynamicMetadataClient: UscDynamicMetadataClient = <UscDynamicMetadataClient>$("#".concat(this.uscDynamicMetadataId)).data();
                if (!jQuery.isEmptyObject(uscDynamicMetadataClient)) {
                    uscDynamicMetadataClient.clearPage();
                    this._selectedMetadataRepository = data;
                    if (data) {
                        uscDynamicMetadataClient.loadMetadataRepository(data);
                    }
                }
            });

        }
    }

    /**
      * Controllo Diritti di inserimento
      * @param 
      */
    checkInsertRight(): void {
        (<DossierService>this.service).hasInsertRight(
            (data: any) => {
                if (data == null) return;
                if (data) {
                    this.loadContainers();
                }
                else {
                    this._btnConfirm.set_enabled(false);
                    this._loadingPanel.hide(this.dossierPageContentId);
                    $("#".concat(this.dossierPageContentId)).hide();
                    this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione pagina.<br \> Utente non autorizzato all'inserimento di un nuovo Dossier.");
                }
            },
            (exception: ExceptionDTO) => {
                this._btnConfirm.set_enabled(false);
                this._loadingPanel.hide(this.dossierPageContentId);
                $("#".concat(this.dossierPageContentId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private loadContainers = () => {
        this._containerService.getDossierInsertAuthorizedContainers(
            (data: any) => {
                let containers: ContainerModel[] = <ContainerModel[]>data;
                this.addContainers(containers, this._rdlContainer);
                this._btnConfirm.set_enabled(true);
                this._loadingPanel.hide(this.dossierPageContentId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.dossierPageContentId);
                this._btnConfirm.set_enabled(false);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }


    /**
    * Callback da code-behind per l'inserimento di un Dossier
    * @param contact
    * @param category
    */
    insertCallback(contact: string, role: string): void {
        let dossierModel: DossierModel = <DossierModel>{};

        //riferimento
        this.fillContacts(contact, dossierModel);
        //settore
        this.fillRoles(role, dossierModel);
        this.fillModelFromPage(dossierModel);

        if (this.metadataRepositoryEnabled && this._selectedMetadataRepository && !this.fillMetadataModel(dossierModel)) {
            this._loadingPanel.hide(this.dossierPageContentId);
            this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
            this._btnConfirm.set_enabled(true);
            return;
        }

        (<DossierService>this.service).insertDossier(dossierModel,
            (data: any) => {
                if (data == null) return;
                if (this._selectedDossierFoldersModel) {
                    $.when(this.saveFolders(this._selectedDossierFoldersModel.Folders, data.UniqueId))
                        .done(() => window.location.href = "DossierVisualizza.aspx?Type=Dossier&IdDossier=".concat(data.UniqueId, "&DossierTitle=", data.Year.toString(), "/", ("000000000" + data.Number.toString()).slice(-7)))
                        .fail(() => {
                            this.showNotificationMessage(this.uscNotificationId, "Errore nella generazione automatica dell'alberatura del Dossier.");
                        });
                } else {
                    window.location.href = "DossierVisualizza.aspx?Type=Dossier&IdDossier=".concat(data.UniqueId, "&DossierTitle=", data.Year.toString(), "/", ("000000000" + data.Number.toString()).slice(-7));
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.dossierPageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private saveFolders(modelFolders: DossierFolderInsertViewModel[], dossierId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        let dossier: DossierModel = <DossierModel>{};
        dossier.UniqueId = dossierId;
        $.when(this.persistFoldersRecursive(modelFolders, dossier, dossierId))
            .done(() => {
                if (modelFolders.length > 0) {
                    $.when(this.saveFolders(modelFolders, dossierId))
                        .done(() => promise.resolve())
                        .fail(() => promise.reject());
                } else {
                    promise.resolve();
                }
            })
            .fail(() => promise.reject());
        return promise.promise();
    }

    private persistFoldersRecursive(modelFolders: DossierFolderInsertViewModel[], dossier: DossierModel, parentId?: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        if (!modelFolders) {
            return promise.resolve();
        }
        let folder: DossierFolderInsertViewModel = modelFolders.shift();
        let dossierFolder = <DossierFolderModel>{};
        dossierFolder.Status = DossierFolderStatus.InProgress;
        dossierFolder.Name = folder.Name;
        dossierFolder.ParentInsertId = parentId;
        dossierFolder.DossierFolderRoles = this.getFolderRoles(folder.Roles);
        dossierFolder.Dossier = dossier;
        if (folder.Fascicle) {
            let toInsertFascicle: FascicleModel = folder.Fascicle;
            toInsertFascicle.UniqueId = undefined;
            dossierFolder.Status = DossierFolderStatus.DoAction;
            let actionModel: BuildActionModel = {} as BuildActionModel;
            actionModel.BuildType = BuildActionType.Build;
            actionModel.Model = JSON.stringify(toInsertFascicle);
            dossierFolder.JsonMetadata = JSON.stringify([actionModel]);
        }

        this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.BuildDossierFolders,
            (data: any) => {
                if (folder.Children && folder.Children.length > 0) {
                    $.when(this.persistFoldersRecursive(folder.Children, dossier, data.UniqueId))
                        .done(() => {
                            if (folder.Children.length > 0) {
                                $.when(this.persistFoldersRecursive(folder.Children, dossier, data.UniqueId))
                                    .done(() => promise.resolve())
                                    .fail(() => promise.reject());
                            } else {
                                promise.resolve();
                            }
                        })
                        .fail(() => promise.reject());
                } else if (modelFolders.length > 0) {
                    $.when(this.persistFoldersRecursive(modelFolders, dossier, parentId))
                        .done(() => promise.resolve())
                        .fail(() => promise.reject());
                } else {
                    promise.resolve();
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.dossierPageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
        return promise.promise();
    }

    private getFolderRoles(roles: number[]): DossierFolderRoleModel[] {
        let dossierFolderRoles: DossierFolderRoleModel[] = [];
        if (!roles) {
            return dossierFolderRoles;
        }

        for (let idRole of roles) {
            let dossierFolderRole: DossierFolderRoleModel = <DossierFolderRoleModel>{};
            let role: RoleModel = <RoleModel>{};
            role.EntityShortId = idRole;
            dossierFolderRole.Role = role;
            dossierFolderRoles.push(dossierFolderRole);
        }
        return dossierFolderRoles;
    }

    /**
    * Esegue il fill dei controlli della pagina in  modello DossierModel in inserimento
    */
    private fillModelFromPage(model: DossierModel): DossierModel {
        let txtObject: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        model.Subject = txtObject.get_value();
        model.Note = this._txtNote.get_value();

        let selectedDate = new Date(this._rdpStartDate.get_selectedDate().getTime() - this._rdpStartDate.get_selectedDate().getTimezoneOffset() * 60000);
        model.StartDate = selectedDate;

        let containerModel: ContainerModel = <ContainerModel>{};
        containerModel.EntityShortId = Number(this._rdlContainer.get_selectedItem().get_value());
        model.Container = containerModel;

        return model;
    }

    private fillMetadataModel(model: DossierModel): boolean {

        let uscDynamicMetadataClient: UscDynamicMetadataClient = <UscDynamicMetadataClient>$("#".concat(this.uscDynamicMetadataId)).data();
        if (!jQuery.isEmptyObject(uscDynamicMetadataClient)) {
            model.JsonMetadata = uscDynamicMetadataClient.bindModelFormPage();
            if (!model.JsonMetadata) {
                return false;
            }

            let currentRepository = <MetadataRepositoryModel>{};
            currentRepository.UniqueId = this._selectedMetadataRepository;
            model.MetadataRepository = currentRepository;

        }
        return true;
    }

}

export = DossierInserimento;