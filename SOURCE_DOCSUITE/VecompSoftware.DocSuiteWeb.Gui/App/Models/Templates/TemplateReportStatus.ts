
enum TemplateReportStatus {
    Draft = 0,
    Active = 1,
    NotActive = 2
}

export = TemplateReportStatus;

namespace TemplateReportStatus {
    export function toPublicDescription(status: TemplateReportStatus): string {
        switch (status) {
            case TemplateReportStatus.Draft:
                return "Bozza";
            case TemplateReportStatus.Active:
                return "Pubblicato";
            case TemplateReportStatus.NotActive:
                return "Non Attivo";
        }
    }
}