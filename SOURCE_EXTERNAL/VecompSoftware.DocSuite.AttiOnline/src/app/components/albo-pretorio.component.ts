import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Router, Event, NavigationEnd } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { trigger, state, style, animate } from '@angular/animations'
import { GroupDescriptor, process } from '@progress/kendo-data-query';
import { DomSanitizer } from '@angular/platform-browser';

import { ResolutionModel } from '../models/resolution.model';
import { ResolutionType } from '../models/resolution.type';
import { BaseHelper } from '../helpers/base.helper';
import { ResolutionService } from '../services/resolution.service';
import { AppConfigService } from '../services/app-config.service';
import { ODataResultModel } from '../models/odata-result.model'; 
import { LoadingSpinnerService } from '../services/loading-spinner.service';

@Component({
    moduleId: module.id,
    selector: 'albo-pretorio',
    templateUrl: 'albo-pretorio.component.html',
    styleUrls: [
        'albo-pretorio.component.css'
    ]
})

export class AlboPretorioComponent implements OnInit {
    private resultModel: ODataResultModel;
    private gridData: GridDataResult;
    private resolutionType: ResolutionType;
    isAllowedModality: boolean;
    private resolutionTypeVisibile: boolean;
    private documentHandlerUrl: string;
    private proposerColumnVisible: boolean;
    private serviceColumnVisbile: boolean;
    private sorting: string;
    private groupable: boolean;
    private groups: GroupDescriptor[];
    private introductionVisible: boolean = true;
    private headerUrl;
    private footerUrl;
    private asmnAlboUrl;
    private asmnAlboUrlText;
    private tooltipBackHome: string = "";
    private gridServices: GridDataResult;
    private services;

    constructor(private resolutionService: ResolutionService, private router: Router, private toastr: ToastrService,
        vRef: ViewContainerRef, private baseHelper: BaseHelper, private spinnerService: LoadingSpinnerService, private domSanitizer: DomSanitizer) {
        router.events.subscribe((event: Event) => {
           this.urlChanging(event);
        })
    }

    ngOnInit(){
        if (!AppConfigService.settings.activeRoutes.alboPretorio) {
            this.router.navigate(['error-page']);
            console.log('Nell\'URL è stata indicata una rotta inesistente.');
            this.isAllowedModality = false;
        }
        else {
            this.documentHandlerUrl = AppConfigService.settings.alboPretorioDocumentHandlerUrl;
            this.isAllowedModality = true;
            this.proposerColumnVisible = AppConfigService.settings.alboPretorioProposerColumnVisibility;
            this.serviceColumnVisbile = AppConfigService.settings.alboPretorioServiceColumnVisibility;
            this.headerUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(AppConfigService.settings.headerUrl);
            this.footerUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(AppConfigService.settings.footerUrl);
            this.asmnAlboUrlText = AppConfigService.settings.ASMNAlboUrl;
            this.asmnAlboUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(AppConfigService.settings.ASMNAlboUrl);  
            this.services = [
                {
                    code: "DS",
                    name: "DIRETTORE SANITARIO"
                },
                {
                    code: "DA",
                    name: "DIRETTORE AMMINISTRATIVO"
                },
                {
                    code: "CS",
                    name: "DIRETTORE DELLE ATTIVITA’ SOCIO SANITARIE"
                },
                {
                    code: "DIR.OP.AVEN",
                    name: "DIREZIONE OPERATIVA AVEN"
                },
                {
                    code: "RPH",
                    name: "PRESIDIO OSPEDALIERO"
                },
                {
                    code: "R.SRU",
                    name: "FUNZIONE STAFF RISORSE UMANE"
                },
                {
                    code: "R.SC",
                    name: "FUNZIONE STAFF COMUNICAZIONE"
                },
                {
                    code: "R.DIPF",
                    name: "DIPARTIMENTO FARMACEUTICO"
                },
                {
                    code: "DSMDP",
                    name: "DIPARTIMENTO SALUTE MENTALE E DIPENDENZE PATOLOGICHE"
                },
                {
                    code: "DSP",
                    name: "DIPARTIMENTO SANITA’ PUBBLICA"
                },
                {
                    code: "P.C.P.",
                    name: "PROGRAMMA CURE PRIMARIE"
                },
                {
                    code: "R.AGE",
                    name: "SERVIZIO AFFARI GENERALI"
                },
                {
                    code: "R.SILA",
                    name: "SERVIZIO INTERAZIENDALE ATTIVITA' LEGALI E ASSICURATIVE"
                },
                {
                    code: "R.SLA",
                    name: "SERVIZIO LEGALE E DELLE ASSICURAZIONI"
                },
                {
                    code: "R.APP",
                    name: "SERVIZIO APPROVVIGIONAMENTI"
                },
                {
                    code: "RSTP",
                    name: "SERVIZIO GESTIONE SERVIZI TECNICI E PATRIMONIO"
                },
                {
                    code: "R.GEP",
                    name: "SERVIZIO GESTIONE ECONOMICA DEL PERSONALE"
                },
                {
                    code: "R.GGP",
                    name: "SERVIZIO GESTIONE GIURIDICA DEL PERSONALE"
                },
                {
                    code: "R.LA",
                    name: "SERVIZIO LOGISTICO ALBERGHIERO"
                },
                {
                    code: "REF",
                    name: "SERVIZIO GESTIONE RISORSE ECONOMICHE E FINANZIARIE"
                },
                {
                    code: "MON",
                    name: "Distretto Montecchio Emilia"
                },
                {
                    code: "RE",
                    name: "Distretto Reggio Emilia"
                },
                {
                    code: "GUA",
                    name: "Distretto Guastalla"
                },
                {
                    code: "COR",
                    name: "Distretto Correggio"
                },
                {
                    code: "SCA",
                    name: "Distretto Scandiano"
                },
                {
                    code: "CM",
                    name: "Distretto Castelnovo Monti"
                }
            ];
            this.gridServices = {
                data: this.services,
                total: null
            }
        }
    }

