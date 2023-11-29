/// <reference path="Service.ts" />
/// <reference path="../App/ViewModels/CategoryViewModel.ts" />

module UdsDesigner {

    declare var rivets: any;

    export class CtrlBase {
        binder: any;
        ctrlType: string;
        initCallbacks: Function[];
        modifyEnable: boolean;
        columns: number;
        rows: number;
        format: string;

        constructor() {
            this.initCallbacks = new Array<Function>();
            this.modifyEnable = true;
        }

        static getElementCtrl(element: Element): CtrlBase {
            return jQuery.data(element, "ctrlData");
        }

        bind(element: Element) {
            this.unbind();

            jQuery.data(element, "ctrlData", this);
            this.binder = rivets.bind(element, { ctrl: this });
        }

        unbind() {
            if (this.binder != null) {
                this.binder.unbind();
                this.binder = null;
            }
        }

        clone() {
            return jQuery.extend(true, {}, this);
        }

        setValues(obj: CtrlBase) {
            this.modifyEnable = obj.modifyEnable;
        }

        getValues(): any {
            return {
                modifyEnable: this.modifyEnable
            }
        }

        afterAppend(): void {
            //Da implementare nelle classi che estendono CtrlBase, chiamato all'aggiunta di un pannello
        }

        beforeRemoval(): void {
            //Da implementare nelle classi che estendono CtrlBase, chiamato alla rimozione di un pannello
        }

        onConfirm(): void {
            //Da implementare nelle classi che estendono CtrlBase, chiamato al click di conferma in un pannello
        }
    }


    export class TitleCtrl extends CtrlBase {
        label: string;
        titleReadOnly: boolean;
        subject: string;
        enabledWorkflow: boolean;
        enabledProtocol: boolean;
        enabledPEC: boolean;
        enabledPECButton: boolean;
        enabledMailButton: boolean;
        enabledMailRoleButton: boolean;
        enabledLinkButton: boolean;
        visibleMailButton: boolean;
        enableManualSignature: boolean;
        enabledCancelMotivation: boolean;
        enabledCQRSSync: boolean;
        enabledConservation: boolean;
        idRepository: any;
        DSWEnvironment: Number;
        idCategory: string;
        categoryReadOnly: boolean;
        categoryName: string;
        categorySearchable: boolean;
        categoryDefaultEnabled: boolean;
        idContainer: string;
        containerName: string;
        containerReadOnly: boolean;
        containerSearchable: boolean;
        createContainer: boolean;
        alias: string;
        incrementalIdentityEnabled: boolean;
        signatureMetadataEnabled: boolean;
        enableWorkflowCheckBox: boolean;
        enableConservationCheckBox: boolean;
        subjectResultVisibility: boolean;
        categoryResultVisibility: boolean;
        hideRegistrationIdentifier: boolean;
        stampaConformeEnabled: boolean;
        showArchiveInProtocolSummaryEnabled: boolean;
        requiredRevisionUDSRepository: boolean;
        modifyEnable: boolean;
        showLastChangedDate: boolean;
        showLastChangedUser: boolean;
        selectedItem: number = 1;
        documentTypeCoherencyInArchivingCollaborationDisabled: boolean;

        constructor() {
            super();
            this.ctrlType = "Title";
            this.enabledWorkflow = false;
            this.enabledProtocol = false;
            this.enabledPEC = false;
            this.enabledPECButton = false;
            this.enabledMailButton = false;
            this.enabledMailRoleButton = false;
            this.enabledLinkButton = false;
            this.visibleMailButton = false;
            this.enabledCQRSSync = true;
            this.enabledConservation = false;
            this.categorySearchable = true;
            this.categoryReadOnly = false;
            this.categoryDefaultEnabled = true;
            this.titleReadOnly = false;
            this.containerReadOnly = true;
            this.containerSearchable = true;
            this.createContainer = true;
            this.enabledCancelMotivation = true;
            this.alias = "";
            this.label = "";
            this.incrementalIdentityEnabled = true;
            this.signatureMetadataEnabled = false;
            this.enableManualSignature = false;
            this.idContainer = "";
            this.enableWorkflowCheckBox = !($("#workflowManager")[0].getAttribute("value") == 'True');
            this.enableConservationCheckBox = !($("#conservationManager")[0].getAttribute("value") == 'True');
            this.modifyEnable = true;
            this.showLastChangedDate = false;
            this.showLastChangedUser = false;
            this.subjectResultVisibility = true;
            this.categoryResultVisibility = true;
            this.hideRegistrationIdentifier = false;
            this.stampaConformeEnabled = true;
            this.showArchiveInProtocolSummaryEnabled = true;
            this.requiredRevisionUDSRepository = false;
            this.documentTypeCoherencyInArchivingCollaborationDisabled = false;

            this.initializeEvents();
        }

        setValues(obj: TitleCtrl) {
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
            this.enabledConservation = obj.enabledConservation;
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
            this.stampaConformeEnabled = obj.stampaConformeEnabled;
            this.showArchiveInProtocolSummaryEnabled = obj.showArchiveInProtocolSummaryEnabled;
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
            this.enableConservationCheckBox = obj.enableConservationCheckBox;
            this.subjectResultVisibility = obj.subjectResultVisibility;
            this.categoryResultVisibility = obj.categoryResultVisibility;
            this.modifyEnable = obj.modifyEnable;
            this.showLastChangedDate = obj.showLastChangedDate;
            this.showLastChangedUser = obj.showLastChangedUser;
            this.documentTypeCoherencyInArchivingCollaborationDisabled = obj.documentTypeCoherencyInArchivingCollaborationDisabled;
        }

