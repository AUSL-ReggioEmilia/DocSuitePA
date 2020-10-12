<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDocumentUnitConservationRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentUnitConservationRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var <%= Me.ClientID %>_uscDocumentUnitConservationRest;
        require(["UserControl/uscDocumentUnitConservationRest"], function (UscDocumentUnitConservationRest) {
            $(function () {
                <%= Me.ClientID %>_uscDocumentUnitConservationRest = new UscDocumentUnitConservationRest(tenantModelConfiguration.serviceConfiguration);
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.idDocumentUnit = "<%= IdDocumentUnit %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.imgConservationId = "<%= imgConservation.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.lblConservationDescriptionId = "<%= lblConservationDescription.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.lblArchivedDateId = "<%= lblArchivedDate.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.lblParerUriId = "<%= lblParerUri.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.lblHasErrorId = "<%= lblHasError.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.lblLastErrorId = "<%= lblLastError.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.windowConservationDetailsId = "<%= windowConservationDetails.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.imgConservationInfoId = "<%= imgConservationInfo.ClientID %>";
                <%= Me.ClientID %>_uscDocumentUnitConservationRest.initialize();
            });
        });

        function openConservationDetails() {
            <%= Me.ClientID %>_uscDocumentUnitConservationRest.openConservationDetails();
            return false;
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerConservation" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowConservationDetails" Height="600" Width="750" runat="server" Title="Dettaglio della conservazione" 
            DestroyOnClose="true" ShowContentDuringLoad="false">
            <ContentTemplate>
                <table class="datatable">
                    <tr>
                        <td class="label col-dsw-2">Data archiviazione:</td>
                        <td class="col-dsw-8">
                            <asp:Label runat="server" ID="lblArchivedDate"></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="label">Uri di conservazione:</td>
                        <td>
                            <asp:Label runat="server" ID="lblParerUri"></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="label">Errori:</td>
                        <td>
                            <asp:Label runat="server" ID="lblHasError"></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="label">Ultimo errore:</td>
                        <td>
                            <asp:Label runat="server" ID="lblLastError"></asp:Label></td>
                    </tr>
                </table>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<telerik:RadPageLayout runat="server" ID="pnlMainContent" CssClass="dsw-panel">
    <Rows>
        <telerik:LayoutRow runat="server" CssClass="dsw-panel-title" ID="rowTitle">
            <Content>
                Stato di conservazione
                <asp:ImageButton runat="server" CssClass="dsw-align-right" Style="margin-top: 1px; margin-right: 2px;" ImageUrl="../Comm/images/info.png"
                    OnClientClick="return openConservationDetails();" ID="imgConservationInfo" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow runat="server" CssClass="dsw-panel-content">
            <Content>
                <div style="margin: 5px">
                    <asp:Image runat="server" ID="imgConservation" Style="vertical-align: middle;" />
                    <asp:Label runat="server" ID="lblConservationDescription" Style="margin-left: 5px;"></asp:Label>
                </div>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
