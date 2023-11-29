/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />


import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DocumentUnitService = require('App/Services/DocumentUnits/DocumentUnitService');
import EnumHelper = require("App/Helpers/EnumHelper");
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DocumentUnitModelMapper = require('App/Mappers/DocumentUnits/DocumentUnitModelMapper');
import DocumentUnitViewModel = require('App/ViewModels/DocumentUnits/DocumentUnitViewModel');
import Environment = require('App/Models/Environment');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');
class FullTextSearch {
    btnFullTextSearchId: string;
    txtFullTextSearchId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    tenantId: string;
    rgvDocumentListsId: string;
    actionsToolbarId: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _documentUnitService: DocumentUnitService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _btnFullTextSearch: Telerik.Web.UI.RadToolBarButton;
    private _txtFullTextSearch: Telerik.Web.UI.RadTextBox;
    private _rgvDocumentLists: Telerik.Web.UI.RadGrid;
    private _rgvDocumentMasterTableView: Telerik.Web.UI.GridTableView;
    private _environmentImageDictionary;
    private _actionToolbar: Telerik.Web.UI.RadToolBar;
    private _toolBarFullText: Telerik.Web.UI.RadToolBarButton;


    /**
     * Costruttore
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }
    initialize() {
        let documentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
        this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._actionToolbar = $find(this.actionsToolbarId) as Telerik.Web.UI.RadToolBar;
        this._toolBarFullText = <Telerik.Web.UI.RadToolBarButton>this._actionToolbar.findItemByValue("toolBarFullText");
        this._txtFullTextSearch = <Telerik.Web.UI.RadTextBox>this._toolBarFullText.findControl("txtFullTextSearch");
        this._btnFullTextSearch = <Telerik.Web.UI.RadToolBarButton>this._actionToolbar.findItemByValue(this.btnFullTextSearchId);

        this._rgvDocumentLists = <Telerik.Web.UI.RadGrid>$find(this.rgvDocumentListsId);
        this._rgvDocumentMasterTableView = this._rgvDocumentLists.get_masterTableView();
        this._rgvDocumentLists.get_element().style.display = "none";
        this._actionToolbar.add_buttonClicked(this.ToolBar_ButtonClick);

        /// Document units environment images
        this._environmentImageDictionary = {};
        this._registerEnvironmentImages();

        this.setLastSearch();
    }

    ToolBar_ButtonClick = (event: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case "searchFullText": {
                let documentType: string = this._txtFullTextSearch.get_value();
                let IdTentant: string = this.tenantId;
                this.loadDocumentUnitsFullText(documentType, IdTentant);
                break;
            }
        }
    };

    loadDocumentUnitsFullText = (searchFullText: string, idTenant: string) => {
        this._loadingPanel.show(this.pageContentId);
        this.getDocumentUnitsFullText(searchFullText, idTenant)
            .done((data) => {
                var dateExpired = new Date();
                dateExpired.setSeconds(dateExpired.getSeconds() + 15);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TIME, dateExpired.toString());
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TEXT, searchFullText);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_DATA, JSON.stringify(data));
                this.loadResult(data);
                this._loadingPanel.hide(this.pageContentId);
            }).fail((exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
            });
    }

    private loadResult(documentUnits: DocumentUnitModel[]) {
        this._rgvDocumentLists.get_element().style.display = "";
        let models: Array<DocumentUnitViewModel> = new Array<DocumentUnitViewModel>();
        $.each(documentUnits, (index, documentUnitModel) => {
            let model: DocumentUnitViewModel;
            let valuesUrl = this.formatUrl(documentUnitModel);
            model = {
                DocumentUnit: documentUnitModel,
                ViewerUrl: valuesUrl[0],
                DocumentUnitUrl: valuesUrl[1],
                IconUrl: this.getIconUrl(documentUnitModel.Environment),
                RegistrationDate: moment(documentUnitModel.RegistrationDate).format("DD/MM/YYYY")
            };
            models.push(model);
        });
        this._rgvDocumentMasterTableView.set_dataSource(models);
        this._rgvDocumentMasterTableView.set_virtualItemCount(documentUnits.length);
        this._rgvDocumentMasterTableView.dataBind();
    }

    private getDocumentUnitsFullText(searchFullText: string, idTenant: string): JQueryPromise<DocumentUnitModel[]> {
        let promise: JQueryDeferred<DocumentUnitModel[]> = $.Deferred<DocumentUnitModel[]>();
        this._documentUnitService.getDocumentUnitsFullText(searchFullText, idTenant, (data: DocumentUnitModel[]) => {
            promise.resolve(data);
        }, (exception: ExceptionDTO) => {
            promise.reject(exception);
        });
        return promise.promise();
    }

    private formatUrl(documentUnit: DocumentUnitModel): string[] {
        var formatDocumentUnitUrl: string = "";
        var formatViewerUrl: string = "";
        formatViewerUrl = ``;
        formatDocumentUnitUrl = ``;


        let env: Environment = (documentUnit.Environment < 100 ? <Environment>documentUnit.Environment : Environment.UDS);
        switch (env) {
            case Environment.Any: {
                formatViewerUrl = `../Viewers/ProtocolViewer.aspx?UniqueId=${documentUnit.UniqueId}&Type=Prot"`;
                formatDocumentUnitUrl = `../Prot/ProtVisualizza.aspx?UniqueId=${documentUnit.UniqueId}&Type=Prot`;
                break;
            }
            case Environment.Dossier: {
                formatViewerUrl = `../Viewers/DossierViewer.aspx?Type=Dossier&IdDossier=${documentUnit.UniqueId}`;
                formatDocumentUnitUrl = `../Dossier/DossierVisualizza.aspx?Type=Dossier&IdDossier=${documentUnit.UniqueId}&DossierTitle=${documentUnit.Number}`;
                break;
            }
            case Environment.DocumentSeries: {
                formatViewerUrl = `../Viewers/DocumentSeriesItemViewer.aspx?ids=${documentUnit.EntityId.toString()}&Type=Prot"`;
                formatDocumentUnitUrl = `..Series/Item.aspx?IdDocumentSeriesItem=${documentUnit.EntityId.toString()}&Action=View&Type=Series`;
                break;
            }
            case Environment.Fascicle: {
                formatViewerUrl = `../Viewers/FascicleViewer.aspx?Type=Fasc&FascicleIds=${documentUnit.IdFascicle}`;
                formatDocumentUnitUrl = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${documentUnit.IdFascicle}`;
                break;
            }
            case Environment.Resolution: {
                formatViewerUrl = `../Viewers/ResolutionViewer.aspx?${documentUnit.UniqueId}.aspx?UniqueId=${documentUnit.UniqueId}&Type=Prot"`;
                formatDocumentUnitUrl = `../Resl/ReslVisualizza.aspx?Type=Resl&IdResolution=${documentUnit.UniqueId}`;
                break;
            }
            case Environment.Protocol: {
                formatViewerUrl = `../Viewers/ProtocolViewer.aspx?UniqueId=${documentUnit.UniqueId}&Type=Prot"`;
                formatDocumentUnitUrl = `../Prot/ProtVisualizza.aspx?UniqueId=${documentUnit.UniqueId}&Type=Prot`;;
                break;
            }
            case Environment.UDS: {
                formatViewerUrl = "../Viewers/UDSViewer.aspx?IdUDS=".concat(documentUnit.UniqueId, " & IdUDSRepository=", documentUnit.UDSRepository.UniqueId);
                formatDocumentUnitUrl = `../UDS/UDSView.aspx?IdUDSRepository=${documentUnit.UDSRepository.UniqueId}&IdUDS=${documentUnit.UniqueId}&Action=View&Type=UDS`;
                break;
            }
        }
        return [formatViewerUrl, formatDocumentUnitUrl]
    }
 
    setLastSearch() {
        let documentUnitsFullTextTime: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TIME);
        if (documentUnitsFullTextTime) {
            let dateExpired = Date.parse(documentUnitsFullTextTime);
            if (dateExpired < Date.now()) {
                sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TIME);
                sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TEXT);
                sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNITS_FULLTEXT_DATA);
                this._txtFullTextSearch.set_value("");
            }
            else {
                let newdateExpired = new Date();
                newdateExpired.setSeconds(newdateExpired.getSeconds() + 15);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TIME, newdateExpired.toString());
                this._txtFullTextSearch.set_value(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_TEXT));
                let documentUnits: DocumentUnitModel[] = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_ST_DOCUMENTUNIT_DATA));
                this.loadResult(documentUnits);
            }
        }
    }

    private _registerEnvironmentImages(): void {
        this._environmentImageDictionary[Environment.Protocol] = "../Comm/Images/DocSuite/Protocollo16.png";
        this._environmentImageDictionary[Environment.Resolution] = "../Comm/Images/DocSuite/Atti16.png";
        this._environmentImageDictionary[Environment.DocumentSeries] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
        this._environmentImageDictionary[Environment.UDS] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
    }

    private getIconUrl(env: Environment): string {
        return this._environmentImageDictionary[env] ?
            this._environmentImageDictionary[env] : "../App_Themes/DocSuite2008/imgset16/document.png";
    }
}
export = FullTextSearch;