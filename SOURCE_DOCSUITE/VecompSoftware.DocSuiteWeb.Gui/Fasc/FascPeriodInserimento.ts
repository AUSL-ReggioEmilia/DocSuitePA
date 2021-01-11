/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />

import FascicleBase = require('Fasc/FascBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import UscFascicleInsert = require('UserControl/uscFascicleInsert');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import CategoryFascicleService = require('App/Services/Commons/CategoryFascicleService');
import CategoryFascicleViewModel = require('App/ViewModels/Commons/CategoryFascicleViewModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import AjaxModel = require('App/Models/AjaxModel');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

declare var Page_IsValid: any;
class FascPeriodInserimento extends FascicleBase {

    btnConfermaId: string;
    uscFascInsertId: string;
    ajaxLoadingPanelId: string;
    currentPageId: string;
    uscNotificationId: string;
    ajaxManagerId: string;

    private _btnConferma: Telerik.Web.UI.RadButton;
    private _serviceConfigurations: ServiceConfiguration[];
    private _uscFascInsertId: string;
    private _categoryFascicles: Array<CategoryFascicleViewModel>;
    private _fascicleService: FascicleService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _categoryFascicleService: CategoryFascicleService;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }


    /**
    * --------------------------------------------- Events ---------------------------------
    */

    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();

        this._btnConferma = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._btnConferma.add_clicking(this.btmConferma_ButtonClicked);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._uscFascInsertId = this.uscFascInsertId;

        let fascicleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME);
        this._fascicleService = new FascicleService(fascicleConfiguration);

        let categoryFascicleService: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_CATEGORY_FASCICLE);
        this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);

    }

    btmConferma_ButtonClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonCancelEventArgs) => {
        if (!Page_IsValid) {
            eventArgs.set_cancel(true);
            return;
        }

        this._loadingPanel.show(this.currentPageId);
        this._btnConferma.set_enabled(false);
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.ActionName = "Insert";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    insertAllData(fascicle: FascicleModel) {
        let newFascicle: FascicleModel;
        this._categoryFascicleService.geAvailablePeriodicCategoryFascicles(fascicle.Category.EntityShortId.toString(),
            (data: any) => {
                if (data) {
                    this._categoryFascicles = data;
                    $.each(this._categoryFascicles, (index: number, categoryFascicle: CategoryFascicleViewModel) => {
                        newFascicle = fascicle;                        
                        if (FascicleType[categoryFascicle.FascicleType] == FascicleType.Period) {
                            $(document).queue((next) => {
                                newFascicle.DSWEnvironment = categoryFascicle.Environment;
                                this._fascicleService.insertFascicle(newFascicle, null,
                                    (data: any) => {
                                        next();
                                    },
                                    (exception: ExceptionDTO) => {
                                        this.showNotificationException(this.uscNotificationId, exception);

                                    });
                            });
                        }

                    });
                }
                $(document).queue((next) => {
                    window.location.href = "../Fasc/FascRicerca.aspx?Type=Fasc";
                    this._loadingPanel.hide(this.currentPageId);
                    next();
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.currentPageId);
                this.showNotificationException(this.uscNotificationId, exception);
                this._btnConferma.set_enabled(true);
            });
    }


    insertCallback(metadataDesignerModel: string, metadataValueModel: string): void {
        let uscFascInsert: UscFascicleInsert = <UscFascicleInsert>$("#".concat(this._uscFascInsertId)).data();
        if (!jQuery.isEmptyObject(uscFascInsert)) {
            let fascicle: FascicleModel = new FascicleModel;
            fascicle = uscFascInsert.getFascicle();

            if (!!metadataValueModel) {
                fascicle.MetadataValues = metadataValueModel;
                fascicle.MetadataDesigner = metadataDesignerModel;
                if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY)) {
                    let metadataRepository: MetadataRepositoryModel = new MetadataRepositoryModel();
                    metadataRepository.UniqueId = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY);
                    fascicle.MetadataRepository = metadataRepository;
                }
            }

            this.insertAllData(fascicle);
        }
    }

    insertFascicle(fascicle: FascicleModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._fascicleService.insertFascicle(fascicle, null,
                (data: any) => {
                    promise.resolve();
                },
                (exception: ExceptionDTO) => {
                    this.showNotificationException(this.uscNotificationId, exception);
                    promise.reject();
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }

        return promise.promise();
    }
}
export = FascPeriodInserimento