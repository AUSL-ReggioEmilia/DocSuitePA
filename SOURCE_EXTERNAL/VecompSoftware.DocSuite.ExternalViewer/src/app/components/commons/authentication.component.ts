import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { AuthenticationService } from '../../services/commons/authentication.service';
import { ResponseModel } from '../../models/commons/response.model';
import { AuthenticationModel } from '../../models/commons/authentication.model';
import { ErrorLogModel } from '../../models/commons/error-log.model';


@Component({
    moduleId: module.id,
    template: `<loading-spinner></loading-spinner>`
})


export class AuthenticationComponent implements OnInit {

    url: string;

    constructor(private authenticationService: AuthenticationService, private route: ActivatedRoute, private router: Router) { }

    ngOnInit() {
        this.route.params.subscribe(
            (param: any) => {
                let appId: string = param['appId'];
                let kind: string = param['kind'];
                let parameters: string = param['param'];
                let authrule: string = param['authrule'];
                this.url = param['url'];
               
                this.authenticationService.postAuthToken(appId, kind, parameters, authrule).subscribe(response => this.successCallback(response), this.errorCallback);
            }
        );
    }

    successCallback(response: ResponseModel<AuthenticationModel>) {
        if (response && response.results.length > 0) {
            let authModel: AuthenticationModel = response.results[0];
            sessionStorage.setItem('access_token', authModel.accessToken);
            window.location.href = this.url;
        }
    }

    errorCallback = (error): void => {
        console.log(error);
        let errorMessage: string;
        if (error instanceof ErrorLogModel) {
            let errorLog = error;
            if (errorLog.status) {
                errorMessage = errorLog.status.toString().concat(' - ');
            }
            errorMessage = errorMessage.concat(errorLog.errorMessages[0]);

            let extras: NavigationExtras = {
                queryParams: {
                    "status": error.status
                }
            };

            this.router.navigate(['error-page'], extras);
        }
        else {
            this.router.navigate(['error-page']);
        }        
    }
}