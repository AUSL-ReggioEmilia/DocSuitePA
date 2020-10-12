define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/MetadataRepositoryService", "App/DTOs/ExceptionDTO", "./uscSetiContactSel", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, ServiceConfigurationHelper, MetadataRepositoryService, ExceptionDTO, uscSetiContactSel, SessionStorageKeysHelper) {
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
            var _this = this;
            this._metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
            this._metadataRepositoryService = new MetadataRepositoryService(this._metadataRepositoryConfiguration);
            this._manager = $find(this.managerId);
            $("#".concat(this.pnlDynamicContentId)).data(this);
            $("#".concat(this.fascicleInsertCommonIdEvent)).on(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, function (sender, args) {
                var ajaxModel = {};
                ajaxModel.ActionName = "PopulateFields";
                ajaxModel.Value = [JSON.stringify(args)];
                $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
            });
        };
        uscDynamicMetadata.prototype.loadDynamicMetadata = function (id) {
            var _this = this;
            sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY);
            if (!id || id == "") {
                var ajaxModel = {};
                ajaxModel.ActionName = "ClearControls";
                ajaxModel.Value = [];
                $find(this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
            }
            else {
                this._metadataRepositoryService.getFullModelById(id, function (data) {
                    if (data && !!data.JsonMetadata) {
                        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY, data.UniqueId);
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