<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ReportDesignerList.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReportDesignerList" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscReportDesigner.ascx" TagName="uscReportDesigner" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var reportDesignerList;
            require(["Reporting/ReportDesignerList", "jquery", "jqueryui"], function (ReportDesignerList) {
                $(function () {
                    reportDesignerList = new ReportDesignerList(tenantModelConfiguration.serviceConfiguration);
                    reportDesignerList.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    reportDesignerList.uscReportDesignerId = "<%= uscReportDesigner.PageContentId %>";
                    reportDesignerList.rtvReportsId = "<%= rtvReports.ClientID %>";
                    reportDesignerList.btnNewId = "<%= btnNew.ClientID %>";
                    reportDesignerList.btnEditId = "<%= btnEdit.ClientID %>";
                    reportDesignerList.splPageId = "<%= splPage.ClientID %>";
                    reportDesignerList.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    reportDesignerList.toolBarSearchId = "<%= ToolBarSearch.ClientID%>";

                    reportDesignerList.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" Orientation="Vertical" ID="splPage">
            <telerik:RadPane runat="server" Width="30%" Scrolling="None">
                <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton Value="searchDescription" runat="server">
                            <ItemTemplate>
                                <telerik:RadTextBox ID="txtReportName" EmptyMessage="Nome" runat="server" Width="150px" Style="margin-left: 3px;"></telerik:RadTextBox>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadTreeView runat="server" ID="rtvReports" Height="100%" Width="100%">
                    <Nodes>
                        <telerik:RadTreeNode Expanded="true" runat="server" NodeType="Root" Selected="true" Text="Report" Value="" />
                    </Nodes>
                </telerik:RadTreeView>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server"></telerik:RadSplitBar>

            <telerik:RadPane runat="server" Width="70%">
                <usc:uscReportDesigner runat="server" ID="uscReportDesigner" IsEditable="false"></usc:uscReportDesigner>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnNew" AutoPostBack="false" Text="Aggiungi"></telerik:RadButton>
    <telerik:RadButton runat="server" ID="btnEdit" AutoPostBack="false" Text="Modifica"></telerik:RadButton>
</asp:Content>
