<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMetadataRepositorySel.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMetadataRepositorySel" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">
        var uscMetadataRepositorySel;
        require(["UserControl/uscMetadataRepositorySel"], function (UscMetadataRepositorySel) {
            $(function () {
                uscMetadataRepositorySel = new UscMetadataRepositorySel(tenantModelConfiguration.serviceConfiguration);
                uscMetadataRepositorySel.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    uscMetadataRepositorySel.rcbMetadataRepositoryId = "<%= rcbMetadataRepository.ClientID %>";
                    uscMetadataRepositorySel.maxNumberElements = "<%= MaxNumberDropdownElements %>";
                    uscMetadataRepositorySel.metadataPageContentId = "<%= metadataPageContent.ClientID %>";
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
                <telerik:RadComboBox ID="rcbMetadataRepository" AllowCustomText="false" Width="300px" CausesValidation="false" EnableLoadOnDemand="true"
                    EnableVirtualScrolling="false" ShowMoreResultsBox="true" AutoPostBack="false" MaxHeight="160px" runat="server" />
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
