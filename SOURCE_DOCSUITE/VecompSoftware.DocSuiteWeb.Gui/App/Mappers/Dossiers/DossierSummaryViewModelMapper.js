define(["require", "exports"], function (require, exports) {
    var DossierSummaryViewModelMapper = /** @class */ (function () {
        function DossierSummaryViewModelMapper() {
        }
        DossierSummaryViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Year = source.Year;
            toMap.Number = ("000000000" + source.Number.toString()).slice(-7);
            toMap.Subject = source.Subject;
            toMap.Note = source.Note;
            toMap.ContainerName = source.ContainerName;
            toMap.ContainerId = source.ContainerId;
            toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");
            toMap.RegistrationUser = source.RegistrationUser;
            if (source.LastChangedDate) {
                toMap.LastChangedDate = moment(source.LastChangedDate).format("DD/MM/YYYY");
            }
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.ContactId = source.ContactId;
            toMap.JsonMetadata = source.JsonMetadata;
            toMap.FormattedStartDate = moment(source.StartDate).format("DD/MM/YYYY");
            toMap.StartDate = source.StartDate;
            toMap.Contacts = new Array();
            toMap.Roles = new Array();
            toMap.Documents = new Array();
            return toMap;
        };
        return DossierSummaryViewModelMapper;
    }());
    return DossierSummaryViewModelMapper;
});
//# sourceMappingURL=DossierSummaryViewModelMapper.js.map