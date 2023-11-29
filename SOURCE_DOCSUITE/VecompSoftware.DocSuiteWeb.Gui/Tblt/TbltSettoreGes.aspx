<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltSettoreGes" CodeBehind="TbltSettoreGes.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <style type="text/css" media="all">
        .column_center_position {
            text-align: center !important;
        }
    </style>

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(roleOperation, roleID, nodeType) {
                var oRole = new Object();
                oRole.Operation = roleOperation;
                oRole.ID = roleID;
                oRole.NodeType = nodeType;
                HideLoadingPanel();
                GetRadWindow().close(oRole);
            }

            function MutuallyExclusive(gridId, radio, rowId) {
                var masterTable = $find(gridId.id).get_masterTableView();
                masterTable.selectItem(masterTable.get_dataItems()[rowId].get_element());

                var currentRdbId = radio.id;
                var items = document.getElementById(gridId.id).getElementsByTagName('input');
                for (var i = 0; i < items.length; i++) {
                    if (items[i].type == "radio") {
                        if (items[i].id != currentRdbId) items[i].checked = false;
                    }
                }
            }

            function RowGridDeselected(sender, args) {
                var item = args.get_gridDataItem().get_cell("RadioButtonTemplateColumn");
                var elements = item.getElementsByTagName('input');
                for (var i = 0; i < elements.length; i++) {
                    if (elements[i].type == "radio") {
                        if (elements[i].checked == true)
                            elements[i].checked = false;
                    }
                }
            }

            function ShowLoadingPanel() {
                var ajaxLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var loadingPanel = "<%= pnlRinomina.ClientID%>";
                ajaxLoadingPanel.show(loadingPanel);
            }

            function HideLoadingPanel() {
                var ajaxLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
            var loadingPanel = "<%= pnlRinomina.ClientID%>";
                ajaxLoadingPanel.hide(loadingPanel);
            }

            function OnClientClicking(sender, args) {
                var windowManager = $find("<%= RadWindowManagerRoleGes.ClientID %>");
                var actionName = "<%= Action.ToLower() %>";
                switch (actionName) {
                    case "recovery":
                    case "delete":
                        var callBackFunction = Function.createDelegate(sender, function (shouldSubmit) {
                            if (shouldSubmit) {
                                if (actionName == "recovery") {
                                    sender.set_commandArgument('activateAllChildren');
                                } else {
                                    sender.set_commandArgument('disableAllChildren');
                                }
                            }
                            this.click();
                        });
                        windowManager.radconfirm("Vuoi che l'operazione sia gestita anche su tutti i figli del settore selezionato?", callBackFunction, 300, 100, null, "Gestione");
                        args.set_cancel(true);
                        break;
                    case "clone":
                        var cloneCallBackFunction = Function.createDelegate(sender, function (shouldSubmit) {
                            if (shouldSubmit) {
                                this.click();
                            }
                        });
                        windowManager.radconfirm("Vuoi procedere a clonare massivamente il settore selezionato?", cloneCallBackFunction, 300, 100, null, "Clona");
                        args.set_cancel(true);
                        break;
                }
            }


        </script>
    </telerik:RadScriptBlock>

    <telerik:RadTreeView EnableViewState="false" ID="RadTreeViewSelectedRole" runat="server" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerRoleGes" runat="server" />
    <asp:Panel ID="pnlRinomina" runat="server" Width="100%">
        <table class="datatable">
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblOldName" Text="Nome:" Font-Bold="true" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtOldName" runat="server" MaxLength="100" Width="40%" Enabled="False" />
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblOldMail" Text="Email:" Font-Bold="true" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtOldEmail" runat="server" MaxLength="100" Width="100%" Enabled="False" />
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblOldServ" Text="Codice servizio:" Font-Bold="true" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtOldServ" runat="server" MaxLength="100" Width="40%" Enabled="False" />
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblNewNome" Text="Nome:" Font-Bold="true" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtNewNome" runat="server" MaxLength="100" Width="40%" />
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblNewMail" Text="Email:" Font-Bold="true" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtNewMail" runat="server" MaxLength="254" Width="100%" />
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label Font-Bold="true" ID="lblNewServ" runat="server" Text="Codice servizio:" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtNewServ" MaxLength="100" runat="server" Width="100%" />
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label Font-Bold="true" ID="Label2" runat="server" Text="Indirizzo Sharepoint:" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtOldSharepoint" MaxLength="100" runat="server" Width="100%" />
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label Font-Bold="true" ID="lblCollapsed" runat="server" Text="Nodo compresso:" />
                </td>
                <td width="80%">
                    <asp:CheckBox ID="cbCollapsed" MaxLength="100" runat="server" Width="100%" />
                </td>
            </tr>
            <tr id="trPEC" runat="server">
                <td class="label" width="20%" style="vertical-align: top;">
                    <asp:Label Font-Bold="true" ID="lblPECMails" runat="server" Text="Indirizzi di posta certificata:" />
                </td>
                <td width="80%">
                    <div>
                        <telerik:RadGrid runat="server" ID="grdMailboxes" Width="100%" AllowMultiRowSelection="true">
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id">
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="Select" HeaderText="Casella Abilitata">
                                        <HeaderStyle Width="16px"></HeaderStyle>
                                        <ItemStyle CssClass="column_center_position"></ItemStyle>
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="30px" HeaderText="Predefinita">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle CssClass="column_center_position" />
                                        <ItemTemplate>
                                            <asp:RadioButton runat="server" ID="rbtDefaultMailBox" AutoPostBack="false" GroupName="defaultMailBox" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="MailBoxName" HeaderText="Nome Casella">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblMailboxName" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings EnableRowHoverStyle="False">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                                <ClientEvents OnRowDeselected="RowGridDeselected"></ClientEvents>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </div>
                </td>
            </tr>
            <tr id="trProtocolBox" runat="server">
                <td class="label" width="20%" style="vertical-align: top;">
                    <asp:Label Font-Bold="true" ID="lblProtocolBoxes" runat="server" Text="Caselle di Protocollazione:" />
                </td>
                <td width="80%">
                    <div>
                        <telerik:RadGrid runat="server" ID="grdProtocolBoxes" Width="100%" AllowMultiRowSelection="true">
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id">
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="Select" HeaderText="Casella Abilitata">
                                        <HeaderStyle Width="16px"></HeaderStyle>
                                        <ItemStyle CssClass="column_center_position"></ItemStyle>
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="30px" HeaderText="Predefinita">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle CssClass="column_center_position" />
                                        <ItemTemplate>
                                            <asp:RadioButton runat="server" CssClass="column_center_position" ID="rbtDefaultMailBox" AutoPostBack="false" GroupName="defaultMailBox" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="MailBoxName" HeaderText="Nome Casella">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblMailboxName" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings EnableRowHoverStyle="False">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                                <ClientEvents OnRowDeselected="RowGridDeselected"></ClientEvents>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlInserimento" runat="server" Width="100%">
        <table class="datatable">
            <tr>
                <td class="label" width="20%">
                    <asp:Label Font-Bold="true" ID="lblName" runat="server" Text="Nome:" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtName" MaxLength="100" runat="server" Width="40%" />
                </td>
            </tr>
            <tr id="trEmailNew" runat="server">
                <td class="label" width="20%">
                    <asp:Label Font-Bold="true" ID="lblEmail" runat="server" Text="Email:" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtEmail" MaxLength="100" runat="server" Width="100%" />
                </td>
            </tr>
            <tr id="trServiceCodeNew" runat="server">
                <td class="label" width="20%">
                    <asp:Label Font-Bold="true" ID="lblServiceCod" runat="server" Text="Codice servizio:" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtServiceCod" MaxLength="100" runat="server" Width="100%" />
                </td>
            </tr>
            <tr id="trSharepointNew" runat="server">
                <td class="label" width="20%">
                    <asp:Label Font-Bold="true" ID="lblUriSharepoint" runat="server" Text="Indirizzo Sharepoint:" />
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtUriSharepoint" MaxLength="100" runat="server" Width="100%" />
                </td>
            </tr>
            <tr id="trCollapseNew" runat="server">
                <td class="label" width="20%">
                    <asp:Label Font-Bold="true" ID="lblNewCollapsed" runat="server" Text="Nodo compresso:" />
                </td>
                <td width="80%">
                    <asp:CheckBox ID="cbNewCollapsed" MaxLength="100" runat="server" Width="100%" />
                </td>
            </tr>

            <tr id="trNewPEC" runat="server">
                <td class="label" width="20%" style="vertical-align: top;">
                    <asp:Label Font-Bold="true" ID="lblNewPECMails" runat="server" Text="Indirizzi di posta certificata:" />
                </td>
                <td width="80%">
                    <div>
                        <telerik:RadGrid runat="server" ID="grdMailboxesNew" Width="100%" AllowMultiRowSelection="true">
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id">
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="Select" HeaderText="Casella Abilitata">
                                        <HeaderStyle Width="16px"></HeaderStyle>
                                        <ItemStyle CssClass="column_center_position"></ItemStyle>
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderText="Predefinita">
                                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                                        <ItemStyle CssClass="column_center_position"></ItemStyle>
                                        <ItemTemplate>
                                            <asp:RadioButton runat="server" ID="rbtDefaultMailBox" AutoPostBack="false" GroupName="defaultMailBox" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="MailBoxName" HeaderText="Nome Casella">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblMailboxName" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings EnableRowHoverStyle="False">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </div>
                </td>
            </tr>
            <tr id="trProtocolBoxNew" runat="server">
                <td class="label" width="20%" style="vertical-align: top;">
                    <asp:Label Font-Bold="true" ID="lblProtocolBoxesNew" runat="server" Text="Caselle di Protocollazione:" />
                </td>
                <td width="80%">
                    <div>
                        <telerik:RadGrid runat="server" ID="grdProtocolBoxesNew" Width="100%" AllowMultiRowSelection="true">
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id">
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="Select" HeaderText="Casella Abilitata">
                                        <HeaderStyle Width="16px"></HeaderStyle>
                                        <ItemStyle CssClass="column_center_position"></ItemStyle>
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" HeaderText="Predefinita">
                                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                                        <ItemStyle CssClass="column_center_position"></ItemStyle>
                                        <ItemTemplate>
                                            <asp:RadioButton runat="server" ID="rbtDefaultMailBox" AutoPostBack="false" GroupName="defaultMailBox" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="MailBoxName" HeaderText="Nome Casella">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblMailboxName" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings EnableRowHoverStyle="False">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                                <ClientEvents OnRowDeselected="RowGridDeselected"></ClientEvents>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </div>
                </td>
            </tr>

        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConferma" runat="server" Text="Conferma" OnClientClicking="OnClientClicking" />
    <asp:Label ID="ErrLabel" runat="server" Style="color: Red;" Text="" Visible="false" />
</asp:Content>
