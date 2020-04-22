<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocmAnnulla.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmAnnulla" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Annullamento" %>

<%@ Register Src="~/UserControl/uscDocument.ascx" TagName="UscDocument" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:UscDocument runat="server" ID="uscDocumentData" />
    <br />
    <table class="datatable">
        <tr>
            <th>
                Estremi del provvedimento di annullamento della Pratica
            </th>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox runat="server" ID="txtCancelDescription" Width="100%" MaxLength="255"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="rfvDescrizione" runat="server" Display="Dynamic"
                    ErrorMessage="Descrizione Obbligatoria" ControlToValidate="txtCancelDescription"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <DocSuite:PromptClickOnceButton ID="btnConfirm" runat="server" Text="Conferma annullamento" ConfirmBeforeSubmit="false" DisableAfterClick="true" />
</asp:Content>
