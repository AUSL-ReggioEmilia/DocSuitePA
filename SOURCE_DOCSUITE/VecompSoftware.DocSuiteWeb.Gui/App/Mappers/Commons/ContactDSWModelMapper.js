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
define(["require", "exports", "App/Mappers/BaseMapper", "./ContactTypeDSWModelMapper", "./ContactAddressDSWModelMapper"], function (require, exports, BaseMapper, ContactTypeDSWModelMapper, ContactAddressDSWModelMapper) {
    var ContactDSWModelMapper = /** @class */ (function (_super) {
        __extends(ContactDSWModelMapper, _super);
        function ContactDSWModelMapper() {
            return _super.call(this) || this;
        }
        ContactDSWModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Id = source.EntityId;
            toMap.IsActive = source.isActive;
            toMap.BirthDate = source.BirthDate;
            toMap.Description = source.Description;
            toMap.Code = source.Code;
            toMap.FiscalCode = source.FiscalCode;
            toMap.TelephoneNumber = source.TelephoneNumber;
            toMap.FaxNumber = source.FaxNumber;
            toMap.EmailAddress = source.EmailAddress;
            toMap.CertifiedMail = source.CertifiedMail;
            toMap.Note = source.Note;
            if (source.Description) {
                var splittedName = source.Description.split("|");
                toMap.FirstName = splittedName[0];
                if (splittedName.length > 1) {
                    toMap.LastName = splittedName[1];
                }
            }
            toMap.SearchCode = source.SearchCode;
            toMap.ContactType = new ContactTypeDSWModelMapper().Map(source);
            toMap.Address = new ContactAddressDSWModelMapper().Map(source);
            return toMap;
        };
        return ContactDSWModelMapper;
    }(BaseMapper));
    return ContactDSWModelMapper;
});
//# sourceMappingURL=ContactDSWModelMapper.js.map