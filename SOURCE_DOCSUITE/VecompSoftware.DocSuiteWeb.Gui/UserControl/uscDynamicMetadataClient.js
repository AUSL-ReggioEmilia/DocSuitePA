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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "App/Models/Commons/MetadataRepositoryModel", "App/DTOs/ExceptionDTO", "App/ViewModels/Metadata/MetadataViewModel", "App/ViewModels/Metadata/BaseFieldViewModel", "App/ViewModels/Metadata/EnumFieldViewModel", "App/ViewModels/Metadata/DiscussionFieldViewModel", "App/ViewModels/Metadata/TextFieldViewModel", "App/ViewModels/Metadata/CommentFieldViewModel"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, MetadataRepositoryModel, ExceptionDTO, MetadataViewModel, BaseFieldViewModel, EnumFieldViewModel, DiscussionFieldViewModel, TextFieldViewModel, CommentFieldViewModel) {
    var uscDynamicMetadataClient = /** @class */ (function (_super) {
        __extends(uscDynamicMetadataClient, _super);
        /**
         * Costruttore
         *  @param serviceConfigurations
         */
        function uscDynamicMetadataClient(serviceConfigurations) {
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
        uscDynamicMetadataClient.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this.bindLoaded();
        };
        /*
         * -------------------------- Methods --------------------------
         */
        /**
         * Eseguo il Bind della pagina
         */
        uscDynamicMetadataClient.prototype.bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        /**
         * Carico il modello nello usercontrol
         * @param idMetadataRepository
         */
        uscDynamicMetadataClient.prototype.loadMetadataRepository = function (idMetadataRepository) {
            var _this = this;
            this._service.getFullModelById(idMetadataRepository, function (data) {
                if (data) {
                    _this._currentMetadataRepositoryModel = data;
                    _this.loadPageItems(data.JsonMetadata);
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
         * Carico gli elementi della pagina
         * @param metadataRepositoryModel
         */
        uscDynamicMetadataClient.prototype.loadPageItems = function (jsonMetadata) {
            var _this = this;
            var metadataViewModel = JSON.parse(jsonMetadata);
            this.clearPage();
            $.each(metadataViewModel.TextFields, function (index, textFieldViewModel) {
                _this.fillHTMLGenericElement(_this.componentTextId, _this.controlsCounter, textFieldViewModel, "riTextBox");
                _this.controlsCounter++;
            });
            $.each(metadataViewModel.NumberFields, function (index, numberFieldViewModel) {
                _this.fillHTMLGenericElement(_this.componentNumberId, _this.controlsCounter, numberFieldViewModel, "riTextBox");
                _this.controlsCounter++;
            });
            $.each(metadataViewModel.BoolFields, function (index, boolFieldViewModel) {
                _this.fillHTMLGenericElement(_this.componentCheckBoxId, _this.controlsCounter, boolFieldViewModel, "form-control");
                _this.controlsCounter++;
            });
            $.each(metadataViewModel.DateFields, function (index, dateFieldViewModel) {
                _this.fillDateElement(_this.componentDateId, _this.controlsCounter, dateFieldViewModel, "riTextBox");
                _this.controlsCounter++;
            });
            this._discussions = metadataViewModel.DiscussionFields;
            $.each(metadataViewModel.DiscussionFields, function (index, discussionFieldViewModel) {
                _this.fillHTMLGenericElement(_this.componentCommentId, _this.controlsCounter, discussionFieldViewModel, "riTextBox");
                _this.controlsCounter++;
            });
            $.each(metadataViewModel.EnumFields, function (index, enumFieldViewModel) {
                _this.fillHTMLEnumElement(_this.componentEnumId, _this.controlsCounter, enumFieldViewModel, "ddlOptions");
                _this.controlsCounter++;
            });
            this.bindLoaded();
        };
        /**
         * Clono un elemento generico e lo completo con i vari attributi
         * @param idComponent
         * @param incrementalIdentifier
         * @param model
         * @param cssClass
         */
        uscDynamicMetadataClient.prototype.fillHTMLGenericElement = function (idComponent, incrementalIdentifier, model, cssClass) {
            var idCloned = this.cloneElement(idComponent, incrementalIdentifier);
            var clonedElement = ($("#".concat(idCloned))[0]);
            clonedElement.style.display = "";
            this.prepareLabel(idCloned, 0, model.Label);
            var inputElement = this.findGenericInputControl(idCloned, 0, cssClass);
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
        ;
        /**
         * Clono l'elemento ENUM e lo completo con i vari attributi
         * @param idComponent
         * @param incrementalIdentifier
         * @param model
         * @param cssClass
         */
        uscDynamicMetadataClient.prototype.fillHTMLEnumElement = function (idComponent, incrementalIdentifier, model, cssClass) {
            var idCloned = this.cloneElement(idComponent, incrementalIdentifier);
            var clonedElement = ($("#".concat(idCloned))[0]);
            clonedElement.style.display = "";
            this.prepareLabel(idCloned, 0, model.Label);
            var ddlElement = this.findSelectControl(idCloned, 0);
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
            $.each(model.Options, function (innerIndex, value) {
                var isSelected = (value == model.Value);
                $("#" + ddlElement.id).append($('<option>', {
                    value: innerIndex + 1,
                    text: value,
                    selected: isSelected
                }));
            });
        };
        ;
        /**
         * Popolo il campo data
         * @param idComponent
         * @param incrementalIdentifier
         * @param model
         * @param cssClass
         */
        uscDynamicMetadataClient.prototype.fillDateElement = function (idComponent, incrementalIdentifier, model, cssClass) {
            var idCloned = this.cloneElement(idComponent, incrementalIdentifier);
            var clonedElement = ($("#".concat(idCloned))[0]);
            clonedElement.style.display = "";
            var inputElement = this.findGenericInputControl(idCloned, 0, cssClass);
            var labelValidator = this.findLabelElement(idCloned, 1);
            this.prepareLabel(idCloned, 0, model.Label);
            this.setDefaultValues(inputElement, model);
            this.setRequiredAttribute(inputElement, model);
            $("#".concat(idCloned, " :input.", cssClass)).on("blur", { idParent: idCloned, class: cssClass }, this.onBlurEvent);
        };
        /**
         * evento al onfocusout del campo data
         * @param event
         */
        uscDynamicMetadataClient.prototype.onBlurEvent = function (event) {
            var idParent = event.data.idParent;
            var cssClass = event.data.class;
            var inputElement = ($("#".concat(idParent, " :input.", cssClass))[0]);
            var labelValidator = $("#" + idParent).find('label')[1];
            if (moment(inputElement.value, 'DD/MM/YYYY', 'it').isValid()) {
                labelValidator.style.visibility = "hidden";
            }
            else {
                labelValidator.style.visibility = "visible";
            }
            return false;
        };
        /**
         * Preparo il label
         * @param idParent
         * @param position
         * @param labelCaption
         */
        uscDynamicMetadataClient.prototype.prepareLabel = function (idParent, position, labelCaption) {
            var labelElement = this.findLabelElement(idParent, position);
            labelElement.textContent = labelCaption.concat(": ");
            labelElement.setAttribute(uscDynamicMetadataClient.FIELD_LABEL, labelCaption);
        };
        /**
         * Imposto alcuni default comuni dei campi di input
         * @param element
         * @param model
         */
        uscDynamicMetadataClient.prototype.setDefaultValues = function (element, model) {
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
        };
        /**
         * Imposto l'attributo required
         * @param element
         * @param model
         */
        uscDynamicMetadataClient.prototype.setRequiredAttribute = function (element, model) {
            element.setAttribute(uscDynamicMetadataClient.REQUIRED_FIELD, "false");
            if (model.Required) {
                element.required = true;
                element.setAttribute(uscDynamicMetadataClient.REQUIRED_FIELD, "true");
            }
        };
        /**
         * Recupero i dati dalla pagina
         */
        uscDynamicMetadataClient.prototype.bindModelFormPage = function () {
            var _this = this;
            var content = $("#menuContent").children();
            var item = new MetadataRepositoryModel();
            var metadataModel = new MetadataViewModel();
            var baseField;
            var enumField;
            var textField;
            var currentDiscussion;
            var inputElement;
            var requiredElement;
            var labelElement;
            var missingRequiredFields = [];
            var invalidInput = [];
            if (content.length < 1) {
                return null;
            }
            $.each(content, function (index, divElement) {
                labelElement = _this.findLabelElement(divElement.id, 0);
                var dataset = divElement.getAttribute("data-type");
                switch (dataset) {
                    case "Text":
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "riTextBox");
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
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "riTextBox");
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
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "riTextBox");
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
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "form-control");
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
                        var ddlElement = _this.findSelectControl(divElement.id, 0);
                        enumField = new EnumFieldViewModel();
                        enumField.Value = $("#".concat(ddlElement.id, " :selected")).text();
                        if (inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE)) {
                            enumField.DefaultValue = inputElement.getAttribute(uscDynamicMetadataClient.DEFAULT_VALUE);
                        }
                        enumField.Required = (inputElement.getAttribute(uscDynamicMetadataClient.REQUIRED_FIELD) == "true");
                        enumField.Label = labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL);
                        enumField.Options = {};
                        $.each(ddlElement, function (index, option) {
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
                        inputElement = _this.findGenericInputControl(divElement.id, 0, "riTextBox");
                        currentDiscussion = _this._discussions.filter(function (x) { return x.Label == labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL); })[0];
                        if (!currentDiscussion) {
                            currentDiscussion = new DiscussionFieldViewModel();
                            currentDiscussion.Label = labelElement.getAttribute(uscDynamicMetadataClient.FIELD_LABEL);
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
                var message_1 = "";
                $.each(missingRequiredFields, function (index, missingField) {
                    message_1 = message_1.concat(missingField, " \n ");
                });
                alert("Attenzione, i nomi dei seguenti campi sono obbligatori: \n ".concat(message_1));
                return "";
            }
            if (invalidInput.length > 0) {
                var message_2 = "";
                $.each(invalidInput, function (index, invalidField) {
                    message_2 = message_2.concat(invalidField, " \n ");
                });
                alert("Attenzione, i seguenti campi sono in formato non valido: \n ".concat(message_2));
                return "";
            }
            return JSON.stringify(metadataModel);
        };
        uscDynamicMetadataClient.FIELD_LABEL = "FieldLabel";
        uscDynamicMetadataClient.REQUIRED_FIELD = "RequiredField";
        uscDynamicMetadataClient.DEFAULT_VALUE = "DefaultValue";
        return uscDynamicMetadataClient;
    }(MetadataRepositoryBase));
    return uscDynamicMetadataClient;
});
//# sourceMappingURL=uscDynamicMetadataClient.js.map