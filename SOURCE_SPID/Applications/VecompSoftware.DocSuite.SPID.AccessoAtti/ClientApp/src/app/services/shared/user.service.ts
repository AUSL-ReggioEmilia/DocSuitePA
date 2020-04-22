import { Injectable } from '@angular/core';
import { of } from 'rxjs';

import { UserModel } from "@app-models/_index";

export const USER_MODEL: string = 'user-model';

@Injectable()
export class UserService {
    private userModel: UserModel;

    constructor() { }

    loadUser(): Promise<UserModel | null> {
        return of(this.loadUserModel()).toPromise()
            .then(user => {
                if (user) {
                    this.userModel = user as UserModel;
                    return this.userModel;
                }
                return null;
            })
            .catch(err => { return Promise.reject(err) });
    }

    private loadUserModel(): UserModel | null {
        return JSON.parse(sessionStorage.getItem(USER_MODEL));
    }

    get currentUser(): UserModel {
        return this.userModel;
    }
}
