import { BaseServiceModel } from '../base-service.model';

export class AuthenticationModel implements BaseServiceModel {
    constructor() {
        this.serviceModelName = "AuthenticationModel";
    }

    accessToken: string;
    tokenType: string;
    expiresIn: number;
    userName: string;
    issued: string;
    expires: string;
    readonly serviceModelName: string;
}