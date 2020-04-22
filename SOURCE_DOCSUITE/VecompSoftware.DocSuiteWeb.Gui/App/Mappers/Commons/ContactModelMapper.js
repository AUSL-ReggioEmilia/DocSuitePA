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
    var ContactModelMapper = /** @class */ (function (_super) {
        __extends(ContactModelMapper, _super);
        function ContactModelMapper() {
            return _super.call(this) || this;
        }
        ContactModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.ActiveFrom = source.ActiveFrom;
            toMap.ActiveTo = source.ActiveTo;
            toMap.Address = source.Address;
            toMap.BirthDate = source.BirthDate;
            toMap.CertifiedMail = source.CertifiedMail;
            toMap.City = source.City;
            toMap.CityCode = source.CityCode;
            toMap.CivicNumber = source.CivicNumber;
            toMap.Code = source.Code;
            toMap.Description = source.Description;
            toMap.EmailAddress = source.EmailAddress;
            toMap.EntityId = source.EntityId;
            toMap.FaxNumber = source.FaxNumber;
            toMap.FiscalCode = source.FiscalCode;
            toMap.FullIncrementalPath = source.FullIncrementalPath;
            toMap.IdContactType = source.IdContactType;
            toMap.IncrementalFather = source.IncrementalFather;
            toMap.isActive = source.isActive;
            toMap.isChanged = source.isChanged;
            toMap.isLocked = source.isLocked;
            toMap.isNotExpandable = source.isNotExpandable;
            toMap.Note = source.Note;
            toMap.SearchCode = source.SearchCode;
            toMap.TelephoneNumber = source.TelephoneNumber;
            toMap.UniqueId = source.UniqueId;
            toMap.ZipCode = source.ZipCode;
            toMap.Title = source.Title;
            toMap.PlaceName = source.PlaceName;
            return toMap;
        };
        return ContactModelMapper;
    }(BaseMapper));
    return ContactModelMapper;
});
//# sourceMappingURL=ContactModelMapper.js.map