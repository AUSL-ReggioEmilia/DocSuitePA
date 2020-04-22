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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper) {
    var TbltMetadataRepositoryDesigner = /** @class */ (function (_super) {
        __extends(TbltMetadataRepositoryDesigner, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function TbltMetadataRepositoryDesigner(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME)) || this;
            /*
             * ----------------------------- Events ---------------------------
             */
            /**
             *  Evento scatenato al click del bottone Pubblica
             * @param sender
             * @param args
             */
            _this.btnPublish_OnClick = function (sender, args) {
                _this._loadingPanel.show(_this.pageContentId);
                _this.setControls(false);
                var item = _this.getMetadataRepository();
                if (item == null) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.setControls(true);
                }
                item.Status = 1;
                _this.saveMetadataRepository(item);
            };
            /**
             * Evento scatenato al click del bottone Pubblica
             * @param sender
             * @param args
             */
            _this.btnDraft_OnClick = function (sender, args) {
                _this._loadingPanel.show(_this.pageContentId);
                _this.setControls(false);
                var item = _this.getMetadataRepository();
                if (item == null) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.setControls(true);
                }
                item.Status = 0;
                _this.saveMetadataRepository(item);
            };
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /**
         * Inizializzazione
         */
        TbltMetadataRepositoryDesigner.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._btnPublish = $find(this.btnPublishId);
            this._btnPublish.add_clicking(this.btnPublish_OnClick);
            this._btnDraft = $find(this.btnDraftId);
            this._btnDraft.add_clicking(this.btnDraft_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            if (this.metadataRepositoryId && this.isEditPage) {
                this.loadMetadataRepository(this.metadataRepositoryId);
            }
        };
        /*
         * ---------------------------- Methods ---------------------------
         */
        /**
         * Funzione che setta la visibilià dei bottoni
         * @param visibility
         */
        TbltMetadataRepositoryDesigner.prototype.setControls = function (visibility) {
            this._btnPublish.set_enabled(visibility);
            this._btnDraft.set_enabled(visibility);
        };
        /**
         * Recupero il MetadataRepositoryModel dallo userControl
         */
        TbltMetadataRepositoryDesigner.prototype.getMetadataRepository = function () {
            var item;
            var uscMetadataDesigner = $("#".concat(this.uscMetadataRepositoryDesignerId)).data();
            if (!jQuery.isEmptyObject(uscMetadataDesigner)) {
                item = uscMetadataDesigner.prepareModel();
            }
            return item;
        };
        /**
         * Metodo di inserimento del MetadataRepository
         * @param metadataRepository
         */
        TbltMetadataRepositoryDesigner.prototype.insertRepositoryModel = function (metadataRepository) {
            var _this = this;
            this._service.insertMetadataRepository(metadataRepository, function (data) {
                if (data) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    window.location.href = "../Tblt/TbltMetadataRepository.aspx?Type=Comm";
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.setControls(true);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * aggiorno una metadataRepository esistente
         * @param metadataRepository
         */
        TbltMetadataRepositoryDesigner.prototype.updateRepositoryModel = function (metadataRepository) {
            var _this = this;
            this._service.updateMetadataRepository(metadataRepository, function (data) {
                if (data) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    window.location.href = "../Tblt/TbltMetadataRepository.aspx?Type=Comm";
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.setControls(true);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * carico una metadataRepository esistente
         * @param idMetadataRepository
         */
        TbltMetadataRepositoryDesigner.prototype.loadMetadataRepository = function (idMetadataRepository) {
            var _this = this;
            this._service.getFullModelById(idMetadataRepository, function (data) {
                if (data) {
                    _this.loadPageItems(data);
                }
            });
        };
        /**
         * inserisco un metadataRepository esistente nel desinger
         * @param metadataRepositoryModel
         */
        TbltMetadataRepositoryDesigner.prototype.loadPageItems = function (metadataRepositoryModel) {
            var uscMetadataDesigner = $("#".concat(this.uscMetadataRepositoryDesignerId)).data();
            if (!jQuery.isEmptyObject(uscMetadataDesigner)) {
                uscMetadataDesigner.loadModel(metadataRepositoryModel);
            }
        };
        /**
         * inserisco o aggiorno la metadataRepository corrente
         * @param item
         */
        TbltMetadataRepositoryDesigner.prototype.saveMetadataRepository = function (item) {
            if (this.isEditPage == true) {
                item.UniqueId = sessionStorage.getItem("UniqueIdMetadataRepository");
                this.updateRepositoryModel(item);
            }
            else {
                this.insertRepositoryModel(item);
            }
        };
        return TbltMetadataRepositoryDesigner;
    }(MetadataRepositoryBase));
    return TbltMetadataRepositoryDesigner;
});
//# sourceMappingURL=TbltMetadataRepositoryDesigner.js.map