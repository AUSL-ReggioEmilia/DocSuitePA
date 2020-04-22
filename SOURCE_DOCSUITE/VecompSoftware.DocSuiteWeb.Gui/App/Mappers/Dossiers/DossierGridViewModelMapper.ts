import DossierGridViewModel = require('App/ViewModels/Dossiers/DossierGridViewModel');
import IMapper = require('App/Mappers/IMapper');

class DossierGridViewModelMapper implements IMapper<DossierGridViewModel>{
    construnctor() {
    }
    public Map(source: any): DossierGridViewModel {
        let toMap: DossierGridViewModel = <DossierGridViewModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Year = source.Year;
        toMap.Number = source.Number;
        toMap.Subject = source.Subject;
        toMap.Title = source.Title;
        toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");
        toMap.StartDate = moment(source.StartDate).format("DD/MM/YYYY");
        toMap.EndDate = source.EndDate;
        toMap.ContainerName = source.ContainerName;
        toMap.MasterRoleName = (!!source.Roles && !!source.Roles[0] && !!source.Roles[0].Role) ? source.Roles[0].Role.Name : null;
        toMap.ImageUrl = source.EndDate ? "../Docm/Images/DocmChiusura.gif" : "../Docm/Images/Pratica.gif";
        toMap.TooltipImageUrl = source.EndDate ? "Dossier chiuso" : "Dossier aperto";

        return toMap;
    }
}

export = DossierGridViewModelMapper;