/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import MetadataViewModel = require('App/ViewModels/Metadata/MetadataViewModel');
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
        this.addComponent(data)
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
    }



    /**
     * Metodo di preparazione del modello da salvare
     */
    prepareModel(): MetadataRepositoryModel {
        let content = $("#menuContent").children();
        let item = new MetadataRepositoryModel();
        let metadataModel = new MetadataViewModel();
        let baseField: BaseFieldViewModel;
        let enumField: EnumFieldViewModel;
        let textField: TextFieldViewModel;
        let discussionField: DiscussionFieldViewModel;

        let inputElement: HTMLInputElement;
        let requiredElement: HTMLInputElement;

        if (!this.validFields()) {
            return null;
        }

        $.each(content, (index: number, divElement: HTMLDivElement) => {
            inputElement = <HTMLInputElement>($("#".concat(divElement.id, " :input.form-control"))[0]);
            requiredElement = <HTMLInputElement>($("#".concat(divElement.id, " :input.checkBox"))[0]);
            let dataset: string = divElement.getAttribute("data-type");

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
                    $.each($("#" + divElement.id).find('li'), (index: number, listElement: HTMLOListElement) => {
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
        let metadataViewModel: MetadataViewModel = JSON.parse(metadataRepositoryModel.JsonMetadata);

        let inputElement: HTMLInputElement;;
        let listElement: HTMLOListElement;
        let idCloned: string;

        this.clearPage();

        idCloned = this.cloneElement(this.componentTitleId, this.integerId);        this.integerId++;        inputElement = this.findStandardInputElement(idCloned, 0);        inputElement.value = metadataRepositoryModel.Name;        $.each(metadataViewModel.TextFields, (index: number, textFieldViewModel) => {            idCloned = this.fillHTMLElement(this.componentTextId, this.integerId, textFieldViewModel);            this.integerId++;        });        $.each(metadataViewModel.NumberFields, (index: number, numberFieldViewModel) => {            idCloned = this.fillHTMLElement(this.componentNumberId, this.integerId, numberFieldViewModel);            this.integerId++;                    });        $.each(metadataViewModel.DateFields, (index: number, dateFieldViewModel) => {            idCloned = this.fillHTMLElement(this.componentDateId, this.integerId, dateFieldViewModel);            this.integerId++;        });        $.each(metadataViewModel.BoolFields, (index: number, boolFieldViewModel) => {            idCloned = this.fillHTMLElement(this.componentCheckBoxId, this.integerId, boolFieldViewModel);            this.integerId++;        });        $.each(metadataViewModel.EnumFields, (index: number, enumFieldViewModel) => {            idCloned = this.fillHTMLElement(this.componentEnumId, this.integerId, enumFieldViewModel);            this.integerId++;            $.each(enumFieldViewModel.Options, (index: number, option) => {                let node: HTMLElement = document.createElement("LI");                if (enumFieldViewModel.Options[index] != "") {                    this.createNewNode(enumFieldViewModel.Options[index], node, idCloned);
                }            });        });        $.each(metadataViewModel.DiscussionFields, (index: number, discussionFieldViewModel) => {            idCloned = this.fillHTMLElement(this.componentCommentId, this.integerId, discussionFieldViewModel);            this.integerId++;

        });

        this.bindLoaded();
    }

    /**
     * Popoplo un contollo con i dati del modello
     * @param idComponent
     * @param incrementalInteger
     * @param model
     */
    fillHTMLElement(idComponent: string, incrementalInteger: number, model: BaseFieldViewModel):string {

        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let inputElement: HTMLInputElement = this.findStandardInputElement(idCloned, 0);        let requiredElement: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 0);        inputElement.value = model.Label;        requiredElement.checked = model.Required;

        return idCloned;
    }
}

export = uscMetadataRepositoryDesigner;