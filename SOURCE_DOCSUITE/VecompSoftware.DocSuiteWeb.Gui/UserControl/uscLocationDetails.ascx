<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscLocationDetails.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscLocationDetails" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script type="text/javascript">
        var uscLocationDetails;
        require(["UserControl/uscLocationDetails"], function (UscLocationDetails) {
            $(function () {
                uscLocationDetails = new UscLocationDetails(tenantModelConfiguration.serviceConfiguration);
                uscLocationDetails.lblNameId = "<%=lblName.ClientID%>";
                uscLocationDetails.lblArchiveProtocolId = "<%=lblArchiveProtocol.ClientID%>";
                uscLocationDetails.lblArchiveDossierId = "<%=lblArchiveDossier.ClientID%>";
                uscLocationDetails.lblArchiveAttiId = "<%=lblArchiveAtti.ClientID%>";
                uscLocationDetails.pnlDetailsId = "<%= pnlDetails.ClientID %>";
                uscLocationDetails.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel">
    <div class="dsw-panel-content">
        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbDetails">
            <Items>
                <telerik:RadPanelItem Text="Informazioni" Expanded="true" Value="pnlInformations">
                    <ContentTemplate>
                        <asp:Panel runat="server">
                            <div class="col-dsw-10">
                                <b>Nome:</b>
                                <asp:Label runat="server" ID="lblName"></asp:Label>
                            </div>
                            <div class="col-dsw-10" id="divFolderName">
                                <b>Archivio Protocollo:</b>
                                <asp:Label runat="server" ID="lblArchiveProtocol"></asp:Label>
                            </div>
                            <div class="col-dsw-10">
                                <b>Archivio Dossier:</b>
                                <asp:Label runat="server" ID="lblArchiveDossier"></asp:Label>
                            </div>
                            <div class="col-dsw-10">
                                <b>Archivio Atti:</b>
                                <asp:Label runat="server" ID="lblArchiveAtti"></asp:Label>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </telerik:RadPanelItem>
            </Items>
        </telerik:RadPanelBar>
    </div>
</asp:Panel>
