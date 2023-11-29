<%@ Page Title="Check In Multiplo" Language="vb" AutoEventWireup="false" CodeBehind="CollaborationVersioningManagement.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CollaborationVersioningManagement" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/Viewers/ViewerLight.ascx" TagName="ViewerLight" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="DocumentUpload" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadCodeBlock ID="rcbScripts" runat="server">
        <script type="text/javascript">

            function OpenMonitorWindow() {
                var browserWidth = document.body.clientWidth;
                var browserHeight = document.body.clientHeight;
                var oWnd = $find("<%= wndMonitor.ClientID%>");

                if (browserWidth && browserWidth > 0 && browserHeight && browserHeight > 0) {
                    var windowWidth = Math.ceil(browserWidth * 90 / 100);
                    var windowHeight = Math.ceil(browserHeight * 50 / 100);
                    oWnd.setSize(windowWidth, windowHeight);
                }

                oWnd.show();
                return false;
            }

            function ClearMonitor() {
                document.getElementById("<%= txtMonitor.ClientID%>").value = "";
                return false;
            }

            function CloseMonitor() {
                var oWnd = $find("<%= wndMonitor.ClientID%>");
                oWnd.close();
                return false;
            }

            var microsoftExcel = null;
            var microsoftWord = null;
            var handle = null;

            function InitMicrosoftWord() {
                if (!microsoftWord) {
                    try {
                        microsoftWord = new ActiveXObject("Word.Application");
                    }
                    catch (e) {
                        alert("Problema con l'apertura di Microsoft Word. Verificare le impostazioni di sicurezza.");
                        return false;
                    }
                }
                return true;
            }

            function InitMicrosoftExcel() {
                if (!microsoftExcel) {
                    try {
                        microsoftExcel = new ActiveXObject("Excel.Application");
                    }
                    catch (e) {
                        alert("Problema con l'apertura di Microsoft Excel. Verificare le impostazioni di sicurezza.");
                        return false;
                    }
                }
                return true;
            }

            function OpenDocuments(path) {
                if (InitMicrosoftWord()) {
                    try {
                        microsoftWord.Visible = true;
                        handle = microsoftWord.Documents.Open(path);
                        microsoftWord.WindowState = 2;
                        microsoftWord.WindowState = 1;
                        return true;
                    }
                    catch (e) {
                        alert("Problema con l'apertura di Microsoft Word. Contattare l'assistenza.");
                    }
                }
                return false;
            }

            function OpenWorkbooks(path) {
                if (InitMicrosoftExcel()) {
                    try {
                        microsoftExcel.visible = true;
                        handle = microsoftExcel.Workbooks.Open(path, 3, false);
                        microsoftExcel.WindowState = 2;
                        microsoftExcel.WindowState = 1;
                        return true;
                    }
                    catch (e) {
                        alert("Problema con l'apertura di Microsoft Excel. Contattare l'assistenza.");
                    }
                }
                return false;
            }

            function OpenAlert(path) {
                alert("Estensione non supportata.", path);
            }

            function OpenWord(path) {
                if (!OpenDocuments(path)) {
                    alert("Problema con l'apertura del file. Impossibile trovare il percorso del file. Contattare l'assistenza.");
                    return false;
                }
                return false;
            }

            function OpenExcel(path) {
                if (!OpenWorkbooks(path)) {
                    alert("Problema con l'apertura del file. Impossibile trovare il percorso del file. Contattare l'assistenza.");
                    return false;
                }
                return false;
            }
        </script>
    </telerik:RadCodeBlock>

    <div style="height: 60%;">
        <usc:ViewerLight ID="uscViewerLight" runat="server" CheckBoxes="True" CollapseOnSingleDocument="False" LeftPaneStartWidth="400" />
    </div>
    <div>
        <usc:DocumentUpload ID="uscCheckInDocuments" runat="server" Caption="Check In Nuove Versioni" IsDocumentRequired="False" MultipleDocuments="True" HideScannerMultipleDocumentButton="true" />
    </div>

    <telerik:RadWindow runat="server" ID="wndMonitor" Title="Dettaglio Operazioni" Behaviors="Close" KeepInScreenBounds="true" Width="700px" Height="500px">
        <ContentTemplate>
            <div style="width: 100%; height: 93%;">
                <telerik:RadTextBox ID="txtMonitor" runat="server" TextMode="MultiLine" ReadOnly="true" Width="100%" Height="100%" />
            </div>
            <div>
                <asp:Button ID="cmdClearMonitor" runat="server" Text="Pulisci" Width="200px" />
                <input type="button" id="cmdCloseMonitor" value="Chiudi" onclick="CloseMonitor();" style="width: 200px;" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="cmdCheckOut" runat="server" Text="Check Out" Width="200px" />
    <asp:Button ID="cmdUndoCheckOut" runat="server" Text="Annulla Check Out" Width="200px" />
    <asp:Button ID="cmdCheckIn" runat="server" Text="Check In Automatico" Width="200px" />
    <asp:Button ID="cmdCheckInManual" runat="server" Text="Check In Manuale" Width="200px" />
    <input type="button" id="cmdMonitor" runat="server" onclick="OpenMonitorWindow();" value="Dettaglio Operazioni" style="width: 200px;" />
    <asp:Button ID="cmdViewCollaboration" runat="server" Text="Pagina Precedente" Width="200px" />
</asp:Content>
