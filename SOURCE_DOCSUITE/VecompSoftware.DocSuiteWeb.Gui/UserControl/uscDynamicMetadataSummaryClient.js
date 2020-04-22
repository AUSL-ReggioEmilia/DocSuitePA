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
    var uscDynamicMetadataSummaryClient = /** @class */ (function (_super) {
        __extends(uscDynamicMetadataSummaryClient, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function uscDynamicMetadataSummaryClient(serviceConfigurations) {
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
        uscDynamicMetadataSummaryClient.prototype.initialize = function () {
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
        uscDynamicMetadataSummaryClient.prototype.bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        /**
         * Carico i metadati nello usc
         * @param jsonMetadata
         */
        uscDynamicMetadataSummaryClient.prototype.loadMetadatas = function (jsonMetadata) {
            var _this = this;
            var metadataModel = JSON.parse(jsonMetadata);
            this.clearPage();
            var idCloned;
            var content = document.getElementById("menuContent");
            $.each(metadataModel.TextFields, function (index, textField) {
                _this.fillHTMLGenericElement(_this.componentTextId, _this.controlsCounter, textField);
                _this.controlsCounter++;
            });
            $.each(metadataModel.DateFields, function (index, textField) {
                _this.fillHTMLGenericElement(_this.componentDateId, _this.controlsCounter, textField);
                _this.controlsCounter++;
            });
            $.each(metadataModel.EnumFields, function (index, textField) {
                _this.fillHTMLGenericElement(_this.componentEnumId, _this.controlsCounter, textField);
                _this.controlsCounter++;
            });
            $.each(metadataModel.NumberFields, function (index, textField) {
                _this.fillHTMLGenericElement(_this.componentNumberId, _this.controlsCounter, textField);
                _this.controlsCounter++;
            });
            $.each(metadataModel.DiscussionFields, function (index, textField) {
                _this.fillHTMLComment(_this.componentCommentId, _this.controlsCounter, textField);
                _this.controlsCounter++;
            });
            $.each(metadataModel.BoolFields, function (index, textField) {
                _this.fillHTMLCheckBox(_this.componentCheckBoxId, _this.controlsCounter, textField);
                _this.controlsCounter++;
            });
            this.bindLoaded();
        };
        /**
         * Popolo con un campo base le componenti della pagina
         * @param idComponent
         * @param incrementalInteger
         * @param model
         */
        uscDynamicMetadataSummaryClient.prototype.fillHTMLGenericElement = function (idComponent, incrementalInteger, model) {
            var idCloned = this.cloneElement(idComponent, incrementalInteger);
            var labelField = this.findLabelElement(idCloned, 0);
            var valueField = this.findLabelElement(idCloned, 1);
            labelField.textContent = model.Label.concat(": ");
            valueField.textContent = "";
            if (model.Value) {
                valueField.textContent = model.Value;
            }
        };
        uscDynamicMetadataSummaryClient.prototype.fillHTMLCheckBox = function (idComponent, incrementalInteger, model) {
            var idCloned = this.cloneElement(idComponent, incrementalInteger);
            var labelField = this.findLabelElement(idCloned, 0);
            var inputCheckBox = this.findStandardInputElement(idCloned, 0);
            inputCheckBox.checked = (model.Value.toLowerCase() == "true");
            labelField.textContent = model.Label.concat(": ");
        };
        uscDynamicMetadataSummaryClient.prototype.fillHTMLComment = function (idComponent, incrementalInteger, model) {
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
        uscDynamicMetadataSummaryClient.prototype.openCommentsWindow = function (event) {
            var label = event.data.label;
            var managerId = event.data.managerId;
            this._manager = $find(managerId);
            var wnd = this._manager.open("../Comm/ViewMetadataComments.aspx?Type=Fasc&Label=".concat(label), "managerViewComments", null);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        return uscDynamicMetadataSummaryClient;
    }(MetadataRepositoryBase));
    return uscDynamicMetadataSummaryClient;
});
//# sourceMappingURL=uscDynamicMetadataSummaryClient.js.map