<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ProtParerDetail.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtParerDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <script language='javascript' id="InviaMail">
    //restituisce un riferimento alla radwindow
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseMe() {
            GetRadWindow().Close();
            return false;
        }


    </script>
<table class="datatable">
    <tr>
        <td class="label" style="width: 50%">Protocollo</td>
        <td style="width: 50%"><asp:Label runat="server" id="lblProtocol"></asp:Label></td>
    </tr>
    <tr>
        <td class="label">Data archiviazione</td>
        <td><asp:Label runat="server" id="lblArchivedDate"></asp:Label></td>
    </tr>
    <tr>
        <td class="label">Parer Uri</td>
        <td><asp:Label runat="server" id="lblParerUri"></asp:Label></td>
    </tr>
    <tr>
        <td class="label">Archivio</td>
        <td><asp:Label runat="server" id="lblIsForArchive"></asp:Label></td>
    </tr>
    <tr>
        <td class="label">Errori</td>
        <td><asp:Label runat="server" id="lblHasError"></asp:Label></td>
    </tr>
    <tr>
        <td class="label">Ultimo errore</td>
        <td><asp:Label runat="server" id="lblLastError"></asp:Label></td>
    </tr>
    <tr>
        <td class="label">Data ultima spedizione</td>
        <td><asp:Label runat="server" id="lblLastSendDate"></asp:Label></td>
    </tr>
</table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <div style="text-align: right; margin: 5px;">
        <asp:Button runat="server" ID="cmdClose" OnClientClick="return CloseMe(); " Text="Chiudi" />
    </div>
</asp:Content>
