<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonDomainUserSelRest.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonDomainUserSelRest"  MasterPageFile="~/MasterPages/DocSuite2008.Master"%>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">

            var commonDomainUserSelRest;
            require(["UserControl/CommonDomainUserSelRest"], function (CommonDomainUserSelRest) {
                $(function () {
                    commonDomainUserSelRest = new CommonDomainUserSelRest(tenantModelConfiguration.serviceConfiguration);

                    commonDomainUserSelRest.btnSearchId = "<%=btnSearch.ClientID%>";
                    commonDomainUserSelRest.txtFilterId = "<%=txtFilter.ClientID%>";
                    commonDomainUserSelRest.tvwContactDomainId = "<%=tvwContactDomain.ClientID%>";
                    commonDomainUserSelRest.btnConfirmId = "<%=btnConfirm.ClientID%>";
                    commonDomainUserSelRest.pageContentId = "<%=PageContent%>";
                    commonDomainUserSelRest.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    commonDomainUserSelRest.initialize();
                });
            });
        </script>
        <style>
            .font_node {
                font-weight: bold;
            }
        </style>
    </telerik:RadScriptBlock>    
</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <div style="width: 100%;">
        <table class="datatable">
            <tr>
                <td class="label labelPanel" style="width: 30%;">Utente:
                </td>
                <td>
                    <asp:Panel DefaultButton="btnSearch" runat="server" Style="display: inline;">
                        <asp:TextBox ID="txtFilter" runat="server" Width="200px" MaxLength="30" />
                    </asp:Panel>
                    <telerik:RadButton ID="btnSearch" runat="server" Text="Ricerca" AutoPostBack="false" />
                </td>
            </tr>
        </table>
    </div>
    <div style="width: 100%;">
        <telerik:RadTreeView ID="tvwContactDomain" runat="server" />
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnConfirm" runat="server" Text="Conferma" />
</asp:Content>
