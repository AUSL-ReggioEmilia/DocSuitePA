<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FascProcessInserimento.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.FascProcessInserimento"
    MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Fascicolo - Inserimento" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscFascicleProcessInsert.ascx" TagName="uscFascicleProcessInsert" TagPrefix="usc" %>


<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var fascInserimento;
            require(["Fasc/FascProcessInserimento"], function (FascProcessInserimento) {
                $(function () {
                    fascInserimento = new FascProcessInserimento(tenantModelConfiguration.serviceConfiguration);
                    fascInserimento.ajaxLoadingPanelId = "<%=MasterDocSuite.AjaxDefaultLoadingPanel.ClientId %>";
                    fascInserimento.btnInsertId = "<%= btnInsert.ClientID %>";
                    fascInserimento.idCategory = "<%= If(IdCategory.HasValue, IdCategory.Value, String.Empty) %>";
                    fascInserimento.idDocumentUnit = "<%= If(IdDocumentUnit.HasValue, IdDocumentUnit.Value, String.Empty) %>";
                    fascInserimento.pnlContentId = "<%= pnlContent.ClientID %>";
                    fascInserimento.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascInserimento.uscFascicleInsertId = "<%= uscFascicleProcessInsert.PageContentDiv.ClientID %>";
                    fascInserimento.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlContent">
        <usc:uscFascicleProcessInsert runat="server" ID="uscFascicleProcessInsert" />
    </asp:Panel>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnInsert" AutoPostBack="false" runat="server" CausesValidation="true" Width="150px" Text="Conferma inserimento" />
</asp:Content>
