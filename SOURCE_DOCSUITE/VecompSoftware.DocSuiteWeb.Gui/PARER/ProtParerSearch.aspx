<%@ Page Title="Verifica archiviazione" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ProtParerSearch.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtParerSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <td class="label labelPanel col-dsw-3">Anno:
            </td>
            <td class="col-dsw-7">
                <telerik:RadNumericTextBox ID="txtYear" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" Width="56px" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">Da numero:
            </td>
            <td>
                <asp:TextBox ID="txtNumberFrom" runat="server" Width="96px" MaxLength="7" />
                <asp:RegularExpressionValidator runat="server" ID="vNumber" ErrorMessage="Errore formato" ControlToValidate="txtNumberFrom" ValidationExpression="\d*" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">A numero:
            </td>
            <td>
                <asp:TextBox ID="txtNumberTo" runat="server" Width="96px" MaxLength="7" />
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ErrorMessage="Errore formato" ControlToValidate="txtNumberTo" ValidationExpression="\d*" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">Da data:
            </td>
            <td>
                <telerik:RadDatePicker ID="dateParerFrom" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">A data:
            </td>
            <td>
                <telerik:RadDatePicker ID="dateParerTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">Stato Archiviazione:
            </td>
            <td>
                <telerik:RadComboBox ID="rcbParerStatus" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="false"  Width="300" Culture="it-IT" ViewStateMode ="Enabled" CausesValidation ="False" EnableItemCaching="True" />                
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="cmdSearch" Text="Ricerca" />
</asp:Content>
