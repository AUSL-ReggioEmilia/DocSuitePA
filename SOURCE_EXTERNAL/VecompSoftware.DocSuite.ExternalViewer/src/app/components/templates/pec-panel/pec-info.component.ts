import { Component, Input } from '@angular/core';

import { PECMailModel } from '../../../models/pec-mails/pec-mail.model';

@Component({
    moduleId: module.id,
    selector: 'pec-info',
    templateUrl: 'pec-info.component.html',
    styleUrls: [ 'pec-info.component.css' ]
})

export class PecInfoComponent{
    @Input() item: PECMailModel;
}