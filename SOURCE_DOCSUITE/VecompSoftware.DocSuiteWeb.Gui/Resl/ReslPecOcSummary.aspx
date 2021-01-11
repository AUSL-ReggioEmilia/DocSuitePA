<%@ Page Title="Dettaglio PEC a Collegio Sindacale" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ReslPecOcSummary.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslPecOcSummary" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadAjaxPanel runat="server" ID="AjaxDataPanel">
        <table class="dataform">
            <tbody>
                <tr>
                    <td class="label" style="width: 25%;">
                        Tipologia:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="ReslType" />
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">
                        Data:
                    </td>
                    <td>
                        <asp:Label id="Data" runat="server" Text="" />
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">
                        Status:
                    </td>
                    <td>
                        <asp:Label id="Status" runat="server" Text="" />
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">
                        Estrai allegati:
                    </td>
                    <td>
                        <asp:CheckBox id="ExtractAttachment" runat="server" Enabled=false />
                    </td>
                </tr>
            </tbody>
        </table>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table class="dataform" id="editForm" runat="server">
        <tbody>
            <tr>
                <td class="label" style="width: 25%;">
                    Testo della mail:
                </td>
                <td>
                    <telerik:RadTextBox EmptyMessage="Inserire qui il testo" Height="240px" ID="MailBody" runat="server" TextMode="MultiLine" Width="100%" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 25%;">
                    &nbsp;
                </td>
                <td>
                    <asp:Button runat="server" ID="Confirm" Text="Conferma l'invio" />
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="Send" Text="Invia la richiesta al collegio sindacale" />
    <asp:Button runat="server" ID="Delete" Text="Elimina richiesta" />
</asp:Content>
