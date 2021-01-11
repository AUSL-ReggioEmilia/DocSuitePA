import ConservationStatusType = require("./ConservationStatusType");
import ConservationType = require("./ConservationType");

interface ConservationModel {
    UniqueId: number;
    EntityType: string;
    Message: string;
    SendDate?: Date;
    Uri: string;
    Status: ConservationStatusType;
    Type: ConservationType;
}

export = ConservationModel;