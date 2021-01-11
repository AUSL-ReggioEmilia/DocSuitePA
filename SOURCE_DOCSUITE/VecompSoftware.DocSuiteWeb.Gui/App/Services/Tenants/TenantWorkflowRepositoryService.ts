import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TenantWorkflowRepositoryModel = require('App/Models/Tenants/TenantWorkflowRepositoryModel');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import WorkflowRepositoryModelMapper = require('App/Mappers/Workflows/WorkflowRepositoryModelMapper');

class TenantWorkflowRepositoryService extends BaseService {
  _configuration: ServiceConfiguration;

  constructor(configuration: ServiceConfiguration) {
    super();
    this._configuration = configuration;
  }

  getTenantWorkflowRepositories(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.ODATAUrl.concat("?$expand=Tenant,WorkflowRepository");
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        callback(response.value);
      };
    }, error);
  }

  getTenantWorkflowRepositoryById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let url: string = `${this._configuration.ODATAUrl}?$expand=Tenant,WorkflowRepository&filter=UniqueId eq ${uniqueId}`;
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        callback(response.value);
      };
    }, error);
  }

  insertTenantWorkflowRepository(model: TenantWorkflowRepositoryModel,
    callback?: (data: any) => any,
    error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.WebAPIUrl;
    this.postRequest(url, JSON.stringify(model), callback, error);
  }

  updateTenantWorkflowRepository(model: TenantWorkflowRepositoryModel,
    callback?: (data: any) => any,
    error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.WebAPIUrl;
    this.putRequest(url, JSON.stringify(model), callback, error);
  }

  deleteTenantWorkflowRepository(model: TenantWorkflowRepositoryModel,
    callback?: (data: any) => any,
    error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.WebAPIUrl;
    this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    getAllWorkflowRepositories(uniqueId: string, name: string, topElement: string,
        skipElement: number,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=Tenant,WorkflowRepository&$filter=contains(WorkflowRepository/Name,'${name}') and Tenant/UniqueId eq ${uniqueId}`);
        let qs: string = ` and &$count=true&$top=${topElement}&$skip=${skipElement.toString()}`;

        this.getRequest(url,
            qs,
            (response: any) => {
                if (callback) {
                    let modelMapper = new WorkflowRepositoryModelMapper();
                    let tenantWorkflowRepo: WorkflowRepositoryModel[] = [];
                    $.each(response.value, function (i, value) {
                        tenantWorkflowRepo.push(modelMapper.Map(value.WorkflowRepository));
                    });
                    callback(tenantWorkflowRepo);
                }
            },
            error);
    }
}

export = TenantWorkflowRepositoryService;