import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class ServiceBusTopicService extends BaseService {
    public static TOPIC_NAME_ENTITY_EVENT = "entity_event";
    public static SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY = "PECMailErrorSummary";

    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getTopicMessages(topicName: string, subscriptionName: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.
            concat("?topicName=" + topicName + "&subscriptionName=" + subscriptionName + "&correlationId=null");
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                callback(response.value);
            };
        }, error);
    }

}

export = ServiceBusTopicService;