<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscZenDeskHelp.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscZenDeskHelp" %>

<link rel="stylesheet" href="../Content/zenDesk.css" />

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock" EnableViewState="false">
    <script type="text/javascript">
        var zenDesk;

        require(["UserControl/uscZenDeskHelp"], function (uscZenDeskHelp) {
            $(function () {
                zenDesk = new uscZenDeskHelp(tenantModelConfiguration.serviceConfiguration);
                zenDesk.rtbSearchId = "<%= rtbSearch.ClientID %>";
                zenDesk.btnRulesId = "<%= btnRules.ClientID %>";
                zenDesk.btnSolutionsId = "<%= btnSolutions.ClientID %>";
                zenDesk.btnDocSuiteId = "<%= btnDocSuite.ClientID %>";
                zenDesk.btnFAQsId = "<%= btnFAQs.ClientID %>";
                zenDesk.serializedCategories = "<%= Categories %>";
                zenDesk.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                zenDesk.ajaxLoadingPanelId = "<%=  AjaxDefaultLoadingPanel.ClientID %>";
                zenDesk.ZenDeskPaneId = "<%= zenDeskPageContent.ClientID %>";
                zenDesk.rtvArticlesId = "<%= rtvArticles.ClientID %>";
                zenDesk.rpArticleId = "<%= rpArticle.ClientID %>";
                zenDesk.btnSearchId = "<%= btnSearch.ClientID %>";
                zenDesk.isButtonPressed = "<%= IsButtonPressed %>";
                zenDesk.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<table class="datatable">
    <tr>
        <td class="label labelPanel panel-text">Cerca:
        </td>
        <td style="width: 75%; padding: 7px;">
            <telerik:RadTextBox runat="server" ID="rtbSearch" CssClass="search-input" EmptyMessage="Cerca..." AutoPostBack="false" Width="40%" />
            <asp:Image ID="aspimage1" runat="server" ImageAlign="AbsMiddle" CssClass="styleIcon" ImageUrl="../App_Themes/DocSuite2008/imgset16/search.png" />
            <telerik:RadButton runat="server" ID="btnSearch" CssClass="search-button" AutoPostBack="false" Width="10%" Text="Cerca" />
        </td>
    </tr>
    <tr>
        <td class="label labelPanel panel-text">Seleziona categoria:
        </td>
        <td style="width: 75%; padding: 7px;">
            <telerik:RadButton runat="server" ID="btnRules" Text="Normativa" CssClass="categoryButton" AutoPostBack="false" Width="10%" />
            <telerik:RadButton runat="server" ID="btnSolutions" Text="Soluzioni" CssClass="categoryButton" AutoPostBack="false" Width="10%" />
            <telerik:RadButton runat="server" ID="btnDocSuite" Text="DocSuite" CssClass="categoryButton" AutoPostBack="false" Width="10%" />
            <telerik:RadButton runat="server" ID="btnFAQs" Text="FAQs" CssClass="categoryButton" AutoPostBack="false" Width="10%" />
        </td>
    </tr>
</table>

<telerik:RadPageLayout runat="server" ID="zenDeskPageContent" Width="99%" Height="90%" HtmlTag="Div">
    <Rows>
        <telerik:LayoutRow Width="100%" Height="100%">
            <Content>
                <telerik:RadSplitter runat="server" ID="rsZenDesk" Width="100%" Height="100%">
                    <telerik:RadPane runat="server" ID="rpTree" Width="25%" Height="100%">
                        <telerik:RadTreeView runat="server" ID="rtvArticles" Skin="Bootstrap" ShowLineImages="false" />
                    </telerik:RadPane>
                    <telerik:RadSplitBar runat="server" ID="Bar1" />
                    <telerik:RadPane runat="server" ID="rpArticle" CssClass="article-body" Width="75%" Height="100%">
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
