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
define(["require", "exports", "App/Services/BaseService"], function (require, exports, BaseService) {
    var ServiceBusTopicService = /** @class */ (function (_super) {
        __extends(ServiceBusTopicService, _super);
        function ServiceBusTopicService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ServiceBusTopicService.prototype.getTopicMessages = function (topicName, subscriptionName, callback, error) {
            var url = this._configuration.ODATAUrl.
                concat("?topicName=" + topicName + "&subscriptionName=" + subscriptionName + "&correlationId=null");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
                ;
            }, error);
        };
        ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT = "entity_event";
        ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY = "PECMailErrorSummary";
        return ServiceBusTopicService;
    }(BaseService));
    return ServiceBusTopicService;
});
//# sourceMappingURL=ServiceBusTopicService.js.map