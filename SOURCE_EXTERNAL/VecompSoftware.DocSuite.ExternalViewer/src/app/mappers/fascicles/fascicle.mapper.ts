import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { FascicleModel } from '../../models/fascicles/fascicle.model'; 
import { FascicleType } from '../../models/fascicles/fascicle.type'; 
import { FascicleContactModel } from '../../models/fascicles/fascicle-contact.model'; 
import { FascicleContactMapper } from '../../mappers/fascicles/fascicle-contact.mapper'; 
import { CategoryModel } from '../../models/commons/category.model'; 
import { CategoryMapper } from '../commons/category.mapper';
import { DocumentModel } from '../../models/commons/document.model';
import { DocumentUnitMapper } from '../document-units/document-unit.mapper';
import { DocumentMapper } from '../commons/document.mapper';
import { FascicleFolderMapper } from './fascicle-folder.mapper';

@Injectable()
export class FascicleMapper implements BaseMapper {

    constructor(private categoryMapper: CategoryMapper, private fascicleContactMapper: FascicleContactMapper,
        private documentUnitMapper: DocumentUnitMapper, private documentMapper: DocumentMapper,
        private fascicleFolderMapper: FascicleFolderMapper) { }

    mapFromJson(json: any): FascicleModel {

        if (!json) {
            return null;
        }

       let model: FascicleModel = new FascicleModel();

        model.id = json.Id;
        model.year = json.Year;
        model.number = json.Number;
        model.subject = json.Subject;
        model.note = json.Note;
        model.title = json.Title;
        model.registrationUser = json.RegistrationUser;
        model.registrationDate = json.RegistrationDate;
        model.lastChangedUser = json.LastChangedUser;
        model.lastChangedDate = json.LastChangedDate;
        model.type = FascicleType[json.FascicleType as string];
        model.startDate = json.StartDate;
        model.endDate = json.EndDate;
        model.name = json.FascicleName;

        model.category = json.Category ? this.categoryMapper.mapFromJson(json.Category) : null;
        model.contacts = json.Contacts ? json.Contacts.map(contact => this.fascicleContactMapper.mapFromJson(contact)) : null;
        model.documentUnits = json.DocumentUnits ? json.DocumentUnits.map(documentUnit => this.documentUnitMapper.mapFromJson(documentUnit)) : null;
        model.documents = json.Documents ? json.Documents.map(document => this.documentMapper.mapFromJson(document)) : null;
        model.fascicleFolders = json.FascicleFolders ? json.FascicleFolders.map(fascicleFolder => this.fascicleFolderMapper.mapFromJson(fascicleFolder)) : null;

        return model;
    }

}