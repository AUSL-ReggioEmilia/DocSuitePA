import { IdentifierModel } from '../identifier.model';
import { ContainerModel } from '../commons/container.model'; 

export interface DocumentSeriesModel extends IdentifierModel {

    name: string;
    documentSeriesSpecification: string;

    container: ContainerModel;
}