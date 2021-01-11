import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';

import { AuthService } from '@app-services/auth/auth.service';
import { ConfigurationService } from '@app-services/shared/configuration.service';
import { IdpType } from '@app-models/idptype.model';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

    hasError: boolean = false;
    isFedERa: boolean = false;
    isSPID: boolean = false;
    applicationName: string;

    constructor(private route: ActivatedRoute, private configurationService: ConfigurationService,
        private authService: AuthService, private router: Router) {
        this.applicationName = configurationService.config.ApplicationName;
        if (!this.authService.isTokenExpired()) {
            this.router.navigate(['/home']);
        }
    }

    ngOnInit() {        
        let authFailedParam: Params | null = this.route.snapshot.queryParams["authFailed"];
        if (authFailedParam) {
            this.hasError = true;
        }

        if (this.configurationService.config) {
            switch (this.configurationService.config.IdpType) {
                case IdpType.SPID:
                    this.isSPID = true;
                    break;
                case IdpType.FedERa:
                    this.isFedERa = true;
                    break;
            }
        }
    }    
}
