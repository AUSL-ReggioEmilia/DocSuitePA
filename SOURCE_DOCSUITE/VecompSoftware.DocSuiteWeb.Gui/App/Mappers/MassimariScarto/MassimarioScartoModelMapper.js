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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Models/MassimariScarto/MassimarioScartoModel"], function (require, exports, BaseMapper, MassimarioScartoModel) {
    var MassimarioScartoModelMapper = /** @class */ (function (_super) {
        __extends(MassimarioScartoModelMapper, _super);
        function MassimarioScartoModelMapper() {
            return _super.call(this) || this;
        }
        MassimarioScartoModelMapper.prototype.Map = function (source) {
            var toMap = new MassimarioScartoModel();
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Status = source.Status;
            toMap.Name = source.Name;
            toMap.Code = source.Code;
            toMap.FullCode = source.FullCode;
            toMap.Note = source.Note;
            toMap.ConservationPeriod = source.ConservationPeriod;
            toMap.StartDate = source.StartDate;
            toMap.EndDate = source.EndDate;
            toMap.MassimarioScartoPath = source.MassimarioScartoPath;
            toMap.MassimarioScartoParentPath = source.MassimarioScartoParentPath;
            toMap.MassimarioScartoLevel = source.MassimarioScartoLevel;
            toMap.FakeInsertId = source.FakeInsertId;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            return toMap;
        };
        return MassimarioScartoModelMapper;
    }(BaseMapper));
    return MassimarioScartoModelMapper;
});
//# sourceMappingURL=MassimarioScartoModelMapper.js.map