        getValues(): any {
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
                enabledConservation: this.enabledConservation,
                enabledCancelMotivation: this.enabledCancelMotivation,
                enableAutomaticSignature: this.enableManualSignature,
                enableWorkflowCheckBox: this.enableWorkflowCheckBox,
                enableConservationCheckBox: this.enableConservationCheckBox,
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
                hideRegistrationIdentifier: this.hideRegistrationIdentifier,
                stampaConformeEnabled: this.stampaConformeEnabled,
                showArchiveInProtocolSummaryEnabled: this.showArchiveInProtocolSummaryEnabled,
                requiredRevisionUDSRepository: this.requiredRevisionUDSRepository,
                subjectResultVisibility: this.subjectResultVisibility,
                categoryResultVisibility: this.categoryResultVisibility,
                modifyEnable: this.modifyEnable,
                showLastChangedDate: this.showLastChangedDate,
                showLastChangedUser: this.showLastChangedUser,
                documentTypeCoherencyInArchivingCollaborationDisabled: this.documentTypeCoherencyInArchivingCollaborationDisabled
            }
        }

        selectedItemChanged(e: any, scope: any): any {
            var columns = $("#itemChanger").val();
            scope.ctrl.binder.bind($("#TitleOption")[0], scope.ctrl);
            scope.ctrl.selectedItem = columns
        }

        selectCategory(e, scope): void {
            var categoryModel = new UdsDesigner.CategoryViewModel();
            scope.ctrl.openConfigurationModal(categoryModel);
        }

        selectContainer(e, scope): void {
            var containerModel = new UdsDesigner.ContainerViewModel();
            scope.ctrl.openConfigurationModal(containerModel);
        }

        private openConfigurationModal(model: UdsDesigner.BaseTreeViewModel) {
            this.bind($("#TitleOption")[0]);
            var containerDialog = $("#configuration_modal");
            var context = this;
            UdsDesigner.Service.loadControls("configurationModal", function (data) {
                containerDialog.html(data);
                var content: JQuery = containerDialog.find('#configuration_body_modal');
                model.setup();
                containerDialog.on('show.bs.modal', function () {
                    content.css('overflow-y', 'auto');
                    content.css('max-height', $(window).height() * 0.6);
                });
                containerDialog.modal('show');
            });
        }

        private closeConfigurationModal(): void {
            var categoryDialog = $("#configuration_modal");
            categoryDialog.modal('hide');
        }

