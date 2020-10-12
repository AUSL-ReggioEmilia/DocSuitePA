define(["require", "exports", "App/Models/PosteWeb/StatusColor", "App/Helpers/NullSafe"], function (require, exports, StatusColor, NullSafe) {
    var POLRequestHelper = /** @class */ (function () {
        function POLRequestHelper() {
        }
        POLRequestHelper.DetermineStatusColor = function (input) {
            var displayColor = StatusColor.Red;
            if (NullSafe.Do(function () { return input.IsFaulted; }, false)) {
                displayColor = StatusColor.Red;
            }
            else if (NullSafe.Do(function () { return POLRequestHelper.IsNullOrEmpty(input.GetStatus.UrlAccept); }, false)) {
                displayColor = StatusColor.Yellow;
            }
            else if (NullSafe.Do(function () { return POLRequestHelper.IsNullOrEmpty(input.GetStatus.UrlCPF); }, false)) {
                displayColor = StatusColor.Green;
            }
            else {
                displayColor = StatusColor.Blue;
            }
            return displayColor;
        };
        POLRequestHelper.IsNullOrEmpty = function (str) {
            return !str;
        };
        return POLRequestHelper;
    }());
    return POLRequestHelper;
});
//# sourceMappingURL=POLRequestHelper.js.map