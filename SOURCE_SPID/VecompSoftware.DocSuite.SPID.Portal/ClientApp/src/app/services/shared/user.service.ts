import { Injectable } from '@angular/core';

import { AuthService } from '@app-services/auth/auth.service';
import { JwtAuthHelper } from '@app-helpers/auth/jwt-auth.helper';

@Injectable()
export class UserService {

    constructor(private authService: AuthService, private jwtHelper: JwtAuthHelper) { }

    getUserDescription(): string {
        let token: string | null = this.authService.getToken();
        if (token) {
            let description: string | null = this.jwtHelper.getTokenSubject(token);
            return description;
        }
        return '';
    }

    getUserDescriptionWithFiscalCode(): string {
        let token: string | null = this.authService.getToken();
        if (token) {
            let description: string | null = this.jwtHelper.getTokenSubject(token);
            let fiscalCode: string | null = this.jwtHelper.getTokenFiscalCode(token);
            if (description) {
                return description.concat((fiscalCode) ? " (".concat(fiscalCode, ")") : "");
            }
        }
        return '';
    }
}
