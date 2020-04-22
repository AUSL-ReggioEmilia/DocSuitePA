/// <reference path="Service.ts" />
/// <reference path="../App/ViewModels/CategoryViewModel.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var UdsDesigner;
(function (UdsDesigner) {
    var CtrlBase = /** @class */ (function () {
        function CtrlBase() {
            this.initCallbacks = new Array();
            this.modifyEnable = true;
        }
        CtrlBase.getElementCtrl = function (element) {
            return jQuery.data(element, "ctrlData");
        };
        CtrlBase.prototype.bind = function (element) {
            this.unbind();
            jQuery.data(element, "ctrlData", this);
            this.binder = rivets.bind(element, { ctrl: this });
        };
        CtrlBase.prototype.unbind = function () {
            if (this.binder != null) {
                this.binder.unbind();
                this.binder = null;
            }
        };
        CtrlBase.prototype.clone = function () {
            return jQuery.extend(true, {}, this);
        };
        CtrlBase.prototype.setValues = function (obj) {
            this.modifyEnable = obj.modifyEnable;
        };
        CtrlBase.prototype.getValues = function () {
            return {
                modifyEnable: this.modifyEnable
            };
        };
        CtrlBase.prototype.afterAppend = function () {
            //Da implementare nelle classi che estendono CtrlBase, chiamato all'aggiunta di un pannello
        };
        CtrlBase.prototype.beforeRemoval = function () {
            //Da implementare nelle classi che estendono CtrlBase, chiamato alla rimozione di un pannello
        };
        CtrlBase.prototype.onConfirm = function () {
            //Da implementare nelle classi che estendono CtrlBase, chiamato al click di conferma in un pannello
        };
        return CtrlBase;
    }());
    UdsDesigner.CtrlBase = CtrlBase;
    var TitleCtrl = /** @class */ (function (_super) {
        __extends(TitleCtrl, _super);
        function TitleCtrl() {
            var _this = _super.call(this) || this;
            _this.selectedItem = 1;
            _this.ctrlType = "Title";
            _this.enabledWorkflow = false;
            _this.enabledProtocol = false;
            _this.enabledPEC = false;
            _this.enabledPECButton = false;
            _this.enabledMailButton = false;
            _this.enabledMailRoleButton = false;
            _this.enabledLinkButton = false;
            _this.visibleMailButton = false;
            _this.enabledCQRSSync = true;
            _this.categorySearchable = true;
            _this.categoryReadOnly = false;
            _this.categoryDefaultEnabled = true;
            _this.titleReadOnly = false;
            _this.containerReadOnly = true;
            _this.containerSearchable = true;
            _this.createContainer = true;
            _this.enabledCancelMotivation = true;
            _this.alias = "";
            _this.label = "";
            _this.incrementalIdentityEnabled = true;
            _this.signatureMetadataEnabled = false;
            _this.enableManualSignature = false;
            _this.idContainer = "";
            _this.enableWorkflowCheckBox = !($("#workflowManager")[0].getAttribute("value") == 'True');
            _this.modifyEnable = true;
            _this.subjectResultVisibility = true;
            _this.categoryResultVisibility = true;
            _this.hideRegistrationIdentifier = false;
            _this.requiredRevisionUDSRepository = false;
            _this.initializeEvents();
            return _this;
        }
        TitleCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.subject = obj.subject;
            this.enabledWorkflow = obj.enabledWorkflow;
            this.enabledProtocol = obj.enabledProtocol;
            this.enabledPEC = obj.enabledPEC;
            this.enabledPECButton = obj.enabledPECButton;
            this.enabledMailButton = obj.enabledMailButton;
            this.enabledMailRoleButton = obj.enabledMailRoleButton;
            this.enabledLinkButton = obj.enabledLinkButton;
            this.visibleMailButton = obj.visibleMailButton;
            this.enabledCancelMotivation = obj.enabledCancelMotivation;
            this.enabledCQRSSync = obj.enabledCQRSSync;
            this.idRepository = obj.idRepository;
            this.DSWEnvironment = obj.DSWEnvironment;
            this.idCategory = obj.idCategory;
            this.categoryName = obj.categoryName;
            this.categorySearchable = obj.categorySearchable;
            this.categoryDefaultEnabled = obj.categoryDefaultEnabled;
            this.idContainer = obj.idContainer;
            this.containerName = obj.containerName;
            this.categoryReadOnly = obj.categoryReadOnly;
            this.titleReadOnly = obj.titleReadOnly;
            this.containerReadOnly = obj.containerReadOnly;
            this.containerSearchable = obj.containerSearchable;
            this.createContainer = obj.createContainer;
            this.hideRegistrationIdentifier = obj.hideRegistrationIdentifier;
            this.requiredRevisionUDSRepository = obj.requiredRevisionUDSRepository;
            if (this.createContainer) {
                this.idContainer = "";
                this.containerName = "";
            }
            this.incrementalIdentityEnabled = obj.incrementalIdentityEnabled;
            this.signatureMetadataEnabled = obj.signatureMetadataEnabled;
            this.enableManualSignature = obj.enableManualSignature;
            this.alias = obj.alias;
            this.enableWorkflowCheckBox = obj.enableWorkflowCheckBox;
            this.subjectResultVisibility = obj.subjectResultVisibility;
            this.categoryResultVisibility = obj.categoryResultVisibility;
            this.modifyEnable = obj.modifyEnable;
        };
        TitleCtrl.prototype.getValues = function () {
            if (this.createContainer) {
                this.idContainer = "";
                this.containerName = "";
            }
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                subject: this.subject,
                enabledWorkflow: this.enabledWorkflow,
                enabledProtocol: this.enabledProtocol,
                enabledPEC: this.enabledPEC,
                enabledPECButton: this.enabledPECButton,
                enabledMailButton: this.enabledMailButton,
                enabledMailRoleButton: this.enabledMailRoleButton,
                enabledLinkButton: this.enabledLinkButton,
                visibleMailButton: this.visibleMailButton,
                enabledCQRSSync: this.enabledCQRSSync,
                idRepository: this.idRepository,
                DSWEnvironment: this.DSWEnvironment,
                idCategory: this.idCategory,
                categoryName: this.categoryName,
                categorySearchable: this.categorySearchable,
                categoryDefaultEnabled: this.categoryDefaultEnabled,
                categoryReadOnly: this.categoryReadOnly,
                titleReadOnly: this.titleReadOnly,
                idContainer: this.idContainer,
                containerName: this.containerName,
                containerReadOnly: this.containerReadOnly,
                containerSearchable: this.containerSearchable,
                createContainer: this.createContainer,
                alias: this.alias,
                incrementalIdentityEnabled: this.incrementalIdentityEnabled,
                signatureMetadataEnabled: this.signatureMetadataEnabled,
                enabledCancelMotivation: this.enabledCancelMotivation,
                hideRegistrationIdentifier: this.hideRegistrationIdentifier,
                requiredRevisionUDSRepository: this.requiredRevisionUDSRepository,
                enableAutomaticSignature: this.enableManualSignature,
                enableWorkflowCheckBox: this.enableWorkflowCheckBox,
                subjectResultVisibility: this.subjectResultVisibility,
                categoryResultVisibility: this.categoryResultVisibility,
                modifyEnable: this.modifyEnable
            };
        };
        TitleCtrl.prototype.selectedItemChanged = function (e, scope) {
            var columns = $("#itemChanger").val();
            scope.ctrl.binder.bind($("#TitleOption")[0], scope.ctrl);
            scope.ctrl.selectedItem = columns;
        };
        TitleCtrl.prototype.selectCategory = function (e, scope) {
            var categoryModel = new UdsDesigner.CategoryViewModel();
            scope.ctrl.openConfigurationModal(categoryModel);
        };
        TitleCtrl.prototype.selectContainer = function (e, scope) {
            var containerModel = new UdsDesigner.ContainerViewModel();
            scope.ctrl.openConfigurationModal(containerModel);
        };
        TitleCtrl.prototype.openConfigurationModal = function (model) {
            this.bind($("#TitleOption")[0]);
            var containerDialog = $("#configuration_modal");
            var context = this;
            UdsDesigner.Service.loadControls("configurationModal", function (data) {
                containerDialog.html(data);
                var content = containerDialog.find('#configuration_body_modal');
                model.setup();
                containerDialog.on('show.bs.modal', function () {
                    content.css('overflow-y', 'auto');
                    content.css('max-height', $(window).height() * 0.6);
                });
                containerDialog.modal('show');
            });
        };
        TitleCtrl.prototype.closeConfigurationModal = function () {
            var categoryDialog = $("#configuration_modal");
            categoryDialog.modal('hide');
        };
        TitleCtrl.prototype.initializeEvents = function () {
            var context = this;
            $(document).on("categorySelected", function (event, idCategory, categoryName) {
                var el = CtrlBase.getElementCtrl($("#TitleOption")[0]);
                el.idCategory = idCategory;
                el.categoryName = categoryName;
                el.bind($("#TitleOption")[0]);
                context.closeConfigurationModal();
            });
            $(document).on("containerSelected", function (event, idContainer, containerName) {
                var el = CtrlBase.getElementCtrl($("#TitleOption")[0]);
                el.idContainer = idContainer;
                el.containerName = containerName;
                el.bind($("#TitleOption")[0]);
                context.closeConfigurationModal();
            });
        };
        return TitleCtrl;
    }(CtrlBase));
    UdsDesigner.TitleCtrl = TitleCtrl;
    var HeaderCtrl = /** @class */ (function (_super) {
        __extends(HeaderCtrl, _super);
        function HeaderCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Header";
            _this.label = "Sezione";
            return _this;
        }
        HeaderCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        HeaderCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                columns: this.columns,
                rows: this.rows
            };
        };
        return HeaderCtrl;
    }(CtrlBase));
    UdsDesigner.HeaderCtrl = HeaderCtrl;
    var TextCtrl = /** @class */ (function (_super) {
        __extends(TextCtrl, _super);
        function TextCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Text";
            _this.label = "Testo";
            _this.multiLine = false;
            _this.HTMLEnable = false;
            _this.defaultValue = "";
            _this.defaultSearchValue = "";
            _this.required = false;
            _this.readOnly = false;
            _this.searchable = true;
            _this.isSignature = false;
            _this.hiddenField = false;
            _this.requiredEnabled = false;
            _this.resultVisibility = false;
            _this.resultPosition = 0;
            return _this;
        }
        TextCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.multiLine = obj.multiLine;
            this.HTMLEnable = obj.HTMLEnable;
            this.defaultValue = obj.defaultValue;
            this.defaultSearchValue = obj.defaultSearchValue;
            this.required = obj.required;
            this.readOnly = obj.readOnly;
            this.searchable = obj.searchable;
            this.isSignature = obj.isSignature;
            this.hiddenField = obj.hiddenField;
            this.modifyEnable = obj.modifyEnable;
            this.requiredEnabled = obj.requiredEnabled;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        TextCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                multiLine: this.multiLine,
                HTMLEnable: this.HTMLEnable,
                defaultValue: this.defaultValue,
                defaultSearchValue: this.defaultSearchValue,
                required: this.required,
                readOnly: this.readOnly,
                searchable: this.searchable,
                isSignature: this.isSignature,
                hiddenField: this.hiddenField,
                modifyEnable: this.modifyEnable,
                requiredEnabled: this.requiredEnabled,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                columns: this.columns,
                rows: this.rows
            };
        };
        TextCtrl.prototype.beforeRemoval = function () {
            $(".element-Title").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.ctrlType == "Title") {
                    var ctrlTitle = ctrl;
                    ctrlTitle.signatureMetadataEnabled = true;
                    ctrlTitle.enableManualSignature = false;
                    ctrlTitle.bind($(".element-Title")[index]);
                }
            });
        };
        TextCtrl.prototype.onConfirm = function () {
            $(".element-Title").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.ctrlType == "Title") {
                    var ctrlTitle = ctrl;
                    ctrlTitle.enableManualSignature = $("#isSignature").is(':checked');
                    ctrlTitle.bind($(".element-Title")[index]);
                }
            });
        };
        TextCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof TextCtrl) {
                    var ctrlTitle = ctrl;
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    if (!$("#multiline").is(':checked')) {
                        $("#HTMLEnable").prop('checked', false);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        };
        return TextCtrl;
    }(CtrlBase));
    UdsDesigner.TextCtrl = TextCtrl;
    var DateCtrl = /** @class */ (function (_super) {
        __extends(DateCtrl, _super);
        function DateCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Date";
            _this.label = "Data";
            _this.restrictedYear = false;
            _this.enableDefaultDate = false;
            _this.defaultValue = "";
            _this.defaultSearchValue = "";
            _this.required = false;
            _this.readOnly = false;
            _this.searchable = true;
            _this.hiddenField = false;
            _this.requiredEnabled = false;
            _this.resultVisibility = false;
            _this.resultPosition = 0;
            return _this;
        }
        DateCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.restrictedYear = obj.restrictedYear;
            this.enableDefaultDate = obj.enableDefaultDate;
            this.defaultValue = obj.defaultValue;
            this.defaultSearchValue = obj.defaultSearchValue;
            this.required = obj.required;
            this.readOnly = obj.readOnly;
            this.searchable = obj.searchable;
            this.hiddenField = obj.hiddenField;
            this.modifyEnable = obj.modifyEnable;
            this.requiredEnabled = obj.requiredEnabled;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        DateCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                restrictedYear: this.restrictedYear,
                enableDefaultDate: this.enableDefaultDate,
                defaultValue: this.defaultValue,
                defaultSearchValue: this.defaultSearchValue,
                required: this.required,
                readOnly: this.readOnly,
                searchable: this.searchable,
                hiddenField: this.hiddenField,
                modifyEnable: this.modifyEnable,
                requiredEnabled: this.requiredEnabled,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                columns: this.columns,
                rows: this.rows
            };
        };
        DateCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof DateCtrl) {
                    var ctrlTitle = ctrl;
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        };
        return DateCtrl;
    }(CtrlBase));
    UdsDesigner.DateCtrl = DateCtrl;
    var NumberCtrl = /** @class */ (function (_super) {
        __extends(NumberCtrl, _super);
        function NumberCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Number";
            _this.label = "Numero";
            _this.defaultValue = "";
            _this.defaultSearchValue = "";
            _this.required = false;
            _this.readOnly = false;
            _this.searchable = true;
            _this.hiddenField = false;
            _this.requiredEnabled = false;
            _this.resultVisibility = false;
            _this.resultPosition = 0;
            _this.format = "";
            return _this;
        }
        NumberCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.defaultValue = obj.defaultValue;
            this.defaultSearchValue = obj.defaultSearchValue;
            this.required = obj.required;
            this.readOnly = obj.readOnly;
            this.searchable = obj.searchable;
            this.hiddenField = obj.hiddenField;
            this.modifyEnable = obj.modifyEnable;
            this.requiredEnabled = obj.requiredEnabled;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
            this.format = obj.format;
        };
        NumberCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                defaultValue: this.defaultValue,
                defaultSearchValue: this.defaultSearchValue,
                required: this.required,
                readOnly: this.readOnly,
                searchable: this.searchable,
                hiddenField: this.hiddenField,
                modifyEnable: this.modifyEnable,
                requiredEnabled: this.requiredEnabled,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                columns: this.columns,
                rows: this.rows,
                format: this.format
            };
        };
        NumberCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof NumberCtrl) {
                    var ctrlTitle = ctrl;
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        };
        return NumberCtrl;
    }(CtrlBase));
    UdsDesigner.NumberCtrl = NumberCtrl;
    var CheckboxCtrl = /** @class */ (function (_super) {
        __extends(CheckboxCtrl, _super);
        function CheckboxCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Checkbox";
            _this.label = "Checkbox";
            _this.defaultValue = "";
            _this.defaultSearchValue = "";
            _this.required = false;
            _this.readOnly = false;
            _this.searchable = true;
            _this.hiddenField = false;
            _this.requiredEnabled = false;
            return _this;
        }
        CheckboxCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.defaultValue = obj.defaultValue;
            this.defaultSearchValue = obj.defaultSearchValue;
            this.required = obj.required;
            this.readOnly = obj.readOnly;
            this.searchable = obj.searchable;
            this.hiddenField = obj.hiddenField;
            this.modifyEnable = obj.modifyEnable;
            this.requiredEnabled = obj.requiredEnabled;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        CheckboxCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                defaultValue: this.defaultValue,
                defaultSearchValue: this.defaultSearchValue,
                required: this.required,
                readOnly: this.readOnly,
                searchable: this.searchable,
                hiddenField: this.hiddenField,
                modifyEnable: this.modifyEnable,
                requiredEnabled: this.requiredEnabled,
                columns: this.columns,
                rows: this.rows
            };
        };
        CheckboxCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof CheckboxCtrl) {
                    var ctrlTitle = ctrl;
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        };
        return CheckboxCtrl;
    }(CtrlBase));
    UdsDesigner.CheckboxCtrl = CheckboxCtrl;
    var LookupCtrl = /** @class */ (function (_super) {
        __extends(LookupCtrl, _super);
        function LookupCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Lookup";
            _this.label = "Lookup";
            _this.lookupRepositoryName = "Seleziona archivio ";
            _this.lookupFieldName = "Seleziona proprietà ";
            _this.required = false;
            _this.searchable = true;
            _this.hiddenField = false;
            _this.requiredEnabled = false;
            _this.repositories = [];
            _this.fields = [];
            _this.isLoadingRepositories = false;
            _this.isLoadingFields = false;
            _this.fieldsReadOnly = true;
            _this.resultVisibility = false;
            _this.resultPosition = 0;
            _this.multipleValues = false;
            _this.multipleEnabled = true;
            return _this;
        }
        LookupCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.required = obj.required;
            this.searchable = obj.searchable;
            this.hiddenField = obj.hiddenField;
            this.modifyEnable = obj.modifyEnable;
            this.requiredEnabled = obj.requiredEnabled;
            this.lookupRepositoryName = obj.lookupRepositoryName;
            this.lookupFieldName = obj.lookupFieldName;
            this.fieldsReadOnly = obj.fieldsReadOnly;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.multipleValues = obj.multipleValues;
            this.multipleEnabled = false;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        LookupCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                required: this.required,
                searchable: this.searchable,
                hiddenField: this.hiddenField,
                modifyEnable: this.modifyEnable,
                requiredEnabled: this.requiredEnabled,
                lookupRepositoryName: this.lookupRepositoryName,
                lookupFieldName: this.lookupFieldName,
                fieldsReadOnly: this.fieldsReadOnly,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                multipleValues: this.multipleValues,
                multipleEnabled: this.multipleEnabled,
                columns: this.columns,
                rows: this.rows
            };
        };
        LookupCtrl.prototype.getHiddenValues = function () {
            return {
                repositories: this.repositories,
                isLoadingRepositories: this.isLoadingRepositories,
                fields: this.fields,
                isLoadingFields: this.isLoadingFields
            };
        };
        LookupCtrl.prototype.getRepositories = function (e, scope) {
            var elements = scope.ctrl.getHiddenValues();
            if (elements.repositories && elements.repositories.length > 0) {
                return;
            }
            scope.ctrl.isLoadingRepositories = true;
            scope.ctrl.binder.bind($("#LookupOption")[0], scope.ctrl);
            var context = scope;
            UdsDesigner.Service.loadUDSRepositories(function (data) {
                if (data && data.d) {
                    context.ctrl.repositories = data.d;
                    context.ctrl.isLoadingRepositories = false;
                    context.ctrl.binder.bind($("#LookupOption")[0], context.ctrl);
                }
            });
        };
        LookupCtrl.prototype.selectedRepositoryItemChanged = function (e, scope) {
            scope.ctrl.lookupRepositoryName = e.target.innerText;
            scope.ctrl.lookupFieldName = "Seleziona proprietà ";
            scope.ctrl.fields = null;
            scope.ctrl.fieldsReadOnly = false;
            scope.ctrl.binder.bind($("#LookupOption")[0], scope.ctrl);
        };
        LookupCtrl.prototype.selectedFieldItemChanged = function (e, scope) {
            scope.ctrl.lookupFieldName = e.target.innerText;
            scope.ctrl.binder.bind($("#LookupOption")[0], scope.ctrl);
        };
        LookupCtrl.prototype.getFields = function (e, scope) {
            var elements = scope.ctrl.getHiddenValues();
            if (elements.fields && elements.fields.length > 0) {
                return;
            }
            scope.ctrl.isLoadingFields = true;
            scope.ctrl.binder.bind($("#LookupOption")[0], scope.ctrl);
            var context = scope;
            UdsDesigner.Service.loadUDSFields(function (data) {
                if (data && data.d && data.d[0]) {
                    context.ctrl.fields = data.d[0];
                    var values = data.d[0];
                    values.push("Oggetto");
                    context.ctrl.fields = values;
                    context.ctrl.isLoadingFields = false;
                    context.ctrl.binder.bind($("#LookupOption")[0], context.ctrl);
                }
            });
        };
        LookupCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof LookupCtrl) {
                    var ctrlTitle = ctrl;
                    ctrlTitle.requiredEnabled = $("#hiddenField").is(':checked');
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        };
        return LookupCtrl;
    }(CtrlBase));
    UdsDesigner.LookupCtrl = LookupCtrl;
    var DocumentCtrl = /** @class */ (function (_super) {
        __extends(DocumentCtrl, _super);
        function DocumentCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Document";
            _this.label = "Documenti";
            _this.archive = "Archivio";
            _this.collectionType = "*";
            _this.readOnly = false;
            _this.searchable = true;
            _this.required = true;
            _this.enableMultifile = true;
            _this.enableUpload = true;
            _this.enableScanner = true;
            _this.enableSign = true;
            _this.copyProtocol = false;
            _this.copyResolution = false;
            _this.copySeries = false;
            _this.archives = [];
            _this.isLoadingArchives = false;
            _this.archiveReadOnly = false;
            _this.signRequired = false;
            _this.createBiblosArchive = true;
            _this.documentDeletable = false;
            _this.dematerialisationEnabled = false;
            _this.enableDematerialisationCheckBox = !($("#dematerialisationEnabled")[0].getAttribute("value") == 'True');
            return _this;
        }
        DocumentCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.archive = obj.archive;
            this.collectionType = obj.collectionType;
            this.readOnly = obj.readOnly;
            this.searchable = obj.searchable;
            this.enableMultifile = obj.enableMultifile;
            this.enableUpload = obj.enableUpload;
            this.enableScanner = obj.enableScanner;
            this.enableSign = obj.enableSign;
            this.copyProtocol = obj.copyProtocol;
            this.copyResolution = obj.copyResolution;
            this.copySeries = obj.copySeries;
            this.archiveReadOnly = obj.archiveReadOnly;
            this.signRequired = obj.signRequired;
            this.createBiblosArchive = obj.createBiblosArchive;
            this.required = obj.required;
            this.documentDeletable = obj.documentDeletable;
            this.dematerialisationEnabled = obj.dematerialisationEnabled;
            this.modifyEnable = obj.modifyEnable;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        DocumentCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                archive: this.archive,
                collectionType: this.collectionType,
                readOnly: this.readOnly,
                searchable: this.searchable,
                required: this.required,
                enableMultifile: this.enableMultifile,
                enableUpload: this.enableUpload,
                enableScanner: this.enableScanner,
                enableSign: this.enableSign,
                copyProtocol: this.copyProtocol,
                copyResolution: this.copyResolution,
                copySeries: this.copySeries,
                archiveReadOnly: this.archiveReadOnly,
                signRequired: this.signRequired,
                createBiblosArchive: this.createBiblosArchive,
                documentDeletable: this.documentDeletable,
                dematerialisationEnabled: this.dematerialisationEnabled,
                modifyEnable: this.modifyEnable,
                columns: this.columns,
                rows: this.rows
            };
        };
        DocumentCtrl.prototype.getHiddenValues = function () {
            return {
                archives: this.archives,
                isLoadingArchives: this.isLoadingArchives
            };
        };
        DocumentCtrl.prototype.getBiblosArchives = function (e, scope) {
            var elements = scope.ctrl.getHiddenValues();
            if (elements.archives && elements.archives.length > 0) {
                return;
            }
            scope.ctrl.isLoadingArchives = true;
            scope.ctrl.binder.bind($("#DocumentOption")[0], scope.ctrl);
            var context = scope;
            UdsDesigner.Service.loadBiblosArchives(function (data) {
                context.ctrl.archives = data.d;
                context.ctrl.isLoadingArchives = false;
                context.ctrl.binder.bind($("#DocumentOption")[0], context.ctrl);
            });
        };
        DocumentCtrl.prototype.selectedItemChanged = function (e, scope) {
            var elements = scope.ctrl.getValues();
            scope.ctrl.archive = e.target.innerText;
            scope.ctrl.binder.bind($("#DocumentOption")[0], scope.ctrl);
        };
        return DocumentCtrl;
    }(CtrlBase));
    UdsDesigner.DocumentCtrl = DocumentCtrl;
    var ContactCtrl = /** @class */ (function (_super) {
        __extends(ContactCtrl, _super);
        function ContactCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Contact";
            _this.label = "Contatti";
            _this.enableAD = true;
            _this.enableAddressBook = false;
            _this.enableADDistribution = false;
            _this.enableManual = true;
            _this.enableExcelImport = false;
            _this.enableMultiContact = true;
            _this.isProtocollable = false;
            _this.required = false;
            _this.searchable = true;
            _this.resultVisibility = false;
            _this.resultPosition = 0;
            _this.initCallbacks.push(_this.initialize);
            return _this;
        }
        ContactCtrl.prototype.initialize = function (context) {
            var titleData = CtrlBase.getElementCtrl($(".element-Title")[0]).getValues();
            context.isProtocollable = false;
            if (titleData) {
                context.isProtocollable = titleData.enabledProtocol;
            }
        };
        ContactCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.enableAD = obj.enableAD;
            this.enableAddressBook = obj.enableAddressBook;
            this.enableADDistribution = obj.enableADDistribution;
            this.enableManual = obj.enableManual;
            this.enableExcelImport = obj.enableExcelImport;
            this.enableMultiContact = obj.enableMultiContact;
            this.isProtocollable = obj.isProtocollable;
            this.contactTypeSelected = obj.contactTypeSelected;
            this.required = obj.required;
            this.searchable = obj.searchable;
            this.modifyEnable = obj.modifyEnable;
            this.contactTypes = obj.contactTypes;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        ContactCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                enableAD: this.enableAD,
                enableAddressBook: this.enableAddressBook,
                enableADDistribution: this.enableADDistribution,
                enableManual: this.enableManual,
                enableExcelImport: this.enableExcelImport,
                enableMultiContact: this.enableMultiContact,
                isProtocollable: this.isProtocollable,
                contactTypeSelected: this.contactTypeSelected,
                required: this.required,
                searchable: this.searchable,
                modifyEnable: this.modifyEnable,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                columns: this.columns,
                rows: this.rows
            };
        };
        ContactCtrl.prototype.selectedItemChanged = function (e, scope) {
            var elements = scope.ctrl.getValues();
            scope.ctrl.contactTypeSelected = e.target.innerText;
            scope.ctrl.binder.bind($("#ContactOption")[0], scope.ctrl);
        };
        ContactCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof ContactCtrl) {
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                }
            });
        };
        ContactCtrl.prototype.getContactTypes = function (e, scope) {
            scope.ctrl.binder.bind($("#ContactOption")[0], scope.ctrl);
            var context = scope;
            UdsDesigner.Service.loadContactTypes(function (data) {
                context.ctrl.contactTypes = data.d;
                context.ctrl.binder.bind($("#ContactOption")[0], context.ctrl);
            });
        };
        return ContactCtrl;
    }(CtrlBase));
    UdsDesigner.ContactCtrl = ContactCtrl;
    var AuthorizationCtrl = /** @class */ (function (_super) {
        __extends(AuthorizationCtrl, _super);
        function AuthorizationCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Authorization";
            _this.label = "Autorizzazione";
            _this.enableMultiAuth = true;
            _this.required = false;
            _this.searchable = true;
            _this.resultVisibility = false;
            _this.resultPosition = 0;
            return _this;
        }
        AuthorizationCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.enableMultiAuth = obj.enableMultiAuth;
            this.required = obj.required;
            this.searchable = obj.searchable;
            this.modifyEnable = obj.modifyEnable;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        AuthorizationCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                enableMultiAuth: this.enableMultiAuth,
                required: this.required,
                searchable: this.searchable,
                modifyEnable: this.modifyEnable,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                rows: this.rows,
                columns: this.columns
            };
        };
        AuthorizationCtrl.prototype.afterAppend = function () {
            $(".element-Title").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.ctrlType == "Title") {
                    var ctrlTitle = ctrl;
                    ctrlTitle.visibleMailButton = true;
                    ctrlTitle.bind($(".element-Title")[index]);
                }
            });
        };
        AuthorizationCtrl.prototype.beforeRemoval = function () {
            $(".element-Title").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.ctrlType == "Title") {
                    var ctrlTitle = ctrl;
                    ctrlTitle.visibleMailButton = false;
                    ctrlTitle.enabledMailRoleButton = false;
                    ctrlTitle.bind($(".element-Title")[index]);
                }
            });
        };
        AuthorizationCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof AuthorizationCtrl) {
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                }
            });
        };
        return AuthorizationCtrl;
    }(CtrlBase));
    UdsDesigner.AuthorizationCtrl = AuthorizationCtrl;
    var EnumCtrl = /** @class */ (function (_super) {
        __extends(EnumCtrl, _super);
        function EnumCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Enum";
            _this.label = "Scelta Multipla";
            _this.defaultValue = "";
            _this.defaultSearchValue = "";
            _this.readOnly = false;
            _this.required = false;
            _this.searchable = true;
            _this.enumOptions = [];
            _this.hiddenField = false;
            _this.requiredEnabled = false;
            _this.resultVisibility = false;
            _this.resultPosition = 0;
            _this.multipleValues = false;
            _this.multipleEnabled = true;
            return _this;
        }
        EnumCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.defaultValue = obj.defaultValue;
            this.defaultSearchValue = obj.defaultSearchValue;
            this.readOnly = obj.readOnly;
            this.required = obj.required;
            this.searchable = obj.searchable;
            this.enumOptions = obj.enumOptions;
            this.hiddenField = obj.hiddenField;
            this.modifyEnable = obj.modifyEnable;
            this.requiredEnabled = obj.requiredEnabled;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
            this.multipleValues = obj.multipleValues;
            this.multipleEnabled = false;
        };
        EnumCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                defaultValue: this.defaultValue,
                defaultSearchValue: this.defaultSearchValue,
                required: this.required,
                readOnly: this.readOnly,
                searchable: this.searchable,
                enumOptions: this.enumOptions,
                hiddenField: this.hiddenField,
                modifyEnable: this.modifyEnable,
                requiredEnabled: this.requiredEnabled,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                columns: this.columns,
                rows: this.rows,
                multipleValues: this.multipleValues,
                multipleEnabled: this.multipleEnabled
            };
        };
        EnumCtrl.prototype.deleteItem = function (e, scope) {
            scope.ctrl.enumOptions.splice(scope.index, 1);
        };
        EnumCtrl.prototype.selectedValue = function (e, scope) {
            scope.ctrl.OpenEnumDetailsModal(e, scope);
        };
        EnumCtrl.prototype.OpenEnumDetailsModal = function (e, scope) {
            this.enumValue = e.currentTarget.innerHTML;
            console.log(scope);
            var containerDialog = $("#configuration_modal");
            var context = this;
            UdsDesigner.Service.loadControls("enumDetailsModal", function (data) {
                containerDialog.html(data);
                var content = containerDialog.find('#configuration_body_modal');
                containerDialog.on('show.bs.modal', function () {
                    content.css('overflow-y', 'auto');
                    content.css('max-height', $(window).height() * 0.6);
                });
                containerDialog.modal('show');
                $("#txtEnumValue").val(context.enumValue);
                $("#enum_save").click(function () {
                    if ($("#txtEnumValue").val() === "") {
                        alert("Impossibile aggiungere un valore vuoto");
                    }
                    else {
                        context.enumValue = $("#txtEnumValue").val();
                        for (var i = 0; i < scope.ctrl.enumOptions.length; i++) {
                            if (scope.ctrl.enumOptions[i] === e.currentTarget.innerHTML) {
                                scope.ctrl.enumOptions[i] = context.enumValue;
                            }
                        }
                        e.currentTarget.innerHTML = context.enumValue;
                        containerDialog.modal('hide');
                    }
                });
            });
        };
        EnumCtrl.prototype.addNewOption = function (e, scope) {
            var elements = scope.ctrl.getValues();
            if (elements.enumOptions == undefined) {
                scope.ctrl.enumOptions = [];
            }
            var value = $("#txtAddElement").val();
            if ($.inArray(value, scope.ctrl.enumOptions) > -1) {
                alert("E' già presente un elemento con lo stesso nome.");
                return;
            }
            scope.ctrl.enumOptions.push(value);
            $("#txtAddElement").val("");
            scope.ctrl.binder.bind($("#EnumOption")[0], scope.ctrl);
        };
        EnumCtrl.prototype.selectedItemChanged = function (e, scope) {
            var elements = scope.ctrl.getValues();
            scope.ctrl.defaultValue = e.target.innerText;
            scope.ctrl.binder.bind($("#EnumOption")[0], scope.ctrl);
        };
        EnumCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof EnumCtrl) {
                    var ctrlTitle = ctrl;
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        };
        return EnumCtrl;
    }(CtrlBase));
    UdsDesigner.EnumCtrl = EnumCtrl;
    var StatusType = /** @class */ (function () {
        function StatusType() {
        }
        return StatusType;
    }());
    UdsDesigner.StatusType = StatusType;
    var StatusCtrl = /** @class */ (function (_super) {
        __extends(StatusCtrl, _super);
        function StatusCtrl() {
            var _this = _super.call(this) || this;
            _this.ctrlType = "Status";
            _this.label = "Stato";
            _this.defaultValue = "";
            _this.defaultSearchValue = "";
            _this.readOnly = false;
            _this.required = false;
            _this.searchable = true;
            _this.statusValue = "";
            _this.statusType = [];
            _this.hiddenField = false;
            _this.requiredEnabled = false;
            _this.resultVisibility = false;
            _this.resultPosition = 0;
            return _this;
        }
        StatusCtrl.prototype.setValues = function (obj) {
            this.label = obj.label;
            this.defaultValue = obj.defaultValue;
            this.defaultSearchValue = obj.defaultSearchValue;
            this.readOnly = obj.readOnly;
            this.required = obj.required;
            this.searchable = obj.searchable;
            this.statusValue = obj.statusValue;
            this.statusType = obj.statusType;
            this.hiddenField = obj.hiddenField;
            this.modifyEnable = obj.modifyEnable;
            this.requiredEnabled = obj.requiredEnabled;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
        };
        StatusCtrl.prototype.getValues = function () {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                defaultValue: this.defaultValue,
                defaultSearchValue: this.defaultSearchValue,
                required: this.required,
                readOnly: this.readOnly,
                searchable: this.searchable,
                statusValue: this.statusValue,
                statusType: this.statusType,
                hiddenField: this.hiddenField,
                modifyEnable: this.modifyEnable,
                requiredEnabled: this.requiredEnabled,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                columns: this.columns,
                rows: this.rows
            };
        };
        StatusCtrl.prototype.addNewOption = function (e, scope) {
            var elements = scope.ctrl.getValues();
            if (elements.statusValue == undefined) {
                scope.ctrl.statusValue = "";
            }
            var value = $("#txtAddElement").val();
            if (value === "") {
                alert("Impossibile aggiungere un valore vuoto");
                return;
            }
            if (scope.ctrl.statusType.filter(function (x) {
                return x.value === value;
            })[0] !== undefined) {
                alert("E' già presente un elemento con lo stesso nome.");
                return;
            }
            scope.ctrl.statusValue = value;
            var status = new StatusType();
            status.Value = scope.ctrl.statusValue;
            scope.ctrl.statusType.push(status);
            $("#txtAddElement").val("");
            scope.ctrl.binder.bind($("#StatusOption")[0], scope.ctrl);
        };
        StatusCtrl.prototype.selectedItemChanged = function (e, scope) {
            var elements = scope.ctrl.getValues();
            scope.ctrl.defaultValue = e.target.innerText;
            scope.ctrl.binder.bind($("#StatusOption")[0], scope.ctrl);
        };
        StatusCtrl.prototype.deleteItem = function (e, scope) {
            scope.ctrl.statusType.splice(scope.index, 1);
        };
        StatusCtrl.prototype.selectedValore = function (e, scope) {
            scope.ctrl.openStatusDetailsModal(e, scope);
        };
        StatusCtrl.prototype.openStatusDetailsModal = function (e, scope) {
            this.statusValue = e.currentTarget.innerHTML;
            //this.bind($("#StatusOption")[0]);
            var containerDialog = $("#configuration_modal");
            var context = this;
            UdsDesigner.Service.loadControls("statusDetailsModal", function (data) {
                containerDialog.html(data);
                var content = containerDialog.find('#configuration_body_modal');
                containerDialog.on('show.bs.modal', function () {
                    content.css('overflow-y', 'auto');
                    content.css('max-height', $(window).height() * 0.6);
                });
                containerDialog.modal('show');
                var status = scope.ctrl.statusType.filter(function (x) {
                    return x.Value === context.statusValue;
                })[0];
                $("#txtIconPath").val(status.IconPath);
                $("#txtMappingTag").val(status.MappingTag);
                $("#txtTag").val(status.TagValue);
                $("#txtValore").val(e.currentTarget.innerHTML);
                $("#status_save").click(function () {
                    if (scope.ctrl.statusType.IconPath === undefined)
                        scope.ctrl.statusType.IconPath = "";
                    if ($("#txtIconPath").val() === "") {
                        alert("Impossibile aggiungere un valore vuoto");
                    }
                    else {
                        var status_1 = scope.ctrl.statusType.filter(function (x) {
                            return x.Value === context.statusValue;
                        })[0];
                        status_1.IconPath = $("#txtIconPath").val();
                        status_1.MappingTag = $("#txtMappingTag").val();
                        status_1.TagValue = $("#txtTag").val();
                        context.statusValue = $("#txtValore").val();
                        for (var i = 0; i < scope.ctrl.statusType.length; i++) {
                            if (scope.ctrl.statusType[i].Value === e.currentTarget.innerHTML) {
                                scope.ctrl.statusType[i].Value = context.statusValue;
                            }
                        }
                        e.currentTarget.innerHTML = context.statusValue;
                        containerDialog.modal('hide');
                    }
                });
            });
        };
        StatusCtrl.prototype.changeRequired = function (e, scope) {
            $(".element").each(function (index, item) {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof StatusCtrl) {
                    var ctrlTitle = ctrl;
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        };
        return StatusCtrl;
    }(CtrlBase));
    UdsDesigner.StatusCtrl = StatusCtrl;
})(UdsDesigner || (UdsDesigner = {}));
//# sourceMappingURL=controls.js.map