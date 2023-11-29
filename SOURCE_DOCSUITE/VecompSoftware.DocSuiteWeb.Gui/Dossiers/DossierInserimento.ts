import DossierModel = require('App/Models/Dossiers/DossierModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import DossierBase = require('Dossiers/DossierBase');
import DossierService = require('App/Services/Dossiers/DossierService');
import ContainerService = require('App/Services/Commons/ContainerService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscDynamicMetadataRest = require('UserControl/uscDynamicMetadataRest');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
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
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import SetiContactModel = require('App/Models/Commons/SetiContactModel');
import uscRoleRest = require('UserControl/uscRoleRest');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import UscRoleRestEventType = require('App/Models/Commons/UscRoleRestEventType');
import uscContattiSelRest = require('UserControl/uscContattiSelRest');
import ContactModel = require('APP/Models/Commons/ContactModel');
import DossierRoleModel = require('App/Models/Dossiers/DossierRoleModel');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import uscCategoryRest = require('UserControl/uscCategoryRest');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import EnumHelper = require('App/Helpers/EnumHelper');
import DossierType = require('App/Models/Dossiers/DossierType');

declare var Page_IsValid: any;

class DossierInserimento extends DossierBase {

    dossierPageContentId: string;
    btnConfirmId: string;
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
    currentTenantId: string;
    uscSetiContactSelId: string;
    uscRoleRestId: string;
    uscContattiSelRestId: string;
    uscRoleMasterId: string;
    roleInsertId: number[];
    contactInsertId: number[] = [];
    dossierRolesList: DossierRoleModel[] = [];
    uscCategoryRestId: string;
    rcbDossierTypeId: string;
    rfvDossierTypeId: string;
    defaultCategoryId: number;
    _enumHelper: EnumHelper;
    dossierTypologyEnabled: boolean;
    rowDossierTypeKeyId: string;
    rowDossierTypeValueId: string;

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
    private _uscRoleRest: uscRoleRest;
    private _uscContattiSelRest: uscContattiSelRest;
    private selectedResponsibleRole: RoleModel;
    private _rcbDossierType: Telerik.Web.UI.RadComboBox;
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
    btnConfirm_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this.insertCallback();
    }

    rdlContainer_OnSelectedIndexChanged = (sender: Telerik.Web.UI.RadDropDownList, eventArgs: Telerik.Web.UI.DropDownListIndexChangedEventArgs) => {
        if (sender.get_selectedItem()) {
            let uscMetadataRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataRepositorySelId)).data();
            let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
            uscMetadataRepositorySel.setRepositoryRestrictions([]);
            uscMetadataRepositorySel.clearComboboxText();
            uscMetadataRepositorySel.enableSelection();
            uscDynamicMetadataRest.clearPage();
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
                                uscDynamicMetadataRest.loadMetadataRepository(repositoryId);
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
        this._enumHelper = new EnumHelper();
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

        this._uscRoleRest = <uscRoleRest>$(`#${this.uscRoleRestId}`).data();
        this._uscRoleRest.renderRolesTree([]);
        this.registerUscRoleRestEventHandlers();

        this._uscContattiSelRest = <uscContattiSelRest>$(`#${this.uscContattiSelRestId}`).data();
        this.registerUscContactRestEventHandlers();

        if (this.metadataRepositoryEnabled) {
            this._rowMetadataRepository.show();

            $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, (args, data) => {
                let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
                if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                    uscDynamicMetadataRest.clearPage();
                    this._selectedMetadataRepository = data;
                    if (data) {
                        uscDynamicMetadataRest.loadMetadataRepository(data);
                    }
                }
            });
            /*event for filing out the fields with the chosen Seti contact*/
            $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_SETI_CONTACT_EVENT, (sender, args: SetiContactModel) => {
                let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
                uscDynamicMetadataRest.populateMetadataRepository(args);
            });
        }

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRestId).done((instance) => {
            instance.setToolbarVisibility(true);
            instance.renderRolesTree([]);
        });
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId).done((instance) => {
            instance.setToolbarVisibility(true);
            instance.renderRolesTree([]);
        });
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRestId).done((instance) => instance.setToolbarVisibility(true));
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId).done((instance) => instance.setToolbarVisibility(true));

        this._rcbDossierType = <Telerik.Web.UI.RadComboBox>$find(this.rcbDossierTypeId);
        $(`#${this.rowDossierTypeKeyId}`).hide();
        $(`#${this.rowDossierTypeValueId}`).hide();
        if (this.dossierTypologyEnabled) {
            this.populateDossierTypeComboBox();
            $(`#${this.rowDossierTypeKeyId}`).show();
            $(`#${this.rowDossierTypeValueId}`).show();
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
        this._containerService.getDossierInsertAuthorizedContainers(this.currentTenantId,
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
    insertCallback(): void {
        let dossierModel: DossierModel = <DossierModel>{};

        if (!this._rdlContainer.get_selectedItem()) {
            this.showNotificationMessage(this.uscNotificationId, "La selezione del contenitore è obbligatoria");
            return;
        }

        //riferimento
        this.fillContacts(JSON.stringify(this.contactInsertId), dossierModel);
        //settore
        dossierModel.DossierRoles = this.dossierRolesList;
        this.fillModelFromPage(dossierModel);
        //metadati
        if (this.metadataRepositoryEnabled && this._selectedMetadataRepository && !this.fillMetadataModel(dossierModel)) {
            this.enableBtnConfirm();
            return;
        }
        //classificatore
        let uscCategoryRest: uscCategoryRest = <uscCategoryRest>$(`#${this.uscCategoryRestId}`).data();
        dossierModel.Category = uscCategoryRest.getSelectedCategory();
        if (!dossierModel.Category) {
            dossierModel.Category = <CategoryModel>{
                EntityShortId: this.defaultCategoryId
            };
        }
        this._loadingPanel.show(this.dossierPageContentId);
        this._btnConfirm.set_enabled(false);
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
                this._btnConfirm.set_enabled(true);
                this._loadingPanel.hide(this.dossierPageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private registerUscContactRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscContattiSelRest>(this.uscContattiSelRestId)
            .done((instance) => {
                instance.registerEventHandler(instance.uscContattiSelRestEvents.ContactDeleted, (contactIdToDelete: number) => {
                    this.contactInsertId = this.contactInsertId.filter(x => x != contactIdToDelete);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(instance.uscContattiSelRestEvents.NewContactsAdded, (newAddedContact: ContactModel) => {
                    this.contactInsertId.push(newAddedContact.EntityId);
                    return $.Deferred<void>().resolve();
                });
            });
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

        model.DossierType = this.dossierTypologyEnabled
            ? DossierType[this._rcbDossierType.get_selectedItem().get_value()]
            : DossierType[DossierType.Procedure];

        return model;
    }

    private fillMetadataModel(model: DossierModel): boolean {
        let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
        let metadataRepository = uscDynamicMetadataRest.getMetadataRepository();
        let setiIntegrationEnabledField: boolean;
        if (metadataRepository && metadataRepository.JsonMetadata) {
            let metadataJson: MetadataDesignerViewModel = JSON.parse(metadataRepository.JsonMetadata);
            setiIntegrationEnabledField = metadataJson.SETIFieldEnabled;
        }

        if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
            let metadatas: [string, string] = uscDynamicMetadataRest.bindModelFormPage(setiIntegrationEnabledField);
            if (!metadatas) {
                return false;
            }
            model.MetadataDesigner = metadatas[0];
            model.MetadataValues = metadatas[1];

            let currentRepository = <MetadataRepositoryModel>{};
            currentRepository.UniqueId = this._selectedMetadataRepository;
            model.MetadataRepository = currentRepository;

        }
        return true;
    }

    private deleteRoleFromModel(roleIdToDelete: number): void {
        if (!roleIdToDelete)
            return;

        let dossierRoles: DossierRoleModel[] = [];
        dossierRoles = this.dossierRolesList.filter(x => x.Role.IdRole !== roleIdToDelete && x.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
        this.dossierRolesList = dossierRoles;
    }

    private registerUscRoleRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRestId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    this.deleteRoleFromModel(roleId);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    this.addRoleToModel(this.uscRoleMasterId, newAddedRoles, (role) => {
                        existedRole = role;
                    });
                    return $.Deferred<RoleModel>().resolve(existedRole);
                });
            });

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    this.deleteRoleFromModel(roleId);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    this.addRoleToModel(this.uscRoleRestId, newAddedRoles, (role) => {
                        existedRole = role;
                    });
                    if (!existedRole) {
                        this.selectedResponsibleRole = newAddedRoles[0];
                    }
                    return $.Deferred<RoleModel>().resolve(existedRole, true);
                });
            });
    }

    private addRoleToModel(toCheckControlId: string, newAddedRoles: RoleModel[], existedRoleCallback?: (RoleModel) => void): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(toCheckControlId)
            .done((instance) => {
                let existedRole: RoleModel = instance.existsRole(newAddedRoles);
                if (existedRole) {
                    alert(`Non è possibile selezionare il settore ${existedRole.Name} in quanto già presente come settore ${toCheckControlId == this.uscRoleMasterId ? "responsabile" : "autorizzato"} del fascicolo`);
                    existedRoleCallback(existedRole);
                    newAddedRoles = newAddedRoles.filter(x => x.IdRole !== existedRole.IdRole);
                }
                let dossierRoles: DossierRoleModel[] = newAddedRoles.map(x => <DossierRoleModel>{
                    Role: x,
                    IsMaster: toCheckControlId != this.uscRoleMasterId,
                    AuthorizationRoleType: toCheckControlId != this.uscRoleMasterId
                        ? AuthorizationRoleType.Responsible
                        : AuthorizationRoleType.Accounted
                });
                for (let dossierRole of dossierRoles) {
                    this.dossierRolesList.push(dossierRole);
                }
            });
    }

    private populateDossierTypeComboBox(): void {
        for (let dossierType in DossierType) {
            if (typeof DossierType[dossierType] === 'string' && dossierType !== DossierType.Process.toString()) {
                let rcbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                rcbItem.set_text(this._enumHelper.getDossierTypeDescription(DossierType[dossierType]));
                rcbItem.set_value(DossierType[dossierType]);
                this._rcbDossierType.get_items().add(rcbItem);
            }
        }
        this._rcbDossierType.findItemByValue(DossierType[DossierType.Procedure]).select();
    }

    private enableBtnConfirm(): void {
        this._loadingPanel.hide(this.dossierPageContentId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.set_enabled(true);
    }
}

export = DossierInserimento;