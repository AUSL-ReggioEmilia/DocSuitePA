import IFascicleService = require('App/Services/Fascicles/IFascicleService');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import NotLinkedFasciclesModel = require('App/Models/Fascicles/NotLinkedFasciclesModel');
import Guid = require('App/Helpers/GuidHelper');
import UpdateActionType = require("App/Models/UpdateActionType");

class FascicleLocalService implements IFascicleService {
    insertFascicle(model: FascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        model.UniqueId = Guid.newGuid();
        if (callback) {
            callback(model);
        }
    }
    updateFascicle(model: FascicleModel,actiontype?:UpdateActionType ,callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        throw new Error("Method not implemented.");
    }
    closeFascicle(model: FascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        throw new Error("Method not implemented.");
    }
    getFascicle(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        throw new Error("Method not implemented.");
    }
    getAvailableFascicles(uniqueId: string, name: string, topElement: string, skipElement: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        throw new Error("Method not implemented.");
    }
    getAssociatedFascicles(uniqueId: string, environment: number, qs: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        throw new Error("Method not implemented.");
    }
    getNotLinkedFascicles(model: NotLinkedFasciclesModel, name: string, topElement: string, skipElement: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        throw new Error("Method not implemented.");
    }
    getLinkedFascicles(model: FascicleModel, qs: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        throw new Error("Method not implemented.");
    }
    getFascicleByCategory(idCategory: number, name: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        throw new Error("Method not implemented.");
    }
}

export = FascicleLocalService;