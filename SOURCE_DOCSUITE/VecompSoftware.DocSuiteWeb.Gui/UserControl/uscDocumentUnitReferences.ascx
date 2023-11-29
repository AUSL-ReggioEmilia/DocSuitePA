<%@ Control Language="VB" AutoEventWireup="false" CodeBehind="uscDocumentUnitReferences.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentUnitReferences" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var documentUnit;
        require(["UserControl/uscDocumentUnitReferences"], function (UscDocumentUnitReferences) {
            $(function () {
                documentUnit = new UscDocumentUnitReferences(tenantModelConfiguration.serviceConfiguration);

                documentUnit.radTreeDocumentsId = "<%=RadTreeDocuments.ClientID%>"
                documentUnit.referenceUniqueId = "<%=ReferenceUniqueId%>";
                documentUnit.documentUnitYear = "<%=DocumentUnitYear%>";
                documentUnit.documentUnitNumber = "<%=DocumentUnitNumber%>";

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
                documentUnit.showPECUnifiedLinks = "<%=ShowPECUnified%>";
                documentUnit.showResolutionlMessageLinks = "<%=ShowResolutionlMessageLinks%>";
                documentUnit.showResolutionDocumentSeriesLinks = "<%=ShowResolutionDocumentSeriesLinks%>";
                documentUnit.showFasciclesLinks = "<%=ShowFasciclesLinks%>";
                documentUnit.showDossierLinks = "<%=ShowDossierLinks%>";
                documentUnit.showRemoveUDSLinksButton = "<%=ShowRemoveUDSLinksButton%>";
                documentUnit.showWorkflowActivities = "<%=ShowWorkflowActivities%>";
                documentUnit.showTNotice = "<%=ShowTNotice%>";
                documentUnit.showDeletedFascicleDocumentUnits = "<%=ShowDeletedFascicleDocumentUnits%>";
                documentUnit.showDeletedFascicleDocuments = "<%=ShowDeletedFascicleDocuments%>";
                documentUnit.showDocumentUnitFascicleLinks = "<%=ShowDocumentUnitFascicleLinks%>";

                documentUnit.btnExpandDocumentUnitReferenceId = "<%=btnExpandDocumentUnitReference.ClientID%>";
                documentUnit.documentUnitReferenceInfoId = "<%=documentUnitReferenceInfo.ClientID%>";
                documentUnit.docSuiteNextBaseUrl = "<%=DocSuiteNextBaseUrl%>";
                documentUnit.initialize();
            });
        });

        function removeLink(uniqueId, udsId, relationId) {
            documentUnit.removeLink(uniqueId, udsId, relationId);
        }
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

<telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbDocuments" Visible="true" CssClass="dsw-panel RadPanelBar_uscDocumentUnitReferences_Office2007">
    <Items>
        <telerik:RadPanelItem Text="Collegamenti" Expanded="true" runat="server" PreventCollapse="true">
            <HeaderTemplate>
                <div class="dsw-panel-title">
                    Collegamenti
            <telerik:RadButton ID="btnExpandDocumentUnitReference" CssClass="dsw-vertical-middle" runat="server" AutoPostBack="false" Width="16px" Height="16px" Visible="true">
                <Image EnableImageButton="true" />
            </telerik:RadButton>
                </div>
            </HeaderTemplate>
            <ContentTemplate>
                <div class="dsw-panel-content" id="documentUnitReferenceInfo" runat="server">
                    <telerik:RadTreeView ID="RadTreeDocuments" runat="server" Width="100%">
                    </telerik:RadTreeView>
                </div>
            </ContentTemplate>
        </telerik:RadPanelItem>
    </Items>
</telerik:RadPanelBar>


<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
