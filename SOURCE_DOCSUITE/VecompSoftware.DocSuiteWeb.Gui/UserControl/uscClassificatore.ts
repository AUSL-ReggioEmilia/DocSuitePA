/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import CategoryModel = require('App/Models/Commons/CategoryModel');

declare var ValidatorEnable: any;
class uscClassificatore {
    contentId: string;
    uscNotificationId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    anyNodeCheckId: string;

    private static SESSION_NAME_SELECTED_CATEGORIES: string = "SelectedCategories";
    public static LOADED_EVENT: string = "onLoaded";

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfiguration: ServiceConfiguration;
    private _selectedCategories: Array<CategoryModel>;
    private _sessionStorageKey: string;

    /**
    * Costruttore
         * @param serviceConfiguration
         * @param workflowStartConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
    *------------------------- Events -----------------------------
    */

    /**
    * ------------------------- Methods -----------------------------
    */

    /**
    * Initialize
    */
    initialize() {
        this._selectedCategories = new Array<CategoryModel>();
        this._sessionStorageKey = this.contentId.concat(uscClassificatore.SESSION_NAME_SELECTED_CATEGORIES);
        this.bindLoaded();
    }

    setCategories = (ajaxResult: any) => {
        let CategoriesToAdd: string[] = new Array<string>();
        //per ora ne passa una sola, ma potenzialmente potrebbero essere un array
        CategoriesToAdd.push(ajaxResult.Value);

        this._selectedCategories = new Array<CategoryModel>();

        if (ajaxResult.ActionName=="true"){
            let sessionValue = this.getCategories();
            if (sessionValue != null) {
                let source: any = JSON.parse(sessionValue);
                this._selectedCategories = this.parseCategoriesFromJson(source);
            }
        }        

        for (let r of CategoriesToAdd) {
            let categoryAdded: CategoryModel = <CategoryModel>{};
            categoryAdded.EntityShortId = parseInt(r);

            this._selectedCategories.push(categoryAdded);
        }

        sessionStorage[this._sessionStorageKey] = JSON.stringify(this._selectedCategories);
    }

    getCategories = () => {
        let result: any = sessionStorage[this._sessionStorageKey];
        if (result == null) {
            return null;
        }
        return result;
    }

    parseCategoriesFromJson = (source: any) => {
        let result: Array<CategoryModel> = new Array<CategoryModel>();
        for (let s of source) {
            let category: CategoryModel = <CategoryModel>{};
            category.EntityShortId = s.EntityShortId;
            result.push(category);
        }
        return result
    } 

    clearSessionStorage = () => {
        if (sessionStorage[this._sessionStorageKey] != null) {
            sessionStorage.removeItem(this._sessionStorageKey);
        }
    }

    /**
* Scateno l'evento di "Load Completed" del controllo
*/
    private bindLoaded(): void {
        $("#".concat(this.contentId)).data(this);
        $("#".concat(this.contentId)).triggerHandler(uscClassificatore.LOADED_EVENT);

    }

    enableValidators = (state: boolean) => {
        ValidatorEnable($get(this.anyNodeCheckId), state);
    }
}
export = uscClassificatore;