import StatusColor = require("App/Models/PosteWeb/StatusColor");
import NullSafe = require("App/Helpers/NullSafe");
import POLRequestExtendedProperties = require("./POLRequestExtendedProperties");

class POLRequestHelper {
    public static DetermineStatusColor(input : POLRequestExtendedProperties): StatusColor {
        let displayColor: StatusColor = StatusColor.Red;

        if (NullSafe.Do<boolean>(() => input.IsFaulted, false)) {
            displayColor = StatusColor.Red;
        } else if (NullSafe.Do<boolean>(() => POLRequestHelper.IsNullOrEmpty(input.GetStatus.UrlAccept), false)) {
            displayColor = StatusColor.Yellow;
        } else if (NullSafe.Do<boolean>(() => POLRequestHelper.IsNullOrEmpty(input.GetStatus.UrlCPF), false)) {
            displayColor = StatusColor.Green;
        } else {
            displayColor = StatusColor.Blue;
        }

        return displayColor;
    }

    private static IsNullOrEmpty(str: string): boolean {
        return !str;
    }
}

export = POLRequestHelper