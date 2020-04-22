import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import InsertActionType = require("App/Models/InsertActionType");
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

interface IDossierFolderService {
    getChildren(uniqueId: string, status: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    insertDossierFolder(dossierFolder: DossierFolderModel, insertAction?: InsertActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    deleteDossierFolder(model: DossierFolderModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    updateDossierFolder(dossierFolder: DossierFolderModel, updateAction?: UpdateActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    getDossierFolder(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    getFullDossierFolder(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
}

export = IDossierFolderService;