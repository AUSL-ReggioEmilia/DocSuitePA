import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

import { AppConfigService } from '../../services/commons/app-config.service';
import { LoadingSpinnerService } from '../../services/commons/loading-spinner.service';
import { PDFDocumentProxy } from 'pdfjs-dist';
import { PdfViewerComponent } from 'ng2-pdf-viewer';
import saveAs from 'save-as'

@Component({
    moduleId: module.id,
    templateUrl: 'document.component.html',
    styleUrls: ['document.component.css', '../commons/loading.component.css']
})


export class DocumentComponent implements OnInit {

    source: any;
    id: string;
    number: string;
    loading: boolean = true;
    pageNumber: number;
    totalPages: number;
    isFirstPage: boolean;
    isLastPage: boolean;
    pdfQuery = '';
    isPdfToolbarHidden: boolean;

    @ViewChild(PdfViewerComponent)
    private pdfComponent: PdfViewerComponent;

    constructor(private toastr: ToastrService, private route: ActivatedRoute,
        private domSanitizer: DomSanitizer, private spinnerService: LoadingSpinnerService) {
        this.isPdfToolbarHidden = true;
        this.spinnerService.start();
    };

    ngOnInit() {
        this.spinnerService.start()
        this.route.params.subscribe(
            (param: any) => {
                this.id = param['id'];

                if (!this.id) {
                    this.toastr.info('Nell\'URL indicare un identificativo valido.', 'Attenzione!', { timeOut: AppConfigService.settings.toastLife });
                    this.spinnerService.stop();
                    return false;
                }

                let handlerUrl: string = AppConfigService.settings.documentHandler
                    .concat('?Id=', this.id);
                this.source = this.domSanitizer.bypassSecurityTrustResourceUrl(handlerUrl);

                this.initializeToolbar();
                this.loading = false;

            }
        );
    }

    initializeToolbar() {
        this.pageNumber = 1;
        this.isFirstPage = true;
        this.isLastPage = false;
    }

    afterPdfLoad(pdf: PDFDocumentProxy) {
        this.isPdfToolbarHidden = false;
        this.totalPages = pdf.numPages;
        this.spinnerService.stop();
    }

    goToFirstPage() {
        this.pageNumber = 1;
        this.isFirstPage = true;
        this.isLastPage = false;
    }

    goToPreviousPage() {
        this.pageNumber--;
        if (this.pageNumber === 1) {
            this.isFirstPage = true;
            this.isLastPage = false;
        }
    }

    goToNextPage() {
        this.pageNumber++;
        if (this.pageNumber === this.totalPages) {
            this.isFirstPage = false;
            this.isLastPage = true;
        }
    }

    goToLastPage() {
        this.pageNumber = this.totalPages;
        this.isFirstPage = false;
        this.isLastPage = true;
    }

    changePageNumber(event) {
        this.pageNumber = event.path[0].value;
    }

    searchQueryChanged(newQuery: string) {
        if (newQuery !== this.pdfQuery) {
            this.pdfQuery = newQuery;
            this.pdfComponent.pdfFindController.executeCommand('find', {
                query: this.pdfQuery,
                highlightAll: true
            });
        } else {
            this.pdfComponent.pdfFindController.executeCommand('findagain', {
                query: this.pdfQuery,
                highlightAll: true
            });
        }
    }

    download() {
        const xhr = new XMLHttpRequest();
        xhr.open('GET', this.source.changingThisBreaksApplicationSecurity, true);
        xhr.responseType = 'blob';

        xhr.onload = (e: any) => {
            if (xhr.status === 200) {
                const blob = new Blob([xhr.response], { type: 'application/pdf' });
                console.log(blob, xhr.response);
                var url = window.URL.createObjectURL(blob);
                saveAs(blob, `Documento - ${this.id}.pdf`);
            }
        };
        xhr.send();
    }

    pdfError(event) {
        console.log(event);
    }
}