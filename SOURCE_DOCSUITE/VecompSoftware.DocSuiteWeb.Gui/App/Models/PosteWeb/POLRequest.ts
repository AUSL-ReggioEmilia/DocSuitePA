import POLRequestExtendedProperties = require("./ExtendedProperties/POLRequestExtendedProperties");
import StatusColor = require("./StatusColor");

interface POLRequest {
    Id: string,
    DocumentName: string,
    DocumentPosteFileType : string,
    RequestId: string,
    ErrorMessage: string,
    GuidPoste: string,
    IdArchiveChain: string,
    IdArchiveChainPoste : string,
    IdOrdine: string,
    Status: number,
    StatusDescription : string,
    ExtendedProperties: string,
    ExtendedPropertiesDeserialized: POLRequestExtendedProperties,
    TotalCost: number,
    RegistrationDate: string,
    DisplayColor: StatusColor
}

export = POLRequest;
