import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import MetadataViewModel = require('App/ViewModels/Metadata/MetadataViewModel');
import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import EnumFieldViewModel = require('App/ViewModels/Metadata/EnumFieldViewModel');
import DiscussionFieldViewModel = require('App/ViewModels/Metadata/DiscussionFieldViewModel');
import TextFieldViewModel = require('App/ViewModels/Metadata/TextFieldViewModel');
import CommentFieldViewModel = require('App/ViewModels/Metadata/CommentFieldViewModel');


class uscDynamicMetadataClient extends MetadataRepositoryBase {

    pageContentId: string;
    uscNotificationId: string;
    componentTextId: string;
    componentDateId: string;
    componentNumberId: string;
    componentCheckBoxId: string;
    componentCommentId: string;
    componentEnumId: string;
    currentUser: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _currentMetadataRepositoryModel: MetadataRepositoryModel
    private _discussions: DiscussionFieldViewModel[];

    private controlsCounter = 0;

    static FIELD_LABEL: string = "FieldLabel";
    static REQUIRED_FIELD: string = "RequiredField";
    static DEFAULT_VALUE: string = "DefaultValue";

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

    /**
     * Carico il modello nello usercontrol
     * @param idMetadataRepository
     */
    loadMetadataRepository(idMetadataRepository: string) {
        this._service.getFullModelById(idMetadataRepository,
            (data: MetadataRepositoryModel) => {
                if (data) {
                    this._currentMetadataRepositoryModel = data;
                    this.loadPageItems(data.JsonMetadata);
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
     * Carico gli elementi della pagina
     * @param metadataRepositoryModel
     */
    loadPageItems(jsonMetadata: string) {
        let metadataViewModel: MetadataViewModel = JSON.parse(jsonMetadata);

        this.clearPage();

        $.each(metadataViewModel.TextFields, (index: number, textFieldViewModel) => {
            this.fillHTMLGenericElement(this.componentTextId, this.controlsCounter, textFieldViewModel, "riTextBox");
            this.controlsCounter++;
        });

        $.each(metadataViewModel.NumberFields, (index: number, numberFieldViewModel) => {
            this.fillHTMLGenericElement(this.componentNumberId, this.controlsCounter, numberFieldViewModel, "riTextBox");
            this.controlsCounter++;
        });

        $.each(metadataViewModel.BoolFields, (index: number, boolFieldViewModel) => {
            this.fillHTMLGenericElement(this.componentCheckBoxId, this.controlsCounter, boolFieldViewModel, "form-control");
            this.controlsCounter++;
        });

        $.each(metadataViewModel.DateFields, (index: number, dateFieldViewModel) => {
            this.fillDateElement(this.componentDateId, this.controlsCounter, dateFieldViewModel, "riTextBox")
            this.controlsCounter++
        });

        this._discussions = metadataViewModel.DiscussionFields;
        $.each(metadataViewModel.DiscussionFields, (index: number, discussionFieldViewModel) => {
            this.fillHTMLGenericElement(this.componentCommentId, this.controlsCounter, discussionFieldViewModel, "riTextBox")
            this.controlsCounter++
        });

        $.each(metadataViewModel.EnumFields, (index: number, enumFieldViewModel) => {
            this.fillHTMLEnumElement(this.componentEnumId, this.controlsCounter, enumFieldViewModel, "ddlOptions");
            this.controlsCounter++;
        });

        this.bindLoaded();
    }


    /**
     * Clono un elemento generico e lo completo con i vari attributi
     * @param idComponent
     * @param incrementalIdentifier
     * @param model
     * @param cssClass
     */
    fillHTMLGenericElement(idComponent: string, incrementalIdentifier: number, model: BaseFieldViewModel, cssClass: string) {

        let idCloned: string = this.cloneElement(idComponent, incrementalIdentifier);
        let clonedElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idCloned))[0]);
        clonedElement.style.display = "";
        this.prepareLabel(idCloned, 0, model.Label);
        let inputElement: HTMLInputElement = this.findGenericInputControl(idCloned, 0, cssClass)

        this.setDefaultValues(inputElement, model);
        this.setRequiredAttribute(inputElement, model);

        if (model.DefaultValue) {
            inputElement.value = model.DefaultValue;
            inputElement.checked = (model.DefaultValue.toLowerCase() == "true");
        }

        if (model.Value) {
            inputElement.value = model.Value;
            inputElement.checked = (model.Value.toLowerCase() == "true");
        }
    };

    /**
     * Clono l'elemento ENUM e lo completo con i vari attributi
     * @param idComponent
     * @param incrementalIdentifier
     * @param model
     * @param cssClass
     */
    fillHTMLEnumElement(idComponent: string, incrementalIdentifier: number, model: EnumFieldViewModel, cssClass: string) {

        let idCloned: string = this.cloneElement(idComponent, incrementalIdentifier);
        let clonedElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idCloned))[0]);
        clonedElement.style.display = "";
        this.prepareLabel(idCloned, 0, model.Label);
        let ddlElement: HTMLSelectElement = this.findSelectControl(idCloned, 0)
        ddlElement.setAttribute("id", "id".concat(incrementalIdentifier.toString()));
        incrementalIdentifier++;

        ddlElement.setAttribute(uscDynamicMetadataClient.REQUIRED_FIELD, "false");
        if (model.Required) {
            ddlElement.required = true;
            ddlElement.setAttribute(uscDynamicMetadataClient.REQUIRED_FIELD, "true");
        }

        $("#" + ddlElement.id).append($('<option>', {
            value: 0,
            text: ""
        }));

        if (model.DefaultValue && !model.Value) {
            ddlElement.setAttribute(uscDynamicMetadataClient.DEFAULT_VALUE, model.DefaultValue);
            if (!isNaN(parseInt(model.DefaultValue))) {
                model.Value = model.DefaultValue;
            }
        }

        $.each(model.Options, (innerIndex: number, value) => {
            let isSelected: boolean = (value == model.Value)
            $("#" + ddlElement.id).append($('<option>', {
                value: innerIndex + 1,
                text: value,
                selected: isSelected
            }));
        });
    };

    /**
     * Popolo il campo data
     * @param idComponent
     * @param incrementalIdentifier
     * @param model
     * @param cssClass
     */
    fillDateElement(idComponent: string, incrementalIdentifier: number, model: BaseFieldViewModel, cssClass: string) {
        let idCloned: string = this.cloneElement(idComponent, incrementalIdentifier);
        let clonedElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idCloned))[0]);
        clonedElement.style.display = "";
        let inputElement: HTMLInputElement = this.findGenericInputControl(idCloned, 0, cssClass)
        let labelValidator: HTMLLabelElement = this.findLabelElement(idCloned, 1);
        this.prepareLabel(idCloned, 0, model.Label);
        this.setDefaultValues(inputElement, model);
        this.setRequiredAttribute(inputElement, model);
        $("#".concat(idCloned, " :input.", cssClass)).on("blur", { idParent: idCloned, class: cssClass }, this.onBlurEvent);

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
        if (moment(inputElement.value, 'DD/MM/YYYY', 'it').isValid()) {
            labelValidator.style.visibility = "hidden"
        }
        else {
            labelValidator.style.visibility = "visible"
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
        labelElement.setAttribute(uscDynamicMetadataClient.FIELD_LABEL, labelCaption);
    }


    /**
     * Imposto alcuni default comuni dei campi di input
     * @param element
     * @param model
     */
    setDefaultValues(element: HTMLInputElement, model: BaseFieldViewModel) {
        if (model.DefaultValue) {
            element.setAttribute(uscDynamicMetadataClient.DEFAULT_VALUE, model.DefaultValue);
        }

        if (model.DefaultValue) {
            element.value = model.DefaultValue;
            element.checked = (model.DefaultValue.toLowerCase() == "true");
        }

        if (model.Value) {
            element.value = model.Value;
            element.checked = (model.Value.toLowerCase() == "true");
        }
    }

    /**
     * Imposto l'attributo required
     * @param element
     * @param model
     */
    setRequiredAttribute(element: HTMLInputElement, model: BaseFieldViewModel) {
        element.setAttribute(uscDynamicMetadataClient.REQUIRED_FIELD, "false");
        if (model.Required) {
            element.required = true;
            element.setAttribute(uscDynamicMetadataClient.REQUIRED_FIELD, "true");
        }
    }

    /**
     * Recupero i dati dalla pagina
     */
    bindModelFormPage(): string {

        let content = $("#menuContent").children();
        let item = new MetadataRepositoryModel();
        let metadataModel = new MetadataViewModel();
        let baseField: BaseFieldViewModel;
        let enumField: EnumFieldViewModel;
        let textField: TextFieldViewModel;
        let currentDiscussion: DiscussionFieldViewModel

        let inputElement: HTMLInputElement;
        let requiredElement: HTMLInputElement;
        let labelElement: HTMLLabelElement;

        let missingRequiredFields: string[] = [];
        let invalidInput: string[] = [];

        if (content.length < 1) {
            return null;
        }

        $.each(content, (index: number, divElement: HTMLDivElement) => {

            labelElement = this.findLabelElement(divElement.id, 0);
            let dataset: string = divElement.getAttribute("data-type");

            switch (dataset) {
                case "Text":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "riTextBox");
                    textField = new TextFieldViewModel();
                    textField.Value = inputElement.value;
                    if (inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE)) {
                        textField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE);
                    }
                    textField.Label = labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL);
                    textField.Required = (inputElement.getAttribute(uscDynamicMetadataClient.REQUIRED_FIELD) == "true");
                    if (textField.Required && !textField.Value) {
                        missingRequiredFields.push(textField.Label);
                    }
                    metadataModel.TextFields.push(textField);
                    break;
                case "Number":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "riTextBox");
                    baseField = new BaseFieldViewModel();
                    baseField.Value = inputElement.value;
                    if (inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE)) {
                        baseField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE);
                    }
                    baseField.Label = labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL);
                    baseField.Required = (inputElement.getAttribute(uscDynamicMetadataClient.REQUIRED_FIELD) == "true");
                    if (baseField.Required && !baseField.Value) {
                        missingRequiredFields.push(baseField.Label);
                    }
                    if (baseField.Value && isNaN(parseInt(baseField.Value))) {
                        invalidInput.push(baseField.Label);
                    }
                    metadataModel.NumberFields.push(baseField);
                    break;
                case "Date":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "riTextBox");
                    baseField = new BaseFieldViewModel();
                    baseField.Value = moment(inputElement.value, 'DD/MM/YYYY', 'it').format("L");
                    if (inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE)) {
                        baseField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE);
                    }
                    baseField.Label = labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL);
                    baseField.Required = (inputElement.getAttribute(uscDynamicMetadataClient.REQUIRED_FIELD) == "true");
                    if (baseField.Required && !baseField.Value) {
                        missingRequiredFields.push(baseField.Label);
                    }
                    if (baseField.Value && !moment(baseField.Value).isValid()) {
                        invalidInput.push(baseField.Label);
                    }
                    metadataModel.DateFields.push(baseField);
                    break;
                case "CheckBox":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "form-control");
                    baseField = new BaseFieldViewModel();
                    baseField.Value = inputElement.checked.toString();
                    if (inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE)) {
                        baseField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE);
                    }
                    baseField.Label = labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL);
                    baseField.Required = (inputElement.getAttribute(uscDynamicMetadataClient.REQUIRED_FIELD) == "true");
                    if (baseField.Required && baseField.Value == null) {
                        missingRequiredFields.push(baseField.Label);
                    }
                    metadataModel.BoolFields.push(baseField);
                    break;
                case "Enum":
                    let ddlElement: HTMLSelectElement = this.findSelectControl(divElement.id, 0)
                    enumField = new EnumFieldViewModel();
                    enumField.Value = $("#".concat(ddlElement.id, " :selected")).text();
                    if (inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE)) {
                        enumField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE);
                    }
                    enumField.Required = (inputElement.getAttribute(uscDynamicMetadataClient.REQUIRED_FIELD) == "true");
                    enumField.Label = labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL);
                    enumField.Options = {};
                    $.each(ddlElement, (index: number, option: HTMLOptionElement) => {
                        if (option.textContent) {
                            enumField.Options[index + 1] = option.textContent;
                        }
                    });

                    if (enumField.Required && enumField.Value == null) {
                        missingRequiredFields.push(enumField.Label);
                    }
                    metadataModel.EnumFields.push(enumField);
                    break;
                case "Comment":
                    inputElement = this.findGenericInputControl(divElement.id, 0, "riTextBox");
                    currentDiscussion = this._discussions.filter(x => x.Label == labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL))[0];
                    if (!currentDiscussion) {
                        currentDiscussion = new DiscussionFieldViewModel();
                        currentDiscussion.Label = labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL);
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

                    if (inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE)) {
                        currentDiscussion.DefaultValue = inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE);
                    }
                    currentDiscussion.Required = (inputElement.getAttribute(uscDynamicMetadataClient.REQUIRED_FIELD) == "true");
                    if (currentDiscussion.Required && !inputElement.value) {
                        missingRequiredFields.push(currentDiscussion.Label);
                    }
                    metadataModel.DiscussionFields.push(currentDiscussion);
                    break;
                default:
                    break;
            }
        });


        if (missingRequiredFields.length > 0) {
            let message: string = "";
            $.each(missingRequiredFields, (index: number, missingField) => {
                message = message.concat(missingField, " \n ");
            });
            alert("Attenzione, i nomi dei seguenti campi sono obbligatori: \n ".concat(message));
            return "";
        }

        if (invalidInput.length > 0) {
            let message: string = "";
            $.each(invalidInput, (index: number, invalidField) => {
                message = message.concat(invalidField, " \n ");
            });
            alert("Attenzione, i seguenti campi sono in formato non valido: \n ".concat(message));
            return "";
        }

        return JSON.stringify(metadataModel);
    }
}

export = uscDynamicMetadataClient;