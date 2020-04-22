import { Component, OnInit, Input } from '@angular/core';

const agidLogoRxImage: string = 'images/spid/spid-agid-logo-lb-rx.png';

@Component({
    selector: 'app-login-spid',
    templateUrl: './login-spid.component.html'
})
export class LoginSpidComponent implements OnInit {

    agidImage: string;

    constructor() {
        this.agidImage = agidLogoRxImage;
    }

    ngOnInit() {
    }
}
