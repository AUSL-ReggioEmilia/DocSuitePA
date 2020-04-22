
class CommonSelContactOmniBus {
    txtNomeId: string;
    txtCognomeId: string;
    rcbDistrettoId: string;
    txtCdcId: string;
    btnFindId: string;
    btnFindUniqueId: string;
    rtvResultsId: string;
    btnConfirmId: string;
    btnConfirmAndNewId: string;
    ajaxManagerId: string;
    callerId: string;

    private _txtNome: Telerik.Web.UI.RadTextBox;
    private _txtCognome: Telerik.Web.UI.RadTextBox;
    private _rcbDistretto: Telerik.Web.UI.RadComboBox;
    private _txtCdc: Telerik.Web.UI.RadTextBox;
    private _btnFind: Telerik.Web.UI.RadButton;
    private _rtvResults: Telerik.Web.UI.RadTreeView;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _btnConfirmAndNew: Telerik.Web.UI.RadButton;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor() {
        
    }

    /**
     * Inizializzazione della classe
     */
    initialize(): void {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._txtNome = <Telerik.Web.UI.RadTextBox>$find(this.txtNomeId);
        this._txtCognome = <Telerik.Web.UI.RadTextBox>$find(this.txtCognomeId);
        this._rcbDistretto = <Telerik.Web.UI.RadComboBox>$find(this.rcbDistrettoId);
        this._txtCdc = <Telerik.Web.UI.RadTextBox>$find(this.txtCdcId);
        this._btnFind = <Telerik.Web.UI.RadButton>$find(this.btnFindId);
        this._btnFind.add_clicked(this.btnFind_onClick);

        this._rtvResults = <Telerik.Web.UI.RadTreeView>$find(this.rtvResultsId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_onClick);

        this._btnConfirmAndNew = <Telerik.Web.UI.RadButton>$find(this.btnConfirmAndNewId);
        this._btnConfirmAndNew.add_clicked(this.btnConfirmAndNew_onClick);

        this._txtNome.focus();
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante di ricerca contatti
     * @param sender
     * @param args
     */
    private btnFind_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs): void => {
        this._ajaxManager.ajaxRequestWithTarget(this.btnFindUniqueId, '');
    }

    /**
     * Evento scatenato al click del pulsante di conferma
     * @param sender
     * @param args
     */
    private btnConfirm_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs): void => {
        this._rtvResults = <Telerik.Web.UI.RadTreeView>$find(this.rtvResultsId);
        let selectedContact: Telerik.Web.UI.RadTreeNode = this._rtvResults.get_selectedNode();
        if (selectedContact == undefined || selectedContact.get_value() == "root") {
            alert("Nessun contatto selezionato");
            return;
        }

        let serializedContact: string = selectedContact.get_attributes().getAttribute("serializedModel");
        this.returnValuesJson(serializedContact);
    }

    /**
     * Evento scatenato al click del pulsante di conferma e nuovo
     * @param sender
     * @param args
     */
    private btnConfirmAndNew_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs): void => {
        this._rtvResults = <Telerik.Web.UI.RadTreeView>$find(this.rtvResultsId);
        let selectedContact: Telerik.Web.UI.RadTreeNode = this._rtvResults.get_selectedNode();
        if (selectedContact == undefined || selectedContact.get_value() == "root") {
            alert("Nessun contatto selezionato");
            return;
        }

        let serializedContact: string = selectedContact.get_attributes().getAttribute("serializedModel");
        this.returnValuesJsonAndNew(serializedContact);
    }
    

    /**
     *------------------------- Methods -----------------------------
     */

    private returnValuesJson(serializedContact: string): void {
        this.closeWindow(serializedContact);
        return;
    }

    private returnValuesJsonAndNew(serializedContact): void {
        (<any>window).GetRadWindow().BrowserWindow[this.callerId.concat("_AddUsersToControl")](serializedContact);
    }

    private closeWindow(contact): void {
        (<any>window).GetRadWindow().close(contact);
    }
}

export = CommonSelContactOmniBus;