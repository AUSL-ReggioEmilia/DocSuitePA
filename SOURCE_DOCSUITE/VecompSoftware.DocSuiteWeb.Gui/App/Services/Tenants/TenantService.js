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
define(["require", "exports", "App/Services/BaseService", "App/ViewModels/Tenants/TenantViewModel", "App/Mappers/Tenants/TenantViewModelMapper", "App/Mappers/Commons/ContainerModelMapper", "App/Mappers/PECMails/PECMailBoxViewModelMapper", "App/Mappers/Commons/RoleModelMapper", "App/Mappers/Tenants/TenantConfigurationModelMapper", "App/Models/ODATAResponseModel", "App/Mappers/Commons/ContactModelMapper", "App/Models/Tenants/TenantTypologyTypeEnum"], function (require, exports, BaseService, TenantViewModel, TenantViewModelMapper, ContainerModelMapper, PECMailBoxModelMapper, RoleModelMapper, TenantConfigurationModelMapper, ODATAResponseModel, ContactModelMapper, TenantTypologyTypeEnum) {
    var TenantService = /** @class */ (function (_super) {
        __extends(TenantService, _super);
        function TenantService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        TenantService.prototype.getTenants = function (callback, error) {
            var url = this._configuration.ODATAUrl + "?$orderby=CompanyName&$filter=TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "'";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
                ;
            }, error);
        };
        TenantService.prototype.getTenantById = function (tenantId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=UniqueId eq " + tenantId + " and TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "'&$expand=TenantAOO($filter=TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "')";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var model = new TenantViewModel();
                    var mapper = new TenantViewModelMapper();
                    model = mapper.Map(response.value[0]);
                    callback(model);
                }
                ;
            }, error);
        };
        TenantService.prototype.getTenantContainers = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=Containers&$filter=UniqueId eq " + uniqueId);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper_1 = new ContainerModelMapper();
                    var tenantContainers_1 = [];
                    $.each(response.value[0].Containers, function (i, value) {
                        tenantContainers_1.push(modelMapper_1.Map(value));
                    });
                    callback(tenantContainers_1);
                }
                ;
            }, error);
        };
        TenantService.prototype.getTenantPECMailBoxes = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=PECMailBoxes&$filter=UniqueId eq " + uniqueId);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper_2 = new PECMailBoxModelMapper();
                    var tenantPECMailBoxes_1 = [];
                    $.each(response.value[0].PECMailBoxes, function (i, value) {
                        tenantPECMailBoxes_1.push(modelMapper_2.Map(value));
                    });
                    callback(tenantPECMailBoxes_1);
                }
                ;
            }, error);
        };
        TenantService.prototype.getTenantRoles = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=Roles&$filter=UniqueId eq " + uniqueId);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper_3 = new RoleModelMapper();
                    var tenantRoles_1 = [];
                    $.each(response.value[0].Roles, function (i, value) {
                        tenantRoles_1.push(modelMapper_3.Map(value));
                    });
                    callback(tenantRoles_1);
                }
                ;
            }, error);
        };
        TenantService.prototype.getTenantContacts = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=Contacts&$filter=UniqueId eq " + uniqueId);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper_4 = new ContactModelMapper();
                    var tenantContacts_1 = [];
                    $.each(response.value[0].Contacts, function (i, value) {
                        tenantContacts_1.push(modelMapper_4.Map(value));
                    });
                    callback(tenantContacts_1);
                }
                ;
            }, error);
        };
        TenantService.prototype.getTenantWorkflowRepositories = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=TenantWorkflowRepositories($expand=WorkflowRepository)&$filter=UniqueId eq " + uniqueId);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var tenantWorkflowRepositories = response.value[0].TenantWorkflowRepositories;
                    callback(tenantWorkflowRepositories);
                }
                ;
            }, error);
        };
        TenantService.prototype.getTenantConfigurations = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=Configurations($expand=Tenant)&$filter=UniqueId eq " + uniqueId);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper_5 = new TenantConfigurationModelMapper();
                    var tenantConfigurations_1 = [];
                    $.each(response.value[0].Configurations, function (i, value) {
                        tenantConfigurations_1.push(modelMapper_5.Map(value));
                    });
                    callback(tenantConfigurations_1);
                }
                ;
            }, error);
        };
        TenantService.prototype.updateTenant = function (model, updateActionType, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (updateActionType) {
                url = url + "?actionType=" + updateActionType.toString();
            }
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        TenantService.prototype.insertTenant = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        TenantService.prototype.getAllTenants = function (name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=contains(CompanyName,'" + name + "') and TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "'&$count=true&$top=" + topElement + "&$skip=" + skipElement.toString();
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var mapper = new TenantViewModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);
                    callback(responseModel);
                }
            }, error);
        };
        TenantService.prototype.getAllPECMailBoxes = function (uniqueId, name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=PECMailBoxes($filter=contains(MailBoxRecipient,'" + name + "'))&$filter=UniqueId eq " + uniqueId);
            var qs = " and &$count=true&$top=" + topElement + "&$skip=" + skipElement.toString();
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var modelMapper_6 = new PECMailBoxModelMapper();
                    var tenantPECMailBoxes_2 = [];
                    $.each(response.value[0].PECMailBoxes, function (i, value) {
                        tenantPECMailBoxes_2.push(modelMapper_6.Map(value));
                    });
                    callback(tenantPECMailBoxes_2);
                }
            }, error);
        };
        return TenantService;
    }(BaseService));
    return TenantService;
});
//# sourceMappingURL=TenantService.js.map