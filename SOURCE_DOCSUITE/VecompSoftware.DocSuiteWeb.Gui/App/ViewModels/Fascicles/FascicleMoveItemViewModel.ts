import Environment = require("App/Models/Environment");

interface FascicleMoveItemViewModel {
    uniqueId: string;
    name: string;
    environment: Environment;
}

export = FascicleMoveItemViewModel;