import DossierBase = require('Dossiers/DossierBase');
import DossierSearchFilterDTO = require("App/DTOs/DossierSearchFilterDTO");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscDossierGrid = require('UserControl/uscDossierGrid');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');
import AjaxModel = require('App/Models/AjaxModel');
import DossierGridViewModel = require('App/ViewModels/Dossiers/DossierGridViewModel');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import MetadataValueViewModel = require('App/ViewModels/Metadata/MetadataValueViewModel');

class DossierRisultati extends DossierBase {

    uscDossierGridId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    ajaxManagerId: string;
    dynamicMetadataEnabled: boolean;

    dossierModel: DossierGridViewModel[];

    private _serviceConfigurations: ServiceConfiguration[];
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;

    /**
    * Costruttore
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.uscDossierGridId);
        $("#".concat(this.uscDossierGridId)).bind(UscDossierGrid.LOADED_EVENT, () => {
            this.loadDossierGrid();
        });
        this.loadDossierGrid();

        $("#".concat(this.uscDossierGridId)).bind(UscDossierGrid.PAGE_CHANGED_EVENT, (args) => {
            let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$("#".concat(this.uscDossierGridId)).data();
            if (!jQuery.isEmptyObject(uscDossierGrid)) {
                this.pageChange(uscDossierGrid);
            }
        });
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);

    }


    private loadDossierGrid() {
        let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$("#".concat(this.uscDossierGridId)).data();
        if (!jQuery.isEmptyObject(uscDossierGrid)) {
            this.loadResults(uscDossierGrid, 0);
        }
    }

    private pageChange(uscDossierGrid: UscDossierGrid) {
        this._loadingPanel.show(this.uscDossierGridId);
        let skip = uscDossierGrid.getGridCurrentPageIndex() * uscDossierGrid.getGridPageSize();
        this.loadResults(uscDossierGrid, skip);
    }

    private loadResults(uscDossierGrid: UscDossierGrid, skip: number) {
        let top: number = skip + uscDossierGrid.getGridPageSize();

        let filter: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIER_SEARCH);
        let dossierSearchFilter: DossierSearchFilterDTO;
        if (filter) {
            dossierSearchFilter = <DossierSearchFilterDTO>JSON.parse(filter);
        }

        dossierSearchFilter.Skip = skip;
        dossierSearchFilter.Top = top;

        this.service.getAuthorizedDossiers(dossierSearchFilter, (data: DossierGridViewModel[]) => {
            if (!data) return;

            if (this.dynamicMetadataEnabled) {
                this.dossierModel = data;
                let ajaxModel: AjaxModel = <AjaxModel>{};
                ajaxModel.Value = [JSON.stringify(data)];
                ajaxModel.ActionName = "GenerateColumns";
                this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            } else {
                uscDossierGrid.setDataSource(data);
                this.service.countAuthorizedDossiers(dossierSearchFilter,
                    (data) => {
                        if (data == undefined) return;
                        uscDossierGrid.setItemCount(data);
                        this._loadingPanel.hide(this.uscDossierGridId);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.uscDossierGridId);
                        $("#".concat(this.uscDossierGridId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    }
                );
            }
        },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.uscDossierGridId);
                $("#".concat(this.uscDossierGridId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    generateColumnsCallback(data: string) {
        let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$("#".concat(this.uscDossierGridId)).data();
        let dossiers: DossierGridViewModel[] = this.dossierModel;

        let extendedDossierSource: any[] = dossiers;

        for (let dossier of extendedDossierSource) {
            let metadataColumns: BaseFieldViewModel[] = [];
            if (dossier.MetadataDesigner) {
                let metadataDesigner: MetadataDesignerViewModel = JSON.parse(dossier.MetadataDesigner)
                if (metadataDesigner) {
                    this.addMetadataColumns(metadataColumns, metadataDesigner.BoolFields.filter(x => x.ShowInResults));
                    this.addMetadataColumns(metadataColumns, metadataDesigner.ContactFields.filter(x => x.ShowInResults));
                    this.addMetadataColumns(metadataColumns, metadataDesigner.DateFields.filter(x => x.ShowInResults));
                    this.addMetadataColumns(metadataColumns, metadataDesigner.DiscussionFields.filter(x => x.ShowInResults));
                    this.addMetadataColumns(metadataColumns, metadataDesigner.EnumFields.filter(x => x.ShowInResults));
                    this.addMetadataColumns(metadataColumns, metadataDesigner.NumberFields.filter(x => x.ShowInResults));
                    this.addMetadataColumns(metadataColumns, metadataDesigner.TextFields.filter(x => x.ShowInResults));
                }

                if (dossier.MetadataValues) {
                    let metadataValuesModel: MetadataValueViewModel[] = JSON.parse(dossier.MetadataValues.replace(/\r?\n|\r/g, ""))

                    if (metadataColumns.length > 0 && metadataValuesModel) {
                        for (let metadataColumn of metadataColumns) {
                            if (metadataValuesModel.filter(x => x.KeyName == metadataColumn.KeyName)[0] && !dossier[metadataColumn.KeyName]) {
                                let value: string = metadataValuesModel.filter(x => x.KeyName == metadataColumn.KeyName)[0].Value;
                                if (metadataDesigner.BoolFields.filter(x => x.KeyName == metadataColumn.KeyName).length > 0) {
                                    value = value.toLowerCase() == "true" ? "Vero" : "Falso";
                                }
                                if (metadataDesigner.DateFields.filter(x => x.KeyName == metadataColumn.KeyName).length > 0) {
                                    value = moment(value).format("DD/MM/YYYY");
                                }
                                dossier[metadataColumn.KeyName] = value;
                            }
                        }
                    }
                }
            }
        }

        uscDossierGrid.setDataSource(extendedDossierSource);
        this._loadingPanel.hide(this.uscDossierGridId);
    }

    addMetadataColumns(metadataColumns: BaseFieldViewModel[], metadataDesignerFields: BaseFieldViewModel[]): void {
        for (let metadataDesignerField of metadataDesignerFields) {
            metadataColumns.push(metadataDesignerField);
        }
    }
}
export = DossierRisultati;