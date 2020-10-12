/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/DocumentUnits/DocumentUnitService", "App/Helpers/EnumHelper", "App/Models/Environment", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, ServiceConfigurationHelper, DocumentUnitService, EnumHelper, Environment, SessionStorageKeysHelper) {
    var FullTextSearch = /** @class */ (function () {
        /**
         * Costruttore
         */
        function FullTextSearch(serviceConfigurations) {
            var _this = this;
            this.ToolBar_ButtonClick = function (event, args) {
                switch (args.get_item().get_value()) {
                    case "searchFullText": {
                        var documentType = _this._txtFullTextSearch.get_value();
                        var IdTentant = _this.tenantId;
                        _this.loadDocumentUnitsFullText(documentType, IdTentant);
                        break;
                    }
                }
            };
            this.loadDocumentUnitsFullText = function (searchFullText, idTenant) {
                _this._loadingPanel.show(_this.pageContentId);
                _this.getDocumentUnitsFullText(searchFullText, idTenant)
                    .done(function (data) {
                    var dateExpired = new Date();
                    dateExpired.setSeconds(dateExpired.getSeconds() + 15);
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TIME, dateExpired.toString());
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TEXT, searchFullText);
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_DATA, JSON.stringify(data));
                    _this.loadResult(data);
                    _this._loadingPanel.hide(_this.pageContentId);
                }).fail(function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        FullTextSearch.prototype.initialize = function () {
            var documentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
            this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._actionToolbar = $find(this.actionsToolbarId);
            this._toolBarFullText = this._actionToolbar.findItemByValue("toolBarFullText");
            this._txtFullTextSearch = this._toolBarFullText.findControl("txtFullTextSearch");
            this._btnFullTextSearch = this._actionToolbar.findItemByValue(this.btnFullTextSearchId);
            this._rgvDocumentLists = $find(this.rgvDocumentListsId);
            this._rgvDocumentMasterTableView = this._rgvDocumentLists.get_masterTableView();
            this._rgvDocumentLists.get_element().style.display = "none";
            this._actionToolbar.add_buttonClicked(this.ToolBar_ButtonClick);
            /// Document units environment images
            this._environmentImageDictionary = {};
            this._registerEnvironmentImages();
            this.setLastSearch();
        };
        FullTextSearch.prototype.loadResult = function (documentUnits) {
            var _this = this;
            this._rgvDocumentLists.get_element().style.display = "";
            var models = new Array();
            $.each(documentUnits, function (index, documentUnitModel) {
                var model;
                var valuesUrl = _this.formatUrl(documentUnitModel);
                model = {
                    DocumentUnit: documentUnitModel,
                    ViewerUrl: valuesUrl[0],
                    DocumentUnitUrl: valuesUrl[1],
                    IconUrl: _this.getIconUrl(documentUnitModel.Environment),
                    RegistrationDate: moment(documentUnitModel.RegistrationDate).format("DD/MM/YYYY")
                };
                models.push(model);
            });
            this._rgvDocumentMasterTableView.set_dataSource(models);
            this._rgvDocumentMasterTableView.set_virtualItemCount(documentUnits.length);
            this._rgvDocumentMasterTableView.dataBind();
        };
        FullTextSearch.prototype.getDocumentUnitsFullText = function (searchFullText, idTenant) {
            var promise = $.Deferred();
            this._documentUnitService.getDocumentUnitsFullText(searchFullText, idTenant, function (data) {
                promise.resolve(data);
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        FullTextSearch.prototype.formatUrl = function (documentUnit) {
            var formatDocumentUnitUrl = "";
            var formatViewerUrl = "";
            formatViewerUrl = "";
            formatDocumentUnitUrl = "";
            var env = (documentUnit.Environment < 100 ? documentUnit.Environment : Environment.UDS);
            switch (env) {
                case Environment.Any: {
                    formatViewerUrl = "../Viewers/ProtocolViewer.aspx?UniqueId=" + documentUnit.UniqueId + "&Type=Prot\"";
                    formatDocumentUnitUrl = "../Prot/ProtVisualizza.aspx?UniqueId=" + documentUnit.UniqueId + "&Type=Prot";
                    break;
                }
                case Environment.Dossier: {
                    formatViewerUrl = "../Viewers/DossierViewer.aspx?Type=Dossier&IdDossier=" + documentUnit.UniqueId;
                    formatDocumentUnitUrl = "../Dossier/DossierVisualizza.aspx?Type=Dossier&IdDossier=" + documentUnit.UniqueId + "&DossierTitle=" + documentUnit.Number;
                    break;
                }
                case Environment.DocumentSeries: {
                    formatViewerUrl = "../Viewers/DocumentSeriesItemViewer.aspx?ids=" + documentUnit.EntityId.toString() + "&Type=Prot\"";
                    formatDocumentUnitUrl = "..Series/Item.aspx?IdDocumentSeriesItem=" + documentUnit.EntityId.toString() + "&Action=View&Type=Series";
                    break;
                }
                case Environment.Fascicle: {
                    formatViewerUrl = "../Viewers/FascicleViewer.aspx?Type=Fasc&FascicleIds=" + documentUnit.IdFascicle;
                    formatDocumentUnitUrl = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + documentUnit.IdFascicle;
                    break;
                }
                case Environment.Resolution: {
                    formatViewerUrl = "../Viewers/ResolutionViewer.aspx?" + documentUnit.UniqueId + ".aspx?UniqueId=" + documentUnit.UniqueId + "&Type=Prot\"";
                    formatDocumentUnitUrl = "../Resl/ReslVisualizza.aspx?Type=Resl&IdResolution=" + documentUnit.UniqueId;
                    break;
                }
                case Environment.Protocol: {
                    formatViewerUrl = "../Viewers/ProtocolViewer.aspx?UniqueId=" + documentUnit.UniqueId + "&Type=Prot\"";
                    formatDocumentUnitUrl = "../Prot/ProtVisualizza.aspx?UniqueId=" + documentUnit.UniqueId + "&Type=Prot";
                    ;
                    break;
                }
                case Environment.UDS: {
                    formatViewerUrl = "../Viewers/UDSViewer.aspx?IdUDS=".concat(documentUnit.UniqueId, " & IdUDSRepository=", documentUnit.UDSRepository.UniqueId);
                    formatDocumentUnitUrl = "../UDS/UDSView.aspx?IdUDSRepository=" + documentUnit.UDSRepository.UniqueId + "&IdUDS=" + documentUnit.UniqueId + "&Action=View&Type=UDS";
                    break;
                }
            }
            return [formatViewerUrl, formatDocumentUnitUrl];
        };
        FullTextSearch.prototype.setLastSearch = function () {
            var documentUnitsFullTextTime = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TIME);
            if (documentUnitsFullTextTime) {
                var dateExpired = Date.parse(documentUnitsFullTextTime);
                if (dateExpired < Date.now()) {
                    sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TIME);
                    sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TEXT);
                    sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNITS_FULLTEXT_DATA);
                    this._txtFullTextSearch.set_value("");
                }
                else {
                    var newdateExpired = new Date();
                    newdateExpired.setSeconds(newdateExpired.getSeconds() + 15);
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TIME, newdateExpired.toString());
                    this._txtFullTextSearch.set_value(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TEXT));
                    var documentUnits = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_DATA));
                    this.loadResult(documentUnits);
                }
            }
        };
        FullTextSearch.prototype._registerEnvironmentImages = function () {
            this._environmentImageDictionary[Environment.Protocol] = "../Comm/Images/DocSuite/Protocollo16.gif";
            this._environmentImageDictionary[Environment.Resolution] = "../Comm/Images/DocSuite/Atti16.gif";
            this._environmentImageDictionary[Environment.DocumentSeries] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
            this._environmentImageDictionary[Environment.UDS] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
        };
        FullTextSearch.prototype.getIconUrl = function (env) {
            return this._environmentImageDictionary[env] ?
                this._environmentImageDictionary[env] : "../App_Themes/DocSuite2008/imgset16/document.png";
        };
        return FullTextSearch;
    }());
    return FullTextSearch;
});
//# sourceMappingURL=FullTextSearch.js.map