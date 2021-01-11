<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.AllineaProgressiviProtocollo" MasterPageFile="~/MasterPages/DocSuite2008.Master" Codebehind="AllineaProgressiviProtocollo.aspx.vb" Title="Allinea Progressivi Protocollo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
    <script language="javascript" type="text/javascript">
        function OnRequestStart(sender, args) {
            document.getElementById('btnConfermaV').disabled = true;
        }
        function OnResponseEnd(sender, args) {
            document.getElementById('btnConfermaV').disabled = false;
        }
    </script>


      <table class="Table">
        <tr>
            <td width="30%">
            </td>
            <td width="70%">
            </td>
        </tr>
            <tr class="tabella">
                <td class="Scuro" align="left" colspan="2">
                    Ultimo Protocollo Utilizzato</td>
            </tr>
        <tr class="Chiaro">
            <td align="right">
                <b>Anno/Numero:&nbsp;</b></td>
            <td align="left">
                <asp:TextBox ID="txtYearNumber" runat="server" Enabled="False" Width="200px"></asp:TextBox></td>
        </tr>
        <tr class="tabella" align="right">
            <td class="Scuro" align="left" colspan="2">
                Prossimo numero protocollo previsto</td>
        </tr>
        <tr class="Chiaro" align="right">
            <td align="right">
                <b>Numero:&nbsp;</b></td>
            <td align="left">
                <asp:TextBox runat="server" ID="txtParameterNumber" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>

        <tr class="Chiaro" align="left">
            <td align="left" colspan="2">
                <asp:Button ID="btnConferma" runat="server" Text="Allinea progressivo"></asp:Button>
            </td>
        </tr>

    </table>
</asp:Content>
