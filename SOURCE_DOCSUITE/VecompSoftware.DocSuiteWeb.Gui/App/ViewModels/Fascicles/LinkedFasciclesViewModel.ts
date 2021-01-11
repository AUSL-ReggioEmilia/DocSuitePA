
import FascicleType = require('App/Models/Fascicles/FascicleType');

interface LinkedFasciclesViewModel {
    UniqueId: string;
    FascicleLinkUniqueId: string;
    Name: string;
    ImageUrl: string;
    OpenCloseTooltip: string;
    Category: string;
    FascicleTypeImageUrl: string;
    FascicleTypeToolTip: string;
}

export = LinkedFasciclesViewModel;