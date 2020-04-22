/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ReportBuilderModel = require('App/Models/Reports/ReportBuilderModel');
import ReportBuilderProjectionModel = require('App/Models/Reports/ReportBuilderProjectionModel');
import ReportBuilderConditionModel = require('App/Models/Reports/ReportBuilderConditionModel');
import ReportBuilderSortModel = require('App/Models/Reports/ReportBuilderSortModel');
import ReportBuilderPropertyModel = require('App/Models/Reports/ReportBuilderPropertyModel');
import ReportBuilderItem = require('App/Models/Reports/ReportBuilderItem');
import Environment = require('App/Models/Environment');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class uscReportDesigner {
    uscNotificationId: string;
    pnlContentId: string;
    rgvPropertiesId: string;
    rgvConditionId: string;
    rgvSortId: string;
    rtsDesignerId: string;
    editable: boolean;

    static ON_END_LOAD_EVENT = "onEndLoad";
    static ON_CHANGED_VIEW = "onChangedView";
    private static PROJECTION_CELL_NAME = "projection";
    private static PROJECTION_ALIAS_CELL_NAME = "alias";
    private static PROJECTION_TAGNAME_CELL_NAME = "tagName";
    private static CONDITION_CELL_NAME = "condition";
    private static DROP_CLASS_NAME = "drop";
    static PROJECTIONS_VIEW = "projectionsview";
    static CONDITIONS_VIEW = "conditionsview";
    private static CONDITION_ICON_URL = "../App_Themes/DocSuite2008/imgset16/conditions_editor.png";
    private static PROJECTION_ICON_URL = "../App_Themes/DocSuite2008/imgset16/extended_property.png";

    private _rgvProperties: Telerik.Web.UI.RadGrid;
    private _rgvCondition: Telerik.Web.UI.RadGrid;
    private _rgvSort: Telerik.Web.UI.RadGrid;
    private _rtsDesigner: Telerik.Web.UI.RadTabStrip;

    private controlsCounter = 0;

    constructor() {
    }

    /**
     *------------------------- Events -----------------------------
     */
    Designer_DroppingItems = (source: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeDroppingEventArgs, item: ReportBuilderItem) => {
        try {
            var target = (<any>args).get_htmlElement();
            if (!target) return;

            if (!$(target).hasClass(uscReportDesigner.DROP_CLASS_NAME)) {
                return;
            }

            this.dropItem((<any>args).get_sourceNode(), $(target), item);
        } catch (e) {
            console.error(e);
            this.showNotification('Errore nella fase di gestione degli elementi da aggiungere al designer.');
        }        
    }

    RgvCondition_OnItemDataBound = (source: Telerik.Web.UI.RadGrid, args: Telerik.Web.UI.GridRowDataBoundEventArgs) => {
        try {
            let item: ReportBuilderConditionModel = args.get_dataItem() as ReportBuilderConditionModel;
            let gridItem: Telerik.Web.UI.GridDataItem = args.get_item();
            let cellProperty: JQuery = $(gridItem.get_cell(uscReportDesigner.CONDITION_CELL_NAME));
            cellProperty.addClass(uscReportDesigner.DROP_CLASS_NAME);
            if (cellProperty.find('.report-element').length > 0) {
                cellProperty.find('.report-element').remove();
            }

            cellProperty.html('');
            if (!item.ConditionName) {
                cellProperty.html('<i>Trascina qui una condizione...</i>');
                return;
            }

            let clonedItem = this.cloneElement(item.ConditionName, undefined, uscReportDesigner.CONDITION_ICON_URL);
            clonedItem.data(item);
            cellProperty.append(clonedItem); 
        } catch (e) {
            console.error(e);
            this.showNotification("Errore nella fase di popolamento della vista.");
            throw e;
        }               
    }

    RgvProjections_OnItemDataBound = (source: Telerik.Web.UI.RadGrid, args: Telerik.Web.UI.GridRowDataBoundEventArgs) => {
        try {
            let item: ReportBuilderProjectionModel = args.get_dataItem() as ReportBuilderProjectionModel;
            let gridItem: Telerik.Web.UI.GridDataItem = args.get_item();
            let cellAlias: JQuery = $(gridItem.get_cell(uscReportDesigner.PROJECTION_ALIAS_CELL_NAME));
            cellAlias.find('.riTextBox').val(item.Alias);
            let cellProperty: JQuery = $(gridItem.get_cell(uscReportDesigner.PROJECTION_CELL_NAME));
            cellProperty.addClass(uscReportDesigner.DROP_CLASS_NAME);
            if (this.editable) {
                cellProperty.sortable().disableSelection();
            }
            let cellTagName: JQuery = $(gridItem.get_cell(uscReportDesigner.PROJECTION_TAGNAME_CELL_NAME));
            cellTagName.addClass('tagName');

            for (let projection of item.ReportProperties) {
                let description: string = projection.DisplayName;
                if (!description) {
                    description = projection.Name;
                }
                let clonedItem = this.cloneElement(description, 'trascina per ordinare', uscReportDesigner.PROJECTION_ICON_URL);
                clonedItem.data(projection);
                cellProperty.append(clonedItem);
            }
        } catch (e) {
            console.error(e);
            this.showNotification("Errore nella fase di popolamento della vista.");
            throw e;
        }        
    }

    RtsDesigner_OnTabSelected = (source: Telerik.Web.UI.RadTabStrip, args: Telerik.Web.UI.RadTabStripEventArgs) => {
        $("#".concat(this.pnlContentId)).trigger(uscReportDesigner.ON_CHANGED_VIEW, this.currentActiveView());
    }

    /**
     *------------------------- Methods -----------------------------
     */
    initialize() {
        this._rgvCondition = $find(this.rgvConditionId) as Telerik.Web.UI.RadGrid;
        this._rgvCondition.add_rowDataBound(this.RgvCondition_OnItemDataBound);
        this._rgvProperties = $find(this.rgvPropertiesId) as Telerik.Web.UI.RadGrid;
        this._rgvProperties.add_rowDataBound(this.RgvProjections_OnItemDataBound);
        this._rgvSort = $find(this.rgvSortId) as Telerik.Web.UI.RadGrid;
        this._rtsDesigner = $find(this.rtsDesignerId) as Telerik.Web.UI.RadTabStrip;
        this._rtsDesigner.add_tabSelected(this.RtsDesigner_OnTabSelected);
        this.completeLoad();
    }

    private completeLoad(): void {
        $("#".concat(this.pnlContentId)).data(this);
        $("#".concat(this.pnlContentId)).trigger(uscReportDesigner.ON_END_LOAD_EVENT);
    }

    loadDesignerModel(model: ReportBuilderModel): void {
        try {
            this._rtsDesigner.findTabByValue(uscReportDesigner.PROJECTIONS_VIEW).select();
            this.loadConditionModels(model.Conditions);
            this.loadProjectionModels(model.Projections);
        } catch (e) {
            console.error(e);
            this.showNotification('Errore in caricamento dati del designer.');            
        }        
    }

    getDesignerModel(): ReportBuilderModel
    getDesignerModel(model: ReportBuilderModel): ReportBuilderModel
    getDesignerModel(model?: ReportBuilderModel): ReportBuilderModel {
        try {
            if (!model) {
                model = new ReportBuilderModel();
            }
            let conditionModels: ReportBuilderConditionModel[] = this.getConditionModels();
            let projectionModels: ReportBuilderProjectionModel[] = this.getProjectionModels();

            model.Conditions = conditionModels;
            model.Projections = projectionModels;
            return model;
        } catch (e) {
            console.error(e);
            this.showNotification('Errore nel recupero delle informazioni dal designer.');
        }        
    }

    removeControl(element: HTMLElement): void {
        try {
            if (!element || !this.editable) {
                return;
            }

            let parent: JQuery = $(element).parents('.report-element');
            parent.remove();
            this.refreshSources();
        } catch (e) {
            console.error(e);
            this.showNotification("Errore nella fase di cancellazione dell'elemento selezionato.");
        }        
    }

    dropItem(source: Telerik.Web.UI.RadTreeNode, target: JQuery, item: ReportBuilderItem): void {
        let currentView: string = this.currentActiveView();
        switch (currentView) {
            case uscReportDesigner.CONDITIONS_VIEW:
                this.dropConditionItem(target, item as ReportBuilderConditionModel);
                break;
            case uscReportDesigner.PROJECTIONS_VIEW:
                let dataItem: ReportBuilderPropertyModel = item as ReportBuilderPropertyModel;
                let environmentName: string = Environment.toPublicDescription(dataItem.EntityType);
                let itemDescription: string = this.getPropertyLongDescription(source, item as ReportBuilderPropertyModel, '');
                itemDescription = environmentName.concat(" - ", itemDescription);
                dataItem.DisplayName = itemDescription;
                this.dropProjectionItem(itemDescription, target, item as ReportBuilderPropertyModel);
                break;
        }
    }

    private dropProjectionItem(itemDescription: string, target: JQuery, item: ReportBuilderPropertyModel): void {
        if (this.currentActiveView() != uscReportDesigner.PROJECTIONS_VIEW) {
            return;
        }

        let clonedItem = this.cloneElement(itemDescription, 'trascina per ordinare', uscReportDesigner.PROJECTION_ICON_URL);
        clonedItem.data(item);
        $(target).append(clonedItem);
        this.refreshSources();
    }

    private dropConditionItem(target: JQuery, item: ReportBuilderConditionModel): void {
        if (this.currentActiveView() != uscReportDesigner.CONDITIONS_VIEW) {
            return;
        }

        if (target.find('.report-element').length > 0) {
            return;
        }
        let clonedItem = this.cloneElement(item.ConditionName, undefined, uscReportDesigner.CONDITION_ICON_URL);
        clonedItem.data(item);
        $(target).append(clonedItem);
        this.refreshSources();
    }

    private dropSortItem(target: JQuery, item: ReportBuilderSortModel): void {
        //TODO: Implementare gestione ordinamento
        throw "not implemented";
    }

    private getProjectionModels(): ReportBuilderProjectionModel[] {
        let models: ReportBuilderProjectionModel[] = [];
        let gridDataItems: Telerik.Web.UI.GridDataItem[] = this._rgvProperties.get_masterTableView().get_dataItems() as Telerik.Web.UI.GridDataItem[];
        for (let gridDataItem of gridDataItems) {
            let dataItem: ReportBuilderProjectionModel = gridDataItem.get_dataItem() as ReportBuilderProjectionModel;
            dataItem.ReportProperties = [];
            let jqItem: JQuery = $(gridDataItem.get_cell(uscReportDesigner.PROJECTION_CELL_NAME));            
            jqItem.find('.report-element').each((index: number, e: Element) => {
                let propertyModel: ReportBuilderPropertyModel = $(e).data() as ReportBuilderPropertyModel;
                dataItem.ReportProperties.push(propertyModel);
            });
            models.push(dataItem);
        }
        return models;
    }

    private getConditionModels(): ReportBuilderConditionModel[] {
        let models: ReportBuilderConditionModel[] = [];
        let gridDataItems: Telerik.Web.UI.GridDataItem[] = this._rgvCondition.get_masterTableView().get_dataItems() as Telerik.Web.UI.GridDataItem[];
        for (let gridDataItem of gridDataItems) {
            let jqItem: JQuery = $(gridDataItem.get_cell(uscReportDesigner.CONDITION_CELL_NAME));
            jqItem.find('.report-element').each((index: number, e: Element) => {
                let conditionModel: ReportBuilderConditionModel = $(e).data() as ReportBuilderConditionModel;
                if (conditionModel.ConditionName) {
                    models.push(conditionModel);
                }                
            });
        }
        return models;
    }

    private getSortModels(): ReportBuilderSortModel[] {
        //TODO: implementare gestione ordinamento
        throw "not implemented";
    }

    private loadConditionModels(conditions: ReportBuilderConditionModel[]): void {
        if (this.editable) {
            conditions.push(new ReportBuilderConditionModel());
        }

        $(this._rgvCondition.get_masterTableView().get_element()).find(".report-element").remove();
        setTimeout(() => {
            this._rgvCondition.get_masterTableView().set_dataSource(conditions);
            this._rgvCondition.get_masterTableView().dataBind();
        }, 1);        
    }

    private loadProjectionModels(projections: ReportBuilderProjectionModel[]): void {
        if (projections.length == 0) {
            return;
        }

        $(this._rgvProperties.get_masterTableView().get_element()).find(".report-element").remove();
        setTimeout(() => {
            this._rgvProperties.get_masterTableView().set_dataSource(projections);
            this._rgvProperties.get_masterTableView().dataBind();
        }, 1);        
    }

    private cloneElement(description: string, extraInfo: string, icon: string): JQuery {
        let clonedItem: JQuery = $('#control-template').clone();
        clonedItem.attr('id', 'uid-'.concat(this.controlsCounter.toString()));
        clonedItem.find('.template-text').html(description);
        clonedItem.find('.template-icon').attr('src', icon);
        clonedItem.find('.small-description').html(extraInfo);
        this.controlsCounter++;
        return clonedItem;
    }

    currentActiveView(): string {
        let selectedValue: string = this._rtsDesigner.get_selectedTab().get_value();
        return selectedValue;
    }

    private refreshSources(): void {
        let selectedView: string = this.currentActiveView();
        switch (selectedView) {
            case uscReportDesigner.CONDITIONS_VIEW:
                let currentSource: ReportBuilderConditionModel[] = this.getConditionModels();
                currentSource.push(new ReportBuilderConditionModel());
                this._rgvCondition.get_masterTableView().set_dataSource(currentSource);
                this._rgvCondition.get_masterTableView().dataBind();
                break;
            case uscReportDesigner.PROJECTIONS_VIEW:
                break;
        }
    }

    private resetRowsStyle(grid: Telerik.Web.UI.RadGrid): void {
        let gridDataItems: Telerik.Web.UI.GridDataItem = grid.get_masterTableView().get_dataItems();
        for (let idataItem in gridDataItems) {
            if (Number(idataItem) % 2) {
                $(gridDataItems[idataItem].get_element()).attr('class', 'rgAltRow');
            }
            else {
                $(gridDataItems[idataItem].get_element()).attr('class', 'rgRow');
            }
        }
    }

    private getPropertyLongDescription(node: Telerik.Web.UI.RadTreeNode, item: ReportBuilderPropertyModel, longDescription: string): string {
        if (!node) {
            return longDescription;
        }

        let tmpDesc: string = item.DisplayName
        if (!tmpDesc) {
            tmpDesc = item.Name;
        }
        longDescription = (longDescription) ? tmpDesc.concat(" - ", longDescription) : tmpDesc;

        if (node.get_level() > 1) {
            let parent: Telerik.Web.UI.RadTreeNode = node.get_parent() as Telerik.Web.UI.RadTreeNode;
            let dataItem: ReportBuilderPropertyModel = $(parent.get_element()).data();
            longDescription = this.getPropertyLongDescription(parent, dataItem, longDescription);
        }
        return longDescription;
    }

    private showNotification(message: string): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(message);
        }
    }
}

export = uscReportDesigner;