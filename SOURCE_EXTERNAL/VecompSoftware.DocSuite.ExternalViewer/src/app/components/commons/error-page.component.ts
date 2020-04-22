import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { LoadingSpinnerService } from '../../services/commons/loading-spinner.service';

@Component({
    moduleId: module.id,
    templateUrl: 'error-page.component.html'
})


export class ErrorPageComponent implements OnInit {

    status: number;
    title: string;
    message: string;

    constructor(private spinnerService: LoadingSpinnerService, private route: ActivatedRoute) {
        this.spinnerService.stop();

    }

    ngOnInit() {
        this.route.queryParams.subscribe(params => {
            let paramStatus: number = +params["status"];
            switch (paramStatus) {
                case 400:
                    this.status = paramStatus;
                    this.title = 'Richiesta non valida';
                    this.message = 'Errore nella richiesta.';
                    break;
                case 401:
                    this.status = paramStatus;
                    this.title = 'Autorizzazione Negata';
                    this.message = 'Utente non autorizzato a visualizzare il contenuto richiesto.';
                    break;
                case 500:
                    this.status = paramStatus;
                    this.title = 'Errore non previsto';
                    this.message = 'Errore interno del Server.';
                    break;
                default:
                    this.status = 404;
                    this.title = 'Pagina non trovata';
                    this.message = 'La pagina ricercata non è stata trovata. Controllare che l\'URL sia corretto e ricaricare la pagina.';
            }
        });

    }
}