import { Injectable } from '@angular/core';

const arubaImage: string = 'images/spid/spid-idp-arubaid.png';
const infocertImage: string = 'images/spid/spid-idp-infocertid.png';
const namirialImage: string = 'images/spid/spid-idp-namirialid.png';
const posteImage: string = 'images/spid/spid-idp-posteid.png';
const sielteImage: string = 'images/spid/spid-idp-sielteid.png';
const registerImage: string = 'images/spid/spid-idp-spiditalia.png';
const timImage: string = 'images/spid/spid-idp-timid.png';

@Injectable()
export class IdpImageHelper{
    private readonly spidImages: { [key: string]: any } = {
        'idp_aruba': arubaImage,
        'idp_infocert': infocertImage,
        'idp_namirial': namirialImage,
        'idp_poste': posteImage,
        'idp_sielte': sielteImage,
        'idp_register': registerImage,
        'idp_tim': timImage
    }

    get arubaImage(): string {
        return this.getSpidIcon('idp_aruba');
    }

    get infocertImage(): string {
        return this.getSpidIcon('idp_infocert');
    }

    get namirialImage(): string {
        return this.getSpidIcon('idp_namirial');
    }

    get posteImage(): string {
        return this.getSpidIcon('idp_poste');
    }

    get sielteImage(): string {
        return this.getSpidIcon('idp_sielte');
    }

    get registerImage(): string {
        return this.getSpidIcon('idp_register');
    }

    get timImage(): string {
        return this.getSpidIcon('idp_tim');
    }

    private getSpidIcon(idp: string): string {
        return this.spidImages[idp];
    }
}