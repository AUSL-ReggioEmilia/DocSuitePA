import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import EnumHelper = require('App/Helpers/EnumHelper');
import CustomActionsIconModel = require('App/Models/Commons/CustomActionsIconModel');

class uscCustomActionsRest {

    pageContentId: string;
    isSummary: boolean;
    menuContentId: string;
    componentCheckBoxId: string;
    summaryComponentCheckboxId: string;
    summaryComponentIconId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;

    private static BOOLEAN_PropertyType = "boolean";
    private static ATTRIBUTE_PropertyName = "PropertyName";
    public static LOADED_EVENT: string = "onLoaded";

    _menuContent(): HTMLElement {
        return document.getElementById(this.menuContentId);
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize() {
        $(`#${this.pageContentId}`).data(this);
        $(`#${this.pageContentId}`).triggerHandler(uscCustomActionsRest.LOADED_EVENT);
        if (this.isSummary) {
            this._menuContent().parentElement.style.marginLeft = "0px";
            //style fix
            if (document.getElementById("mainTable").parentElement.parentElement.parentElement.parentElement) {
                document.getElementById("mainTable").parentElement.parentElement.parentElement.parentElement.style.overflow = "hidden";
                document.getElementById("mainTable").style.margin = "auto";
            }
        }
        else {
            document.getElementById("mainTable").removeAttribute("style");
        }
    }

    loadItems(customActions: any, customActionsIcons?: CustomActionsIconModel[]): void {
        this._clearPage();
        let incremental: number = 0;
        for (let customActionProperty in customActions) {
            switch (typeof customActions[customActionProperty]) {
                case uscCustomActionsRest.BOOLEAN_PropertyType: {
                    if (this.isSummary) {
                        if (customActionsIcons && customActionsIcons.filter(x => x.UseIconFor === customActionProperty).length > 0) {
                            let customActionsIcon: CustomActionsIconModel = customActionsIcons.filter(x => x.UseIconFor === customActionProperty)[0];
                            this._fillHTMLIconElement(incremental, this.summaryComponentIconId, customActionsIcon.IconURL, customActionsIcon.Tooltip);
                        }
                        else if (customActions[customActionProperty]) {
                            this._fillHTMLGenericElement(incremental, this.summaryComponentCheckboxId, customActionProperty, customActions[customActionProperty], "form-control");
                        }
                    }
                    else {
                        this._fillHTMLGenericElement(incremental, this.componentCheckBoxId, customActionProperty, customActions[customActionProperty], "form-control");
                    }
                    break;
                }
            }
            incremental++;
        }
    }

    private _fillHTMLGenericElement(incremental: number, idComponent: string, propertyName: string, value: any, cssClass: string): void {
        let idCloned: string = this._cloneElement(idComponent, incremental);
        let labelField: HTMLLabelElement = <HTMLLabelElement>$(`#${idCloned}`).find('label')[0];
        labelField.textContent = `${this._enumHelper.getCustomActionDescription(propertyName)}: `;
        labelField.setAttribute(uscCustomActionsRest.ATTRIBUTE_PropertyName, propertyName);
        let inputElement: HTMLInputElement = <HTMLInputElement>($(`#${idCloned} :input.${cssClass}`))[0];
        if (value) {
            inputElement.value = value;
            inputElement.checked = value;
        }
    }

    private _fillHTMLIconElement(incremental: number, idComponent: string, iconUrl: string, tooltip: string): void {
        let idCloned: string = this._cloneElement(idComponent, incremental);
        let iconElement: HTMLDivElement = <HTMLDivElement>document.getElementById(idCloned);
        let htmlIcon: HTMLImageElement = <HTMLImageElement>iconElement.children[0];
        htmlIcon.src = iconUrl;
        htmlIcon.title = tooltip;
    }

    private _cloneElement(elementId: string, incremental: number): string {
        let element;
        element = document.getElementById(elementId);
        let cln = element.cloneNode(true);
        cln.setAttribute("id", element.id + incremental)
        this._menuContent().appendChild(cln);
        return cln.id;
    }

    private _clearPage() {
        while (this._menuContent().firstChild) {
            this._menuContent().removeChild(this._menuContent().firstChild);
        }
    }

    getCustomActions<T>(): T {
        let customActions: T = <T>{};
        $.each(this._menuContent().children, (index: number, divElement: HTMLDivElement) => {
            let dataset: string = divElement.getAttribute("data-type");
            switch (dataset) {
                case "CheckBox":
                    let propertyName: string = divElement.querySelectorAll(`[${uscCustomActionsRest.ATTRIBUTE_PropertyName}]`)[0].getAttribute(uscCustomActionsRest.ATTRIBUTE_PropertyName);
                    let propertyValue: boolean = (<HTMLInputElement>divElement.querySelectorAll("input[type=radio]")[0]).checked;
                    customActions[propertyName] = propertyValue;
                    break;
                default:
                    break;
            }
        });
        return customActions;
    }
}

export = uscCustomActionsRest;