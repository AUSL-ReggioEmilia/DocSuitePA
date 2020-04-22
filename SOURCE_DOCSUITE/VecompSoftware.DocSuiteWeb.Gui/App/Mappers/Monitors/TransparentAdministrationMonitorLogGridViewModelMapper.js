define(["require", "exports"], function (require, exports) {
    var TransparentAdministrationMonitorLogGridViewModelMapper = /** @class */ (function () {
        function TransparentAdministrationMonitorLogGridViewModelMapper() {
        }
        TransparentAdministrationMonitorLogGridViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Date = moment(source.Date).format("DD/MM/YYYY");
            toMap.Note = source.Note;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.Rating = source.Rating.split('|').join(',');
            toMap.DocumentUnitName = source.DocumentUnitName;
            toMap.IdDocumentUnit = source.IdDocumentUnit;
            toMap.DocumentUnitTitle = "<a href=\"../Series/Item.aspx?UniqueId=" + source.IdDocumentUnit + "&Action=2&PreviousPage=" + location.href + "\">" + source.DocumentUnitTitle + "</a>";
            toMap.IdRole = source.IdRole;
            toMap.RoleName = source.RoleName;
            return toMap;
        };
        return TransparentAdministrationMonitorLogGridViewModelMapper;
    }());
    return TransparentAdministrationMonitorLogGridViewModelMapper;
});
//# sourceMappingURL=TransparentAdministrationMonitorLogGridViewModelMapper.js.map