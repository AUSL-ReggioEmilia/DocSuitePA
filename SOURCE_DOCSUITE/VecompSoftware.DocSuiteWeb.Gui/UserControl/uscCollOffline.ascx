<%@ Control Language="vb" AutoEventWireup="false" Codebehind="uscCollOffline.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscCollOffline" %>
<html>
<head>
    <title>Collaborazione Offline</title>
    <link href="../SubStyles/collOffline.css" type="text/css" rel="stylesheet">
</head>
<body>
    <div class="prot">
        <div class="titolo" id="divTitolo" runat="server">
            <asp:Label runat="server" ID="lblTitle"></asp:Label>
        </div>
        <br />
        <table class="datatable" style="width: 100%;">
            <tr>
                <th colspan="2">
                    <asp:Label runat="server" ID="lblDocTitle"></asp:Label></th>
            </tr>
            <tr>
                <td class="label">
                    Documento:
                </td>
                <td width="70%">
                    <asp:Panel ID="pnlDocument" runat="server" AccessKey=" ">
                    </asp:Panel>
                </td>
            </tr>
        </table>
        
        <br />
        <table class="datatable" style="width: 100%;">
            <tr>
                <th colspan="2">
                    Allegati</th>
            </tr>
            <tr>
                <td class="label">
                    Allegati:
                </td>
                <td width="70%">
                    <asp:Panel ID="pnlAttachments" runat="server" AccessKey=" ">
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <br />
        <table class="datatable" style="width: 100%;">
            <tr>
                <th colspan="2">
                    Dati</th>
            </tr>
            <tr>
                <td class="label">
                    Proponente:</td>
                <td width="70%">
                    <asp:Label ID="lblProposer" runat="server" /></td>
            </tr>
            <tr>
                <td class="label">
                    Destinatari:</td>
                <td width="70%">
                    <asp:Label ID="lblDestinations" runat="server" /></td>
            </tr>
            <tr>
                <td class="label">
                    <asp:Literal ID="lblMemorandumDateTitle" runat="server" Text="Data promemoria:"/>                    
                </td>
                <td width="70%">
                    <asp:Label ID="lblMemurandumDate" runat="server" /></td>
            </tr>
            <tr id="tAlertData" runat="server">
                <td class="label">
                    <asp:Literal ID="lblAlertDateTitle" runat="server" Text="Data Avviso:"/>                    
                </td>
                <td width="70%">
                    <asp:Label ID="lblAlertDate" runat="server" /></td>
            </tr>
            <tr>
                <td class="label">
                    Oggetto:</td>
                <td width="70%">
                    <asp:Label ID="lblObject" runat="server" /></td>
            </tr>
            <tr>
                <td class="label">
                    Note:</td>
                <td width="70%">
                    <asp:Label ID="lblNote" runat="server" /></td>
            </tr>
        </table>
    </div>
</body>
</html>
