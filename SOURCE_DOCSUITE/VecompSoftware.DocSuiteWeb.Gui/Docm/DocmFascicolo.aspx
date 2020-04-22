<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocmFascicolo.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmFascicolo" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscFascicleSelect.ascx" TagName="UscFascicleSelect" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentFolder.ascx" TagName="UscDocumentFolder" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock2">
        <script type="text/javascript" language="javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(value) {
                var oWindow = GetRadWindow();
                oWindow.close(value);
            }
        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <th align="left" colspan="2">Informazioni</th>
        </tr>
        <tr>
            <td style="width: 155px" class="label">Motivo:</td>
            <td>
                <telerik:RadTextBox ID="txtReason" runat="server" Width="100%" MaxLength="255" />
            </td>
        </tr>
        <tr>
            <td style="width: 155px" class="label">Note:</td>
            <td>
                <telerik:RadTextBox ID="txtNote" runat="server" Width="100%" MaxLength="255" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:UscFascicleSelect runat="server" ID="uscFascicleSelect" />

    <asp:Panel ID="pnlCartella" runat="server">
        <table style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; width: 100%; border-bottom: gray 1px solid; border-collapse: collapse; border-color: Gray;" cellspacing="0" cellpadding="3" border="0" class="datatable">
            <tr>
                <th>Cartella Del Documento</th>
            </tr>
            <tr>
                <td>
                    <usc:UscDocumentFolder ID="uscDocumentFolderProt" runat="server" Type="Resl" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnInserimento" runat="server" Text="Conferma" Enabled="False" />
    <asp:Button ID="btnModifica" runat="server" Text="Modifica" />
    <asp:Button ID="btnCancella" runat="server" Text="Cancella" />
</asp:Content>


