import DocumentModel = require('App/Models/Commons/DocumentModel');

class UscUploadDocumentRest {

    asyncUploadDocumentId: string;
    cbmultipleFilesId: string;
    multipleFilesId: string;

    private _asyncUploadDocument: Telerik.Web.UI.RadAsyncUpload;
    private BLACKLIST: string = ".exe|.dll|.cmd|.sql|.tsql|.zip|.js|.bat|.cs|.vb|.rar";
    private static SESSION_NAME_SELECTED_DOCUMENT: string = "SelectedDocument";
    private _sessionStorageKey: string;
    private _sessionStorageScannerKey: string = "component.scanner.upload.scan";
    private documentsList: DocumentModel[];
    private _multipleFiles: boolean;
    private fileExists: boolean;

    public static LOADED_EVENT: string = "onLoaded";

    constructor() {

    }

    initialize() {
        this._asyncUploadDocument = <Telerik.Web.UI.RadAsyncUpload>$find(this.asyncUploadDocumentId);
        this._asyncUploadDocument.add_fileSelected(this.on_clientFileSelected);
        this._asyncUploadDocument.add_fileUploading(this.on_clientFileUploading);
        this._asyncUploadDocument.add_filesSelected(this.on_clientFilesUploaded);
        this._asyncUploadDocument.add_fileUploadRemoved(this.on_clientRemoved);
        this._multipleFiles = this.convertToBoolean(this.multipleFilesId);

        this.documentsList = [];
        this.fileExists = false;

        this._sessionStorageKey = this.asyncUploadDocumentId.concat(UscUploadDocumentRest.SESSION_NAME_SELECTED_DOCUMENT);
        this.clearSessionStorage();

        if (this._multipleFiles) {
            this._asyncUploadDocument.set_maxFileCount(0);
        } else {
            this._asyncUploadDocument.set_maxFileCount(1);
        }
        this.bindLoaded();
    }

    on_clientFileSelected = (sender: Telerik.Web.UI.RadAsyncUpload, args: Telerik.Web.UI.RadAsyncUploadFileSelectedEventArgs) => {
        if (this.checkIfFileWithSameNameExists(args.get_fileName())) {
            this.validationFileExists(args.get_fileName());
            this.fileExists = true;
        }
    }

    on_clientFileUploading = (sender: Telerik.Web.UI.RadAsyncUpload, args: Telerik.Web.UI.RadAsyncUploadFileUploadingEventArgs) => {
        if (this.fileExists) {
            args.set_cancel(true);
        }
        if (this.isBlackListed(args.get_fileName())) {
            this.validationBlackList(args.get_fileName());
            args.set_cancel(true);
        }
    }

    private checkIfFileWithSameNameExists(fileName: string): boolean {
        for (let i = 0; i <= this.documentsList.length - 1; i++) {
            if (this.documentsList[i].FileName === fileName) {
                return true;
            }
        }
        return false;
    }

    on_clientFilesUploaded = (sender: any, args: Telerik.Web.UI.RadAsyncUploadFilesSelectedEventArgs) => {
        let fileInput: any = $(`#${sender._fileInput.id}`);
        let fileList: any = fileInput[0].files;
        let fileReader = new FileReader();
        if (fileReader && fileList && fileList.length) {
            fileReader.readAsArrayBuffer(fileList[0]);
            fileReader.onload = () => {
                let imageData: any = fileReader.result;
                let b64encoded = this.arrayBufferToBase64(imageData);
                this.saveSessionDocumentModel(b64encoded, fileList[0].name);
            };
        }
    }
    private arrayBufferToBase64(buffer): any {
        let binary = '';
        let bytes = new Uint8Array(buffer);
        let len = bytes.byteLength;
        for (let i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
    getDocument = () => {
        //combine documents from local storage with the scans as a whole.

        let sessionDocument: any = sessionStorage[this._sessionStorageKey];
        let sessionScan: any = sessionStorage[this._sessionStorageScannerKey];

        let documents: DocumentModel[] = [];
        let scans: DocumentModel[] = [];

        if (sessionDocument != null) {
            documents = JSON.parse(sessionDocument);
        }
        if (sessionScan != null) {
            scans = JSON.parse(sessionScan);
        }

        let result: DocumentModel[] = documents;
        for (let i = 0; i < scans.length; i++) {
            result.push(scans[i]);
        }
        return JSON.stringify(result);
    }
    clearSessionStorage = () => {
        if (sessionStorage[this._sessionStorageKey] != null) {
            sessionStorage.removeItem(this._sessionStorageKey);
        }
    }

    private saveSessionDocumentModel(b64encoded: string, fileName: string): void {
        let model: DocumentModel = {
            Segnature: "",
            FileName: fileName,
            ContentStream: b64encoded
        };
        this.documentsList.push(model);
        let documentModelJSON = JSON.stringify(this.documentsList);
        sessionStorage.setItem(this._sessionStorageKey, documentModelJSON);
    }

    on_clientRemoved = (sender: Telerik.Web.UI.RadAsyncUpload, args: Telerik.Web.UI.RadAsyncUploadFileUploadRemovedEventArgs) => {
        if ($("#ErrorHolder").is(':visible')) {
            let files: any = this._asyncUploadDocument.getUploadedFiles();
            var i = files.length;
            while (i--) {
                if (this.isBlackListed(files[i]))
                    return;
            }
            this.hideErrors();
        }

        let fileToDelete = args.get_fileName();

        for (let i = this.documentsList.length - 1; i >= 0; i--) {
            if (this.documentsList[i].FileName === fileToDelete) {
                this.documentsList.splice(i, 1);
                break;
            }
        }

        this.fileExists = false;

        let documentModelJSON = JSON.stringify(this.documentsList);
        sessionStorage.setItem(this._sessionStorageKey, documentModelJSON);

        if (this.documentsList.length == 0) {
            this.clearSessionStorage();
            this.documentsList = [];
        }
    }

    hideErrors = () => {
        $("#ErrorHolder").empty();
        $("#ErrorHolder").hide();
    }

    validationBlackList = (args: string) => {
        $("#ErrorHolder").append("<p>Estensione non valida per il file: '" + args + "'.</p>");
        $("#ErrorHolder").show();
    }

    validationFileExists = (args: string) => {
        $("#ErrorHolder").append("<p>Il file esiste già: '" + args + "'.</p>");
        $("#ErrorHolder").show();
    }

    isBlackListed = (fileName: string): boolean => {
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

    convertToBoolean = (input: string): boolean | undefined => {
        try {
            return JSON.parse(input.toLowerCase());
        }
        catch (e) {
            return undefined;
        }
    }

    private bindLoaded(): void {
        $("#".concat(this.asyncUploadDocumentId)).data(this);
        $("#".concat(this.asyncUploadDocumentId)).triggerHandler(UscUploadDocumentRest.LOADED_EVENT);

    }
}


export = UscUploadDocumentRest;