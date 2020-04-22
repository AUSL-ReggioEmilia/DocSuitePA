import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Router, Event, NavigationEnd } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { trigger, state, style, animate } from '@angular/animations'

import { ResolutionService } from '../services/resolution.service';
import { BaseHelper } from '../helpers/base.helper';
import { ResolutionModel } from '../models/resolution.model';
import { ResolutionType } from '../models/resolution.type';
import { ODataResultModel } from '../models/odata-result.model'; 
import { SearchModel } from '../models/search.model'; 
import { AppConfigService } from '../services/app-config.service';
import { LoadingSpinnerService } from '../services/loading-spinner.service';

@Component({
    moduleId: module.id,
    selector: 'published-resolutions',
    templateUrl: 'published-resolutions.component.html',
    styleUrls: [
        'resolutions-results.component.css'
    ]
})
    
export class PublishedResolutionsComponent implements OnInit {
    private resultModel: ODataResultModel;
    private pageSize: number = AppConfigService.settings.gridItemsNumber;
    private skip: number = 0;
    private gridData: GridDataResult;
    private resolutionType: ResolutionType;
    private resolutionTypeVisibile: boolean;
    private model: SearchModel = new SearchModel();
    private totalCount: number;
    isAllowedModality: boolean;
    private isDeliberation: boolean;
    private documentHandlerUrl: string;
    private serviceColumnVisbile: boolean;
    private proposerColumnVisible: boolean;

    constructor(private resolutionService: ResolutionService, private router: Router, private toastr: ToastrService,
        vRef: ViewContainerRef, private baseHelper: BaseHelper, private spinnerService: LoadingSpinnerService) {
        router.events.subscribe((event: Event) => {
            this.urlChanging(event);
        })
    }

    ngOnInit() {
        if (!AppConfigService.settings.activeRoutes.publishedConsultation) {
            this.router.navigate(['error-page']);
            console.log('Nell\'URL è stata indicata una rotta inesistente.');
            this.isAllowedModality = false;
        }
        else {
            this.documentHandlerUrl = AppConfigService.settings.publishedDocumentHandlerUrl;
            this.isAllowedModality = true;
            this.proposerColumnVisible = AppConfigService.settings.publishedConsultationProposerColumnVisibility;
            this.serviceColumnVisbile = AppConfigService.settings.publishedConsultationServiceColumnVisibility;
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

    private successfulCallback(response: ODataResultModel) {
        this.spinnerService.stop();
        this.resultModel = response;
        this.resultModel.results.forEach((item: ResolutionModel) => {
            item.documentUrl = this.documentHandlerUrl.concat("?UniqueId=", item.id.toString());
        });
        this.gridData = {
            data: this.resultModel.results,
            total: this.totalCount
        }
    }

    private successfulCountCallback(response: number) {
        this.totalCount = response;
    }

    private errorCallback = (error): void => {
        this.spinnerService.stop();
        this.toastr.clear();
        this.toastr.error('Errore nel recupero dei dati.', 'Errore!', { timeOut: AppConfigService.settings.toastLife });
        console.log('Errore nel component resolution-grid:'.concat(error));
    }

    protected pageChange(event: PageChangeEvent): void {
        this.skip = event.skip;
        this.loadResolutions(this.resolutionType);
    }

    private loadResolutions(type: ResolutionType): void {
        let year: string = '';
        let number: string = '';
        let subject: string = '';
        let adoptionDate: string = '';
        let proposer: string = '';
        if (!!this.model) {
            if (!!this.model.year) {
                year = this.model.year;
            }
            if (!!this.model.number) {
                number = this.model.number;
            }
            if (!!this.model.subject) {
                subject = this.model.subject;
            }
            if (!!this.model.adoptionDate) {
                adoptionDate = this.baseHelper.dateConverter(this.model.adoptionDate);
            }
            if (!!this.model.proposer) {
                proposer = this.model.proposer;
            }
        }
        this.resolutionService.getPublishedResolutions(type, this.skip, this.pageSize, year, number, subject, adoptionDate, proposer).subscribe(response => this.successfulCallback(response), this.errorCallback);       
    }

    private countResolutions(type: ResolutionType): void {
        let year: string = '';
        let number: string = '';
        let subject: string = '';
        let adoptionDate: string = '';
        let proposer: string = '';
        if (!!this.model) {
            if (!!this.model.year) {
                year = this.model.year;
            }
            if (!!this.model.number) {
                number = this.model.number;
            }
            if (!!this.model.subject) {
                subject = this.model.subject;
            }
            if (!!this.model.adoptionDate) {
                adoptionDate = this.baseHelper.dateConverter(this.model.adoptionDate);
            }
            if (!!this.model.proposer) {
                proposer = this.model.proposer;
            }
        }
        this.resolutionService.countPublishedResolutions(type, this.skip, this.pageSize, year, number, subject, adoptionDate, proposer).subscribe(response => this.successfulCountCallback(response), this.errorCallback);
    }

    private setDataGrid() {
        this.skip = 0;
        this.pageSize = AppConfigService.settings.gridItemsNumber;
        this.countResolutions(this.resolutionType);
        this.loadResolutions(this.resolutionType);
    }

    resolutionTypeSelected(type) {
        this.spinnerService.start();
        this.resolutionType = type as ResolutionType;
        this.setDataGrid();      
    }

    onSubmitSearch(value: SearchModel) {
        if (!!this.model && !!this.model.year) {
            let convertedYear: number = +this.model.year;
            if (!convertedYear || this.model.year.length > 4) {
                alert('Valore non consentito nel campo Anno.');
                return;
            }
        }
        this.model = value;
        this.setDataGrid();
    }

    onPanelChange(event: any) {
        if (!this.model) {
            this.model = new SearchModel();
        }
    }
 

}