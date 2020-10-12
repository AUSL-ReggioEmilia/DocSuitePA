import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import POLRequestModelMapper = require('App/Mappers/PosteWeb/POLRequestModelMapper');
import POLRequest = require('App/Models/PosteWeb/POLRequest');
import TNoticeStatusSummaryDTO = require('App/DTOs/TNoticeStatusSummaryDTO');
import POLRequestSummaryMapper = require('App/Mappers/PosteWeb/POLRequestSummaryMapper');

class TNoticeService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getRequestByRequestId(requestId: string, callback?: (data: POLRequest) => any, error?: (exception: ExceptionDTO) => any): void {

        let url: string = this._configuration.ODATAUrl;
        let query: string = `/POLRequestService.GetPOLRequest(uniqueId=@p0)?@p0=${requestId}`

        url = `${url}${query}`;

        this.getRequest(url, null,
            (response: any) => {
                if (callback && response) {

                    let mapper = new POLRequestModelMapper();
                    let _polRequest: POLRequest = mapper.Map(response.value[0]);

                    if (_polRequest === null) {
                        return;
                    }

                    callback(_polRequest)
                }
            }, error);
    }

    countRequestsSummariesByDocumentId(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "/$count?$filter=DocumentUnit/UniqueId eq ".concat(documentUnitId);
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    getRequestsSummariesByDocumentId(documentUnitId: string, callback?: (data: TNoticeStatusSummaryDTO[]) => any, error?: (exception: ExceptionDTO) => any): void {

        let url: string = this._configuration.ODATAUrl;
        let query: string = `/POLRequestService.GetPOLRequestsByDocumentUnitId(documentUnitId=@p0)?@p0=${documentUnitId}`

        url = `${url}${query}`;
        let _this = this;

        this.getRequest(url, null,
            (response: any) => {
                if (callback && response) {

                    let summaries: TNoticeStatusSummaryDTO[] = []

                    $.each(response.value, function (i, value) {
                        let mapper = new POLRequestSummaryMapper();

                        let summary: TNoticeStatusSummaryDTO = mapper.Map(value);

                        summaries.push(summary)
                    });

                    callback(summaries)
                }
            }, error);
    }
}

export = TNoticeService;