import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRouteSnapshot, NavigationExtras } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

import { ResponseModel } from '../../models/commons/response.model';
import { ErrorLogModel } from '../../models/commons/error-log.model';
import { FascicleModel } from '../../models/fascicles/fascicle.model';
import { FascicleService } from '../fascicles/fascicle.service';
import { BaseHelper } from '../../helpers/base.helper'; 
import { AppConfigService } from '../../services/commons/app-config.service';
import { LoadingSpinnerService } from '../commons/loading-spinner.service';

@Injectable()
export class FascicleMenuResolve implements Resolve<ResponseModel<FascicleModel>> {

    constructor(private fascicleService: FascicleService, private router: Router,
        private baseHelper: BaseHelper, private toastr: ToastrService,
        private spinnerService: LoadingSpinnerService) { }

    //TODO: il resolve non si deve basare su un service specifico ma su quello generico. 
    //Bisogna creare un metodo che dall'url torni il service dell'ud.Vedi ReflectiveInjector.saveandcreate
    resolve(route: ActivatedRouteSnapshot): any {
        this.toastr.clear();
        let id: string = route.params['id'];
        let title: string = route.params['title'];

        if (id || (id && this.baseHelper.isGuid(id))) {
            return this.fascicleService.getFascicleSummary(id)
                .pipe(catchError(this.handleException));
        }

        else if (title) {
            return this.fascicleService.getFascicleSummaryByTitle(title)
                .pipe(catchError(this.handleException));
        }

        if (!id || (id && !this.baseHelper.isGuid(id)) || !title) {
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