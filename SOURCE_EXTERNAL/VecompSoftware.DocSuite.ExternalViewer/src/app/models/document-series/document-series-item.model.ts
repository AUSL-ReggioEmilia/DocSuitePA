import { IdentifierModel } from '../identifier.model';
import { BaseModel } from '../base.model';
import { DocumentUnitModel } from '../document-unit.model';
import { DocumentSeriesItemStatus } from './document-series-item-status';

export interface DocumentSeriesItemModel extends DocumentUnitModel, BaseModel, IdentifierModel {

    publicationDate?: Date;
    retireDate?: Date;
    status?: DocumentSeriesItemStatus;
    mainDocumentId: string;
    annexedId: string;
    unpublishedAnnexedId: string;




}