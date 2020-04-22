<%@ Control Language="VB" AutoEventWireup="false" CodeBehind="uscDocumentUnitReferences.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentUnitReferences" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var documentUnit;
        require(["UserControl/uscDocumentUnitReferences"], function (UscDocumentUnitReferences) {
            $(function () {
                documentUnit = new UscDocumentUnitReferences(tenantModelConfiguration.serviceConfiguration);

                documentUnit.radTreeDocumentsId = "<%=RadTreeDocuments.ClientID%>"
                documentUnit.documentUnitId = "<%=IdDocumentUnit%>";
                documentUnit.documentUnitYear = "<%=DocumentUnitYear%>";
                documentUnit.documentUnitNumber = "<%=DocumentUnitNumber%>";
                documentUnit.rpbDocumentsId = "<%= rpbDocuments.ClientID%>";
                documentUnit.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                documentUnit.managerWindowsId = "<%= manager.ClientID %>";
                documentUnit.administrationTrasparenteProtocol = "<%=AdministrationTrasparenteProtocol%>";
                documentUnit.seriesTitle = "<%=SeriesTitle%>";
                documentUnit.protocolDocumentSeriesButtonEnable = "<%=ProtocolDocumentSeriesButtonEnable%>";

                documentUnit.showFascicleLinks = "<%=ShowFascicleLinks%>";
                documentUnit.showProtocolRelationLinks = "<%=ShowProtocolRelationLinks%>";
                documentUnit.showArchiveRelationLinks = "<%=ShowArchiveRelationLinks%>";
                documentUnit.showProtocolMessageLinks = "<%=ShowProtocolMessageLinks%>";
                documentUnit.showProtocolDocumentSeriesLinks = "<%=ShowProtocolDocumentSeriesLinks%>";
                documentUnit.showDocumentSeriesMessageLinks = "<%=ShowDocumentSeriesMessageLinks%>";
                documentUnit.showDocumentSeriesResolutionsLinks = "<%=ShowDocumentSeriesResolutionsLinks AndAlso ResolutionEnable%>";
                documentUnit.showDocumentSeriesProtocolsLinks = "<%=ShowDocumentSeriesProtocolsLinks%>";

                documentUnit.showArchiveLinks = "<%=ShowArchiveLinks%>";
                documentUnit.showProtocolLinks = "<%=ShowProtocolLinks%>";

                documentUnit.showIncomingPECMailLinks = "<%=ShowPECIncoming%>";
                documentUnit.showOutgoingPECMailLinks = "<%=ShowPECOutgoing%>";

                documentUnit.showResolutionlMessageLinks = "<%=ShowResolutionlMessageLinks%>";
                documentUnit.showResolutionDocumentSeriesLinks = "<%=ShowResolutionDocumentSeriesLinks%>";

                documentUnit.showFasciclesLinks = "<%=ShowFasciclesLinks%>";
                documentUnit.showDossierLinks = "<%=ShowDossierLinks%>";

                documentUnit.showActiveWorkflowActivities = "<%=ShowActiveWorkflowActivities%>";

                documentUnit.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="300" ID="searchMessages" runat="server" Title="Messaggi" Width="750" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadWindowManager EnableViewState="False" ID="manager1" runat="server">
    <Windows>
        <telerik:RadWindow Height="300" ID="searchPECDetails" runat="server" Title="PEC" Width="750" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbDocuments" Visible="true">
    <Items>
        <telerik:RadPanelItem Text="Collegamenti" Expanded="true" runat="server">
            <ContentTemplate>
                <telerik:RadTreeView ID="RadTreeDocuments" runat="server" Width="100%">
                </telerik:RadTreeView>
            </ContentTemplate>
        </telerik:RadPanelItem>
    </Items>
</telerik:RadPanelBar>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
