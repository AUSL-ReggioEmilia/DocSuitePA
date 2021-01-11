import { Component, Input, OnInit } from '@angular/core';

import { PECMailModel } from '../../../models/pec-mails/pec-mail.model';

@Component({
    moduleId: module.id,
    selector: 'pec-trace',
    templateUrl: 'pec-trace.component.html'
})

export class PecTraceComponent implements OnInit {
    @Input() receipt: PECMailModel;
    iconUrl: string;

    ngOnInit() {
        this.iconUrl = 'app/images/pec/pec-errore-consegna.gif';
        switch (this.receipt.step) {
            case 'accettazione':
                this.iconUrl = 'app/images/pec/pec-accettazione.gif';
                break;
            case 'avvenuta-consegna':
                this.iconUrl = 'app/images/pec/pec-avvenuta-consegna.gif';
                break;
            case 'preavviso-errore-consegna':
                this.iconUrl = 'app/images/pec/pec-preavviso-errore-consegna.gif';
                break;
            case 'errore-consegna':
            case 'non-accettazione':
                this.iconUrl = 'app/images/pec/pec-errore-consegna.gif';
                break;
        }
    }
}