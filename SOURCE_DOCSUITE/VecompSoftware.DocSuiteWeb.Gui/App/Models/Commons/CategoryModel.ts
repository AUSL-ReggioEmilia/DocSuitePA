import MassimarioScartoModel = require('App/Models/MassimariScarto/MassimarioScartoModel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');

class CategoryModel {
    constructor()
    {       
    }

    EntityShortId: number;
    IdCategory: number;
    Name: string;
    IsActive: number;
    Code: number;
    FullIncrementalPath: string;
    FullCode: string;
    FullSearchComputed: string;
    MassimarioScarto: MassimarioScartoModel;
    MetadataRepository: MetadataRepositoryModel;
    Parent: CategoryModel;
    UniqueId: string;
    Id: number;
    StartDate: Date;
    EndDate?: Date;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate?: Date;

    /**
     * Formatta il FullCode del classificatore
     */
    getFullCodeDotted(): string {
        let codes: string[] = new Array<string>();
        let splittedCodes: Array<string> = this.FullCode.split("|");
        $.each(splittedCodes, (index: number, code: string) => {
            codes.push(Number(code).toString());
        });
        let fullCodeDotted: string = codes.join(".");
        return fullCodeDotted;
    }
}

export = CategoryModel;