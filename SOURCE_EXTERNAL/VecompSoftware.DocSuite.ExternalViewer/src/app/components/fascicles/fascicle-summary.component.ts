import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FascicleModel } from '../../models/fascicles/fascicle.model';
import { FascicleType } from '../../models/fascicles/fascicle.type';
import { ResponseModel } from '../../models/commons/response.model';
import { TreeListModel } from '../../models/commons/tree-list.model';
import { ErrorLogModel } from '../../models/commons/error-log.model';
import { LoadingSpinnerService } from '../../services/commons/loading-spinner.service';
import { AppConfigService } from '../../services/commons/app-config.service';
import { DossierModel } from '../../models/dossiers/dossier.model';

@Component({
    moduleId: module.id,
    templateUrl: 'fascicle-summary.component.html',
    styleUrls: ['fascicle-summary.component.css']
})

export class FascicleSummaryComponent implements OnInit {

    response: ResponseModel<FascicleModel>;
    fascicle: FascicleModel;  
    fascicleType: string;
    referenceUser: string;
    fascicleContacts: TreeListModel[];
   
    constructor(private route: ActivatedRoute, private toastr: ToastrService, private spinnerService: LoadingSpinnerService) {
        
    }

    ngOnInit(): void {
        this.route.parent.data.subscribe(response => this.successCallback(response['responseModel']), this.errorCallback);          
    }

    successCallback(response: ResponseModel<FascicleModel>) {
        if (response.results[0] instanceof DossierModel) {
            this.route.data.subscribe(response => this.successCallback(response['responseModel']), this.errorCallback);
        }
        else {
            this.response = response;
            if (response && response.results[0] && response.results.length == 1) {
                this.fascicle = response.results[0];
                switch (this.fascicle.type) {
                    case FascicleType.Procedure:
                        this.fascicleType = 'Procedimento';
                        break;
                    case FascicleType.Period:
                        this.fascicleType = 'Periodico';
                        break;
                    case FascicleType.Activity:
                        this.fascicleType = 'Attività';
                        break;
                }

                if (this.fascicle.contacts.length > 0) {
                    this.referenceUser = this.fascicle.contacts[this.fascicle.contacts.length - 1].name;
                }
            }
            this.spinnerService.stop();
        }
    }

    errorCallback = (error): void => {
        //TODO: toast notification per eccezione critica
        this.toastr.clear();
        this.spinnerService.stop();
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