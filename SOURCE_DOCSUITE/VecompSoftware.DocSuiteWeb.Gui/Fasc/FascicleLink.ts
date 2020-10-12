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
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import DossierSummaryFolderViewModel = require('../App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import LinkedDossierViewModel = require('App/ViewModels/Dossiers/LinkedDossierViewModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');
import InsertActionType = require('App/Models/InsertActionType');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import UpdateActionType = require('App/Models/UpdateActionType');
import DossierModel = require("App/Models/Dossiers/DossierModel");
import uscDossierSummary = require('UserControl/uscDossierSummary');
import UscDossierFolders = require('UserControl/uscDossierFolders');

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
    rtsFascicleLinkId: string;
    radWindowManagerFascicleLink: string;
    btnSearchDossierId: string;
    rgvLinkedDossiersId: string;
    isFascicleTabSelected: boolean = false;
    fascicoliCollegatiId: string;
    dossierDisponibiliId: string;
    dossierCollegatiId: string;
    uscDossierSummaryId: string;
    dossierSummaryContainerId: string;
    dossierFolderCotainerId: string;
    uscDossierFoldersId: string;
    dossierFoldersLinked: DossierFolderModel[];

    private _btnLink: Telerik.Web.UI.RadButton;
    private _btnRemove: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rgvLinkedFascicles: Telerik.Web.UI.RadGrid;
    private _currentFascicle: FascicleModel;
    private _serviceConfigurations: ServiceConfiguration[];
    private _domainUserService: DomainUserService;
    private _fascicleLinkService: FascicleLinkService;
    private _rtsFascicleLink: Telerik.Web.UI.RadTabStrip;
    private _btnSearchDossier: Telerik.Web.UI.RadButton;
    private _dossierFolderService: DossierFolderService;
    private _rgvLinkedDossiers: Telerik.Web.UI.RadGrid;
    private _fascicleService: FascicleService;

    private _dossierFolderToLink: string;
    private _uscDossierSummary: uscDossierSummary;

    public static DossierTAB: string = "Dossier";
    public static FascicoliTAB: string = "Fascicoli";
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
        this._rgvLinkedDossiers = <Telerik.Web.UI.RadGrid>$find(this.rgvLinkedDossiersId);

        this._btnLink.set_enabled(false);
        this._btnRemove.set_enabled(false);

        this._rtsFascicleLink = $find(this.rtsFascicleLinkId) as Telerik.Web.UI.RadTabStrip;
        this._rtsFascicleLink.add_tabSelecting(this.RtsFascicleLink_OnTabSelecting);

        this._btnSearchDossier = <Telerik.Web.UI.RadButton>$find(this.btnSearchDossierId);
        this._btnSearchDossier.add_clicked(this.btnSearchDossier_OnClick);

        $(`#${this.uscFascicleSearchId}`).hide();
        this.loadFascicleSummary();

        this._uscDossierSummary = <uscDossierSummary>$(`#${this.uscDossierSummaryId}`).data();

        try {
            let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOMAIN_TYPE_NAME);
            this._domainUserService = new DomainUserService(domainUserConfiguration);

            let fascicleLinkServiceConfiguration: ServiceConfiguration = $.grep(this._serviceConfigurations, (x) => x.Name == FascicleBase.FASCICLE_LINK_TYPE_NAME)[0];
            this._fascicleLinkService = new FascicleLinkService(fascicleLinkServiceConfiguration);

            let dossierFolderServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOSSIERFOLDER_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderServiceConfiguration);

            let fascicleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME);
            this._fascicleService = new FascicleService(fascicleServiceConfiguration);
        }
        catch (error) {
            this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
        }
    }



    /**
     *------------------------- Events -----------------------------
     */

    btnSearchDossier_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let url: string = `../Dossiers/DossierRicerca.aspx?Type=Dossier&IsWindowPopupEnable=True&DossierStatusEnabled=False`;
        this.openWindow(url, "windowOpenDossierRicerca", 750, 600, this.closeWindowCallback);
    }

    RtsFascicleLink_OnTabSelecting = (source: Telerik.Web.UI.RadTabStrip, args: Telerik.Web.UI.RadTabStripCancelEventArgs) => {
        if (args.get_tab().get_value() == FascicleLink.DossierTAB) {
            this.isFascicleTabSelected = false;
            $(`#${this.dossierDisponibiliId}`).show();
            $(`#${this.dossierCollegatiId}`).show();

            this.loadDossier();

            $(`#${this.fascicoliCollegatiId}`).hide();
            $(`#${this.uscFascicleSearchId}`).hide();
            this._btnLink.set_enabled(false);
            this._btnRemove.set_enabled(false);
        } else {
            this.isFascicleTabSelected = true;
            $(`#${this.dossierDisponibiliId}`).hide();
            $(`#${this.dossierCollegatiId}`).hide();

            $(`#${this.fascicoliCollegatiId}`).show();
            $(`#${this.uscFascicleSearchId}`).show();

            $("#".concat(this.uscFascSummaryId)).bind(UscFascSummary.LOADED_EVENT, (args) => {
                this.loadFascicle();
            });

            this.loadFascicle();
        }
    }

    btnLink_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (this.isFascicleTabSelected == true) {
            this.linkFascicle();
        } else {
            this.linkDossier();
        }
    }

    btnRemove_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCheckedCancelEventArgs) => {
        args.set_cancel(true);
        if (this.isFascicleTabSelected == true) {
            this.removeFascicleLink();
        } else {
            this.removeDossierLink();
        }
    }

    /**
  *------------------------- Methods -----------------------------
  */

    openWindow(url, name, width, height, onCloseCallback?): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerFascicleLink);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        if (onCloseCallback) {
            wnd.add_close(onCloseCallback);
        }
        return false;
    }

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

    private loadFascicleSummary(): void {
        this._loadingPanel.show(this.pageContentId);
        this.service.getFascicle(this.currentFascicleId,
            (data: any) => {
                let fascicleModel: FascicleModel = <FascicleModel>data;
                let uscFascSummary: UscFascSummary = <UscFascSummary>$("#".concat(this.uscFascSummaryId)).data();
                if (!jQuery.isEmptyObject(uscFascSummary)) {
                    uscFascSummary.loadData(fascicleModel);
                }
                this._loadingPanel.hide(this.pageContentId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private loadDossierSummary(dossierId): JQueryPromise<void> {
        this._loadingPanel.show(this.pageContentId);
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this.dossierSumm(dossierId).done(() => {
            $(`#${this.dossierSummaryContainerId}`).show();
            $(`#${this.dossierFolderCotainerId}`).show();
            this._dossierFolderToLink = dossierId;
            this._btnLink.set_enabled(true);
            this._btnRemove.set_enabled(true);
            this._loadingPanel.hide(this.pageContentId);
            promise.resolve();
        }).fail((exception) => {
            promise.reject(exception);
        }).always(() => {
            this._loadingPanel.hide(this.pageContentId);
        });
        return promise.promise();
    }

    private dossierSumm(dossierId): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let uscDossierSumm = <uscDossierSummary>$(`#${this.uscDossierSummaryId}`).data();
        uscDossierSumm.loadDossierSummary(dossierId).done(() => {
            this.loadFolders(dossierId)
            promise.resolve();
        });
        return promise.promise();
    }

    private loadFolders(dossierId): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._dossierFolderService.getChildren(dossierId, DossierFolderStatus.Folder,
                (data: any) => {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        let uscDossierFolders: UscDossierFolders = <UscDossierFolders>$("#".concat(this.uscDossierFoldersId)).data();
                        uscDossierFolders.setRootNode(uscDossierSummary.DOSSIER_TITLE, dossierId);
                        UscDossierFolders.defaultFilterStatus = 8;
                        uscDossierFolders.loadNodes(data);
                        uscDossierFolders.setButtonVisibility(false);
                        uscDossierFolders.setStatusVisibility(false);

                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
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

    closeWindowCallback = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (!args.get_argument()) {
            return;
        }
        let idDossier: string = args.get_argument();
        this._loadingPanel.show(this.pageContentId);
        this.loadDossierSummary(idDossier);
    }

    private pad(currentNumber: number, paddingSize: number): string {
        let s = currentNumber + "";
        while (s.length < paddingSize) {
            s = `0${s}`
        }
        return s;
    }

    private loadDossier(): void {
        let models: Array<LinkedDossierViewModel> = new Array<LinkedDossierViewModel>();
        this._dossierFolderService.getLinkedDossierByFascicleId(this.currentFascicleId, (data: any) => {
            if (data == null) {
                this._btnRemove.set_enabled(false);
                this._btnLink.set_enabled(false);
                return;
            }
            this.dossierFoldersLinked = data;

            for (let dossierFolder of this.dossierFoldersLinked) {
                let model: LinkedDossierViewModel;
                let dossierName = `Dossier: ${dossierFolder.Dossier.Year}/${this.pad(+dossierFolder.Dossier.Number, 7)}`;

                model = {
                    UniqueId: dossierFolder.Dossier.UniqueId, DossierFolderName: dossierFolder.Name, DossierName: dossierName,
                    Subject: dossierFolder.Dossier.Subject, StartDate: moment(dossierFolder.Dossier.StartDate).format("DD/MM/YYYY"),
                    Contenitori: dossierFolder.Dossier.Container.Name, Category: dossierFolder.Category.Name
                }
                models.push(model);
            }

            let tableView: Telerik.Web.UI.GridTableView = this._rgvLinkedDossiers.get_masterTableView();

            tableView.clearSelectedItems();
            tableView.set_dataSource(models);
            tableView.dataBind();

            this._btnLink.set_enabled(true);
            this._btnRemove.set_enabled(true);
        });
    }

    private linkFascicle(): void {
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

    private linkDossier() {
        if (!this.currentFascicleId || !this._dossierFolderToLink) return;

        this._loadingPanel.show(this.pageContentId);
        this._fascicleService.getFascicle(this.currentFascicleId, (data) => {
            let fascicleModel: FascicleModel = data;
            let dossierModel: DossierModel = <DossierModel>{};
            dossierModel.UniqueId = this._dossierFolderToLink;

            let uscDossierFolders: UscDossierFolders = <UscDossierFolders>$("#".concat(this.uscDossierFoldersId)).data();
            let selectedDossierFolderNode: string = uscDossierFolders.getSelectedDossierFolderNode().get_value();
            let dossierFolder: DossierFolderModel = <DossierFolderModel>{
                ParentInsertId: selectedDossierFolderNode,
                Fascicle: fascicleModel,
                Status: DossierFolderStatus.Fascicle,
                Dossier: dossierModel,
                Category: fascicleModel.Category
            };
            this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.InsertDossierFolderAssociatedToFascicle,
                (data: any) => {
                    $(`#${this.dossierSummaryContainerId}`).hide();
                    $(`#${this.dossierFolderCotainerId}`).hide();
                    this.loadDossier();
                    this.dossierFoldersLinked.push(dossierFolder);
                    this._loadingPanel.hide(this.pageContentId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.pageContentId);
                    this.showNotificationException(this.uscNotificationId, exception);
                });
        },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private removeFascicleLink(): void {
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

    private removeDossierLink(): void {
        let dataItems: any = this._rgvLinkedDossiers.get_selectedItems();
        if (dataItems.length == 0) {
            this.showNotificationMessage(this.uscNotificationId, "Nessun dossier selezionato");
            return;
        }

        let dossierFModel: DossierFolderModel = <DossierFolderModel>dataItems[0].get_dataItem();
        let dossierFolderLink: DossierFolderModel = <DossierFolderModel>{};
        dossierFolderLink.UniqueId = this.dossierFoldersLinked.filter(x => x.Dossier.UniqueId == dossierFModel.UniqueId)[0].UniqueId;
        dossierFolderLink.Name = this.dossierFoldersLinked.filter(x => x.Dossier.UniqueId == dossierFModel.UniqueId)[0].Name;
        dossierFolderLink.Status = DossierFolderStatus.InProgress;

        this.dossierFoldersLinked = this.dossierFoldersLinked.filter(x => x.Dossier.UniqueId != dossierFModel.UniqueId);

        this._loadingPanel.show(this.pageContentId);
        this._dossierFolderService.updateDossierFolder(dossierFolderLink, UpdateActionType.RemoveFascicleFromDossierFolder, (data: any) => {
            this.loadDossier();
            this._loadingPanel.hide(this.pageContentId);
        }, (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.pageContentId);
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }
}


export = FascicleLink;