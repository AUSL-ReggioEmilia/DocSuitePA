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
define(["require", "exports", "Tblt/MetadataRepositoryBase", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/ViewModels/Metadata/MetadataFinderViewModel", "App/Models/Commons/MetadataFinderType"], function (require, exports, MetadataRepositoryBase, ServiceConfigurationHelper, ExceptionDTO, MetadataFinderViewModel, MetadataFinderType) {
    var uscAdvancedSearchDynamicMetadataRest = /** @class */ (function (_super) {
        __extends(uscAdvancedSearchDynamicMetadataRest, _super);
        function uscAdvancedSearchDynamicMetadataRest(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME)) || this;
            _this._controlsCounter = 0;
            _this.getMetadataFinderModels = function () {
                var searchPanelElements = $("#menuContent").children();
                var currentFinderModel;
                var finderModels = [];
                var metadataValuesErrorCounter = 0;
                $.each(searchPanelElements, function (index, currentPanelElement) {
                    var elementType = currentPanelElement.getAttribute(uscAdvancedSearchDynamicMetadataRest.DATATYPE_ATTR);
                    currentFinderModel = _this._elementTypeFinderModelFactoryAction[elementType](currentPanelElement);
                    if (currentFinderModel.Value || currentFinderModel.ToValue) {
                        finderModels.push(currentFinderModel);
                        if (elementType === uscAdvancedSearchDynamicMetadataRest.DATE_ELEMENTTYPE || elementType === uscAdvancedSearchDynamicMetadataRest.NUMBER_ELEMENTTYPE) {
                            var validRangeValues = _this._validateRangeMetadataValues(elementType, currentFinderModel.Value, currentFinderModel.ToValue);
                            if (!validRangeValues) {
                                metadataValuesErrorCounter++;
                            }
                        }
                    }
                });
                return [finderModels, metadataValuesErrorCounter === 0];
            };
            _this._initializeSearchPanelElements = function (metadataDesignerModel) {
                _this._controlsCounter = 0;
                var totalNumOfElements = _this._getTotalNumberOfMetadataElements(metadataDesignerModel);
                for (var currentPosition = 0; currentPosition <= totalNumOfElements; currentPosition++) {
                    var _a = _this._getElementAtCurrentPosition(currentPosition, metadataDesignerModel), currentMetadataElement = _a[0], currentElementType = _a[1];
                    if (currentMetadataElement) {
                        _this._metadataElementTypeInitializationAction[currentElementType](_this._controlsCounter, currentMetadataElement);
                        _this._controlsCounter++;
                    }
                }
            };
            _this._onBlurEvent = function (eventInfo) {
                var parentElementId = eventInfo.data.idParent;
                var cssClass = eventInfo.data.class;
                var startInputElement = ($("#".concat(parentElementId, " :input.", cssClass))[0]);
                var endInputElement = ($("#".concat(parentElementId, " :input.", cssClass))[1]);
                var labelValidator = $("#" + parentElementId).find('label')[3];
                var validRangeValues = cssClass === uscAdvancedSearchDynamicMetadataRest.DATE_FIELD_CSSCLASS
                    ? _this._validateMetadataRangeDateValues(startInputElement.value, endInputElement.value)
                    : _this._validateMetadataRangeNumberValues(startInputElement.value, endInputElement.value);
                labelValidator.style.visibility = validRangeValues ? "hidden" : "visible";
            };
            return _this;
        }
        uscAdvancedSearchDynamicMetadataRest.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._bindLoaded();
            this._registerMetadataElementsInitializationActions();
            this._registerElementTypesFinderModelFactoryActions();
            this._registerPageControls();
        };
        uscAdvancedSearchDynamicMetadataRest.prototype.loadMetadataRepository = function (metadataRepositoryId) {
            var _this = this;
            this._service.getFullModelById(metadataRepositoryId, function (metadataRepositoryModel) {
                if (metadataRepositoryModel) {
                    _this._currentMetadataRepositoryModel = metadataRepositoryModel;
                    _this._initializeMetadataControls();
                }
            }, this._errorHandler);
        };
        uscAdvancedSearchDynamicMetadataRest.prototype.getCurrentMetadataRepositoryModel = function () {
            return this._currentMetadataRepositoryModel;
        };
        uscAdvancedSearchDynamicMetadataRest.prototype.setPanelSearchType = function (isAdvancedSearch, metadataRepositoryId) {
            if (isAdvancedSearch) {
                this.loadMetadataRepository(metadataRepositoryId);
            }
            else {
                this.clearPage();
            }
        };
        uscAdvancedSearchDynamicMetadataRest.prototype.clearAdvancedSearchPanelContent = function () {
            this.clearPage();
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._validateDateFormat = function (dateValue) {
            var dateFormat = moment(dateValue, 'DD/MM/YYYY', 'it');
            return dateFormat.isValid();
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._compareDateRange = function (startDateValue, endDateValue) {
            var startDate = new Date(moment(startDateValue, 'DD/MM/YYYY', 'it').format("L"));
            var endDate = new Date(moment(endDateValue, 'DD/MM/YYYY', 'it').format("L"));
            return startDate < endDate;
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._validateMetadataRangeDateValues = function (startDateValue, endDateValue) {
            if (startDateValue && endDateValue) {
                if (this._validateDateFormat(startDateValue) && this._validateDateFormat(endDateValue)) {
                    return this._compareDateRange(startDateValue, endDateValue);
                }
                return false;
            }
            else if (startDateValue) {
                return this._validateDateFormat(startDateValue);
            }
            else if (endDateValue) {
                return this._validateDateFormat(endDateValue);
            }
            return true;
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._validateMetadataRangeNumberValues = function (startValue, endValue) {
            if (startValue && endValue) {
                var startNumberValue = Number(startValue);
                var endNumberValue = Number(endValue);
                return startNumberValue < endNumberValue;
            }
            return true;
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._createFinderModel = function (metadataType, keyName, value, toValue) {
            if (toValue === void 0) { toValue = null; }
            var metadataFinderModel = new MetadataFinderViewModel();
            metadataFinderModel.MetadataType = MetadataFinderType[metadataType];
            metadataFinderModel.KeyName = keyName;
            metadataFinderModel.Value = value;
            metadataFinderModel.ToValue = toValue;
            return metadataFinderModel;
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._validateRangeMetadataValues = function (elementType, startValue, endValue) {
            var rangeValuesAreValid = elementType === uscAdvancedSearchDynamicMetadataRest.NUMBER_ELEMENTTYPE
                ? this._validateMetadataRangeNumberValues(startValue, endValue)
                : this._validateMetadataRangeDateValues(startValue, endValue);
            return rangeValuesAreValid;
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._initializeMetadataControls = function () {
            var metadataDesignerModel = JSON.parse(this._currentMetadataRepositoryModel.JsonMetadata);
            if (!metadataDesignerModel) {
                return;
            }
            this.clearPage();
            this._initializeSearchPanelElements(metadataDesignerModel);
            this._bindLoaded();
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._initializeHTMLGenericElement = function (idSelector, incrementalId, label, keyName, cssClass) {
            var cloneElementId = this._createHTMLElementClone(idSelector, incrementalId);
            this._prepareLabel(cloneElementId, 0, label);
            var clonedInputElement = this.findGenericInputControl(cloneElementId, 0, cssClass);
            clonedInputElement.value = "";
            clonedInputElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.KEY_NAME, keyName);
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._initializeRangeHTMLElement = function (idSelector, incrementalId, label, keyName, cssClass) {
            var cloneElementId = this._createHTMLElementClone(idSelector, incrementalId);
            this._prepareLabel(cloneElementId, 0, label);
            var rangeStartElement = this.findGenericInputControl(cloneElementId, 0, cssClass);
            var rangeEndElement = this.findGenericInputControl(cloneElementId, 1, cssClass);
            rangeStartElement.value = "";
            rangeEndElement.value = "";
            rangeStartElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.KEY_NAME, keyName);
            rangeEndElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.KEY_NAME, keyName);
            $("#" + cloneElementId + " input." + cssClass).on("blur", { idParent: cloneElementId, class: cssClass }, this._onBlurEvent);
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._initializeEnumHTMLElement = function (idSelector, incrementalId, label, keyName, options) {
            var cloneElementId = this._createHTMLElementClone(idSelector, incrementalId);
            this._prepareLabel(cloneElementId, 0, label);
            var ddlElement = this.findSelectControl(cloneElementId, 0);
            ddlElement.setAttribute("id", "id" + incrementalId);
            ddlElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.KEY_NAME, keyName);
            ddlElement.value = "";
            $("#" + ddlElement.id).append($('<option>', {
                value: 0,
                text: ""
            }));
            $.each(options, function (innerIndex, value) {
                $("#" + ddlElement.id).append($('<option>', {
                    value: innerIndex + 1,
                    text: value
                }));
            });
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._getElementAtCurrentPosition = function (currentPosition, metadataDesignerModel) {
            for (var currentElementType in metadataDesignerModel) {
                var currentMetadataElement = undefined;
                if (typeof metadataDesignerModel[currentElementType] !== "boolean") {
                    currentMetadataElement = metadataDesignerModel[currentElementType].filter(function (x) { return x.Position === currentPosition; })[0];
                }
                if (currentMetadataElement) {
                    return [currentMetadataElement, currentElementType];
                }
            }
            return [null, ""];
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._getTotalNumberOfMetadataElements = function (metadataDesignerModel) {
            var totalNumOfElements = 0;
            for (var currentKey in metadataDesignerModel) {
                if (typeof (metadataDesignerModel[currentKey]) !== "boolean") {
                    totalNumOfElements += metadataDesignerModel[currentKey].length;
                }
            }
            return totalNumOfElements;
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._createHTMLElementClone = function (idSelector, incrementalId) {
            var cloneElementId = this.cloneElement(idSelector, incrementalId);
            var clonedElement = ($("#".concat(cloneElementId))[0]);
            var elementClassList = clonedElement.classList;
            if (elementClassList) {
                clonedElement.classList.remove(uscAdvancedSearchDynamicMetadataRest.DISPLAY_NONE_CSSCLASS);
            }
            else {
                // IE9 compatibility because classList is not supported
                var currentCssClasses = clonedElement.className.split(" ").filter(function (cls) { return cls !== uscAdvancedSearchDynamicMetadataRest.DISPLAY_NONE_CSSCLASS; });
                clonedElement.className = currentCssClasses.join(" ");
            }
            return cloneElementId;
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._prepareLabel = function (idParent, position, labelCaption) {
            var labelElement = this.findLabelElement(idParent, position);
            labelElement.textContent = labelCaption.concat(": ");
            labelElement.setAttribute(uscAdvancedSearchDynamicMetadataRest.FIELD_LABEL, labelCaption);
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._errorHandler = function (exception) {
            if (exception && this._uscErrorNotification && exception instanceof ExceptionDTO) {
                if (!jQuery.isEmptyObject(this._uscErrorNotification)) {
                    this._uscErrorNotification.showNotification(exception);
                }
            }
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._registerMetadataElementsInitializationActions = function () {
            var _this = this;
            this._metadataElementTypeInitializationAction = {};
            this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_TEXT_FIELD] = function (incrementalId, model) {
                _this._initializeHTMLGenericElement(_this.componentTextId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
            };
            this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_BOOL_FIELD] = function (incrementalId, model) {
                _this._initializeHTMLGenericElement(_this.componentCheckBoxId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.BOOL_FIELD_CSSCLASS);
            };
            this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_DISCUSION_FIELD] = function (incrementalId, model) {
                _this._initializeHTMLGenericElement(_this.componentCommentId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
            };
            this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_DATE_FIELD] = function (incrementalId, model) {
                _this._initializeRangeHTMLElement(_this.componentDateId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.DATE_FIELD_CSSCLASS);
            };
            this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_NUMBER_FIELD] = function (incrementalId, model) {
                _this._initializeRangeHTMLElement(_this.componentNumberId, incrementalId, model.Label, model.KeyName, uscAdvancedSearchDynamicMetadataRest.NUMBER_FIELD_CSSCLASS);
            };
            this._metadataElementTypeInitializationAction[MetadataRepositoryBase.CONTROL_ENUM_FIELD] = function (incrementalId, model) {
                _this._initializeEnumHTMLElement(_this.componentEnumId, incrementalId, model.Label, model.KeyName, model.Options);
            };
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._registerElementTypesFinderModelFactoryActions = function () {
            var _this = this;
            this._elementTypeFinderModelFactoryAction = {};
            this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.TEXT_ELEMENTTYPE] = function (currentPanelElement) {
                var inputElement = _this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
                var fieldKeyName = inputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;
                return _this._createFinderModel(MetadataFinderType.Text, fieldKeyName, inputElement.value);
            };
            this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.NUMBER_ELEMENTTYPE] = function (currentPanelElement) {
                var startNumberInputElement = _this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
                var endNumberInputElement = _this.findGenericInputControl(currentPanelElement.id, 1, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
                var fieldKeyName = startNumberInputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;
                return _this._createFinderModel(MetadataFinderType.Number, fieldKeyName, startNumberInputElement.value, endNumberInputElement.value);
            };
            this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.DATE_ELEMENTTYPE] = function (currentPanelElement) {
                var startDateInputElement = _this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
                var endDateInputElement = _this.findGenericInputControl(currentPanelElement.id, 1, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
                var fieldKeyName = startDateInputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;
                var startDateValue = startDateInputElement.value ? moment(startDateInputElement.value, 'DD/MM/YYYY', 'it').format("L") : null;
                var endDateValue = endDateInputElement.value ? moment(endDateInputElement.value, 'DD/MM/YYYY', 'it').format("L") : null;
                return _this._createFinderModel(MetadataFinderType.Date, fieldKeyName, startDateValue, endDateValue);
            };
            this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.CHECKBOX_ELEMENTTYPE] = function (currentPanelElement) {
                var inputElement = _this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.BOOL_FIELD_CSSCLASS);
                var fieldKeyName = inputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;
                return _this._createFinderModel(MetadataFinderType.Bool, fieldKeyName, inputElement.value);
            };
            this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.ENUM_ELEMENTTYPE] = function (currentPanelElement) {
                var ddlElement = _this.findSelectControl(currentPanelElement.id, 0);
                var fieldKeyName = ddlElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;
                var ddlSelectedValue = $("#".concat(ddlElement.id, " :selected")).text();
                return _this._createFinderModel(MetadataFinderType.Enum, fieldKeyName, ddlSelectedValue);
            };
            this._elementTypeFinderModelFactoryAction[uscAdvancedSearchDynamicMetadataRest.COMMENT_ELEMENTTYPE] = function (currentPanelElement) {
                var inputElement = _this.findGenericInputControl(currentPanelElement.id, 0, uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS);
                var fieldKeyName = inputElement.attributes[uscAdvancedSearchDynamicMetadataRest.KEY_NAME].value;
                return _this._createFinderModel(MetadataFinderType.Discussion, fieldKeyName, inputElement.value);
            };
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._registerPageControls = function () {
            this._uscErrorNotification = $("#".concat(this.uscErrorNotificationId)).data();
        };
        uscAdvancedSearchDynamicMetadataRest.prototype._bindLoaded = function () {
            $("#".concat(this.pageContentId)).data(this);
        };
        uscAdvancedSearchDynamicMetadataRest.KEY_NAME = "KeyName";
        uscAdvancedSearchDynamicMetadataRest.FIELD_LABEL = "FieldLabel";
        uscAdvancedSearchDynamicMetadataRest.TEXT_FIELD_CSSCLASS = "riTextBox";
        uscAdvancedSearchDynamicMetadataRest.NUMBER_FIELD_CSSCLASS = "numberTextBox";
        uscAdvancedSearchDynamicMetadataRest.DATE_FIELD_CSSCLASS = "dateTextBox";
        uscAdvancedSearchDynamicMetadataRest.BOOL_FIELD_CSSCLASS = "form-control";
        uscAdvancedSearchDynamicMetadataRest.DATATYPE_ATTR = "data-type";
        uscAdvancedSearchDynamicMetadataRest.DISPLAY_NONE_CSSCLASS = "dsw-display-none";
        uscAdvancedSearchDynamicMetadataRest.TEXT_ELEMENTTYPE = "Text";
        uscAdvancedSearchDynamicMetadataRest.NUMBER_ELEMENTTYPE = "Number";
        uscAdvancedSearchDynamicMetadataRest.DATE_ELEMENTTYPE = "Date";
        uscAdvancedSearchDynamicMetadataRest.CHECKBOX_ELEMENTTYPE = "CheckBox";
        uscAdvancedSearchDynamicMetadataRest.ENUM_ELEMENTTYPE = "Enum";
        uscAdvancedSearchDynamicMetadataRest.COMMENT_ELEMENTTYPE = "Comment";
        return uscAdvancedSearchDynamicMetadataRest;
    }(MetadataRepositoryBase));
    return uscAdvancedSearchDynamicMetadataRest;
});
//# sourceMappingURL=UscAdvancedSearchDynamicMetadataRest.js.map