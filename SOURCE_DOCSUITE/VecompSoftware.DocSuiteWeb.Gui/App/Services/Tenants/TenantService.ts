import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TenantViewModel = require('App/ViewModels/Tenants/TenantViewModel');
import TenantViewModelMapper = require('App/Mappers/Tenants/TenantViewModelMapper');
import TenantSearchFilterDTO = require('App/DTOs/TenantSearchFilterDTO');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');
import PECMailBoxModelMapper = require('App/Mappers/PECMails/PECMailBoxViewModelMapper');
import RoleModel = require('App/Models/Commons/RoleModel');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import WorkflowRepositoryModelMapper = require('App/Mappers/Workflows/WorkflowRepositoryModelMapper');
import TenantWorkflowRepositoryModel = require('App/Models/Tenants/TenantWorkflowRepositoryModel');
import TenantConfigurationModel = require('App/Models/Tenants/TenantConfigurationModel');
import TenantConfigurationModelMapper = require('App/Mappers/Tenants/TenantConfigurationModelMapper');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ContactModel = require('../../Models/Commons/ContactModel');
import ContactModelMapper = require('../../Mappers/Commons/ContactModelMapper');

class TenantService extends BaseService {
  _configuration: ServiceConfiguration;

  constructor(configuration: ServiceConfiguration) {
    super();
    this._configuration = configuration;
  }

  getTenants(searchFilter: TenantSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let urlPart: string = this._configuration.ODATAUrl;

    urlPart = urlPart + "?$expand=Configurations($expand=Tenant),Containers,PECMailBoxes,Roles,TenantWorkflowRepositories($expand=WorkflowRepository)";
    let oDataFilters: string = "";

    if (searchFilter.tenantName) {
      oDataFilters = oDataFilters.concat(` &$filter=contains(TenantName, '${searchFilter.tenantName}')`);
    }
    if (searchFilter.companyName) {
      oDataFilters = oDataFilters.concat(` and contains(CompanyName, '${searchFilter.companyName}')`);
    }
    oDataFilters = oDataFilters.concat(" &$orderby=CompanyName");

    let url: string = urlPart.concat(oDataFilters);
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        callback(response.value);
      };
    }, error);
  }

  getTenantContainers(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.ODATAUrl.concat(`?$expand=Containers&$filter=UniqueId eq ${uniqueId}`);
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        let modelMapper = new ContainerModelMapper();
        let tenantContainers: ContainerModel[] = [];
        $.each(response.value[0].Containers, function (i, value) {
          tenantContainers.push(modelMapper.Map(value));
        });
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

  getTenantRoles(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.ODATAUrl.concat(`?$expand=Roles&$filter=UniqueId eq ${uniqueId}`);
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        let modelMapper = new RoleModelMapper();
        let tenantRoles: RoleModel[] = [];
        $.each(response.value[0].Roles, function (i, value) {
          tenantRoles.push(modelMapper.Map(value));
        });
        callback(tenantRoles);
      };
    }, error);
    }

    getTenantContacts(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=Contacts&$filter=UniqueId eq ${uniqueId}`);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new ContactModelMapper();
                let tenantContacts: ContactModel[] = [];
                $.each(response.value[0].Contacts, function (i, value) {
                    tenantContacts.push(modelMapper.Map(value));
                });
                callback(tenantContacts);
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

  updateTenant(model: TenantViewModel,
    callback?: (data: any) => any,
      error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.WebAPIUrl;
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
        let qs: string = `$filter=contains(CompanyName,'${name}')&$count=true&$top=${topElement}&$skip=${skipElement.toString()}`;

        this.getRequest(url,
            qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<TenantViewModel> = new ODATAResponseModel<TenantViewModel>(response);

                    let mapper = new TenantViewModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);;

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