<%@ Page AutoEventWireup="false" CodeBehind="ProtHighlightUser.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtHighlightUser" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione utente da Rubrica Aziendale" %>

<%@ Register Src="../UserControl/uscSearchADUser.ascx" TagName="uscUserSearch" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">

            function CloseWindow() {
                GetRadWindow().close();
            }

            function populateNote(text) {
                var textBox = $find("<%= rtbProtocolNotes.ClientID %>");
                if (text) {
                    textBox.set_value(text);
                    textBox.disable();
                } else {
                    textBox.set_value("");
                    textBox.enable();
                }
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc:uscUserSearch runat="server" ID="uscUserSearch" />
    <div style="width: 100%;">
        <table class="datatable">
            <tr id="note" style="display: none;" runat="server">
                <td class="label labelPanel" style="width: 30%;">Note:
                </td>
                <td>
                    <telerik:RadTextBox ID="rtbProtocolNotes" Width="300px" Rows="7" TextMode="MultiLine" runat="server" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button ID="btnConfirm" runat="server" Text="Conferma" />
    <asp:Button ID="btnHighlightToMe" runat="server" Text="Evidenzia a me" />
    <asp:Button ID="btnRemoveHighlight" runat="server" Text="Rimuovi evidenza" />
</asp:Content>
