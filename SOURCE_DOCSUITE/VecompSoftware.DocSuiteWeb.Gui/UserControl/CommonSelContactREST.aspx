<%@ Page Title="Rubrica smart" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonSelContactRest.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelContactRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContactRest.ascx" TagPrefix="uc1" TagName="uscContactRest" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var commonSelContactREST;
            require(["UserControl/CommonSelContactRest"], function (CommonSelContactRest) {
                $(function () {
                    commonSelContactREST = new CommonSelContactRest(tenantModelConfiguration.serviceConfiguration);
                    commonSelContactREST.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    commonSelContactREST.btnConfirmAndNewId = "<%= btnConfirmAndNew.ClientID %>";
                    commonSelContactREST.uscContactRestId = "<%= uscContactRest.PanelContent.ClientID %>";
                    commonSelContactREST.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    commonSelContactREST.callerId = "<%= CallerID %>";
                    commonSelContactREST.filterByParentId = "<%= If(FilterByParentId.HasValue, FilterByParentId.Value, "undefined")  %>";
                    commonSelContactREST.ajaxFlatLoadingPanelId = "<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID %>";
                    commonSelContactREST.pnlButtonsId = "<%= pnlButtons.ClientID %>";
                    commonSelContactREST.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">    
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <uc1:uscContactREST runat="server" id="uscContactRest"/>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnConfirm" AutoPostBack="false" Text="Conferma" Width="150px"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnConfirmAndNew" AutoPostBack="false" Text="Conferma e nuovo" Width="150px"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
