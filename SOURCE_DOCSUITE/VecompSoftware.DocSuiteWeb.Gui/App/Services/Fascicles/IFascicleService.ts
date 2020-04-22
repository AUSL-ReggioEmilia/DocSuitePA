import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import NotLinkedFasciclesModel = require('App/Models/Fascicles/NotLinkedFasciclesModel');
import UpdateActionType = require("App/Models/UpdateActionType");
interface IFascicleService {
    insertFascicle(model: FascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    updateFascicle(model: FascicleModel,actionType?:UpdateActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    closeFascicle(model: FascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    getFascicle(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    getAvailableFascicles(uniqueId: string,  name: string, topElement: string, skipElement: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    getAssociatedFascicles(uniqueId: string, environment: number, qs: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    getNotLinkedFascicles(model: NotLinkedFasciclesModel, name: string, topElement: string, skipElement: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    getLinkedFascicles(model: FascicleModel, qs: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
    getFascicleByCategory(idCategory: number, name: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;
}

export = IFascicleService;