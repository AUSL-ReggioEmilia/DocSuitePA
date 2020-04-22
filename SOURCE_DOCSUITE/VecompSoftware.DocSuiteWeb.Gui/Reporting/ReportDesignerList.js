/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Templates/TemplateReportService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, TemplateReportService, ServiceConfigurationHelper, ExceptionDTO) {
    var ReportDesignerList = /** @class */ (function () {
        function ReportDesignerList(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.rtvReport_OnNodeClicked = function (source, args) {
                var selectedNode = args.get_node();
                if (!selectedNode || !selectedNode.get_value()) {
                    $("#".concat(_this.uscReportDesignerId)).hide();
                    _this._btnNew.set_enabled(true);
                    _this._btnEdit.set_enabled(false);
                    return;
                }
                _this.showLoading();
                $.when(_this.loadDesigner(args.get_node().get_value()))
                    .done(function () {
                    _this._btnNew.set_enabled(false);
                    _this._btnEdit.set_enabled(true);
                })
                    .fail(function (err) { return _this.showNotification(err); })
                    .always(function () { return _this.hideLoading(); });
            };
            this.btnNew_OnClick = function (source, args) {
                location.href = "ReportDesigner.aspx";
            };
            this.btnEdit_OnClick = function (source, args) {
                var selectedNode = _this._rtvReports.get_selectedNode();
                if (!selectedNode || !selectedNode.get_value()) {
                    return;
                }
                location.href = "ReportDesigner.aspx?ReportUniqueId=".concat(selectedNode.get_value());
            };
            this.toolBarSearch_ButtonClicked = function (sender, eventArgs) {
                var txtSearchDescription = _this._toolBarSearch.findItemByValue('searchDescription').findControl('txtReportName');
                _this.showLoading();
                $.when(_this.loadReports(txtSearchDescription.get_value()))
                    .done(function () {
                    _this._btnNew.set_enabled(true);
                    _this._btnEdit.set_enabled(false);
                    $("#".concat(_this.uscReportDesignerId)).hide();
                })
                    .fail(function (err) { return _this.showNotification(err); })
                    .always(function () { return _this.hideLoading(); });
            };
            var service = ServiceConfigurationHelper.getService(serviceConfigurations, ReportDesignerList.TEMPLATE_REPORT_NAME);
            if (!service) {
                this.showNotification("Nessun servizio configurato per la gestione dei report.");
            }
            this._service = new TemplateReportService(service);
        }
        /**
         *------------------------- Methods -----------------------------
         */
        ReportDesignerList.prototype.initialize = function () {
            var _this = this;
            this._rtvReports = $find(this.rtvReportsId);
            this._rtvReports.add_nodeClicked(this.rtvReport_OnNodeClicked);
            this._btnNew = $find(this.btnNewId);
            this._btnNew.add_clicked(this.btnNew_OnClick);
            this._btnEdit = $find(this.btnEditId);
            this._btnEdit.add_clicked(this.btnEdit_OnClick);
            this._toolBarSearch = $find(this.toolBarSearchId);
            this._toolBarSearch.add_buttonClicked(this.toolBarSearch_ButtonClicked);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            $("#".concat(this.uscReportDesignerId)).hide();
            this._btnNew.set_enabled(true);
            this._btnEdit.set_enabled(false);
            this.showLoading();
            $.when(this.loadReports())
                .fail(function (err) { return _this.showNotification(err); })
                .always(function () { return _this.hideLoading(); });
        };
        ReportDesignerList.prototype.loadReports = function (name) {
            var promise = $.Deferred();
            var rootNode = this._rtvReports.get_nodes().getNode(0);
            rootNode.get_nodes().clear();
            this._service.find(name, function (data) {
                try {
                    var templateReports = data;
                    var node = void 0;
                    for (var _i = 0, templateReports_1 = templateReports; _i < templateReports_1.length; _i++) {
                        var template = templateReports_1[_i];
                        node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(template.Name);
                        node.set_value(template.UniqueId);
                        rootNode.get_nodes().add(node);
                    }
                    rootNode.expand();
                    promise.resolve();
                }
                catch (e) {
                    console.error(e);
                    promise.reject("Errore nel recupero dei report disponibili.");
                }
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        ReportDesignerList.prototype.loadDesigner = function (reportId) {
            var _this = this;
            var promise = $.Deferred();
            var uscReportDesigner = $("#".concat(this.uscReportDesignerId)).data();
            this._service.getById(reportId, function (data) {
                try {
                    var templateReport = data;
                    var reportBuilderModel = JSON.parse(templateReport.ReportBuilderJsonModel);
                    uscReportDesigner.loadDesignerModel(reportBuilderModel);
                    $("#".concat(_this.uscReportDesignerId)).show();
                    promise.resolve();
                }
                catch (e) {
                    console.error(e);
                    promise.reject("Errore nel caricamento dei dati del designer selezionato.");
                }
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        ReportDesignerList.prototype.showLoading = function () {
            this._ajaxLoadingPanel.show(this.splPageId);
        };
        ReportDesignerList.prototype.hideLoading = function () {
            this._ajaxLoadingPanel.hide(this.splPageId);
        };
        ReportDesignerList.prototype.showNotification = function (error) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (error instanceof ExceptionDTO) {
                    uscNotification.showNotification(error);
                }
                else {
                    uscNotification.showNotificationMessage(error);
                }
            }
        };
        ReportDesignerList.TEMPLATE_REPORT_NAME = "TemplateReport";
        return ReportDesignerList;
    }());
    return ReportDesignerList;
});
//# sourceMappingURL=ReportDesignerList.js.map