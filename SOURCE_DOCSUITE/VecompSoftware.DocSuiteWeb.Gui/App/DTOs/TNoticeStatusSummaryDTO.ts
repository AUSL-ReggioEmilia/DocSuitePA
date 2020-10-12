import StatusColor = require("App/Models/PosteWeb/StatusColor");

interface TNoticeStatusSummaryDTO {
    RequestUniqueId: string;
    DisplayColor: StatusColor;
    Status: string;
    RegistrationDate: string;
}

export = TNoticeStatusSummaryDTO;
