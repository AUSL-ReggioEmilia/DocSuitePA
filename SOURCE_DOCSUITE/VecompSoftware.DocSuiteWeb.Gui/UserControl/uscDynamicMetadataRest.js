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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/ViewModels/Metadata/MetadataDesignerViewModel", "App/ViewModels/Metadata/MetadataValueViewModel", "App/ViewModels/Metadata/BaseFieldViewModel", "App/ViewModels/Metadata/EnumFieldViewModel", "App/ViewModels/Metadata/DiscussionFieldViewModel", "App/ViewModels/Metadata/TextFieldViewModel", "App/ViewModels/Metadata/CommentFieldViewModel", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, ExceptionDTO, MetadataDesignerViewModel, MetadataValueViewModel, BaseFieldViewModel, EnumFieldViewModel, DiscussionFieldViewModel, TextFieldViewModel, CommentFieldViewModel, SessionStorageKeysHelper) {
    var uscDynamicMetadataRest = /** @class */ (function (_super) {
        __extends(uscDynamicMetadataRest, _super);
        /**
         * Costruttore
         *  @param serviceConfigurations
         */
        function uscDynamicMetadataRest(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME)) || this;
            _this.controlsCounter = 0;
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /*
         * ---------------------------- Events -------------------------
         */
        /**
         * Inizializzazione
         */
        uscDynamicMetadataRest.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this.bindLoaded();
        };
        /*
         * -------------------------- Methods --------------------------
         */
        /**
         * Eseguo il Bind della pagina
         */
        uscDynamicMetadataRest.prototype.bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        uscDynamicMetadataRest.prototype.getMetadataRepository = function () {
            return this._currentMetadataRepositoryModel;
        };
        /**
         * Carico il modello nello usercontrol
         * @param idMetadataRepository
         */
        uscDynamicMetadataRest.prototype.loadMetadataRepository = function (idMetadataRepository) {
            var _this = this;
            sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY);
            this._service.getFullModelById(idMetadataRepository, function (data) {
                if (data) {
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY, idMetadataRepository);
                    _this._currentMetadataRepositoryModel = data;
                    _this.loadPageItems(data.JsonMetadata, null);
                }
            }, function (exception) {
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (exception && uscNotification && exception instanceof ExceptionDTO) {
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                }
            });
        };
        /**
         * Populato il modello nello usercontrol
         * @param idMetadataRepository
         */
        uscDynamicMetadataRest.prototype.populateMetadataRepository = function (model, jsonMetadata) {
            var metadataViewModel;
            if (jsonMetadata) {
                metadataViewModel = JSON.parse(jsonMetadata);
            }
            else {
                metadataViewModel = JSON.parse(this._currentMetadataRepositoryModel.JsonMetadata);
            }
            if (metadataViewModel.TextFields) {
                $.each(metadataViewModel.TextFields, function (index, textFieldViewModel) {
                    var elm = $("[keyname=" + textFieldViewModel.KeyName + "]");
                    elm.val(model[textFieldViewModel.KeyName]);
                });
            }
            if (metadataViewModel.DateFields) {
                $.each(metadataViewModel.DateFields, function (index, dateFieldViewModel) {
                    var elm = $("[keyname=" + dateFieldViewModel.KeyName + "]");
                    elm.val(model[dateFieldViewModel.KeyName]);
                });
            }
            if (metadataViewModel.NumberFields) {
                $.each(metadataViewModel.NumberFields, function (index, numberFieldViewModel) {
                    var elm = $("[keyname=" + numberFieldViewModel.KeyName + "]");
                    elm.val(model[numberFieldViewModel.KeyName]);
                });
            }
            if (metadataViewModel.BoolFields) {
                $.each(metadataViewModel.BoolFields, function (index, boolFieldViewModel) {
                    var elm = $("[keyname=" + boolFieldViewModel.KeyName + "]");
                    elm[0].checked = model[boolFieldViewModel.KeyName];
                });
            }
            this._discussions = metadataViewModel.DiscussionFields;
            if (metadataViewModel.DiscussionFields) {
                $.each(metadataViewModel.DiscussionFields, function (index, discussionFieldViewModel) {
                    var elm = $("[keyname=" + discussionFieldViewModel.KeyName + "]");
                    elm.val(model[discussionFieldViewModel.KeyName]);
                });
            }
            if (metadataViewModel.EnumFields) {
                $.each(metadataViewModel.EnumFields, function (index, enumFieldViewModel) {
                    var elm = $("[keyname=" + enumFieldViewModel.KeyName + "]");
                    for (var i = 0; i < elm[0].options.length; i++) {
                        if (elm[0].options[i].text === model[enumFieldViewModel.KeyName]) {
                            elm[0].selectedIndex = i;
                            break;
                        }
                    }
                });
            }
        };
        /**
         * Carico gli elementi della pagina
         * @param metadataRepositoryModel
         */
        uscDynamicMetadataRest.prototype.loadPageItems = function (jsonDesignerMetadata, jsonValueMetadataModels) {
            var metadataDesignerModel = JSON.parse(jsonDesignerMetadata);
            var metadataValues = [];
            if (jsonValueMetadataModels) {
                metadataValues = JSON.parse(jsonValueMetadataModels);
            }
            this.clearPage();
            this.arrangeControlsInPosition(metadataDesignerModel, metadataValues);
            this.bindLoaded();
        };
        uscDynamicMetadataRest.prototype.arrangeControlsInPosition = function (metadataDesignerModel, metadataValues) {
            var aggregatedSum = 0;
            for (var arr in metadataDesignerModel) {
                if (typeof (metadataDesignerModel[arr]) !== "boolean") {
                    aggregatedSum += metadataDesignerModel[arr].length;
                }
            }
            this._discussions = metadataDesignerModel.DiscussionFields;
            var _loop_1 = function () {
                var metadataField = null;
                var currentType = void 0;
                for (var arr in metadataDesignerModel) {
                    currentType = arr;
                    var obj = undefined;
                    if (typeof metadataDesignerModel[arr] !== "boolean") {
                        obj = metadataDesignerModel[arr].filter(function (x) { return x.Position == i; })[0];
                    }
                    if (obj) {
                        metadataField = obj;
                        break;
                    }
                }
                if (!metadataField) {
                    return "continue";
                }
                var currentValue = null;
                if (metadataValues && metadataField) {
                    var currentMetadataValue = metadataValues.filter(function (x) { return x.KeyName == metadataField.KeyName; })[0];
                    if (currentMetadataValue) {
                        currentValue = currentMetadataValue.Value;
                    }
                }
                switch (currentType) {
                    case MetadataRepositoryBase.CONTROL_TEXT_FIELD:
                        this_1.fillHTMLGenericElement(this_1.componentTextId, this_1.controlsCounter, metadataField, currentValue, "riTextBox");
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_DATE_FIELD:
                        this_1.fillDateElement(this_1.componentDateId, this_1.controlsCounter, metadataField, currentValue, "riTextBox");
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_NUMBER_FIELD:
                        this_1.fillHTMLGenericElement(this_1.componentNumberId, this_1.controlsCounter, metadataField, currentValue, "riTextBox");
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_BOOL_FIELD:
                        this_1.fillHTMLGenericElement(this_1.componentCheckBoxId, this_1.controlsCounter, metadataField, currentValue, "form-control");
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_ENUM_FIELD:
                        this_1.fillHTMLEnumElement(this_1.componentEnumId, this_1.controlsCounter, metadataField, currentValue, "ddlOptions");
                        this_1.controlsCounter++;
                        break;
                    case MetadataRepositoryBase.CONTROL_DISCUSION_FIELD:
                        this_1.fillHTMLGenericElement(this_1.componentCommentId, this_1.controlsCounter, metadataField, currentValue, "riTextBox");
                        this_1.controlsCounter++;
                        break;
                    default:
                        break;
                }
            };
            var this_1 = this;
            for (var i = 0; i <= aggregatedSum; i++) {
                _loop_1();
            }
        };
        /**
         * Clono un elemento generico e lo completo con i vari attributi
         * @param idComponent
         * @param incrementalIdentifier
         * @param model
         * @param cssClass
         */
        uscDynamicMetadataRest.prototype.fillHTMLGenericElement = function (idComponent, incrementalIdentifier, model, value, cssClass) {
            var idCloned = this.cloneElement(idComponent, incrementalIdentifier);
            var clonedElement = ($("#".concat(idCloned))[0]);
            clonedElement.style.display = "";
            this.prepareLabel(idCloned, 0, model.Label);
            var inputElement = this.findGenericInputControl(idCloned, 0, cssClass);
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
        ;
        /**
         * Clono l'elemento ENUM e lo completo con i vari attributi
         * @param idComponent
         * @param incrementalIdentifier
         * @param model
         * @param cssClass
         */
        uscDynamicMetadataRest.prototype.fillHTMLEnumElement = function (idComponent, incrementalIdentifier, model, value, cssClass) {
            var idCloned = this.cloneElement(idComponent, incrementalIdentifier);
            var clonedElement = ($("#".concat(idCloned))[0]);
            clonedElement.style.display = "";
            this.prepareLabel(idCloned, 0, model.Label);
            var ddlElement = this.findSelectControl(idCloned, 0);
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
                $.each(model.Options, function (innerIndex, v) {
                    var isSelected = (v == value);
                    $("#" + ddlElement.id).append($('<option>', {
                        value: innerIndex + 1,
                        text: v,
                        selected: isSelected
                    }));
                });
            }
        };
        ;
        /**
         * Popolo il campo data
         * @param idComponent
         * @param incrementalIdentifier
         * @param model
         * @param cssClass
         */
        uscDynamicMetadataRest.prototype.fillDateElement = function (idComponent, incrementalIdentifier, model, value, cssClass) {
            var idCloned = this.cloneElement(idComponent, incrementalIdentifier);
            var clonedElement = ($("#".concat(idCloned))[0]);
            clonedElement.style.display = "";
            var inputElement = this.findGenericInputControl(idCloned, 0, cssClass);
            var labelValidator = this.findLabelElement(idCloned, 1);
            this.prepareLabel(idCloned, 0, model.Label);
            this.setDefaultValues(inputElement, model);
            if (value) {
                inputElement.value = value;
            }
            this.setRequiredAttribute(inputElement, model);
            $("#".concat(idCloned, " :input.", cssClass)).on("blur", { idParent: idCloned, class: cssClass }, this.onBlurEvent);
            inputElement.setAttribute(uscDynamicMetadataRest.KEY_NAME, model.KeyName);
        };
        /**
         * evento al onfocusout del campo data
         * @param event
         */
        uscDynamicMetadataRest.prototype.onBlurEvent = function (event) {
            var idParent = event.data.idParent;
            var cssClass = event.data.class;
            var inputElement = ($("#".concat(idParent, " :input.", cssClass))[0]);
            var labelValidator = $("#" + idParent).find('label')[1];
            if (inputElement.value) {
                var dateValue = new Date(inputElement.value);
                if (dateValue && moment(dateValue).isValid()) {
                    labelValidator.style.visibility = "hidden";
                }
                else {
                    labelValidator.style.visibility = "visible";
                }
            }
            return false;
        };
        /**
         * Preparo il label
         * @param idParent
         * @param position
         * @param labelCaption
         */
        uscDynamicMetadataRest.prototype.prepareLabel = function (idParent, position, labelCaption) {
            var labelElement = this.findLabelElement(idParent, position);
            labelElement.textContent = labelCaption.concat(": ");
            labelElement.setAttribute(uscDynamicMetadataRest.FIELD_LABEL, labelCaption);
        };
        /**
         * Imposto alcuni default comuni dei campi di input
         * @param element
         * @param model
         */
        uscDynamicMetadataRest.prototype.setDefaultValues = function (element, model) {
            if (model.DefaultValue) {
                element.setAttribute(uscDynamicMetadataRest.DEFAULT_VALUE, model.DefaultValue);
            }
            if (model.DefaultValue) {
                element.value = model.DefaultValue;
                element.checked = (model.DefaultValue.toLowerCase() == "true");
            }
        };
        /**
         * Imposto l'attributo required
         * @param element
         * @param model
         */
        uscDynamicMetadataRest.prototype.setRequiredAttribute = function (element, model) {
            element.setAttribute(uscDynamicMetadataRest.REQUIRED_FIELD, "false");
            if (model.Required) {
                element.required = true;
                element.setAttribute(uscDynamicMetadataRest.REQUIRED_FIELD, "true");
            }
        };
        /**
         * Recupero i dati dalla pagina
         */
        uscDynamicMetadataRest.prototype.bindModelFormPage = function (setiField) {
            var _this = this;
            var content = $("#menuContent").children();
            if (content.length < 1) {
                return ["", ""];
            }
            var metadataModel = new MetadataDesignerViewModel();
            var baseField;
            var enumField;
            var textField;
            var currentDiscussion;
            var currentValue;
            var currentValues = [];
            var inputElement;
            var labelElement;
            var missingRequiredFields = [];
            var invalidInput = [];
            if (setiField) {
                metadataModel.SETIFieldEnabled = setiField;
            }
            $.each(content, function (index, divElement) {
                labelElement = _this.findLabelElement(divElement.id, 0);
                var dataset = divElement.getAttribute("data-type");
                switch (dataset) {
                    case "Text":
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "riTextBox");
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
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "riTextBox");
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
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "riTextBox");
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
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "form-control");
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
                        var ddlElement = _this.findSelectControl(divElement.id, 0);
                        enumField = new EnumFieldViewModel();
                        enumField.KeyName = ddlElement.attributes[uscDynamicMetadataRest.KEY_NAME].value;
                        enumField.Position = index;
                        if (inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE)) {
                            enumField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataRest.DEFAULT_VALUE);
                        }
                        enumField.Required = (inputElement.getAttribute(uscDynamicMetadataRest.REQUIRED_FIELD) == "true");
                        enumField.Label = labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL);
                        enumField.Options = {};
                        $.each(ddlElement, function (index, option) {
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
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "riTextBox");
                        currentDiscussion = _this._discussions.filter(function (x) { return x.Label == labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL); })[0];
                        if (!currentDiscussion) {
                            currentDiscussion = new DiscussionFieldViewModel();
                            currentDiscussion.Label = labelElement.getAttribute(uscDynamicMetadataRest.FIELD_LABEL);
                        }
                        if (inputElement.value) {
                            var comment = new CommentFieldViewModel();
                            comment.Comment = inputElement.value;
                            comment.Author = _this.currentUser;
                            comment.RegistrationDate = moment().toDate();
                            if (!currentDiscussion.Comments) {
                                currentDiscussion.Comments = new Array();
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
                var message_1 = "";
                $.each(missingRequiredFields, function (index, missingField) {
                    message_1 = message_1.concat(missingField, " \n ");
                });
                alert("Attenzione, i nomi dei seguenti campi sono obbligatori: \n ".concat(message_1));
                return undefined;
            }
            if (invalidInput.length > 0) {
                var message_2 = "";
                $.each(invalidInput, function (index, invalidField) {
                    message_2 = message_2.concat(invalidField, " \n ");
                });
                alert("Attenzione, i seguenti campi sono in formato non valido: \n ".concat(message_2));
                return undefined;
            }
            return [JSON.stringify(metadataModel), JSON.stringify(currentValues)];
        };
        uscDynamicMetadataRest.FIELD_LABEL = "FieldLabel";
        uscDynamicMetadataRest.REQUIRED_FIELD = "RequiredField";
        uscDynamicMetadataRest.DEFAULT_VALUE = "DefaultValue";
        uscDynamicMetadataRest.KEY_NAME = "KeyName";
        return uscDynamicMetadataRest;
    }(MetadataRepositoryBase));
    return uscDynamicMetadataRest;
});
//# sourceMappingURL=uscDynamicMetadataRest.js.map