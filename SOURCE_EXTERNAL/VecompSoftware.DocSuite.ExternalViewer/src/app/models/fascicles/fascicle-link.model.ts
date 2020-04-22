import { IdentifierModel } from '../identifier.model';
import { FascicleModel } from './fascicle.model';


export interface FascicleLinkModel extends IdentifierModel {

    fascicle: FascicleModel;
    fascicleLinked: FascicleModel;
   
}