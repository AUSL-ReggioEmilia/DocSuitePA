<%@ Page AutoEventWireup="false" CodeBehind="ReslFlusso.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslFlusso" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Atti - Gestione Flusso" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="UploadDocument" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblTitolo" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
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

            function ForzaAvanzamento(source, args) {
                var chkForzaAvanzamento = document.getElementById("<%= chkForzaAvanzamento.ClientID %>");
                if (chkForzaAvanzamento && chkForzaAvanzamento.checked == false) {
                    args.IsValid = false;
                }
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(true);
                return false;
            }
        </script>
    </telerik:RadScriptBlock>

    <asp:Panel ID="pnlreslFlussoContent" runat="server" Width="100%">
        <%-- NextStep --%>
        <asp:Panel ID="pnlNextStep" runat="server">
            <table width="100%">
                <tr class="titolo">
                    <td align="center">
                        <asp:Label ID="lblActualStep" runat="server"></asp:Label>
                        <asp:Label ID="Label2" runat="server" Width="20px">  ->  </asp:Label>
                        <asp:Label ID="lblNextStep" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Forza Avanzamento --%>
        <asp:Panel ID="pnlForzaAvanzamento" runat="server" Visible="false">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">Forza Avanzamento
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td>
                        <label style="font-weight: bold;">
                            Attenzione:</label>&nbsp;<asp:Label ID="lblForzaAvanzamento" runat="server" /><br />
                        <br />
                        <asp:CheckBox ID="chkForzaAvanzamento" runat="server" Text="Forza Avanzamento" Font-Bold="true" /><br />
                        <asp:CustomValidator ID="cvForzaAvanzamento" runat="server" Display="Dynamic" ClientValidationFunction="ForzaAvanzamento" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Frontalino digitale --%>
        <asp:Panel ID="pnlFrontalino" runat="server" Visible="false">
            <table class="datatable" style="width: 100%">
                <tr>
                    <th colspan="2">
                        <asp:Label ID="lblFrontalino" runat="server" Text="Frontalino: "></asp:Label>
                    </th>
                </tr>
                <tr>
                    <td class="label" style="width: 15%">Data:
                    </td>
                    <td style="width: 85%">
                        <asp:RadioButtonList ID="radioFrontalino" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Selected="True" Text="Cartaceo" Value="frontalinocartaceo"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="Digitale" Value="frontalinodigitale"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
        </asp:Panel>
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
                        <telerik:RadDatePicker ID="txtData" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtData" Display="Dynamic" ErrorMessage="Data inizio obbligatoria" ID="rfvData" runat="server" />
                        <br />
                        <asp:CompareValidator ID="cvCompareData" runat="server" Type="Date" Operator="GreaterThanEqual"
                            ControlToValidate="txtData" ErrorMessage="La data del workflow deve essere maggiore o uguale a "
                            Display="Dynamic" ControlToCompare="txtLastWorkflowDate"></asp:CompareValidator>
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Numero di Servizio --%>
        <asp:Panel ID="pnlNumeroServizio" runat="server">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">Numero di servizio
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td class="label" style="width: 20%">
                        <asp:Label runat="server" ID="lblNumeroServizio" Text="Codice/Numero"></asp:Label>
                    </td>
                    <td style="width: 80%">
                        <asp:DropDownList ID="ddlServizio" runat="server" Visible="False" AutoPostBack="true">
                        </asp:DropDownList>
                        &nbsp;
                    <span runat="server" id="ddlServizioSeparator" visible="False">/&nbsp;</span>
                        <asp:TextBox ID="txtNumeroServizio" runat="server" MaxLength="255"></asp:TextBox>
                        &nbsp;
                    <asp:CompareValidator EnableClientScript="true" ID="revNumero" runat="server" ControlToValidate="txtNumeroServizio" ErrorMessage="Formato Errato" Operator="DataTypeCheck" Type="Integer" />
                        <asp:RequiredFieldValidator ID="rfvServizio" runat="server" ControlToValidate="ddlServizio" ErrorMessage="<br />Codice di servizio Obbligatorio" Display="Dynamic" Visible="True" />
                        <asp:RequiredFieldValidator ID="rfvNumero" runat="server" ControlToValidate="txtNumeroServizio" ErrorMessage="<br />Numero Obbligatorio" Display="Dynamic" />
                        &nbsp;
                    </td>
                </tr>
                <tr class="Chiaro">
                    <td colspan="2" style="width: 100%">
                        <asp:CheckBox ID="chkTuttiICodici" runat="server" AutoPostBack="True" Text="Visualizza tutti i codici servizio"
                            Visible="False" />
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
                    <td colspan="2">
                        <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="true" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadDocumenti" IsDocumentRequired="true" MultipleDocuments="false" runat="server" TreeViewCaption="Documento" Type="Resl" WindowWidth="620" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
                <%-- Document Pages --%>
                <asp:Panel ID="pnlDocPages" runat="server">
                    <tr class="Chiaro">
                        <td class="label">Numero di pagine:
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txtDocPage" AutoPostBack="False" runat="server"></telerik:RadTextBox>
                        </td>
                    </tr>
                    <tr class="Spazio">
                        <td></td>
                    </tr>
                </asp:Panel>
            </table>
        </asp:Panel>
        <%-- Documenti Omissis --%>
        <asp:Panel ID="pnlDocumentiOmissis" runat="server">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">
                        <asp:Label ID="lblDocumentiOmissis" Text="Documenti Omissis" runat="server"></asp:Label>
                    </th>
                </tr>
                <tr class="Chiaro">

                    <td colspan="2">
                        <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadDocumentiOmissis" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" TreeViewCaption="Documenti Omissis" Type="Resl" WindowWidth="620" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Allegati --%>
        <asp:Panel ID="pnlAttach" runat="server">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">
                        <asp:Label ID="lblAllegati" Text="Allegati (parte integrante)" runat="server"></asp:Label>
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td colspan="2">
                        <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadAllegati" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" TreeViewCaption="Allegati (parte integrante)" Type="Resl" WindowWidth="620" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Allegati Omissis --%>
        <asp:Panel ID="pnlAttachOmissis" runat="server">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">
                        <asp:Label ID="lblAllegatiOmissis" Text="Allegati Omissis (parte integrante)" runat="server"></asp:Label>
                    </th>
                </tr>
                <tr class="Chiaro">
 
                    <td colspan="2">
                        <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadAllegatiOmissis" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" TreeViewCaption="Allegati Omissis (parte integrante)" Type="Resl" WindowWidth="620" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Allegati Riservati --%>
        <asp:Panel ID="pnlPrivacyAttachment" runat="server">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">
                        <asp:Label ID="lblPrivacyAttachment" Text="Allegati Riservati" runat="server"></asp:Label>
                    </th>
                </tr>
                <tr class="Chiaro">
              
                    <td colspan="2">
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
                        <asp:Label ID="lblAnnexes" Text="Annessi (non parte integrante)" runat="server"></asp:Label>
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td colspan="2">
                        <usc:UploadDocument ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadAnnexes" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" TreeViewCaption="Annessi (non parte integrante)" Type="Resl" WindowWidth="620" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Motivazione --%>
        <asp:Panel ID="pnMotivazione" runat="server">
            <table class="datatable" width="100%">
                <tr class="Chiaro">
                    <td class="label" style="width: 20%">Motivo:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="txtMotivazione" runat="server" Width="100%" TextMode="MultiLine" Rows="5"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="Requiredfieldvalidator1" runat="server" Display="Dynamic" ErrorMessage="Motivazione Obbligatoria" ControlToValidate="txtMotivazione" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Privacy --%>
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
        <%-- Documenti privacy --%>
        <asp:Panel ID="pnlPrivacy" runat="server">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">Opzioni Privacy</th>
                </tr>
                <tr class="Chiaro">
                    <td class="label" style="width: 20%">1) Stampare:
                    </td>
                    <td style="width: 80%">
                        <asp:HyperLink ID="privacyImageToPrint" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/file_extension_pdf.png" ToolTip="File da stampare" Target="_blank"></asp:HyperLink>
                        <asp:HyperLink ID="privacyTextToPrint" runat="server" Text="File da stampare" Target="_blank"></asp:HyperLink>
                    </td>
                </tr>
                <tr class="Chiaro">
                    <td class="label" style="width: 20%">2) Caricare con omissis:
                    </td>
                    <td style="width: 80%">
                        <usc:UploadDocument ButtonFDQEnabled="true" ButtonFileEnabled="true" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="true" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="documentUploader" IsDocumentRequired="false" MultipleDocuments="false" runat="server" TreeViewCaption="Documento" Type="Resl" WindowWidth="620" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Label ID="lblInfo" runat="server" Visible="False" ForeColor="Red">Manca la data di Ricezione in Regione. Impossibile rendere esecutiva la Delibera.</asp:Label>
        <asp:TextBox ID="txtLastWorkflowDate" runat="server" Width="16px"></asp:TextBox>
        <asp:TextBox ID="txtIdLocation" runat="server" Width="16px"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvLocazione" runat="server" ControlToValidate="txtIdLocation" ErrorMessage="Locazione Obbligatoria" Display="Dynamic"></asp:RequiredFieldValidator>
        <%-- Selezione documenti automatismi --%>
        <asp:Panel runat="server" ID="pnlAutomatismi" Visible="False">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">Pubblicazione Web
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 80%">
                        <telerik:RadTreeView EnableViewState="true" ID="rtvListDocument" runat="server" MultipleSelect="true" CheckBoxes="True">
                            <Nodes>
                                <telerik:RadTreeNode Checkable="false" Expanded="true" runat="server" Selected="false" Text="Documenti" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma"></asp:Button>
</asp:Content>
