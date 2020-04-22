var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Services/BaseService", "App/DTOs/ExceptionDTO", "App/Services/Dossiers/DossierFolderLocalService"], function (require, exports, BaseService, ExceptionDTO, DossierFolderLocalService) {
    var RoleLocalService = /** @class */ (function (_super) {
        __extends(RoleLocalService, _super);
        /**
        * Costruttore
        */
        function RoleLocalService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /*
        *
        */
        RoleLocalService.prototype.getDossierFolderRole = function (dossierFolderId, callback, error) {
            try {
                var jsFolders = sessionStorage[DossierFolderLocalService.DOSSIERFOLDERS_SESSIONNAME];
                if (!jsFolders) {
                    callback(undefined);
                    return;
                }
                var folders = JSON.parse(jsFolders);
                folders = folders.filter(function (f) { return f.UniqueId == dossierFolderId; });
                if (!folders || folders.length == 0) {
                    callback(undefined);
                    return;
                }
                var folder_1 = folders[0];
                if (!folder_1.DossierFolderRoles || folder_1.DossierFolderRoles.length == 0) {
                    callback(undefined);
                    return;
                }
                var roles = [];
                for (var _i = 0, _a = folder_1.DossierFolderRoles; _i < _a.length; _i++) {
                    var folderRole = _a[_i];
                    roles.push(folderRole.Role.EntityShortId);
                }
                var url = this._configuration.ODATAUrl;
                var data = "$filter=EntityShortId eq ".concat(roles.join(" or EntityShortId eq "));
                this.getRequest(url, data, function (response) {
                    if (callback && response) {
                        var _loop_1 = function (role) {
                            var currentFolderRole = folder_1.DossierFolderRoles.filter(function (f) { return f.Role.EntityShortId == role.EntityShortId; })[0];
                            role.DossierFolderRoles = [currentFolderRole];
                        };
                        for (var _i = 0, _a = response.value; _i < _a.length; _i++) {
                            var role = _a[_i];
                            _loop_1(role);
                        }
                        callback(response.value);
                    }
                }, error);
            }
            catch (e) {
                var exception = new ExceptionDTO();
                exception.statusText = e.stack;
                error(exception);
            }
        };
        return RoleLocalService;
    }(BaseService));
    return RoleLocalService;
});
//# sourceMappingURL=RoleLocalService.js.map