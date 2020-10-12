define(["require", "exports", "App/Mappers/Dossiers/DossierFolderSummaryModelMapper", "App/Models/Dossiers/DossierFolderStatus", "App/Models/UpdateActionType", "App/DTOs/ExceptionDTO", "App/Helpers/GuidHelper"], function (require, exports, DossierSummaryFolderViewModelMapper, DossierFolderStatus, UpdateActionType, ExceptionDTO, Guid) {
    var DossierFolderLocalService = /** @class */ (function () {
        /**
        * Costruttore
        */
        function DossierFolderLocalService() {
        }
        DossierFolderLocalService.prototype.getStorageFolders = function () {
            var jsFolders = sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME];
            if (!jsFolders) {
                return [];
            }
            return JSON.parse(jsFolders);
        };
        DossierFolderLocalService.prototype.getChildren = function (uniqueId, status, callback, error) {
            try {
                var folders = this.getStorageFolders();
                if (!folders) {
                    callback(undefined);
                    return;
                }
                if (uniqueId) {
                    folders = folders.filter(function (f) { return f.ParentInsertId == uniqueId; });
                    if (!folders) {
                        callback(undefined);
                        return;
                    }
                }
                var instance = new DossierSummaryFolderViewModelMapper();
                var models = [];
                for (var _i = 0, folders_1 = folders; _i < folders_1.length; _i++) {
                    var folder = folders_1[_i];
                    models.push(instance.Map(folder));
                }
                callback(models);
            }
            catch (e) {
                var exception = new ExceptionDTO();
                exception.statusText = e.stack;
                error(exception);
            }
        };
        /**
        * Inserisco una nuova DossierFolder
        */
        DossierFolderLocalService.prototype.insertDossierFolder = function (dossierFolder, insertAction, callback, error) {
            try {
                var folders = this.getStorageFolders();
                if (!folders) {
                    folders = [];
                }
                dossierFolder.UniqueId = Guid.newGuid();
                if (dossierFolder.Fascicle) {
                    dossierFolder.Name = dossierFolder.Fascicle.FascicleObject;
                }
                var existSameName = folders.filter(function (x) { return x.ParentInsertId == dossierFolder.ParentInsertId && x.Name.trim() == dossierFolder.Name.trim(); }).length > 0;
                if (existSameName) {
                    var error_1 = new Error();
                    error_1.stack = "Il nome delle cartelle di un Dossier deve essere univoco per cartelle dello stesso livello.";
                    throw error_1;
                }
                folders.push(dossierFolder);
                var parentFolders = folders.filter(function (f) { return f.UniqueId == dossierFolder.ParentInsertId; });
                if (dossierFolder.ParentInsertId && parentFolders.length > 0) {
                    var parentFolder = parentFolders[0];
                    parentFolder.Status = DossierFolderStatus.Folder;
                }
                sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME] = JSON.stringify(folders);
                callback(dossierFolder);
            }
            catch (e) {
                var exception = new ExceptionDTO();
                exception.statusText = e.stack;
                error(exception);
            }
        };
        /**
        * Cancellazione di una DossierFolder esistente
        * @param model
        * @param callback
        * @param error
        */
        DossierFolderLocalService.prototype.deleteDossierFolder = function (model, callback, error) {
            try {
                var folders = this.getStorageFolders();
                if (!folders) {
                    callback(undefined);
                    return;
                }
                var filteredFolders = folders.filter(function (f) { return f.UniqueId != model.UniqueId; });
                if (model.ParentInsertId) {
                    var existChildren = filteredFolders.filter(function (f) { return f.ParentInsertId == model.ParentInsertId; }).length > 0;
                    if (!existChildren) {
                        var parentFolders = folders.filter(function (f) { return f.UniqueId == model.ParentInsertId; });
                        if (parentFolders.length > 0) {
                            var parentFolder = parentFolders[0];
                            parentFolder.Status = DossierFolderStatus.InProgress;
                        }
                    }
                }
                sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME] = JSON.stringify(filteredFolders);
                callback(undefined);
            }
            catch (e) {
                var exception = new ExceptionDTO();
                exception.statusText = e.stack;
                error(exception);
            }
        };
        /**
        * Aggiorno una DossierFolder
        */
        DossierFolderLocalService.prototype.updateDossierFolder = function (dossierFolder, updateAction, callback, error) {
            try {
                var folders = this.getStorageFolders();
                if (!folders) {
                    callback(undefined);
                    return;
                }
                var filteredFolders_1 = folders.filter(function (f) { return f.UniqueId == dossierFolder.UniqueId; });
                if (!filteredFolders_1) {
                    callback(undefined);
                    return;
                }
                var existSameName = folders.filter(function (x) { return x.ParentInsertId == filteredFolders_1[0].ParentInsertId && x.Name.trim() == dossierFolder.Name.trim() && x.UniqueId != dossierFolder.UniqueId; }).length > 0;
                if (existSameName) {
                    var error_2 = new Error();
                    error_2.stack = "Il nome delle cartelle di un Dossier deve essere univoco per cartelle dello stesso livello.";
                    throw error_2;
                }
                if (updateAction == UpdateActionType.DossierFolderAuthorizationsPropagation) {
                    this.propagateFolderRolesRecursive(filteredFolders_1[0].ParentInsertId, dossierFolder.DossierFolderRoles, folders);
                }
                var folder = filteredFolders_1[0];
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
            }
            catch (e) {
                var exception = new ExceptionDTO();
                exception.statusText = e.stack;
                error(exception);
            }
        };
        DossierFolderLocalService.prototype.propagateFolderRolesRecursive = function (parentId, dossierFolderRoles, folders) {
            var childrenFolders = folders.filter(function (f) { return f.ParentInsertId == parentId; });
            if (!childrenFolders || childrenFolders.length == 0) {
                return;
            }
            var _loop_1 = function (folder) {
                folder.DossierFolderRoles = [];
                folder.DossierFolderRoles = dossierFolderRoles;
                var existChildren = folders.filter(function (f) { return f.ParentInsertId == folder.UniqueId; }).length > 0;
                if (existChildren) {
                    this_1.propagateFolderRolesRecursive(folder.UniqueId, dossierFolderRoles, folders);
                }
            };
            var this_1 = this;
            for (var _i = 0, childrenFolders_1 = childrenFolders; _i < childrenFolders_1.length; _i++) {
                var folder = childrenFolders_1[_i];
                _loop_1(folder);
            }
        };
        /*
        *
        */
        DossierFolderLocalService.prototype.getDossierFolder = function (uniqueId, callback, error) {
            try {
                var folders = this.getStorageFolders();
                if (!folders) {
                    callback(undefined);
                    return;
                }
                var filteredFolders = folders.filter(function (f) { return f.UniqueId == uniqueId; });
                if (!filteredFolders) {
                    callback(undefined);
                    return;
                }
                var folder = filteredFolders[0];
                var instance = new DossierSummaryFolderViewModelMapper();
                callback(instance.Map(folder));
            }
            catch (e) {
                var exception = new ExceptionDTO();
                exception.statusText = e.stack;
                error(exception);
            }
        };
        DossierFolderLocalService.prototype.getFullDossierFolder = function (uniqueId, callback, error) {
            try {
                var folders = this.getStorageFolders();
                if (!folders) {
                    callback(undefined);
                    return;
                }
                var filteredFolders = folders.filter(function (f) { return f.UniqueId == uniqueId; });
                if (!filteredFolders) {
                    callback(undefined);
                    return;
                }
                var folder = filteredFolders[0];
                callback(folder);
            }
            catch (e) {
                var exception = new ExceptionDTO();
                exception.statusText = e.stack;
                error(exception);
            }
        };
        DossierFolderLocalService.prototype.getAllParentsOfFascicle = function (idDossier, idFascicle, callback, error) {
            throw new Error("Method not implemented.");
        };
        DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME = "dossierfoldersessionname";
        return DossierFolderLocalService;
    }());
    return DossierFolderLocalService;
});
//# sourceMappingURL=DossierFolderLocalService.js.map