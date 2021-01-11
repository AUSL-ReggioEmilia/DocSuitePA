import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { ProtocolModel } from '../../models/protocols/protocol.model'; 
import { ProtocolStatusType } from '../../models/protocols/protocol-status.type'; 
import { ProtocolType } from '../../models/protocols/protocol.type'; 
import { ProtocolSectorModel } from '../../models/protocols/protocol-sector.model'; 
import { ProtocolContactModel } from '../../models/protocols/protocol-contact.model'; 
import { CategoryModel } from '../../models/commons/category.model'; 
import { ContainerModel } from '../../models/commons/container.model'; 
import { CategoryMapper } from '../commons/category.mapper';
import { ContainerMapper } from '../commons/container.mapper';
import { ProtocolSectorMapper } from '../protocols/protocol-sector.mapper';
import { ProtocolUserMapper } from '../protocols/protocol-user.mapper';
import { ProtocolContactMapper } from '../protocols/protocol-contact.mapper';
import { DocumentModel } from '../../models/commons/document.model';
import { DocumentMapper } from '../commons/document.mapper';

@Injectable()
export class ProtocolMapper implements BaseMapper {

    constructor(private categoryMapper: CategoryMapper, private containerMapper: ContainerMapper, private protocolSectorMapper: ProtocolSectorMapper,
        private protocolContactMapper: ProtocolContactMapper, private documentMapper: DocumentMapper, private protocolUserMapper: ProtocolUserMapper) { }

    mapFromJson(json: any): ProtocolModel {

        if (!json) {
            return null;
        }

       let model: ProtocolModel = new ProtocolModel();

        model.id = json.Id;
        model.year = json.Year;
        model.number = json.Number;
        model.subject = json.Subject;
        model.note = json.Note;
        model.title = json.Title;
        model.location = json.Location;
        model.registrationUser = json.RegistrationUser;
        model.registrationDate = json.RegistrationDate;
        model.lastChangedUser = json.LastChangedUser;
        model.lastChangedDate = json.LastChangedDate;
        model.onlinePublication = json.OnlinePublication;
        model.type = ProtocolStatusType[json.ProtocolType as string];
        model.status = ProtocolStatusType[json.Status as string];
        model.annulmentReason = json.AnnulmentReason;
        model.addressee = json.Addressee;
        model.assignee = json.Assignee; 
        model.serviceCategory = json.ServiceCategory;

        model.category = json.Category ? this.categoryMapper.mapFromJson(json.Category) : null;
        model.container = json.Container ? this.containerMapper.mapFromJson(json.Container) : null;
        model.sectors = json.Sectors ? json.Sectors.map(sector => this.protocolSectorMapper.mapFromJson(sector)) : null;
        model.contacts = json.Contacts ? json.Contacts.map(contact => this.protocolContactMapper.mapFromJson(contact)) : null;
        model.documents = json.Documents ? json.Documents.map(document => this.documentMapper.mapFromJson(document)) : null;
        model.users = json.Users ? json.Users.map(user => this.protocolUserMapper.mapFromJson(user)) : null;

        return model;
    }

}