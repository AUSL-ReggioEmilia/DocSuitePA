import MassimarioScartoStatusType = require('App/Models/MassimariScarto/MassimarioScartoStatusType');

class MassimarioScartoModel {
    UniqueId: string;
    Status: MassimarioScartoStatusType;
    Name: string;
    Code: number;
    FullCode: string;
    Note: string;
    ConservationPeriod: number;
    StartDate: Date;
    EndDate: Date;
    MassimarioScartoPath: string;
    MassimarioScartoParentPath: string;
    MassimarioScartoLevel: number;
    FakeInsertId: string;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate: Date;

    constructor() {       
    }

    isActive(): boolean {
        return this.EndDate == undefined || moment(this.EndDate).isAfter(moment().toDate());
    }

    getPeriodLabel(): string {
        let label: string = '';
        switch (this.ConservationPeriod) {
            case -1:
                label = 'Illimitato';
                break;
            default:
                label = this.ConservationPeriod.toString().concat(' Ann', this.ConservationPeriod == 1 ? 'o' : 'i');
                break;
        }
        return label;
    }
}

export = MassimarioScartoModel;