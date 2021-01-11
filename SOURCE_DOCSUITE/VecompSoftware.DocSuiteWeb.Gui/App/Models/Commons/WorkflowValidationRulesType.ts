enum WorkflowValidationRulesType {
    IsExist = "Esiste cartella di fascicolo",
    HasFile = "La cartella contiene almeno un file/inserto",
    HasDocumentUnit = "La cartella contiene almeno una unità documentale",
    HasSignedFile = "La cartella contiene almeno un file/inserto firmato digitalmente"
}

export = WorkflowValidationRulesType;