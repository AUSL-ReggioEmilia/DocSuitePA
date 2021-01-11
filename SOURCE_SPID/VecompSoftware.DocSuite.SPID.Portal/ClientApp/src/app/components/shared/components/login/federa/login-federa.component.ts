import { Component, OnInit } from '@angular/core';

const agidLogoRxImage: string = 'images/spid/spid-agid-logo-lb-rx.png';
const federaLogoImage: string = 'images/federa/logo-federa.png';

@Component({
    selector: 'app-login-federa',
    templateUrl: './login-federa.component.html'
})
export class LoginFederaComponent implements OnInit {

    agidImage: string;
    federaImage: string;

    constructor() {
        this.agidImage = agidLogoRxImage;
        this.federaImage = federaLogoImage;
    }

    ngOnInit() {
    }

}
