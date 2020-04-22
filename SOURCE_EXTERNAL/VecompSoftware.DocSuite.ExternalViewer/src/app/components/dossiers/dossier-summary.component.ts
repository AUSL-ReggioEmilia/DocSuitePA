import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';
import { LoadingSpinnerService } from '../../services/commons/loading-spinner.service';
import { ErrorLogModel } from '../../models/commons/error-log.model';
import { AppConfigService } from '../../services/commons/app-config.service';
import { DossierModel } from '../../models/dossiers/dossier.model';
import { ResponseModel } from '../../models/commons/response.model';

@Component({
  selector: 'app-dossier-summary',
  templateUrl: './dossier-summary.component.html',
  styleUrls: ['./dossier-summary.component.css']
})
export class DossierSummaryComponent implements OnInit {

    dossier: DossierModel;
    dossierRoleName: string;

    constructor(private toastr: ToastrService, private route: ActivatedRoute,
        private spinnerService: LoadingSpinnerService) {
        this.spinnerService.stop();
    }

    ngOnInit() {
        this.route.parent.data.subscribe(response => this.dossierSuccessCallback(response['responseModel']), this.errorCallback);
    }

    dossierSuccessCallback(response: ResponseModel<DossierModel>) {
        this.dossier = response.results[0];
        this.dossierRoleName = this.dossier.dossierRoles[0].role.name;
    }

    errorCallback = (error): void => {
        //TODO: toast notification per eccezione critica
        this.toastr.clear();
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
