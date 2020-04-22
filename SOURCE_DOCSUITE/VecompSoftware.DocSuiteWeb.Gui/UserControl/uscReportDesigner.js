/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Models/Reports/ReportBuilderModel", "App/Models/Reports/ReportBuilderConditionModel", "App/Models/Environment"], function (require, exports, ReportBuilderModel, ReportBuilderConditionModel, Environment) {
    var uscReportDesigner = /** @class */ (function () {
        function uscReportDesigner() {
            var _this = this;
            this.controlsCounter = 0;
            /**
             *------------------------- Events -----------------------------
             */
            this.Designer_DroppingItems = function (source, args, item) {
                try {
                    var target = args.get_htmlElement();
                    if (!target)
                        return;
                    if (!$(target).hasClass(uscReportDesigner.DROP_CLASS_NAME)) {
                        return;
                    }
                    _this.dropItem(args.get_sourceNode(), $(target), item);
                }
                catch (e) {
                    console.error(e);
                    _this.showNotification('Errore nella fase di gestione degli elementi da aggiungere al designer.');
                }
            };
            this.RgvCondition_OnItemDataBound = function (source, args) {
                try {
                    var item = args.get_dataItem();
                    var gridItem = args.get_item();
                    var cellProperty = $(gridItem.get_cell(uscReportDesigner.CONDITION_CELL_NAME));
                    cellProperty.addClass(uscReportDesigner.DROP_CLASS_NAME);
                    if (cellProperty.find('.report-element').length > 0) {
                        cellProperty.find('.report-element').remove();
                    }
                    cellProperty.html('');
                    if (!item.ConditionName) {
                        cellProperty.html('<i>Trascina qui una condizione...</i>');
                        return;
                    }
                    var clonedItem = _this.cloneElement(item.ConditionName, undefined, uscReportDesigner.CONDITION_ICON_URL);
                    clonedItem.data(item);
                    cellProperty.append(clonedItem);
                }
                catch (e) {
                    console.error(e);
                    _this.showNotification("Errore nella fase di popolamento della vista.");
                    throw e;
                }
            };
            this.RgvProjections_OnItemDataBound = function (source, args) {
                try {
                    var item = args.get_dataItem();
                    var gridItem = args.get_item();
                    var cellAlias = $(gridItem.get_cell(uscReportDesigner.PROJECTION_ALIAS_CELL_NAME));
                    cellAlias.find('.riTextBox').val(item.Alias);
                    var cellProperty = $(gridItem.get_cell(uscReportDesigner.PROJECTION_CELL_NAME));
                    cellProperty.addClass(uscReportDesigner.DROP_CLASS_NAME);
                    if (_this.editable) {
                        cellProperty.sortable().disableSelection();
                    }
                    var cellTagName = $(gridItem.get_cell(uscReportDesigner.PROJECTION_TAGNAME_CELL_NAME));
                    cellTagName.addClass('tagName');
                    for (var _i = 0, _a = item.ReportProperties; _i < _a.length; _i++) {
                        var projection = _a[_i];
                        var description = projection.DisplayName;
                        if (!description) {
                            description = projection.Name;
                        }
                        var clonedItem = _this.cloneElement(description, 'trascina per ordinare', uscReportDesigner.PROJECTION_ICON_URL);
                        clonedItem.data(projection);
                        cellProperty.append(clonedItem);
                    }
                }
                catch (e) {
                    console.error(e);
                    _this.showNotification("Errore nella fase di popolamento della vista.");
                    throw e;
                }
            };
            this.RtsDesigner_OnTabSelected = function (source, args) {
                $("#".concat(_this.pnlContentId)).trigger(uscReportDesigner.ON_CHANGED_VIEW, _this.currentActiveView());
            };
        }
        /**
         *------------------------- Methods -----------------------------
         */
        uscReportDesigner.prototype.initialize = function () {
            this._rgvCondition = $find(this.rgvConditionId);
            this._rgvCondition.add_rowDataBound(this.RgvCondition_OnItemDataBound);
            this._rgvProperties = $find(this.rgvPropertiesId);
            this._rgvProperties.add_rowDataBound(this.RgvProjections_OnItemDataBound);
            this._rgvSort = $find(this.rgvSortId);
            this._rtsDesigner = $find(this.rtsDesignerId);
            this._rtsDesigner.add_tabSelected(this.RtsDesigner_OnTabSelected);
            this.completeLoad();
        };
        uscReportDesigner.prototype.completeLoad = function () {
            $("#".concat(this.pnlContentId)).data(this);
            $("#".concat(this.pnlContentId)).trigger(uscReportDesigner.ON_END_LOAD_EVENT);
        };
        uscReportDesigner.prototype.loadDesignerModel = function (model) {
            try {
                this._rtsDesigner.findTabByValue(uscReportDesigner.PROJECTIONS_VIEW).select();
                this.loadConditionModels(model.Conditions);
                this.loadProjectionModels(model.Projections);
            }
            catch (e) {
                console.error(e);
                this.showNotification('Errore in caricamento dati del designer.');
            }
        };
        uscReportDesigner.prototype.getDesignerModel = function (model) {
            try {
                if (!model) {
                    model = new ReportBuilderModel();
                }
                var conditionModels = this.getConditionModels();
                var projectionModels = this.getProjectionModels();
                model.Conditions = conditionModels;
                model.Projections = projectionModels;
                return model;
            }
            catch (e) {
                console.error(e);
                this.showNotification('Errore nel recupero delle informazioni dal designer.');
            }
        };
        uscReportDesigner.prototype.removeControl = function (element) {
            try {
                if (!element || !this.editable) {
                    return;
                }
                var parent_1 = $(element).parents('.report-element');
                parent_1.remove();
                this.refreshSources();
            }
            catch (e) {
                console.error(e);
                this.showNotification("Errore nella fase di cancellazione dell'elemento selezionato.");
            }
        };
        uscReportDesigner.prototype.dropItem = function (source, target, item) {
            var currentView = this.currentActiveView();
            switch (currentView) {
                case uscReportDesigner.CONDITIONS_VIEW:
                    this.dropConditionItem(target, item);
                    break;
                case uscReportDesigner.PROJECTIONS_VIEW:
                    var dataItem = item;
                    var environmentName = Environment.toPublicDescription(dataItem.EntityType);
                    var itemDescription = this.getPropertyLongDescription(source, item, '');
                    itemDescription = environmentName.concat(" - ", itemDescription);
                    dataItem.DisplayName = itemDescription;
                    this.dropProjectionItem(itemDescription, target, item);
                    break;
            }
        };
        uscReportDesigner.prototype.dropProjectionItem = function (itemDescription, target, item) {
            if (this.currentActiveView() != uscReportDesigner.PROJECTIONS_VIEW) {
                return;
            }
            var clonedItem = this.cloneElement(itemDescription, 'trascina per ordinare', uscReportDesigner.PROJECTION_ICON_URL);
            clonedItem.data(item);
            $(target).append(clonedItem);
            this.refreshSources();
        };
        uscReportDesigner.prototype.dropConditionItem = function (target, item) {
            if (this.currentActiveView() != uscReportDesigner.CONDITIONS_VIEW) {
                return;
            }
            if (target.find('.report-element').length > 0) {
                return;
            }
            var clonedItem = this.cloneElement(item.ConditionName, undefined, uscReportDesigner.CONDITION_ICON_URL);
            clonedItem.data(item);
            $(target).append(clonedItem);
            this.refreshSources();
        };
        uscReportDesigner.prototype.dropSortItem = function (target, item) {
            //TODO: Implementare gestione ordinamento
            throw "not implemented";
        };
        uscReportDesigner.prototype.getProjectionModels = function () {
            var models = [];
            var gridDataItems = this._rgvProperties.get_masterTableView().get_dataItems();
            var _loop_1 = function (gridDataItem) {
                var dataItem = gridDataItem.get_dataItem();
                dataItem.ReportProperties = [];
                var jqItem = $(gridDataItem.get_cell(uscReportDesigner.PROJECTION_CELL_NAME));
                jqItem.find('.report-element').each(function (index, e) {
                    var propertyModel = $(e).data();
                    dataItem.ReportProperties.push(propertyModel);
                });
                models.push(dataItem);
            };
            for (var _i = 0, gridDataItems_1 = gridDataItems; _i < gridDataItems_1.length; _i++) {
                var gridDataItem = gridDataItems_1[_i];
                _loop_1(gridDataItem);
            }
            return models;
        };
        uscReportDesigner.prototype.getConditionModels = function () {
            var models = [];
            var gridDataItems = this._rgvCondition.get_masterTableView().get_dataItems();
            for (var _i = 0, gridDataItems_2 = gridDataItems; _i < gridDataItems_2.length; _i++) {
                var gridDataItem = gridDataItems_2[_i];
                var jqItem = $(gridDataItem.get_cell(uscReportDesigner.CONDITION_CELL_NAME));
                jqItem.find('.report-element').each(function (index, e) {
                    var conditionModel = $(e).data();
                    if (conditionModel.ConditionName) {
                        models.push(conditionModel);
                    }
                });
            }
            return models;
        };
        uscReportDesigner.prototype.getSortModels = function () {
            //TODO: implementare gestione ordinamento
            throw "not implemented";
        };
        uscReportDesigner.prototype.loadConditionModels = function (conditions) {
            var _this = this;
            if (this.editable) {
                conditions.push(new ReportBuilderConditionModel());
            }
            $(this._rgvCondition.get_masterTableView().get_element()).find(".report-element").remove();
            setTimeout(function () {
                _this._rgvCondition.get_masterTableView().set_dataSource(conditions);
                _this._rgvCondition.get_masterTableView().dataBind();
            }, 1);
        };
        uscReportDesigner.prototype.loadProjectionModels = function (projections) {
            var _this = this;
            if (projections.length == 0) {
                return;
            }
            $(this._rgvProperties.get_masterTableView().get_element()).find(".report-element").remove();
            setTimeout(function () {
                _this._rgvProperties.get_masterTableView().set_dataSource(projections);
                _this._rgvProperties.get_masterTableView().dataBind();
            }, 1);
        };
        uscReportDesigner.prototype.cloneElement = function (description, extraInfo, icon) {
            var clonedItem = $('#control-template').clone();
            clonedItem.attr('id', 'uid-'.concat(this.controlsCounter.toString()));
            clonedItem.find('.template-text').html(description);
            clonedItem.find('.template-icon').attr('src', icon);
            clonedItem.find('.small-description').html(extraInfo);
            this.controlsCounter++;
            return clonedItem;
        };
        uscReportDesigner.prototype.currentActiveView = function () {
            var selectedValue = this._rtsDesigner.get_selectedTab().get_value();
            return selectedValue;
        };
        uscReportDesigner.prototype.refreshSources = function () {
            var selectedView = this.currentActiveView();
            switch (selectedView) {
                case uscReportDesigner.CONDITIONS_VIEW:
                    var currentSource = this.getConditionModels();
                    currentSource.push(new ReportBuilderConditionModel());
                    this._rgvCondition.get_masterTableView().set_dataSource(currentSource);
                    this._rgvCondition.get_masterTableView().dataBind();
                    break;
                case uscReportDesigner.PROJECTIONS_VIEW:
                    break;
            }
        };
        uscReportDesigner.prototype.resetRowsStyle = function (grid) {
            var gridDataItems = grid.get_masterTableView().get_dataItems();
            for (var idataItem in gridDataItems) {
                if (Number(idataItem) % 2) {
                    $(gridDataItems[idataItem].get_element()).attr('class', 'rgAltRow');
                }
                else {
                    $(gridDataItems[idataItem].get_element()).attr('class', 'rgRow');
                }
            }
        };
        uscReportDesigner.prototype.getPropertyLongDescription = function (node, item, longDescription) {
            if (!node) {
                return longDescription;
            }
            var tmpDesc = item.DisplayName;
            if (!tmpDesc) {
                tmpDesc = item.Name;
            }
            longDescription = (longDescription) ? tmpDesc.concat(" - ", longDescription) : tmpDesc;
            if (node.get_level() > 1) {
                var parent_2 = node.get_parent();
                var dataItem = $(parent_2.get_element()).data();
                longDescription = this.getPropertyLongDescription(parent_2, dataItem, longDescription);
            }
            return longDescription;
        };
        uscReportDesigner.prototype.showNotification = function (message) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(message);
            }
        };
        uscReportDesigner.ON_END_LOAD_EVENT = "onEndLoad";
        uscReportDesigner.ON_CHANGED_VIEW = "onChangedView";
        uscReportDesigner.PROJECTION_CELL_NAME = "projection";
        uscReportDesigner.PROJECTION_ALIAS_CELL_NAME = "alias";
        uscReportDesigner.PROJECTION_TAGNAME_CELL_NAME = "tagName";
        uscReportDesigner.CONDITION_CELL_NAME = "condition";
        uscReportDesigner.DROP_CLASS_NAME = "drop";
        uscReportDesigner.PROJECTIONS_VIEW = "projectionsview";
        uscReportDesigner.CONDITIONS_VIEW = "conditionsview";
        uscReportDesigner.CONDITION_ICON_URL = "../App_Themes/DocSuite2008/imgset16/conditions_editor.png";
        uscReportDesigner.PROJECTION_ICON_URL = "../App_Themes/DocSuite2008/imgset16/extended_property.png";
        return uscReportDesigner;
    }());
    return uscReportDesigner;
});
//# sourceMappingURL=uscReportDesigner.js.map