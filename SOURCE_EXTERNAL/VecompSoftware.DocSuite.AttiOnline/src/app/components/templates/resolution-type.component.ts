import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { AppConfigService } from '../../services/app-config.service';

@Component({
    moduleId: module.id,
    selector: 'resolution-type',
    templateUrl: 'resolution-type.component.html'
})
export class ResolutionTypeComponent {
    @Input() resolutionType: number;
    @Output() resolutionTypeSelected: EventEmitter<any> = new EventEmitter<any>()
    delibereDescription: string = AppConfigService.settings.delibere;
    determineDescription: string = AppConfigService.settings.determine;

    typeSelected(event) {
        this.resolutionTypeSelected.emit(event);
    }

}