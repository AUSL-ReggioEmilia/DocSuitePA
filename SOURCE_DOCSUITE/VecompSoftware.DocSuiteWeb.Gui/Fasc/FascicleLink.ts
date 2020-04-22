/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleLinkModel = require('App/Models/Fascicles/FascicleLinkModel');
import FascicleLinkType = require('App/Models/Fascicles/FascicleLinkType');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import FascicleLinkService = require('App/Services/Fascicles/FascicleLinkService');
import FascicleBase = require('Fasc/FascBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import LinkedFasciclesViewModel = require('App/ViewModels/Fascicles/LinkedFasciclesViewModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscFascSummary = require('UserControl/uscFascSummary');
import uscFascicleSearch = require('UserControl/uscFascicleSearch');

class FascicleLink extends FascicleBase {
    currentFascicleId: string;
    btnLinkId: string;
    btnRemoveId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    rgvLinkedFasciclesId: string;
    uscNotificationId: string;
    btnLinkUniqueId: string;
    ajaxManagerId: string;
    maxNumberElements: string;
    uscFascSummaryId: string;
    uscFascicleSearchId: string;

    private _btnLink: Telerik.Web.UI.RadButton;
    private _btnRemove: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rgvLinkedFascicles: Telerik.Web.UI.RadGrid;
    private _currentFascicle: FascicleModel;
    private _serviceConfigurations: ServiceConfiguration[];
    private _domainUserService: DomainUserService;
    private _fascicleLinkService: FascicleLinkService;
    /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
    * Inizializzazione
    */
    initialize(): void {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._btnLink = <Telerik.Web.UI.RadButton>$find(this.btnLinkId);
        if (this._btnLink) {
            this._btnLink.add_clicking(this.btnLink_OnClick);
        }
        this._btnRemove = <Telerik.Web.UI.RadButton>$find(this.btnRemoveId);
        if (this._btnRemove) {
            this._btnRemove.add_clicking(this.btnRemove_OnClick);
        }
        this._rgvLinkedFascicles = <Telerik.Web.UI.RadGrid>$find(this.rgvLinkedFasciclesId);

        this._btnLink.set_enabled(false);
        this._btnRemove.set_enabled(false);

        try {
            let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOMAIN_TYPE_NAME);
            this._domainUserService = new DomainUserService(domainUserConfiguration);

            let fascicleLinkServiceConfiguration: ServiceConfiguration = $.grep(this._serviceConfigurations, (x) => x.Name == FascicleBase.FASCICLE_LINK_TYPE_NAME)[0];
            this._fascicleLinkService = new FascicleLinkService(fascicleLinkServiceConfiguration);

            $("#".concat(this.uscFascSummaryId)).bind(UscFascSummary.LOADED_EVENT, (args) => {
                this.loadFascicle();
            });

            this.loadFascicle();
        }
        catch (error) {
            this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
        }
    }



    /**
     *------------------------- Events -----------------------------
     */
    btnLink_OnClick = (sender: any, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        args.set_cancel(true);
        let selectedFascicle: FascicleModel;
        let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
        if (!jQuery.isEmptyObject(uscFascicleSearch)) {
            selectedFascicle = uscFascicleSearch.getSelectedFascicle();
        }
        if (selectedFascicle == null) {
            this.showNotificationMessage(this.uscNotificationId, "Nessun fascicolo selezionato");
            return;
        }

        let model: FascicleLinkModel = new FascicleLinkModel(selectedFascicle.UniqueId);
        let currentFascicle: FascicleModel = <FascicleModel>this._currentFascicle;
        model.Fascicle = currentFascicle;
        model.FascicleLinkType = FascicleLinkType.Manual;

        this._loadingPanel.show(this.pageContentId);
        this._fascicleLinkService.insertFascicleLink(model,
            (data: any) => {
                this.loadData(this._currentFascicle, () => {
                    this._loadingPanel.hide(this.pageContentId);
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    btnRemove_OnClick = (sender: any, args: Telerik.Web.UI.RadButtonCheckedCancelEventArgs) => {
        args.set_cancel(true);
        let dataItems: any = this._rgvLinkedFascicles.get_selectedItems();
        if (dataItems.length == 0) {
            this.showNotificationMessage(this.uscNotificationId, "Nessun fascicolo selezionato");
            return;
        }

        let currentFascicle: FascicleModel = <FascicleModel>this._currentFascicle;
        let model: LinkedFasciclesViewModel = <LinkedFasciclesViewModel>dataItems[0].get_dataItem();
        let fascicleLink: FascicleLinkModel = new FascicleLinkModel(model.UniqueId);
        fascicleLink.Fascicle = currentFascicle;
        fascicleLink.UniqueId = model.FascicleLinkUniqueId;

        this._loadingPanel.show(this.pageContentId);
        this._fascicleLinkService.deleteFascicleLink(fascicleLink,
            (data: any) => {
                this.loadData(this._currentFascicle, () => {
                    this._loadingPanel.hide(this.pageContentId);
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
  *------------------------- Methods -----------------------------
  */

    private loadFascicle(): void {
        this._loadingPanel.show(this.pageContentId);
        this.service.getFascicle(this.currentFascicleId,
            (data: any) => {
                if (data == null) {
                    this._btnRemove.set_enabled(false);
                    this._btnLink.set_enabled(false);
                    return;
                }

                let fascicleModel: FascicleModel = <FascicleModel>data;
                let uscFascSummary: UscFascSummary = <UscFascSummary>$("#".concat(this.uscFascSummaryId)).data();
                if (!jQuery.isEmptyObject(uscFascSummary)) {
                    uscFascSummary.loadData(fascicleModel);
                }
                this._currentFascicle = fascicleModel;

                this.loadData(fascicleModel, () => {
                    this._loadingPanel.hide(this.pageContentId);
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
    * TODO: da togliere a favore di Signalr
    */
    private loadData(fascicle: FascicleModel, callback?: Function): void {
        this.service.getLinkedFascicles(fascicle, null,
            (data: FascicleModel) => {
                let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
                if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                    uscFascicleSearch.clearSelections();
                }

                this.refreshLinkedFascicles(data);
                if (callback) {
                    callback();
                }
            },
            (exception: ExceptionDTO) => {
                this._btnLink.set_enabled(true);
                this._btnRemove.set_enabled(true);
                this.showNotificationException(this.uscNotificationId, exception);
                if (callback) {
                    callback();
                }
            }
        );
    }

    refreshLinkedFascicles(data: FascicleModel) {
        let models: Array<LinkedFasciclesViewModel> = new Array<LinkedFasciclesViewModel>();
        if (data == null) return;
        if (data.FascicleLinks.length > 0) {
            try {
                $.each(data.FascicleLinks, (index, fascicleLink) => {
                    let model: LinkedFasciclesViewModel;
                    let imageUrl: string = "";
                    let openCloseTooltip: string = "";
                    let fascicleTypeImageUrl: string = "";
                    let fascicleTypeTooltip: string = "";
                    if (fascicleLink.FascicleLinked.EndDate == null) {
                        imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                        openCloseTooltip = "Aperto";
                    } else {
                        imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                        openCloseTooltip = "Chiuso";
                    }

                    switch (FascicleType[fascicleLink.FascicleLinked.FascicleType.toString()]) {
                        case FascicleType.Period:
                            fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/history.png";
                            fascicleTypeTooltip = "Periodico";
                            break;
                        case FascicleType.Legacy:
                            fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_legacy.png";
                            fascicleTypeTooltip = "Fascicolo non a norma";
                            break;
                        case FascicleType.Procedure:
                            fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png";
                            fascicleTypeTooltip = "Per procedimento";
                            break;
                        case FascicleType.SubFascicle:
                            fascicleTypeImageUrl = "";
                            fascicleTypeTooltip = "Sotto fascicolo";
                            break;
                    }


                    let tileText: string = fascicleLink.FascicleLinked.Title.concat(" ",
                        fascicleLink.FascicleLinked.FascicleObject);

                    model = {
                        Name: tileText, FascicleLinkUniqueId: fascicleLink.UniqueId, UniqueId: fascicleLink.FascicleLinked.UniqueId, Category: fascicleLink.FascicleLinked.Category.Name,
                        ImageUrl: imageUrl, OpenCloseTooltip: openCloseTooltip, FascicleTypeImageUrl: fascicleTypeImageUrl, FascicleTypeToolTip: fascicleTypeTooltip
                    };
                    models.push(model);
                });
            }
            catch (error) {
                 this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                console.log((<Error>error).message);
                return;
            }
        }

        let tableView: Telerik.Web.UI.GridTableView = this._rgvLinkedFascicles.get_masterTableView();

        tableView.clearSelectedItems();
        tableView.set_dataSource(models);
        tableView.dataBind();

        //TODO: da rivedere
        let row = tableView.get_dataItems();
        for (let i = 0; i < row.length; i++) {
            if (i % 2) {
                row[i].addCssClass("Chiaro");
            }
            else {
                row[i].addCssClass("Scuro");
            }
        }
        this._btnLink.set_enabled(true);
        this._btnRemove.set_enabled(true);
    }
}


export = FascicleLink;