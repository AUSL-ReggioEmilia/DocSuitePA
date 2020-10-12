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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Securities/DomainUserService", "App/DTOs/ExceptionDTO"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, DomainUserService, ExceptionDTO) {
    var uscDynamicMetadataSummaryRest = /** @class */ (function (_super) {
        __extends(uscDynamicMetadataSummaryRest, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function uscDynamicMetadataSummaryRest(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME)) || this;
            _this.controlsCounter = 0;
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /*
         * ---------------------------- Events ---------------------------------
         */
        /**
         * Inizializzazione
         */
        uscDynamicMetadataSummaryRest.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
            this._domainUserService = new DomainUserService(domainUserConfiguration);
            this._manager = $find(this.managerId);
            this.bindLoaded();
        };
        /*
         * --------------------------- Methods ------------------------------
         */
        /**
         * Scateno l'evento di Load Completed del controllo
         */
        uscDynamicMetadataSummaryRest.prototype.bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        /**
         * Carico i metadati nello usc
         * @param jsonMetadata
         */
        uscDynamicMetadataSummaryRest.prototype.loadMetadatas = function (jsonDesignerMetadata, jsonValueMetadataModels) {
            var metadataModel = JSON.parse(jsonDesignerMetadata);
            var metadataValues = [];
            if (jsonValueMetadataModels) {
                metadataValues = JSON.parse(jsonValueMetadataModels);
            }
            this.clearPage();
            this.arrangeControlsInPosition(metadataModel, metadataValues);
            this.bindLoaded();
        };
        uscDynamicMetadataSummaryRest.prototype.arrangeControlsInPosition = function (metadataDesignerViewModel, metadataValues) {
            var aggregatedSum = 0;
            for (var arr in metadataDesignerViewModel) {
                if (typeof (metadataDesignerViewModel[arr]) !== "boolean") {
                    aggregatedSum += metadataDesignerViewModel[arr].length;
                }
            }
            var _loop_1 = function () {
                var metadataField = null;
                var currentType = void 0;
                for (var arr in metadataDesignerViewModel) {
                    currentType = arr;
                    var obj = undefined;
                    if (typeof (metadataDesignerViewModel[arr]) !== "boolean") {
                        obj = metadataDesignerViewModel[arr].filter(function (x) { return x.Position == i; })[0];
                    }
                    if (obj) {
                        metadataField = obj;
                        break;
                    }
                }
                if (!metadataField) {
                    return "continue";
                }
                var currentValue = null;
                if (metadataValues) {
                    var currentMetadataValue = metadataValues.filter(function (x) { return x.KeyName == metadataField.KeyName; })[0];
                    if (currentMetadataValue) {
                        currentValue = currentMetadataValue.Value;
                    }
                }
                switch (currentType) {
                    case MetadataRepositoryBase.CONTROL_TEXT_FIELD:
                        this_1.fillHTMLGenericElement(this_1.componentTextId, this_1.controlsCounter, metadataField, currentValue);
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_DATE_FIELD:
                        if (currentValue) {
                            currentValue = moment(currentValue).format("DD/MM/YYYY");
                        }
                        this_1.fillHTMLGenericElement(this_1.componentDateId, this_1.controlsCounter, metadataField, currentValue);
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_NUMBER_FIELD:
                        this_1.fillHTMLGenericElement(this_1.componentNumberId, this_1.controlsCounter, metadataField, currentValue);
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_BOOL_FIELD:
                        this_1.fillHTMLCheckBox(this_1.componentCheckBoxId, this_1.controlsCounter, metadataField, currentValue);
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_ENUM_FIELD:
                        this_1.fillHTMLGenericElement(this_1.componentEnumId, this_1.controlsCounter, metadataField, currentValue);
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_DISCUSION_FIELD:
                        this_1.fillHTMLComment(this_1.componentCommentId, this_1.controlsCounter, metadataField);
                        this_1.controlsCounter++;
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
         * Popolo con un campo base le componenti della pagina
         * @param idComponent
         * @param incrementalInteger
         * @param model
         */
        uscDynamicMetadataSummaryRest.prototype.fillHTMLGenericElement = function (idComponent, incrementalInteger, model, currentValue) {
            var idCloned = this.cloneElement(idComponent, incrementalInteger);
            var labelField = this.findLabelElement(idCloned, 0);
            var valueField = this.findLabelElement(idCloned, 1);
            labelField.textContent = model.Label.concat(": ");
            valueField.textContent = "";
            if (currentValue) {
                valueField.textContent = currentValue;
            }
        };
        uscDynamicMetadataSummaryRest.prototype.fillHTMLCheckBox = function (idComponent, incrementalInteger, model, currentValue) {
            var idCloned = this.cloneElement(idComponent, incrementalInteger);
            var labelField = this.findLabelElement(idCloned, 0);
            var inputCheckBox = this.findStandardInputElement(idCloned, 0);
            inputCheckBox.checked = (currentValue.toLowerCase() == "true");
            labelField.textContent = model.Label.concat(": ");
        };
        uscDynamicMetadataSummaryRest.prototype.fillHTMLComment = function (idComponent, incrementalInteger, model) {
            var _this = this;
            var idCloned = this.cloneElement(idComponent, incrementalInteger);
            var labelField = this.findLabelElement(idCloned, 0);
            var valueField = this.findLabelElement(idCloned, 1);
            var imgButton = this.findGenericInputControl(idCloned, 0, "imgButton");
            $("#".concat(idCloned, " :input.", "imgButton")).on("click", { label: model.Label, managerId: this.managerId }, this.openCommentsWindow);
            if (model.Comments && model.Comments.length > 0) {
                var latestComment_1 = model.Comments.pop();
                if (latestComment_1.Author) {
                    this._domainUserService.getUser(latestComment_1.Author, function (user) {
                        if (user) {
                            valueField.innerHTML = "<i>".concat(user.DisplayName, " - ", moment(latestComment_1.RegistrationDate.toString(), "YYY-MM-DDTHH:mm:ssZ").format("DD/MM/YYYY"), "</i> <br>", latestComment_1.Comment);
                        }
                    }, function (exception) {
                        var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                        if (exception && uscNotification && exception instanceof ExceptionDTO) {
                            if (!jQuery.isEmptyObject(uscNotification)) {
                                uscNotification.showNotification(exception);
                            }
                        }
                    });
                }
            }
            labelField.textContent = model.Label.concat(": ");
        };
        uscDynamicMetadataSummaryRest.prototype.openCommentsWindow = function (event) {
            var label = event.data.label;
            var managerId = event.data.managerId;
            this._manager = $find(managerId);
            var wnd = this._manager.open("../Comm/ViewMetadataComments.aspx?Type=Fasc&Label=".concat(label), "managerViewComments", null);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        return uscDynamicMetadataSummaryRest;
    }(MetadataRepositoryBase));
    return uscDynamicMetadataSummaryRest;
});
//# sourceMappingURL=uscDynamicMetadataSummaryRest.js.map