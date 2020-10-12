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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscMetadataRepository", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, UscMetadataRepository, SessionStorageKeysHelper) {
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
            _this.folderToolBar_onClick = function (sender, args) {
                switch (args.get_item().get_value()) {
                    case TbltMetadataRepository.CREATE_OPTION: {
                        window.location.href = "../Tblt/TbltMetadataRepositoryDesigner.aspx?Type=Comm";
                        break;
                    }
                    case TbltMetadataRepository.MODIFY_OPTION: {
                        _this._metadataRepositoryId = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_UNIQUEID_METADATA_REPOSITORY);
                        if (!!_this._metadataRepositoryId) {
                            window.location.href = "../Tblt/TbltMetadataRepositoryDesigner.aspx?Type=Comm&IdMetadtaRepository=".concat(_this._metadataRepositoryId, "&IsEditPage=True");
                        }
                        break;
                    }
                }
            };
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /**
         * Inizializzazione
         */
        TbltMetadataRepository.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._folderToolbar = $find(this.folderToolbarId);
            this._folderToolbar.add_buttonClicked(this.folderToolBar_onClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_ROOT_NODE_CLICKED, function (args, data) {
                var uscMetadaRepository = $("#".concat(_this.uscMetadataRepositorySummaryId)).data();
                if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                    uscMetadaRepository.clearPage();
                }
                _this._folderToolbar.findItemByValue(TbltMetadataRepository.CREATE_OPTION).set_enabled(true);
                _this._folderToolbar.findItemByValue(TbltMetadataRepository.MODIFY_OPTION).set_enabled(false);
            });
            $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_NODE_CLICKED, function (args, data) {
                var uscMetadaRepository = $("#".concat(_this.uscMetadataRepositorySummaryId)).data();
                if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_UNIQUEID_METADATA_REPOSITORY, data);
                    uscMetadaRepository.loadMetadataRepository(data);
                }
                _this._folderToolbar.findItemByValue(TbltMetadataRepository.CREATE_OPTION).set_enabled(false);
                _this._folderToolbar.findItemByValue(TbltMetadataRepository.MODIFY_OPTION).set_enabled(true);
            });
        };
        TbltMetadataRepository.CREATE_OPTION = "create";
        TbltMetadataRepository.MODIFY_OPTION = "modify";
        TbltMetadataRepository.DELETE_OPTION = "delete";
        return TbltMetadataRepository;
    }(MetadataRepositoryBase));
    return TbltMetadataRepository;
});
//# sourceMappingURL=TbltMetadataRepository.js.map