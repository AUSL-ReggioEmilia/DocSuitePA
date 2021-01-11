<%@ Page Title="Gestione report" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ReportDesigner.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReportDesigner" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscReportDesigner.ascx" TagName="uscReportDesigner" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscReportDesignerToolbox.ascx" TagName="uscReportToolbox" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscReportDesignerInformation.ascx" TagName="uscReportInformation" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">    
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var reportDesigner;
            require(["Reporting/ReportDesigner", "jquery", "jqueryui"], function (ReportDesigner) {
                $(function () {
                    reportDesigner = new ReportDesigner(tenantModelConfiguration.serviceConfiguration);
                    reportDesigner.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    reportDesigner.uscReportInformationId = "<%= uscReportInformation.PageContentId %>";
                    reportDesigner.uscReportDesignerId = "<%= uscReportDesigner.PageContentId %>";
                    reportDesigner.uscReportToolboxId = "<%= uscReportToolbox.PageContentId %>";
                    reportDesigner.toolboxItems = <%= ToolboxItems %>;
                    reportDesigner.btnDraftId = "<%= btnDraft.ClientID %>";
                    reportDesigner.btnSaveId = "<%= btnSave.ClientID %>";
                    reportDesigner.splPageId = "<%= splPage.ClientID %>";
                    reportDesigner.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    reportDesigner.reportUniqueId = "<%= ReportUniqueId %>";
                    reportDesigner.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">        
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" Orientation="Vertical" ID="splPage">
            <telerik:RadPane runat="server" Width="280px" Scrolling="None">
                <usc:uscReportInformation runat="server" ID="uscReportInformation"></usc:uscReportInformation>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server"></telerik:RadSplitBar>

            <telerik:RadPane runat="server">
                <usc:uscReportDesigner runat="server" ID="uscReportDesigner" IsEditable="true"></usc:uscReportDesigner>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server"></telerik:RadSplitBar>
            
            <telerik:RadPane runat="server" Width="250px">
                <usc:uscReportToolbox runat="server" ID="uscReportToolbox"></usc:uscReportToolbox>    
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnDraft" AutoPostBack="false" Text="Bozza" ValidationGroup="ReportData"></telerik:RadButton>
    <telerik:RadButton runat="server" ID="btnSave" AutoPostBack="false" Text="Conferma" ValidationGroup="ReportData"></telerik:RadButton>
</asp:Content>
