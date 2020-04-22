import { Injectable } from '@angular/core'; 

import { ContactType } from '../models/commons/contact.type'; 
import { Environment } from '../models/environment';

@Injectable()
export class BaseHelper {

    isGuid = (value: any): boolean => {
        let regex = /[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i;
        let match: RegExpExecArray = regex.exec(value);
        return !!match;
    }

    setDocumentUnitImageUrl = (documentUnitType: number): string => {
        if (!documentUnitType) {
            return '';
        }

        switch (true) {
            case (documentUnitType == Environment.Protocol):
                return 'app/images/document-units/protocol.gif';
            case (documentUnitType == Environment.Resolution):
                return 'app/images/document-units/resolution.gif';
            case (documentUnitType == Environment.DocumentSeries):
            case (documentUnitType > 100):
                return 'app/images/document-units/document-series.png';
        }
    }


    setImageUrl = (fileName: string): string => {

        if (!fileName) {
            return '';
        }

        switch (true) {
            case this.matchExtension(fileName, 'p7m'):
            case this.matchExtension(fileName, 'm7m'):
            case this.matchExtension(fileName, 'p7x'):
            case this.matchExtension(fileName, 'p7s'):
            case this.matchExtension(fileName, 'tds'):
                return 'app/images/documents/document-signed-gold.png';
            case this.matchExtension(fileName, 'pdf'):
                return 'app/images/documents/file-extension-pdf.png';
            case this.matchExtension(fileName, 'doc'):
            case this.matchExtension(fileName, 'docx'):
            case this.matchExtension(fileName, 'odt'):
                return 'app/images/documents/file-extension-doc.png';
            case this.matchExtension(fileName, 'xls'):
            case this.matchExtension(fileName, 'xlsx'):
                return 'app/images/documents/file-extension-xls.png';
            case this.matchExtension(fileName, 'txt'):
                return 'app/images/documents/file-extension-txt.png';
            case this.matchExtension(fileName, 'zip'):
                return 'app/images/documents/file-extension-zip.png';
            case this.matchExtension(fileName, 'rar'):
                return 'app/images/documents/file-extension-rar.png';
            case this.matchExtension(fileName, 'msg'):
            case this.matchExtension(fileName, 'eml'):
                return 'app/images/documents/file-extension-eml.png';
        }
    }

    setContactIconUrl = (contactType: ContactType): string => {

        if (!contactType) {
            return '';
        }

        switch (contactType) {
            case ContactType.Administration:
                return 'app/images/contacts/administration.gif';
            case ContactType.Group:
                return 'app/images/contacts/group.gif';
            case ContactType.Sector:
                return 'app/images/contacts/sector.gif';
            case ContactType.AOO:
                return 'app/images/contacts/aoo.gif';;
            case ContactType.AO:
                return 'app/images/contacts/uo.gif';
            case ContactType.Role:
                return 'app/images/contacts/role.gif';
            case ContactType.Citizen:
                return 'app/images/contacts/person.gif';
            case ContactType.AOOManual:
            case ContactType.CitizenManual:
                return 'app/images/contacts/manual.gif';
            case ContactType.IPA:
                return 'app/images/contacts/building.gif';
        }
    }

    matchExtension(fileName: string, extension: string): boolean {
        return fileName.endsWith(extension);
    }
}