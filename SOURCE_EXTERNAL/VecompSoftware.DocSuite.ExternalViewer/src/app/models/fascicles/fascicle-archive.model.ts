import { IdentifierModel } from '../identifier.model';
//import { ArchiveRepositoryModel } from '../archives/archive-repository.model';
import { FascicleReferenceType } from './fascicle-reference.type';
import { FascicleModel } from './fascicle.model';

export interface FascicleArchiveModel extends IdentifierModel{

    referenceType: FascicleReferenceType;
    archiveId: string;

    fascicle: FascicleModel;
    //archiveRepository: ArchiveRepositoryModel;
}