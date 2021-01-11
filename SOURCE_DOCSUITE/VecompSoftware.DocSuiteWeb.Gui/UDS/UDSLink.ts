import AjaxModel = require('App/Models/AjaxModel');

class UDSLink {
    btnConnectId: string;
    currentUDSRepositoryId: string;
    currentIdUDS: string;
    selectedUDSRepositoryId: string;
    ajaxManagerId: string;
    currentAction: string;

    private UDS_RESULTS_PAGE_URL: string = "UDSResults.aspx?Type=UDS&isFromUDSLink=True";
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;


    private _btnConnect: Telerik.Web.UI.RadButton;

    //default constructor
    constructor() {

    }

    initialize() {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnConnect = <Telerik.Web.UI.RadButton>$find(this.btnConnectId);
        if (this._btnConnect) {
            this._btnConnect.add_clicking(this._btnConnect_onClick);
        }
    }

    _btnConnect_onClick = () => {
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.ActionName = "CurentFinderChanged";
        ajaxModel.Value.push(this.selectedUDSRepositoryId);
        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
        window.location.href = `${this.UDS_RESULTS_PAGE_URL}&IdUDS=${this.currentIdUDS}&IdUDSRepository=${this.selectedUDSRepositoryId}&Action=${this.currentAction}`;
    }    
}

export = UDSLink;