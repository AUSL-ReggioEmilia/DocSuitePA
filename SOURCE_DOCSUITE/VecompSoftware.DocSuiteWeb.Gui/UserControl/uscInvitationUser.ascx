<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscInvitationUser.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscInvitationUser" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function  <%= Me.ID %>_CloseWindow(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseWindow);
            if (args.get_argument() !== null) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|AddDomain|" + args.get_argument());
            }
        }

        function <%= Me.ID %>_OpenWindow(url, name, parameters, closeFunction) {
            var wnd = $find(name);
            wnd.setUrl(url + "?" + parameters);
            wnd.set_destroyOnClose(true);
            wnd.add_close(closeFunction);
            wnd.show();
            wnd.center();
            return false;
        }

        function  <%= Me.ID %>_AddUsersToControl(value) {
            if (value !== null) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|AddDomain|" + value);
            }
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerUpload" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelContact" ReloadOnShow="false" runat="server" Title="Selezione Utente" />
    </Windows>
</telerik:RadWindowManager>

<table class="datatable">
    <tr>
        <th>Utenti</th>
    </tr>
    <tr>
        <td>
            <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%" Height="29px">
                <Items>
                    <telerik:RadToolBarButton runat="server" CausesValidation="False" Value="btnAddUser" CommandName="btnAddUser" ToolTip="Partecipanti" ImageUrl="../App_Themes/DocSuite2008/imgset16/user_add.png" />
                    <telerik:RadToolBarButton runat="server" CausesValidation="False" Value="btnRemoveUser" CommandName="btnRemoveUser" ToolTip="Rimuovi partecipante" ImageUrl="../App_Themes/DocSuite2008/imgset16/user_delete.png" />
                </Items>
            </telerik:RadToolBar>

        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadGrid runat="server" ID="dgvInvitatedUser" AutoGenerateColumns="false" Width="100%" EnableEmbeddedSkins="false" Skin="DeskDocumentCustomSkin" AllowSorting="False" AllowPaging="False">
                <MasterTableView DataKeyNames="SerializeKey" AllowFilteringByColumn="False" TableLayout="Fixed">
                    <NoRecordsTemplate>
                        <div>Nessun utente selezionato</div>
                    </NoRecordsTemplate>
                    <Columns>
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Utente">
                            <HeaderStyle Width="40%"></HeaderStyle>
                            <ItemStyle Height="30px"></ItemStyle>
                            <ItemTemplate>
                                <asp:Image runat="server" ID="imgUser" ImageUrl="../App_Themes/DocSuite2008/imgset16/user.png" />
                                <asp:Label runat="server" ID="lblUserName" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Ruolo" UniqueName="invitationType">
                            <HeaderStyle Width="30%"></HeaderStyle>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblInvitationType" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Ruolo" UniqueName="invitationTypeMod">
                            <HeaderStyle Width="30%"></HeaderStyle>
                            <ItemTemplate>                                
                                <telerik:RadComboBox runat="server" Width="100%" ID="ddlInvitationType" AutoPostBack="True" CausesValidation="false" OnSelectedIndexChanged="DdlInvitationType_SelectedIndexChanged" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings EnableRowHoverStyle="true" EnablePostBackOnRowClick="True">
                    <Scrolling AllowScroll="true" SaveScrollPosition="true" UseStaticHeaders="true" />
                    <Selecting AllowRowSelect="true" />
                </ClientSettings>
            </telerik:RadGrid>
        </td>
    </tr>
</table>
