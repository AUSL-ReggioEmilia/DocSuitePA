import CategoryFascicleModel = require('App/Models/Commons/CategoryFascicleModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import RoleModel = require('App/Models/Commons/RoleModel');

class CategoryFascicleRightModel {
    constructor() {
    }
    UniqueId: string;
    CategoryFascicle: CategoryFascicleModel;
    Role: RoleModel;
    Container: ContainerModel;
}
export = CategoryFascicleRightModel;


