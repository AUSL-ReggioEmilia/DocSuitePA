/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />/// <reference path="../scripts/typings/moment/moment.d.ts" />/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />import ServiceConfiguration = require('App/Services/ServiceConfiguration');import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');import ExceptionDTO = require('App/DTOs/ExceptionDTO');import UscErrorNotification = require('UserControl/uscErrorNotification');import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
class uscMetadataRepositorySummary extends MetadataRepositoryBase {    uscNotificationId: string;    componentTextId: string;
    componentDateId: string;
    componentNumberId: string;
    componentCheckBoxId: string;
    componentTitleId: string;
    componentCommentId: string;
    componentEnumId: string;    integerId: number;    pageContentId: string;    setiIntegrationEnabledId: boolean;    private _serviceConfigurations: ServiceConfiguration[];    /**     * Costruttore     * @param serviceConfigurations     */    constructor(serviceConfigurations: ServiceConfiguration[]) {        super(ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME));        this._serviceConfigurations = serviceConfigurations;    }    /**     * ------------------------------------ Events ---------------------------     */    /**     * inizializzazione     */    initialize() {        super.initialize();        this.integerId = 1;        this.bindLoaded();    }    /**     * ----------------------------------- Methods ----------------------------     */    /**     * funzione che carica le componenti della pagina     * @param idMetadataRepository     */    loadMetadataRepository(idMetadataRepository: string) {        this._service.getFullModelById(idMetadataRepository,            (data: MetadataRepositoryModel) => {                if (data) {                    this.loadPageItems(data);                }            },            (exception: ExceptionDTO) => {                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();                if (exception && uscNotification && exception instanceof ExceptionDTO) {                    if (!jQuery.isEmptyObject(uscNotification)) {                        uscNotification.showNotification(exception);                    }                }            });    }    /**     * funzione che aggiunge tutte le componenti corrispondenti al Json del menu     */    loadPageItems(metadataRepositoryModel: MetadataRepositoryModel) {        let metadataViewModel: MetadataDesignerViewModel = JSON.parse(metadataRepositoryModel.JsonMetadata);        let element;        let content: HTMLElement = document.getElementById("menuContent");        let idCloned: string;        let labelElement: HTMLLabelElement;        let requiredElement: HTMLInputElement;        this.clearPage();        idCloned = this.cloneElement(this.componentTitleId, this.integerId);        this.integerId++;        labelElement = this.findLabelElement(idCloned, 0);        labelElement.textContent = metadataRepositoryModel.Name;        let setiEnabled: HTMLElement = document.getElementById("setiFieldId");        if (this.setiIntegrationEnabledId && metadataViewModel.SETIFieldEnabled !== undefined) {/*Ensure that the message is not visible if the SetiIntegrationEnabled is set to false*/            setiEnabled.innerText = metadataViewModel.SETIFieldEnabled ? "(Integrazione SETI abilitata)" : "(Integrazione SETI non abilitata)";
        }        this.arrangeControlsInPosition(metadataViewModel, idCloned);    }
    private arrangeControlsInPosition(metadataViewModel: MetadataDesignerViewModel, idCloned: string) {
        let aggregatedSum: number = 0;        for (let arr in metadataViewModel) {            if (typeof (metadataViewModel[arr]) !== "boolean") {                aggregatedSum += metadataViewModel[arr].length;
            }        }        for (var i = 0; i <= aggregatedSum; i++) {            let metadataField: any = null;            let currentType;            for (let arr in metadataViewModel) {
                currentType = arr;

                let obj = undefined;
                if (typeof (metadataViewModel[arr]) !== "boolean") {
                    obj = metadataViewModel[arr].filter(x => x.Position == i)[0];
                }
                if (obj) {
                    metadataField = obj;
                    break;
                }
            }            switch (currentType) {
                case MetadataRepositoryBase.CONTROL_TEXT_FIELD:
                    idCloned = this.fillHTMLElement(this.componentTextId, metadataField.Position, metadataField);
                    break;
                case MetadataRepositoryBase.CONTROL_DATE_FIELD:
                    idCloned = this.fillHTMLElement(this.componentDateId, metadataField.Position, metadataField);
                    break;
                case MetadataRepositoryBase.CONTROL_NUMBER_FIELD:
                    idCloned = this.fillHTMLElement(this.componentNumberId, metadataField.Position, metadataField);
                    break;
                case MetadataRepositoryBase.CONTROL_BOOL_FIELD:
                    idCloned = this.fillHTMLElement(this.componentCheckBoxId, metadataField.Position, metadataField);
                    break;
                case MetadataRepositoryBase.CONTROL_ENUM_FIELD:
                    idCloned = this.fillHTMLElement(this.componentEnumId, metadataField.Position, metadataField);
                    $.each(metadataField.Options, (index: number, option) => {                        let node: HTMLElement = document.createElement("LI");                        if (metadataField.Options[index] != "") {                            this.createNewNode(metadataField.Options[index], node, idCloned);
                        }                    });
                    break;
                case MetadataRepositoryBase.CONTROL_DISCUSION_FIELD:
                    idCloned = this.fillHTMLElement(this.componentCommentId, metadataField.Position, metadataField);
                    break;
                default:
                    break;
            }        }
    }    /**
     * Scateno l'evento di "Load Completed" del controllo
     */
    private bindLoaded(): void {
        $("#".concat(this.pageContentId)).data(this);
    }    fillHTMLElement(idComponent: string, incrementalInteger: number, model: BaseFieldViewModel): string {        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let labelElement: HTMLLabelElement = this.findLabelElement(idCloned, 1);        let requiredElement: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 0);        let showInResultsElement: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 1);        let hiddenFieldLabelElement: HTMLLabelElement = this.findLabelElement(idCloned, 6);        let hiddenFieldElement: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 2);        let readOnlyElement: HTMLInputElement = this.findInputCheckBoxElement(idCloned, 3);        let keynameElement: HTMLLabelElement = this.findLabelElement(idCloned, 3);        let formatType: HTMLSelectElement = this.findSelectControl(idCloned, 0);        labelElement.textContent = model.Label;        keynameElement.textContent = model.KeyName;
        requiredElement.checked = model.Required;
        showInResultsElement.checked = model.ShowInResults;
        hiddenFieldElement.checked = model.HiddenField;
        readOnlyElement.checked = model.ReadOnly;

        if (formatType) {
            formatType.value = model.FormatType;
        }
        
        hiddenFieldElement.style.display = hiddenFieldElement.checked ? "inline-block" : "none";
        hiddenFieldLabelElement.style.display = hiddenFieldElement.checked ? "inline-block" : "none";

        return idCloned;
    }}export = uscMetadataRepositorySummary;