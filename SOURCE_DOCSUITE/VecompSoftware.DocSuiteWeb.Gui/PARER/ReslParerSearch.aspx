<%@ Page Title="Verifica archiviazione" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    CodeBehind="ReslParerSearch.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslParerSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <td class="label labelPanel col-dsw-3">
                Tipologia:
            </td>
            <td class="col-dsw-7">
                <asp:DropDownList ID="ddlDocumentType" runat="server" AutoPostBack="false" AppendDataBoundItems="true">
                    <asp:ListItem Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="rfvDocumentType" ControlToValidate="ddlDocumentType"
                    Display="Dynamic" ErrorMessage="Campo Tipologia Documento Obbligatorio">
                </asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr id="trServizio" runat="server">
            <td class="label labelPanel col-dsw-3">
                Codice di Servizio:
            </td>
            <td>
                <asp:DropDownList ID="ddlServizio" runat="server" Visible="true" AutoPostBack="false">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="label labelPanel col-dsw-3">
                Anno:
            </td>
            <td class="col-dsw-7">
                <asp:TextBox ID="txtYear" runat="server" MaxLength="4" Width="56px" />
                <asp:RegularExpressionValidator runat="server" ID="vYear" ErrorMessage="Errore formato"
                    ControlToValidate="txtYear" ValidationExpression="\d{4}" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel col-dsw-3">
                Da Numero:
            </td>
            <td class="col-dsw-7">
                <asp:TextBox ID="txtNumberFrom" runat="server" Width="96px" MaxLength="7" />
                <asp:RegularExpressionValidator runat="server" ID="vNumber" ErrorMessage="Errore formato"
                    ControlToValidate="txtNumberFrom" ValidationExpression="\d*" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel col-dsw-3">
                A Numero:
            </td>
            <td class="col-dsw-7">
                <asp:TextBox ID="txtNumberTo" runat="server" Width="96px" MaxLength="7" />
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ErrorMessage="Errore formato"
                    ControlToValidate="txtNumberTo" ValidationExpression="\d*" />
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
