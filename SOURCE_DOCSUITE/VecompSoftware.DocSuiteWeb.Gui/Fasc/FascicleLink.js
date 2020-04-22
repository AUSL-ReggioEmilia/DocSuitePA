/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
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
define(["require", "exports", "App/Models/Fascicles/FascicleLinkModel", "App/Models/Fascicles/FascicleLinkType", "App/Models/Fascicles/FascicleType", "App/Services/Fascicles/FascicleLinkService", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Securities/DomainUserService", "UserControl/uscFascSummary", "../app/core/extensions/string"], function (require, exports, FascicleLinkModel, FascicleLinkType, FascicleType, FascicleLinkService, FascicleBase, ServiceConfigurationHelper, DomainUserService, UscFascSummary) {
    var FascicleLink = /** @class */ (function (_super) {
        __extends(FascicleLink, _super);
        /**
         * Costruttore
         * @param serviceConfiguration
         */
        function FascicleLink(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
             *------------------------- Events -----------------------------
             */
            _this.btnLink_OnClick = function (sender, args) {
                args.set_cancel(true);
                var selectedFascicle;
                var uscFascicleSearch = $("#" + _this.uscFascicleSearchId).data();
                if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                    selectedFascicle = uscFascicleSearch.getSelectedFascicle();
                }
                if (selectedFascicle == null) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Nessun fascicolo selezionato");
                    return;
                }
                var model = new FascicleLinkModel(selectedFascicle.UniqueId);
                var currentFascicle = _this._currentFascicle;
                model.Fascicle = currentFascicle;
                model.FascicleLinkType = FascicleLinkType.Manual;
                _this._loadingPanel.show(_this.pageContentId);
                _this._fascicleLinkService.insertFascicleLink(model, function (data) {
                    _this.loadData(_this._currentFascicle, function () {
                        _this._loadingPanel.hide(_this.pageContentId);
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.btnRemove_OnClick = function (sender, args) {
                args.set_cancel(true);
                var dataItems = _this._rgvLinkedFascicles.get_selectedItems();
                if (dataItems.length == 0) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Nessun fascicolo selezionato");
                    return;
                }
                var currentFascicle = _this._currentFascicle;
                var model = dataItems[0].get_dataItem();
                var fascicleLink = new FascicleLinkModel(model.UniqueId);
                fascicleLink.Fascicle = currentFascicle;
                fascicleLink.UniqueId = model.FascicleLinkUniqueId;
                _this._loadingPanel.show(_this.pageContentId);
                _this._fascicleLinkService.deleteFascicleLink(fascicleLink, function (data) {
                    _this.loadData(_this._currentFascicle, function () {
                        _this._loadingPanel.hide(_this.pageContentId);
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * Inizializzazione
        */
        FascicleLink.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._btnLink = $find(this.btnLinkId);
            if (this._btnLink) {
                this._btnLink.add_clicking(this.btnLink_OnClick);
            }
            this._btnRemove = $find(this.btnRemoveId);
            if (this._btnRemove) {
                this._btnRemove.add_clicking(this.btnRemove_OnClick);
            }
            this._rgvLinkedFascicles = $find(this.rgvLinkedFasciclesId);
            this._btnLink.set_enabled(false);
            this._btnRemove.set_enabled(false);
            try {
                var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOMAIN_TYPE_NAME);
                this._domainUserService = new DomainUserService(domainUserConfiguration);
                var fascicleLinkServiceConfiguration = $.grep(this._serviceConfigurations, function (x) { return x.Name == FascicleBase.FASCICLE_LINK_TYPE_NAME; })[0];
                this._fascicleLinkService = new FascicleLinkService(fascicleLinkServiceConfiguration);
                $("#".concat(this.uscFascSummaryId)).bind(UscFascSummary.LOADED_EVENT, function (args) {
                    _this.loadFascicle();
                });
                this.loadFascicle();
            }
            catch (error) {
                this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
            }
        };
        /**
      *------------------------- Methods -----------------------------
      */
        FascicleLink.prototype.loadFascicle = function () {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this.service.getFascicle(this.currentFascicleId, function (data) {
                if (data == null) {
                    _this._btnRemove.set_enabled(false);
                    _this._btnLink.set_enabled(false);
                    return;
                }
                var fascicleModel = data;
                var uscFascSummary = $("#".concat(_this.uscFascSummaryId)).data();
                if (!jQuery.isEmptyObject(uscFascSummary)) {
                    uscFascSummary.loadData(fascicleModel);
                }
                _this._currentFascicle = fascicleModel;
                _this.loadData(fascicleModel, function () {
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
        * TODO: da togliere a favore di Signalr
        */
        FascicleLink.prototype.loadData = function (fascicle, callback) {
            var _this = this;
            this.service.getLinkedFascicles(fascicle, null, function (data) {
                var uscFascicleSearch = $("#" + _this.uscFascicleSearchId).data();
                if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                    uscFascicleSearch.clearSelections();
                }
                _this.refreshLinkedFascicles(data);
                if (callback) {
                    callback();
                }
            }, function (exception) {
                _this._btnLink.set_enabled(true);
                _this._btnRemove.set_enabled(true);
                _this.showNotificationException(_this.uscNotificationId, exception);
                if (callback) {
                    callback();
                }
            });
        };
        FascicleLink.prototype.refreshLinkedFascicles = function (data) {
            var models = new Array();
            if (data == null)
                return;
            if (data.FascicleLinks.length > 0) {
                try {
                    $.each(data.FascicleLinks, function (index, fascicleLink) {
                        var model;
                        var imageUrl = "";
                        var openCloseTooltip = "";
                        var fascicleTypeImageUrl = "";
                        var fascicleTypeTooltip = "";
                        if (fascicleLink.FascicleLinked.EndDate == null) {
                            imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                            openCloseTooltip = "Aperto";
                        }
                        else {
                            imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                            openCloseTooltip = "Chiuso";
                        }
                        switch (FascicleType[fascicleLink.FascicleLinked.FascicleType.toString()]) {
                            case FascicleType.Period:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/history.png";
                                fascicleTypeTooltip = "Periodico";
                                break;
                            case FascicleType.Legacy:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_legacy.png";
                                fascicleTypeTooltip = "Fascicolo non a norma";
                                break;
                            case FascicleType.Procedure:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png";
                                fascicleTypeTooltip = "Per procedimento";
                                break;
                            case FascicleType.SubFascicle:
                                fascicleTypeImageUrl = "";
                                fascicleTypeTooltip = "Sotto fascicolo";
                                break;
                        }
                        var tileText = fascicleLink.FascicleLinked.Title.concat(" ", fascicleLink.FascicleLinked.FascicleObject);
                        model = {
                            Name: tileText, FascicleLinkUniqueId: fascicleLink.UniqueId, UniqueId: fascicleLink.FascicleLinked.UniqueId, Category: fascicleLink.FascicleLinked.Category.Name,
                            ImageUrl: imageUrl, OpenCloseTooltip: openCloseTooltip, FascicleTypeImageUrl: fascicleTypeImageUrl, FascicleTypeToolTip: fascicleTypeTooltip
                        };
                        models.push(model);
                    });
                }
                catch (error) {
                    this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                    console.log(error.message);
                    return;
                }
            }
            var tableView = this._rgvLinkedFascicles.get_masterTableView();
            tableView.clearSelectedItems();
            tableView.set_dataSource(models);
            tableView.dataBind();
            //TODO: da rivedere
            var row = tableView.get_dataItems();
            for (var i = 0; i < row.length; i++) {
                if (i % 2) {
                    row[i].addCssClass("Chiaro");
                }
                else {
                    row[i].addCssClass("Scuro");
                }
            }
            this._btnLink.set_enabled(true);
            this._btnRemove.set_enabled(true);
        };
        return FascicleLink;
    }(FascicleBase));
    return FascicleLink;
});
//# sourceMappingURL=FascicleLink.js.map