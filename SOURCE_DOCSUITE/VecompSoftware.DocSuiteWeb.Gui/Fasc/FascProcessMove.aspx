<%@ Page Title="Sposta Fascicolo" Language="vb" AutoEventWireup="false" CodeBehind="FascProcessMove.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascMove" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var fascProcessMove;
            require(["Fasc/FascProcessMove"], function (FascProcessMove) {
                $(function () {
                    fascProcessMove = new FascProcessMove(tenantModelConfiguration.serviceConfiguration);
                    fascProcessMove.sourceFascicleId = "<%= FascicleId %>";
                    fascProcessMove.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascProcessMove.lblFascNameId = "<%= lblFascName.ClientID %>";
                    fascProcessMove.processesTreeViewId = "<%= processesTreeView.ClientID%>";
                    fascProcessMove.moveFascicleConfirmBtnId = "<%= btnConferma.ClientID %>";
                    fascProcessMove.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                    fascProcessMove.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    fascProcessMove.fascicleParentFolderId = "<%= ParentFolderId %>";
                    fascProcessMove.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascProcessMove.initialize();
                });
            });

            function treeView_LoadNodeChildrenOnExpand(sender, args) {
                fascProcessMove.treeView_LoadNodeChildrenOnExpand(sender, args);
            }
        </script>
    </telerik:RadScriptBlock>

    <table width="100%" class="dataform">
        <tr>
            <td>
                <b>Fascicolo:</b>
                <asp:Label runat="server" ID="lblFascName"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <asp:Panel runat="server" Height="100%" Width="100%" ID="pnlMainContent" Style="overflow: hidden;">
        <telerik:RadTreeView ID="processesTreeView" 
                             EnableViewState="false" 
                             runat="server" 
                             Width="100%" Height="100%"/>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnConferma" Text="Conferma Selezione" runat="server" AutoPostBack="False"/>
</asp:Content>