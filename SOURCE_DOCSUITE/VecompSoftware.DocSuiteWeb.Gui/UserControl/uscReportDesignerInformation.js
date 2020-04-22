/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/ViewModels/Reports/ReportInformationViewModel", "App/Models/Environment", "App/Models/Reports/ReportBuilderPropertyModel", "App/Models/Reports/ReportBuilderPropertyType"], function (require, exports, ReportInformationViewModel, Environment, ReportBuilderPropertyModel, ReportBuilderPropertyType) {
    var uscReportDesignerInformation = /** @class */ (function () {
        function uscReportDesignerInformation() {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.RdlEntity_OnSelectedIndexChanged = function (sender, args) {
                var selectedItem = args.get_item();
                $('#'.concat(_this.rowMetadataId)).hide();
                $('#'.concat(_this.rowUDId)).hide();
                if (selectedItem.get_value() == Environment.Fascicle.toString()) {
                    $('#'.concat(_this.rowMetadataId)).show();
                    $('#'.concat(_this.rowUDId)).show();
                }
            };
            this.BtnLoad_OnClick = function (sender, args) {
                if (Page_ClientValidate("ReportData")) {
                    var selectedEntity = _this._rdlEntity.get_selectedItem();
                    if (!selectedEntity || !selectedEntity.get_value()) {
                        alert("E' obbligatorio selezionare una tipologia.");
                        return;
                    }
                    $("#".concat(_this.pnlContentId)).trigger(uscReportDesignerInformation.ON_EXECUTE_LOAD_EVENT);
                }
            };
        }
        /**
         *------------------------- Methods -----------------------------
         */
        uscReportDesignerInformation.prototype.initialize = function () {
            this._rdlEntity = $find(this.rdlEntityId);
            this._rdlEntity.add_selectedIndexChanged(this.RdlEntity_OnSelectedIndexChanged);
            this._rdlUDType = $find(this.rdlUDTypeId);
            this._txtName = $find(this.txtNameId);
            this._btnLoad = $find(this.btnLoadId);
            this._btnLoad.add_clicked(this.BtnLoad_OnClick);
            $('#'.concat(this.rowMetadataId)).hide();
            $('#'.concat(this.rowUDId)).hide();
            this.completeLoad();
        };
        uscReportDesignerInformation.prototype.completeLoad = function () {
            $("#".concat(this.pnlContentId)).data(this);
            $("#".concat(this.pnlContentId)).trigger(uscReportDesignerInformation.ON_END_LOAD_EVENT);
        };
        uscReportDesignerInformation.prototype.loadInformations = function (model) {
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
                    var uscMetadataRepositorySel = $("#".concat(this.uscMetadataSelId)).data();
                    uscMetadataRepositorySel.setComboboxText(model.SelectedMetadata);
                }
                if (model.Environments && model.Environments.length > 0) {
                    this.initializeTypologies(model.Environments);
                    if (model.SelectedEnvironment) {
                        var toSelectTypology = this._rdlEntity.findItemByValue(model.SelectedEnvironment.toString());
                        toSelectTypology.select();
                    }
                }
                if (model.DocumentUnits && model.DocumentUnits.length > 0) {
                    this.initializeUD(model.DocumentUnits);
                    if (model.SelectedDocumentUnit) {
                        var toSelectUD = this._rdlUDType.findItemByValue(model.SelectedDocumentUnit.toString());
                        toSelectUD.select();
                    }
                }
            }
            catch (e) {
                console.error(e);
                this.showNotification("Errore nel caricamento delle informazioni.");
            }
        };
        uscReportDesignerInformation.prototype.getInformations = function () {
            var _this = this;
            var promise = $.Deferred();
            var uscMetadataRepositorySel = $("#".concat(this.uscMetadataSelId)).data();
            $.when(uscMetadataRepositorySel.getSelectedMetadata())
                .done(function (model) {
                try {
                    var returnModel = new ReportInformationViewModel();
                    returnModel.Name = _this._txtName.get_value();
                    if (model) {
                        returnModel.SelectedMetadata = model.UniqueId;
                        var metadatas = JSON.parse(model.JsonMetadata);
                        returnModel.MetadataProperties = _this.fillMetadataProperties(metadatas);
                    }
                    var selectedEnv = _this._rdlEntity.get_selectedItem();
                    if (selectedEnv && selectedEnv.get_value()) {
                        returnModel.SelectedEnvironment = Number(selectedEnv.get_value());
                    }
                    var selectedUd = _this._rdlUDType.get_selectedItem();
                    if (selectedUd && selectedUd.get_value()) {
                        returnModel.SelectedDocumentUnit = Number(selectedUd.get_value());
                    }
                    promise.resolve(returnModel);
                }
                catch (e) {
                    console.error(e);
                    promise.reject();
                }
            });
            return promise.promise();
        };
        uscReportDesignerInformation.prototype.fillMetadataProperties = function (metadatas) {
            var metadataProperties = [];
            for (var _i = 0, _a = metadatas.TextFields; _i < _a.length; _i++) {
                var textMetadata = _a[_i];
                metadataProperties.push(this.createMetadataProperty(textMetadata.Label, ReportBuilderPropertyType.MetadataText));
            }
            for (var _b = 0, _c = metadatas.BoolFields; _b < _c.length; _b++) {
                var boolMetadata = _c[_b];
                metadataProperties.push(this.createMetadataProperty(boolMetadata.Label, ReportBuilderPropertyType.MetadataBool));
            }
            for (var _d = 0, _e = metadatas.DateFields; _d < _e.length; _d++) {
                var dateMetadata = _e[_d];
                metadataProperties.push(this.createMetadataProperty(dateMetadata.Label, ReportBuilderPropertyType.MetadataDateTime));
            }
            for (var _f = 0, _g = metadatas.DiscussionFields; _f < _g.length; _f++) {
                var discussionMetadata = _g[_f];
                metadataProperties.push(this.createMetadataProperty(discussionMetadata.Label, ReportBuilderPropertyType.MetadataDiscussion));
            }
            for (var _h = 0, _j = metadatas.EnumFields; _h < _j.length; _h++) {
                var enumMetadata = _j[_h];
                metadataProperties.push(this.createMetadataProperty(enumMetadata.Label, ReportBuilderPropertyType.MetadataEnum));
            }
            for (var _k = 0, _l = metadatas.NumberFields; _k < _l.length; _k++) {
                var numberMetadata = _l[_k];
                metadataProperties.push(this.createMetadataProperty(numberMetadata.Label, ReportBuilderPropertyType.MetadataNumber));
            }
            return metadataProperties;
        };
        uscReportDesignerInformation.prototype.createMetadataProperty = function (name, propertyType) {
            var metadataProperty = new ReportBuilderPropertyModel();
            metadataProperty.DisplayName = name;
            metadataProperty.Name = name;
            metadataProperty.EntityType = Environment.Fascicle;
            metadataProperty.PropertyType = propertyType;
            return metadataProperty;
        };
        uscReportDesignerInformation.prototype.initializeTypologies = function (typologies) {
            var item;
            this._rdlEntity.get_items().clear();
            for (var _i = 0, typologies_1 = typologies; _i < typologies_1.length; _i++) {
                var typology = typologies_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(Environment.toPublicDescription(typology));
                item.set_value(typology.toString());
                this._rdlEntity.get_items().add(item);
            }
        };
        uscReportDesignerInformation.prototype.initializeUD = function (documentUnits) {
            var item;
            this._rdlUDType.get_items().clear();
            for (var _i = 0, documentUnits_1 = documentUnits; _i < documentUnits_1.length; _i++) {
                var documentUnit = documentUnits_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(Environment.toPublicDescription(documentUnit));
                item.set_value(documentUnit.toString());
                this._rdlUDType.get_items().add(item);
            }
        };
        uscReportDesignerInformation.prototype.showNotification = function (message) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(message);
            }
        };
        uscReportDesignerInformation.ON_END_LOAD_EVENT = "onEndLoad";
        uscReportDesignerInformation.ON_EXECUTE_LOAD_EVENT = "onExecuteLoad";
        return uscReportDesignerInformation;
    }());
    return uscReportDesignerInformation;
});
//# sourceMappingURL=uscReportDesignerInformation.js.map