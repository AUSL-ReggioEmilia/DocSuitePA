
class PrivacyLevelModel {
    constructor()
    {       
    }

    Description: string;
    Level: number;
    Colour: string;
    IsActive: boolean;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate?: Date;
    UniqueId: string;
}

export = PrivacyLevelModel;