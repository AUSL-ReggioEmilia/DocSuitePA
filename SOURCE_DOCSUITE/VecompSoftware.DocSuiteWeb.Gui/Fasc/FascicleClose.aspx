<%@ Page Title="Fascicoli da chiudere" Language="vb" AutoEventWireup="false" CodeBehind="FascicleClose.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascicleClose" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscFascGrid.ascx" TagName="uscFascGrid" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script type="text/javascript">
            var fascClose;
            require(["Fasc/FascicleClose"], function (FascicleClose) {
                $(function () {
                    fascClose = new FascicleClose(tenantModelConfiguration.serviceConfiguration);
                    fascClose.gridId = "<%= uscFascicleGrid.Grid.ClientID %>";
                    fascClose.btnCloseFasciclesId = "<%= btnCloseFascicles.ClientID %>";
                    fascClose.btnSelectAllId = "<%= btnSelectAll.ClientID %>";
                    fascClose.btnDeselectAllId = "<%= btnDeselectAll.ClientID %>";
                    fascClose.cancelFasciclesWindowId = "<%= windowCancelFascicles.ClientID %>";
                    fascClose.fasciclesTreeId = "<%= fasciclesTree.ClientID %>";
                    fascClose.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";

                    fascClose.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindow ID="windowCancelFascicles" Height="600" Width="750" runat="server" Title="Chiusura dei fascicoli">
        <ContentTemplate>
            <telerik:RadTreeView ID="fasciclesTree" runat="server" Width="100%">
                <Nodes>
                    <telerik:RadTreeNode runat="server" Text="Chiusura dei fascicoli" Value="" Font-Bold="true" />
                </Nodes>
            </telerik:RadTreeView>
        </ContentTemplate>
    </telerik:RadWindow>

    <div class="titolo" id="divTitolo" runat="server" visible="true">
        &nbsp;<asp:Label ID="lblHeader" runat="server" />
        <br class="Spazio" />
    </div>

</asp:Content>

<asp:Content ID="pageContent" runat="server" ContentPlaceHolderID="cphContent">
    <usc:uscFascGrid runat="server" ID="uscFascicleGrid" />
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server" Width="100%">
        <telerik:RadButton AutoPostBack="false" ID="btnCloseFascicles" runat="server" Width="130px" Text="Chiudi" />
        <telerik:RadButton AutoPostBack="false" ID="btnSelectAll" runat="server" Width="120px" Text="Seleziona tutti"></telerik:RadButton>
        <telerik:RadButton AutoPostBack="false" ID="btnDeselectAll" runat="server" Width="120px" Text="Annulla selezione"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
