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
define(["require", "exports", "App/Services/BaseService"], function (require, exports, BaseService) {
    var ContainerPropertyService = /** @class */ (function (_super) {
        __extends(ContainerPropertyService, _super);
        /**
         * Costruttore
         */
        function ContainerPropertyService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ContainerPropertyService.prototype.getByContainer = function (idContainer, name, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=Container/EntityShortId eq ".concat(idContainer.toString());
            if (name) {
                data = data.concat(" and Name eq '", name, "'");
            }
            this.getRequest(url, data, function (response) {
                if (callback)
                    callback(response.value);
            }, error);
        };
        /**
        * Inserisco una nuova ContainerProperty
        */
        ContainerPropertyService.prototype.insertContainerProperty = function (containerProperty, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(containerProperty), callback, error);
        };
        /**
        * Aggiorno una ContainerProperty
        */
        ContainerPropertyService.prototype.updateContainerProperty = function (containerProperty, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(containerProperty), callback, error);
        };
        return ContainerPropertyService;
    }(BaseService));
    return ContainerPropertyService;
});
//# sourceMappingURL=ContainerPropertyService.js.map