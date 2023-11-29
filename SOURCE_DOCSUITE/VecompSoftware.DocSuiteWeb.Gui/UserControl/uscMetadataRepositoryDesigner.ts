/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
import EnumFieldViewModel = require('App/ViewModels/Metadata/EnumFieldViewModel');
import DiscussionFieldViewModel = require('App/ViewModels/Metadata/DiscussionFieldViewModel');
import TextFieldViewModel = require('App/ViewModels/Metadata/TextFieldViewModel');

class uscMetadataRepositoryDesigner extends MetadataRepositoryBase {

    uscNotificationId: string;
    componentTextId: string;
    componentDateId: string;
    componentNumberId: string;
    componentCheckBoxId: string;
    componentTitleId: string;
    componentCommentId: string;
    componentEnumId: string;
    btnPublishId: string;
    btnDraftId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    ajaxManagerId: string;
    integerId: number;
    enumCounter: number;
    setiEnabledId: boolean;
    currentComponentToBeDragged: any;
    setiFieldCheckId: any;
    previousElementIE: any;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _btnPublish: Telerik.Web.UI.RadButton;
    private _btnDraft: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _manager: Telerik.Web.UI.RadWindowManager;

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME));
        this._serviceConfigurations = serviceConfigurations;
    }


    /*
     * ------------------------- Events -----------------------
     */

    /**
     * Inizializzazione
     */
    initialize() {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.ajaxManagerId);
        this.integerId = 1;

        this.setName();
        this.bindLoaded();

    }

    /**
     * Metodo che gestisce l'aggiunta di valori dell'enum
     * @param ev
     */
    addValue(ev: any) {
        let idParent: string = ev.target.parentNode.parentNode.id;
        let inputElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idParent, " :input.enumInput"))[0]);
        if (inputElement) {
            if (inputElement.value != "") {
                let node: HTMLElement = document.createElement("LI");
                let textNode = document.createTextNode(inputElement.value);
                node.appendChild(textNode);
                let listElement: HTMLOListElement = <HTMLOListElement>($("#" + idParent).find('ul')[0]);
                listElement.appendChild(node);
                inputElement.value = "";
            }
        }
    }

    /**
     * Metodo per fare il drag degli elementi nella pagina
     * @param ev
     */
    drag(ev: any) {

        ev.dataTransfer.setData("text", ev.target.id);
    }

    /**
     * Metodo per aggiungere le componenti al click
     * @param ev
     */
    click(ev: any) {
        this.addComponent(ev.target.id);
    }

    /**
     * Metodo scatenato al drop di elementi nella pagina
     * @param ev
     */
    drop(ev: any) {
        ev.preventDefault();
        let data = ev.dataTransfer.getData("text");
        this.addComponent(data);
    }

    /**
     * Aggiungo una componente alla lista di elementi
     * @param idComponent
     */
    addComponent(idComponent: string) {
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
        let cln = elmnt.cloneNode(true);
        cln.setAttribute("idParent", elmnt.id);
        cln.setAttribute("id", elmnt.id + this.integerId);

        this.integerId++;
        let close = cln.getElementsByClassName("close")[0];
        close.setAttribute("id", close.id + this.integerId);
        this.integerId++;
        if (cln.getElementsByClassName("controls")[1]) {
            let values = cln.getElementsByClassName("controls")[1];
            values.setAttribute("id", values.id + this.integerId);
            this.integerId++;
        }

        let content = document.getElementById("menuContent");
        content.appendChild(cln);

        this.bindLoaded();

        this.currentComponentToBeDragged = cln;
        this.setNewlyAddedComponentToMovable(cln);
        this.setNewlyAddedComponentToInputEditable(cln);

    }
    private setNewlyAddedComponentToInputEditable(element) {
        let data: any = [];
        let elementChildren = element.children;
        for (let i = 0; i < elementChildren.length; i++) {
            if (elementChildren[i] && this.hasClass(elementChildren[i], 'controls')) {
                let table = elementChildren[i].children;
                let input = table ? table[0].getElementsByTagName('input') : [];
                for (var j = 0; j < input.length; j++) {
                    data.push(input[j]);
                }
            }
        }
        this.registerFocusBlurEvents(data);
    }

    /**
     * Rimuovo una componente dalla lista
     * @param ev
     */
    removeItem(ev: any) {
        let xdiv: HTMLElement = document.getElementById(ev.target.id);
        let elementToClose = xdiv.parentNode;
        elementToClose.parentNode.removeChild(elementToClose);
    }

    /**
     * ------------------------ Methods -----------------------
     */


    /**
     * Imposto il campo nome di default
     */
    setName() {
        let elmnt: HTMLElement = document.getElementById(this.componentTitleId);
        let cln = elmnt.cloneNode(true);
        let content: HTMLElement = document.getElementById("menuContent");
        content.appendChild(cln);
        if (this.setiEnabledId) {
            let field: HTMLElement = document.getElementById(`${this.setiFieldCheckId}`);
            field.style.display = "block";
        }
    }



    /**
     * Metodo di preparazione del modello da salvare
     */
    prepareModel(): MetadataRepositoryModel {
        let content = $("#menuContent").children();
        let item = new MetadataRepositoryModel();
        let metadataModel = new MetadataDesignerViewModel();
        let baseField: BaseFieldViewModel;
        let enumField: EnumFieldViewModel;
        let textField: TextFieldViewModel;
        let discussionField: DiscussionFieldViewModel;

        let inputElement: HTMLInputElement;
        let inputKeyNameElement: HTMLInputElement;
        let requiredElement: HTMLInputElement;
        let showInResultsElement: HTMLInputElement;
        let hiddenField: HTMLInputElement;
        let readOnly: HTMLInputElement;
        let formatType: HTMLInputElement;

        if (!this.validFields()) {
            return null;
        }

        let setiField: any = $(`#seti_input_check`);

        $.each(content, (index: number, divElement: HTMLDivElement) => {
            inputElement = <HTMLInputElement>($("#".concat(divElement.id, " :input.form-control"))[0]);
            inputKeyNameElement = <HTMLInputElement>($("#".concat(divElement.id, " :input.form-control"))[1]);
            requiredElement = <HTMLInputElement>($("#".concat(divElement.id, " :input.checkBox"))[0]);
            showInResultsElement = <HTMLInputElement>($("#".concat(divElement.id, " :input.checkBox"))[1]);
            hiddenField = <HTMLInputElement>($("#".concat(divElement.id, " :input.checkBox"))[2]);
            readOnly = <HTMLInputElement>($("#".concat(divElement.id, " :input.checkBox"))[3]);
            formatType = <HTMLInputElement>($("#".concat(divElement.id, " option:selected"))[0]);
            let dataset: string = divElement.getAttribute("data-type");
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
                    textField.ShowInResults = showInResultsElement.checked;
                    textField.HiddenField = hiddenField.checked;
                    textField.ReadOnly = readOnly.checked;
                    textField.Position = index;
                    metadataModel.TextFields.push(textField);
                    break;
                case "Number":
                    baseField = new BaseFieldViewModel();
                    baseField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                    baseField.Label = inputElement.value;
                    baseField.Required = requiredElement.checked;
                    baseField.ShowInResults = showInResultsElement.checked;
                    baseField.HiddenField = hiddenField.checked;
                    baseField.ReadOnly = readOnly.checked;
                    baseField.FormatType = formatType.value;
                    baseField.Position = index;
                    metadataModel.NumberFields.push(baseField);
                    break;
                case "Date":
                    baseField = new BaseFieldViewModel();
                    baseField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                    baseField.Label = inputElement.value;
                    baseField.Required = requiredElement.checked;
                    baseField.ShowInResults = showInResultsElement.checked;
                    baseField.HiddenField = hiddenField.checked;
                    baseField.ReadOnly = readOnly.checked;
                    baseField.Position = index;
                    metadataModel.DateFields.push(baseField);
                    break;
                case "CheckBox":
                    baseField = new BaseFieldViewModel();
                    baseField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                    baseField.Label = inputElement.value;
                    baseField.Required = requiredElement.checked;
                    baseField.ShowInResults = showInResultsElement.checked;
                    baseField.HiddenField = hiddenField.checked;
                    baseField.ReadOnly = readOnly.checked;
                    baseField.Position = index;
                    metadataModel.BoolFields.push(baseField);
                    break;
                case "Enum":
                    enumField = new EnumFieldViewModel();
                    enumField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                    enumField.Label = inputElement.value;
                    enumField.Required = requiredElement.checked;
                    enumField.ShowInResults = showInResultsElement.checked;
                    enumField.HiddenField = hiddenField.checked;
                    enumField.ReadOnly = readOnly.checked;
                    enumField.Position = index;
                    enumField.Options = {};
                    $.each($("#" + divElement.id).find('li'), (index: number, listElement: HTMLOListElement) => {
                        enumField.Options[index + 1] = listElement.textContent;
                    });
                    metadataModel.EnumFields.push(enumField);
                    break;
                case "Comment":
                    discussionField = new DiscussionFieldViewModel();
                    discussionField.KeyName = inputKeyNameElement.value ? inputKeyNameElement.value : inputElement.value;
                    discussionField.Label = inputElement.value;
                    discussionField.Required = requiredElement.checked;
                    discussionField.ShowInResults = showInResultsElement.checked;
                    discussionField.HiddenField = hiddenField.checked;
                    discussionField.ReadOnly = readOnly.checked;
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
    }

    /**
     * Scateno l'evento di "Load Completed" del controllo
     */
    private bindLoaded(): void {
        $("#".concat(this.pageContentId)).data(this);
    }


    /**
     * verifico se sono presenti nel dom due componenti con lo stesso nome
     * @param labels
     */
    duplicateElemntsName(labels: string[]): boolean {
        let vlauesSoFar = Object.create(null);
        for (let i = 0; i < labels.length; ++i) {
            let value = labels[i];
            if (value in vlauesSoFar) {
                return true;
            }
            vlauesSoFar[value] = true;
        }
        return false;
    }

    /**
     * Verifico che non ci siano campi duplicati o vuoti
     */
    validFields(): boolean {
        let content = $("#menuContent").children();
        let inputElement: HTMLInputElement;
        let invalidField: boolean = false;
        let labels: string[] = [];
        $.each(content, (index: number, divElement: HTMLDivElement) => {
            inputElement = <HTMLInputElement>($("#".concat(divElement.id, " :input.form-control"))[0]);
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
    }

    /**
     * Carico un modello esistente nella pagina
     * @param metadataRepositoryModel
     */
    loadModel(metadataRepositoryModel: MetadataRepositoryModel) {
        let metadataViewModel: MetadataDesignerViewModel = JSON.parse(metadataRepositoryModel.JsonMetadata);
        let inputElement: HTMLInputElement;;
        let listElement: HTMLOListElement;
        let idCloned: string;

        this.clearPage();

        idCloned = this.cloneElement(this.componentTitleId, this.integerId);
        this.integerId++;
        inputElement = this.findStandardInputElement(idCloned, 0);
        inputElement.value = metadataRepositoryModel.Name;

        if (this.setiEnabledId) {
            let field: HTMLElement = document.getElementById(`${this.setiFieldCheckId}`);
            field.style.display = "block";
        }
        let setiField: any = $(`input#seti_input_check`);
        if (setiField) {
            setiField[0].checked = metadataViewModel.SETIFieldEnabled;
        }

        this.arrangeControlsInPosition(metadataViewModel, idCloned);

        this.setMovableComponents();
        this.bindLoaded();
    }

    private arrangeControlsInPosition(metadataViewModel: MetadataDesignerViewModel, idCloned: string) {
        let aggregatedSum: number = 0;        for (let arr in metadataViewModel) {            if (typeof metadataViewModel[arr] !== "boolean") {                aggregatedSum += metadataViewModel[arr].length;
            }
        }
        for (var i = 0; i <= aggregatedSum; i++) {
            let metadataField: any = null;
            let currentType;
            for (let arr in metadataViewModel) {

                currentType = arr;
                let obj = undefined;
                if (typeof (metadataViewModel[arr]) !== "boolean") {
                    obj = metadataViewModel[arr].filter(x => x.Position == i)[0];
                }
                if (obj) {
                    metadataField = obj;
                    break;
                }
            }

            switch (currentType) {
                case "TextFields":
                    idCloned = this.fillHTMLElement(this.componentTextId, metadataField.Position, metadataField, currentType);
                    break;
                case "DateFields":
                    idCloned = this.fillHTMLElement(this.componentDateId, metadataField.Position, metadataField, currentType);
                    break;
                case "NumberFields":
                    idCloned = this.fillHTMLElement(this.componentNumberId, metadataField.Position, metadataField, currentType);
                    break;
                case "BoolFields":
                    idCloned = this.fillHTMLElement(this.componentCheckBoxId, metadataField.Position, metadataField, currentType);
                    break;
                case "EnumFields":
                    idCloned = this.fillHTMLElement(this.componentEnumId, metadataField.Position, metadataField, currentType);
                    $.each(metadataField.Options, (index: number, option) => {
                        let node: HTMLElement = document.createElement("LI");
                        if (metadataField.Options[index] != "") {
                            this.createNewNode(metadataField.Options[index], node, idCloned);
                        }
                    });
                    break;
                case "DiscussionFields":
                    idCloned = this.fillHTMLElement(this.componentCommentId, metadataField.Position, metadataField, currentType);
                    break;
                default:
                    break;
            }
        }
    }

    /**
     * Popoplo un contollo con i dati del modello
     * @param idComponent
     * @param incrementalInteger
     * @param model
     */
    fillHTMLElement(idComponent: string, incrementalInteger: number, model: BaseFieldViewModel, currentType: string): string {
        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let inputElement: HTMLInputElement = this.findStandardInputElement(idCloned, 0);
        let inputKeyNameElement: HTMLInputElement = this.findStandardInputElement(idCloned, 1);
        let requiredElement: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 0);
        let showInResultsElement: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 1);
        let hiddenField: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 2);
        let readOnly: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 3);
        let formatType: HTMLSelectElement = this.findSelectControl(idCloned, 0);

        inputElement.value = model.Label;
        requiredElement.checked = model.Required;
        showInResultsElement.checked = model.ShowInResults;
        hiddenField.checked = model.HiddenField;
        readOnly.checked = model.ReadOnly;
        inputKeyNameElement.value = model.KeyName ? model.KeyName : "";

        if (formatType) {
            formatType.value = model.FormatType;
            formatType.disabled = true;
            formatType.title = "Il formato non è modificabile dopo l'inserimento.";
        }

        this.setDeleteComponentId(currentType, incrementalInteger);
        this.registerFocusBlurEvents([inputElement, inputKeyNameElement, requiredElement]);

        return idCloned;
    }

    private registerFocusBlurEvents(elements: Array<HTMLInputElement>) {
        for (let element of elements) {
            element.addEventListener('focus', (ev) => {
                this.removeSelectable(element, ev);
            })
            element.addEventListener('blur', (ev) => {
                this.addSelectable(element, ev);
            })
        }
    }

    private removeSelectable(element: any, ev: any) {
        let parent = this.findParentRecursive(element);
        let isIE = /(MSIE|Trident\/|Edge\/)/i.test(navigator.userAgent);
        parent.removeAttribute('draggable');
        element.focus();
        if (isIE) { //IE
            let range = element.createTextRange();
            range.collapse(false);
            range.select();
        } else { //others
            let range = document.createRange();
            range.collapse(false);
            window.getSelection().addRange(range);
        }
    }

    private addSelectable(element: HTMLInputElement, ev: any) {
        var parent = this.findParentRecursive(element);
        parent.setAttribute('draggable', 'true');
    }
    private findParentRecursive(element) {
        if (!element) {
            return undefined;
        }
        if (this.hasClass(element, 'element')) {
            return element;
        } else {
            return this.findParentRecursive(element.parentNode);
        }
    }
    /**
     * Quando il componente viene disegnato sul DOM, assegnare un ID univoco
     * @param incrementalInteger
     */
    private setDeleteComponentId(currentType: string, incrementalInteger: number) {
        switch (currentType) {
            case "TextFields":
                let closeElement: HTMLElement = document.getElementById("closeText");
                closeElement.id = `closeText${incrementalInteger}`;
                break;
            case "DateFields":
                let closeDate: HTMLElement = document.getElementById("closeDate");
                closeDate.id = `closeDate${incrementalInteger}`;
                break;
            case "NumberFields":
                let closeNumber: HTMLElement = document.getElementById("closeNumber");
                closeNumber.id = `closeNumber${incrementalInteger}`;
                break;
            case "BoolFields":
                let closeCheckBox: HTMLElement = document.getElementById("closeCheckBox");
                closeCheckBox.id = `closeCheckBox${incrementalInteger}`;
                break;
            case "EnumFields":
                let closeEnum: HTMLElement = document.getElementById("closeEnum");
                if (closeEnum) {
                    closeEnum.id = `closeEnum${incrementalInteger}`;
                }
                break;
            case "DiscussionFields":
                let closeComment: HTMLElement = document.getElementById("closeComment");
                closeComment.id = `closeComment${incrementalInteger}`;
                break;
            default:
                break;
        }
    }


    private setMovableComponents() {
        let menuSubItems: any = $("#menuContent").children();
        let isIE = /(MSIE|Trident\/|Edge\/)/i.test(navigator.userAgent);
        $.each(menuSubItems, (index, menuSubItem) => {
            if (index > 0) {
                /*Make Title non draggable*/
                menuSubItems[index].setAttribute('draggable', 'true');
                menuSubItems[index].classList.add('component');

                menuSubItem.addEventListener('dragstart', (ev) => {
                    this.currentComponentToBeDragged = ev.target;
                });
                menuSubItem.addEventListener('dragover', (ev) => {
                    this.dragOver(ev);
                });
                menuSubItem.addEventListener('dragleave', (ev) => {
                    this.dragLeave(ev);
                });

                if (!isIE) {
                    menuSubItem.addEventListener('drop', (ev) => {
                        /*needed because Firefox doesn't know dragend event*/
                        this.dropFinished(ev);
                    });
                } else {
                    menuSubItem.addEventListener('dragenter', (ev) => {
                        this.dragEnterIE(ev);
                    });
                    menuSubItem.addEventListener('dragend', (ev) => {
                        /*needed because IE doesn't know drop event*/
                        this.dragEndFinished(menuSubItems, ev);
                        this.animateLeaveHover(this.previousElementIE);
                        this.previousElementIE = null;
                    });
                }
            }
        });
    }
    private setNewlyAddedComponentToMovable(cln: any) {
        let isIE = /(MSIE|Trident\/|Edge\/)/i.test(navigator.userAgent);

        cln.setAttribute('draggable', 'true');
        cln.classList.add('component');
        cln.addEventListener('dragover', (ev) => {
            this.dragOver(ev);
        });
        cln.addEventListener('dragleave', (ev) => {
            this.dragLeave(ev);
        });
        cln.addEventListener('dragstart', (ev) => {
            this.currentComponentToBeDragged = ev.target;
            $("#".concat(this.pageContentId)).data(this);
        });

        if (isIE) {
            cln.addEventListener('dragenter', (ev) => {
                this.dragEnterIE(ev);
            });
            cln.addEventListener('dragend', (evt) => {
                let menuSubItems: any = $("#menuContent").children();
                let draggingElement: any = evt.target;
                let x = evt.clientX;
                let y = evt.clientY;
                let elementMouseDraggedTo: any = document.elementFromPoint(x, y);
                if (!elementMouseDraggedTo) {
                    return;
                }
                let { dragToIndex, dragFromIndex } = this.getNewCoordinates(menuSubItems, draggingElement, elementMouseDraggedTo);
                if (dragToIndex == -1 || dragFromIndex == -1) {
                    return; /*invalid position to move*/
                }
                let newMenuSubItems: any = this.renewMenuSubItems(menuSubItems, dragFromIndex, dragToIndex);
                let content = document.getElementById("menuContent");
                /*draw the newly changed items on the canvas*/
                $.each(menuSubItems, (index: number, menuSubItem) => {
                    content.removeChild(menuSubItems[index]);
                    content.appendChild(newMenuSubItems[index]);
                });

                this.animateLeaveHover(this.previousElementIE);
                this.previousElementIE = null;
            });
        } else {
            cln.addEventListener('drop', (ev) => {
                this.dropFinished(ev);
            });
        }

    }
    dragEnterIE(evt: any) {
        this.showDroppableSelectionIE(evt);
    }
    dragOver(evt: any) {
        this.showDroppableSelection(evt);
    }

    dragLeave(evt: any) {
        this.hideDroppableSelection(evt);
    }

    dropFinished(evt: any) {
        this.hideDroppableSelection(evt);

        let indexDestination = -1;
        let indexStarting = -1;

        let menuSubItems: any = $("#menuContent").children();

        let thisObj = $("#".concat(this.pageContentId)).data();

        $.each(menuSubItems, (index, mouseSubItem) => {
            if (mouseSubItem == thisObj.currentComponentToBeDragged) {
                indexStarting = index;
            }
        });

        let elementMouseDraggedTo = evt.target.parentNode;
        $.each(menuSubItems, (index, mouseSubItem) => {

            if ((elementMouseDraggedTo.nodeName == "UL") && (elementMouseDraggedTo.parentElement == menuSubItems[index].getElementsByClassName("controls")[1])) {
                /*when dealing with enums that have big lists and the drop is on them*/
                indexDestination = index == 0 ? 1 : index; /*this will ensure that the title alwayas stays on top of the stack.*/
            } else if (mouseSubItem == evt.target.parentElement) {
                indexDestination = index;
            }

        });
        if (indexDestination == -1 || indexStarting == -1) {
            return;/*invalid position to move*/
        }

        let newMenuSubItems: any = this.renewMenuSubItems(menuSubItems, indexStarting, indexDestination);
        let content = document.getElementById("menuContent");

        /*draw the newly changed items on the canvas*/
        $.each(menuSubItems, (index: number, menuSubItem) => {
            content.removeChild(menuSubItems[index]);
            content.appendChild(newMenuSubItems[index]);
        });
    }

    dragEndFinished(menuSubItems: any, evt: any) {
        let draggingElement: any = evt.target;
        let x = evt.clientX;
        let y = evt.clientY;
        let elementMouseDraggedTo: any = document.elementFromPoint(x, y);

        if (!elementMouseDraggedTo) {
            return;
        }

        let { dragToIndex, dragFromIndex } = this.getNewCoordinates(menuSubItems, draggingElement, elementMouseDraggedTo);

        if (dragToIndex == -1 || dragFromIndex == -1) {
            return;/*invalid position to move*/
        }

        let newMenuSubItems: any = this.renewMenuSubItems(menuSubItems, dragFromIndex, dragToIndex);
        let content = document.getElementById("menuContent");

        /*draw the newly changed items on the canvas*/
        $.each(menuSubItems, (index: number, menuSubItem) => {
            content.removeChild(menuSubItems[index]);
            content.appendChild(newMenuSubItems[index]);
        });
    }

    /* Helpers */
    private getNewCoordinates(menuSubItems: any, draggingElement: any, elementMouseDraggedTo: any) {
        let dragToIndex = -1;
        let dragFromIndex = 1;
        $.each(menuSubItems, (index, mouseSubItem) => {
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
        return { dragToIndex, dragFromIndex };
    }

    private renewMenuSubItems(oldMenuSubItems, oldIndex, newIndex) {
        if (newIndex >= oldMenuSubItems.length) {
            let i = newIndex - oldMenuSubItems.length + 1;
            while (i--) {
                oldMenuSubItems.push(undefined);
            }
        }
        oldMenuSubItems.splice(newIndex, 0, oldMenuSubItems.splice(oldIndex, 1)[0]);
        return oldMenuSubItems;
    };

    private hideDroppableSelection(evt: any) {
        if (this.hasClass(evt.target, "component")) {

            this.animateLeaveHover(evt.target);
        }
        else if (this.hasClass(evt.target.parentElement, "ul")) {
            this.animateLeaveHover(evt.target.parentElement.parentElement);
        }
        else {
            this.animateLeaveHover(evt.target.parentElement);
        }
    }

    private showDroppableSelection(evt: any) {
        if (this.hasClass(evt.target.parentElement, "ul")) {
            this.animateEnterHover(evt.target.parentElement.parentElement);
        }
        else if (this.hasClass(evt.target, "component")) {
            this.animateEnterHover(evt.target);
        }
        else {
            this.animateEnterHover(evt.target.parentElement);
        }
    }

    private showDroppableSelectionIE(element: any) {
        if (this.hasClass(element.target.parentElement, "ul")) {
            this.animatEnterHoverIE(element.target.parentElement.parentElement);
        }
        else if (this.hasClass(element.target, "component")) {
            this.animatEnterHoverIE(element.target);
        }
        else {
            this.animatEnterHoverIE(element.target.parentElement);
        }


    }

    private animatEnterHoverIE(element) {
        if (!this.previousElementIE) {
            this.previousElementIE = element;
            this.animateLeaveHover(element);
        }

        if (this.previousElementIE !== element) {
            this.animateLeaveHover(this.previousElementIE);
            this.previousElementIE = element;
            this.animateEnterHover(element);
        }
    }

    private animateEnterHover(element) {
        element.style.transform = "scale(0.98,0.98)";
        element.style.transitionDuration = "200ms";
        element.style.transitionTimingFunction = "ease-out";
        element.style.opacity = "0.98";

    }

    private animateLeaveHover(element) {
        element.style.transform = "scale(1,1)";
        element.style.transitionDuration = "200ms";
        element.style.transitionTimingFunction = "ease-in";
        element.style.opacity = "1";
    }

    private hasClass(element, className) {
        return (' ' + element.className + ' ').indexOf(' ' + className + ' ') > -1;
    }

    /*End helpers*/
}

export = uscMetadataRepositoryDesigner;