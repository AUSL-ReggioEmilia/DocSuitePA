<%@ Page Title="Dettaglio Registro" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    CodeBehind="ReslJournalSummary.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslJournalSummary" %>
<%@ Register Src="~/Viewers/ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function <%= Me.ID %>_OpenWindow(url, name, width, height, filename)
            {	
                var manager = $find("<%=RadWindowManager.ClientID %>");
                var path = url + "?" + "TempFileName=" + filename + "&PrintFileName=Registro";
                var wnd = manager.open(path, name);         
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.setSize(width, height);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.center();
                return false;
            }

            function <%= Me.ID %>_OpenWindowSign(url, name, parameters) {
                HidePDFActivex();
                var wnd = $find(name);
                wnd.setUrl(url + "?" + parameters);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Close);
                wnd.show();
                wnd.center();
                return false;
            }
    
            // richiamata quando la finestra di firma viene chiusa
            function <%= Me.ID %>_CloseSignWindow(sender, args) {                
                ToggleToolBar(getToolBar(), true);
                ShowPDFActivex();
                if (args.get_argument() !== null) {
                    var argument = "<%= Me.ClientID %>|SIGN|" + args.get_argument();
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
                }
            }            

            function <%= Me.ID %>_OpenPreview(path, width, height) {                
                var manager = $find("<%= RadWindowManager.ClientID %>");
                var wnd = manager.open(path, "Preview");
                wnd.setSize(width,height);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);                
                wnd.set_modal(true);
                wnd.center();
                return false;
            }

            function DeleteRegConfirm() {
                var ajaxManager = <%= AjaxManager.ClientID %>;
                var r = confirm("Eliminare il registro corrente?");
                if (r == true) {
                    ajaxManager.ajaxRequest("<%= Me.ClientID %>" + "|delete|");
                    return false;
                }
                return false;
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadWindowManager BackColor="Gray" Behaviors="Close" DestroyOnClose="true" EnableViewState="False" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow Behaviors="Close" BackColor="Gray" DestroyOnClose="True" Width="800" Height="490" ID="signDocument" InitialBehaviors="Maximize" ReloadOnShow="True" runat="server" Title="Firma Documento" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxPanel runat="server" ID="DataPanel">
        <table class="dataform">
            <tbody>
                <tr>
                    <td class="label" style="width: 25%;">Descrizione:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="Desc"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">Registro:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="Template"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">Anno:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="Year"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">Mese:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="Month"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="FirstPageNumberRow" visible="false">
                    <td class="label" style="width: 25%;">Pagina Iniziale:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="FirstPageNumber"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="LastPageNumberRow" visible="false">
                    <td class="label" style="width: 25%;">Pagina Finale:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LastPageNumber"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">Numero di Atti:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="CountResolutions"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">Data ultima firma:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="SignatureDate"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 25%;">Utente ultima firma:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="SignatureUser"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
        <asp:Button ID="Show" runat="server" Text="Visualizza Registro" />
    </telerik:RadAjaxPanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscViewerLight CheckBoxes="true" DocumentSourcePage="BiblosDocumentHandler" ID="uscViewerLight" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="Sign" Text="Firma" />
    <asp:Button runat="server" ID="Delete" Text="Elimina registro" />
    <asp:Button runat="server" ID="Summary" Text="Torna all'elenco dei registri" />
</asp:Content>
