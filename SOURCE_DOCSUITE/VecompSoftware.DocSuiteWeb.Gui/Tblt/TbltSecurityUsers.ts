import UscErrorNotification = require('UserControl/uscErrorNotification');

class TbltSecurityUsers {
    radWindowManagerGroupsId: string;
    rtvUsersId: string;
    rtvGroupsId: string;
    uscNotificationId: string;
    ajaxManagerId: string;
    actionsToolbarId: string;

    private _wndManager: Telerik.Web.UI.RadWindowManager;
    private _rtvUsers: Telerik.Web.UI.RadTreeView;
    private _rtvGroups: Telerik.Web.UI.RadTreeView;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _actionToolbar: Telerik.Web.UI.RadToolBar;

    private static ADD_COMMANDNAME = "AddUser";
    private static DELETEUSER_COMMANDNAME = "DeleteUser";
    private static COPYFROMUSER_COMMANDNAME = "CopyFromUser";
    private static GROUPSADD_COMMANDNAME = "AddGroups";
    private static GUIDEDGROUPSADD_COMMANDNAME = "GuidedGroupsAdd";
    private static DELETE_COMMANDNAME = "Delete";

    private get toolbarActions(): Array<[string, () => void]> {
        let items: Array<[string, () => void]> = [
            [TbltSecurityUsers.ADD_COMMANDNAME, () => this.btnAddUser_OnClick()],
            [TbltSecurityUsers.DELETEUSER_COMMANDNAME, () => this.btnDeleteUser_OnClick()], 
            [TbltSecurityUsers.COPYFROMUSER_COMMANDNAME, () => this.btnCopyFromUser_OnClick()],
            [TbltSecurityUsers.GROUPSADD_COMMANDNAME, () => this.btnGroupsAdd_OnClick()],
            [TbltSecurityUsers.GUIDEDGROUPSADD_COMMANDNAME, () => this.btnGuidedGroupsAdd_OnClick()],
            [TbltSecurityUsers.DELETE_COMMANDNAME, () => this.btnDelete_OnClick()]
        ];
        return items;
    }

    /**
* Costruttore
* @param serviceConfigurations
*/
    constructor() {
        $(document).ready(() => {

        });
    }

    /**
   * Inizializzazione classe
   */
    initialize() {
        this._actionToolbar = $find(this.actionsToolbarId) as Telerik.Web.UI.RadToolBar;
        this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);

