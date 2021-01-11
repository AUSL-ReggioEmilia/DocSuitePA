
enum Environment {
    Any = 0,
    Protocol = 1,
    Resolution = 2,
    Document = 3,
    DocumentSeries = 4,
    Desk = 5,
    Workflow = 6,
    UDS = 7,
    Fascicle = 8,
    Dossier = 9
}

export = Environment;

namespace Environment {
    export function toPublicDescription(env: Environment): string {
        switch (env) {
            case Environment.Protocol:
                return "Protocolli";
            case Environment.Resolution:
                return "Atti"; //TODO: Gestire in maniera differente con l'indicazione da DB
            case Environment.Document:
                return "Pratiche";
            case Environment.DocumentSeries:
                return "Serie documentali";
            case Environment.Desk:
                return "Tavoli";
            case Environment.UDS:
                return "Archivi";
            case Environment.Fascicle:
                return "Fascicoli";
            case Environment.Dossier:
                return "Dossier";
            default:
                return env.toString();
        }
    }
}