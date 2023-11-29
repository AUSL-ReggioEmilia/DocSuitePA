import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import DiscussionFieldViewModel = require('App/ViewModels/Metadata/DiscussionFieldViewModel');
import CommentFieldViewModel = require('App/ViewModels/Metadata/CommentFieldViewModel');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import MetadataValueViewModel = require('App/ViewModels/Metadata/MetadataValueViewModel');

class uscDynamicMetadataSummaryRest extends MetadataRepositoryBase {

    pageContentId: string;
    componentTextId: string;
    componentDateId: string;
    componentNumberId: string;
    componentCheckBoxId: string;
    componentCommentId: string;
    componentEnumId: string;
    managerId: string;
    uscNotificationId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _domainUserService: DomainUserService;

    private controlsCounter = 0;

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME));
        this._serviceConfigurations = serviceConfigurations;
    }

    /*
     * ---------------------------- Events ---------------------------------
     */

    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();

        let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
        this._domainUserService = new DomainUserService(domainUserConfiguration);

        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);
        this.bindLoaded();
    }


    /*
     * --------------------------- Methods ------------------------------
     */

    /**
     * Scateno l'evento di Load Completed del controllo
     */
    private bindLoaded(): void {
        $("#".concat(this.pageContentId)).data(this);
    }

    /**
     * Carico i metadati nello usc
     * @param jsonMetadata
     */
    loadMetadatas(jsonDesignerMetadata: string, jsonValueMetadataModels: string) {
        let metadataModel: MetadataDesignerViewModel = JSON.parse(jsonDesignerMetadata);
        let metadataValues: MetadataValueViewModel[] = [];
        if (jsonValueMetadataModels) {
            metadataValues = JSON.parse(jsonValueMetadataModels.replace(/\r?\n|\r/g, ""));
        }

        this.clearPage();

        this.arrangeControlsInPosition(metadataModel, metadataValues);

        this.bindLoaded();
    }

    private arrangeControlsInPosition(metadataDesignerViewModel: MetadataDesignerViewModel, metadataValues: MetadataValueViewModel[]) {
        let aggregatedSum: number = 0;
        for (let arr in metadataDesignerViewModel) {
            if (typeof (metadataDesignerViewModel[arr]) !== "boolean") {
                aggregatedSum += metadataDesignerViewModel[arr].length;
            }
        }
        for (var i = 0; i <= aggregatedSum; i++) {
            let metadataField: any = null;
            let currentType;
            for (let arr in metadataDesignerViewModel) {

                currentType = arr;
                let obj = undefined;
                if (typeof (metadataDesignerViewModel[arr]) !== "boolean") {
                    obj = metadataDesignerViewModel[arr].filter(x => x.Position == i)[0];
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
            if (metadataValues) {
                let currentMetadataValue: MetadataValueViewModel = metadataValues.filter(x => x.KeyName == metadataField.KeyName)[0];
                if (currentMetadataValue) {
                    currentValue = currentMetadataValue.Value;
                }
            }

            switch (currentType) {
                case MetadataRepositoryBase.CONTROL_TEXT_FIELD:
                    this.fillHTMLGenericElement(this.componentTextId, this.controlsCounter, metadataField, currentValue);
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_DATE_FIELD:
                    if (currentValue) {
                        currentValue = moment(currentValue, "YYYY-MM-DD").format("DD/MM/YYYY");
                    }
                    this.fillHTMLGenericElement(this.componentDateId, this.controlsCounter, metadataField, currentValue);
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_NUMBER_FIELD:
                    this.fillHTMLGenericElement(this.componentNumberId, this.controlsCounter, metadataField, currentValue);
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_BOOL_FIELD:
                    this.fillHTMLCheckBox(this.componentCheckBoxId, this.controlsCounter, metadataField, currentValue);
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_ENUM_FIELD:
                    this.fillHTMLGenericElement(this.componentEnumId, this.controlsCounter, metadataField, currentValue);
                    this.controlsCounter++;
                    break;
                case MetadataRepositoryBase.CONTROL_DISCUSION_FIELD:
                    this.fillHTMLComment(this.componentCommentId, this.controlsCounter, metadataField);
                    this.controlsCounter++;
                    break;
                default:
                    break;
            }
        }
    }
    /**
     * Popolo con un campo base le componenti della pagina
     * @param idComponent
     * @param incrementalInteger
     * @param model
     */
    fillHTMLGenericElement(idComponent: string, incrementalInteger: number, model: BaseFieldViewModel, currentValue: string) {
        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let labelField: HTMLLabelElement = this.findLabelElement(idCloned, 0);
        let valueField: HTMLLabelElement = this.findLabelElement(idCloned, 1);
        labelField.textContent = model.Label.concat(": ");
        valueField.textContent = ""
        if (currentValue) {
            valueField.textContent = currentValue;
        }
        if (model.HiddenField) {
            labelField.hidden = true;
            valueField.hidden = true;
        }
    }

    fillHTMLCheckBox(idComponent: string, incrementalInteger: number, model: BaseFieldViewModel, currentValue: string) {
        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let labelField: HTMLLabelElement = this.findLabelElement(idCloned, 0);
        let inputCheckBox: HTMLInputElement = this.findStandardInputElement(idCloned, 0);
        inputCheckBox.checked = (currentValue.toLowerCase() == "true");
        labelField.textContent = model.Label.concat(": ");
        if (model.HiddenField) {
            labelField.hidden = true;
            inputCheckBox.parentElement.hidden = true;
        }
    }

    fillHTMLComment(idComponent: string, incrementalInteger: number, model: DiscussionFieldViewModel) {
        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let labelField: HTMLLabelElement = this.findLabelElement(idCloned, 0);
        let valueField: HTMLLabelElement = this.findLabelElement(idCloned, 1);
        let imgButton: HTMLInputElement = this.findGenericInputControl(idCloned, 0, "imgButton");
        $("#".concat(idCloned, " :input.", "imgButton")).on("click", { label: model.Label, managerId: this.managerId }, this.openCommentsWindow);
        if (model.Comments && model.Comments.length > 0) {
            let latestComment: CommentFieldViewModel = model.Comments.pop();
            if (latestComment.Author) {
                this._domainUserService.getUser(latestComment.Author,
                    (user: DomainUserModel) => {
                        if (user) {
                            valueField.innerHTML = "<i>".concat(user.DisplayName, " - ", moment(latestComment.RegistrationDate.toString(), "YYY-MM-DDTHH:mm:ssZ").format("DD/MM/YYYY"), "</i> <br>", latestComment.Comment);
                        }
                    },
                    (exception: ExceptionDTO) => {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (exception && uscNotification && exception instanceof ExceptionDTO) {
                            if (!jQuery.isEmptyObject(uscNotification)) {
                                uscNotification.showNotification(exception);
                            }
                        }
                    }
                );
            }
        }
        labelField.textContent = model.Label.concat(": ");

        if (model.HiddenField) {
            labelField.hidden = true;
            imgButton.parentElement.hidden = true;
        }
    }

    openCommentsWindow(event) {
        let label: string = event.data.label
        let managerId: string = event.data.managerId
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(managerId);
        let wnd: Telerik.Web.UI.RadWindow = this._manager.open("../Comm/ViewMetadataComments.aspx?Type=Fasc&Label=".concat(label), "managerViewComments", null);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }
}

export = uscDynamicMetadataSummaryRest;