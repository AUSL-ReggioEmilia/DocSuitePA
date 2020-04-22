define(["require", "exports"], function (require, exports) {
    var UscUploadDocumentRest = /** @class */ (function () {
        function UscUploadDocumentRest() {
            var _this = this;
            this.BLACKLIST = ".exe|.dll|.cmd|.sql|.tsql|.zip|.js|.bat|.cs|.vb|.rar";
            this._sessionStorageScannerKey = "component.scanner.upload.scan";
            this.on_clientFileSelected = function (sender, args) {
                if (_this.checkIfFileWithSameNameExists(args.get_fileName())) {
                    _this.validationFileExists(args.get_fileName());
                    _this.fileExists = true;
                }
            };
            this.on_clientFileUploading = function (sender, args) {
                if (_this.fileExists) {
                    args.set_cancel(true);
                }
                if (_this.isBlackListed(args.get_fileName())) {
                    _this.validationBlackList(args.get_fileName());
                    args.set_cancel(true);
                }
            };
            this.on_clientFilesUploaded = function (sender, args) {
                var fileInput = $("#" + sender._fileInput.id);
                var fileList = fileInput[0].files;
                var fileReader = new FileReader();
                if (fileReader && fileList && fileList.length) {
                    fileReader.readAsArrayBuffer(fileList[0]);
                    fileReader.onload = function () {
                        var imageData = fileReader.result;
                        var b64encoded = _this.arrayBufferToBase64(imageData);
                        _this.saveSessionDocumentModel(b64encoded, fileList[0].name);
                    };
                }
            };
            this.getDocument = function () {
                //combine documents from local storage with the scans as a whole.
                var sessionDocument = sessionStorage[_this._sessionStorageKey];
                var sessionScan = sessionStorage[_this._sessionStorageScannerKey];
                var documents = [];
                var scans = [];
                if (sessionDocument != null) {
                    documents = JSON.parse(sessionDocument);
                }
                if (sessionScan != null) {
                    scans = JSON.parse(sessionScan);
                }
                var result = documents;
                for (var i = 0; i < scans.length; i++) {
                    result.push(scans[i]);
                }
                return JSON.stringify(result);
            };
            this.clearSessionStorage = function () {
                if (sessionStorage[_this._sessionStorageKey] != null) {
                    sessionStorage.removeItem(_this._sessionStorageKey);
                }
            };
            this.on_clientRemoved = function (sender, args) {
                if ($("#ErrorHolder").is(':visible')) {
                    var files = _this._asyncUploadDocument.getUploadedFiles();
                    var i = files.length;
                    while (i--) {
                        if (_this.isBlackListed(files[i]))
                            return;
                    }
                    _this.hideErrors();
                }
                var fileToDelete = args.get_fileName();
                for (var i_1 = _this.documentsList.length - 1; i_1 >= 0; i_1--) {
                    if (_this.documentsList[i_1].FileName === fileToDelete) {
                        _this.documentsList.splice(i_1, 1);
                        break;
                    }
                }
                _this.fileExists = false;
                var documentModelJSON = JSON.stringify(_this.documentsList);
                sessionStorage.setItem(_this._sessionStorageKey, documentModelJSON);
                if (_this.documentsList.length == 0) {
                    _this.clearSessionStorage();
                    _this.documentsList = [];
                }
            };
            this.hideErrors = function () {
                $("#ErrorHolder").empty();
                $("#ErrorHolder").hide();
            };
            this.validationBlackList = function (args) {
                $("#ErrorHolder").append("<p>Estensione non valida per il file: '" + args + "'.</p>");
                $("#ErrorHolder").show();
            };
            this.validationFileExists = function (args) {
                $("#ErrorHolder").append("<p>Il file esiste già: '" + args + "'.</p>");
                $("#ErrorHolder").show();
            };
            this.isBlackListed = function (fileName) {
                if (fileName.indexOf('.') == -1)
                    return true;
                var disallowed = _this.BLACKLIST;
                if (disallowed.length == 0)
                    return false;
                var splitted = disallowed.split('|');
                var extension = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();
                var i = splitted.length;
                while (i--)
                    if (splitted[i] == extension)
                        return true;
                return false;
            };
            this.convertToBoolean = function (input) {
                try {
                    return JSON.parse(input.toLowerCase());
                }
                catch (e) {
                    return undefined;
                }
            };
        }
        UscUploadDocumentRest.prototype.initialize = function () {
            this._asyncUploadDocument = $find(this.asyncUploadDocumentId);
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
            }
            else {
                this._asyncUploadDocument.set_maxFileCount(1);
            }
            this.bindLoaded();
        };
        UscUploadDocumentRest.prototype.checkIfFileWithSameNameExists = function (fileName) {
            for (var i = 0; i <= this.documentsList.length - 1; i++) {
                if (this.documentsList[i].FileName === fileName) {
                    return true;
                }
            }
            return false;
        };
        UscUploadDocumentRest.prototype.arrayBufferToBase64 = function (buffer) {
            var binary = '';
            var bytes = new Uint8Array(buffer);
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
        };
        UscUploadDocumentRest.prototype.saveSessionDocumentModel = function (b64encoded, fileName) {
            var model = {
                Segnature: "",
                FileName: fileName,
                ContentStream: b64encoded
            };
            this.documentsList.push(model);
            var documentModelJSON = JSON.stringify(this.documentsList);
            sessionStorage.setItem(this._sessionStorageKey, documentModelJSON);
        };
        UscUploadDocumentRest.prototype.bindLoaded = function () {
            $("#".concat(this.asyncUploadDocumentId)).data(this);
            $("#".concat(this.asyncUploadDocumentId)).triggerHandler(UscUploadDocumentRest.LOADED_EVENT);
        };
        UscUploadDocumentRest.SESSION_NAME_SELECTED_DOCUMENT = "SelectedDocument";
        UscUploadDocumentRest.LOADED_EVENT = "onLoaded";
        return UscUploadDocumentRest;
    }());
    return UscUploadDocumentRest;
});
//# sourceMappingURL=uscUploadDocumentRest.js.map