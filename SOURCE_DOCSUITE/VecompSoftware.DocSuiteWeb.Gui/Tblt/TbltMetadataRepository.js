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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscMetadataRepository"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, UscMetadataRepository) {
    var TbltMetadataRepository = /** @class */ (function (_super) {
        __extends(TbltMetadataRepository, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function TbltMetadataRepository(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME)) || this;
            /*
             * ----------------------------- Events ---------------------------
             */
            /**
             *  Evento scatenato al click del bottone Aggiungi
             * @param sender
             * @param args
             */
            _this.btnAggiungi_OnClick = function (sender, args) {
                window.location.href = "../Tblt/TbltMetadataRepositoryDesigner.aspx?Type=Comm";
            };
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /**
         * Evento scatenato al clik del bottone Modifica
         * @param sender
         * @param args
         */
        TbltMetadataRepository.prototype.btnModifica_OnClick = function (sender, args) {
            this._metadataRepositoryId = sessionStorage.getItem("UniqueIdMetadataRepository");
            if (!!this._metadataRepositoryId) {
                window.location.href = "../Tblt/TbltMetadataRepositoryDesigner.aspx?Type=Comm&IdMetadtaRepository=".concat(this._metadataRepositoryId, "&IsEditPage=True");
            }
        };
        /**
         * Inizializzazione
         */
        TbltMetadataRepository.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._btnAggiungi = $find(this.btnAggiungiId);
            this._btnAggiungi.add_clicking(this.btnAggiungi_OnClick);
            this._btnModifica = $find(this.btnModificaId);
            this._btnModifica.add_clicking(this.btnModifica_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_ROOT_NODE_CLICKED, function (args, data) {
                var uscMetadaRepository = $("#".concat(_this.uscMetadataRepositorySummaryId)).data();
                if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                    uscMetadaRepository.clearPage();
                }
                _this._btnAggiungi.set_enabled(true);
                _this._btnModifica.set_enabled(false);
            });
            $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_NODE_CLICKED, function (args, data) {
                var uscMetadaRepository = $("#".concat(_this.uscMetadataRepositorySummaryId)).data();
                if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                    sessionStorage.setItem("UniqueIdMetadataRepository", data);
                    uscMetadaRepository.loadMetadataRepository(data);
                }
                _this._btnAggiungi.set_enabled(false);
                _this._btnModifica.set_enabled(true);
            });
        };
        return TbltMetadataRepository;
    }(MetadataRepositoryBase));
    return TbltMetadataRepository;
});
//# sourceMappingURL=TbltMetadataRepository.js.map