import ContainerPropertyType = require('App/Models/Commons/ContainerPropertyType');
import ContainerModel = require('App/Models/Commons/ContainerModel');

interface ContainerPropertyModel {
    UniqueId: string;
    Name: string;
    ContainerType: ContainerPropertyType;
    ValueInt: number;
    ValueDate: Date;
    ValueDouble: number;
    ValueString: string;
    ValueGuid: string;
    ValueBoolean: boolean;
    Container: ContainerModel;
}

export = ContainerPropertyModel;