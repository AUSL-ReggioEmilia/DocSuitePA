<%@  Title="Aggiunta Registro" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    CodeBehind="ReslJournalAdd.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslJournalAdd" %>
<%@ Register Src="~/Viewers/ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function <%= Me.ID %>_OpenPreview(filename, label, width, height) {
                var manager = $find("<%= RadWindowManagerProtocollo.ClientID %>");
                var path = "../Viewers/TempFileViewer.aspx?DownloadFile=" + filename + "&label=" + label;
                var wnd = manager.open(path, "Preview");
                wnd.setSize(width, height);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.center();
                return false;
            }

            function Preview_Click() {
                var previewButton = document.getElementById("<%= Preview.ClientID %>");
                previewButton.disabled = true;
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("preview");
                return false;
            }

            function Conferma_Click(arg) {
                var confermaButton = document.getElementById("<%= Conferma.clientID %>");
                confermaButton.disabled = true;
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("confirm|" + arg);
                return false;
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerProtocollo" runat="server">
        <Windows></Windows>
    </telerik:RadWindowManager>

    <telerik:RadAjaxPanel runat="server" ID="AjaxDataPanel">
        <table class="dataform">
            <tbody>
                <tr>
                    <td class="label" style="width: 25%;">Registro:
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="Templates" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">Anno:
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="Years" AutoPostBack="true" Enabled="false">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">Mese:
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="Months" Enabled="false">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="PagFromRow" visible="false">
                    <td class="label" style="width: 25%;">Numero Pagina Iniziale:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="PagFrom"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="PagToRow" visible="false">
                    <td class="label" style="width: 25%;">Numero Pagina Finale:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="PagTo"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscViewerLight CheckBoxes="true" DocumentSourcePage="BiblosDocumentHandler" ID="uscViewerLight" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadAjaxPanel runat="server" ID="Buttons">
        <asp:Button runat="server" ID="Preview" Text="Crea Registro" />
        <asp:Button runat="server" ID="Conferma" Text="Conferma inserimento" Enabled="false" />
    </telerik:RadAjaxPanel>
</asp:Content>
