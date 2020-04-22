/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "App/Models/Commons/MetadataRepositoryModel", "App/ViewModels/Metadata/BaseFieldViewModel", "App/ViewModels/Metadata/MetadataViewModel", "App/ViewModels/Metadata/EnumFieldViewModel", "App/ViewModels/Metadata/DiscussionFieldViewModel", "App/ViewModels/Metadata/TextFieldViewModel"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, MetadataRepositoryModel, BaseFieldViewModel, MetadataViewModel, EnumFieldViewModel, DiscussionFieldViewModel, TextFieldViewModel) {
    var uscMetadataRepositoryDesigner = /** @class */ (function (_super) {
        __extends(uscMetadataRepositoryDesigner, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function uscMetadataRepositoryDesigner(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME)) || this;
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /*
         * ------------------------- Events -----------------------
         */
        /**
         * Inizializzazione
         */
        uscMetadataRepositoryDesigner.prototype.initialize = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.ajaxManagerId);
            this.integerId = 1;
            this.setName();
            this.bindLoaded();
        };
        /**
         * Metodo che gestisce l'aggiunta di valori dell'enum
         * @param ev
         */
        uscMetadataRepositoryDesigner.prototype.addValue = function (ev) {
            var idParent = ev.target.parentNode.parentNode.id;
            var inputElement = ($("#".concat(idParent, " :input.enumInput"))[0]);
            if (inputElement) {
                if (inputElement.value != "") {
                    var node = document.createElement("LI");
                    var textNode = document.createTextNode(inputElement.value);
                    node.appendChild(textNode);
                    var listElement = ($("#" + idParent).find('ul')[0]);
                    listElement.appendChild(node);
                    inputElement.value = "";
                }
            }
        };
        /**
         * Metodo per fare il drag degli elementi nella pagina
         * @param ev
         */
        uscMetadataRepositoryDesigner.prototype.drag = function (ev) {
            ev.dataTransfer.setData("text", ev.target.id);
        };
        /**
         * Metodo per aggiungere le componenti al click
         * @param ev
         */
        uscMetadataRepositoryDesigner.prototype.click = function (ev) {
            this.addComponent(ev.target.id);
        };
        /**
         * Metodo scatenato al drop di elementi nella pagina
         * @param ev
         */
        uscMetadataRepositoryDesigner.prototype.drop = function (ev) {
            ev.preventDefault();
            var data = ev.dataTransfer.getData("text");
            this.addComponent(data);
        };
        /**
         * Aggiungo una componente alla lista di elementi
         * @param idComponent
         */
        uscMetadataRepositoryDesigner.prototype.addComponent = function (idComponent) {
            var elmnt;
            switch (idComponent) {
                case "Title":
                    elmnt = document.getElementById(this.componentTitleId);
                    break;
                case "Text":
                    elmnt = document.getElementById(this.componentTextId);
                    break;
                case "Number":
                    elmnt = document.getElementById(this.componentNumberId);
                    break;
                case "Date":
                    elmnt = document.getElementById(this.componentDateId);
                    break;
                case "CheckBox":
                    elmnt = document.getElementById(this.componentCheckBoxId);
                    break;
                case "Enumerator":
                    elmnt = document.getElementById(this.componentEnumId);
                    break;
                case "Comment":
                    elmnt = document.getElementById(this.componentCommentId);
                    break;
                default:
                    break;
            }
            var cln = elmnt.cloneNode(true);
            cln.setAttribute("idParent", elmnt.id);
            cln.setAttribute("id", elmnt.id + this.integerId);
            this.integerId++;
            var close = cln.getElementsByClassName("close")[0];
            close.setAttribute("id", close.id + this.integerId);
            this.integerId++;
            if (cln.getElementsByClassName("controls")[1]) {
                var values = cln.getElementsByClassName("controls")[1];
                values.setAttribute("id", values.id + this.integerId);
                this.integerId++;
            }
            var content = document.getElementById("menuContent");
            content.appendChild(cln);
            this.bindLoaded();
        };
        /**
         * Rimuovo una componente dalla lista
         * @param ev
         */
        uscMetadataRepositoryDesigner.prototype.removeItem = function (ev) {
            var xdiv = document.getElementById(ev.target.id);
            var elementToClose = xdiv.parentNode;
            elementToClose.parentNode.removeChild(elementToClose);
        };
        /**
         * ------------------------ Methods -----------------------
         */
        /**
         * Imposto il campo nome di default
         */
        uscMetadataRepositoryDesigner.prototype.setName = function () {
            var elmnt = document.getElementById(this.componentTitleId);
            var cln = elmnt.cloneNode(true);
            var content = document.getElementById("menuContent");
            content.appendChild(cln);
        };
        /**
         * Metodo di preparazione del modello da salvare
         */
        uscMetadataRepositoryDesigner.prototype.prepareModel = function () {
            var content = $("#menuContent").children();
            var item = new MetadataRepositoryModel();
            var metadataModel = new MetadataViewModel();
            var baseField;
            var enumField;
            var textField;
            var discussionField;
            var inputElement;
            var requiredElement;
            if (!this.validFields()) {
                return null;
            }
            $.each(content, function (index, divElement) {
                inputElement = ($("#".concat(divElement.id, " :input.form-control"))[0]);
                requiredElement = ($("#".concat(divElement.id, " :input.checkBox"))[0]);
                var dataset = divElement.getAttribute("data-type");
                switch (dataset) {
                    case "Title":
                        item.Name = inputElement.value;
                        break;
                    case "Text":
                        textField = new TextFieldViewModel();
                        textField.Label = inputElement.value;
                        textField.Required = requiredElement.checked;
                        metadataModel.TextFields.push(textField);
                        break;
                    case "Number":
                        baseField = new BaseFieldViewModel();
                        baseField.Label = inputElement.value;
                        baseField.Required = requiredElement.checked;
                        metadataModel.NumberFields.push(baseField);
                        break;
                    case "Date":
                        baseField = new BaseFieldViewModel();
                        baseField.Label = inputElement.value;
                        baseField.Required = requiredElement.checked;
                        metadataModel.DateFields.push(baseField);
                        break;
                    case "CheckBox":
                        baseField = new BaseFieldViewModel();
                        baseField.Label = inputElement.value;
                        baseField.Required = requiredElement.checked;
                        metadataModel.BoolFields.push(baseField);
                        break;
                    case "Enum":
                        enumField = new EnumFieldViewModel();
                        enumField.Label = inputElement.value;
                        enumField.Required = requiredElement.checked;
                        enumField.Options = {};
                        $.each($("#" + divElement.id).find('li'), function (index, listElement) {
                            enumField.Options[index + 1] = listElement.textContent;
                        });
                        metadataModel.EnumFields.push(enumField);
                        break;
                    case "Comment":
                        discussionField = new DiscussionFieldViewModel();
                        discussionField.Label = inputElement.value;
                        discussionField.Required = requiredElement.checked;
                        metadataModel.DiscussionFields.push(discussionField);
                        break;
                    default:
                        break;
                }
            });
            item.Version = 1;
            item.JsonMetadata = JSON.stringify(metadataModel);
            return item;
        };
        /**
         * Scateno l'evento di "Load Completed" del controllo
         */
        uscMetadataRepositoryDesigner.prototype.bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        /**
         * verifico se sono presenti nel dom due componenti con lo stesso nome
         * @param labels
         */
        uscMetadataRepositoryDesigner.prototype.duplicateElemntsName = function (labels) {
            var vlauesSoFar = Object.create(null);
            for (var i = 0; i < labels.length; ++i) {
                var value = labels[i];
                if (value in vlauesSoFar) {
                    return true;
                }
                vlauesSoFar[value] = true;
            }
            return false;
        };
        /**
         * Verifico che non ci siano campi duplicati o vuoti
         */
        uscMetadataRepositoryDesigner.prototype.validFields = function () {
            var content = $("#menuContent").children();
            var inputElement;
            var invalidField = false;
            var labels = [];
            $.each(content, function (index, divElement) {
                inputElement = ($("#".concat(divElement.id, " :input.form-control"))[0]);
                if (inputElement.value === "") {
                    invalidField = true;
                    return;
                }
                labels.push(inputElement.value);
            });
            if (invalidField) {
                alert("Attenzione, i nomi dei campi sono obbligatori. Ci sono dei campi che non sono stati valorizzati.");
                this._loadingPanel.hide(this.pageContentId);
                return false;
            }
            if (this.duplicateElemntsName(labels)) {
                alert("Attenzione, i nomi dei campi devono essere univoci. Ci sono dei campi con nomi duplicati.");
                this._loadingPanel.hide(this.pageContentId);
                return false;
            }
            return true;
        };
        /**
         * Carico un modello esistente nella pagina
         * @param metadataRepositoryModel
         */
        uscMetadataRepositoryDesigner.prototype.loadModel = function (metadataRepositoryModel) {
            var _this = this;
            var metadataViewModel = JSON.parse(metadataRepositoryModel.JsonMetadata);
            var inputElement;
            ;
            var listElement;
            var idCloned;
            this.clearPage();
            idCloned = this.cloneElement(this.componentTitleId, this.integerId);
            this.integerId++;
            inputElement = this.findStandardInputElement(idCloned, 0);
            inputElement.value = metadataRepositoryModel.Name;
            $.each(metadataViewModel.TextFields, function (index, textFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentTextId, _this.integerId, textFieldViewModel);
                _this.integerId++;
            });
            $.each(metadataViewModel.NumberFields, function (index, numberFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentNumberId, _this.integerId, numberFieldViewModel);
                _this.integerId++;
            });
            $.each(metadataViewModel.DateFields, function (index, dateFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentDateId, _this.integerId, dateFieldViewModel);
                _this.integerId++;
            });
            $.each(metadataViewModel.BoolFields, function (index, boolFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentCheckBoxId, _this.integerId, boolFieldViewModel);
                _this.integerId++;
            });
            $.each(metadataViewModel.EnumFields, function (index, enumFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentEnumId, _this.integerId, enumFieldViewModel);
                _this.integerId++;
                $.each(enumFieldViewModel.Options, function (index, option) {
                    var node = document.createElement("LI");
                    if (enumFieldViewModel.Options[index] != "") {
                        _this.createNewNode(enumFieldViewModel.Options[index], node, idCloned);
                    }
                });
            });
            $.each(metadataViewModel.DiscussionFields, function (index, discussionFieldViewModel) {
                idCloned = _this.fillHTMLElement(_this.componentCommentId, _this.integerId, discussionFieldViewModel);
                _this.integerId++;
            });
            this.bindLoaded();
        };
        /**
         * Popoplo un contollo con i dati del modello
         * @param idComponent
         * @param incrementalInteger
         * @param model
         */
        uscMetadataRepositoryDesigner.prototype.fillHTMLElement = function (idComponent, incrementalInteger, model) {
            var idCloned = this.cloneElement(idComponent, incrementalInteger);
            var inputElement = this.findStandardInputElement(idCloned, 0);
            var requiredElement = this.findInputCheckBoxElement(idCloned, 0);
            inputElement.value = model.Label;
            requiredElement.checked = model.Required;
            return idCloned;
        };
        return uscMetadataRepositoryDesigner;
    }(MetadataRepositoryBase));
    return uscMetadataRepositoryDesigner;
});
//# sourceMappingURL=uscMetadataRepositoryDesigner.js.map