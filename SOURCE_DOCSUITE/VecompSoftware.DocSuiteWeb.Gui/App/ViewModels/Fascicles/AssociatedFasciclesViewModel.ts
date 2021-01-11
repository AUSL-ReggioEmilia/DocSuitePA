
import FascicleReferenceType = require('App/Models/Fascicles/FascicleReferenceType');

interface AssociatedFasciclesViewModel {
    UniqueId: string;
    Name: string;
    ImageUrl: string;
    ReferenceImageUrl: string;
    ReferenceTooltip: string;
    OpenCloseTooltip: string;
    FascicleType: FascicleReferenceType;
    UDUniqueId: string;
}

export = AssociatedFasciclesViewModel;