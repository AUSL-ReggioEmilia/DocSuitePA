import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '@app-services/auth/auth.service';
import { UserService } from '@app-services/shared/user.service';

@Component({
    selector: 'app-header-user',
    templateUrl: './app-header-user.component.html'
})
export class AppHeaderUserComponent implements OnInit {

    userLoggedDescription: string;

    constructor(private router: Router, private authService: AuthService,
        private userService: UserService) { }

    ngOnInit() {
        this.userLoggedDescription = this.userService.getUserDescriptionWithFiscalCode();
    }

    logOut() {
        this.authService.logOut();
    }

}
