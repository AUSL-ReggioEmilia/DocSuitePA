import IMapper = require('App/Mappers/IMapper');
import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import BaseEntityRoleViewModel = require('App/ViewModels/BaseEntityRoleViewModel');
import CategoryTreeViewModelMapper = require('App/Mappers/Commons/CategoryTreeViewModelMapper');
declare var moment: any;
class DossierSummaryViewModelMapper implements IMapper<DossierSummaryViewModel>{
    constructor() {
    }

    public Map(source: any): DossierSummaryViewModel {
        let toMap: DossierSummaryViewModel = <DossierSummaryViewModel>{};
        if (!source) {
            return null;
        }
        toMap.UniqueId = source.UniqueId;
        toMap.Year = source.Year;
        toMap.Number = ("000000000" + source.Number.toString()).slice(-7);
        toMap.Subject = source.Subject;
        toMap.Note = source.Note;
        toMap.ContainerName = source.ContainerName;
        toMap.ContainerId = source.ContainerId;
        toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");
        toMap.RegistrationUser = source.RegistrationUser;
        if (source.LastChangedDate) {
            toMap.LastChangedDate = moment(source.LastChangedDate).format("DD/MM/YYYY");
        }
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.ContactId = source.ContactId;
        toMap.MetadataDesigner = source.MetadataDesigner;
        toMap.MetadataValues = source.MetadataValues;
        toMap.FormattedStartDate = moment(source.StartDate).format("DD/MM/YYYY");
        toMap.StartDate = source.StartDate;
        toMap.DossierType = source.DossierType;
        toMap.Status = source.Status;
        toMap.Contacts = new Array<BaseEntityViewModel>();
        toMap.Roles = new Array<BaseEntityRoleViewModel>();
        toMap.Documents = new Array<BaseEntityViewModel>();
        toMap.Category = new CategoryTreeViewModelMapper().Map(source.Category);
       
        return toMap;
    }
}
export = DossierSummaryViewModelMapper;