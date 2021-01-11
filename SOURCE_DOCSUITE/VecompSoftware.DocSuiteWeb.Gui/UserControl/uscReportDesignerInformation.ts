/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import ReportInformationViewModel = require('App/ViewModels/Reports/ReportInformationViewModel');
import Environment = require('App/Models/Environment');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
import ReportBuilderPropertyModel = require('App/Models/Reports/ReportBuilderPropertyModel');
import ReportBuilderPropertyType = require('App/Models/Reports/ReportBuilderPropertyType');
import UscErrorNotification = require('UserControl/uscErrorNotification');

declare var Page_ClientValidate: any;
class uscReportDesignerInformation {
    uscNotificationId: string;
    pnlContentId: string;
    rdlEntityId: string;
    rowMetadataId: string;
    rowUDId: string;
    rdlUDTypeId: string;
    txtNameId: string;
    btnLoadId: string;
    uscMetadataSelId: string;
    lblCreatedById: string;
    lblCreatedDateId: string;
    lblStatusId: string;

    private _rdlEntity: Telerik.Web.UI.RadComboBox;
    private _rdlUDType: Telerik.Web.UI.RadComboBox;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _btnLoad: Telerik.Web.UI.RadButton;

    static ON_END_LOAD_EVENT = "onEndLoad";
    static ON_EXECUTE_LOAD_EVENT = "onExecuteLoad";

    constructor() {
        
    }

