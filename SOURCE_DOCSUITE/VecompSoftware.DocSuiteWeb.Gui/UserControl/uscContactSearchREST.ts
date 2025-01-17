﻿/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ContactService = require("App/Services/Commons/ContactService");
import ContactSearchFilterDTO = require("App/DTOs/ContactSearchFilterDTO");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ImageHelper = require("../App/Helpers/ImageHelper");

class uscContactSearchRest {
    pnlMainContentId: string;
    rcdsContactsFinderId: string;
    rsbSearchBoxId: string;
    toolTipManagerId: string;
    applyAuthorizations?: boolean;
    excludeRoleContacts?: boolean;
    parentId?: number;
    parentToExclude?: number;
    filterByParentId?: number;
    tenantId?: string;

    private readonly _contactService: ContactService;
    private _rcdsContactsFinder: Telerik.Web.UI.RadClientDataSource;
    private _rsbSearchBox: Telerik.Web.UI.RadSearchBox;
    private _toolTipManager: Telerik.Web.UI.RadToolTipManager;
    private _selectedContactRoleIdSessionKey: string = "";

    static LOADED_EVENT: string = "onLoaded";
    static SELECTED_CONTACT_EVENT: string = "onSelectedContact";
    private readonly SEARCH_BY_PARENT_COMMAND = "searchByParent";
    private readonly DOWN_ARROW_ICON = "../App_Themes/DocSuite2008/imgset16/down_arrow.png";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let contactServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Contact");
        this._contactService = new ContactService(contactServiceConfiguration);
    }

    /**
    *------------------------- Events -----------------------------
    */

    rsbSearchBox_dataRequesting = (sender: Telerik.Web.UI.RadSearchBox, args: Telerik.Web.UI.SearchBoxDataRequestingEventArgs) => {
        args.set_cancel(true);
        let finderModel: ContactSearchFilterDTO = <ContactSearchFilterDTO>{
            Filter: args.get_text(),
            ApplyAuthorizations: this.applyAuthorizations,
            ExcludeRoleContacts: this.excludeRoleContacts,
            ParentId: this.filterByParentId,
            ParentToExclude: this.parentToExclude
        };

        if (sessionStorage.getItem(this._selectedContactRoleIdSessionKey)) {
            let idRole: number = +sessionStorage.getItem(this._selectedContactRoleIdSessionKey);
            finderModel.IdRole = idRole;
        }

        (<any>sender)._onRequestStart();
        this._contactService.findContacts(finderModel,
            (data: any) => {
                (<any>this._rcdsContactsFinder).set_data(data);
                this._rcdsContactsFinder.get_filterExpressions().clear();
                this._rcdsContactsFinder.fetch(() => {
                    let dataItemView = this._rcdsContactsFinder.view();
                    (<any>sender)._loadItemsFromData(dataItemView, true);
                    let searchBoxButton: Telerik.Web.UI.SearchBoxButton = this._rsbSearchBox.get_buttons().getButton(0);
                    searchBoxButton.set_imageUrl(this.DOWN_ARROW_ICON);
                });
            },
            (exception: ExceptionDTO) => {
                console.error(exception.statusText);
            }
        );
    }

    rsbSearchBox_search = (sender: Telerik.Web.UI.RadSearchBox, args: Telerik.Web.UI.SearchBoxSearchEventArgs) => {
        if (args.get_value()) {
            $(`#${this.pnlMainContentId}`).triggerHandler(uscContactSearchRest.SELECTED_CONTACT_EVENT, args.get_value());
        }
    }

    /**
    *------------------------- Methods -----------------------------
    */

    initialize(): void {
        this._rcdsContactsFinder = $find(this.rcdsContactsFinderId) as Telerik.Web.UI.RadClientDataSource;
        this._rsbSearchBox = $find(this.rsbSearchBoxId) as Telerik.Web.UI.RadSearchBox;
        this._rsbSearchBox.add_dataRequesting(this.rsbSearchBox_dataRequesting);
        this._rsbSearchBox.add_search(this.rsbSearchBox_search);
        this._rsbSearchBox.add_buttonCommand(this.rsbSearchBox_buttonCommand);
        this._toolTipManager = $find(this.toolTipManagerId) as Telerik.Web.UI.RadToolTipManager;

        let searchBoxButton: Telerik.Web.UI.SearchBoxButton = this._rsbSearchBox.get_buttons().getButton(0);
        if (!this.filterByParentId) {
            searchBoxButton.get_element().style.display = "none";
        }
        searchBoxButton.set_cssClass("searchBoxButton");
        this._selectedContactRoleIdSessionKey = `${this.pnlMainContentId}_selectedContactRoleIdSessionKey`;

        this.bindLoaded();
    }

    private bindLoaded(): void {
        $(`#${this.pnlMainContentId}`).data(this);
        $(`#${this.pnlMainContentId}`).triggerHandler(uscContactSearchRest.LOADED_EVENT);
    }

    showTooltip(idContact: number): void {
        let targetId: string = `item_${idContact}`;
        let target: Sys.UI.DomElement = Sys.UI.DomElement.getElementById(targetId);
        let tooltip: Telerik.Web.UI.RadToolTip = this._toolTipManager.getToolTipByElement(target);
        let createTooltipAction: () => JQueryPromise<void> = () => $.Deferred<void>().resolve().promise();
        if (!tooltip) {
            createTooltipAction = () => {
                let promise: JQueryDeferred<void> = $.Deferred<void>();
                this.createContactTree(`parentTree_${idContact}`, idContact)
                    .done(() => {
                        let content: string = document.getElementById(targetId.replace("item", "toolTipContent")).innerHTML;
                        tooltip = this._toolTipManager.createToolTip(target) as any;
                        tooltip.set_content(content);
                        promise.resolve();
                    })
                    .fail((exception: ExceptionDTO) => promise.reject(exception));
                return promise.promise();
            }
        }

        createTooltipAction()
            .done(() => {
                setTimeout(() => {
                    tooltip.show();
                }, 20);
            })
            .fail((exception: ExceptionDTO) => console.error(exception.statusText));
    }

    createContactTree(targetId: string, idContact: number): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let treeListHtml: string = "<ul>";
        this._contactService.getContactParents(idContact,
            (data: any) => {
                if (!data || data.length == 0) {
                    promise.resolve();
                    return;
                }

                let imageControlHtml: string;
                let labelControlHtml: string;
                for (var i = 0; i < data.length; i++) {
                    imageControlHtml = `<img src="${ImageHelper.getContactTypeImageUrl(data[i].ContactType)}" style="vertical-align: middle; margin-left: ${i * 20}px;"></img>`;
                    labelControlHtml = `<span style="vertical-align: middle;">${data[i].Description}</span>`;
                    treeListHtml = `${treeListHtml}<li>${imageControlHtml}${labelControlHtml}</li>`;
                }
                treeListHtml = `${treeListHtml}</ul>`;
                $(`#${targetId}`).html(treeListHtml);
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        );
        return promise.promise();
    }

    clear(): void {
        this._rsbSearchBox.clear();
        (<any>this._rsbSearchBox.get_inputElement()).value = "";
    }

    rsbSearchBox_buttonCommand = (sender: Telerik.Web.UI.RadSearchBox, args: Telerik.Web.UI.SearchBoxButtonCommandEventArgs) => {
        switch (args.get_commandName()) {
            case this.SEARCH_BY_PARENT_COMMAND: {
                if (!this.filterByParentId || !sessionStorage.getItem(this._selectedContactRoleIdSessionKey)) {
                    break;
                }
                (<any>sender)._onRequestStart();
                let idRole: number = +sessionStorage.getItem(this._selectedContactRoleIdSessionKey);
                this._contactService.getByParentId(this.filterByParentId, idRole, 20,
                    (data: any) => {
                        for (let contact of data) {
                            contact.Description = contact.Description.replace("|", " ");
                        }
                        (<any>this._rcdsContactsFinder).set_data(data);
                        this._rcdsContactsFinder.get_filterExpressions().clear();
                        this._rcdsContactsFinder.fetch(() => {
                            let dataItemView = this._rcdsContactsFinder.view();
                            (<any>sender)._loadItemsFromData(dataItemView, true);
                        });
                    },
                    (exception: ExceptionDTO) => {
                        console.error(exception.statusText);
                    });
                break;
            }
        }
    }
}

export = uscContactSearchRest;