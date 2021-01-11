import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot, NavigationExtras } from '@angular/router';
import { DossierModel } from '../../models/dossiers/dossier.model';
import { DossierService } from '../../services/dossiers/dossier.service';
import { ToastrService } from 'ngx-toastr';
import { BaseHelper } from '../../helpers/base.helper';
import { LoadingSpinnerService } from '../../services/commons/loading-spinner.service';
import { AppConfigService } from '../../services/commons/app-config.service';
import { catchError } from 'rxjs/operators';
import { ErrorLogModel } from '../../models/commons/error-log.model';

@Injectable()
export class DossierResolve implements Resolve<DossierModel> {

    constructor(private dossierService: DossierService, private router: Router,
        private baseHelper: BaseHelper, private toastr: ToastrService,
        private spinnerService: LoadingSpinnerService) { }

    resolve(route: ActivatedRouteSnapshot): any {

        let id: string = route.params['id'];
        let year: string = route.params['year'];
        let number: string = route.params['number'];

        if (id || (id && this.baseHelper.isGuid(id))) {
            return this.dossierService.getDossierById(id)
                .pipe(catchError(this.handleException));
        }

        else if (year && number) {
            return this.dossierService.getDossierByYearAndNumber(+year, +number)
                .pipe(catchError(this.handleException));
        }

        if (!id || (id && !this.baseHelper.isGuid(id)) || !year || !number) {
            this.toastr.info('Nell\'URL sono presenti dei parametri non validi.', 'Attenzione!', { timeOut: AppConfigService.settings.toastLife });
            this.router.navigate(['error-page']);
            return false;
        }

    }

    handleException = (error: any): any => {
        this.toastr.clear();
        this.spinnerService.stop();
        let errorMessage: string;
        if (error instanceof ErrorLogModel) {
            let errorLog = error;
            errorMessage = errorLog.status.toString().concat(' - ', errorLog.errorMessages[0]);

            let extras: NavigationExtras = {
                queryParams: {
                    "status": error.status
                }
            };

            this.router.navigate(['error-page'], extras);
        }
        else {
            errorMessage = error;
        }
        this.toastr.error(errorMessage, "Attenzione!", { timeOut: AppConfigService.settings.toastLife });
    }
}