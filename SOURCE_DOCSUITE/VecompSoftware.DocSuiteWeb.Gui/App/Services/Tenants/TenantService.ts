import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TenantViewModel = require('App/ViewModels/Tenants/TenantViewModel');
import TenantViewModelMapper = require('App/Mappers/Tenants/TenantViewModelMapper');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');
import PECMailBoxModelMapper = require('App/Mappers/PECMails/PECMailBoxViewModelMapper');
import RoleModel = require('App/Models/Commons/RoleModel');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');
import TenantWorkflowRepositoryModel = require('App/Models/Tenants/TenantWorkflowRepositoryModel');
import TenantConfigurationModel = require('App/Models/Tenants/TenantConfigurationModel');
import TenantConfigurationModelMapper = require('App/Mappers/Tenants/TenantConfigurationModelMapper');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import ContactModelMapper = require('App/Mappers/Commons/ContactModelMapper');
import UpdateActionType = require('App/Models/UpdateActionType');
import TenantTypologyTypeEnum = require('App/Models/Tenants/TenantTypologyTypeEnum');
import PaginationModel = require('App/Models/Commons/PaginationModel');

class TenantService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getTenants(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$orderby=CompanyName&$filter=TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}'`;

        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                callback(response.value);
            };
        }, error);
    }

    getTenantById(tenantId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=UniqueId eq ${tenantId} and TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}'&$expand=TenantAOO($filter=TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}')`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let model: TenantViewModel = new TenantViewModel();
                let mapper = new TenantViewModelMapper();
                model = mapper.Map(response.value[0]);
                callback(model);
            };
        }, error);
    }

    getTenantContainers(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, paginationModel?: PaginationModel): void {
        let baseOdataURL: string = this._configuration.ODATAUrl.concat(`?$expand=Containers`);
        let odataFilterQuery: string = `$filter=UniqueId eq ${uniqueId}`;

        let url: string = paginationModel
            ? `${baseOdataURL}($skip=${paginationModel.Skip};$top=${paginationModel.Take};$count=true)&${odataFilterQuery}`
            : `${baseOdataURL}&${odataFilterQuery}`;

        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new ContainerModelMapper();
                let tenantContainers: ContainerModel[] = [];
                $.each(response.value[0].Containers, function (i, value) {
                    tenantContainers.push(modelMapper.Map(value));
                });

                if (!paginationModel) {
                    callback(tenantContainers);
                    return;
                }

                callback(tenantContainers);
            };
        }, error);
    }

    getTenantPECMailBoxes(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=PECMailBoxes&$filter=UniqueId eq ${uniqueId}`);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new PECMailBoxModelMapper();
                let tenantPECMailBoxes: PECMailBoxModel[] = [];
                $.each(response.value[0].PECMailBoxes, function (i, value) {
                    tenantPECMailBoxes.push(modelMapper.Map(value));
                });
                callback(tenantPECMailBoxes);
            };
        }, error);
    }

    getTenantWorkflowRepositories(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=TenantWorkflowRepositories($expand=WorkflowRepository)&$filter=UniqueId eq ${uniqueId}`);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let tenantWorkflowRepositories: TenantWorkflowRepositoryModel[] = response.value[0].TenantWorkflowRepositories;
                callback(tenantWorkflowRepositories);
            };
        }, error);
    }

    getTenantConfigurations(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=Configurations($expand=Tenant)&$filter=UniqueId eq ${uniqueId}`);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new TenantConfigurationModelMapper();
                let tenantConfigurations: TenantConfigurationModel[] = [];
                $.each(response.value[0].Configurations, function (i, value) {
                    tenantConfigurations.push(modelMapper.Map(value));
                });
                callback(tenantConfigurations);
            };
        }, error);
    }

    updateTenant(model: TenantViewModel, updateActionType: UpdateActionType,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (updateActionType) {
            url = `${url}?actionType=${updateActionType.toString()}`;
        }

        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    insertTenant(model: TenantViewModel,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    getAllTenants(name: string, topElement: string,
        skipElement: number,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=contains(CompanyName,'${name}') and TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}'&$count=true&$top=${topElement}&$skip=${skipElement.toString()}`;

        this.getRequest(url,
            qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<TenantViewModel> = new ODATAResponseModel<TenantViewModel>(response);

                    let mapper = new TenantViewModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);

                    callback(responseModel);
                }
            },
            error);
    }

    getAllPECMailBoxes(uniqueId: string, name: string, topElement: string,
        skipElement: number,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=PECMailBoxes($filter=contains(MailBoxRecipient,'${name}'))&$filter=UniqueId eq ${uniqueId}`);
        let qs: string = ` and &$count=true&$top=${topElement}&$skip=${skipElement.toString()}`;

        this.getRequest(url,
            qs,
            (response: any) => {
                if (callback) {
                    let modelMapper = new PECMailBoxModelMapper();
                    let tenantPECMailBoxes: PECMailBoxModel[] = [];
                    $.each(response.value[0].PECMailBoxes, function (i, value) {
                        tenantPECMailBoxes.push(modelMapper.Map(value));
                    });
                    callback(tenantPECMailBoxes);
                }
            },
            error);
    }

}

export = TenantService;