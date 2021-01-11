import MetadataRepositoryBase = require("Tblt/MetadataRepositoryBase");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MetadataRepositoryModel = require("App/Models/Commons/MetadataRepositoryModel");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import UscErrorNotification = require('UserControl/uscErrorNotification');
import MetadataDesignerViewModel = require("App/ViewModels/Metadata/MetadataDesignerViewModel");
import BaseFieldViewModel = require("App/ViewModels/Metadata/BaseFieldViewModel");
import TextFieldViewModel = require("App/ViewModels/Metadata/TextFieldViewModel");
import DiscussionFieldViewModel = require("App/ViewModels/Metadata/DiscussionFieldViewModel");
import EnumFieldViewModel = require("App/ViewModels/Metadata/EnumFieldViewModel");
import MetadataFinderViewModel = require("App/ViewModels/Metadata/MetadataFinderViewModel");
import MetadataFinderType = require("App/Models/Commons/MetadataFinderType");

class uscAdvancedSearchDynamicMetadataRest extends MetadataRepositoryBase {
    public uscErrorNotificationId: string;
    public pageContentId: string;
    public componentTextId: string;
    public componentDateId: string;
    public componentNumberId: string;
    public componentCheckBoxId: string;
    public componentCommentId: string;
    public componentEnumId: string;

    private _uscErrorNotification: UscErrorNotification;
    private _currentMetadataRepositoryModel: MetadataRepositoryModel;
    private _metadataElementTypeInitializationAction: any;
    private _elementTypeFinderModelFactoryAction: any;

    private static KEY_NAME: string = "KeyName";
    private static FIELD_LABEL: string = "FieldLabel";
    private static TEXT_FIELD_CSSCLASS = "riTextBox";
    private static NUMBER_FIELD_CSSCLASS = "numberTextBox";
    private static DATE_FIELD_CSSCLASS = "dateTextBox";
    private static BOOL_FIELD_CSSCLASS = "form-control";
    private static DATATYPE_ATTR = "data-type";
    private static DISPLAY_NONE_CSSCLASS = "dsw-display-none";

    private static TEXT_ELEMENTTYPE = "Text";
    private static NUMBER_ELEMENTTYPE = "Number";
    private static DATE_ELEMENTTYPE = "Date";
    private static CHECKBOX_ELEMENTTYPE = "CheckBox";
    private static ENUM_ELEMENTTYPE = "Enum";
    private static COMMENT_ELEMENTTYPE = "Comment";

