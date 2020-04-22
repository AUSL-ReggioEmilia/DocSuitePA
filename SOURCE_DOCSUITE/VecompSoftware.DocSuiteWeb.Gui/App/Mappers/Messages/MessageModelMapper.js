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
define(["require", "exports", "App/Mappers/BaseMapper"], function (require, exports, BaseMapper) {
    var MessageModelMapper = /** @class */ (function (_super) {
        __extends(MessageModelMapper, _super);
        function MessageModelMapper() {
            return _super.call(this) || this;
        }
        MessageModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.EntityId = source.EntityId;
            toMap.Status = source.Status;
            toMap.UniqueId = source.UniqueId;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.MessageContacts = source.MessageContacts;
            toMap.MessageEmails = source.MessageEmails;
            return toMap;
        };
        return MessageModelMapper;
    }(BaseMapper));
    return MessageModelMapper;
});
//# sourceMappingURL=MessageModelMapper.js.map