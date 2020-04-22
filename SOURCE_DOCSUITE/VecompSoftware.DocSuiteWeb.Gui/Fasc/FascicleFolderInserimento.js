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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Fascicles/FascicleFolderService", "App/Models/Fascicles/FascicleFolderStatus", "App/Models/Fascicles/FascicleFolderTypology", "App/Mappers/Fascicles/FascicleSummaryFolderViewModelMapper", "App/DTOs/ValidationExceptionDTO", "App/Helpers/GuidHelper"], function (require, exports, FascBase, ServiceConfigurationHelper, FascicleFolderService, FascicleFolderStatus, FascicleFolderTypology, FascicelFolderSummaryModelMapper, ValidationExceptionDTO, Guid) {
    var FascicleFolderInserimento = /** @class */ (function (_super) {
        __extends(FascicleFolderInserimento, _super);
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function FascicleFolderInserimento(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME)) || this;
            /**
        *------------------------- Events -----------------------------
        */
            /**
           * Evento scatenato al click del pulsante ConfermaInserimento
           * @method
           * @param sender
           * @param eventArgs
           * @returns
           */
            _this.btmConferma_ButtonClicked = function (sender, eventArgs) {
                if (!Page_IsValid) {
                    return;
                }
                _this._loadingPanel.show(_this.currentPageId);
                _this._btnConferma.set_enabled(false);
                _this.insertFascicleFolder();
            };
            _this.callInsertFascicleFolderService = function (fascicleFolder) {
                _this._fascicleFolderService.insertFascicleFolder(fascicleFolder, null, function (data) {
                    if (data == null)
                        return;
                    var model = {};
                    model.ActionName = "ManageParent";
                    model.Value = [];
                    var mapper = new FascicelFolderSummaryModelMapper();
                    var modelFascicleFolderSummary = mapper.Map(data);
                    if (fascicleFolder.Fascicle != null && fascicleFolder.Fascicle.UniqueId != null) {
                        modelFascicleFolderSummary.idFascicle = fascicleFolder.Fascicle.UniqueId;
                    }
                    model.Value.push(JSON.stringify(modelFascicleFolderSummary));
                    _this._loadingPanel.hide(_this.currentPageId);
                    _this.closeWindow(model);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.currentPageId);
                    _this.exceptionWindow(exception);
                    _this._btnConferma.set_enabled(true);
                });
            };
            _this.exceptionWindow = function (exception) {
                var message = "";
                var ex = exception;
                if (exception && exception instanceof ValidationExceptionDTO && exception.validationMessages.length > 0) {
                    message = message.concat("Gli errori sono i seguenti: <br />");
                    exception.validationMessages.forEach(function (item) {
                        message = message.concat(item.message, "<br />");
                    });
                }
                _this.showNotificationException(_this.uscNotificationId, ex, message);
            };
            /**
            * Recupero la cartella salvata nella session storage
            * @param idFascicleFolder
            */
            _this.getFolderParent = function (idFascicleFolder) {
                var fascicleFolder = {};
                var result = sessionStorage[_this.sessionUniqueKey];
                if (result == null) {
                    return null;
                }
                var source = JSON.parse(result);
                if (source) {
                    fascicleFolder.UniqueId = source.UniqueId;
                    fascicleFolder.Fascicle = {};
                    fascicleFolder.Fascicle.UniqueId = source.idFascicle;
                    fascicleFolder.Typology = source.Typology;
                }
                return fascicleFolder;
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * Inizializzazione
        */
        FascicleFolderInserimento.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConferma = $find(this.btnConfermaId);
            this._btnConferma.add_clicked(this.btmConferma_ButtonClicked);
            this._txtName = $find(this.txtNameId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.managerId);
            var fascicleFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME);
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);
            $("#".concat(this.fascicleNameRowId)).show();
            this._loadingPanel.hide(this.currentPageId);
            this._txtName.focus();
        };
        /**
        *---------------------------- Methods ---------------------------
        */
        FascicleFolderInserimento.prototype.insertFascicleFolder = function () {
            var fascicleFolder = {};
            var fascicle = {};
            fascicleFolder.Status = FascicleFolderStatus.Active;
            fascicleFolder.Name = this._txtName.get_textBoxValue();
            fascicleFolder.Fascicle = fascicle;
            var fascicleFolderToUpdate = this.getFolderParent(this.currentFascicleFolderId);
            if (fascicleFolderToUpdate) {
                fascicleFolder.ParentInsertId = fascicleFolderToUpdate.UniqueId;
                fascicleFolder.Fascicle.UniqueId = fascicleFolderToUpdate.Fascicle.UniqueId;
                fascicleFolder.Typology = fascicleFolderToUpdate.Typology;
                var typology = FascicleFolderTypology[fascicleFolder.Typology.toString()];
                if (isNaN(typology)) {
                    typology = FascicleFolderTypology[typology.toString()];
                }
                if (typology == FascicleFolderTypology.Fascicle) {
                    fascicleFolder.Typology = FascicleFolderTypology.SubFascicle;
                }
            }
            ;
            if (this.doNotUpdateDatabase === "False") {
                this.callInsertFascicleFolderService(fascicleFolder);
            }
            else {
                this._loadingPanel.hide(this.currentPageId);
                var model = {};
                model.ActionName = "ManageParent";
                model.Value = [];
                fascicleFolder.UniqueId = Guid.newGuid();
                fascicleFolder.Typology = FascicleFolderTypology.Fascicle;
                model.Value.push(JSON.stringify(fascicleFolder));
                sessionStorage.setItem("InsertedFascicleFolder", JSON.stringify(model));
                this.closeWindow();
            }
        };
        /**
    * Chiude la RadWindow
    */
        FascicleFolderInserimento.prototype.closeWindow = function (message) {
            var wnd = this.getRadWindow();
            wnd.close(message);
        };
        /**
    * Recupera una RadWindow dalla pagina
    */
        FascicleFolderInserimento.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        return FascicleFolderInserimento;
    }(FascBase));
    return FascicleFolderInserimento;
});
//# sourceMappingURL=FascicleFolderInserimento.js.map