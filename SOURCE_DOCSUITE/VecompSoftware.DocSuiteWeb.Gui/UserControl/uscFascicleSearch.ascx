<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicleSearch.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicleSearch" %>

<%@ Register Src="uscFascSummary.ascx" TagName="uscFascSummary" TagPrefix="usc" %>
<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var uscFascicleSearch;
        require(["UserControl/uscFascicleSearch"], function (UscFascicleSearch) {
            $(function () {
                uscFascicleSearch = new UscFascicleSearch(tenantModelConfiguration.serviceConfiguration);
                uscFascicleSearch.managerWindowsId = "<%= manager.ClientID %>";
                uscFascicleSearch.searchWindowId = "<%= searchWindow.ClientID %>";
                uscFascicleSearch.ajaxFlatLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>";
                uscFascicleSearch.uscFascicleSummaryId = "<%= uscFascSummary.PageContentDiv.ClientID %>";
                uscFascicleSearch.btnSearchId = "<%= btnSearch.ClientID %>";
                uscFascicleSearch.pageContentId = "<%= finderContent.ClientID %>";
                uscFascicleSearch.summaryContentId = "<%= summaryContent.ClientID %>";
                uscFascicleSearch.pageId = "<%= pageContent.ClientID %>";
                uscFascicleSearch.finderContentId = "<%= finderContent.ClientID %>";
                uscFascicleSearch.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="600" ID="searchWindow" runat="server" Title="Ricerca fascicolo" Width="750" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadPageLayout runat="server" ID="pageContent">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <asp:Panel runat="server" ID="finderContent" CssClass="dsw-panel">
                    <div class="dsw-panel-title">
                        <label>Fascicoli disponibili</label>
                    </div>
                    <div class="dsw-panel-content">
                        <telerik:RadPageLayout runat="server">
                            <telerik:LayoutRow HtmlTag="Div">
                                <Columns>
                                    <telerik:LayoutColumn Span="2">
                                        <telerik:RadButton ID="btnSearch" runat="server" Text="Cerca" AutoPostBack="false" CausesValidation="false" />
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="10">
                                        <asp:Panel runat="server" ID="summaryContent" Style="display: none;">
                                            <usc:uscFascSummary ID="uscFascSummary" runat="server" IsSummaryLink="true" />
                                        </asp:Panel>
                                    </telerik:LayoutColumn>
                                </Columns>
                            </telerik:LayoutRow>
                        </telerik:RadPageLayout>                        
                    </div>
                </asp:Panel>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
