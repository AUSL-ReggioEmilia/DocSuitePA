<%@ Page Title="Firma tramite GoSign" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonGoSignFlow.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonGoSignFlow" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var commonGoSignFlow;
            require(["UserControl/CommonGoSignFlow"], function (CommonGoSignFlow) {
                $(function () {
                    commonGoSignFlow = new CommonGoSignFlow("<%= GoSignSessionIs %>", "<%= InfocertProxySignUrl %>");

                     commonGoSignFlow.btnSignCompletedId = "<%=btnSignCompleted.ClientID%>";
                     commonGoSignFlow.btnCancelId = "<%=btnCancel.ClientID %>";
                     commonGoSignFlow.windowManagerId = "<%=MasterDocSuite.DefaultWindowManager.ClientID%>";

                     commonGoSignFlow.initialize();
                 });
             });
        </script>
    </telerik:RadCodeBlock>

    <div class="warningArea">
        Nota bene! Una volta che lo stato della transazione è OK, fare click sul pulsante "Firma completata" per salvare il documento firmato.
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <iframe id="goSignFrame" width="100%" height="100%" style="border: none;"></iframe>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadPageLayout runat="server" HtmlTag="Div">
        <telerik:LayoutRow>
            <Content>
                <telerik:RadButton runat="server" ID="btnSignCompleted" Text="Firma completata" Width="150px"></telerik:RadButton>
                <telerik:RadButton runat="server" ID="btnCancel" Text="Annulla" Width="150px" Style="margin-left: 5px;"></telerik:RadButton>
            </Content>
        </telerik:LayoutRow>
    </telerik:RadPageLayout>
</asp:Content>
