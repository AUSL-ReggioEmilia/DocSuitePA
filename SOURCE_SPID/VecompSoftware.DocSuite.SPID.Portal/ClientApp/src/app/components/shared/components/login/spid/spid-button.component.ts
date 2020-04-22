import { Component, OnInit, Input } from '@angular/core';

import { IdpImageHelper } from '@app-helpers/common/idp-image.helper';
import { AuthService } from '@app-services/auth/auth.service';

const spidIcoCircleImage: string = 'images/spid/spid-ico-circle-bb.png';

@Component({
    selector: 'app-spid-button',
    templateUrl: './spid-button.component.html'
})
export class SpidButtonComponent implements OnInit {

    spidImage: string;
    idpImageHelper: IdpImageHelper;

    constructor(idpImageHelper: IdpImageHelper, private authService: AuthService) {
        this.spidImage = spidIcoCircleImage;
        this.idpImageHelper = idpImageHelper;
    }

    ngOnInit() {
    } 

    onClick(idp: string): void {
        this.authService.logIn(idp);
    }
}
