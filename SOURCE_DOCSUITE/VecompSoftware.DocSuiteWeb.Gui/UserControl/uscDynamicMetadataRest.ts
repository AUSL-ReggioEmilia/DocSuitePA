import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
import MetadataValueViewModel = require('App/ViewModels/Metadata/MetadataValueViewModel');
import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import EnumFieldViewModel = require('App/ViewModels/Metadata/EnumFieldViewModel');
import DiscussionFieldViewModel = require('App/ViewModels/Metadata/DiscussionFieldViewModel');
import TextFieldViewModel = require('App/ViewModels/Metadata/TextFieldViewModel');
import CommentFieldViewModel = require('App/ViewModels/Metadata/CommentFieldViewModel');
import SetiContactModel = require('App/Models/Commons/SetiContactModel');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');


class uscDynamicMetadataRest extends MetadataRepositoryBase {

    pageContentId: string;
    uscNotificationId: string;
    componentTextId: string;
    componentDateId: string;
    componentNumberId: string;
    componentCheckBoxId: string;
    componentCommentId: string;
    componentEnumId: string;
    currentUser: string;
    validationEnabled: boolean;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _currentMetadataRepositoryModel: MetadataRepositoryModel
    private _discussions: DiscussionFieldViewModel[];

    private controlsCounter = 0;

    static FIELD_LABEL: string = "FieldLabel";
    static REQUIRED_FIELD: string = "RequiredField";
    static DEFAULT_VALUE: string = "DefaultValue";
    static KEY_NAME: string = "KeyName";

    /**
     * Costruttore
     *  @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME));
        this._serviceConfigurations = serviceConfigurations;
    }


    /*
     * ---------------------------- Events ------------------------- 
     */


    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();