    /**
     *------------------------- Events -----------------------------
     */
    private RdlEntity_OnSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let selectedItem: Telerik.Web.UI.RadComboBoxItem = args.get_item();
        $('#'.concat(this.rowMetadataId)).hide();
        $('#'.concat(this.rowUDId)).hide();
        if (selectedItem.get_value() == Environment.Fascicle.toString()) {
            $('#'.concat(this.rowMetadataId)).show();
            $('#'.concat(this.rowUDId)).show();
        }
    }

    private BtnLoad_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (Page_ClientValidate("ReportData")) {
            let selectedEntity: Telerik.Web.UI.RadComboBoxItem = this._rdlEntity.get_selectedItem();
            if (!selectedEntity || !selectedEntity.get_value()) {
                alert("E' obbligatorio selezionare una tipologia.");
                return;
            }
            $("#".concat(this.pnlContentId)).trigger(uscReportDesignerInformation.ON_EXECUTE_LOAD_EVENT);
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */
    initialize() {
        this._rdlEntity = $find(this.rdlEntityId) as Telerik.Web.UI.RadComboBox;
        this._rdlEntity.add_selectedIndexChanged(this.RdlEntity_OnSelectedIndexChanged);
        this._rdlUDType = $find(this.rdlUDTypeId) as Telerik.Web.UI.RadComboBox;
        this._txtName = $find(this.txtNameId) as Telerik.Web.UI.RadTextBox;
        this._btnLoad = $find(this.btnLoadId) as Telerik.Web.UI.RadButton;
        this._btnLoad.add_clicked(this.BtnLoad_OnClick);

        $('#'.concat(this.rowMetadataId)).hide();
        $('#'.concat(this.rowUDId)).hide();

        this.completeLoad();
    }

    private completeLoad() {
        $("#".concat(this.pnlContentId)).data(this);
        $("#".concat(this.pnlContentId)).trigger(uscReportDesignerInformation.ON_END_LOAD_EVENT);
    }

    loadInformations(model: ReportInformationViewModel): void {
        try {
            if (!model) {
                console.warn("Nessun modello passato per il caricamento delle informazioni");
                return;
            }

            if (model.Name) {
                this._txtName.set_value(model.Name);
            }

            if (model.CreatedBy) {
                $("#".concat(this.lblCreatedById)).html(model.CreatedBy);
            }

            if (model.CreatedDate) {
                $("#".concat(this.lblCreatedDateId)).html(moment(model.CreatedDate).format("DD/MM/YYYY"));
            }

            if (model.StatusLabel) {
                $("#".concat(this.lblStatusId)).html(model.StatusLabel);
            }

            if (model.SelectedMetadata) {
                let uscMetadataRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataSelId)).data();
                uscMetadataRepositorySel.setComboboxText(model.SelectedMetadata);
            }

            if (model.Environments && model.Environments.length > 0) {
                this.initializeTypologies(model.Environments);
                if (model.SelectedEnvironment) {
                    let toSelectTypology: Telerik.Web.UI.RadComboBoxItem = this._rdlEntity.findItemByValue(model.SelectedEnvironment.toString());
                    toSelectTypology.select();
                }
            }

            if (model.DocumentUnits && model.DocumentUnits.length > 0) {
                this.initializeUD(model.DocumentUnits);
                if (model.SelectedDocumentUnit) {
                    let toSelectUD: Telerik.Web.UI.RadComboBoxItem = this._rdlUDType.findItemByValue(model.SelectedDocumentUnit.toString());
                    toSelectUD.select();
                }
            }
        } catch (e) {
            console.error(e);
            this.showNotification("Errore nel caricamento delle informazioni.");
        }        
    }

    getInformations(): JQueryPromise<ReportInformationViewModel> {
        let promise: JQueryDeferred<ReportInformationViewModel> = $.Deferred<ReportInformationViewModel>();
        let uscMetadataRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataSelId)).data();
        $.when(uscMetadataRepositorySel.getSelectedMetadata())
            .done((model: MetadataRepositoryModel) => {
                try {
                    let returnModel: ReportInformationViewModel = new ReportInformationViewModel();
                    returnModel.Name = this._txtName.get_value();

                    if (model) {
                        returnModel.SelectedMetadata = model.UniqueId;
                        let metadatas: MetadataDesignerViewModel = JSON.parse(model.JsonMetadata);
                        returnModel.MetadataProperties = this.fillMetadataProperties(metadatas);
                    }

                    let selectedEnv: Telerik.Web.UI.RadComboBoxItem = this._rdlEntity.get_selectedItem();
                    if (selectedEnv && selectedEnv.get_value()) {
                        returnModel.SelectedEnvironment = Number(selectedEnv.get_value());
                    }

                    let selectedUd: Telerik.Web.UI.RadComboBoxItem = this._rdlUDType.get_selectedItem();
                    if (selectedUd && selectedUd.get_value()) {
                        returnModel.SelectedDocumentUnit = Number(selectedUd.get_value());
                    }
                    promise.resolve(returnModel);
                } catch (e) {
                    console.error(e);
                    promise.reject();
                }                
            });       
        return promise.promise();
    }

    private fillMetadataProperties(metadatas: MetadataDesignerViewModel): ReportBuilderPropertyModel[] {
        let metadataProperties: ReportBuilderPropertyModel[] = [];        
        for (let textMetadata of metadatas.TextFields) {
            metadataProperties.push(this.createMetadataProperty(textMetadata.Label, ReportBuilderPropertyType.MetadataText));
        }

        for (let boolMetadata of metadatas.BoolFields) {
            metadataProperties.push(this.createMetadataProperty(boolMetadata.Label, ReportBuilderPropertyType.MetadataBool));
        }

        for (let dateMetadata of metadatas.DateFields) {
            metadataProperties.push(this.createMetadataProperty(dateMetadata.Label, ReportBuilderPropertyType.MetadataDateTime));
        }

        for (let discussionMetadata of metadatas.DiscussionFields) {
            metadataProperties.push(this.createMetadataProperty(discussionMetadata.Label, ReportBuilderPropertyType.MetadataDiscussion));
        }

        for (let enumMetadata of metadatas.EnumFields) {
            metadataProperties.push(this.createMetadataProperty(enumMetadata.Label, ReportBuilderPropertyType.MetadataEnum));
        }

        for (let numberMetadata of metadatas.NumberFields) {
            metadataProperties.push(this.createMetadataProperty(numberMetadata.Label, ReportBuilderPropertyType.MetadataNumber));
        }
        return metadataProperties;
    }

    private createMetadataProperty(name: string, propertyType: ReportBuilderPropertyType): ReportBuilderPropertyModel {
        let metadataProperty: ReportBuilderPropertyModel = new ReportBuilderPropertyModel();
        metadataProperty.DisplayName = name;
        metadataProperty.Name = name;
        metadataProperty.EntityType = Environment.Fascicle;
        metadataProperty.PropertyType = propertyType;
        return metadataProperty;
    }

    private initializeTypologies(typologies: Environment[]): void {
        let item: Telerik.Web.UI.RadComboBoxItem;
        this._rdlEntity.get_items().clear();
        for (let typology of typologies) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(Environment.toPublicDescription(typology));
            item.set_value(typology.toString());
            this._rdlEntity.get_items().add(item);
        }
    }

    private initializeUD(documentUnits: Environment[]): void {
        let item: Telerik.Web.UI.RadComboBoxItem;
        this._rdlUDType.get_items().clear();
        for (let documentUnit of documentUnits) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(Environment.toPublicDescription(documentUnit));
            item.set_value(documentUnit.toString());
            this._rdlUDType.get_items().add(item);
        }
    }

    private showNotification(message: string): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(message);
        }
    }
}

export = uscReportDesignerInformation;