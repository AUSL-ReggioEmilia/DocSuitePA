import { Component, OnInit } from '@angular/core';

import { AuthService } from '@app-services/auth/auth.service';

const federaIcoCircleImage: string = 'images/federa/icon-federa.png';

@Component({
    selector: 'app-federa-button',
    templateUrl: './federa-button.component.html'
})
export class FederaButtonComponent implements OnInit {

    federaImage: string;

    constructor(private authService: AuthService) { 
        this.federaImage = federaIcoCircleImage;
    }

    ngOnInit() {
    }

    onClick(idp: string): void {
        this.authService.logIn(idp);
    }
}
