interface ResolutionKindModel {
    UniqueId: string;
    Name: string;
    IsActive: boolean;
    AmountEnabled: boolean;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate?: Date;
}
export = ResolutionKindModel;