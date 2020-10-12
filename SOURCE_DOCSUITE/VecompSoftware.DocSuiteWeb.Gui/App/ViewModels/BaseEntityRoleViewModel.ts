import BaseEntityViewModel = require("./BaseEntityViewModel");

class BaseEntityRoleViewModel extends BaseEntityViewModel {
    IsMaster: boolean;
    IsActive: boolean;
}

export = BaseEntityRoleViewModel