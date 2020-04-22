import { Injectable } from '@angular/core';
import { BaseMapper } from '../base.mapper';
import { DossierFolderMapper } from './dossier-folder.mapper';
import { DossierModel } from '../../models/dossiers/dossier.model';
import { DossierRoleMapper } from './dossier-role.mapper';

@Injectable()
export class DossierMapper implements BaseMapper {

    constructor(private dossierFolderMapper: DossierFolderMapper, private dossierRoleMapper: DossierRoleMapper) { }

    mapFromJson(json: any): DossierModel {

        if (!json) {
            return null;
        }

        let model: DossierModel = new DossierModel();

        model.uniqueId = json.UniqueId;
        model.year = json.Year;
        model.number = json.Number;
        model.subject = json.Subject;
        model.note = json.Note;
        model.title = json.Title;
        model.registrationUser = json.RegistrationUser;
        model.registrationDate = json.RegistrationDate;
        model.lastChangedUser = json.LastChangedUser;
        model.lastChangedDate = json.LastChangedDate;
        model.startDate = json.StartDate;
        model.endDate = json.EndDate;
        model.containerId = json.ContainerId;
        model.containerName = json.ContainerName;
        model.jsonMetadata = json.JsonMetadata;

        model.dossierFolders = json.DossierFolders ? json.DossierFolders.map(dossierFolder => this.dossierFolderMapper.mapFromJson(dossierFolder)) : null;
        model.dossierRoles = json.Roles ? json.Roles.map(role => this.dossierRoleMapper.mapFromJson(role)) : null;

        return model;
    }

}