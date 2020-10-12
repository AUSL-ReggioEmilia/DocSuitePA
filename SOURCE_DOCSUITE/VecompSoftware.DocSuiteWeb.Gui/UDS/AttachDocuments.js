/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/UDS/UDSService", "App/Services/UDS/UDSRepositoryService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Helpers/StringHelper"], function (require, exports, UDSService, UDSRepositoryService, ServiceConfigurationHelper, ExceptionDTO, StringHelper) {
    var AttachDocuments = /** @class */ (function () {
        function AttachDocuments(serviceConfigurations) {
            var _this = this;
            this.onSearchClientClose = function (sender, args) {
                if (args.get_argument() === null) {
                    return;
                }
                _this._ajaxLoadingPanel.show(_this.pageContentId);
                _this._idUDS = args.get_argument().split('|')[0];
                _this._idUDSRepository = args.get_argument().split('|')[1];
                _this._udsRepositoryService.getUDSRepositoryByID(_this._idUDSRepository, function (data) {
                    var udsRepository = data[0];
                    var udsConfiguration = ServiceConfigurationHelper.getService(_this._serviceConfigurations, udsRepository.Name);
                    _this._udsService = new UDSService(udsConfiguration);
                    _this._udsService.getUDSByUniqueId(_this._idUDS, function (data) {
                        var udsModel = data;
                        _this._populateUDSDetails(udsModel, udsRepository.Name);
                        _this._populateDocumentsGrid();
                    }, function (exception) {
                        _this._ajaxLoadingPanel.hide(_this.pageContentId);
                        _this._showNotificationException(_this.uscNotificationId, exception);
                    });
                }, function (exception) {
                    _this._ajaxLoadingPanel.hide(_this.pageContentId);
                    _this._showNotificationException(_this.uscNotificationId, exception);
                });
                sender.remove_close(_this.onSearchClientClose);
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        AttachDocuments.prototype.initialize = function () {
            var _this = this;
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._btnSearch = document.getElementById(this.btnSearchId);
            this._tblPreview = document.getElementById(this.tblPreviewId);
            this._lblRecordDetails = document.getElementById(this.lblRecordDetailsId);
            this._lblDocumentUDS = document.getElementById(this.lblDocumentUDSId);
            this._lblObject = document.getElementById(this.lblObjectId);
            this._lblCategoryDescription = document.getElementById(this.lblCategoryDescriptionId);
            this._btnAdd = document.getElementById(this.btnAddId);
            this._btnSearch.onclick = function (event) {
                _this._openSearchWindow();
                return false;
            };
            this._btnAdd.onclick = function (event) {
                _this._addDocuments();
                return false;
            };
            var udsRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, AttachDocuments.UDSREPOSITORY_TYPE_NAME);
            this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
        };
        AttachDocuments.prototype._openSearchWindow = function () {
            var winManager = this._getRadWindow().get_windowManager();
            var window = winManager.open("../UDS/UDSLink.aspx?Type=UDS&Action=CopyDocuments", null, null);
            window.set_width(this._getRadWindow().get_width());
            window.set_height(this._getRadWindow().get_height());
            window.add_close(this.onSearchClientClose);
            window.center();
        };
        AttachDocuments.prototype._getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        AttachDocuments.prototype.closeWindow = function (argument) {
            var oWindow = this._getRadWindow();
            oWindow.close(argument);
        };
        AttachDocuments.prototype._populateUDSDetails = function (udsModel, udsRepositoryName) {
            var stringHelper = new StringHelper();
            this._lblRecordDetails.innerText = udsModel.Year + "/" + stringHelper.pad(udsModel.Number, 6) + " del " + new Date(udsModel.RegistrationDate).format("dd/MM/yyyy");
            this._lblDocumentUDS.innerText = udsRepositoryName;
            this._lblObject.innerText = udsModel.Subject;
            this._lblCategoryDescription.innerText = udsModel.Category.Code + "." + udsModel.Category.Name;
            this._tblPreview.style.removeProperty("display");
        };
        AttachDocuments.prototype._populateDocumentsGrid = function () {
            var ajaxModel = {
                ActionName: "SetGridDocuments",
                Value: [this._idUDSRepository, this._idUDS]
            };
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            this._ajaxLoadingPanel.hide(this.pageContentId);
        };
        AttachDocuments.prototype.gridDocumentsCallback = function () {
            this._ajaxLoadingPanel.hide(this.pageContentId);
            this._btnAdd.style.removeProperty("display");
        };
        AttachDocuments.prototype._addDocuments = function () {
            var ajaxModel = {
                ActionName: "AddDocuments",
                Value: [this._idUDSRepository, this._idUDS]
            };
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        AttachDocuments.prototype._showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this._showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        AttachDocuments.prototype._showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        AttachDocuments.UDSREPOSITORY_TYPE_NAME = "UDSRepository";
        return AttachDocuments;
    }());
    return AttachDocuments;
});
//# sourceMappingURL=AttachDocuments.js.map