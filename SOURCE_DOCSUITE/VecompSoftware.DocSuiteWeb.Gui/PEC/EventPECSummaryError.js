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
define(["require", "exports", "PEC/EventPECSummaryErrorBase", "App/Services/ServiceBus/ServiceBusTopicService", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, EventPECSummaryErrorBase, ServiceBusTopicService, ServiceConfigurationHelper) {
    var EventPECSummaryError = /** @class */ (function (_super) {
        __extends(EventPECSummaryError, _super);
        /**
        * Costruttore
        * @param serviceConfiguration
        */
        function EventPECSummaryError(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, EventPECSummaryErrorBase.SERVICEBUSTOPIC_TYPE_NAME)) || this;
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        *------------------------- Methods -----------------------------
        */
        /**
        * Metodo di inizializzazione
        */
        EventPECSummaryError.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            //region [ Grid Configuration Initialization ]
            this._eventPECSummaryErrorGrid = $find(this.eventPECSummaryErrorGridId);
            this._masterTableView = this._eventPECSummaryErrorGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this.bindLoaded();
            //endregion
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            $("#".concat(this.eventPECSummaryErrorGridId)).bind(EventPECSummaryError.LOADED_EVENT, function () {
                _this.loadPECSummaryErrorGrid();
            });
            $("#".concat(this.eventPECSummaryErrorGridId)).bind(EventPECSummaryError.PAGE_CHANGED_EVENT, function (args) {
                if (!jQuery.isEmptyObject(_this._eventPECSummaryErrorGrid)) {
                    _this.pageChange();
                }
            });
            this.loadResults(ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT, ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY, 0);
        };
        //region [ Grid Configuration Methods ]
        EventPECSummaryError.prototype.onPageChanged = function () {
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            $("#".concat(this.pageId)).triggerHandler(EventPECSummaryError.PAGE_CHANGED_EVENT);
            this.loadResults(ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT, ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY, skip);
        };
        EventPECSummaryError.prototype.onGridDataBound = function () {
            var row = this._masterTableView.get_dataItems();
            for (var i = 0; i < row.length; i++) {
                if (i % 2) {
                    row[i].addCssClass("Chiaro");
                }
                else {
                    row[i].addCssClass("Scuro");
                }
            }
        };
        EventPECSummaryError.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(EventPECSummaryError.LOADED_EVENT);
        };
        EventPECSummaryError.prototype.setDataSource = function (results) {
            this._masterTableView.set_dataSource(results);
            this._masterTableView.dataBind();
        };
        EventPECSummaryError.prototype.setItemCount = function (count) {
            this._masterTableView.set_virtualItemCount(count);
            this._masterTableView.dataBind();
        };
        //endregion
        EventPECSummaryError.prototype.loadPECSummaryErrorGrid = function () {
            if (!jQuery.isEmptyObject(this._eventPECSummaryErrorGrid)) {
                this.loadResults(ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT, ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY, 0);
            }
        };
        EventPECSummaryError.prototype.pageChange = function () {
            this._loadingPanel.show(this.eventPECSummaryErrorGridId);
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_currentPageIndex();
            this.loadResults(ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT, ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY, skip);
        };
        EventPECSummaryError.prototype.loadResults = function (topicName, subscriptionName, skip) {
            var _this = this;
            this._loadingPanel.show(this.eventPECSummaryErrorGridId);
            this.service.getTopicMessages(topicName, subscriptionName, function (data) {
                if (!data)
                    return;
                var result = data;
                var gridSummary = [];
                var pageSize = _this._masterTableView.get_pageSize();
                _this._masterTableView.set_virtualItemCount(result.length);
                var pageCount = skip + pageSize < result.length ? skip + pageSize : result.length;
                for (var i = skip; i < pageCount; i++) {
                    var content = JSON.parse(result[i].Content);
                    var summary = JSON.parse(result[i].Content).Contents.$values[0].ContentValue;
                    summary.ReceivedDate = new Date(summary.ReceivedDate).format("dd/MM/yyyy hh:mm");
                    gridSummary.push(summary);
                }
                _this._masterTableView.set_dataSource(gridSummary);
                _this._masterTableView.dataBind();
                for (var rowIndex = 0; rowIndex < _this._masterTableView.get_dataItems().length; rowIndex++) {
                    _this._masterTableView.getCellByColumnUniqueName(_this._masterTableView.get_dataItems()[rowIndex], "Subject").innerHTML =
                        "<a runat=\"server\" href=\"../PEC/EventPECStreamError.aspx?CorrelatedId=".concat(gridSummary[rowIndex].CorrelatedId).concat("\">").concat(gridSummary[rowIndex].Subject).concat("</a>");
                }
                _this._loadingPanel.hide(_this.eventPECSummaryErrorGridId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.eventPECSummaryErrorGridId);
                $("#".concat(_this.eventPECSummaryErrorGridId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        EventPECSummaryError.LOADED_EVENT = "onLoaded";
        EventPECSummaryError.PAGE_CHANGED_EVENT = "onPageChanged";
        return EventPECSummaryError;
    }(EventPECSummaryErrorBase));
    return EventPECSummaryError;
});
//# sourceMappingURL=EventPECSummaryError.js.map