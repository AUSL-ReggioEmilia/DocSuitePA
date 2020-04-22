import TransparentAdministrationMonitorLogGridViewModel = require('App/ViewModels/Monitors/TransparentAdministrationMonitorLogGridViewModel');
import IMapper = require('App/Mappers/IMapper');

class TransparentAdministrationMonitorLogGridViewModelMapper implements IMapper<TransparentAdministrationMonitorLogGridViewModel>{
    constructor() {
    }
    public Map(source: any): TransparentAdministrationMonitorLogGridViewModel {
        let toMap: TransparentAdministrationMonitorLogGridViewModel = <TransparentAdministrationMonitorLogGridViewModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Date = moment(source.Date).format("DD/MM/YYYY");
        toMap.Note = source.Note;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.Rating = source.Rating.split('|').join(',');
        toMap.DocumentUnitName = source.DocumentUnitName;
        toMap.IdDocumentUnit = source.IdDocumentUnit;
        toMap.DocumentUnitTitle = source.DocumentUnitTitle;
        toMap.DocumentUnitSummaryUrl = `<a href="../Series/Item.aspx?Type=Series&UniqueId=${source.IdDocumentUnit}&Action=2&PreviousPage=${location.href}">${source.DocumentUnitTitle}</a>`;
        toMap.IdRole = source.IdRole;
        toMap.RoleName = source.RoleName;
        return toMap;
    }
}

export = TransparentAdministrationMonitorLogGridViewModelMapper;