    private _controlsCounter: number = 0;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME));
    }

    public initialize(): void {
        super.initialize();

        this._bindLoaded();
        this._registerMetadataElementsInitializationActions();
        this._registerElementTypesFinderModelFactoryActions();
        this._registerPageControls();
    }

    public loadMetadataRepository(metadataRepositoryId: string): void {
        this._service.getFullModelById(metadataRepositoryId,
            (metadataRepositoryModel: MetadataRepositoryModel) => {
                if (metadataRepositoryModel) {
                    this._currentMetadataRepositoryModel = metadataRepositoryModel;
                    this._initializeMetadataControls();
                }
            }, this._errorHandler);
    }

    public getCurrentMetadataRepositoryModel(): MetadataRepositoryModel {
        return this._currentMetadataRepositoryModel;
    }

    public setPanelSearchType(isAdvancedSearch: boolean, metadataRepositoryId: string): void {
        if (isAdvancedSearch) {
            this.loadMetadataRepository(metadataRepositoryId);
        } else {
            this.clearPage();
        }
    }

    public getMetadataFinderModels = (): [MetadataFinderViewModel[], boolean] => {
        let searchPanelElements = $("#menuContent").children();
        let currentFinderModel: MetadataFinderViewModel;

        let finderModels: MetadataFinderViewModel[] = [] as MetadataFinderViewModel[];
        let metadataValuesErrorCounter: number = 0;

        $.each(searchPanelElements, (index: number, currentPanelElement: HTMLDivElement) => {
            let elementType: string = currentPanelElement.getAttribute(uscAdvancedSearchDynamicMetadataRest.DATATYPE_ATTR);

            currentFinderModel = this._elementTypeFinderModelFactoryAction[elementType](currentPanelElement);

            if (currentFinderModel.Value || currentFinderModel.ToValue) {
                finderModels.push(currentFinderModel);

                if (elementType === uscAdvancedSearchDynamicMetadataRest.DATE_ELEMENTTYPE || elementType === uscAdvancedSearchDynamicMetadataRest.NUMBER_ELEMENTTYPE) {
                    let validRangeValues: boolean = this._validateRangeMetadataValues(elementType, currentFinderModel.Value, currentFinderModel.ToValue);

                    if (!validRangeValues) {
                        metadataValuesErrorCounter++;
                    }
                }
            }
        });

        return [finderModels, metadataValuesErrorCounter === 0];
    }

    public clearAdvancedSearchPanelContent(): void {
        this.clearPage();
    }

    private _validateDateFormat(dateValue: string): boolean {
        let dateFormat = moment(dateValue, 'DD/MM/YYYY', 'it');
        return dateFormat.isValid();
    }

    private _compareDateRange(startDateValue: string, endDateValue: string) {
        let startDate: Date = new Date(moment(startDateValue, 'DD/MM/YYYY', 'it').format("L"));
        let endDate: Date = new Date(moment(endDateValue, 'DD/MM/YYYY', 'it').format("L"));

        return startDate < endDate;
    }

    private _validateMetadataRangeDateValues(startDateValue: string, endDateValue: string): boolean {
        if (startDateValue && endDateValue) {
            if (this._validateDateFormat(startDateValue) && this._validateDateFormat(endDateValue)) {
                return this._compareDateRange(startDateValue, endDateValue);
            }
            return false;
        } else if (startDateValue) {
            return this._validateDateFormat(startDateValue);
        } else if (endDateValue) {
            return this._validateDateFormat(endDateValue);
        }

        return true;
    }

    private _validateMetadataRangeNumberValues(startValue: string, endValue: string): boolean {

        if (startValue && endValue) {
            let startNumberValue: number = Number(startValue);
            let endNumberValue: number = Number(endValue);

            return startNumberValue < endNumberValue;
        }

        return true;
    }

    private _createFinderModel(metadataType: MetadataFinderType, keyName: string, value: string, toValue: string = null): MetadataFinderViewModel {
        let metadataFinderModel: MetadataFinderViewModel = new MetadataFinderViewModel();
        metadataFinderModel.MetadataType = MetadataFinderType[metadataType];
        metadataFinderModel.KeyName = keyName;
        metadataFinderModel.Value = value;
        metadataFinderModel.ToValue = toValue;

        return metadataFinderModel;
    }

    private _validateRangeMetadataValues(elementType: string, startValue: string, endValue: string): boolean {
        let rangeValuesAreValid: boolean = elementType === uscAdvancedSearchDynamicMetadataRest.NUMBER_ELEMENTTYPE
            ? this._validateMetadataRangeNumberValues(startValue, endValue)
            : this._validateMetadataRangeDateValues(startValue, endValue);

        return rangeValuesAreValid;
    }

    private _initializeMetadataControls(): void {
        let metadataDesignerModel: MetadataDesignerViewModel = JSON.parse(this._currentMetadataRepositoryModel.JsonMetadata);
        if (!metadataDesignerModel) {
            return;
        }

        this.clearPage();
        this._initializeSearchPanelElements(metadataDesignerModel);
        this._bindLoaded();
    }

    private _initializeSearchPanelElements = (metadataDesignerModel: MetadataDesignerViewModel): void => {
        this._controlsCounter = 0;
        let totalNumOfElements: number = this._getTotalNumberOfMetadataElements(metadataDesignerModel);

        for (let currentPosition = 0; currentPosition <= totalNumOfElements; currentPosition++) {
            let [currentMetadataElement, currentElementType]: [any, string] = this._getElementAtCurrentPosition(currentPosition, metadataDesignerModel);

            if (currentMetadataElement) {
                this._metadataElementTypeInitializationAction[currentElementType](this._controlsCounter, currentMetadataElement);
                this._controlsCounter++;
            }
        }
    }

    private _initializeHTMLGenericElement(idSelector: string, incrementalId: number, label: string, keyName: string, cssClass: string): void {
        let cloneElementId: string = this._createHTMLElementClone(idSelector, incrementalId);
        this._prepareLabel(cloneElementId, 0, label);

        let clonedInputElement: HTMLInputElement = this.findGenericInputControl(cloneElementId, 0, cssClass);
        clonedInputElement.value = "";
        clonedInputElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.KEY_NAME, keyName);
    }

    private _initializeRangeHTMLElement(idSelector: string, incrementalId: number, label: string, keyName: string, cssClass: string): void {
        let cloneElementId: string = this._createHTMLElementClone(idSelector, incrementalId);
        this._prepareLabel(cloneElementId, 0, label);

        let rangeStartElement: HTMLInputElement = this.findGenericInputControl(cloneElementId, 0, cssClass);
        let rangeEndElement: HTMLInputElement = this.findGenericInputControl(cloneElementId, 1, cssClass);

        rangeStartElement.value = "";
        rangeEndElement.value = "";

        rangeStartElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.KEY_NAME, keyName);
        rangeEndElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.KEY_NAME, keyName);

        $(`#${cloneElementId} input.${cssClass}`).on("blur", { idParent: cloneElementId, class: cssClass }, this._onBlurEvent);
    }

    private _onBlurEvent = (eventInfo): void => {
        let parentElementId: string = eventInfo.data.idParent;
        let cssClass: string = eventInfo.data.class;
        let startInputElement: HTMLInputElement = <HTMLInputElement>($("#".concat(parentElementId, " :input.", cssClass))[0]);
        let endInputElement: HTMLInputElement = <HTMLInputElement>($("#".concat(parentElementId, " :input.", cssClass))[1]);

        let labelValidator: HTMLLabelElement = <HTMLLabelElement>$("#" + parentElementId).find('label')[3];

        let validRangeValues: boolean = cssClass === uscAdvancedSearchDynamicMetadataRest.DATE_FIELD_CSSCLASS
            ? this._validateMetadataRangeDateValues(startInputElement.value, endInputElement.value)
            : this._validateMetadataRangeNumberValues(startInputElement.value, endInputElement.value);

        labelValidator.style.visibility = validRangeValues ? "hidden" : "visible";
    } 

    private _initializeEnumHTMLElement(idSelector: string, incrementalId: number, label: string, keyName: string, options: { [id: number]: string }): void {
        let cloneElementId: string = this._createHTMLElementClone(idSelector, incrementalId);
        this._prepareLabel(cloneElementId, 0, label);
        let ddlElement: HTMLSelectElement = this.findSelectControl(cloneElementId, 0);
        ddlElement.setAttribute("id", `id${incrementalId}`);
        ddlElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.KEY_NAME, keyName);
        ddlElement.value = "";

        $("#" + ddlElement.id).append($('<option>', {
            value: 0,
            text: ""
        }));

        $.each(options, (innerIndex: number, value: string) => {
            $("#" + ddlElement.id).append($('<option>', {
                value: innerIndex + 1,
                text: value
            }));
        });
    }

    private _getElementAtCurrentPosition(currentPosition: number, metadataDesignerModel: MetadataDesignerViewModel): [any, string] {
        for (let currentElementType in metadataDesignerModel) {
            let currentMetadataElement: any  = undefined;
            if (typeof metadataDesignerModel[currentElementType] !== "boolean") {
                currentMetadataElement = metadataDesignerModel[currentElementType].filter(x => x.Position === currentPosition)[0];
            }
            if (currentMetadataElement) {
                return [currentMetadataElement, currentElementType];
            }
        }

        return [null, ""];
    }

    private _getTotalNumberOfMetadataElements(metadataDesignerModel: MetadataDesignerViewModel): number {
        let totalNumOfElements: number = 0;
        for (let currentKey in metadataDesignerModel) {
            if (typeof (metadataDesignerModel[currentKey]) !== "boolean") {
                totalNumOfElements += metadataDesignerModel[currentKey].length;
            }
        }

        return totalNumOfElements;
    }

    private _createHTMLElementClone(idSelector: string, incrementalId: number): string {
        let cloneElementId: string = this.cloneElement(idSelector, incrementalId);
        let clonedElement: HTMLDivElement = <HTMLDivElement>($("#".concat(cloneElementId))[0]);

        let elementClassList: DOMTokenList = clonedElement.classList;

        if (elementClassList) {
            clonedElement.classList.remove(uscAdvancedSearchDynamicMetadataRest.DISPLAY_NONE_CSSCLASS);
        } else {
            // IE9 compatibility because classList is not supported
            let currentCssClasses: string[] = clonedElement.className.split(" ").filter(cls => cls !== uscAdvancedSearchDynamicMetadataRest.DISPLAY_NONE_CSSCLASS);
            clonedElement.className = currentCssClasses.join(" ");
        }

        return cloneElementId;
    }

    private _prepareLabel(idParent: string, position: number, labelCaption: string): void {
        let labelElement: HTMLLabelElement = this.findLabelElement(idParent, position);
        labelElement.textContent = labelCaption.concat(": ");
        labelElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.FIELD_LABEL, labelCaption);
    }

    private _errorHandler(exception: ExceptionDTO): void {
        if (exception && this._uscErrorNotification && exception instanceof ExceptionDTO) {
            if (!jQuery.isEmptyObject(this._uscErrorNotification)) {
                this._uscErrorNotification.showNotification(exception);
            }
        }
    }

    private _registerMetadataElementsInitializationActions(): void {
        this._metadataElementTypeInitializationAction = {};

        this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_TEXT_FIELD] = (incrementalId: number, model: TextFieldViewModel) => {
            this._initializeHTMLGenericElement(this.componentTextId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
        };

        this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_BOOL_FIELD] = (incrementalId: number, model: BaseFieldViewModel) => {
            this._initializeHTMLGenericElement(this.componentCheckBoxId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.BOOL_FIELD_CSSCLASS);
        };

        this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_DISCUSION_FIELD] = (incrementalId: number, model: DiscussionFieldViewModel) => {
            this._initializeHTMLGenericElement(this.componentCommentId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
        };

        this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_DATE_FIELD] = (incrementalId: number, model: BaseFieldViewModel) => {
            this._initializeRangeHTMLElement(this.componentDateId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.DATE_FIELD_CSSCLASS);
        };

        this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_NUMBER_FIELD] = (incrementalId: number, model: BaseFieldViewModel) => {
            this._initializeRangeHTMLElement(this.componentNumberId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.NUMBER_FIELD_CSSCLASS);
        };

        this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_ENUM_FIELD] = (incrementalId: number, model: EnumFieldViewModel) => {
            this._initializeEnumHTMLElement(this.componentEnumId, incrementalId, model.Label, model.KeyName, model.Options);
        };
    }

    private _registerElementTypesFinderModelFactoryActions(): void {
        this._elementTypeFinderModelFactoryAction = {};

        this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.TEXT_ELEMENTTYPE] = (currentPanelElement: HTMLDivElement): MetadataFinderViewModel => {
            let inputElement: HTMLInputElement = this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
            let fieldKeyName: string = inputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;

            return this._createFinderModel(MetadataFinderType.Text, fieldKeyName, inputElement.value);
        };
        this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.NUMBER_ELEMENTTYPE] = (currentPanelElement: HTMLDivElement): MetadataFinderViewModel => {
            let startNumberInputElement: HTMLInputElement = this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
            let endNumberInputElement: HTMLInputElement = this.findGenericInputControl(currentPanelElement.id, 1, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
            let fieldKeyName: string = startNumberInputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;

            return this._createFinderModel(MetadataFinderType.Number, fieldKeyName, startNumberInputElement.value, endNumberInputElement.value);
        };
        this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.DATE_ELEMENTTYPE] = (currentPanelElement: HTMLDivElement): MetadataFinderViewModel => {
            let startDateInputElement: HTMLInputElement = this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
            let endDateInputElement: HTMLInputElement = this.findGenericInputControl(currentPanelElement.id, 1, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
            let fieldKeyName: string = startDateInputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;

            let startDateValue: string = startDateInputElement.value ? moment(startDateInputElement.value, 'DD/MM/YYYY', 'it').format("L") : null;
            let endDateValue: string = endDateInputElement.value ? moment(endDateInputElement.value, 'DD/MM/YYYY', 'it').format("L") : null;

            return this._createFinderModel(MetadataFinderType.Date, fieldKeyName, startDateValue, endDateValue);
        };
        this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.CHECKBOX_ELEMENTTYPE] = (currentPanelElement: HTMLDivElement): MetadataFinderViewModel => {
            let inputElement: HTMLInputElement = this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.BOOL_FIELD_CSSCLASS);
            let fieldKeyName: string = inputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;

            return this._createFinderModel(MetadataFinderType.Bool, fieldKeyName, inputElement.value);
        };
        this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.ENUM_ELEMENTTYPE] = (currentPanelElement: HTMLDivElement): MetadataFinderViewModel => {
            let ddlElement: HTMLSelectElement = this.findSelectControl(currentPanelElement.id, 0);
            let fieldKeyName: string = ddlElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;

            let ddlSelectedValue: string = $("#".concat(ddlElement.id, " :selected")).text();
            return this._createFinderModel(MetadataFinderType.Enum, fieldKeyName, ddlSelectedValue);
        };
        this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.COMMENT_ELEMENTTYPE] = (currentPanelElement: HTMLDivElement): MetadataFinderViewModel => {
            let inputElement: HTMLInputElement = this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
            let fieldKeyName: string = inputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;

            return this._createFinderModel(MetadataFinderType.Discussion, fieldKeyName, inputElement.value);
        };
    }

    private _registerPageControls(): void {
        this._uscErrorNotification = <UscErrorNotification>$("#".concat(this.uscErrorNotificationId)).data();
    }

    private _bindLoaded(): void {
        $("#".concat(this.pageContentId)).data(this);
    }
}

export = uscAdvancedSearchDynamicMetadataRest;