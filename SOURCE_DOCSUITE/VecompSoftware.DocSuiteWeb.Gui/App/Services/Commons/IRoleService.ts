import ExceptionDTO = require('App/DTOs/ExceptionDTO');

interface IRoleService {
    getDossierFolderRole(dossierFolderId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
}
export = IRoleService;