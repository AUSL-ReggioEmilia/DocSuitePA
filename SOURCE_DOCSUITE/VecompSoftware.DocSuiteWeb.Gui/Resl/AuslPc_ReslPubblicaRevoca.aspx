<%@ Page Title="Atti - Pubblicazione" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="AuslPc_ReslPubblicaRevoca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.AuslPc_ReslPubblicaRevoca" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblTitolo" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function checkNumeric(source, args) {
                args.IsValid = !isNaN(args);
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(true);
                return false;
            }
            
            function ExecuteAjaxRequest(operationName)
            {
                var manager = <%= AjaxManager.ClientID %>;
                manager.ajaxRequest(operationName);
            }
        </script>
    </telerik:RadScriptBlock>
    <%-- Data Flusso --%>
    <asp:Panel ID="pnlData" runat="server">
        <table class="datatable" width="100%">
            <tr>
                <th colspan="2">Data flusso
                </th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%">Data:
                </td>
                <td style="width: 80%">
                    <telerik:RadDatePicker ID="txtData" DateInput-DateFormat="dd/MM/yyyy" runat="server">
                        <Calendar ID="Calendar1" runat="server" CellAlign="Center" CellVAlign="Middle" DayNameFormat="FirstLetter"
                            FirstDayOfWeek="Default" MonthLayout="Layout_7columns_x_6rows" Orientation="RenderInRows"
                            TitleAlign="Center">
                        </Calendar>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator ID="rfvData" runat="server" ControlToValidate="txtData"
                        ErrorMessage="Data inizio obbligatoria" Display="Dynamic"></asp:RequiredFieldValidator>
                    <br />
                    <asp:CompareValidator ID="cvCompareData" runat="server" Type="Date" Operator="GreaterThanEqual"
                        ControlToValidate="txtData" ErrorMessage="La data del flusso deve essere maggiore o uguale a "
                        Display="Dynamic" ControlToCompare="txtLastWorkflowDate"></asp:CompareValidator>
                </td>
            </tr>
            <tr class="Spazio">
                <td></td>
            </tr>
        </table>
    </asp:Panel>
    <%-- Documento --%>
    <asp:Panel ID="pnlDocumento" runat="server">
        <table class="datatable" width="100%">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblDocumento" runat="server"></asp:Label>
                </th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%">Documento:
                </td>
                <td style="width: 80%">
                    <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="true" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadDocumenti" IsDocumentRequired="true" MultipleDocuments="false" runat="server" TreeViewCaption="Documento" Type="Resl" WindowWidth="620" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    
  

    <%-- Allegati --%>
    <asp:Panel ID="pnlAttach" runat="server" Style="display: none;">
        <table class="datatable" width="100%">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblAllegati" runat="server"></asp:Label>
                </th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%">Allegati:
                </td>
                <td style="width: 80%">
                    <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadAllegati" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" TreeViewCaption="Allegati" Type="Resl" WindowWidth="620" />
                </td>
            </tr>
            <tr class="Spazio">
                <td></td>
            </tr>
        </table>
    </asp:Panel>

    <%-- Allegati Riservati --%>
    <asp:Panel ID="pnlPrivacyAttachment" runat="server" Style="display: none;">
        <table class="datatable" width="100%">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblPrivacyAttachment" runat="server"></asp:Label>
                </th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%">Allegati:
                </td>
                <td style="width: 80%">
                    <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadPrivacyAttachment" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" TreeViewCaption="Allegati Riservati" Type="Resl" WindowWidth="620" />
                </td>
            </tr>
            <tr class="Spazio">
                <td></td>
            </tr>
        </table>
    </asp:Panel>

    <%-- Allegati non parte integrante (Annessi) --%>
    <asp:Panel ID="pnlAnnexes" runat="server">
        <table class="datatable" width="100%">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblAnnexes" runat="server"></asp:Label>
                </th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%">
                    Annessi (non parte integrante):
                </td>
                <td style="width: 80%">
                    <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadAnnexes" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" TreeViewCaption="Annessi (non parte integrante)" Type="Resl" WindowWidth="620" />
                </td>
            </tr>
            <tr class="Spazio">
                <td>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <%-- Privacy Piacenza --%>
    <asp:Panel ID="pnlOptions" runat="server">
        <table class="datatable" width="100%">
            <tr>
                <th colspan="2">Opzioni</th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%">Usare versione privacy:
                </td>
                <td style="width: 80%">
                    <asp:RadioButtonList ID="selectPrivacy" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Si" Value="True"></asp:ListItem>
                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator runat="server" ID="rfvSelectPrivacy" Enabled="false" ControlToValidate="selectPrivacy" Display="Dynamic" ErrorMessage="Opzione Privacy Obbligatoria">
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr class="Spazio">
                <td></td>
            </tr>
        </table>
    </asp:Panel>
    <%-- Documenti privacy Piacenza --%>
    <asp:Panel ID="pnlPrivacy" runat="server">
        <table class="datatable" width="100%">
            <tr>
                <th colspan="2">Opzioni Privacy</th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%; text-align: left;">1) Stampare:
                </td>
                <td style="width: 80%">
                    <asp:HyperLink ID="privacyImageToPrint" runat="server" ToolTip="File da stampare" Target="_blank"></asp:HyperLink>
                    <asp:HyperLink ID="privacyTextToPrint" runat="server" Text="File da stampare" Target="_blank"></asp:HyperLink>
                </td>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%; text-align: left;">2) Caricare con omissis:
                </td>
                <td style="width: 80%">
                    <usc:UploadDocument ButtonFDQEnabled="true" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="documentUploader" IsDocumentRequired="false" MultipleDocuments="false" runat="server" TreeViewCaption="Documento" Type="Resl" WindowWidth="620" />
                </td>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%; text-align: left;">3) Inserire gli omissis nell'oggetto
                </td>
                <td style="width: 80%">
                    <telerik:RadTextBox ID="txtPrivacyObject" MaxLength="1500" runat="server" Width="100%" />
                    <asp:RequiredFieldValidator ControlToValidate="txtPrivacyObject" Display="Dynamic" ID="rfvPrivacyObject" runat="server" Enabled="False" />
                </td>
            </tr>
            <tr class="Spazio">
                <td></td>
            </tr>
        </table>
    </asp:Panel>s
    <asp:Label ID="lblInfo" runat="server" Visible="False" ForeColor="Red">Manca la data di Ricezione in Regione. Impossibile rendere esecutiva la Delibera.</asp:Label>
    <asp:TextBox ID="txtLastWorkflowDate" runat="server" Width="16px"></asp:TextBox>
    <asp:TextBox ID="txtIdLocation" runat="server" Width="16px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvLocazione" runat="server" ControlToValidate="txtIdLocation" ErrorMessage="Locazione Obbligatoria" Display="Dynamic"></asp:RequiredFieldValidator>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma"></asp:Button>
</asp:Content>
