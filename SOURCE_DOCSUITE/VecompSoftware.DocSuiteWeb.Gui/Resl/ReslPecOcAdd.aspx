<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    CodeBehind="ReslPecOcAdd.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslPecOcAdd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <table class="dataform" runat="server" ID="dataPanel">
        <tbody>
            <tr>
                <td class="label" style="width: 25%;">
                    Tipologia:
                </td>
                <td>
                    <asp:DropDownList AutoPostBack="true" ID="ReslType" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 25%;">
                    <asp:Label id="From" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <telerik:RadDatePicker ID="DateFrom" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="DateFrom" ErrorMessage="Data obbligatoria" runat="server" />
                </td>
            </tr>
            <tr id="rowExtractOneDay" runat="server">
                <td class="label" style="width: 25%;">
                    <asp:Label ID="extractOneDay" runat="server" Text="Estrai singolo giorno"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox id="chkExtractOneDay" Checked="True" runat="server" />
                </td>
            </tr>
            <tr id="DateToRow" runat="server">
                <td class="label" style="width: 25%;">
                    <asp:Label id="To" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <telerik:RadDatePicker ID="DateTo" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="DateTo" ErrorMessage="Data obbligatoria" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 25%;">
                    Estrai allegati:
                </td>
                <td>
                    <asp:CheckBox id="ExtractAttachment" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="Conferma" Text="Conferma inserimento" />
</asp:Content>
