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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Fascicles/FascicleFolderService", "App/Models/Fascicles/FascicleModel", "App/Models/Fascicles/FascicleFolderTypology", "App/Mappers/Fascicles/FascicleSummaryFolderViewModelMapper", "App/DTOs/ValidationExceptionDTO"], function (require, exports, FascBase, ServiceConfigurationHelper, FascicleFolderService, FascicleModel, FascicleFolderTypology, FascicleFolderSummaryModelMapper, ValidationExceptionDTO) {
    var FascicleFolderModifica = /** @class */ (function (_super) {
        __extends(FascicleFolderModifica, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function FascicleFolderModifica(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME)) || this;
            /**
            * ---------------------------- Events ---------------------------------
            */
            /**
            * Evento al click del bottone conferma
            * @param sender
            * @param eventArgs
            * @returns
            */
            _this.btnConferma_ButtonClicked = function (sender, eventArgs) {
                _this._loadingPanel.show(_this.currentPageId);
                _this._btnConferma.set_enabled(false);
                _this.modifyFascicleFolder();
            };
            /*
            * ---------------------------- Methods ----------------------------
            */
            /**
            * Recupero i dati della cartella dalla SessionStorage
            * @param idFasccileFolder
            */
            _this.getFolderFromSessionStorage = function (idFascicleFolder) {
                var fascicleFolder = {};
                var result = sessionStorage[_this.sessionUniqueKey];
                if (result == null) {
                    return null;
                }
                var source = JSON.parse(result);
                if (source) {
                    fascicleFolder.UniqueId = source.UniqueId;
                    fascicleFolder.Name = source.Name;
                    fascicleFolder.Status = source.Status;
                    fascicleFolder.idCategory = source.idCategory;
                    fascicleFolder.idFascicle = source.idFascicle;
                    fascicleFolder.Typology = source.Typology;
                }
                return fascicleFolder;
            };
            _this.closeFascicleFolderModifica = function (data, fascicleFolder) {
                var model = {};
                model.ActionName = "ModifyFolder";
                model.Value = [];
                var mapper = new FascicleFolderSummaryModelMapper();
                var resultModel = mapper.Map(data);
                if (fascicleFolder.Fascicle) {
                    resultModel.idFascicle = fascicleFolder.Fascicle.UniqueId;
                }
                model.Value.push(JSON.stringify(resultModel));
                _this._loadingPanel.hide(_this.currentPageId);
                _this.closeWindow(model);
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
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
         * Inizializzazione
         */
        FascicleFolderModifica.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._btnConferma = $find(this.btnConfermaId);
            this._btnConferma.add_clicked(this.btnConferma_ButtonClicked);
            this._txtName = $find(this.txtNameId);
            this._txtName.focus();
            this._loadingPanel = $find(this.loadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            var fascicleFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME);
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);
            this._loadingPanel.show(this.currentPageId);
            this._currentFolder = this.getFolderFromSessionStorage(this.currentFascicleFolderId);
            this.setData(this._currentFolder);
            this.bindLoaded();
        };
        FascicleFolderModifica.prototype.modifyFascicleFolder = function () {
            var _this = this;
            var fascicleFolder = {};
            fascicleFolder.Name = this._txtName.get_textBoxValue();
            fascicleFolder.UniqueId = this._currentFolder.UniqueId;
            fascicleFolder.Typology = FascicleFolderTypology[this._currentFolder.Typology.toString()];
            if (this._currentFolder.idFascicle) {
                var fasc = new FascicleModel();
                fasc.UniqueId = this._currentFolder.idFascicle;
                fascicleFolder.Fascicle = fasc;
            }
            this._fascicleFolderService.updateFascicleFolder(fascicleFolder, null, function (data) {
                if (data == null)
                    return;
                _this.closeFascicleFolderModifica(data, fascicleFolder);
            }, function (exception) {
                _this._loadingPanel.hide(_this.currentPageId);
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._btnConferma.set_enabled(true);
            });
        };
        /**
         *  Imposto i dati della cartella nella pagina di modifica
         * @param dossierFolder
         */
        FascicleFolderModifica.prototype.setData = function (fascicleFolder) {
            this._txtName.set_value(fascicleFolder.Name);
            this._loadingPanel.hide(this.currentPageId);
        };
        /**
        * salvo lo stato corrente della pagina
        */
        FascicleFolderModifica.prototype.bindLoaded = function () {
            $("#".concat(this.currentPageId)).data(this);
        };
        /**
    * Recupera una RadWindow dalla pagina
    */
        FascicleFolderModifica.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        /**
        * Chiude la RadWindow
        */
        FascicleFolderModifica.prototype.closeWindow = function (message) {
            var wnd = this.getRadWindow();
            wnd.close(message);
        };
        return FascicleFolderModifica;
    }(FascBase));
    return FascicleFolderModifica;
});
//# sourceMappingURL=FascicleFolderModifica.js.map