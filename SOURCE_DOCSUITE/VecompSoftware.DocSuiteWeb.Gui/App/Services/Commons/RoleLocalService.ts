import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import IRoleService = require('App/Services/Commons/IRoleService');
import DossierFolderLocalService = require('App/Services/Dossiers/DossierFolderLocalService');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');


class RoleLocalService extends BaseService implements IRoleService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /*
    *
    */
    getDossierFolderRole(dossierFolderId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        try {
            let jsFolders: string = sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME];
            if (!jsFolders) {
                callback(undefined);
                return;
            }

            let folders: DossierFolderModel[] = JSON.parse(jsFolders);
            folders = folders.filter(f => f.UniqueId == dossierFolderId);
            if (!folders || folders.length == 0) {
                callback(undefined);
                return;
            }

            let folder: DossierFolderModel = folders[0];
            if (!folder.DossierFolderRoles || folder.DossierFolderRoles.length == 0) {
                callback(undefined);
                return;
            }

            let roles: number[] = [];
            for (let folderRole of folder.DossierFolderRoles) {
                roles.push(folderRole.Role.EntityShortId);
            }

            let url: string = this._configuration.ODATAUrl;
            let data: string = "$filter=EntityShortId eq ".concat(roles.join(" or EntityShortId eq "));
            this.getRequest(url, data,
                (response: any) => {
                    if (callback && response) {
                        for (let role of response.value) {
                            let currentFolderRole: DossierFolderRoleModel = folder.DossierFolderRoles.filter(f => f.Role.EntityShortId == role.EntityShortId)[0];
                            role.DossierFolderRoles = [currentFolderRole];
                        }
                        callback(response.value)
                    }
                }, error);
        } catch (e) {
            let exception: ExceptionDTO = new ExceptionDTO();
            exception.statusText = e.stack;
            error(exception);
        }
    }


}
export = RoleLocalService;