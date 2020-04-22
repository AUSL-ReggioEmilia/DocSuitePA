import { IdentifierModel } from '../identifier.model';
import { LocationModel } from './location.model';

export class ContainerModel implements IdentifierModel{

    id: string;
    name: string;
    active: boolean;

}