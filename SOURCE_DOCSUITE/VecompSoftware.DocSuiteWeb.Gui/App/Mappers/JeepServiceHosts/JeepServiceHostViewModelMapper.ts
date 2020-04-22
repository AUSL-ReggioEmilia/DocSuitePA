import JeepServiceHostViewModel = require('App/ViewModels/JeepServiceHosts/JeepServiceHostViewModel');
import IMapper = require('App/Mappers/IMapper');

class JeepServiceHostViewModelMapper implements IMapper<JeepServiceHostViewModel> {
  constructor() {
  }
  public Map(source: JeepServiceHostViewModel): JeepServiceHostViewModel {
    let toMap: JeepServiceHostViewModel = <JeepServiceHostViewModel>{};

    if (!source) {
      return null;
    }

    toMap.UniqueId = source.UniqueId;
    toMap.Hostname = source.Hostname;
    toMap.IsActive = source.IsActive;
    toMap.IsDefault = source.IsDefault;
    toMap.RegistrationUser = source.RegistrationUser;
    toMap.RegistrationDate = source.RegistrationDate;

    return toMap;
  }
}

export = JeepServiceHostViewModelMapper;