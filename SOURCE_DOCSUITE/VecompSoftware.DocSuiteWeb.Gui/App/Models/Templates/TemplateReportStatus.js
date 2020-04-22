define(["require", "exports"], function (require, exports) {
    var TemplateReportStatus;
    (function (TemplateReportStatus) {
        TemplateReportStatus[TemplateReportStatus["Draft"] = 0] = "Draft";
        TemplateReportStatus[TemplateReportStatus["Active"] = 1] = "Active";
        TemplateReportStatus[TemplateReportStatus["NotActive"] = 2] = "NotActive";
    })(TemplateReportStatus || (TemplateReportStatus = {}));
    (function (TemplateReportStatus) {
        function toPublicDescription(status) {
            switch (status) {
                case TemplateReportStatus.Draft:
                    return "Bozza";
                case TemplateReportStatus.Active:
                    return "Pubblicato";
                case TemplateReportStatus.NotActive:
                    return "Non Attivo";
            }
        }
        TemplateReportStatus.toPublicDescription = toPublicDescription;
    })(TemplateReportStatus || (TemplateReportStatus = {}));
    return TemplateReportStatus;
});
//# sourceMappingURL=TemplateReportStatus.js.map