        this._wndManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerGroupsId);
        this._wndManager.getWindowByName('windowGroupsAdd').add_close(this.onCloseWindowGroups);
        this._wndManager.getWindowByName('windowUserAdd').add_close(this.onCloseWindowUser);
        this._wndManager.getWindowByName('windowCopyFromUser').add_close(this.onCloseWindowCopyFromUser);
        this._rtvUsers = <Telerik.Web.UI.RadTreeView>$find(this.rtvUsersId);
        this._rtvGroups = <Telerik.Web.UI.RadTreeView>$find(this.rtvGroupsId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);         
    }


    /**
   *------------------------- Events -----------------------------
   */

    initializeDetailsCallback = (nodeType: string) => {
        if (nodeType && nodeType == 'Root') {
            this._updateActionButtonsVisibility(false);
            return;
        }
        this._updateActionButtonsVisibility(true);
    }

    /**
     * Evento alla chiusura della finestra di copia da utente
     * @param sender
     * @param args
     */
    onCloseWindowCopyFromUser = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            this._ajaxManager.ajaxRequest('copyFromUser|' + args.get_argument());
        }
    }

    /**
     * Evento alla chiusura della finestra di inserimento utente
     * @param sender
     * @param args
     */
    onCloseWindowUser = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            this._ajaxManager.ajaxRequest('users|' + args.get_argument());
        }
    }

    /**
   * Evento alla chiusura della finestra di assegnazione gruppi
   * @param sender
   * @param args
   */
    onCloseWindowGroups = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            this._ajaxManager.ajaxRequest('groups|' + args.get_argument());
        }
    }

    /**
     * Evento scatenato al click del bottone copia da utente
     * @param sender
     * @param args
     */
    btnCopyFromUser_OnClick = () => {
        this.openCopyFromUserWindow();
    }

    /**
     * Evento scatenato al click del bottone aggiungi utente 
     * @param sender
     * @param args
     */
    btnAddUser_OnClick = () => {
        this.openUserWindow();
    }

    /**
     * Evento scatenato al click del bottone elimina utente
     * @param sender
     * @param args
     */
    btnDeleteUser_OnClick = () => {
        this._rtvUsers = <Telerik.Web.UI.RadTreeView>$find(this.rtvUsersId);
        if (!this._rtvUsers.get_selectedNode() || !this._rtvUsers.get_selectedNode().get_value()) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare un utente.");
            return;
        }

        this._wndManager.radconfirm("Sei sicuro di voler rimuovere l'utente da tutti i gruppi?", (arg) => {
            if (arg) {
                this._ajaxManager.ajaxRequest('deleteuser');
            }
        }, 300, 160);
    }

    /**
    * Evento scatenato al click del bottone Assegna gruppi
    * @param sender
    * @param args
    */
    btnGroupsAdd_OnClick = () => {
        this.openGroupsWindow();
    }

    /**
    * Evento scatenato al click del bottone Rimuovi dai gruppi
    * @param sender
    * @param args
    */
    btnDelete_OnClick = () => {
        this._rtvUsers = <Telerik.Web.UI.RadTreeView>$find(this.rtvUsersId);
        if (!this._rtvUsers.get_selectedNode() || !this._rtvUsers.get_selectedNode().get_value()) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare un utente.");
            return;
        }
        this._rtvGroups = <Telerik.Web.UI.RadTreeView>$find(this.rtvGroupsId);
        if (!this._rtvGroups.get_checkedNodes() || this._rtvGroups.get_checkedNodes().length == 0) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare almeno un gruppo.");
            return;
        }

        this._wndManager.radconfirm("Sei sicuro di voler rimuovere l'utente?", (arg) => {
            if (arg) {
                this._ajaxManager.ajaxRequest('delete');
            }
        }, 300, 160);
    }

    /**
    * Evento scatenato al click del bottone Configurazione guidata
    * @param sender
    * @param args
    */
    btnGuidedGroupsAdd_OnClick = () => {
        this._rtvUsers = <Telerik.Web.UI.RadTreeView>$find(this.rtvUsersId);
        if (!this._rtvUsers.get_selectedNode() || !this._rtvUsers.get_selectedNode().get_value()) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare un utente.");
            return;
        }
        let selectedUser: Telerik.Web.UI.RadTreeNode = this._rtvUsers.get_selectedNode();
        if (selectedUser) {
            window.location.href = "../Tblt/TbltSecurityGroupWizard.aspx?Type=Comm&DomainAD=".concat(selectedUser.get_attributes().getAttribute('Domain'), "&AccountAD=", selectedUser.get_attributes().getAttribute('Account'));
        }
    }

    /*
     * ------------------------------- Methods ----------------------------------
     */


    /**
     * Metodo di apertura nuova finestra
     * @param url
     * @param name
     * @param width
     * @param height
     */
    openWindow(url: string, name: string, width: number, height: number): boolean {
        let wnd: Telerik.Web.UI.RadWindow = this._wndManager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.center();
        return false;
    }

    /**
     * Metodo di apertura finestra aggiunta utente
     */
    openUserWindow = () => {
        let url = "../Comm/SelUsers.aspx?Type=Comm";
        this.openWindow(url, "windowUserAdd", 700, 600);
    }

    /**
     * Metodo di apertura finestra aggiunta utente
     */
    openCopyFromUserWindow = () => {
        let url = "../Comm/SelUsers.aspx?Type=Comm";
        this.openWindow(url, "windowCopyFromUser", 700, 600);
    }
    /**
     * Metodo di apertura finestra assegnazione gruppi
     */
    openGroupsWindow = () => {
        this._rtvUsers = <Telerik.Web.UI.RadTreeView>$find(this.rtvUsersId);
        if (!this._rtvUsers.get_selectedNode() || !this._rtvUsers.get_selectedNode().get_value()) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare un utente.");
            return;
        }

        let url = "../UserControl/uscMultiSelGroups.aspx?Type=Comm";
        this.openWindow(url, 'windowGroupsAdd', 500, 600);
    }

    private _updateActionButtonsVisibility(isVisible: boolean): void {
        let toolbarBtnDisplayMode: string = isVisible ? "inline-block" : "none";
        this._actionToolbar.get_items().forEach((item: Telerik.Web.UI.RadToolBarButton) => {
            let btnCommandName: string = item.get_commandName();

            if (btnCommandName === TbltSecurityUsers.ADD_COMMANDNAME || btnCommandName === TbltSecurityUsers.DELETEUSER_COMMANDNAME) {
                return;
            }

            item.get_element().style.display = toolbarBtnDisplayMode;
        });
    }

    protected actionToolbar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let currentActionButtonItem: Telerik.Web.UI.RadToolBarButton = args.get_item() as Telerik.Web.UI.RadToolBarButton;
        let currentAction: () => void = this.toolbarActions.filter((item: [string, () => void]) => item[0] == currentActionButtonItem.get_commandName())
            .map((item: [string, () => void]) => item[1])[0];
        currentAction();
    }

    protected showWarningMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }

}

export = TbltSecurityUsers