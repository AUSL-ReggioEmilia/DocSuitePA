import BuildActionType = require('App/Models/Commons/BuildActionType');
import ReferenceBuildModelType = require('App/Models/Commons/ReferenceBuildModelType');

interface BuildActionModel {
    BuildType: BuildActionType;
    Model: string;
    ReferenceId: string;
    ReferenceType: ReferenceBuildModelType;
}

export = BuildActionModel;