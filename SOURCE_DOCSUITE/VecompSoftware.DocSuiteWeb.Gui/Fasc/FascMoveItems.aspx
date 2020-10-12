<%@ Page Title=" " Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="FascMoveItems.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascMoveItems" %>

<%@ Register Src="~/UserControl/uscFascicleFolders.ascx" TagName="uscFascicleFolders" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var fascMoveItems;
            require(["Fasc/FascMoveItems"], function (FascMoveItems) {
                $(function () {
                    fascMoveItems = new FascMoveItems(tenantModelConfiguration.serviceConfiguration);
                    fascMoveItems.idFascicle = "<%= IdFascicle %>";
                    fascMoveItems.idFascicleFolder = "<%= IdFascicleFolder %>";
                    fascMoveItems.uscFascicleFoldersId = "<%= uscFascicleFolders.PageContentDiv.ClientID %>";
                    fascMoveItems.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascMoveItems.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    fascMoveItems.rtvItemsToMoveId = "<%= rtvItemsToMove.ClientID %>";
                    fascMoveItems.itemsType = "<%= ItemsType %>";
                    fascMoveItems.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascMoveItems.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascMoveItems.pnlPageId = "<%= pnlPage.ClientID %>";
                    fascMoveItems.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascMoveItems.lblItemSelectedDescriptionId = "<%= lblItemSelectedDescription.ClientID %>";
                    fascMoveItems.destinationFascicleId = "<%= DestinationFascicleId %>";
                    fascMoveItems.moveToFascicle = "<%= MoveToFascicle %>".toLocaleLowerCase();
                    fascMoveItems.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <div class="dsw-panel">
        <div class="dsw-panel-title">
            <asp:Label runat="server" ID="lblItemSelectedDescription" Text=""></asp:Label>
        </div>
        <div class="dsw-panel-content">
            <telerik:RadTreeView runat="server" ID="rtvItemsToMove"></telerik:RadTreeView>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlPage" Height="100%">
        <usc:uscFascicleFolders runat="server" ID="uscFascicleFolders" IsVisibile="true" ViewOnlyFolders="true" />
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnConfirm" SingleClick="true" SingleClickText="Attendere..." Text="Conferma" AutoPostBack="false"></telerik:RadButton>
</asp:Content>
