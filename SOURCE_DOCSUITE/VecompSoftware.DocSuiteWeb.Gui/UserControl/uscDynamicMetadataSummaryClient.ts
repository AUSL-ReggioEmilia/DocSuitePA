import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataViewModel = require('App/ViewModels/Metadata/MetadataViewModel');
import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import DiscussionFieldViewModel = require('App/ViewModels/Metadata/DiscussionFieldViewModel');
import CommentFieldViewModel = require('App/ViewModels/Metadata/CommentFieldViewModel');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class uscDynamicMetadataSummaryClient extends MetadataRepositoryBase {

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
    loadMetadatas(jsonMetadata: string) {
        let metadataModel: MetadataViewModel = JSON.parse(jsonMetadata);
        this.clearPage();

        let idCloned: string;
        let content: HTMLElement = document.getElementById("menuContent");

        $.each(metadataModel.TextFields, (index: number, textField) => {
            this.fillHTMLGenericElement(this.componentTextId, this.controlsCounter, textField);
            this.controlsCounter++;
        });

        $.each(metadataModel.DateFields, (index: number, textField) => {
            this.fillHTMLGenericElement(this.componentDateId, this.controlsCounter, textField);
            this.controlsCounter++;
        });

        $.each(metadataModel.EnumFields, (index: number, textField) => {
            this.fillHTMLGenericElement(this.componentEnumId, this.controlsCounter, textField);
            this.controlsCounter++;
        });

        $.each(metadataModel.NumberFields, (index: number, textField) => {
            this.fillHTMLGenericElement(this.componentNumberId, this.controlsCounter, textField);
            this.controlsCounter++;
        });

        $.each(metadataModel.DiscussionFields, (index: number, textField) => {
            this.fillHTMLComment(this.componentCommentId, this.controlsCounter, textField);
            this.controlsCounter++;
        });

        $.each(metadataModel.BoolFields, (index: number, textField) => {
            this.fillHTMLCheckBox(this.componentCheckBoxId, this.controlsCounter, textField);
            this.controlsCounter++;
        });

        this.bindLoaded();
    }

    /**
     * Popolo con un campo base le componenti della pagina
     * @param idComponent
     * @param incrementalInteger
     * @param model
     */
    fillHTMLGenericElement(idComponent: string, incrementalInteger: number, model: BaseFieldViewModel) {
        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let labelField: HTMLLabelElement = this.findLabelElement(idCloned, 0);
        let valueField: HTMLLabelElement = this.findLabelElement(idCloned, 1);
        labelField.textContent = model.Label.concat(": ");
        valueField.textContent = ""
        if (model.Value) {
            valueField.textContent = model.Value;
        }
    }

    fillHTMLCheckBox(idComponent: string, incrementalInteger: number, model: BaseFieldViewModel) {
        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let labelField: HTMLLabelElement = this.findLabelElement(idCloned, 0);
        let inputCheckBox: HTMLInputElement = this.findStandardInputElement(idCloned, 0);
        inputCheckBox.checked = (model.Value.toLowerCase() == "true");
        labelField.textContent = model.Label.concat(": ");
    }

    fillHTMLComment(idComponent: string, incrementalInteger: number, model: DiscussionFieldViewModel) {
        let idCloned: string = this.cloneElement(idComponent, incrementalInteger);
        let labelField: HTMLLabelElement = this.findLabelElement(idCloned, 0);
        let valueField: HTMLLabelElement = this.findLabelElement(idCloned, 1);
        let imgButton: HTMLInputElement = this.findGenericInputControl(idCloned, 0, "imgButton");
        $("#".concat(idCloned, " :input.", "imgButton")).on("click", { label: model.Label, managerId: this.managerId }, this.openCommentsWindow);
        if (model.Comments && model.Comments.length > 0){
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

export = uscDynamicMetadataSummaryClient;