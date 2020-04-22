import DossierSummaryFolderViewModelMapper = require('App/Mappers/Dossiers/DossierFolderSummaryModelMapper');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');
import UpdateActionType = require("App/Models/UpdateActionType");
import InsertActionType = require("App/Models/InsertActionType");
import IDossierFolderService = require('App/Services/Dossiers/IDossierFolderService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import Guid = require('App/Helpers/GuidHelper');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import RoleModel = require('App/Models/Commons/RoleModel');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');

class DossierFolderLocalService implements IDossierFolderService {
    static DOSSIERFOLDERS_SESSIONNAME: string = "dossierfoldersessionname";
    /**
    * Costruttore 
    */
    constructor() {
        
    }

    private getStorageFolders(): DossierFolderModel[] {
        let jsFolders: string = sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME];
        if (!jsFolders) {
            return [];
        }
        return JSON.parse(jsFolders);
    }

    getChildren(uniqueId: string, status: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        try {
            let folders: DossierFolderModel[] = this.getStorageFolders();
            if (!folders) {
                callback(undefined);
                return;
            }

            if (uniqueId) {
                folders = folders.filter(f => f.ParentInsertId == uniqueId);
                if (!folders) {
                    callback(undefined);
                    return;
                }
            }

            let instance = new DossierSummaryFolderViewModelMapper();
            let models: DossierSummaryFolderViewModel[] = [];
            for (let folder of folders) {
                models.push(instance.Map(folder));
            }
            callback(models);
        } catch (e) {
            let exception: ExceptionDTO = new ExceptionDTO();
            exception.statusText = e.stack;
            error(exception);
        }
    }

    /**
    * Inserisco una nuova DossierFolder
    */
    insertDossierFolder(dossierFolder: DossierFolderModel, insertAction?: InsertActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        try {
            let folders: DossierFolderModel[] = this.getStorageFolders();
            if (!folders) {
                folders = [];
            }            

            dossierFolder.UniqueId = Guid.newGuid();

            if (dossierFolder.Fascicle) {
                dossierFolder.Name = dossierFolder.Fascicle.FascicleObject;
            }

            let existSameName: boolean = folders.filter(x => x.ParentInsertId == dossierFolder.ParentInsertId && x.Name.trim() == dossierFolder.Name.trim()).length > 0;
            if (existSameName) {
                let error: Error = new Error();
                error.stack = "Il nome delle cartelle di un Dossier deve essere univoco per cartelle dello stesso livello.";
                throw error;
            }

            folders.push(dossierFolder);
            let parentFolders: DossierFolderModel[] = folders.filter(f => f.UniqueId == dossierFolder.ParentInsertId);
            if (dossierFolder.ParentInsertId && parentFolders.length > 0) {
                let parentFolder: DossierFolderModel = parentFolders[0];
                parentFolder.Status = DossierFolderStatus.Folder;
            } 
            sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME] = JSON.stringify(folders);                       
            callback(dossierFolder);
        } catch (e) {
            let exception: ExceptionDTO = new ExceptionDTO();
            exception.statusText = e.stack;
            error(exception);
        }
    }

    /**
    * Cancellazione di una DossierFolder esistente
    * @param model
    * @param callback
    * @param error
    */
    deleteDossierFolder(model: DossierFolderModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        try {
            let folders: DossierFolderModel[] = this.getStorageFolders();
            if (!folders) {
                callback(undefined);
                return;
            }

            let filteredFolders: DossierFolderModel[] = folders.filter(f => f.UniqueId != model.UniqueId);
            if (model.ParentInsertId) {
                let existChildren: boolean = filteredFolders.filter(f => f.ParentInsertId == model.ParentInsertId).length > 0;
                if (!existChildren) {
                    let parentFolders: DossierFolderModel[] = folders.filter(f => f.UniqueId == model.ParentInsertId);
                    if (parentFolders.length > 0) {
                        let parentFolder: DossierFolderModel = parentFolders[0];
                        parentFolder.Status = DossierFolderStatus.InProgress;
                    }
                }
            }
            sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME] = JSON.stringify(filteredFolders);
            callback(undefined);
        } catch (e) {
            let exception: ExceptionDTO = new ExceptionDTO();
            exception.statusText = e.stack;
            error(exception);
        }
    }

    /**
    * Aggiorno una DossierFolder
    */
    updateDossierFolder(dossierFolder: DossierFolderModel, updateAction?: UpdateActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        try {
            let folders: DossierFolderModel[] = this.getStorageFolders();
            if (!folders) {
                callback(undefined);
                return;
            }

            let filteredFolders: DossierFolderModel[] = folders.filter(f => f.UniqueId == dossierFolder.UniqueId);
            if (!filteredFolders) {
                callback(undefined);
                return;
            }

            let existSameName: boolean = folders.filter(x => x.ParentInsertId == filteredFolders[0].ParentInsertId && x.Name.trim() == dossierFolder.Name.trim() && x.UniqueId != dossierFolder.UniqueId).length > 0;
            if (existSameName) {
                let error: Error = new Error();
                error.stack = "Il nome delle cartelle di un Dossier deve essere univoco per cartelle dello stesso livello.";
                throw error;
            }

            if (updateAction == UpdateActionType.DossierFolderAuthorizationsPropagation) {
                this.propagateFolderRolesRecursive(filteredFolders[0].ParentInsertId, dossierFolder.DossierFolderRoles, folders);
            }

            let folder: DossierFolderModel = filteredFolders[0];
            folder.Name = dossierFolder.Name;
            folder.Status = dossierFolder.Status;
            folder.Fascicle = undefined;
            folder.Category = undefined;
            if (!updateAction || updateAction != UpdateActionType.RemoveFascicleFromDossierFolder) {
                folder.Fascicle = dossierFolder.Fascicle;
                folder.Category = dossierFolder.Category;
            }            
            folder.DossierFolderRoles = dossierFolder.DossierFolderRoles;
            sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME] = JSON.stringify(folders);
            callback(folder);
        } catch (e) {
            let exception: ExceptionDTO = new ExceptionDTO();
            exception.statusText = e.stack;
            error(exception);
        }
    }

    private propagateFolderRolesRecursive(parentId: string, dossierFolderRoles: DossierFolderRoleModel[], folders: DossierFolderModel[]): void {
        let childrenFolders: DossierFolderModel[] = folders.filter(f => f.ParentInsertId == parentId);
        if (!childrenFolders || childrenFolders.length == 0) {
            return;
        }

        for (let folder of childrenFolders) {
            folder.DossierFolderRoles = [];
            folder.DossierFolderRoles = dossierFolderRoles;
            let existChildren: boolean = folders.filter(f => f.ParentInsertId == folder.UniqueId).length > 0;
            if (existChildren) {
                this.propagateFolderRolesRecursive(folder.UniqueId, dossierFolderRoles, folders);
            }
        }
    }

    /*
    *
    */
    getDossierFolder(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        try {
            let folders: DossierFolderModel[] = this.getStorageFolders();
            if (!folders) {
                callback(undefined);
                return;
            }

            let filteredFolders: DossierFolderModel[] = folders.filter(f => f.UniqueId == uniqueId);
            if (!filteredFolders) {
                callback(undefined);
                return;
            }

            let folder: DossierFolderModel = filteredFolders[0];
            let instance = new DossierSummaryFolderViewModelMapper();
            callback(instance.Map(folder));
        } catch (e) {
            let exception: ExceptionDTO = new ExceptionDTO();
            exception.statusText = e.stack;
            error(exception);
        }        
    }

    getFullDossierFolder(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        try {
            let folders: DossierFolderModel[] = this.getStorageFolders();
            if (!folders) {
                callback(undefined);
                return;
            }

            let filteredFolders: DossierFolderModel[] = folders.filter(f => f.UniqueId == uniqueId);
            if (!filteredFolders) {
                callback(undefined);
                return;
            }

            let folder: DossierFolderModel = filteredFolders[0];
            callback(folder);
        } catch (e) {
            let exception: ExceptionDTO = new ExceptionDTO();
            exception.statusText = e.stack;
            error(exception);
        }
    }
}

export = DossierFolderLocalService;