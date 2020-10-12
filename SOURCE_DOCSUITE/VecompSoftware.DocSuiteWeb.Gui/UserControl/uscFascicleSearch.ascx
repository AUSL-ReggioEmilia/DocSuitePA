<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicleSearch.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicleSearch" %>

<%@ Register Src="uscFascSummary.ascx" TagName="uscFascSummary" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscFascicleFolders.ascx" TagName="uscFascicleFolders" TagPrefix="usc" %>
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
                uscFascicleSearch.fascFoldersContentId = "<%= fascFoldersContent.ClientID %>";
                uscFascicleSearch.pageId = "<%= pageContent.ClientID %>";
                uscFascicleSearch.finderContentId = "<%= finderContent.ClientID %>";
                uscFascicleSearch.defaultCategoryId = "<%= DefaultCategoryId %>";
                uscFascicleSearch.fascicleObject = "<%= FascicleObject %>";
                uscFascicleSearch.categoryFullIncrementalPath = "<%= CategoryFullIncrementalPath %>";
                uscFascicleSearch.uscFascFoldersId = "<%= uscFascicleFolders.PageContentDiv.ClientID %>";
                uscFascicleSearch.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscFascicleSearch.fascDetailsPaneId = "<%= fascDetailsPane.ClientID %>";
                uscFascicleSearch.folderSelectionEnabled = <%= FolderSelectionEnabled.ToString().ToLower() %>;
                uscFascicleSearch.btnSearchByCategoryId = "<%= btnSearchByCategory.ClientID %>";
                uscFascicleSearch.btnSearchBySubjectId = "<%= btnSearchBySubject.ClientID %>";
                uscFascicleSearch.btnSearchByMetadataId = "<%= btnSearchByMetadata.ClientID%>";
                uscFascicleSearch.rddlSelectMetadataId = "<%=rddlSelectMetadata.ClientID%>";
                uscFascicleSearch.metadataContainerId = "<%=metadataContainer.ClientID%>";
                uscFascicleSearch.dswEnvironment = <%=DSWEnvironment%>;
                uscFascicleSearch.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscFascicleSearch.initialize();

                new ResizeSensor($(".details-column")[0], function () {
                    var height = $(".details-column").height();

                    if (height > 0) {
                        $(".folders-column").height(height);
                        $(".fascicle-folders-panel").height(height - 4);
                    }
                });
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ShowContentDuringLoad="false" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="600" ID="searchWindow" runat="server" Title="Ricerca fascicolo" Width="750" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadPageLayout runat="server" ID="pageContent">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <asp:Panel runat="server" ID="finderContent" CssClass="dsw-panel">
                    <div class="dsw-panel-title" style="padding: 5px;">
                        <label>Fascicoli disponibili</label>
                        <telerik:RadButton ID="btnSearch" Style="margin-left: 5px;" runat="server" Text="Cerca" AutoPostBack="false" CausesValidation="false" />
                        <telerik:RadButton ID="btnSearchByCategory" Style="margin-left: 5px;" runat="server" Text="Suggeriti per classificazione" AutoPostBack="false" CausesValidation="false" />
                        <telerik:RadButton ID="btnSearchBySubject" Style="margin-left: 5px;" runat="server" Text="Suggeriti per oggetto" AutoPostBack="false" CausesValidation="false" />
                        <telerik:RadButton ID="btnSearchByMetadata" Style="margin-left: 5px; display: none;" runat="server" Text="Suggeriti da metadato" AutoPostBack="false" CausesValidation="false" />
                    </div>
                    <div id="metadataContainer" runat="server" style="margin: 5px 0 0; display: none;">
                        <asp:Label runat="server" ID="rtbMetadata">Metadati</asp:Label>
                        <telerik:RadDropDownList runat="server" Style="margin-left: 5px;" ID="rddlSelectMetadata" Width="300px" AutoPostBack="false" DropDownHeight="200px">
                        </telerik:RadDropDownList>
                    </div>
                    <div class="dsw-panel-content">
                        <telerik:RadPageLayout runat="server" ID="fascDetailsPane">
                            <telerik:LayoutRow HtmlTag="Div">
                                <Columns>
                                    <telerik:LayoutColumn Span="8" ID="fascSummaryColumn" CssClass="details-column" Style="padding-left: 5px; padding-right: 5px;">
                                        <asp:Panel runat="server" ID="summaryContent" Style="display: none;">
                                            <usc:uscFascSummary ID="uscFascSummary" runat="server" IsSummaryLink="true" />
                                        </asp:Panel>
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="4" ID="fascFoldersColumn" CssClass="folders-column" Height="100%" Style="padding: 0px 5px 0px;">
                                        <asp:Panel runat="server" ID="fascFoldersContent" Style="display: none;">
                                            <usc:uscFascicleFolders ID="uscFascicleFolders" ViewOnlyFolders="true" runat="server"></usc:uscFascicleFolders>
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
