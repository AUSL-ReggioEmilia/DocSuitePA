<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionKindDetails.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionKindDetails" %>

<telerik:RadCodeBlock runat="server">
    <script type="text/javascript">
        var uscResolutionKindDetails;
        require(["UserControl/UscResolutionKindDetails"], function (UscResolutionKindDetails) {
            $(function () {
                uscResolutionKindDetails = new UscResolutionKindDetails(tenantModelConfiguration.serviceConfiguration);
                uscResolutionKindDetails.lblStatusId = "<%= lblStatus.ClientID %>";
                uscResolutionKindDetails.pnlInformationsId = "<%= pnlInformations.ClientID %>";
                uscResolutionKindDetails.initialize();
            });
        });
    </script>
</telerik:RadCodeBlock>

<asp:Panel runat="server" ID="pnlInformations">
    <div class="col-dsw-10">
        <b>Stato:</b>
        <asp:Label runat="server" ID="lblStatus"></asp:Label>
    </div>
</asp:Panel>
