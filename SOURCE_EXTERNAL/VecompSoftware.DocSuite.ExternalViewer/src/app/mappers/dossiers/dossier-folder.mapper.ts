import { Injectable } from '@angular/core';
import { BaseMapper } from '../base.mapper';
import { DossierFolderModel } from '../../models/dossiers/dossier-folder.model';

@Injectable()
export class DossierFolderMapper implements BaseMapper {

    constructor() { }

    mapFromJson(json: any): DossierFolderModel {

        if (!json) {
            return null;
        }

        let model: DossierFolderModel = new DossierFolderModel();

        model.id = json.IdDossierFolder;
        model.name = json.Name;
        model.status = json.Status;
        model.jsonMetadata = json.JsonMetadata;
        model.idDossier = json.Dossier_IdDossier;
        model.idFascicle = json.Fascicle_IdFascicle;
        model.idCategory = json.Category_IdCategory;
        model.idRole = json.Role_IdRole;

        return model;
    }

}