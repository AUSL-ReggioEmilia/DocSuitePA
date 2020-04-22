<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltProtSospendi" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UtltProtSospendi.aspx.vb" Title="Sospensione numeri di Protocollo" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table class="datatable">
        <tr>
            <th colspan="2">Ultimo Protocollo utilizzato</th>
        </tr>
        <tr>
            <td class="label col-dsw-2">Anno/Numero:</td>
            <td class="col-dsw-8">
                <asp:TextBox ID="txtYearNumber" runat="server" Enabled="False" Width="200px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label col-dsw-2">Del:</td>
            <td class="col-dsw-8">
                <asp:TextBox ID="txtRegistrationDate" runat="server" Enabled="False" Width="106px" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th colspan="2">Protocolli da sospendere</th>
        </tr>
        <tr>
            <td class="label col-dsw-2">Anno di riferimento:</td>
            <td class="col-dsw-8">
                <telerik:RadComboBox runat="server" ID="rcbSelectSuspendYear" OnSelectedIndexChanged="rcbSelectSuspendYear_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false"></telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td class="label col-dsw-2">Numero Protocolli:</td>
            <td class="col-dsw-8">
                <telerik:RadNumericTextBox ID="txtSuspendNumber" NumberFormat-DecimalDigits="0" runat="server" Width="96px" MaxLength="7" AutoPostBack="True"></telerik:RadNumericTextBox>
                <asp:RequiredFieldValidator runat="server" ID="rfvSuspendNumber" ControlToValidate="txtSuspendNumber" ErrorMessage="Campo Numero Protocolli obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="label col-dsw-2">In data:</td>
            <td class="col-dsw-8">
                <telerik:RadDatePicker ID="rdpSuspendDate" runat="server" Style="height: auto !important;" />
                <asp:TextBox ID="txtCurrentDate" runat="server" Width="40px" MaxLength="10" CssClass="hiddenField"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="rfvSuspendDate" ControlToValidate="rdpSuspendDate" ErrorMessage="Data obbligatoria" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvCompareData" runat="server" ErrorMessage="La data di sospensione deve essere maggiore o uguale alla data di registrazione dell'ultimo Protocollo."
                        ControlToValidate="rdpSuspendDate" Display="Dynamic" Type="Date" Operator="GreaterThanEqual"
                        ControlToCompare="txtRegistrationDate"></asp:CompareValidator>
                <asp:CompareValidator ID="cvCompareData2" runat="server" ErrorMessage="La data di sospensione deve essere minore o uguale alla data massima dell'anno selezionato."
                        ControlToValidate="rdpSuspendDate" Display="Dynamic" Type="Date" Operator="LessThanEqual"
                        ControlToCompare="txtCurrentDate"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <th colspan="2">Ultimo Protocollo sospeso</th>
        </tr>
        <tr>
            <td class="label col-dsw-2">Numero:</td>
            <td class="col-dsw-8">
                <asp:TextBox ID="txtSuspendToNumber" runat="server" Enabled="False" Width="200px"></asp:TextBox>
            </td>
        </tr>
    </table>

    <asp:Repeater ID="rptSospesiResults" runat="server">
        <HeaderTemplate>
            <b>Sono stati sospesi correttamente i seguenti Protocolli:</b>
            <br />
        </HeaderTemplate>
        <ItemTemplate>
            <b><%# Container.DataItem.ToString() %></b>
            <br />
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma"></asp:Button>
    <asp:ValidationSummary DisplayMode="List" ID="vs" runat="server" ShowMessageBox="True" ShowSummary="False" Width="112px" />
</asp:Content>
