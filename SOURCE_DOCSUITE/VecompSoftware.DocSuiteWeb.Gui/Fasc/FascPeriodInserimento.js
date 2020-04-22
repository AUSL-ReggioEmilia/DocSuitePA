/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Fascicles/FascicleService", "App/Models/Fascicles/FascicleModel", "App/Services/Commons/CategoryFascicleService", "App/Models/Fascicles/FascicleType", "App/Models/Commons/MetadataRepositoryModel"], function (require, exports, FascicleBase, ServiceConfigurationHelper, FascicleService, FascicleModel, CategoryFascicleService, FascicleType, MetadataRepositoryModel) {
    var FascPeriodInserimento = /** @class */ (function (_super) {
        __extends(FascPeriodInserimento, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function FascPeriodInserimento(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            _this.btmConferma_ButtonClicked = function (sender, eventArgs) {
                if (!Page_IsValid) {
                    eventArgs.set_cancel(true);
                    return;
                }
                _this._loadingPanel.show(_this.currentPageId);
                _this._btnConferma.set_enabled(false);
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                ajaxModel.ActionName = "Insert";
                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * --------------------------------------------- Events ---------------------------------
        */
        /**
         * Inizializzazione
         */
        FascPeriodInserimento.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._btnConferma = $find(this.btnConfermaId);
            this._btnConferma.add_clicking(this.btmConferma_ButtonClicked);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._uscFascInsertId = this.uscFascInsertId;
            var fascicleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME);
            this._fascicleService = new FascicleService(fascicleConfiguration);
            var categoryFascicleService = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_CATEGORY_FASCICLE);
            this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
        };
        FascPeriodInserimento.prototype.insertAllData = function (fascicle) {
            var _this = this;
            var newFascicle;
            this._categoryFascicleService.geAvailablePeriodicCategoryFascicles(fascicle.Category.EntityShortId.toString(), function (data) {
                if (data) {
                    _this._categoryFascicles = data;
                    $.each(_this._categoryFascicles, function (index, categoryFascicle) {
                        newFascicle = fascicle;
                        if (FascicleType[categoryFascicle.FascicleType] == FascicleType.Period) {
                            $(document).queue(function (next) {
                                newFascicle.DSWEnvironment = categoryFascicle.Environment;
                                _this._fascicleService.insertFascicle(newFascicle, function (data) {
                                    next();
                                }, function (exception) {
                                    _this.showNotificationException(_this.uscNotificationId, exception);
                                });
                            });
                        }
                    });
                }
                $(document).queue(function (next) {
                    window.location.href = "../Fasc/FascRicerca.aspx?Type=Fasc";
                    _this._loadingPanel.hide(_this.currentPageId);
                    next();
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.currentPageId);
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._btnConferma.set_enabled(true);
            });
        };
        FascPeriodInserimento.prototype.insertCallback = function (metadataModel) {
            var uscFascInsert = $("#".concat(this._uscFascInsertId)).data();
            if (!jQuery.isEmptyObject(uscFascInsert)) {
                var fascicle = new FascicleModel;
                fascicle = uscFascInsert.getFascicle();
                if (!!metadataModel) {
                    fascicle.MetadataValues = metadataModel;
                    if (sessionStorage.getItem("MetadataRepository")) {
                        var metadataRepository = new MetadataRepositoryModel();
                        metadataRepository.UniqueId = sessionStorage.getItem("MetadataRepository");
                        fascicle.MetadataRepository = metadataRepository;
                    }
                }
                this.insertAllData(fascicle);
            }
        };
        FascPeriodInserimento.prototype.insertFascicle = function (fascicle) {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._fascicleService.insertFascicle(fascicle, function (data) {
                    promise.resolve();
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                    promise.reject();
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        return FascPeriodInserimento;
    }(FascicleBase));
    return FascPeriodInserimento;
});
//# sourceMappingURL=FascPeriodInserimento.js.map