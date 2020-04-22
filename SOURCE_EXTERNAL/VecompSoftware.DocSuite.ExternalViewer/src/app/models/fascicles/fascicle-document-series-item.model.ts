import { IdentifierModel } from '../identifier.model';
import { DocumentSeriesItemModel } from '../document-series/document-series-item.model';
import { FascicleReferenceType } from './fascicle-reference.type';
import { FascicleModel } from './fascicle.model';

export interface FascicleDocumentSeriesItemModel extends IdentifierModel{

    referenceType: FascicleReferenceType;

    fascicle: FascicleModel;
    documentSeriesItem: DocumentSeriesItemModel;
}