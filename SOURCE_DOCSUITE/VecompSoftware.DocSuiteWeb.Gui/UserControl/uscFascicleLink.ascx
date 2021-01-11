<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicleLink.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicleLink" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscCategory" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscFascSummary.ascx" TagName="uscFascSummary" TagPrefix="uc1" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscFascicleLink;
        require(["UserControl/uscFascicleLink"], function (UscFascicleLink) {
            $(function () {
                uscFascicleLink = new UscFascicleLink(tenantModelConfiguration.serviceConfiguration);
                uscFascicleLink.lblCategoryMessageId = "<%= lblCategoryMessage.ClientID %>";
                uscFascicleLink.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscFascicleLink.fascicleSummaryId = "<%= fascicleSummary.ClientID %>";
                uscFascicleLink.rcbOtherFascicles = "<%= rcbOtherFascicles.ClientID %>";
                uscFascicleLink.pageId = "<%= pageContent.ClientID%>";
                uscFascicleLink.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                uscFascicleLink.fascicleSummaryId = "<%= fascicleSummary.ClientID %>";
                uscFascicleLink.uscCategoryId = "<%= uscCategory.ClientID %>";
                uscFascicleLink.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFascicleLink.uscFascSummaryId = "<%=uscFascSummary.PageContentDiv.ClientID%>";
                uscFascicleLink.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadPageLayout ID="pageContent" HtmlTag="Div" Width="100%" runat="server">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>

                <div class="dsw-panel">
                    <div class="dsw-panel-title">
                        <label id="freeFascicleTitle">Fascicoli disponibili</label>
                    </div>
                    <div class="dsw-panel-content">
                        <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                            <Rows>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Content>
                                        <uc:uscCategory runat="server" ID="uscCategory" Caption="Classificazione" HeaderVisible="false" Required="false" />
                                    </Content>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Content>
                                        <telerik:RadComboBox ID="rcbOtherFascicles" EmptyMessage="Seleziona un fascicolo" AllowCustomText="false" Width="600px"
                                            EnableLoadOnDemand="true" EnableVirtualScrolling="false" ShowMoreResultsBox="true" AutoPostBack="false" MaxHeight="250px"
                                            Style="margin: 5px 0px 5px 2px;" runat="server">
                                        </telerik:RadComboBox>
                                        <asp:Label ID="lblCategoryMessage" runat="server" Font-Bold="true" Style="color: red; margin-left: 5px;"></asp:Label>
                                    </Content>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Content>
                                        <fieldset runat="server" id="fascicleSummary">
                                            <legend>
                                                <b>Fascicolo selezionato</b>
                                            </legend>
                                            <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                                <Rows>
                                                    <telerik:LayoutRow HtmlTag="Div">
                                                        <Content>
                                                            <uc1:uscFascSummary ID="uscFascSummary" runat="server" IsSummaryLink="true" />
                                                        </Content>
                                                    </telerik:LayoutRow>
                                                </Rows>
                                            </telerik:RadPageLayout>
                                        </fieldset>
                                        <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
                                    </Content>
                                </telerik:LayoutRow>
                            </Rows>
                        </telerik:RadPageLayout>
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
