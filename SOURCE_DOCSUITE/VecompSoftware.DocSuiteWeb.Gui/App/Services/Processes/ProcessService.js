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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Processes/ProcessModelMapper"], function (require, exports, BaseService, ProcessModelMapper) {
    var ProcessService = /** @class */ (function (_super) {
        __extends(ProcessService, _super);
        function ProcessService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ProcessService.prototype.getAll = function (searchName, isActive, callback, error) {
            var url = this._configuration.ODATAUrl + "?$expand=Dossier,Category,Roles($expand=Father)";
            if (isActive !== null) {
                var filter = "";
                if (isActive) {
                    filter = "&$filter=(EndDate ge now() or EndDate eq null)";
                }
                else {
                    filter = "&$filter=EndDate le now()";
                }
                url = url.concat(filter);
                if (searchName !== "") {
                    url = url.concat(" and contains(Name,'" + searchName + "')");
                }
            }
            else if (searchName !== "") {
                url = url.concat("&$filter=contains(Name,'" + searchName + "')");
            }
            url = url.concat("&$orderby=Name");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new ProcessModelMapper();
                    var processes = [];
                    for (var _i = 0, _a = response.value; _i < _a.length; _i++) {
                        var value = _a[_i];
                        processes.push(modelMapper.Map(value));
                    }
                    callback(processes);
                }
            }, error);
        };
        ProcessService.prototype.getById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=UniqueId eq " + uniqueId + "&$expand=Category,Dossier,Roles($expand=Father)";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new ProcessModelMapper();
                    var process = modelMapper.Map(response.value[0]);
                    callback(process);
                }
            }, error);
        };
        ProcessService.prototype.getAvailableProcesses = function (name, loadOnlyMy, categoryId, dossierId, callback, error) {
            if (name) {
                name = "'" + name + "'";
            }
            var url = this._configuration.ODATAUrl + "/ProcessService.AvailableProcesses(name=" + name + ",categoryId=" + (!!categoryId ? categoryId.toString() : null) + ",dossierId=" + (!!dossierId ? dossierId.toString() : null) + ",loadOnlyMy=" + loadOnlyMy + ")?$orderby=Name asc";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var mapper = new ProcessModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        ProcessService.prototype.getProcessesByCategoryId = function (categoryId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var odataFilter = "$filter=Category/EntityShortId eq " + categoryId + "&$expand=Dossier,Category,Roles&$orderby=Name";
            this.getRequest(url, odataFilter, function (response) {
                if (callback && response) {
                    var mapper = new ProcessModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        ProcessService.prototype.getProcessByDossierFolderId = function (dossierFolderId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var odataFilter = "$filter=Dossier/DossierFolders/any(df: df/UniqueId eq " + dossierFolderId + ")";
            this.getRequest(url, odataFilter, function (response) {
                if (callback && response) {
                    var mapper = new ProcessModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        ProcessService.prototype.insert = function (process, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(process), callback, error);
        };
        ProcessService.prototype.update = function (process, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(process), callback, error);
        };
        ProcessService.prototype.delete = function (process, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(process), callback, error);
        };
        return ProcessService;
    }(BaseService));
    return ProcessService;
});
//# sourceMappingURL=ProcessService.js.map