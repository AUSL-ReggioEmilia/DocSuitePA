<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMetadataRepositorySel.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMetadataRepositorySel" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSetiContactSel.ascx" TagName="uscSetiContactSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadataRest.ascx" TagName="uscDynamicMetadataRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscAdvancedSearchDynamicMetadataRest.ascx" TagName="uscAdvancedSearchDynamicMetadataRest" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">
        var uscMetadataRepositorySel;
        require(["UserControl/uscMetadataRepositorySel"], function (UscMetadataRepositorySel) {
            $(function () {
                uscMetadataRepositorySel = new UscMetadataRepositorySel(tenantModelConfiguration.serviceConfiguration);
                uscMetadataRepositorySel.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscMetadataRepositorySel.rcbMetadataRepositoryId = "<%= rcbMetadataRepository.ClientID %>";
                uscMetadataRepositorySel.advancedMetadataSearchPanelId = "<%= advancedMetadataSearchPanel.ClientID %>";
                uscMetadataRepositorySel.maxNumberElements = "<%= MaxNumberDropdownElements %>";
                uscMetadataRepositorySel.metadataPageContentId = "<%= metadataPageContent.ClientID %>";
                uscMetadataRepositorySel.setiContactEnabledId = <%=ProtocolEnv.SETIIntegrationEnabled.ToString().ToLower()%>;
                uscMetadataRepositorySel.uscSetiContactSelId = "<%= uscSetiContactSel.PageContentDiv.ClientID%>";
                uscMetadataRepositorySel.setiVisibilityButtonId = <%=SetiContactVisibilityButton.ToString().ToLower()%>;
                uscMetadataRepositorySel.uscAdvancedSearchDynamicMetadataRestId = "<%= uscAdvancedSearchDynamicMetadataRest.PageContent.ClientID %>";
                uscMetadataRepositorySel.enableAdvancedMetadataSearchBtnId = "<%= enableAdvancedMetadataSearchBtn.ClientID %>";
                uscMetadataRepositorySel.txtMetadataValueId = "<%= txtMetadataValue.ClientID %>";
                uscMetadataRepositorySel.advancedMetadataRepositoryEnabled = <%= (EnableAdvancedMetadataSearch AndAlso ProtocolEnv.MetadataRepositoryEnabled).ToString().ToLower() %>;
                uscMetadataRepositorySel.advancedMetadataSearchEnabled = <%=ProtocolEnv.AdvancedMetadataSearchEnabled.ToString().ToLower()%>;

                uscMetadataRepositorySel.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<telerik:RadPageLayout runat="server" ID="metadataPageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="metadataRepositoryRow">
            <Content>
                <telerik:RadComboBox ID="rcbMetadataRepository" AllowCustomText="false" Width="300px" CausesValidation="false" EnableLoadOnDemand="true" EnableVirtualScrolling="false" ShowMoreResultsBox="true" AutoPostBack="false" MaxHeight="160px" runat="server" />
                <div id="advancedMetadataSearchPanel" runat="server" style="display:none;">
                <asp:CheckBox runat="server" ID="enableAdvancedMetadataSearchBtn" Style="margin-left: 10px;" Text="Ricerca avanzata"></asp:CheckBox>
                </div>
                <usc:uscSetiContactSel runat="server" ID="uscSetiContactSel" />
                <div style="margin-top: 5px;">
                    <telerik:RadTextBox ID="txtMetadataValue" MaxLength="255" runat="server" Width="300px" LabelCssClass="strongRiLabel t-col-right-padding" Label="Valore metadati:"/>
                </div>
                <usc:uscAdvancedSearchDynamicMetadataRest runat="server" ID="uscAdvancedSearchDynamicMetadataRest"></usc:uscAdvancedSearchDynamicMetadataRest>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