    private urlChanging(value: Event) {
        let url: string = this.router.url;
        this.gridData = undefined;
        this.resolutionTypeVisibile = true;
        this.resolutionType = null;
        if (value instanceof NavigationEnd) {
            if (url.includes('Delibere')) {
                this.resolutionTypeSelected(1);
                this.resolutionTypeVisibile = false;
            }
            else if (url.includes('Determine')) {
                this.resolutionTypeSelected(0);
                this.resolutionTypeVisibile = false;
            }
        }
    }

    resolutionTypeSelected(type) {
        this.groups = undefined;
        this.spinnerService.start();
        this.resolutionType = type as ResolutionType;
        this.tooltipBackHome = "clicca qui per tornare all'inizio";
        this.groupable = false;
        this.sorting = AppConfigService.settings.deliberePublicationDateSorting;
        if (this.resolutionType == ResolutionType.Atto) {
            this.groupable = AppConfigService.settings.attiGroupableGridEnabled;
            this.sorting = AppConfigService.settings.attiPublicationDateSorting;
        }
        this.loadResolutions(this.resolutionType);
    }

    backClicked(event:any){
        this.introductionVisible = true;
        this.resolutionType = null;
        this.tooltipBackHome = "";
    }

    private loadResolutions(type: ResolutionType): void {
        this.resolutionService.getOnlinePublishedResolutions(type, this.sorting).subscribe(response => this.successfulCallback(response), this.errorCallback);
    }

    private successfulCallback(response: ODataResultModel) {
        this.introductionVisible = false;
        this.resultModel = response;
        this.resultModel.results.forEach((item: ResolutionModel) => {
            item.documentUrl = this.documentHandlerUrl.concat("?UniqueId=", item.id.toString());
            item.serviceName = this.getServiceName(item.serviceCode);
            let name: string = item.serviceCode;
            if (!!item.serviceName) {
                name = name.concat(' (', item.serviceName, ')');
            }
            item.service = name;
        });
        if (this.groupable) {
            this.groups = [{ field: "service" }];
            let total: number = this.resultModel.results.length
            let groupedData: GridDataResult = process(this.resultModel.results, { group: this.groups });
            this.gridData = {
                data: groupedData.data,
                total: total
            }
        }
        else {
            this.gridData = {
                data: this.resultModel.results,
                total: this.resultModel.results.length
            }
        }
        this.spinnerService.stop();
    }

    private errorCallback = (error): void => {
        this.introductionVisible = false;
        this.spinnerService.stop();
        this.toastr.clear();
        this.toastr.error('Errore nel recupero dei dati.', 'Errore!', { timeOut: AppConfigService.settings.toastLife });
        console.log('Errore nel component resolution-grid: '.concat(error));
    }

    private getServiceName(code: string): string {
        if (!!code) {
            let service = this.services.filter((item) => {
                if (item.code === code) {
                    return item;
                }
            })[0];
            return !!service ? service.name : '';
        }
        return '';
    }
}