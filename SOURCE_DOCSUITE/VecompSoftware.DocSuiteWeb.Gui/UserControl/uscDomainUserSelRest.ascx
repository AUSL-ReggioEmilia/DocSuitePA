<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDomainUserSelRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDomainUserSelRest" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var domainUserSelRest;
        require(["UserControl/uscDomainUserSelRest"], function (UscDomainUserSelRest) {
            $(function () {
                domainUserSelRest = new UscDomainUserSelRest(tenantModelConfiguration.serviceConfiguration);

                domainUserSelRest.pageContentId = "<%=PageContent.ClientID%>";
                domainUserSelRest.radTreeContactId = "<%=RadTreeContact.ClientID%>";
                domainUserSelRest.btnSelContactDomainId = "<%=btnSelContactDomain.ClientID%>";
                domainUserSelRest.btnDelContactId ="<%=btnDelContact.ClientID%>";
                domainUserSelRest.radWindowManagerId = "<%=RadWindowManagerContacts.ClientID%>";
                domainUserSelRest.panelButtonsId = "<%=panelButtons.ClientID%>";
                domainUserSelRest.currentUserDescription = "<%= CurrentUserDescription %>";
                domainUserSelRest.currentUser = <%= CurrentUser %>;
                domainUserSelRest.currentUserEmail = "<%= CurrentUserEmail %>";
                domainUserSelRest.initialize(); 
            });
        });
    </script>
    <style>
        .font_node {
            font-weight: bold;
        }
    </style>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerContacts" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelContact" ReloadOnShow="true" runat="server" Title="Selezione Contatto" Behaviors="Maximize,Resize,Minimize,Close">
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>

<table class="datatable" style="min-width: 200px;" id="tableId" runat="server">
    <tr class="Chiaro">
        <td class="DXChiaro">
            <telerik:RadTreeView ID="RadTreeContact" runat="server" Width="100%" CheckBoxes="True" >
                <Nodes>
                    <telerik:RadTreeNode Expanded="true" Font-Bold="true" runat="server" Text="Contatti" Value="Root" />
                </Nodes>
            </telerik:RadTreeView>
        </td>
        <td style="width: 10%">
            <div style="text-align: right; white-space: nowrap;">
                <asp:Panel runat="server" ID="panelButtons" HorizontalAlign="Right">
                    <asp:ImageButton ID="btnSelContactDomain" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" runat="server" />
                    <asp:ImageButton ID="btnDelContact" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" runat="server" />
                </asp:Panel>
            </div>
        </td>
    </tr>
</table>
