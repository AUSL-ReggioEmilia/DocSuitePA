<%@ Control AutoEventWireup="false" CodeBehind="uscDocumentToken.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentToken" Language="vb" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscDocumentDati.ascx" TagName="uscDocmDati" TagPrefix="uc" %>

<table runat="server" id="tblTipologiaRichiesta" class="datatable">
    <tr>
        <th colspan="2">
            Tipologia Richiesta
        </th>
    </tr>
    <tr runat="server" id="tblRowStep">
        <td class="label" style="vertical-align:middle;">
            Step:&nbsp;</td>
        <td width="80%">
            <asp:TextBox ID="txtStep" runat="server" Font-Bold="True" Enabled="False" Width="50px"></asp:TextBox>
        </td>
    </tr>
    <tr runat="server" id="tblRowType">
        <td class="label" style="vertical-align:middle;">
            Tipologia:&nbsp;</td>
        <td width="80%">
            <div style="float:left;">
                <asp:TextBox ID="txtTokenType" runat="server" Font-Bold="True" Enabled="False" Width="50px"></asp:TextBox>
            </div>
            <asp:TextBox ID="txtTokenName" runat="server" Font-Bold="True" Enabled="False" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr runat="server" id="tblRowReturn">
        <td class="label" style="vertical-align:middle;">
            Restituzione:&nbsp;</td>
        <td width="80%">
            <asp:RadioButtonList ID="rblRestituzione" runat="server" AutoPostBack="True">
                <asp:ListItem Value="Si">Si</asp:ListItem>
                <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
            </asp:RadioButtonList></td>
    </tr>
</table>

<table runat="server" id="tblMovRichiesta" class="datatable">
    <tr>
        <th colspan="2">
            Movimentazione Richiesta</th>
    </tr>
    <tr>
        <td width="50%">
            <uc:uscSettori runat="server" ID="uscMittenteMovimentazione" HeaderVisible="false"
                Required="false" ReadOnly="true" Type="Docm" Caption="Settore Mittente" />
        </td>
        <td width="50%">
            <uc:uscSettori ID="uscDestinatarioMovimentazione" runat="server" HeaderVisible="false"
                Required="false"  Type="Docm" Caption="Settore Destinazione" />
        </td>
    </tr>
</table>

<table runat="server" id="tblSettoriCC" class="datatable">
    <tr>
        <th colspan="2">
            Settori in Copia Conoscenza</th>
    </tr>
    <tr>
        <td width="50%"></td>
        <td width="50%">
            <uc:uscSettori runat="server" ID="uscSettoriCC" Caption="Settori in Copia Conoscenza" Required="false" Type="Docm" HeaderVisible="false" />
        </td>
    </tr>
</table>

<uc:uscDocmDati runat="server" id="uscRequestDocument" width="100%" HeaderText="Dati Richiesta" />
