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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "App/Models/Commons/MetadataRepositoryModel", "App/ViewModels/Metadata/BaseFieldViewModel", "App/ViewModels/Metadata/MetadataDesignerViewModel", "App/ViewModels/Metadata/EnumFieldViewModel", "App/ViewModels/Metadata/DiscussionFieldViewModel", "App/ViewModels/Metadata/TextFieldViewModel"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, MetadataRepositoryModel, BaseFieldViewModel, MetadataDesignerViewModel, EnumFieldViewModel, DiscussionFieldViewModel, TextFieldViewModel) {
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
            if (!elmnt) {
                return;
            }
            this.integerId = $("#menuContent").children().length;
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
            this.currentComponentToBeDragged = cln;
            this.setNewlyAddedComponentToMovable(cln);
            this.setNewlyAddedComponentToInputEditable(cln);
        };
        uscMetadataRepositoryDesigner.prototype.setNewlyAddedComponentToInputEditable = function (element) {
            var data = [];
            var elementChildren = element.children;
            for (var i = 0; i < elementChildren.length; i++) {
                if (elementChildren[i] && this.hasClass(elementChildren[i], 'controls')) {
                    var table = elementChildren[i].children;
                    var input = table ? table[0].getElementsByTagName('input') : [];
                    for (var j = 0; j < input.length; j++) {
                        data.push(input[j]);
                    }
                }
            }
            this.registerFocusBlurEvents(data);
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
            if (this.setiEnabledId) {
                var field = document.getElementById("" + this.setiFieldCheckId);
                field.style.display = "block";
            }
        };
        /**
         * Metodo di preparazione del modello da salvare
         */
        uscMetadataRepositoryDesigner.prototype.prepareModel = function () {
            var content = $("#menuContent").children();
            var item = new MetadataRepositoryModel();
            var metadataModel = new MetadataDesignerViewModel();
            var baseField;
            var enumField;
            var textField;
            var discussionField;
            var inputElement;
            var inputKeyNameElement;
            var requiredElement;
            if (!this.validFields()) {
                return null;
            }
            var setiField = $("#seti_input_check");
            $.each(content, function (index, divElement) {
                inputElement = ($("#".concat(divElement.id, " :input.form-control"))[0]);
                inputKeyNameElement = ($("#".concat(divElement.id, " :input.form-control"))[1]);
                requiredElement = ($("#".concat(divElement.id, " :input.checkBox"))[0]);
                var dataset = divElement.getAttribute("data-type");
                switch (dataset) {
                    case "Title":
                        item.Name = inputElement.value;
                        if (setiField) {
                            metadataModel.SETIFieldEnabled = setiField[0].checked;
                        }
                        break;
                    case "Text":
                        textField = new TextFieldViewModel();
                        textField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                        textField.Label = inputElement.value;
                        textField.Required = requiredElement.checked;
                        textField.Position = index;
                        metadataModel.TextFields.push(textField);
                        break;
                    case "Number":
                        baseField = new BaseFieldViewModel();
                        baseField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                        baseField.Label = inputElement.value;
                        baseField.Required = requiredElement.checked;
                        baseField.Position = index;
                        metadataModel.NumberFields.push(baseField);
                        break;
                    case "Date":
                        baseField = new BaseFieldViewModel();
                        baseField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                        baseField.Label = inputElement.value;
                        baseField.Required = requiredElement.checked;
                        baseField.Position = index;
                        metadataModel.DateFields.push(baseField);
                        break;
                    case "CheckBox":
                        baseField = new BaseFieldViewModel();
                        baseField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                        baseField.Label = inputElement.value;
                        baseField.Required = requiredElement.checked;
                        baseField.Position = index;
                        metadataModel.BoolFields.push(baseField);
                        break;
                    case "Enum":
                        enumField = new EnumFieldViewModel();
                        enumField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                        enumField.Label = inputElement.value;
                        enumField.Required = requiredElement.checked;
                        enumField.Position = index;
                        enumField.Options = {};
                        $.each($("#" + divElement.id).find('li'), function (index, listElement) {
                            enumField.Options[index + 1] = listElement.textContent;
                        });
                        metadataModel.EnumFields.push(enumField);
                        break;
                    case "Comment":
                        discussionField = new DiscussionFieldViewModel();
                        discussionField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                        discussionField.Label = inputElement.value;
                        discussionField.Required = requiredElement.checked;
                        discussionField.Position = index;
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
            if (this.setiEnabledId) {
                var field = document.getElementById("" + this.setiFieldCheckId);
                field.style.display = "block";
            }
            var setiField = $("input#seti_input_check");
            if (setiField) {
                setiField[0].checked = metadataViewModel.SETIFieldEnabled;
            }
            this.arrangeControlsInPosition(metadataViewModel, idCloned);
            this.setMovableComponents();
            this.bindLoaded();
        };
        uscMetadataRepositoryDesigner.prototype.arrangeControlsInPosition = function (metadataViewModel, idCloned) {
            var _this = this;
            var aggregatedSum = 0;
            for (var arr in metadataViewModel) {
                if (typeof metadataViewModel[arr] !== "boolean") {
                    aggregatedSum += metadataViewModel[arr].length;
                }
            }
            var _loop_1 = function () {
                var metadataField = null;
                var currentType = void 0;
                for (var arr in metadataViewModel) {
                    currentType = arr;
                    var obj = undefined;
                    if (typeof (metadataViewModel[arr]) !== "boolean") {
                        obj = metadataViewModel[arr].filter(function (x) { return x.Position == i; })[0];
                    }
                    if (obj) {
                        metadataField = obj;
                        break;
                    }
                }
                switch (currentType) {
                    case "TextFields":
                        idCloned = this_1.fillHTMLElement(this_1.componentTextId, metadataField.Position, metadataField, currentType);
                        break;
                    case "DateFields":
                        idCloned = this_1.fillHTMLElement(this_1.componentDateId, metadataField.Position, metadataField, currentType);
                        break;
                    case "NumberFields":
                        idCloned = this_1.fillHTMLElement(this_1.componentNumberId, metadataField.Position, metadataField, currentType);
                        break;
                    case "BoolFields":
                        idCloned = this_1.fillHTMLElement(this_1.componentCheckBoxId, metadataField.Position, metadataField, currentType);
                        break;
                    case "EnumFields":
                        idCloned = this_1.fillHTMLElement(this_1.componentEnumId, metadataField.Position, metadataField, currentType);
                        $.each(metadataField.Options, function (index, option) {
                            var node = document.createElement("LI");
                            if (metadataField.Options[index] != "") {
                                _this.createNewNode(metadataField.Options[index], node, idCloned);
                            }
                        });
                        break;
                    case "DiscussionFields":
                        idCloned = this_1.fillHTMLElement(this_1.componentCommentId, metadataField.Position, metadataField, currentType);
                        break;
                    default:
                        break;
                }
            };
            var this_1 = this;
            for (var i = 0; i <= aggregatedSum; i++) {
                _loop_1();
            }
        };
        /**
         * Popoplo un contollo con i dati del modello
         * @param idComponent
         * @param incrementalInteger
         * @param model
         */
        uscMetadataRepositoryDesigner.prototype.fillHTMLElement = function (idComponent, incrementalInteger, model, currentType) {
            var idCloned = this.cloneElement(idComponent, incrementalInteger);
            var inputElement = this.findStandardInputElement(idCloned, 0);
            var inputKeyNameElement = this.findStandardInputElement(idCloned, 1);
            var requiredElement = this.findInputCheckBoxElement(idCloned, 0);
            inputElement.value = model.Label;
            requiredElement.checked = model.Required;
            inputKeyNameElement.value = model.KeyName ? model.KeyName : "";
            this.setDeleteComponentId(currentType, incrementalInteger);
            this.registerFocusBlurEvents([inputElement, inputKeyNameElement, requiredElement]);
            return idCloned;
        };
        uscMetadataRepositoryDesigner.prototype.registerFocusBlurEvents = function (elements) {
            var _this = this;
            var _loop_2 = function (element) {
                element.addEventListener('focus', function (ev) {
                    _this.removeSelectable(element, ev);
                });
                element.addEventListener('blur', function (ev) {
                    _this.addSelectable(element, ev);
                });
            };
            for (var _i = 0, elements_1 = elements; _i < elements_1.length; _i++) {
                var element = elements_1[_i];
                _loop_2(element);
            }
        };
        uscMetadataRepositoryDesigner.prototype.removeSelectable = function (element, ev) {
            var parent = this.findParentRecursive(element);
            var isIE = /(MSIE|Trident\/|Edge\/)/i.test(navigator.userAgent);
            parent.removeAttribute('draggable');
            element.focus();
            if (isIE) { //IE
                var range = element.createTextRange();
                range.collapse(false);
                range.select();
            }
            else { //others
                var range = document.createRange();
                range.collapse(false);
                window.getSelection().addRange(range);
            }
        };
        uscMetadataRepositoryDesigner.prototype.addSelectable = function (element, ev) {
            var parent = this.findParentRecursive(element);
            parent.setAttribute('draggable', 'true');
        };
        uscMetadataRepositoryDesigner.prototype.findParentRecursive = function (element) {
            if (!element) {
                return undefined;
            }
            if (this.hasClass(element, 'element')) {
                return element;
            }
            else {
                return this.findParentRecursive(element.parentNode);
            }
        };
        /**
         * Quando il componente viene disegnato sul DOM, assegnare un ID univoco
         * @param incrementalInteger
         */
        uscMetadataRepositoryDesigner.prototype.setDeleteComponentId = function (currentType, incrementalInteger) {
            switch (currentType) {
                case "TextFields":
                    var closeElement = document.getElementById("closeText");
                    closeElement.id = "closeText" + incrementalInteger;
                    break;
                case "DateFields":
                    var closeDate = document.getElementById("closeDate");
                    closeDate.id = "closeDate" + incrementalInteger;
                    break;
                case "NumberFields":
                    var closeNumber = document.getElementById("closeNumber");
                    closeNumber.id = "closeNumber" + incrementalInteger;
                    break;
                case "BoolFields":
                    var closeCheckBox = document.getElementById("closeCheckBox");
                    closeCheckBox.id = "closeCheckBox" + incrementalInteger;
                    break;
                case "EnumFields":
                    var closeEnum = document.getElementById("closeEnum");
                    if (closeEnum) {
                        closeEnum.id = "closeEnum" + incrementalInteger;
                    }
                    break;
                case "DiscussionFields":
                    var closeComment = document.getElementById("closeComment");
                    closeComment.id = "closeComment" + incrementalInteger;
                    break;
                default:
                    break;
            }
        };
        uscMetadataRepositoryDesigner.prototype.setMovableComponents = function () {
            var _this = this;
            var menuSubItems = $("#menuContent").children();
            var isIE = /(MSIE|Trident\/|Edge\/)/i.test(navigator.userAgent);
            $.each(menuSubItems, function (index, menuSubItem) {
                if (index > 0) {
                    /*Make Title non draggable*/
                    menuSubItems[index].setAttribute('draggable', 'true');
                    menuSubItems[index].classList.add('component');
                    menuSubItem.addEventListener('dragstart', function (ev) {
                        _this.currentComponentToBeDragged = ev.target;
                    });
                    menuSubItem.addEventListener('dragover', function (ev) {
                        _this.dragOver(ev);
                    });
                    menuSubItem.addEventListener('dragleave', function (ev) {
                        _this.dragLeave(ev);
                    });
                    if (!isIE) {
                        menuSubItem.addEventListener('drop', function (ev) {
                            /*needed because Firefox doesn't know dragend event*/
                            _this.dropFinished(ev);
                        });
                    }
                    else {
                        menuSubItem.addEventListener('dragenter', function (ev) {
                            _this.dragEnterIE(ev);
                        });
                        menuSubItem.addEventListener('dragend', function (ev) {
                            /*needed because IE doesn't know drop event*/
                            _this.dragEndFinished(menuSubItems, ev);
                            _this.animateLeaveHover(_this.previousElementIE);
                            _this.previousElementIE = null;
                        });
                    }
                }
            });
        };
        uscMetadataRepositoryDesigner.prototype.setNewlyAddedComponentToMovable = function (cln) {
            var _this = this;
            var isIE = /(MSIE|Trident\/|Edge\/)/i.test(navigator.userAgent);
            cln.setAttribute('draggable', 'true');
            cln.classList.add('component');
            cln.addEventListener('dragover', function (ev) {
                _this.dragOver(ev);
            });
            cln.addEventListener('dragleave', function (ev) {
                _this.dragLeave(ev);
            });
            cln.addEventListener('dragstart', function (ev) {
                _this.currentComponentToBeDragged = ev.target;
                $("#".concat(_this.pageContentId)).data(_this);
            });
            if (isIE) {
                cln.addEventListener('dragenter', function (ev) {
                    _this.dragEnterIE(ev);
                });
                cln.addEventListener('dragend', function (evt) {
                    var menuSubItems = $("#menuContent").children();
                    var draggingElement = evt.target;
                    var x = evt.clientX;
                    var y = evt.clientY;
                    var elementMouseDraggedTo = document.elementFromPoint(x, y);
                    if (!elementMouseDraggedTo) {
                        return;
                    }
                    var _a = _this.getNewCoordinates(menuSubItems, draggingElement, elementMouseDraggedTo), dragToIndex = _a.dragToIndex, dragFromIndex = _a.dragFromIndex;
                    if (dragToIndex == -1 || dragFromIndex == -1) {
                        return; /*invalid position to move*/
                    }
                    var newMenuSubItems = _this.renewMenuSubItems(menuSubItems, dragFromIndex, dragToIndex);
                    var content = document.getElementById("menuContent");
                    /*draw the newly changed items on the canvas*/
                    $.each(menuSubItems, function (index, menuSubItem) {
                        content.removeChild(menuSubItems[index]);
                        content.appendChild(newMenuSubItems[index]);
                    });
                    _this.animateLeaveHover(_this.previousElementIE);
                    _this.previousElementIE = null;
                });
            }
            else {
                cln.addEventListener('drop', function (ev) {
                    _this.dropFinished(ev);
                });
            }
        };
        uscMetadataRepositoryDesigner.prototype.dragEnterIE = function (evt) {
            this.showDroppableSelectionIE(evt);
        };
        uscMetadataRepositoryDesigner.prototype.dragOver = function (evt) {
            this.showDroppableSelection(evt);
        };
        uscMetadataRepositoryDesigner.prototype.dragLeave = function (evt) {
            this.hideDroppableSelection(evt);
        };
        uscMetadataRepositoryDesigner.prototype.dropFinished = function (evt) {
            this.hideDroppableSelection(evt);
            var indexDestination = -1;
            var indexStarting = -1;
            var menuSubItems = $("#menuContent").children();
            var thisObj = $("#".concat(this.pageContentId)).data();
            $.each(menuSubItems, function (index, mouseSubItem) {
                if (mouseSubItem == thisObj.currentComponentToBeDragged) {
                    indexStarting = index;
                }
            });
            var elementMouseDraggedTo = evt.target.parentNode;
            $.each(menuSubItems, function (index, mouseSubItem) {
                if ((elementMouseDraggedTo.nodeName == "UL") && (elementMouseDraggedTo.parentElement == menuSubItems[index].getElementsByClassName("controls")[1])) {
                    /*when dealing with enums that have big lists and the drop is on them*/
                    indexDestination = index == 0 ? 1 : index; /*this will ensure that the title alwayas stays on top of the stack.*/
                }
                else if (mouseSubItem == evt.target.parentElement) {
                    indexDestination = index;
                }
            });
            if (indexDestination == -1 || indexStarting == -1) {
                return; /*invalid position to move*/
            }
            var newMenuSubItems = this.renewMenuSubItems(menuSubItems, indexStarting, indexDestination);
            var content = document.getElementById("menuContent");
            /*draw the newly changed items on the canvas*/
            $.each(menuSubItems, function (index, menuSubItem) {
                content.removeChild(menuSubItems[index]);
                content.appendChild(newMenuSubItems[index]);
            });
        };
        uscMetadataRepositoryDesigner.prototype.dragEndFinished = function (menuSubItems, evt) {
            var draggingElement = evt.target;
            var x = evt.clientX;
            var y = evt.clientY;
            var elementMouseDraggedTo = document.elementFromPoint(x, y);
            if (!elementMouseDraggedTo) {
                return;
            }
            var _a = this.getNewCoordinates(menuSubItems, draggingElement, elementMouseDraggedTo), dragToIndex = _a.dragToIndex, dragFromIndex = _a.dragFromIndex;
            if (dragToIndex == -1 || dragFromIndex == -1) {
                return; /*invalid position to move*/
            }
            var newMenuSubItems = this.renewMenuSubItems(menuSubItems, dragFromIndex, dragToIndex);
            var content = document.getElementById("menuContent");
            /*draw the newly changed items on the canvas*/
            $.each(menuSubItems, function (index, menuSubItem) {
                content.removeChild(menuSubItems[index]);
                content.appendChild(newMenuSubItems[index]);
            });
        };
        /* Helpers */
        uscMetadataRepositoryDesigner.prototype.getNewCoordinates = function (menuSubItems, draggingElement, elementMouseDraggedTo) {
            var dragToIndex = -1;
            var dragFromIndex = 1;
            $.each(menuSubItems, function (index, mouseSubItem) {
                if (mouseSubItem == draggingElement) {
                    /*element from initial location*/
                    dragFromIndex = index;
                }
                if (menuSubItems[index].getElementsByClassName("controls")[0] == elementMouseDraggedTo || (menuSubItems[index].getElementsByClassName("controls")[1] == elementMouseDraggedTo)) {
                    /*element to current destination*/
                    dragToIndex = index == 0 ? 1 : index; /*this will ensure that the title alwayas stays on top of the stack.*/
                    return;
                }
                if ((elementMouseDraggedTo.parentElement.nodeName == "UL") && (elementMouseDraggedTo.parentElement.parentElement == menuSubItems[index].getElementsByClassName("controls")[1])) {
                    /*when dealing with enums that have big lists and the drop is on them*/
                    dragToIndex = index == 0 ? 1 : index; /*this will ensure that the title alwayas stays on top of the stack.*/
                    return;
                }
                if ((elementMouseDraggedTo.nodeName == "TABLE") && (elementMouseDraggedTo.parentElement == menuSubItems[index].getElementsByClassName("controls")[0])) {
                    /*if user is sloppy, and he drops the component over the table*/
                    dragToIndex = index == 0 ? 1 : index; /*this will ensure that the title alwayas stays on top of the stack.*/
                    return;
                }
            });
            return { dragToIndex: dragToIndex, dragFromIndex: dragFromIndex };
        };
        uscMetadataRepositoryDesigner.prototype.renewMenuSubItems = function (oldMenuSubItems, oldIndex, newIndex) {
            if (newIndex >= oldMenuSubItems.length) {
                var i = newIndex - oldMenuSubItems.length + 1;
                while (i--) {
                    oldMenuSubItems.push(undefined);
                }
            }
            oldMenuSubItems.splice(newIndex, 0, oldMenuSubItems.splice(oldIndex, 1)[0]);
            return oldMenuSubItems;
        };
        ;
        uscMetadataRepositoryDesigner.prototype.hideDroppableSelection = function (evt) {
            if (this.hasClass(evt.target, "component")) {
                this.animateLeaveHover(evt.target);
            }
            else if (this.hasClass(evt.target.parentElement, "ul")) {
                this.animateLeaveHover(evt.target.parentElement.parentElement);
            }
            else {
                this.animateLeaveHover(evt.target.parentElement);
            }
        };
        uscMetadataRepositoryDesigner.prototype.showDroppableSelection = function (evt) {
            if (this.hasClass(evt.target.parentElement, "ul")) {
                this.animateEnterHover(evt.target.parentElement.parentElement);
            }
            else if (this.hasClass(evt.target, "component")) {
                this.animateEnterHover(evt.target);
            }
            else {
                this.animateEnterHover(evt.target.parentElement);
            }
        };
        uscMetadataRepositoryDesigner.prototype.showDroppableSelectionIE = function (element) {
            if (this.hasClass(element.target.parentElement, "ul")) {
                this.animatEnterHoverIE(element.target.parentElement.parentElement);
            }
            else if (this.hasClass(element.target, "component")) {
                this.animatEnterHoverIE(element.target);
            }
            else {
                this.animatEnterHoverIE(element.target.parentElement);
            }
        };
        uscMetadataRepositoryDesigner.prototype.animatEnterHoverIE = function (element) {
            if (!this.previousElementIE) {
                this.previousElementIE = element;
                this.animateLeaveHover(element);
            }
            if (this.previousElementIE !== element) {
                this.animateLeaveHover(this.previousElementIE);
                this.previousElementIE = element;
                this.animateEnterHover(element);
            }
        };
        uscMetadataRepositoryDesigner.prototype.animateEnterHover = function (element) {
            element.style.transform = "scale(0.98,0.98)";
            element.style.transitionDuration = "200ms";
            element.style.transitionTimingFunction = "ease-out";
            element.style.opacity = "0.98";
        };
        uscMetadataRepositoryDesigner.prototype.animateLeaveHover = function (element) {
            element.style.transform = "scale(1,1)";
            element.style.transitionDuration = "200ms";
            element.style.transitionTimingFunction = "ease-in";
            element.style.opacity = "1";
        };
        uscMetadataRepositoryDesigner.prototype.hasClass = function (element, className) {
            return (' ' + element.className + ' ').indexOf(' ' + className + ' ') > -1;
        };
        return uscMetadataRepositoryDesigner;
    }(MetadataRepositoryBase));
    return uscMetadataRepositoryDesigner;
});
//# sourceMappingURL=uscMetadataRepositoryDesigner.js.map