define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/MetadataRepositoryService", "App/DTOs/ExceptionDTO"], function (require, exports, ServiceConfigurationHelper, MetadataRepositoryService, ExceptionDTO) {
    var uscDynamicMetadata = /** @class */ (function () {
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function uscDynamicMetadata(serviceConfigurations) {
            this.DYNAMIC_FIELD_NAME_FORMAT = "field_";
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
     *------------------------- Methods -----------------------------
     */
        uscDynamicMetadata.prototype.initialize = function () {
            this._metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
            this._metadataRepositoryService = new MetadataRepositoryService(this._metadataRepositoryConfiguration);
            this._manager = $find(this.managerId);
            $("#".concat(this.pnlDynamicContentId)).data(this);
        };
        uscDynamicMetadata.prototype.loadDynamicMetadata = function (id) {
            var _this = this;
            sessionStorage.removeItem("MetadataRepository");
            if (!id || id == "") {
                var ajaxModel = {};
                ajaxModel.ActionName = "ClearControls";
                ajaxModel.Value = [];
                $find(this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
            }
            else {
                this._metadataRepositoryService.getFullModelById(id, function (data) {
                    if (data && !!data.JsonMetadata) {
                        sessionStorage.setItem("MetadataRepository", data.UniqueId);
                        var ajaxModel = {};
                        ajaxModel.ActionName = "LoadControls";
                        ajaxModel.Value = [];
                        ajaxModel.Value.push(data.JsonMetadata);
                        $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
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
        };
        uscDynamicMetadata.prototype.openCommentsWindow = function (label) {
            var wnd = this._manager.open("../Comm/ViewMetadataComments.aspx?Type=Fasc&Label=".concat(label), "managerViewComments", null);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        return uscDynamicMetadata;
    }());
    return uscDynamicMetadata;
});
//# sourceMappingURL=uscDynamicMetadata.js.map