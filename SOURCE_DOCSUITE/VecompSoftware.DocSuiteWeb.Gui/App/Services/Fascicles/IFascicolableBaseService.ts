import IFascicolableBaseModel = require('App/Models/Fascicles/IFascicolableBaseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicolableActionType = require("App/Models/FascicolableActionType");

interface IFascicolableBaseService<T extends IFascicolableBaseModel> {
    insertFascicleUD(model: T, actionType?: FascicolableActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    deleteFascicleUD(model: T, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
}

export = IFascicolableBaseService;