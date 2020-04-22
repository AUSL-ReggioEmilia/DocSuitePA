<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscInteropInfo.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscInteropInfo" %>

<table runat="server" id="tblValidationError" visible="false" class="datatable">
    <tr>
        <th colspan="2">Interoperabilità</th>
    </tr>
    <tr class="Chiaro">
         <td class="label">
            <b>Validazione:</b>
        </td>
        <td style="width: 85%;">
            <asp:Label ID="lblValidazioneErrore" runat="server" Width="100%"></asp:Label>
        </td>
    </tr>
</table>

<table runat="server" id="tblValidationOk" visible="false" class="datatable">
    <tr>
        <th colspan="2">Interoperabilità</th>
    </tr>
    <tr class="Chiaro">
        <td class="label">
            <b>Validazione:</b>
        </td>
        <td style="width: 85%;">
            <asp:Label ID="lblValidazione" runat="server" Width="100%"></asp:Label>
        </td>
    </tr>
    <tr class="Chiaro">
        <td class="label">
            <b>Oggetto:</b>
        </td>
        <td style="width: 85%;">
            <asp:Label ID="lblOggetto" runat="server" Width="100%"></asp:Label>
        </td>
    </tr>
    <tr class="Chiaro">
        <td class="label">
            <b>Documento Principale:</b>
        </td>
        <td style="width: 85%;">
            <asp:Label ID="lblDocumentPrincipale" runat="server" Width="100%"></asp:Label>
        </td>
    </tr>
    <tr class="Chiaro">
        <td class="label">
            <b>Mittente:</b>
        </td>
        <td style="width: 85%;">
            <asp:Label ID="lblMittente" runat="server" Width="100%"></asp:Label>
        </td>
    </tr>
    <tr class="Chiaro">
        <td class="label">
            <b>Protocollo mittente:</b>
        </td>
        <td style="width: 85%;">
            <asp:Label ID="lblProtocolloMittente" runat="server" Width="100%"></asp:Label>
        </td>
    </tr>

</table>
