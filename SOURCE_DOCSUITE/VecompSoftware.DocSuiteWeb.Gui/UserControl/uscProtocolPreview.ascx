<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProtocolPreview.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtocolPreview" %>

<table class="datatable">
    <tr>
        <th colspan="2">
            <asp:Label ID="lblTitle" runat="server" Text="Protocollo Selezionato" />
        </th>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Protocollo</td>
        <td style="width: 85%;">
            <telerik:RadButton ButtonType="LinkButton" ID="cmdProtocol" runat="server" />
        </td>
    </tr>

    <tr>
        <td class="label">Tipo</td>
        <td>
            <asp:Label ID="lblType" runat="server" />
        </td>
    </tr>

    <tr runat="server" ID="trProtocolType" visible="false">
        <td class="label">Tipologia spedizione</td>
        <td>
            <asp:Label ID="lblDocType" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="label">Contenitore</td>
        <td>
            <asp:Label ID="lblContainer" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="label">Oggetto</td>
        <td>
            <asp:Label ID="lblObject" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="label">Classificazione</td>
        <td>
            <asp:Label ID="lblCategoryCode" runat="server" /><br />
            <asp:Label ID="lblCategoryDescription" runat="server" />
        </td>
    </tr>
</table>