        this.bindLoaded();
    }


    /*
     * -------------------------- Methods -------------------------- 
     */

    /**
     * Eseguo il Bind della pagina
     */
    private bindLoaded(): void {
        $("#".concat(this.pageContentId)).data(this)
    }

    getMetadataRepository(): MetadataRepositoryModel {
        return this._currentMetadataRepositoryModel;
    }

    /**
     * Carico il modello nello usercontrol
     * @param idMetadataRepository
     */
    loadMetadataRepository(idMetadataRepository: string) {
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY);
        this._service.getFullModelById(idMetadataRepository,
            (data: MetadataRepositoryModel) => {
                if (data) {
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY, idMetadataRepository);
                    this._currentMetadataRepositoryModel = data;
                    this.loadPageItems(data.JsonMetadata, null);
                }
            },
            (exception: ExceptionDTO) => {
                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                if (exception && uscNotification && exception instanceof ExceptionDTO) {
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                }
            });
    }

    /**
     * Populato il modello nello usercontrol
     * @param idMetadataRepository
     */
    populateMetadataRepository(model: SetiContactModel, jsonMetadata?: string) {
        let metadataViewModel: MetadataDesignerViewModel;
        if (jsonMetadata) {
            metadataViewModel = JSON.parse(jsonMetadata);
        } else {
            metadataViewModel = JSON.parse(this._currentMetadataRepositoryModel.JsonMetadata);
        }

        if (metadataViewModel.TextFields) {
            $.each(metadataViewModel.TextFields, (index: number, textFieldViewModel) => {
                let elm = $(`[keyname=${textFieldViewModel.KeyName}]`);
                elm.val(model[textFieldViewModel.KeyName]);
            });
        }
        if (metadataViewModel.DateFields) {
            $.each(metadataViewModel.DateFields, (index: number, dateFieldViewModel) => {
                let elm = $(`[keyname=${dateFieldViewModel.KeyName}]`);
                elm.val(model[dateFieldViewModel.KeyName]);
            });
        }
        if (metadataViewModel.NumberFields) {
            $.each(metadataViewModel.NumberFields, (index: number, numberFieldViewModel) => {
                let elm = $(`[keyname=${numberFieldViewModel.KeyName}]`);
                elm.val(model[numberFieldViewModel.KeyName]);
            });
        }
        if (metadataViewModel.BoolFields) {
            $.each(metadataViewModel.BoolFields, (index: number, boolFieldViewModel) => {
                let elm: any = $(`[keyname=${boolFieldViewModel.KeyName}]`);
                elm[0].checked = model[boolFieldViewModel.KeyName];
            });
        }
        this._discussions = metadataViewModel.DiscussionFields;

        if (metadataViewModel.DiscussionFields) {
            $.each(metadataViewModel.DiscussionFields, (index: number, discussionFieldViewModel) => {
                let elm = $(`[keyname=${discussionFieldViewModel.KeyName}]`);
                elm.val(model[discussionFieldViewModel.KeyName]);
            });
        }

        if (metadataViewModel.EnumFields) {
            $.each(metadataViewModel.EnumFields, (index: number, enumFieldViewModel) => {
                let elm: any = $(`[keyname=${enumFieldViewModel.KeyName}]`);
                for (let i = 0; i < elm[0].options.length; i++) {
                    if (elm[0].options[i].text === model[enumFieldViewModel.KeyName]) {
                        elm[0].selectedIndex = i;
                        break;
                    }
                }
            });
        }
    }

    /**
     * Carico gli elementi della pagina
     * @param metadataRepositoryModel
     */
    loadPageItems(jsonDesignerMetadata: string, jsonValueMetadataModels: string) {
        let metadataDesignerModel: MetadataDesignerViewModel = JSON.parse(jsonDesignerMetadata);
        let metadataValues: MetadataValueViewModel[] = [];
        if (jsonValueMetadataModels) {
            metadataValues = JSON.parse(jsonValueMetadataModels);
        }

        this.clearPage();
        this.arrangeControlsInPosition(metadataDesignerModel, metadataValues);
        this.bindLoaded();
    }


    private arrangeControlsInPosition(metadataDesignerModel: MetadataDesignerViewModel, metadataValues: MetadataValueViewModel[]) {
        let aggregatedSum: number = 0;
        for (let arr in metadataDesignerModel) {
            if (typeof (metadataDesignerModel[arr]) !== "boolean") {
                aggregatedSum += metadataDesignerModel[arr].length;
            }
        }
        this._discussions = metadataDesignerModel.DiscussionFields;
        for (var i = 0; i <= aggregatedSum; i++) {
            let metadataField: any = null;
            let currentType;
            for (let arr in metadataDesignerModel) {

                currentType = arr;
                let obj = undefined;
                if (typeof metadataDesignerModel[arr] !== "boolean") {
                    obj = metadataDesignerModel[arr].filter(x => x.Position == i)[0];
                }
                if (obj) {
                    metadataField = obj;
                    break;
                }
            }

            if (!metadataField) {
                continue;
            }

            let currentValue: string = null;
            if (metadataValues && metadataField) {
                let currentMetadataValue: MetadataValueViewModel = metadataValues.filter(x => x.KeyName == metadataField.KeyName)[0];
                if (currentMetadataValue) {
                    currentValue = currentMetadataValue.Value;
                }
            }

            switch (currentType) {
                case MetadataRepositoryBase.CONTROL_TEXT_FIELD:
                    this.fillHTMLGenericElement(this.componentTextId, this.controlsCounter, metadataField, currentValue, "riTextBox");
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_DATE_FIELD:
                    this.fillDateElement(this.componentDateId, this.controlsCounter, metadataField, currentValue, "riTextBox");
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_NUMBER_FIELD:
                    this.fillHTMLGenericElement(this.componentNumberId, this.controlsCounter, metadataField, currentValue, "riTextBox");
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_BOOL_FIELD:
                    this.fillHTMLGenericElement(this.componentCheckBoxId, this.controlsCounter, metadataField, currentValue, "form-control");
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_ENUM_FIELD:
                    this.fillHTMLEnumElement(this.componentEnumId, this.controlsCounter, metadataField, currentValue, "ddlOptions");
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_DISCUSION_FIELD:
                    this.fillHTMLGenericElement(this.componentCommentId, this.controlsCounter, metadataField, currentValue, "riTextBox");
                    this.controlsCounter++;
                    break;
                default:
                    break;
            }
        }
    }
    /**
     * Clono un elemento generico e lo completo con i vari attributi
     * @param idComponent
     * @param incrementalIdentifier
     * @param model
     * @param cssClass
     */
    fillHTMLGenericElement(idComponent: string, incrementalIdentifier: number, model: BaseFieldViewModel, value: string, cssClass: string) {
        let idCloned: string = this.cloneElement(idComponent, incrementalIdentifier);
        let clonedElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idCloned))[0]);
        clonedElement.style.display = "";
        this.prepareLabel(idCloned, 0, model.Label);
        let inputElement: HTMLInputElement = this.findGenericInputControl(idCloned, 0, cssClass)

        this.setDefaultValues(inputElement, model);
        this.setRequiredAttribute(inputElement, model);
        inputElement.setAttribute(uscDynamicMetadataRest.KEY_NAME, model.KeyName);

        if (model.DefaultValue) {
            inputElement.value = model.DefaultValue;
            inputElement.checked = (model.DefaultValue.toLowerCase() == "true");
        }

        if (value) {
            inputElement.value = value;
            inputElement.checked = (value.toLowerCase() == "true");
        }
    };

    /**
     * Clono l'elemento ENUM e lo completo con i vari attributi
     * @param idComponent
     * @param incrementalIdentifier
     * @param model
     * @param cssClass
     */
    fillHTMLEnumElement(idComponent: string, incrementalIdentifier: number, model: EnumFieldViewModel, value: string, cssClass: string) {

        let idCloned: string = this.cloneElement(idComponent, incrementalIdentifier);
        let clonedElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idCloned))[0]);
        clonedElement.style.display = "";
        this.prepareLabel(idCloned, 0, model.Label);
        let ddlElement: HTMLSelectElement = this.findSelectControl(idCloned, 0)
        ddlElement.setAttribute("id", "id".concat(incrementalIdentifier.toString()));
        incrementalIdentifier++;

        ddlElement.setAttribute(uscDynamicMetadataRest.REQUIRED_FIELD, "false");
        ddlElement.setAttribute(uscDynamicMetadataRest.KEY_NAME, model.KeyName);
        if (model.Required) {
            ddlElement.required = true;
            ddlElement.setAttribute(uscDynamicMetadataRest.REQUIRED_FIELD, "true");
        }


        $("#" + ddlElement.id).append($('<option>', {
            value: 0,
            text: ""
        }));

        if (model.DefaultValue && !value) {
            ddlElement.setAttribute(uscDynamicMetadataRest.DEFAULT_VALUE, model.DefaultValue);
        }

        if (model.Options) {
            $.each(model.Options, (innerIndex: number, v) => {
                let isSelected: boolean = (v == value)
                $("#" + ddlElement.id).append($('<option>', {
                    value: innerIndex + 1,
                    text: v,
                    selected: isSelected
                }));
            });
        }        
    };

    /**
     * Popolo il campo data
     * @param idComponent
     * @param incrementalIdentifier
     * @param model
     * @param cssClass
     */
    fillDateElement(idComponent: string, incrementalIdentifier: number, model: BaseFieldViewModel, value: string, cssClass: string) {
        let idCloned: string = this.cloneElement(idComponent, incrementalIdentifier);
        let clonedElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idCloned))[0]);
        clonedElement.style.display = "";
        let inputElement: HTMLInputElement = this.findGenericInputControl(idCloned, 0, cssClass)
        let labelValidator: HTMLLabelElement = this.findLabelElement(idCloned, 1);
        this.prepareLabel(idCloned, 0, model.Label);
        this.setDefaultValues(inputElement, model);

        if (value) {
            inputElement.value = value;
        }
        this.setRequiredAttribute(inputElement, model);
        $("#".concat(idCloned, " :input.", cssClass)).on("blur", { idParent: idCloned, class: cssClass }, this.onBlurEvent);
        inputElement.setAttribute(uscDynamicMetadataRest.KEY_NAME, model.KeyName);
    }

    /**
     * evento al onfocusout del campo data
     * @param event
     */
    onBlurEvent(event) {
        let idParent: string = event.data.idParent;
        let cssClass: string = event.data.class;
        let inputElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idParent, " :input.", cssClass))[0]);
        let labelValidator: HTMLLabelElement = <HTMLLabelElement>$("#" + idParent).find('label')[1];

        if (inputElement.value) {
            let dateValue: Date = new Date(inputElement.value);
            if (dateValue && moment(dateValue).isValid()) {
                labelValidator.style.visibility = "hidden";
            }
            else {
                labelValidator.style.visibility = "visible";
            }
        }
        return false;
    }

    /**
     * Preparo il label
     * @param idParent
     * @param position
     * @param labelCaption
     */
    prepareLabel(idParent: string, position: number, labelCaption: string) {
        let labelElement: HTMLLabelElement = this.findLabelElement(idParent, position);
        labelElement.textContent = labelCaption.concat(": ");
        labelElement.setAttribute(uscDynamicMetadataRest.FIELD_LABEL, labelCaption);
    }


    /**
     * Imposto alcuni default comuni dei campi di input
     * @param element
     * @param model
     */
    setDefaultValues(element: HTMLInputElement, model: BaseFieldViewModel) {
        if (model.DefaultValue) {
            element.setAttribute(uscDynamicMetadataRest.DEFAULT_VALUE, model.DefaultValue);
        }

        if (model.DefaultValue) {
            element.value = model.DefaultValue;
            element.checked = (model.DefaultValue.toLowerCase() == "true");
        }
    }

    /**
     * Imposto l'attributo required
     * @param element
     * @param model
     */
    setRequiredAttribute(element: HTMLInputElement, model: BaseFieldViewModel) {
        element.setAttribute(uscDynamicMetadataRest.REQUIRED_FIELD, "false");
        if (model.Required) {
            element.required = true;
            element.setAttribute(uscDynamicMetadataRest.REQUIRED_FIELD, "true");
        }
    }

    /**
     * Recupero i dati dalla pagina
     */
    bindModelFormPage(setiField?: boolean): [string,string] {

        let content = $("#menuContent").children();

        if (content.length < 1) {
            return ["", ""];
        }

        let metadataModel = new MetadataDesignerViewModel();
        let baseField: BaseFieldViewModel;
        let enumField: EnumFieldViewModel;
        let textField: TextFieldViewModel;
        let currentDiscussion: DiscussionFieldViewModel;

        let currentValue: MetadataValueViewModel;
        let currentValues: MetadataValueViewModel[] = [];

        let inputElement: HTMLInputElement;
        let labelElement: HTMLLabelElement;

        let missingRequiredFields: string[] = [];
        let invalidInput: string[] = [];

        if (setiField) {
            metadataModel.SETIFieldEnabled = setiField;
        }

        $.each(content, (index: number, divElement: HTMLDivElement) => {

            labelElement = this.findLabelElement(divElement.id, 0);
            let dataset: string = divElement.getAttribute("data-type");
            switch (dataset) {
                case "Text":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "riTextBox");
                    textField = new TextFieldViewModel();                    
                    textField.Position = index;
                    textField.KeyName = inputElement.attributes[uscDynamicMetadataRest.KEY_NAME].value;
                    if (inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE)) {
                        textField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE);
                    }
                    textField.Label = labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL);
                    textField.Required = (inputElement.getAttribute(uscDynamicMetadataRest.REQUIRED_FIELD) == "true");
                    if (textField.Required && !inputElement.value) {
                        missingRequiredFields.push(textField.Label);
                    }

                    currentValue = new MetadataValueViewModel();
                    currentValue.KeyName = textField.KeyName;
                    currentValue.Value = inputElement.value;
                    currentValues.push(currentValue);

                    metadataModel.TextFields.push(textField);
                    break;
                case "Number":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "riTextBox");
                    baseField = new BaseFieldViewModel();
                    baseField.Position = index;
                    baseField.KeyName = inputElement.attributes[uscDynamicMetadataRest.KEY_NAME].value;
                    if (inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE)) {
                        baseField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE);
                    }
                    baseField.Label = labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL);
                    baseField.Required = (inputElement.getAttribute(uscDynamicMetadataRest.REQUIRED_FIELD) == "true");
                    if (baseField.Required && !inputElement.value) {
                        missingRequiredFields.push(baseField.Label);
                    }
                    if (inputElement.value && isNaN(parseInt(inputElement.value))) {
                        invalidInput.push(baseField.Label);
                    }

                    currentValue = new MetadataValueViewModel();
                    currentValue.KeyName = baseField.KeyName;
                    currentValue.Value = inputElement.value;
                    currentValues.push(currentValue);

                    metadataModel.NumberFields.push(baseField);
                    break;
                case "Date":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "riTextBox");
                    baseField = new BaseFieldViewModel();
                    baseField.KeyName = inputElement.attributes[uscDynamicMetadataRest.KEY_NAME].value;
                    baseField.Position = index;
                    if (inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE)) {
                        baseField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE);
                    }
                    baseField.Label = labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL);
                    baseField.Required = (inputElement.getAttribute(uscDynamicMetadataRest.REQUIRED_FIELD) == "true");
                    if (baseField.Required && !inputElement.value) {
                        missingRequiredFields.push(baseField.Label);
                    }
                    if (inputElement.value && !moment(inputElement.value).isValid()) {
                        invalidInput.push(baseField.Label);
                    }

                    currentValue = new MetadataValueViewModel();
                    currentValue.KeyName = baseField.KeyName;
                    currentValue.Value = inputElement.value;
                    currentValues.push(currentValue);

                    metadataModel.DateFields.push(baseField);
                    break;
                case "CheckBox":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "form-control");
                    baseField = new BaseFieldViewModel();
                    baseField.KeyName = inputElement.attributes[uscDynamicMetadataRest.KEY_NAME].value;
                    baseField.Position = index;
                    if (inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE)) {
                        baseField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE);
                    }
                    baseField.Label = labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL);
                    baseField.Required = (inputElement.getAttribute(uscDynamicMetadataRest.REQUIRED_FIELD) == "true");
                    if (baseField.Required && !inputElement.checked) {
                        missingRequiredFields.push(baseField.Label);
                    }

                    currentValue = new MetadataValueViewModel();
                    currentValue.KeyName = baseField.KeyName;
                    currentValue.Value = inputElement.checked.toString();
                    currentValues.push(currentValue);

                    metadataModel.BoolFields.push(baseField);
                    break;
                case "Enum":
                    let ddlElement: HTMLSelectElement = this.findSelectControl(divElement.id, 0)
                    enumField = new EnumFieldViewModel();
                    enumField.KeyName = ddlElement.attributes[uscDynamicMetadataRest.KEY_NAME].value;
                    enumField.Position = index;
                    if (inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE)) {
                        enumField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE);
                    }
                    enumField.Required = (inputElement.getAttribute(uscDynamicMetadataRest.REQUIRED_FIELD) == "true");
                    enumField.Label = labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL);
                    enumField.Options = {};
                    $.each(ddlElement, (index: number, option: HTMLOptionElement) => {
                        if (option.textContent) {
                            enumField.Options[index + 1] = option.textContent;
                        }
                    });

                    if (enumField.Required && !$("#".concat(ddlElement.id, " :selected")).text()) {
                        missingRequiredFields.push(enumField.Label);
                    }

                    currentValue = new MetadataValueViewModel();
                    currentValue.KeyName = enumField.KeyName;
                    currentValue.Value = $("#".concat(ddlElement.id, " :selected")).text();
                    currentValues.push(currentValue);

                    metadataModel.EnumFields.push(enumField);
                    break;
                case "Comment":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "riTextBox");
                    currentDiscussion = this._discussions.filter(x => x.Label == labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL))[0];
                    if (!currentDiscussion) {
                        currentDiscussion = new DiscussionFieldViewModel();
                        currentDiscussion.Label = labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL);
                    }

                    if (inputElement.value) {
                        let comment = new CommentFieldViewModel();
                        comment.Comment = inputElement.value;
                        comment.Author = this.currentUser;
                        comment.RegistrationDate = moment().toDate();
                        if (!currentDiscussion.Comments) {
                            currentDiscussion.Comments = new Array<CommentFieldViewModel>();
                        }
                        currentDiscussion.Comments.push(comment);
                    }
                    currentDiscussion.KeyName = inputElement.attributes[uscDynamicMetadataRest.KEY_NAME].value;
                    currentDiscussion.Position = index;
                    if (inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE)) {
                        currentDiscussion.DefaultValue = inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE);
                    }
                    currentDiscussion.Required = (inputElement.getAttribute(uscDynamicMetadataRest.REQUIRED_FIELD) == "true");
                    if (currentDiscussion.Required && !inputElement.value) {
                        missingRequiredFields.push(currentDiscussion.Label);
                    }

                    currentValue = new MetadataValueViewModel();
                    currentValue.KeyName = currentDiscussion.KeyName;
                    currentValue.Value = inputElement.value;
                    currentValues.push(currentValue);

                    metadataModel.DiscussionFields.push(currentDiscussion);
                    break;
                default:
                    break;
            }
        });


        if (missingRequiredFields.length > 0 && this.validationEnabled) {
            let message: string = "";
            $.each(missingRequiredFields, (index: number, missingField) => {
                message = message.concat(missingField, " \n ");
            });
            alert("Attenzione, i nomi dei seguenti campi sono obbligatori: \n ".concat(message));
            return undefined;
        }

        if (invalidInput.length > 0) {
            let message: string = "";
            $.each(invalidInput, (index: number, invalidField) => {
                message = message.concat(invalidField, " \n ");
            });
            alert("Attenzione, i seguenti campi sono in formato non valido: \n ".concat(message));
            return undefined;
        }

        return [JSON.stringify(metadataModel),JSON.stringify(currentValues)];
    }
}

export = uscDynamicMetadataRest;