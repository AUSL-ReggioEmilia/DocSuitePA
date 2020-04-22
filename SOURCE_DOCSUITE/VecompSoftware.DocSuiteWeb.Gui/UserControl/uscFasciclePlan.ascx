<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFasciclePlan.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFasciclePlan" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscFasciclePlan;
        require(["UserControl/UscFasciclePlan"], function (UscFasciclePlan) {
            $(function () {
                uscFasciclePlan = new UscFasciclePlan(tenantModelConfiguration.serviceConfiguration);
                uscFasciclePlan.rtvEnvironmentsId = "<%= rtvEnvironments.ClientID%>";
                uscFasciclePlan.btnAddPeriodicPlan = "<%= btnAddPeriodicPlan.ClientID%>";
                uscFasciclePlan.btnRemovePeriodicPlan = "<%=  btnRemovePeriodicPlan.ClientID%>";
 
                uscFasciclePlan.pnlFasciclePlanId = "<%= pageContent.ClientID%>";
                uscFasciclePlan.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscFasciclePlan.currentCategoryId = "<%= CurrentCategoryId%>";
                uscFasciclePlan.currentResolutionName = "<%= CurrentResolutionName%>";
                uscFasciclePlan.currentDocumentSeriesName = "<%= CurrentDocumentSeriesName%>";
                uscFasciclePlan.managerCreatePeriodId = "<%= managerCreatePeriod.ClientID%>";
                uscFasciclePlan.managerId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID%>";
                uscFasciclePlan.managerWindowId = "<%= manager.ClientID%>";

                uscFasciclePlan.pnlTreeViewId = "<%= pnlTreeView.ClientID%>";
                uscFasciclePlan.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFasciclePlan.ajaxManagerId = "<%= AjaxManager.ClientID%>";

                uscFasciclePlan.initialize();
            });
        });

        function responseEnd() {
            uscFasciclePlan.hideLoadingPanel();
        }

        function environmentTreeView_ClientNodeClicked(sender, args) {
            uscFasciclePlan.environmentTreeView_ClientNodeClicked(sender, args);
        }

    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="600" ID="managerCreatePeriod" runat="server" Title="Fascicolazione Periodica" Width="750" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <div class="dsw-panel-content">
                    <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                        <Items>
                             <telerik:RadPanelItem Text="Fascicoli periodici attivabili" Expanded="true">
                                <ContentTemplate>
                                    <asp:Panel runat="server" ID="pnlTreeView">
                                        <telerik:RadTreeView BorderStyle="none" ID="rtvEnvironments" runat="server" Width="100%" OnClientNodeClicked="environmentTreeView_ClientNodeClicked">
                                            <Nodes>
                                                <telerik:RadTreeNode Expanded="True" NodeType="Root" runat="server" Selected="True" Text="Tipi di unità documentarie" Value="" />
                                            </Nodes>
                                        </telerik:RadTreeView>
                                    </asp:Panel>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem>
                                <ContentTemplate>
                                    <asp:Panel runat="server" ID="pnlButtons">
                                        <telerik:RadButton ID="btnAddPeriodicPlan" runat="server" Width="160px" Text="Aggiungi periodico" AutoPostBack="false" />
                                        <telerik:RadButton ID="btnRemovePeriodicPlan" runat="server" Width="160px" Text="Rimuovi periodico" AutoPostBack="false" />
                                    </asp:Panel>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                        </Items>
                    </telerik:RadPanelBar>
                </div>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
