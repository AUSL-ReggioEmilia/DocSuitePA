import { Component, Input, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

import { PECMailModel } from '../../../models/pec-mails/pec-mail.model';
import { PECDirectionType } from '../../../models/pec-mails/pec-direction.type';
import { PECMailService } from '../../../services/pec-mails/pec-mail.service';
import { ResponseModel } from '../../../models/commons/response.model';
import { ErrorLogModel } from '../../../models/commons/error-log.model';
import { AppConfigService } from '../../../services/commons/app-config.service';

@Component({
    moduleId: module.id,
    selector: 'pec-panel',
    templateUrl: 'pec-panel.component.html',
    styleUrls: ['pec-panel.component.css']
})

export class PecPanelComponent implements OnInit {
    @Input() year: number;
    @Input() number: number;
    @Input() direction: PECDirectionType;
    @Input() title: string;
    collapseUrl: string;
    isOpen: boolean;
    pecMails: PECMailModel[];

    constructor(location: Location, private pecMailService: PECMailService, private toastr: ToastrService) {
        location.subscribe(value => this.setInitialConfiguration());
    }

    ngOnInit() { 
        this.setInitialConfiguration();
    }

    public collapsePecPanel(): void {
        if (this.isOpen) {
            this.collapseUrl = "app/images/collapse-down.png";
            this.isOpen = false;
        }
        else {
            this.getPECMails();
        }
    }

    setInitialConfiguration() {
        this.isOpen = false;
        this.collapseUrl = "app/images/collapse-down.png";
    }

    getPECMails() {
        this.pecMailService.getPECMails(this.year, this.number, this.direction).subscribe(response => this.successCallback(response), this.errorCallback);
    }

    successCallback(response: ResponseModel<PECMailModel>) {

        if (response && response.results && response.results.length > 0) {
            this.pecMails = response.results;
            this.collapseUrl = "app/images/collapse-up.png";
            this.isOpen = true;
        }
    }

    errorCallback = (error): void => {
        this.toastr.clear();
        this.isOpen = false;
        console.log(error);
        let errorMessage: any;
        if (error.constructor.name == 'ErrorLogModel') {
            let errorLog: ErrorLogModel = error as ErrorLogModel;
            errorMessage = errorLog.status.toString().concat(' - ', errorLog.errorMessages[0]);
        }
        else {
            errorMessage = error;
        }
        this.toastr.error(errorMessage, 'Errore!', { timeOut: AppConfigService.settings.toastLife });
    }
}