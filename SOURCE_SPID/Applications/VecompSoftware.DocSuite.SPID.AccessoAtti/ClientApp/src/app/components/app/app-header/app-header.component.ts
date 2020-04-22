import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

import { UserModel } from '@app-models/_index';
import { UserService } from '@app-services/shared/user.service';

@Component({
    selector: 'app-header',
    templateUrl: './app-header.component.html',
    styleUrls: ['./app-header.component.css']
})
export class AppHeaderComponent implements OnInit {
    user: UserModel = <UserModel>{};

    constructor(private userService: UserService, @Inject(PLATFORM_ID) private platformId: Object) {
        if (isPlatformBrowser(this.platformId)) {
            let userToken: UserModel | null = this.userService.currentUser
            if (userToken) {
                this.user = userToken;
            }
        }
    }

    ngOnInit() {

    }
}
