import PECInvoiceBase = require("PEC/PECInvoiceBase");
import ServiceConfigurationModel = require("App/Services/ServiceConfiguration");
import TenantSearchFilterDTO = require('App/DTOs/TenantSearchFilterDTO');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');

class uscTenantsSelector extends PECInvoiceBase {

    cmbSelectAziendaId: string;
    cmbSelectPecMailBoxId: string;
    cmbWorkflowRepositoriesId: string;
    btnContainerSelectorOkId: string;

    private _cmbSelectAzienda: Telerik.Web.UI.RadComboBox;
    private _cmbSelectPecMailBox: Telerik.Web.UI.RadComboBox;
    private _cmbWorkflowRepositories: Telerik.Web.UI.RadComboBox;
    private _btnContainerSelectorOk: Telerik.Web.UI.RadButton;

    constructor(serviceConfigurations: ServiceConfigurationModel[]) {
        super(serviceConfigurations);
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    initialize(): void {
        super.initialize();
        this._btnContainerSelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnContainerSelectorOkId);
        this._cmbSelectAzienda = <Telerik.Web.UI.RadComboBox>$find(this.cmbSelectAziendaId);
        this._cmbSelectAzienda.add_selectedIndexChanged(this.cmbSelectAzienda_onClick);
        this._cmbSelectPecMailBox = <Telerik.Web.UI.RadComboBox>$find(this.cmbSelectPecMailBoxId);
        this._cmbSelectPecMailBox.add_selectedIndexChanged(this.cmbSelectPecMailBox_onClick);
        this._cmbWorkflowRepositories = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowRepositoriesId);
        this._cmbWorkflowRepositories.add_selectedIndexChanged(this._cmbWorkflowRepositories_onClick);

        let searchDTO: TenantSearchFilterDTO = new TenantSearchFilterDTO();
        this.loadResults(searchDTO);
        this._cmbSelectPecMailBox.disable();
        this._cmbWorkflowRepositories.disable();
    }

    private loadResults(searchDTO: TenantSearchFilterDTO) {
        var comboItem = null;
        this._tenantService.getTenants(searchDTO,
            (data) => {
                if (!data) return;
                $.each(data,
                    (idx, con) => {
                        comboItem = new Telerik.Web.UI.RadComboBoxItem();
                        comboItem.set_text(con.CompanyName);
                        comboItem.set_value(con.UniqueId);
                        this._cmbSelectAzienda.get_items().add(comboItem);
                    });
            });
    }

    cmbSelectAzienda_onClick = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this._cmbSelectPecMailBox.clearSelection();
        this._cmbWorkflowRepositories.clearSelection();
        this.pupulatePECMailBoxes(sender._value);
        this.populateWorkflowRepositories(sender._value);
    }

    cmbSelectPecMailBox_onClick = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        if (this._cmbWorkflowRepositories.get_selectedItem())
            this._btnContainerSelectorOk.set_enabled(true);
    }

    _cmbWorkflowRepositories_onClick = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        if (this._cmbSelectPecMailBox.get_selectedItem())
            this._btnContainerSelectorOk.set_enabled(true);
    }

    private pupulatePECMailBoxes(tenantId) {
        this._cmbSelectPecMailBox.get_items().clear();
        var comboItem = null;
        this._tenantService.getTenantPECMailBoxes(tenantId,
            (data: PECMailBoxModel[]) => {
                if (data === undefined || data.length < 1) {
                    this._cmbSelectPecMailBox.disable();
                    this._btnContainerSelectorOk.set_enabled(false);
                    return
                } else {
                    $.each(data,
                        (idx, con) => {
                            comboItem = new Telerik.Web.UI.RadComboBoxItem();
                            comboItem.set_text(con.MailBoxRecipient);
                            comboItem.set_value(con.EntityShortId);
                            this._cmbSelectPecMailBox.get_items().add(comboItem);
                            this._cmbSelectPecMailBox.enable();
                        });
                }
            });
    }

    private populateWorkflowRepositories(tenantId) {
        this._cmbWorkflowRepositories.get_items().clear();
        var comboItem = null;
        this._tenantService.getTenantWorkflowRepositories(tenantId,
            (data) => {
                if (data === undefined || data.length < 1) {
                    this._cmbWorkflowRepositories.disable();
                    this._btnContainerSelectorOk.set_enabled(false);
                    return;
                } else {
                    $.each(data,
                        (idx, con) => {
                            comboItem = new Telerik.Web.UI.RadComboBoxItem();
                            comboItem.set_text(con.WorkflowRepository.Name);
                            comboItem.set_value(con.WorkflowRepository.UniqueId);
                            this._cmbWorkflowRepositories.get_items().add(comboItem);
                            this._cmbWorkflowRepositories.enable();

                        });
                }
            });
    }

    onClientShow() {
        this._cmbSelectAzienda.clearSelection();
        this._cmbSelectPecMailBox.clearSelection();
        this._cmbWorkflowRepositories.clearSelection();
        this._cmbSelectPecMailBox.disable();
        this._cmbWorkflowRepositories.disable();
        this._btnContainerSelectorOk.set_enabled(false);
    }


}

export = uscTenantsSelector; 