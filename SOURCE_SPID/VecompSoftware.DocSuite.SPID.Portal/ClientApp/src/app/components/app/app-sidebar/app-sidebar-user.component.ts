import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '@app-services/auth/auth.service';
import { UserService } from '@app-services/shared/user.service';

@Component({
    selector: 'app-sidebar-user',
    templateUrl: './app-sidebar-user.component.html'
})
export class AppSidebarUserComponent implements OnInit {

    userLoggedDescription: string;

    constructor(private router: Router, private authService: AuthService,
        private userService: UserService) { }

    ngOnInit() {
        this.userLoggedDescription = this.userService.getUserDescription();
    }

    logOut() {
        this.authService.logOut();
    }

}
