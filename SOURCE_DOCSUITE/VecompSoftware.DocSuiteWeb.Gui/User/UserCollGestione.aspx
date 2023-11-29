<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserCollGestione.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserCollGestione" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UDS/UserControl/uscUDS.ascx" TagPrefix="usc" TagName="UDS" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="DocumentUpload" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="ContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscTemplateCollaborationSelRest.ascx" TagName="uscTemplateCollaborationSelRest" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            $(function () {
                signBtnVisibility();
            });

            function signBtnVisibility() {
                if ("<%= HasDgrooveSigner %>" === "False") {
                    if (document.getElementById("<%= btnDgrooveSigns.ClientID %>")) {
                        document.getElementById("<%= btnDgrooveSigns.ClientID %>").style.display = 'none';
                    }
                    return;
                }

                if (document.getElementById("<%= btnDgrooveSigns.ClientID %>") === null ||
                    document.getElementById("<%= btnMultiSign.ClientID %>" === null)) {
                    return;
                }

                var currentBrowser = getBrowserType();

                if (currentBrowser.startsWith("ie")) {
                    document.getElementById("<%= btnDgrooveSigns.ClientID %>").style.display = 'none';
                    document.getElementById("<%= btnMultiSign.ClientID %>").style.display = '';
                }
                else {
                    document.getElementById("<%= btnDgrooveSigns.ClientID %>").style.display = '';
                    document.getElementById("<%= btnMultiSign.ClientID %>").style.display = 'none';
                }
            }

            function SaveToSessionStorageAndRedirect(documents) {
                sessionStorage.setItem("DocsToSign", documents);
                window.location.href = "../Comm/DgrooveSigns.aspx";
            }

            function ExecuteAjaxRequest(operationName) {
                if (operationName == 'FORWARD') {
                    $("#<%= btnInoltra.ClientID %>").attr("disabled", "disabled");
                }
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(operationName);
            }

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainPanel.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);


                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= buttonPanel.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlButtons);

                var pnltopdiv = "<%= pnlFilterPanel.ClientID%>";
                ajaxFlatLoadingPanel.show(pnltopdiv);
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainPanel.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);


                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= buttonPanel.ClientID%>";
                ajaxFlatLoadingPanel.hide(pnlButtons);

                var pnltopdiv = "<%= pnlFilterPanel.ClientID%>";
                ajaxFlatLoadingPanel.hide(pnltopdiv);
            }

            function setLink(key, value) {
                var link = document.getElementById(key);
                link.href = value;
                link.click();
            }

            function StartDownload(url) {
                setLink("link_download", url);
            }

            function ChangeStrWithValidCharacterHandler(sender, args) {
                window.ChangeStrWithValidCharacter(sender._textBoxElement);
            }

            function toCompleteStep() {
                var wizard = $find("<%= MasterDocSuite.WorkflowWizardControl.ClientID %>");
                wizard.get_wizardSteps().getWizardStep(1).set_enabled(false);
                wizard.get_wizardSteps().getWizardStep(2).set_enabled(true);
                wizard.get_wizardSteps().getWizardStep(2).set_active(true);
                $("#<%= MasterDocSuite.WizardActionColumn.ClientID %>").show();
            }

            function changeBodyClass(classType) {
                var currentBrowser = getBrowserType();
                $("body").attr("class", currentBrowser + " " + classType);
            }

            function OpenWindow(url, name, width, height) {
                var manager = $find("<%=rwmDialogManager.ClientID %>");
                var oWnd = manager.open(url, name);
                oWnd.setSize(width, height);
                oWnd.center();
                return oWnd;
            }

            function OnClientClose(sender, eventArgs) {
                sender.remove_close(OnClientClose);
                if (eventArgs.get_argument() !== null) {
                    var currentArgument = sender.argument;
                    var ajaxAction = 'ABSENTMANAGERS';
                    if (currentArgument && isNaN(currentArgument)) {
                        // Ricevuta stringa come argomento di chiusura
                        var tokens = currentArgument.split("|");
                        switch (tokens[0]) {
                            case 'DOCUMENTUNITDRAFT':
                                ajaxAction = 'DOCUMENTUNITDRAFT';
                                break;
                            default:
                                break;
                        }
                    }
                    ShowLoadingPanel()
                    var manager = $find("<%= AjaxManager.ClientID%>");
                    manager.ajaxRequest(ajaxAction + '|' + eventArgs.get_argument());
                }
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

            var userCollGestione;
            require(["User/UserCollGestione"], function (UserCollGestione) {
                $(function () {
                    userCollGestione = new UserCollGestione(tenantModelConfiguration.serviceConfiguration);
                    userCollGestione.uscTemplateCollaborationSelRestId = "<%= uscTemplateCollaborationSelRest.MainPanel.ClientID %>";
                    userCollGestione.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    userCollGestione.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    userCollGestione.hfCurrentFixedTemplateId = "<%= hfCurrentFixedTemplate.ClientID %>";
                    userCollGestione.hfCurrentTemplateId = "<%= hfCurrentTemplate.ClientID %>";
                    userCollGestione.defaultTemplateId = "<%= DefaultTemplateId %>";
                    userCollGestione.fromWorkflowUI = <%= FromWorkflowUI.ToString().ToLower() %>;
                    userCollGestione.isInsertAction = <%= IsInsertAction.ToString().ToLower() %>;
                    userCollGestione.initialize();
                });
            });


        </script>
    </telerik:RadScriptBlock>
    <telerik:RadWindowManager EnableViewState="False" ID="rwmDialogManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="rwAbsentManager" OnClientClose="OnClientClose" runat="server" Title="Direttori assenti" />
            <telerik:RadWindow ID="rwPrecompilerProtocol" OnClientClose="OnClientClose" runat="server" Title="Bozza protocollo" />
        </Windows>
    </telerik:RadWindowManager>
    <div runat="server" id="pnlFilterPanel">
        <%-- Tipologia Documento --%>
        <table class="datatable">
            <tr id="trSourceProtocol" runat="server" visible="false">
                <td class="label" style="width: 20%">Protocollo di Origine:
                </td>
                <td>
                    <telerik:RadButton ButtonType="LinkButton" ID="cmdSourceProtocol" runat="server" CausesValidation="false" />
                </td>
            </tr>
            <tr id="trDeskSource" runat="server" visible="False">
                <td class="label" style="width: 20%">Tavolo di Origine:
                </td>
                <td>
                    <asp:HyperLink runat="server" ID="deskLink"></asp:HyperLink>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 20%">Tipologia documento/attività:
                </td>
                <td>
                    <usc:uscTemplateCollaborationSelRest runat="server" ID="uscTemplateCollaborationSelRest" />
                    <asp:HiddenField ID="hfCurrentFixedTemplate" runat="server" />
                    <asp:HiddenField ID="hfCurrentTemplate" runat="server" />
                </td>
            </tr>

        </table>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div style="width: 100%;" runat="server" id="pnlMainPanel">
        <%-- Restituzione --%>
        <table id="pnlRifiuto" runat="server" visible="False" class="datatable">
            <tr>
                <th colspan="2" style="text-align: left">Restituzione</th>
            </tr>
            <tr>
                <td style="width: 20%;"></td>
                <td style="font-weight: bold;">
                    <img style="border-width: 0;" src="../Comm/Images/Loop16.gif" width="16" height="16" alt="Collaborazione restituita" />
                    L'utente destinario per Visione/Firma ha restituito la richiesta
                </td>
            </tr>
        </table>
        <%--Documenti--%>
        <table class="datatable">
            <tr>
                <th colspan="2" style="text-align: left">
                    <asp:Label ID="lblDocumentCaption" runat="server" />
                    <asp:Label ID="lblDocumento" runat="server" />
                </th>
            </tr>
            <tr>
                <td style="width: 80%">
                    <usc:DocumentUpload ButtonPreviewEnabled="True" ID="uscDocumento" runat="server" MultipleDocuments="False" />
                </td>
                <td class="stacked">
                    <asp:Button ID="cmdDocumentCheckOut" runat="server" Text="Estrai" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdDocumentUndoCheckOut" runat="server" Text="Annulla" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdDocumentCheckIn" runat="server" Text="Archivia" Width="120px" />
                </td>
            </tr>
        </table>
        <%--Documenti Omissis--%>
        <table class="datatable" id="tblDocumentoOmissis" runat="server" style="display: none;">
            <tr>
                <th colspan="2" style="text-align: left">
                    <asp:Label ID="lblDocumentoOmissisCaption" runat="server" />
                    <asp:Label ID="lblDocumentoOmissis" runat="server" />
                </th>
            </tr>
            <tr>
                <td style="width: 80%">
                    <usc:DocumentUpload ButtonFileEnabled="true" ButtonPreviewEnabled="True" ButtonRemoveEnabled="true" CheckSelectedNode="true" HeaderVisible="false" ID="uscDocumentoOmissis" IsAttachment="true" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" />
                </td>
                <td class="stacked">
                    <asp:Button ID="cmdDocumentOmissisCheckOut" runat="server" Text="Estrai" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdDocumentOmissisUndoCheckOut" runat="server" Text="Annulla" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdDocumentOmissisCheckIn" runat="server" Text="Archivia" Width="120px" />
                </td>
            </tr>
        </table>
        <%--Allegati--%>
        <table class="datatable">
            <tr>
                <th colspan="2" style="text-align: left">
                    <asp:Label ID="lblAttachmentsCaption" runat="server" /></th>
            </tr>
            <tr>
                <td style="width: 80%">
                    <usc:DocumentUpload ButtonPreviewEnabled="True" ID="uscAllegati" runat="server" />
                </td>
                <td class="stacked">
                    <asp:Button ID="cmdAttachmentsCheckOut" runat="server" Text="Estrai" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdAttachmentsUndoCheckOut" runat="server" Text="Annulla" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdAttachmentsCheckIn" runat="server" Text="Archivia" Width="120px" />
                </td>
            </tr>
        </table>
        <%--Allegati Omissis--%>
        <table class="datatable" id="tblAllegatiOmissis" runat="server" style="display: none;">
            <tr>
                <th colspan="2" style="text-align: left">
                    <asp:Label ID="lblAttachmentOmissisCaption" runat="server" /></th>
            </tr>
            <tr>
                <td style="width: 80%">
                    <usc:DocumentUpload ButtonFileEnabled="true" ButtonPreviewEnabled="True" ButtonRemoveEnabled="true" CheckSelectedNode="true" HeaderVisible="false" ID="uscAllegatiOmissis" IsAttachment="true" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" />
                </td>
                <td class="stacked">
                    <asp:Button ID="cmdAttachmentsOmissisCheckOut" runat="server" Text="Estrai" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdAttachmentsOmissisUndoCheckOut" runat="server" Text="Annulla" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdAttachmentsOmissisCheckIn" runat="server" Text="Archivia" Width="120px" />
                </td>
            </tr>
        </table>
        <%--Annessi--%>
        <table class="datatable">
            <tr>
                <th colspan="2" style="text-align: left">
                    <asp:Label ID="lblAnnexedCaption" runat="server" />
                </th>
            </tr>
            <tr>
                <td style="width: 80%">
                    <usc:DocumentUpload ButtonPreviewEnabled="True" ID="uscAnnexed" runat="server" />
                </td>
                <td class="stacked">
                    <asp:Button ID="cmdAnnexedCheckOut" runat="server" Text="Estrai" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdAnnexedUndoCheckOut" runat="server" Text="Annulla" Width="120px" />
                    <asp:Button Enabled="False" ID="cmdAnnexedCheckIn" runat="server" Text="Archivia" Width="120px" />
                </td>
            </tr>
        </table>
        <%-- Proponente --%>
        <asp:Panel ID="pnlProponente" runat="server" Visible="False">
            <table class="datatable">
                <tr>
                    <th colspan="3" style="text-align: left">Proponente
                    </th>
                </tr>
                <tr>
                    <td class="label" style="width: 20%">Proponente:
                    </td>
                    <td style="width: 60%">
                        <usc:ContattiSel ButtonSelectOChartVisible="False" EnableViewState="true" HeaderVisible="false" ID="uscProponente" ReadOnly="true" runat="server" TreeViewCaption="Proponente" />
                    </td>
                    <td style="width: 20%; white-space: nowrap">&nbsp;
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Tipo Operazione --%>
        <asp:Panel ID="pnlTipoOperazione" runat="server" Visible="false">
            <table class="datatable">
                <tr>
                    <td class="label" style="width: 20%">Tipo operazione:
                    </td>
                    <td style="width: 80%">
                        <asp:RadioButtonList AutoPostBack="True" ID="rblTipoOperazione" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Text="*" Value="A" />
                            <asp:ListItem Text="Restituzione" Value="R" />
                            <asp:ListItem Text="Inoltro per Visione/Firma" Value="I" />
                            <asp:ListItem Text="Protocollazione" Value="P" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Visione Firma --%>
        <asp:Panel ID="pnlVisioneFirma" runat="server" Visible="False">
            <table id="Table3" class="datatable">
                <tr>
                    <th colspan="3" style="text-align: left">Destinatari Visione/Firma
                    </th>
                </tr>
                <tr>
                    <td class="label" style="width: 20%">Destinatari:
                    </td>
                    <td style="width: 60%">
                        <usc:ContattiSel ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonRoleVisible="true" ButtonSelectOChartVisible="false" ButtonSelectVisible="true" EnableCheck="True" EnableViewState="true" HeaderVisible="false" ID="uscVisioneFirma" IsRequired="true" Multiple="true" runat="server" TreeViewCaption="Destinatari" UseAD="true" />
                    </td>
                    <td style="width: 20%; white-space: nowrap">&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Inoltro Visione --%>
        <asp:Panel ID="pnlInoltro" runat="server" Visible="False">
            <table class="datatable">
                <tr>
                    <th colspan="3" style="text-align: left">Inoltro per Visione/Firma</th>
                </tr>
                <tr>
                    <td class="label" style="width: 20%">Destinatario per inoltro:
                    </td>
                    <td style="width: 60%">
                        <usc:ContattiSel ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonRoleVisible="true" ButtonSelectOChartVisible="False" ButtonSelectVisible="false" EnableViewState="true" HeaderVisible="false" ID="uscInoltro" IsRequired="true" runat="server" TreeViewCaption="Destinatario" UseAD="true" />
                    </td>
                    <td style="width: 20%; white-space: nowrap">&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Restituzioni --%>
        <asp:Panel ID="pnlRestituzioni" runat="server" Visible="False">
            <table class="datatable">
                <tr>
                    <th colspan="4">Destinatari per
                    <asp:Label ID="lblDestinatari" runat="server" />
                    </th>
                </tr>
                <tr>
                    <%-- Segreterie --%>
                    <td class="label" style="width: 10%">Segreterie:
                    </td>
                    <td style="width: 40%; vertical-align: middle;">
                        <usc:Settori Caption="Segreterie" Checkable="True" HeaderVisible="false" ID="uscSettoriSegreterie" MultipleRoles="True" MultiSelect="true" ReadOnly="True" Required="False" runat="server" />
                    </td>
                    <%-- Altri --%>
                    <td class="label" style="width: 10%">
                        <asp:Literal ID="lblOtherRecipients" runat="server" Text="Altri Destinatari:" />
                    </td>
                    <td style="width: 40%; vertical-align: middle;">
                        <usc:ContattiSel ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonSelectOChartVisible="False" ButtonSelectVisible="true" EnableCheck="true" EnableViewState="true" HeaderVisible="false" ID="uscRestituzioni" IsRequired="false" Multiple="true" runat="server" TreeViewCaption="Destinatari" UseAD="true" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="datatable">
            <tr>
                <th colspan="4">Dati</th>
            </tr>
            <%-- Priorità --%>
            <tr id="tPriority" runat="server">
                <td class="label" style="width: 20%">Priorità:</td>
                <td>
                    <asp:RadioButtonList ID="rblPriority" RepeatDirection="Horizontal" runat="server">
                        <asp:ListItem Selected="True" Text="Normale" Value="N" />
                        <asp:ListItem Text="Bassa" Value="B" />
                        <asp:ListItem Text="Alta" Value="A" />
                    </asp:RadioButtonList>
                </td>
            </tr>

            <%-- Data promemoria --%>
            <tr id="tMemorandumDate" runat="server">
                <td class="label" style="width: 20%;">
                    <asp:Literal ID="lblMemorandumDate" runat="server" />
                </td>
                <td style="width: 80%;">
                    <table class="datatable" style="border: 0;">
                        <tr>
                            <td style="width: 20%; vertical-align: middle; padding: 0;">
                                <telerik:RadDatePicker ID="txtDate" runat="server" />
                            </td>
                            <td style="vertical-align: middle;">
                                <p>
                                    <asp:CompareValidator ControlToValidate="txtDate" Display="Dynamic" ErrorMessage="Data non valida." ID="cvInvoiceDate" Operator="DataTypeCheck" runat="server" Type="Date" />
                                </p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%-- Data Avviso --%>
            <tr id="tAlertData" runat="server">
                <td class="label" style="width: 20%;">Data avviso:
                </td>
                <td style="width: 80%;">
                    <table class="datatable" style="border: 0;">
                        <tr>
                            <td style="width: 20%; vertical-align: middle; padding: 0;">
                                <telerik:RadDatePicker ID="alertDate" runat="server" />
                            </td>
                            <td style="vertical-align: middle;">
                                <p>
                                    <asp:CompareValidator ID="cvAlertDateCompare" Display="Dynamic" ControlToCompare="txtDate" ControlToValidate="alertDate" ErrorMessage="Il campo Data Avviso deve essere una data inferiore a quella di Data scadenza." Operator="LessThan" Type="Date" runat="server"></asp:CompareValidator>
                                </p>
                                <p>
                                    <asp:CompareValidator ControlToValidate="alertDate" Display="Dynamic" ErrorMessage="Data non valida." ID="cvAlertDate" Operator="DataTypeCheck" runat="server" Type="Date" />
                                </p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%-- Oggetto --%>
            <tr id="tObject" runat="server">
                <td class="label" style="width: 20%">Oggetto:</td>
                <td style="width: 80%">
                    <telerik:RadTextBox ID="txtObject" Rows="3" runat="server" TextMode="MultiLine" Width="100%">
                        <ClientEvents OnBlur="ChangeStrWithValidCharacterHandler" />
                    </telerik:RadTextBox>
                    <asp:HiddenField ID="hdfLastTemplateObject" runat="server" />
                </td>
            </tr>
            <%-- Note --%>
            <tr id="tNote" runat="server">
                <td class="label" style="width: 20%">Note:</td>
                <td style="width: 80%">
                    <telerik:RadTextBox ID="txtNote" runat="server" Rows="3" TextMode="MultiLine" Width="100%">
                        <ClientEvents OnBlur="ChangeStrWithValidCharacterHandler" />
                    </telerik:RadTextBox>
                    <asp:HiddenField ID="hdfLastTemplateNote" runat="server" />
                </td>
            </tr>
            <tr id="tResolution" runat="server" visible="false">
                <td class="label" style="width: 20%">Delibere e determine:</td>
                <td style="width: 80%">
                <asp:HyperLink runat="server" ID="resolutionLink"></asp:HyperLink>
                </td>
            </tr>
            <tr id="tEditCollaborationData" runat="server">
                <td class="label" style="width: 20%"></td>
                <td style="width: 80%">
                    <asp:Button runat="server" Width="100px" Text="Modifica" ID="btnEditCollaborationData" />
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="pnlDocumentUnitDraftEditor" runat="server" Visible="false">
        <table class="datatable">
            <tr>
                <td class="label" style="width: 20%">
                    <asp:Button ID="btnDocumentUnitDraftEditor" runat="server" Text="Bozza di protocollo" Width="150px" CausesValidation="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlUDS" runat="server">
        <table class="datatable">
            <tr>
                <th>
                    <asp:Panel runat="server" ID="pnlUDSTitle" Width="100%">
                        <asp:Label ID="lblPnlUDS" runat="server"></asp:Label>
                        <asp:ImageButton CausesValidation="False" ID="btnExpandUDS" runat="server" />
                    </asp:Panel>
                </th>
            </tr>
            <tr>
                <td>
                    <usc:UDS runat="server" ID="uscUDS" ActionType="View" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div class="fakeDiv" runat="server" style="display: none;">
        <a id="link_download" href="">Il documento richiesto è pronto per il download.</a>
    </div>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <%--Pulsantiera--%>
    <asp:Panel runat="server" ID="buttonPanel">
        <asp:Button ID="btnConferma" runat="server" Text="Conferma" Width="120px" />
        <asp:Button ID="btnNewDesk" runat="server" Text="Crea nuovo tavolo" Width="130px" Visible="false" />
        <asp:Button ID="btnResumeDesk" runat="server" Text="Riapri tavolo" Width="100px" Visible="false" />
        <asp:Button ID="cmdPreviewDocuments" runat="server" Text="Documenti" Width="120px" />
        <asp:Button ID="btnRestituzione" runat="server" Text="*" Visible="False" Width="120px" />
        <asp:Button ID="btnRifiuta" runat="server" Text="Restituzione" Visible="False" Width="120px" />
        <asp:Button ID="btnProtocolla" runat="server" Text="Protocolla" Visible="False" Width="120px" />
        <asp:Button ID="btnVisioneProtocolla" runat="server" Text="Protocolla" Visible="False" Width="120px" />
        <asp:Button ID="btnInoltra" runat="server" Text="Prosegui" Visible="False" Width="120px" />
        <asp:Button ID="btnEditUDS" runat="server" Text="Modifica archivio" Visible="False" Width="120px" OnClientClick="ShowLoadingPanel();" />
        <asp:Button ID="btnAbsence" runat="server" Text="Direttori assenti" Visible="false" Width="150px" />
        <asp:Button CausesValidation="False" ID="btnVersioning" runat="server" Text="Versioning" Visible="False" Width="120px" />
        <asp:Button CausesValidation="False" ID="btnRichiamo" runat="server" Text="Richiamo" Visible="False" Width="120px" />
        <asp:Button ID="btnMail" PostBackUrl="~/MailSenders/GenericMailSender.aspx" runat="server" Text="Richiedi info" Width="120px" OnClientClick="ShowLoadingPanel();" />
        <asp:Button ID="cmdSave" runat="server" Text="Salva" Width="120px" />
        <DocSuite:PromptClickOnceButton ConfirmationMessage="Si desidera annullare la collaborazione?" ConfirmBeforeSubmit="true" ID="btnCancella" runat="server" Text="Annulla collaborazione" Visible="false" Width="150px" />
        <asp:Button ID="btnMultiSign" runat="server" Text="Firma" OnClientClick="ShowLoadingPanel();" />
        <asp:Button ID="btnDgrooveSigns" runat="server" Text="Firma" Width="120px" />
        <asp:Button ID="btnRefresh" runat="server" Text="Aggiorna" Width="120px" />
        <asp:Button ID="cmdVersioningManagement" runat="server" Text="Versionamento multiplo" Width="120px" />
        <%--antichità da eliminare col tempo--%>
        <asp:TextBox AutoPostBack="True" ID="txtDestinatarioOK" runat="server" Width="16px" />
        <asp:TextBox AutoPostBack="True" ID="txtRestituzioniOK" runat="server" Width="16px" />
        <asp:TextBox AutoPostBack="True" ID="txtInoltroOK" runat="server" Width="16px" />
        <asp:TextBox AutoPostBack="True" ID="txtLastChanged" runat="server" Width="16px" />
    </asp:Panel>
</asp:Content>
