<%@ Page Title=" " Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltSMSPecNotification.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltSMSPecNotification" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function  <%= Me.ID %>_CloseWindow(sender, args) {
                sender.remove_close(<%= Me.ID %>_CloseWindow);
                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|AddUser|" + args.get_argument());
                }
            }

            function  <%= Me.ID %>_SimpleWindowClose() {
                var window = $find('<%=windowAddMobilePhone.ClientID %>');
                window.close();
            }

            //richiamata quando viene selezionato conferma e nuovo e la finestra rubrica NON viene chiusa
            function  <%= Me.ID %>_AddUsersToControl(value) {
                if (value !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|AddUser|" + value);
                }
            }

            function <%= Me.ID %>_OpenWindow(url, name, parameters) {
                var wnd = $find(name);
                wnd.setUrl(url + "?" + parameters);
                wnd.set_destroyOnClose(true);
                wnd.show();
                wnd.center();
                return false;
            }

            function <%= Me.ID %>_SimpleOpenWindow(name) {
                var wnd = $find(name);
                wnd.set_destroyOnClose(true);
                wnd.show();
                wnd.center();
                return false;
            }

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pageSplitter.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pageSplitter.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function responseEnd(sender, eventArgs) {
                HideLoadingPanel();
            }
        </script>

        <style type="text/css">
            .pecRepeater {
                float: left;
                width: 100%;
                border-bottom: 1px solid #E6E6E6;
                border-bottom: none;
            }

                .pecRepeater a {
                    display: block;
                    padding: 10px 5px 10px 25px;
                    font-size: 11px;
                    text-decoration: none !important;
                    color: #000000 !important;
                    border-bottom: 1px solid #E6E6E6;
                    background: url('../App_Themes/DocSuite2008/imgset16/mail_box.png') no-repeat;
                    background-position: 3px center;
                }

                    .pecRepeater a:hover,
                    .pecRepeater a.selected {
                        background-color: #FFCB60;
                    }

            .titleLabel {
                height: 20px;
                vertical-align: middle;
                text-align: left;
                line-height: 20px;
                font-weight: bold;
                font-size: 11px;
                background-image: none;
                border-bottom: 1px solid #999999;
                color: navy;
                background-color: #dae9fe;
            }

            .rgDataDiv {
                height: auto !important;
            }
        </style>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerUpload" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowSelContact" ReloadOnShow="false" runat="server" Title="Selezione Utente" />
            <telerik:RadWindow Behaviors="Close" DestroyOnClose="true" ReloadOnShow="True" Height="230px" ID="windowAddMobilePhone" runat="server" Title="Aggiungi nuovo numero di cellulare" Width="400px">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlEditorWindow" Width="100%">
                        <asp:Panel runat="server" ID="WarningPanel" CssClass="warningAreaLow">
                            <asp:Label ID="WarningLabel" runat="server" Text="Nessun numero di cellullare presente per l'utente selezionato. Inserire un nuovo numero." />
                        </asp:Panel>
                        <div style="margin-top: 10px;">
                            +39<telerik:RadTextBox MaxLength="15" runat="server" ID="txtMobilePhone"></telerik:RadTextBox>
                            <p>
                                <asp:RequiredFieldValidator runat="server" ID="mobilePhoneRequiredValidator" Style="font-size: 12px !important;" Display="Dynamic" ValidationGroup="mobilePhoneValidationGroup"
                                    ErrorMessage="Inserire un numero di cellulare valido."
                                    ControlToValidate="txtMobilePhone" />
                            </p>
                            <p>
                                <asp:RegularExpressionValidator runat="server" ID="mobilePhoneValidator" Display="Dynamic" ValidationGroup="mobilePhoneValidationGroup"
                                    ErrorMessage="Inserire un numero di cellulare valido."
                                    ControlToValidate="txtMobilePhone"
                                    ValidationExpression="^\d+$" />
                            </p>
                            <p>
                                <asp:RegularExpressionValidator runat="server" ID="mobilePhoneRangeValidator" Display="Dynamic" ValidationGroup="mobilePhoneValidationGroup"
                                    ErrorMessage="Il numero di telefono deve essere compreso tra 6 e 15 caratteri."
                                    ControlToValidate="txtMobilePhone"
                                    ValidationExpression="\s*[0-9]{6,15}\s*" />
                            </p>
                            <p>
                                <asp:Label runat="server" Text="Attenzione! Il numero di cellulare dovrà essere inserito SENZA il prefisso internazionale (es. 0039 o +39)"></asp:Label>
                            </p>
                        </div>
                    </asp:Panel>
                    <div class="window-footer-wrapper">
                        <telerik:RadButton runat="server" ID="btnSaveMobilePhone" Text="Conferma" ValidationGroup="mobilePhoneValidationGroup" />
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadSplitter runat="server" ID="pageSplitter" Width="100%" ResizeWithParentPane="False" Height="100%">
        <telerik:RadPane runat="server" ID="pecPane" Height="100%" Width="40%" Scrolling="Y">
            <asp:Label runat="server" ID="lblPecPaneTitle" CssClass="titleLabel" Text="Elenco PEC" Width="100%" />
            <asp:Panel runat="server" ID="pnlPecList" CssClass="pecRepeater">
                <asp:Repeater runat="server" ID="pecRepeater">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="pecLink" CommandName="ShowUsers" OnClientClick="ShowLoadingPanel()" CommandArgument='<%# Eval("Id")%>'>
                            <%# Eval("Name") %></asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>
        </telerik:RadPane>

        <telerik:RadSplitBar runat="server" ID="splitterBar"></telerik:RadSplitBar>

        <telerik:RadPane runat="server" ID="userPane" Width="60%" Scrolling="Y">
            <asp:Label runat="server" ID="lblUserPaneTitle" CssClass="titleLabel" Text="Utenti Notificati" Width="100%" />
            <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton runat="server" CausesValidation="False" Value="btnAddUser" CommandName="btnAddUser" ToolTip="Aggiungi nuovo utente" ImageUrl="../App_Themes/DocSuite2008/imgset16/user_add.png" />
                </Items>
            </telerik:RadToolBar>
            
            <div style="overflow:hidden;" runat="server">
                <DocSuite.WebComponent:BindGrid EnableExportButtons="False" EnableClearFilterButtons="False" EnableHeaderSection="False" runat="server" ID="dgvUserConfiguration" AutoGenerateColumns="False" AllowPaging="True" CellSpacing="0" GridLines="None">
                    <MasterTableView DataKeyNames="Id" NoMasterRecordsText="Nessun utente trovato" ShowFooter="False" ShowHeader="True" TableLayout="Fixed">
                        <Columns>

                            <telerik:GridTemplateColumn HeaderText="Utenti notificati" AllowSorting="True" ShowSortIcon="True">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="45%" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="30px" />
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAccountName"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn HeaderText="N. Cellulare" AllowSorting="True" ShowSortIcon="True">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="40%" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMobilePhone"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn HeaderText="" AllowSorting="True" ShowSortIcon="True">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="100px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <telerik:RadButton ToolTip="Modifica numero di Cellulare" runat="server" ID="btnUpdateContactMobilePhone" OnClientClicking="ShowLoadingPanel" CommandName="UpdateUser" ButtonType="LinkButton" Height="16px" Width="16px">
                                        <Image ImageUrl="../Comm/Images/Interop/Manuale.gif" />
                                    </telerik:RadButton>
                                    <telerik:RadButton ToolTip="Elimina Utente" runat="server" Style="margin-left: 4px;" ID="btnDeleteUser" OnClientClicking="ShowLoadingPanel" CommandName="DeleteUser" ButtonType="LinkButton" Height="16px" Width="16px">
                                        <Image ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" />
                                    </telerik:RadButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                        </Columns>
                    </MasterTableView>
                    <ClientSettings EnablePostBackOnRowClick="False">
                    </ClientSettings>
                </DocSuite.WebComponent:BindGrid>
            </div>
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
