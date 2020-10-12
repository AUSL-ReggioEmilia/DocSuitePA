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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Commons/ContactModelMapper"], function (require, exports, BaseService, ContactModelMapper) {
    var ContactService = /** @class */ (function (_super) {
        __extends(ContactService, _super);
        function ContactService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ContactService.prototype.getById = function (idContact, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=EntityId eq " + idContact + "&$expand=Title,PlaceName";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    if (response.value.length == 0) {
                        callback(null);
                        return;
                    }
                    var instance = {};
                    var contactMapper = new ContactModelMapper();
                    instance = contactMapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        ContactService.prototype.findContacts = function (finder, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/ContactService.FindContacts(finder=@p0)?@p0=" + JSON.stringify(finder) + "&$orderby=Description&$top=100");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        ContactService.prototype.getContactParents = function (idContact, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/ContactService.GetContactParents(idContact=" + idContact + ")");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        ContactService.prototype.getRoleContacts = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/ContactService.GetRoleContacts()?$orderby=Description");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        ContactService.prototype.getByParentId = function (contactParentId, top, callback, error) {
            var url = this._configuration.ODATAUrl + "/ContactService.GetContactsByParentId(idContact=" + contactParentId + ")";
            if (top && top > 0) {
                url = url + "?$top=" + top;
            }
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    if (callback) {
                        callback(response.value);
                    }
                }
            }, error);
        };
        /**
         * metodo per l'inserimento di un nuovo contatto
         * @param categoryFascicle
         * @param callback
         * @param error
         */
        ContactService.prototype.insertContact = function (contact, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(contact), callback, error);
        };
        return ContactService;
    }(BaseService));
    return ContactService;
});
//# sourceMappingURL=ContactService.js.map