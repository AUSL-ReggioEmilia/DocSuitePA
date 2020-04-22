<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FascRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascRisultati" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscFascGrid.ascx" TagName="uscFascGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script type="text/javascript">
            var fascRisultati;
            require(["Fasc/FascRisultati"], function (FascRisultati) {
                $(function () {
                    fascRisultati = new FascRisultati(tenantModelConfiguration.serviceConfiguration);
                    fascRisultati.gridId = "<%= uscFascicleGrid.Grid.ClientID %>";
                    fascRisultati.selectableFasciclesThreshold = "<%= SelectableFaciclesThreshold%>";
                    fascRisultati.btnDocumentsId = "<%= btnDocuments.ClientID %>";
                    fascRisultati.btnSelectAllId = "<%= btnSelectAll.ClientID %>";
                    fascRisultati.btnDeselectAllId = "<%= btnDeselectAll.ClientID %>";
                    fascRisultati.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascRisultati.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascRisultati.initialize();
                });                
            });

            function GetRadWindow() {
                return fascRisultati.getRadWindow();
            }

            function CloseWindow(value) {
                fascRisultati.closeWindow(value);
            }
        </script>
    </telerik:RadScriptBlock>

    <div class="titolo" id="divTitolo" runat="server" visible="true">
        &nbsp;<asp:Label ID="lblHeader" runat="server" />
        <br class="Spazio" />
    </div>
</asp:Content>

<asp:Content ID="pageContent" runat="server" ContentPlaceHolderID="cphContent">
    <uc1:uscFascGrid runat="server" ID="uscFascicleGrid" />
      <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server" Width="100%">    
    <telerik:RadButton id="btnDocuments" runat="server" width="130px" text="Visualizza documenti" />
	<telerik:RadButton id="btnSelectAll" runat="server" width="120px" text="Seleziona tutti"></telerik:RadButton>
	<telerik:RadButton id="btnDeselectAll" runat="server" width="120px" text="Annulla selezione"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
