import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';


import { JwtAuthHelper } from '@app-helpers/auth/jwt-auth.helper';
import { LocalHttpParams } from '@app-shared/params/local-http.params';

export const TOKEN_NAME: string = 'jwt-application-token';
export const REFERENCE_TOKEN_CODE: string = 'reference-token-code';
export const IDP_NAME: string = 'idpName';

@Injectable()
export class AuthService {

    constructor(private jwtHelper: JwtAuthHelper, @Inject(PLATFORM_ID) private platformId: Object,
        @Inject('BASE_URL') private baseUrl: string, private http: HttpClient) { }

    getToken(): string | null {
        if (isPlatformBrowser(this.platformId)) {
            return localStorage.getItem(TOKEN_NAME);
        }
        return null;
    }

    setToken(token: string): void {
        if (isPlatformBrowser(this.platformId)) {
            return localStorage.setItem(TOKEN_NAME, token);
        }
    }

    getReferenceCode(): string | null {
        if (isPlatformBrowser(this.platformId)) {
            return localStorage.getItem(REFERENCE_TOKEN_CODE);
        }
        return null;
    }

    getIdpName(): string | null {
        if (isPlatformBrowser(this.platformId)) {
            return localStorage.getItem(IDP_NAME);
        }
        return null;
    }

    isTokenExpired(jwt?: string): boolean {
        const token: string | null = jwt || this.getToken();
        return token == null || this.jwtHelper.isTokenExpired(token);
    }

    logOut(): void {
        if (isPlatformBrowser(this.platformId)) {
            let idpName: string | null = this.getIdpName();
            let referenceCode: string | null = this.getReferenceCode();
            if (idpName && referenceCode) {
                localStorage.removeItem(IDP_NAME);
                localStorage.removeItem(TOKEN_NAME);
                localStorage.removeItem(REFERENCE_TOKEN_CODE);
                window.location.href = this.baseUrl.concat('Saml/LogOut?ReferenceCode=', referenceCode, '&IdpName=', idpName);
            }
        }
    }

    logIn(idp: string): void {
        if (isPlatformBrowser(this.platformId)) {
            window.location.href = this.baseUrl.concat('Saml/Auth?idp=', idp);
        }
    }

    refreshToken(): Observable<string | null> {
        let toRefreshToken: string | null = this.getToken();
        if (toRefreshToken) {
            return <Observable<string | null>>this.http.post(this.baseUrl.concat('Saml/Token'), JSON.stringify({
                TokenGrantType: 1,
                ReferenceCode: this.getReferenceCode(),
                Token: toRefreshToken
            }), { params: new LocalHttpParams() });
        }
        return of(null);
    }

    expireToken(): void {
        if (isPlatformBrowser(this.platformId)) {
            localStorage.removeItem(IDP_NAME);
            localStorage.removeItem(TOKEN_NAME);
            localStorage.removeItem(REFERENCE_TOKEN_CODE);
        }
    }
}