        private initializeEvents(): void {
            var context = this;
            $(document).on("categorySelected", function (event, idCategory, categoryName) {
                var el: any = CtrlBase.getElementCtrl($("#TitleOption")[0]);
                el.idCategory = idCategory;
                el.categoryName = categoryName;
                el.bind($("#TitleOption")[0]);
                context.closeConfigurationModal();
            });

            $(document).on("containerSelected", function (event, idContainer, containerName) {
                var el: any = CtrlBase.getElementCtrl($("#TitleOption")[0]);
                el.idContainer = idContainer;
                el.containerName = containerName;
                el.bind($("#TitleOption")[0]);
                context.closeConfigurationModal();
            });
        }

    }

    export class HeaderCtrl extends CtrlBase {
        label: string;
        columns: number;
        rows: number;

        constructor() {
            super();
            this.ctrlType = "Header";
            this.label = "Sezione";
        }

        setValues(obj: HeaderCtrl) {
            this.label = obj.label;
            this.columns = obj.columns;
            this.rows = obj.rows;
        }

        getValues(): any {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                columns: this.columns,
                rows: this.rows
            }
        }
    }

    export class TextCtrl extends CtrlBase {
        public static CUSTOM_ACTION_AD_KEY: string = "Leggi “valore da chiave” dell’utente corrente da Active Directory";

        label: string;
        multiLine: boolean;
        HTMLEnable: boolean;
        defaultValue: string;
        defaultSearchValue: string;
        required: boolean;
        readOnly: boolean;
        searchable: boolean;
        isSignature: boolean;
        hiddenField: boolean;
        requiredEnabled: boolean;
        customActions: string[];
        customActionSelected: string;
        customActionKey: string;
        resultVisibility: boolean;
        resultPosition: number;
        columns: number;
        rows: number;

        constructor() {
            super();

            this.ctrlType = "Text";
            this.label = "Testo";
            this.multiLine = false;
            this.HTMLEnable = false;
            this.defaultValue = "";
            this.defaultSearchValue = "";
            this.required = false;
            this.readOnly = false;
            this.searchable = true;
            this.isSignature = false;
            this.hiddenField = false;
            this.requiredEnabled = false;
            this.customActions = [];
            this.customActionKey = "";
            this.resultVisibility = false;
            this.resultPosition = 0;
        }


        setValues(obj: TextCtrl) {
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
            this.customActions = obj.customActions;
            this.customActionSelected = obj.customActionSelected;
            this.customActionKey = obj.customActionKey;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
        }

        getValues(): any {
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
                customActions: this.customActions,
                customActionSelected: this.customActionSelected,
                customActionKey: this.customActionKey,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                columns: this.columns,
                rows: this.rows
            }
        }

        beforeRemoval(): void {
            $(".element-Title").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.ctrlType == "Title") {
                    var ctrlTitle: TitleCtrl = ctrl as TitleCtrl
                    ctrlTitle.signatureMetadataEnabled = true;
                    ctrlTitle.enableManualSignature = false;
                    ctrlTitle.bind($(".element-Title")[index]);
                }
            });
        }

        onConfirm(): void {
            $(".element-Title").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.ctrlType == "Title") {
                    var ctrlTitle: TitleCtrl = ctrl as TitleCtrl
                    ctrlTitle.enableManualSignature = $("#isSignature").is(':checked');
                    ctrlTitle.bind($(".element-Title")[index]);
                }
            });
        }

        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof TextCtrl) {
                    var ctrlTitle: TextCtrl = ctrl as TextCtrl
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
        }

        selectedItemChanged(e: any, scope: any): any {
            scope.ctrl.customActionSelected = e.target.innerText;
            $("#customActionsKeyId").hide();
            $("#defaultValueId").prop("disabled", false);
            scope.ctrl.setCustomActionBehaviour(scope.ctrl.customActionSelected);
        }

        getCustomActions(e: any, scope: any): void {
            scope.ctrl.binder.bind($("#TextOption")[0], scope.ctrl);
            var context = scope;
            UdsDesigner.Service.loadCustomActions(function (data) {
                context.ctrl.customActions = data.d;
                context.ctrl.binder.bind($("#TextOption")[0], context.ctrl);
            });
        }

        setCustomActionBehaviour(selectedCustomAction: string): void {
            switch (selectedCustomAction) {
                case TextCtrl.CUSTOM_ACTION_AD_KEY: {
                    $("#customActionsKeyId").show();
                    $("#defaultValueId").prop("disabled", true);
                    $("#defaultValueId").val("");
                    break;
                }
                default: {
                    $("#customActionsKeyId").hide();
                    $("#defaultValueId").prop("disabled", false);
                    $("#defaultValueId").val("");
                    break;
                }
            }
        }
    }

    export class DateCtrl extends CtrlBase {
        label: string;
        restrictedYear: boolean;
        defaultValue: string;
        defaultSearchValue: string;
        enableDefaultDate: boolean;
        required: boolean;
        readOnly: boolean;
        searchable: boolean;
        hiddenField: boolean;
        requiredEnabled: boolean;
        resultVisibility: boolean;
        resultPosition: number;
        columns: number;
        rows: number;

        constructor() {
            super();

            this.ctrlType = "Date";
            this.label = "Data";
            this.restrictedYear = false;
            this.enableDefaultDate = false;
            this.defaultValue = "";
            this.defaultSearchValue = "";
            this.required = false;
            this.readOnly = false;
            this.searchable = true;
            this.hiddenField = false;
            this.requiredEnabled = false;
            this.resultVisibility = false;
            this.resultPosition = 0;
        }


        setValues(obj: DateCtrl) {
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
        }

        getValues(): any {
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
            }
        }

        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof DateCtrl) {
                    var ctrlTitle: DateCtrl = ctrl as DateCtrl
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        }
    }

    export class NumberCtrl extends CtrlBase {
        label: string;
        defaultValue: string;
        defaultSearchValue: string;
        required: boolean;
        readOnly: boolean;
        searchable: boolean;
        hiddenField: boolean;
        requiredEnabled: boolean;
        resultVisibility: boolean;
        resultPosition: number;
        columns: number;
        rows: number;
        format: string;
        minValue?: number;
        maxValue?: number;

        constructor() {
            super();

            this.ctrlType = "Number";
            this.label = "Numero";
            this.defaultValue = "";
            this.defaultSearchValue = "";
            this.required = false;
            this.readOnly = false;
            this.searchable = true;
            this.hiddenField = false;
            this.requiredEnabled = false;
            this.resultVisibility = false;
            this.resultPosition = 0;
            this.format = "";
            this.minValue = null;
            this.maxValue = null;
        }


        setValues(obj: NumberCtrl) {
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
            this.minValue = obj.minValue;
            this.maxValue = obj.maxValue;
        }

        getValues(): any {
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
                format: this.format,
                minValue: this.minValue,
                maxValue: this.maxValue
            }
        }

        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof NumberCtrl) {
                    var ctrlTitle: NumberCtrl = ctrl as NumberCtrl
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        }
    }

    export class CheckboxCtrl extends CtrlBase {
        label: string;
        defaultValue: string;
        defaultSearchValue: string;
        required: boolean;
        readOnly: boolean;
        searchable: boolean;
        hiddenField: boolean;
        requiredEnabled: boolean;
        columns: number;
        rows: number;

        constructor() {
            super();

            this.ctrlType = "Checkbox";
            this.label = "Checkbox";
            this.defaultValue = "";
            this.defaultSearchValue = "";
            this.required = false;
            this.readOnly = false;
            this.searchable = true;
            this.hiddenField = false;
            this.requiredEnabled = false;
        }


        setValues(obj: CheckboxCtrl) {
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
        }

        getValues(): any {
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
            }
        }

        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof CheckboxCtrl) {
                    var ctrlTitle: CheckboxCtrl = ctrl as CheckboxCtrl
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        }
    }

    export class LookupCtrl extends CtrlBase {
        label: string;
        required: boolean;
        searchable: boolean;
        hiddenField: boolean;
        requiredEnabled: boolean;
        repositories: string[];
        fields: string[];
        isLoadingRepositories: boolean;
        isLoadingFields: boolean;
        lookupRepositoryName: string;
        lookupFieldName: string;
        fieldsReadOnly: boolean;
        resultVisibility: boolean;
        resultPosition: number;
        multipleValues: boolean;
        multipleEnabled: boolean;
        columns: number;
        rows: number;

        constructor() {
            super();

            this.ctrlType = "Lookup";
            this.label = "Lookup";
            this.lookupRepositoryName = "Seleziona archivio ";
            this.lookupFieldName = "Seleziona proprietà "
            this.required = false;
            this.searchable = true;
            this.hiddenField = false;
            this.requiredEnabled = false;
            this.repositories = [];
            this.fields = [];
            this.isLoadingRepositories = false;
            this.isLoadingFields = false;
            this.fieldsReadOnly = true;
            this.resultVisibility = false;
            this.resultPosition = 0;
            this.multipleValues = false;
            this.multipleEnabled = true;
        }


        setValues(obj: LookupCtrl) {
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
        }

        getValues(): any {
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
            }
        }

        getHiddenValues(): any {
            return {
                repositories: this.repositories,
                isLoadingRepositories: this.isLoadingRepositories,
                fields: this.fields,
                isLoadingFields: this.isLoadingFields
            }
        }

        getRepositories(e: any, scope: any): void {
            var elements: LookupCtrl = scope.ctrl.getHiddenValues();
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
        }

        selectedRepositoryItemChanged(e: any, scope: any): any {
            scope.ctrl.lookupRepositoryName = e.target.innerText;
            scope.ctrl.lookupFieldName = "Seleziona proprietà ";
            scope.ctrl.fields = null;
            scope.ctrl.fieldsReadOnly = false;
            scope.ctrl.binder.bind($("#LookupOption")[0], scope.ctrl);
        }

        selectedFieldItemChanged(e: any, scope: any): any {
            scope.ctrl.lookupFieldName = e.target.innerText;
            scope.ctrl.binder.bind($("#LookupOption")[0], scope.ctrl);
        }

        getFields(e: any, scope: any): void {
            var elements: LookupCtrl = scope.ctrl.getHiddenValues();
            if (elements.fields && elements.fields.length > 0) {
                return;
            }

            scope.ctrl.isLoadingFields = true;
            scope.ctrl.binder.bind($("#LookupOption")[0], scope.ctrl);

            var context = scope;
            UdsDesigner.Service.loadUDSFields(function (data) {
                if (data && data.d && data.d[0]) {
                    context.ctrl.fields = data.d[0];
                    var values: string[] = data.d[0];
                    values.push("Oggetto");
                    context.ctrl.fields = values;
                    context.ctrl.isLoadingFields = false;
                    context.ctrl.binder.bind($("#LookupOption")[0], context.ctrl);
                }
            });
        }

        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof LookupCtrl) {
                    var ctrlTitle: LookupCtrl = ctrl as LookupCtrl
                    ctrlTitle.requiredEnabled = $("#hiddenField").is(':checked');
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        }
    }

    export class DocumentCtrl extends CtrlBase {
        label: string;
        archive: string;
        archiveReadOnly: boolean;
        archives: string[];
        collectionType: string;
        readOnly: boolean;
        required: boolean;
        searchable: boolean;

        enableMultifile: boolean;
        enableUpload: boolean;
        enableScanner: boolean;
        enableSign: boolean;
        copyProtocol: boolean;
        copyResolution: boolean;
        copySeries: boolean;
        copyUDS: boolean;
        isLoadingArchives: boolean;
        signRequired: boolean;
        createBiblosArchive: boolean;
        documentDeletable: boolean;
        columns: number;
        rows: number;

        constructor() {
            super();

            this.ctrlType = "Document";
            this.label = "Documenti";
            this.archive = "Archivio";
            this.collectionType = "*";
            this.readOnly = false;
            this.searchable = true;
            this.required = true;

            this.enableMultifile = false;
            this.enableUpload = true;
            this.enableScanner = true;
            this.enableSign = true;
            this.copyProtocol = false;
            this.copyResolution = false;
            this.copySeries = false;
            this.copyUDS = false;
            this.archives = [];
            this.isLoadingArchives = false;
            this.archiveReadOnly = false;
            this.signRequired = false;
            this.createBiblosArchive = true;
            this.documentDeletable = false;
        }


        setValues(obj: DocumentCtrl) {
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
            this.copyUDS = obj.copyUDS;
            this.archiveReadOnly = obj.archiveReadOnly;
            this.signRequired = obj.signRequired;
            this.createBiblosArchive = obj.createBiblosArchive;
            this.required = obj.required;
            this.documentDeletable = obj.documentDeletable;
            this.modifyEnable = obj.modifyEnable;
            this.columns = obj.columns;
            this.rows = obj.rows;
        }

        getValues(): any {
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
                copyUDS: this.copyUDS,
                archiveReadOnly: this.archiveReadOnly,
                signRequired: this.signRequired,
                createBiblosArchive: this.createBiblosArchive,
                documentDeletable: this.documentDeletable,
                modifyEnable: this.modifyEnable,
                columns: this.columns,
                rows: this.rows
            }
        }

        getHiddenValues(): any {
            return {
                archives: this.archives,
                isLoadingArchives: this.isLoadingArchives
            }
        }

        getBiblosArchives(e: any, scope: any): void {
            var elements: DocumentCtrl = scope.ctrl.getHiddenValues();
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
        }

        selectedItemChanged(e: any, scope: any): any {
            var elements: EnumCtrl = scope.ctrl.getValues();
            scope.ctrl.archive = e.target.innerText;
            scope.ctrl.binder.bind($("#DocumentOption")[0], scope.ctrl);
        }
    }

    export class ContactCtrl extends CtrlBase {
        label: string;

        enableAD: boolean;
        enableAddressBook: boolean;
        enableADDistribution: boolean;
        enableManual: boolean;
        enableExcelImport: boolean;
        enableMultiContact: boolean;
        contactTypeSelected: string;
        contactTypes: string[];
        isProtocollable: boolean;
        required: boolean;
        searchable: boolean;
        resultVisibility: boolean;
        resultPosition: number;
        columns: number;
        rows: number;

        constructor() {
            super();
            this.ctrlType = "Contact";
            this.label = "Contatti";
            this.enableAD = true;
            this.enableAddressBook = true;
            this.enableADDistribution = false;
            this.enableManual = true;
            this.enableExcelImport = false;
            this.enableMultiContact = true;
            this.isProtocollable = false;
            this.required = false;
            this.searchable = true;
            this.resultVisibility = false;
            this.resultPosition = 0;
            this.initCallbacks.push(this.initialize);
        }

        initialize(context: ContactCtrl): void {
            var titleData: any = CtrlBase.getElementCtrl($(".element-Title")[0]).getValues();
            context.isProtocollable = false;
            if (titleData) {
                context.isProtocollable = titleData.enabledProtocol;
            }
        }

        setValues(obj: ContactCtrl) {
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
        }

        getValues(): any {
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
            }
        }

        selectedItemChanged(e: any, scope: any): any {
            var elements: ContactCtrl = scope.ctrl.getValues();
            scope.ctrl.contactTypeSelected = e.target.innerText;
            scope.ctrl.binder.bind($("#ContactOption")[0], scope.ctrl);
        }


        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof ContactCtrl) {
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                }
            });
        }

        getContactTypes(e: any, scope: any): void {
            scope.ctrl.binder.bind($("#ContactOption")[0], scope.ctrl);
            var context = scope;
            UdsDesigner.Service.loadContactTypes(function (data) {
                context.ctrl.contactTypes = data.d;
                context.ctrl.binder.bind($("#ContactOption")[0], context.ctrl);
            });
        }


    }

    export class AuthorizationCtrl extends CtrlBase {
        label: string;
        enableMultiAuth: boolean;
        allowMultiUserAuthorization: boolean;
        userAuthorizationEnabled: boolean;
        required: boolean;
        searchable: boolean;
        resultVisibility: boolean;
        myAuthorizedRolesEnabled: boolean;
        resultPosition: number;
        columns: number;
        rows: number;
        constructor() {
            super();

            this.ctrlType = "Authorization";
            this.label = "Autorizzazione";
            this.enableMultiAuth = true;
            this.allowMultiUserAuthorization = false;
            this.userAuthorizationEnabled = false;
            this.required = false;
            this.searchable = true;
            this.myAuthorizedRolesEnabled = false;
            this.resultVisibility = false;
            this.resultPosition = 0;
        }

        setValues(obj: AuthorizationCtrl) {
            this.label = obj.label;
            this.enableMultiAuth = obj.enableMultiAuth;
            this.allowMultiUserAuthorization = obj.allowMultiUserAuthorization;
            this.userAuthorizationEnabled = obj.userAuthorizationEnabled;
            this.required = obj.required;
            this.searchable = obj.searchable;
            this.modifyEnable = obj.modifyEnable;
            this.myAuthorizedRolesEnabled = obj.myAuthorizedRolesEnabled;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
        }




        getValues(): any {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                enableMultiAuth: this.enableMultiAuth,
                allowMultiUserAuthorization: this.allowMultiUserAuthorization,
                userAuthorizationEnabled: this.userAuthorizationEnabled,
                required: this.required,
                searchable: this.searchable,
                modifyEnable: this.modifyEnable,
                myAuthorizedRolesEnabled: this.myAuthorizedRolesEnabled,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                rows: this.rows,
                columns: this.columns
            }
        }

        afterAppend(): void {
            $(".element-Title").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.ctrlType == "Title") {
                    var ctrlTitle: TitleCtrl = ctrl as TitleCtrl
                    ctrlTitle.visibleMailButton = true;
                    ctrlTitle.bind($(".element-Title")[index]);
                }
            });
        }

        beforeRemoval(): void {
            $(".element-Title").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.ctrlType == "Title") {
                    var ctrlTitle: TitleCtrl = ctrl as TitleCtrl
                    ctrlTitle.visibleMailButton = false;
                    ctrlTitle.enabledMailRoleButton = false;
                    ctrlTitle.bind($(".element-Title")[index]);
                }
            });
        }

        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof AuthorizationCtrl) {
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                }
            });
        }
    }

    export class EnumCtrl extends CtrlBase {
        label: string;
        defaultValue: string;
        defaultSearchValue: string;
        required: boolean;
        readOnly: boolean;
        searchable: boolean;
        enumOptions: string[];
        hiddenField: boolean;
        requiredEnabled: boolean;
        resultVisibility: boolean;
        resultPosition: number;
        columns: number;
        rows: number;
        multipleValues: boolean;
        multipleEnabled: boolean;

        enumValue: string;

        constructor() {
            super();

            this.ctrlType = "Enum";
            this.label = "Scelta multipla";
            this.defaultValue = "";
            this.defaultSearchValue = "";
            this.readOnly = false;
            this.required = false;
            this.searchable = true;
            this.enumOptions = [];
            this.hiddenField = false;
            this.requiredEnabled = false;
            this.resultVisibility = false;
            this.resultPosition = 0;
            this.multipleValues = false;
            this.multipleEnabled = true;
            $("#deleteSelectedItem").hide();
        }

        setValues(obj: EnumCtrl) {
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
        }

        getValues(): any {
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
            }
        }


        deleteItem(e, scope): void {
            if (scope.ctrl.defaultValue === scope.ctrl.enumOptions[scope.index]) {
                scope.ctrl.defaultValue = "";
                $("#deleteSelectedItem").hide();
            }
            scope.ctrl.enumOptions.splice(scope.index, 1);
        }

        deleteSelectedItem(e, scope): void {
            scope.ctrl.defaultValue = "";
            $("#deleteSelectedItem").hide();
        }

        selectedValue(e, scope): void {
            scope.ctrl.OpenEnumDetailsModal(e, scope);
        }

        private OpenEnumDetailsModal(e, scope) {
            this.enumValue = e.currentTarget.innerHTML;
            console.log(scope);
            var containerDialog = $("#configuration_modal");

            var context = this;

            UdsDesigner.Service.loadControls("enumDetailsModal", function (data) {
                containerDialog.html(data);
                var content: JQuery = containerDialog.find('#configuration_body_modal');
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
                        for (let i = 0; i < scope.ctrl.enumOptions.length; i++) {
                            if (scope.ctrl.enumOptions[i] === e.currentTarget.innerHTML) {
                                scope.ctrl.enumOptions[i] = context.enumValue;
                            }
                        }
                        e.currentTarget.innerHTML = context.enumValue;
                        containerDialog.modal('hide');

                    }
                });
            });
        }

        addNewOption(e: any, scope: any): any {
            var elements: EnumCtrl = scope.ctrl.getValues();
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
        }

        selectedItemChanged(e: any, scope: any): any {
            var elements: EnumCtrl = scope.ctrl.getValues();
            scope.ctrl.defaultValue = e.target.innerText;
            scope.ctrl.binder.bind($("#EnumOption")[0], scope.ctrl);
            if (scope.ctrl.defaultValue !== "") {
                $("#deleteSelectedItem").show();
            }
        }

        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof EnumCtrl) {
                    var ctrlTitle: EnumCtrl = ctrl as EnumCtrl
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        }

    }

    export class StatusType {
        IconPath: string;
        MappingTag: string;
        TagValue: string;
        Value: string;
    }

    export class StatusCtrl extends CtrlBase {
        label: string;
        defaultValue: string;
        defaultSearchValue: string;
        required: boolean;
        readOnly: boolean;
        searchable: boolean;
        statusValue: string;
        statusType: StatusType[];
        hiddenField: boolean;
        requiredEnabled: boolean;
        resultVisibility: boolean;
        resultPosition: number;
        columns: number;
        rows: number;

        constructor() {
            super();

            this.ctrlType = "Status";
            this.label = "Stato";
            this.defaultValue = "";
            this.defaultSearchValue = "";
            this.readOnly = false;
            this.required = false;
            this.searchable = true;
            this.statusValue = "";
            this.statusType = [];
            this.hiddenField = false;
            this.requiredEnabled = false;
            this.resultVisibility = false;
            this.resultPosition = 0;
        }

        setValues(obj: StatusCtrl) {
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
        }

        getValues(): any {
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
            }
        }

        addNewOption(e: any, scope: any): any {
            var elements: StatusCtrl = scope.ctrl.getValues();
            if (elements.statusValue == undefined) {
                scope.ctrl.statusValue = "";
            }

            var value = $("#txtAddElement").val();

            if (value === "") {
                alert("Impossibile aggiungere un valore vuoto");
                return;
            }

            if (scope.ctrl.statusType.filter(function (x) {
                return x.value === value
            })[0] !== undefined) {
                alert("E' già presente un elemento con lo stesso nome.");
                return;
            }

            scope.ctrl.statusValue = value;
            let status: StatusType = new StatusType();
            status.Value = scope.ctrl.statusValue;
            scope.ctrl.statusType.push(status);

            $("#txtAddElement").val("");
            scope.ctrl.binder.bind($("#StatusOption")[0], scope.ctrl);
        }

        selectedItemChanged(e: any, scope: any): any {
            var elements: StatusCtrl = scope.ctrl.getValues();
            scope.ctrl.defaultValue = e.target.innerText;
            scope.ctrl.binder.bind($("#StatusOption")[0], scope.ctrl);
            if (scope.ctrl.defaultValue !== "") {
                $("#deleteSelectedItem").show();
            }
        }

        deleteItem(e, scope): void {
            if (scope.ctrl.defaultValue === scope.ctrl.statusValue) {
                scope.ctrl.defaultValue = "";
                $("#deleteSelectedItem").hide();
            }
            scope.ctrl.statusType.splice(scope.index, 1);
        }

        deleteSelectedItem(e, scope): void {
            scope.ctrl.defaultValue = "";
            $("#deleteSelectedItem").hide();
        }

        selectedValore(e, scope): void {
            scope.ctrl.openStatusDetailsModal(e, scope);
        }

        private openStatusDetailsModal(e, scope) {
            this.statusValue = e.currentTarget.innerHTML;

            //this.bind($("#StatusOption")[0]);

            var containerDialog = $("#configuration_modal");

            var context = this;
            UdsDesigner.Service.loadControls("statusDetailsModal", function (data) {
                containerDialog.html(data);
                var content: JQuery = containerDialog.find('#configuration_body_modal');
                containerDialog.on('show.bs.modal', function () {
                    content.css('overflow-y', 'auto');
                    content.css('max-height', $(window).height() * 0.6);

                });

                containerDialog.modal('show');
                let status: StatusType = scope.ctrl.statusType.filter(function (x) {
                    return x.Value === context.statusValue
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
                        let status: StatusType = scope.ctrl.statusType.filter(function (x) {
                            return x.Value === context.statusValue
                        })[0];
                        status.IconPath = $("#txtIconPath").val();
                        status.MappingTag = $("#txtMappingTag").val();
                        status.TagValue = $("#txtTag").val();

                        context.statusValue = $("#txtValore").val();

                        for (let i = 0; i < scope.ctrl.statusType.length; i++) {
                            if (scope.ctrl.statusType[i].Value === e.currentTarget.innerHTML) {
                                scope.ctrl.statusType[i].Value = context.statusValue;
                            }
                        }
                        e.currentTarget.innerHTML = context.statusValue;

                        containerDialog.modal('hide');
                    }
                });
            });
        }

        changeRequired(e, scope): void {
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl instanceof StatusCtrl) {
                    var ctrlTitle: StatusCtrl = ctrl as StatusCtrl
                    ctrlTitle.requiredEnabled = ($("#readOnly").is(':checked') || $("#hiddenField").is(':checked'));
                    $("#required").prop("disabled", ctrlTitle.requiredEnabled);
                    if (!$("#resultVisibility").is(':checked')) {
                        $("#resultPosition").val(0);
                    }
                    ctrlTitle.bind($(".element")[index]);
                }
            });
        }
    }

    export class TreeListField {
        DefaultValue: string;
        Value: string;
    }

    export class TreeListCtrl extends CtrlBase {
        label: string;
        required: boolean;
        readOnly: boolean;
        searchable: boolean;
        treeList: TreeListField[];
        hiddenField: boolean;
        requiredEnabled: boolean;
        resultVisibility: boolean;
        resultPosition: number;
        columns: number;
        rows: number;
        udsFieldListRoot: any;
        nodeToDelete: any;
        idUDSRepository: string;
        treeListName: string;
        odataUrl: string;
        restUrl: string;
        canRemoveTreeList: boolean;

        constructor() {
            super();

            this.ctrlType = "TreeList";
            this.label = "Elenco dei campi";
            this.readOnly = false;
            this.required = false;
            this.searchable = true;
            this.treeList = [];
            this.hiddenField = false;
            this.requiredEnabled = false;
            this.resultVisibility = false;
            this.resultPosition = 0;
        }

        setValues(obj: TreeListCtrl) {
            this.label = obj.label;
            this.readOnly = obj.readOnly;
            this.required = obj.required;
            this.searchable = obj.searchable;
            this.treeList = obj.treeList;
            this.hiddenField = obj.hiddenField;
            this.modifyEnable = obj.modifyEnable;
            this.requiredEnabled = obj.requiredEnabled;
            this.resultVisibility = obj.resultVisibility;
            this.resultPosition = obj.resultPosition;
            this.columns = obj.columns;
            this.rows = obj.rows;
        }

        getValues(): any {
            return {
                ctrlType: this.ctrlType,
                label: this.label,
                required: this.required,
                readOnly: this.readOnly,
                searchable: this.searchable,
                treeList: this.treeList,
                hiddenField: this.hiddenField,
                modifyEnable: this.modifyEnable,
                requiredEnabled: this.requiredEnabled,
                resultVisibility: this.resultVisibility,
                resultPosition: this.resultPosition,
                columns: this.columns,
                rows: this.rows
            }
        }

        loadTreeList(odataUrl: string, restUrl: string): void {
            let thisObj = this;
            thisObj.odataUrl = odataUrl;
            thisObj.restUrl = restUrl;
            var treeList = $("#treelist").kendoTreeList({
                dataSource: new kendo.data.TreeListDataSource({
                    transport: {
                        read: {
                            url: `${thisObj.odataUrl}/UDSFieldLists/UDSFieldListService.GetChildrenByParent(parentId=${thisObj.udsFieldListRoot.UniqueId})`,
                            dataType: "json"
                        },
                        update: {
                            url: `${thisObj.restUrl}/UDSFieldList`,
                            dataType: "json",
                            type: "PUT",
                            contentType: "application/json; charset=utf-8"
                        },
                        destroy: {
                            url: `${thisObj.restUrl}/UDSFieldList?actionType=128`,
                            dataType: "json",
                            type: "DELETE",
                            contentType: "application/json; charset=utf-8",
                        },
                        create: {
                            url: `${thisObj.restUrl}/UDSFieldList`,
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8"
                        },
                        parameterMap: function (data, type) {
                            if (type === "create" || type === "update") {
                                (<any>data).FieldName = thisObj.udsFieldListRoot.FieldName;
                                if ((<any>data).ParentInsertId === null) {
                                    (<any>data).ParentInsertId = thisObj.udsFieldListRoot.UniqueId;
                                }
                                (<any>data).Repository = {
                                    UniqueId: thisObj.udsFieldListRoot.Repository.UniqueId
                                };
                                (<any>data).Environment = thisObj.udsFieldListRoot.Environment;
                                (<any>data).Status = "Active";
                                return kendo.stringify(data);
                            }
                            else if (type === "destroy") {
                                //Request is sent for every child node
                                //TODO: Find a solution to skip api request when IF condition is true
                                if (!thisObj.nodeToDelete || (<any>data).UniqueId !== thisObj.nodeToDelete.UniqueId) {
                                    (<any>data).Status = "Invalid";
                                }
                                return kendo.stringify(data);
                            }
                        }
                    },
                    schema: {
                        model: {
                            id: "UniqueId",
                            parentId: "ParentInsertId",
                            fields: {
                                UniqueId: { type: "string", nullable: false },
                                ParentInsertId: { field: "ParentInsertId", nullable: true }
                            }
                        },
                        data: function (response) {
                            let result = null;
                            if (response.value) {
                                for (let udsFieldList of response.value) {
                                    if (treeList.dataSource.data().filter(x => (<any>x).UniqueId === udsFieldList.UniqueId).length > 0) {
                                        return;
                                    }
                                }
                                result = (<any[]>response.value).map(x =>
                                    thisObj.createUDSFieldListModel(x, x.ParentInsertId ? x.ParentInsertId : null, x.IdUDSRepository, x.CountChildren > 0)
                                );
                            }
                            else {
                                if (response.ParentInsertId === thisObj.udsFieldListRoot.UniqueId) {
                                    response.ParentInsertId = null;
                                }
                                else if (response.ParentInsertId === null) {
                                    let currentNodeArray = treeList.dataSource.data().filter(x => (<any>x).UniqueId === response.UniqueId);
                                    if (currentNodeArray.length > 0) {
                                        response.ParentInsertId = currentNodeArray[0].ParentInsertId;
                                    }
                                }
                                result = thisObj.createUDSFieldListModel(response, response.ParentInsertId, response.Repository.UniqueId, false);
                            }
                            return result;
                        }
                    }
                }),
                columns: [
                    {
                        field: "Name",
                        title: "Nome",
                        expandable: true
                    },
                    {
                        command: [
                            <kendo.ui.TreeListColumnCommandItem>{
                                name: "edit",
                                text: "Modifica"
                            },
                            <kendo.ui.TreeListColumnCommandItem>{
                                name: "destroy",
                                text: "Rimuovi"
                            },
                            <kendo.ui.TreeListColumnCommandItem>{
                                name: "createchild",
                                text: "Aggiungi"
                            }
                        ]
                    }
                ],
                dataBound: function (e) {

                },
                expand: function (e) {
                    if (!(<any>e.model).UniqueId || (<any>e.model).UniqueId === "") {
                        return;
                    }
                    UdsDesigner.Service.loadUDSFieldListChildren(thisObj.odataUrl, (<any>e.model).UniqueId, (data: any) => {
                        for (let child of data.value) {
                            if (treeList.dataSource.data().filter(x => (<any>x).UniqueId === child.UniqueId).length > 0) {
                                continue;
                            }
                            child.ParentInsertId = (<any>e.model).UniqueId;
                            treeList.dataSource.data().push(thisObj.createUDSFieldListModel(child, child.ParentInsertId, child.IdUDSRepository, child.CountChildren > 0));
                        }
                    });
                },
                remove: function (e: kendo.ui.TreeListRemoveEvent) {
                    thisObj.nodeToDelete = null;
                    if ((<any>e.model).hasChildren) {
                        let canDelete: boolean = confirm("Il nodo che desideri rimuovere ha figli. Vuoi rimuoverlo?");
                        if (canDelete) {
                            thisObj.nodeToDelete = e.model;
                        }
                        else {
                            e.preventDefault();
                        }
                    } else {
                        thisObj.nodeToDelete = e.model;
                    }
                },
                toolbar: [
                    {
                        name: "create",
                        text: "Aggiungi"
                    }
                ],
                editable: true
            }).data("kendoTreeList");

        }

        beforeRemoval(): void {
            if (!this.canRemoveTreeList) {
                return;
            }

            let model: any = {
                UniqueId: this.udsFieldListRoot.UniqueId,
                Name: `${this.treeListName}`,
                Status: "Inactive",
                Environment: this.udsFieldListRoot.Environment,
                Repository: {
                    UniqueId: this.idUDSRepository
                }
            };

            this.removeUDSFieldListCtrlNode(model);
        }

        private createUDSFieldListModel(data: any, parentInsertId: string, idUDSRepository: string, hasChildren: boolean): any {
            return {
                Environment: data.Environment,
                FieldName: data.FieldName,
                Name: data.Name,
                ParentInsertId: parentInsertId,
                Status: data.Status,
                UDSFieldListLevel: data.UDSFieldListLevel,
                UDSFieldListPath: data.UDSFieldListPath,
                UniqueId: data.UniqueId,
                Repository: {
                    UniqueId: idUDSRepository
                },
                hasChildren: hasChildren
            };
        }

        private removeUDSFieldListCtrlNode(model: any): void {
            UdsDesigner.Service.removeUDSFieldListCtrlNode(this.restUrl, model, (data: any) => {

            });
        }
    }
}