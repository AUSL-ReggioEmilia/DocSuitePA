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
            var _this = this;
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
            $.each(metadataViewModel.TextFields, function (index, textFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentTextId, _this.integerId, textFieldViewModel);
                _this.integerId++;
            });
            $.each(metadataViewModel.NumberFields, function (index, numberFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentNumberId, _this.integerId, numberFieldViewModel);
                _this.integerId++;
            });
            $.each(metadataViewModel.DateFields, function (index, dateFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentDateId, _this.integerId, dateFieldViewModel);
                _this.integerId++;
            });
            $.each(metadataViewModel.BoolFields, function (index, boolFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentCheckBoxId, _this.integerId, boolFieldViewModel);
                _this.integerId++;
            });
            $.each(metadataViewModel.EnumFields, function (index, enumFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentEnumId, _this.integerId, enumFieldViewModel);
                _this.integerId++;
                $.each(enumFieldViewModel.Options, function (index, option) {
                    var node = document.createElement("LI");
                    if (enumFieldViewModel.Options[index] != "") {
                        _this.createNewNode((Number(index)).toString().concat(") ", enumFieldViewModel.Options[index]), node, idCloned);
                    }
                });
            });
            $.each(metadataViewModel.DiscussionFields, function (index, discussionFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentCommentId, _this.integerId, discussionFieldViewModel);
                _this.integerId++;
            });
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
            labelElement.textContent = model.Label;
            requiredElement.checked = model.Required;
            return idCloned;
        };
        return uscMetadataRepositorySummary;
    }(MetadataRepositoryBase));
    return uscMetadataRepositorySummary;
});
//# sourceMappingURL=uscMetadataRepositorySummary.js.map