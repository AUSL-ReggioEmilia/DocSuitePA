/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, ExceptionDTO) {
    var uscMetadataRepositorySummary = /** @class */ (function (_super) {
        __extends(uscMetadataRepositorySummary, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function uscMetadataRepositorySummary(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME)) || this;
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /**
         * ------------------------------------ Events ---------------------------
         */
        /**
         * inizializzazione
         */
        uscMetadataRepositorySummary.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this.integerId = 1;
            this.bindLoaded();
        };
        /**
         * ----------------------------------- Methods ----------------------------
         */
        /**
         * funzione che carica le componenti della pagina
         * @param idMetadataRepository
         */
        uscMetadataRepositorySummary.prototype.loadMetadataRepository = function (idMetadataRepository) {
            var _this = this;
            this._service.getFullModelById(idMetadataRepository, function (data) {
                if (data) {
                    _this.loadPageItems(data);
                }
            }, function (exception) {
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (exception && uscNotification && exception instanceof ExceptionDTO) {
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                }
            });
        };
        /**
         * funzione che aggiunge tutte le componenti corrispondenti al Json del menu
         */
        uscMetadataRepositorySummary.prototype.loadPageItems = function (metadataRepositoryModel) {
            var metadataViewModel = JSON.parse(metadataRepositoryModel.JsonMetadata);
            var element;
            var content = document.getElementById("menuContent");
            var idCloned;
            var labelElement;
            var requiredElement;
            this.clearPage();
            idCloned = this.cloneElement(this.componentTitleId, this.integerId);
            this.integerId++;
            labelElement = this.findLabelElement(idCloned, 0);
            labelElement.textContent = metadataRepositoryModel.Name;
            var setiEnabled = document.getElementById("setiFieldId");
            if (this.setiIntegrationEnabledId && metadataViewModel.SETIFieldEnabled !== undefined) { /*Ensure that the message is not visible if the SetiIntegrationEnabled is set to false*/
                setiEnabled.innerText = metadataViewModel.SETIFieldEnabled ? "(Integrazione SETI abilitata)" : "(Integrazione SETI non abilitata)";
            }
            this.arrangeControlsInPosition(metadataViewModel, idCloned);
        };
        uscMetadataRepositorySummary.prototype.arrangeControlsInPosition = function (metadataViewModel, idCloned) {
            var _this = this;
            var aggregatedSum = 0;
            for (var arr in metadataViewModel) {
                if (typeof (metadataViewModel[arr]) !== "boolean") {
                    aggregatedSum += metadataViewModel[arr].length;
                }
            }
            var _loop_1 = function () {
                var metadataField = null;
                var currentType = void 0;
                for (var arr in metadataViewModel) {
                    currentType = arr;
                    var obj = undefined;
                    if (typeof (metadataViewModel[arr]) !== "boolean") {
                        obj = metadataViewModel[arr].filter(function (x) { return x.Position == i; })[0];
                    }
                    if (obj) {
                        metadataField = obj;
                        break;
                    }
                }
                switch (currentType) {
                    case MetadataRepositoryBase.CONTROL_TEXT_FIELD:
                        idCloned = this_1.fillHTMLElement(this_1.componentTextId, metadataField.Position, metadataField);
                        break;
                    case MetadataRepositoryBase.CONTROL_DATE_FIELD:
                        idCloned = this_1.fillHTMLElement(this_1.componentDateId, metadataField.Position, metadataField);
                        break;
                    case MetadataRepositoryBase.CONTROL_NUMBER_FIELD:
                        idCloned = this_1.fillHTMLElement(this_1.componentNumberId, metadataField.Position, metadataField);
                        break;
                    case MetadataRepositoryBase.CONTROL_BOOL_FIELD:
                        idCloned = this_1.fillHTMLElement(this_1.componentCheckBoxId, metadataField.Position, metadataField);
                        break;
                    case MetadataRepositoryBase.CONTROL_ENUM_FIELD:
                        idCloned = this_1.fillHTMLElement(this_1.componentEnumId, metadataField.Position, metadataField);
                        $.each(metadataField.Options, function (index, option) {
                            var node = document.createElement("LI");
                            if (metadataField.Options[index] != "") {
                                _this.createNewNode(metadataField.Options[index], node, idCloned);
                            }
                        });
                        break;
                    case MetadataRepositoryBase.CONTROL_DISCUSION_FIELD:
                        idCloned = this_1.fillHTMLElement(this_1.componentCommentId, metadataField.Position, metadataField);
                        break;
                    default:
                        break;
                }
            };
            var this_1 = this;
            for (var i = 0; i <= aggregatedSum; i++) {
                _loop_1();
            }
        };
        /**
         * Scateno l'evento di "Load Completed" del controllo
         */
        uscMetadataRepositorySummary.prototype.bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        uscMetadataRepositorySummary.prototype.fillHTMLElement = function (idComponent, incrementalInteger, model) {
            var idCloned = this.cloneElement(idComponent, incrementalInteger);
            var labelElement = this.findLabelElement(idCloned, 1);
            var requiredElement = this.findInputCheckBoxElement(idCloned, 0);
            var keynameElement = this.findLabelElement(idCloned, 3);
            labelElement.textContent = model.Label;
            keynameElement.textContent = model.KeyName;
            requiredElement.checked = model.Required;
            return idCloned;
        };
        return uscMetadataRepositorySummary;
    }(MetadataRepositoryBase));
    return uscMetadataRepositorySummary;
});
//# sourceMappingURL=uscMetadataRepositorySummary.js.map