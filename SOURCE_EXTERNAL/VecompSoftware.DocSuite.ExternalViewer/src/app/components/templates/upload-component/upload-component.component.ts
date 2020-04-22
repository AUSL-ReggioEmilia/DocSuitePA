import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { LoadingSpinnerService } from '../../../../app/services/commons/loading-spinner.service';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../../../../app/services/commons/app-config.service';

@Component({
    selector: 'uploader',
    templateUrl: './upload-component.component.html',
    styleUrls: ['./upload-component.component.css']
})
export class UploadComponentComponent implements OnInit {

    @Input() documents: Observable<any>;
    @Output() onDocumentUploaded: EventEmitter<any> = new EventEmitter();

    fileUpload: any = [];
    index: number = 0; //this will keep track of your documents array, in case you want to delete them
    sendFiles: UploadDocumentModel = new UploadDocumentModel();
    private eventsSubscription: any

    private BLACKLIST: string = ".exe|.dll|.cmd|.sql|.tsql|.zip|.js|.bat|.cs|.vb|.rar";

    constructor(private spinnerService: LoadingSpinnerService, private toastr: ToastrService) {
        this.sendFiles.fileName = [];
        this.sendFiles.b64ContentStream = [];
    }

    ngOnInit() {
        this.eventsSubscription = this.documents.subscribe((documents) => this.populateUploaderWithFiles(documents))
    }


    private populateUploaderWithFiles(documents) {
        if (documents) {
            for (let i = 0; i < documents.document.length; i++) {
                this.fileUpload.push({ "index": i, "name": documents.document[i] });
                this.sendFiles.fileName.push(documents.document[i]);
                this.sendFiles.b64ContentStream.push(documents.b64ContentStream[i]);
                this.index++;
            }
        }
    }

    changeListener($event): void {
        this.sendFileToSessionStorage($event.target);
    }

    sendFileToSessionStorage(inputValue: any): void {
        this.spinnerService.start();
        if (inputValue.files[0] === undefined) {
            this.spinnerService.stop();
            return;
        }
        var file: File = inputValue.files[0];
        if (this.isBlackListed(file.name)) {
            this.toastr.error("Invalid File format", 'Errore!', { timeOut: AppConfigService.settings.toastLife });
            this.spinnerService.stop();
            return;
        }
        this.fileUpload.push({ "index": this.index, "name": file.name });
        this.index++;
        this.sendFiles.fileName.push(file.name);
        var myReader: FileReader = new FileReader();
        myReader.readAsArrayBuffer(file);

        myReader.onloadend = (evt: any) => {
            let imageData: any = myReader.result;
            let b64encoded = this.arrayBufferToBase64(imageData);
            this.spinnerService.stop();
            this.sendFiles.b64ContentStream.push(b64encoded);
            this.onDocumentUploaded.emit(this.sendFiles);
        }
    }


    private arrayBufferToBase64(buffer: any): any {
        let binary = '';
        let bytes = new Uint8Array(buffer);
        let len = bytes.byteLength;
        for (let i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }

    removeFile(file: any) {
        let index = this.fileUpload.indexOf(file);
        if (index !== -1) {
            this.fileUpload.splice(index, 1);
            this.sendFiles.fileName.splice(index, 1);
            this.sendFiles.b64ContentStream.splice(index, 1);
            this.onDocumentUploaded.emit(this.sendFiles);
        }
    }

    private isBlackListed = (fileName: string): boolean => {
        if (fileName.indexOf('.') == -1)
            return true;
        var disallowed = this.BLACKLIST;
        if (disallowed.length == 0)
            return false;
        var splitted = disallowed.split('|');
        var extension = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();
        var i = splitted.length;
        while (i--)
            if (splitted[i] == extension)
                return true;
        return false;
    }
}
export class UploadDocumentModel {
    fileName: string[];
    b64ContentStream: string[];
}