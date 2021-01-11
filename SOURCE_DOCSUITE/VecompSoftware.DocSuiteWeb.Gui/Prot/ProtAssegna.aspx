<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtAssegna" CodeBehind="ProtAssegna.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Modifica Assegnatario" %>

<%@ Register Src="~/UserControl/uscContattiSelText.ascx" TagName="uscContattiText" TagPrefix="uc" %>
<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <script type="text/javascript">
        function confirmSetStatus() {
            var protocolStatusConfirmRequest = <%=ProtocolStatusConfirm%>;
            var cmbProtocolStatus = document.getElementById("<%=cmbProtocolStatus.ClientID %>");
            if (protocolStatusConfirmRequest.length > 0 && cmbProtocolStatus != null) {
                var selectedValue = cmbProtocolStatus.value;
                var selectedText = cmbProtocolStatus.options[cmbProtocolStatus.selectedIndex].text;
                if (protocolStatusConfirmRequest.indexOf(selectedValue) == -1 || confirm("Sei sicuro di voler impostare il valore dello stato di protocollo a '" + selectedText + "'?")) {
                    return true;
                } else {
                    return false;
                }
            }
            return true;
        }
    </script>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlMainContent" Style="height: 100%">
        <table class="datatable">
            <tr>
                <th>Protocolli: 
                </th>
            </tr>
            <tr>
                <td>
                    <telerik:RadTreeView ID="rtvProtocol" Width="100%" runat="server" PersistLoadOnDemandNodes="true">
                    </telerik:RadTreeView>
                </td>
            </tr>
        </table>
        <%--status--%>
        <table id="tblEditProtocolStatus" class="datatable" runat="server">
            <tr>
                <th colspan="2">Stato del protocollo</th>
            </tr>
            <tr>
                <td class="label" style="width: 8%">Stato: </td>
                <td style="width: 85%;">
                    <asp:DropDownList DataTextField="Description" DataValueField="Id" Width="10%" ID="cmbProtocolStatus" runat="server" />
                </td>
            </tr>
        </table>
        <table runat="server" id="tblOther" class="datatable">
            <tr>
                <th colspan="2">Modifica assegnatario</th>
            </tr>
            <tr>
                <td class="label" style="width: 8%;">
                    <asp:Label Font-Bold="true" ID="lblSubject" runat="server" />Assegnatario: </td>
                <td style="width: 85%;">
                    <uc:uscContattiText ID="uscSubject" MaxLunghezzaTesto="1000" runat="server" TextBoxWidth="200px" ForceAddressBook="True" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConfermaAssegna" runat="server" Text="Conferma modifica" OnClientClick="if(!confirmSetStatus()) return false;" />
</asp:Content>
