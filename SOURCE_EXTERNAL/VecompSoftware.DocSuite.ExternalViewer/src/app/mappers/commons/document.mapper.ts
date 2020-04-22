import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { DocumentModel } from '../../models/commons/document.model'; 
import { DocumentType } from '../../models/commons/document.type'; 
import { BaseHelper } from '../../helpers/base.helper'; 

@Injectable()
export class DocumentMapper implements BaseMapper {

    constructor (private baseHelper: BaseHelper){ }

    mapFromJson(json: any): DocumentModel{

        if (!json) {
            return null;
        }

        let model: DocumentModel = new DocumentModel();
        model.id = json.Id;
        model.documentName = json.DocumentName;
        model.chainId = json.ChainId;
        model.archiveSection = json.ArchiveSection;
        model.documentType = DocumentType[json.DocumentType as string];
        model.imageUrl = this.baseHelper.setImageUrl(model.documentName);

        return model;
    }

}