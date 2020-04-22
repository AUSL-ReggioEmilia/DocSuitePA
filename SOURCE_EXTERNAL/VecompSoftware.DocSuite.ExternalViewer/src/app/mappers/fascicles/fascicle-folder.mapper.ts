import { Injectable } from '@angular/core';
import { BaseMapper } from '../base.mapper';
import { FascicleFolderModel } from '../../models/fascicles/fascicle-folder.model';
import { DocumentUnitMapper } from '../document-units/document-unit.mapper';

@Injectable()
export class FascicleFolderMapper implements BaseMapper {

    constructor(private documentUnitMapper: DocumentUnitMapper) { }

    mapFromJson(json: any): FascicleFolderModel {

        if (!json) {
            return null;
        }

        let model: FascicleFolderModel = new FascicleFolderModel();

        model.id = json.IdFascicleFolder;
        model.name = json.Name;
        model.status = json.Status;
        model.typology = json.Typology;
        model.idFascicle = json.Fascicle_IdFascicle;
        model.idCategory = json.Category_IdCategory;
        model.hasDocuments = json.HasDocuments;
        model.hasChildren = json.HasChildren;
        model.fascicleFolderLevel = json.FascicleFolderLevel;

        return model;
    }

}