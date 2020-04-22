import BuildActionType = require('App/Models/Commons/BuildActionType');

interface BuildActionModel {
    BuildType: BuildActionType;
    Model: string;
    ReferenceId: string;
}

export = BuildActionModel;