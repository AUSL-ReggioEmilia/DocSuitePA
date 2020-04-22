define(["require", "exports"], function (require, exports) {
    var Environment;
    (function (Environment) {
        Environment[Environment["Any"] = 0] = "Any";
        Environment[Environment["Protocol"] = 1] = "Protocol";
        Environment[Environment["Resolution"] = 2] = "Resolution";
        Environment[Environment["Document"] = 3] = "Document";
        Environment[Environment["DocumentSeries"] = 4] = "DocumentSeries";
        Environment[Environment["Desk"] = 5] = "Desk";
        Environment[Environment["Workflow"] = 6] = "Workflow";
        Environment[Environment["UDS"] = 7] = "UDS";
        Environment[Environment["Fascicle"] = 8] = "Fascicle";
        Environment[Environment["Dossier"] = 9] = "Dossier";
    })(Environment || (Environment = {}));
    (function (Environment) {
        function toPublicDescription(env) {
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
        Environment.toPublicDescription = toPublicDescription;
    })(Environment || (Environment = {}));
    return Environment;
});
//# sourceMappingURL=Environment.js.map