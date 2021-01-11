<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltMassimariScarto.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltMassimari" %>

<%@ Register Src="~/UserControl/uscMassimarioScarto.ascx" TagName="MassimarioScarto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var tbltMassimariScarto;
            require(["Tblt/TbltMassimariScarto"], function (TbltMassimariScarto) {
                $(function () {
                    tbltMassimariScarto = new TbltMassimariScarto(tenantModelConfiguration.serviceConfiguration);
                    tbltMassimariScarto.btnSaveId = "<%= btnSave.ClientID %>";
                    tbltMassimariScarto.categoryId = "<%= CategoryId %>";
                    tbltMassimariScarto.uscMassimarioScartoId = "<%= uscMassimarioScarto.TreeMassimarioScarto.ClientID %>";
                    tbltMassimariScarto.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    tbltMassimariScarto.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltMassimariScarto.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltMassimariScarto.initialize();
                });
            });

            function CloseWindow() {
                tbltMassimariScarto.closeWindow();
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:MassimarioScarto runat="server" ID="uscMassimarioScarto" HideCanceledFilter="true"></usc:MassimarioScarto>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnSave" Text="Conferma"></telerik:RadButton>
</asp:Content>
