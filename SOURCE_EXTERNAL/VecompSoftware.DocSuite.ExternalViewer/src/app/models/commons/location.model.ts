import { IdentifierModel } from '../identifier.model';

export interface LocationModel extends IdentifierModel{

    name: string;
    server: number;

}