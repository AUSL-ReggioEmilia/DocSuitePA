import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');
import uscSetiContactSel = require('./uscSetiContactSel');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class uscDynamicMetadata {
    dynamicPageContentId: string;
    uscNotificationId: string;
    ajaxManagerId: string;
    pnlDynamicContentId: string;
    managerId: string;
    fascicleInsertCommonIdEvent: string;
    
    private _serviceConfigurations: ServiceConfiguration[];
    private _metadataRepositoryConfiguration: ServiceConfiguration;
    private _metadataRepositoryService: MetadataRepositoryService;
    private DYNAMIC_FIELD_NAME_FORMAT: string = "field_";
    private _manager: Telerik.Web.UI.RadWindowManager;

    /**
* Costruttore
* @param serviceConfiguration
*/
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    /**
 *------------------------- Methods -----------------------------
 */

    initialize() {
        this._metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
        this._metadataRepositoryService = new MetadataRepositoryService(this._metadataRepositoryConfiguration);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);
        $("#".concat(this.pnlDynamicContentId)).data(this);


        $("#".concat(this.fascicleInsertCommonIdEvent)).on(uscSetiContactSel.SELECTED_SETI_CONTACT_EVENT, (sender, args) => {
            let ajaxModel = <AjaxModel>{};
            ajaxModel.ActionName = "PopulateFields";

            ajaxModel.Value = [JSON.stringify(args)];
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
        });
    }

    loadDynamicMetadata(id: string) {
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY);
        if (!id || id == "") {
            let ajaxModel = <AjaxModel>{};
            ajaxModel.ActionName = "ClearControls";
            ajaxModel.Value = [];
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
        }
        else {
            this._metadataRepositoryService.getFullModelById(id,
                (data: MetadataRepositoryModel) => {
                    if (data && !!data.JsonMetadata) {
                        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY, data.UniqueId);
                        let ajaxModel = <AjaxModel>{};
                        ajaxModel.ActionName = "LoadControls";
                        ajaxModel.Value = [];
                        ajaxModel.Value.push(data.JsonMetadata);
                        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
                    }
                },
                (exception: ExceptionDTO) => {
                    let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                    if (exception && uscNotification && exception instanceof ExceptionDTO) {
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showNotification(exception);
                        }
                    }
                });
        }
    }

    openCommentsWindow(label: string) {
        let wnd: Telerik.Web.UI.RadWindow = this._manager.open("../Comm/ViewMetadataComments.aspx?Type=Fasc&Label=".concat(label), "managerViewComments", null);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }
}

export = uscDynamicMetadata;