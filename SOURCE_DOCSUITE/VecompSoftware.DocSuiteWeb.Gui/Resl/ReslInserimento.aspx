<%@ Page AutoEventWireup="false" CodeBehind="ReslInserimento.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslInserimento" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagPrefix="usc" TagName="SelCategory" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="SelContatti" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagPrefix="usc" TagName="SelOggetto" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelText.ascx" TagPrefix="usc" TagName="SelContattiTesto" %>
<%@ Register Src="~/UserControl/uscPrivacyPanel.ascx" TagPrefix="usc" TagName="PrivacyPanel" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">

            function ControllaAUSLREOC(sender, args) {
                var chkAuslReOcNonSoggetta = $get("<%= chkAUSLREOCNonSoggetta.ClientID %>");
                var chkAuslReOcSoggetta = $get("<%= chkAUSLREOCSoggetta.ClientID %>");

                if (chkAuslReOcNonSoggetta && chkAuslReOcSoggetta && chkAuslReOcNonSoggetta.checked === false && chkAuslReOcSoggetta.checked === false) {
                    args.IsValid = false;
                }
            }

            function CreateNewDraftSeries(idSeries) {
                RequestStart();
                ExecuteAjaxRequest("createNewDraftSeries|" + idSeries);
            }

            function GoToDraftSeries(idSeriesItem) {
                RequestStart();
                ExecuteAjaxRequest("goToDraftSeries|" + idSeriesItem);
            }

            function RemoveDraftLink(idSeriesItem) {
                RequestStart();
                ExecuteAjaxRequest("removeDraftLink|" + idSeriesItem);
                return false;
            }

            function ExecuteAjaxRequest(operationName) {
                var manager = <%= AjaxManager.ClientID %>;
                manager.ajaxRequest(operationName);
            }

            var currentLoadingPanel = null;
            var currentUpdatedControl = null;
            function RequestStart(sender, args) {
                if (currentLoadingPanel != null) {
                    return false;
                }
                currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= pnlResolutionContainer.ClientID%>";
                //show the loading panel over the updated control
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function ResponseEnd() {
                //hide the loading panel and clean up the global variables
                currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= pnlResolutionContainer.ClientID%>";
                if (currentLoadingPanel != null) {
                    currentLoadingPanel.hide(currentUpdatedControl);
                }
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            }

            var seriesToConnect = null;
            function <%= Me.ID %>_OpenDraftSeriesConnectWindow(idSeries) {
                seriesToConnect = idSeries;
                var manager = $find("<%= RadWindowManagerProtocollo.ClientID%>");
                var path = "../Series/SearchResult.aspx?Type=Series&Action=CopyDocuments&Draft=True&ViewDraftAssociatedResolution=False&LimitDraftToSeries=" + idSeries;
                var wnd = manager.open(path, "windowSelDraftSeries");
                wnd.setSize(800, 600);
                wnd.add_close(OnDraftSeriesConnectClose);
                wnd.center();
                return false;
            }

            function OnDraftSeriesConnectClose(sender, args) {
                sender.remove_close(OnDraftSeriesConnectClose);
                if (args.get_argument() !== null) {
                    ExecuteAjaxRequest("draftSeriesConnect|" + args.get_argument() + "|" + seriesToConnect);
                }
            }

            function GetRoleProposerNodeSelected() {
                var tree = $find("<%= rtvRoles.ClientID%>");
                var node = tree.get_selectedNode();

                if (node != null) {
                    if (node.get_value() != "Root") {
                        ExecuteAjaxRequest("selectedRoleProposer|" + node.get_value());
                    } else {
                        alert("Selezionare almeno un settore");
                    }
                }
            }

            function CloseWindowRoleProposer() {
                var oWindow = $find("<%= windowSelRoleCollaborationProposer.ClientID%>");
                oWindow.close();
            }

            function OpenSelRoleProposerWindow() {
                var wnd = $find("<%= windowSelRoleCollaborationProposer.ClientID%>");
                wnd.set_destroyOnClose(true);
                wnd.show();
                wnd.center();
            }

            function ChangeStrWithValidCharacterHandler(sender, args) {
                window.ChangeStrWithValidCharacter(sender._textBoxElement);
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerProtocollo" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowSelProtocollo" ReloadOnShow="false" runat="server" Title="Selezione Oggetto" />
            <telerik:RadWindow ID="windowSelContact" ReloadOnShow="false" runat="server" Title="Selezione Contatto" />
            <telerik:RadWindow ID="windowSelDraftSeries" Behaviors="Close" ReloadOnShow="True" runat="server" Title="Archivi" />
            <telerik:RadWindow Behaviors="Close" Height="200px" ID="windowSelRoleCollaborationProposer" runat="server" Title="Seleziona Proponente da Collaborazione" Width="500px">
                <ContentTemplate>
                    <asp:Panel runat="server">
                        <telerik:RadTreeView ID="rtvRoles" MultipleSelect="False" runat="server">
                            <Nodes>
                                <telerik:RadTreeNode Checkable="false" EnableViewState="True" Expanded="True" Font-Bold="true" runat="server" Text="Settori" Value="Root" />
                            </Nodes>
                        </telerik:RadTreeView>
                        <div style="position: absolute; bottom: 2px; left: 2px;">
                            <asp:Button runat="server" ID="btnSelectRoleProposer" OnClientClick="return GetRoleProposerNodeSelected();" Text="Conferma" ValidationGroup="editorCommentValidator" />
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>

    <asp:Panel runat="server" ID="pnlResolutionContainer" Width="100%" Style="border-collapse: collapse;">
        <%--Visualizzazione della numerazione dell'atto--%>
        <table class="datatable" id="tblResolutionNumber" runat="server" visible="False">
            <tr>
                <td class="label" style="width: 155px;">
                    <asp:Literal ID="lblNumberLabel" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblNumber" runat="server" CssClass="miniLabel" />
                </td>
            </tr>
        </table>
        <%--Sezione gestione Documentale--%>
        <table class="datatable">
            <tr>
                <th colspan="2">Gestione Documentale
                </th>
            </tr>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td style="width: 155px">&nbsp;
                            </td>
                            <td>
                                <asp:RadioButtonList ID="rblProposta" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--Sezione Upload Documento--%>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="label" style="width: 155px">
                                <asp:Label ID="lblDocumentCaption" runat="server" />
                            </td>
                            <td>
                                <usc:UploadDocument Caption="" HeaderVisible="false" ID="uscUploadDocumenti" IsDocumentRequired="false" MultipleDocuments="false" runat="server" Type="Resl" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--Sezione Upload Documento Omissis--%>
            <tr id="mainDocumentOmissisTr" runat="server" style="display: none;">
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="label" style="width: 155px">
                                <asp:Label ID="lblDocumentOmissisCaption" runat="server" />
                            </td>
                            <td>
                                <usc:UploadDocument Caption="" HeaderVisible="false" ID="uscUploadDocumentiOmissis" IsDocumentRequired="false" MultipleDocuments="true" runat="server" Type="Resl" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--Sezione Upload Allegati--%>
            <tr>
                <td colspan="2">
                    <asp:Panel ID="pnlAttach" runat="server">
                        <table width="100%">
                            <tr>
                                <td class="label" style="width: 155px">
                                    <asp:Label ID="lblAttachmentsCaption" runat="server" />
                                </td>
                                <td>
                                    <usc:UploadDocument ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadAttach" IsDocumentRequired="false" MultipleDocuments="true" runat="server" Type="Resl" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <%--Sezione Upload Allegati Omissis--%>
            <tr id="attachmentOmissisTr" runat="server" style="display: none;">
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="label" style="width: 155px">
                                <asp:Label ID="lblAttachmentsOmissisCaption" runat="server" />
                            </td>
                            <td>
                                <usc:UploadDocument ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadAttachOmissis" IsDocumentRequired="false" MultipleDocuments="true" runat="server" Type="Resl" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--Sezione Upload Allegati Riservati--%>
            <tr>
                <td colspan="2">
                    <asp:Panel ID="pnlPrivacyAttachment" runat="server" Visible="true">
                        <table width="100%">
                            <tr>
                                <td class="label" style="width: 155px">
                                    <asp:Label ID="lblPrivacyAttachmentCaption" runat="server" />
                                </td>
                                <td>
                                    <usc:UploadDocument ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadPrivacyAttachment" IsDocumentRequired="false" MultipleDocuments="true" runat="server" Type="Resl" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <%--Sezione Upload Annessi--%>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="label" style="width: 155px">
                                <asp:Label ID="lblAnnexedCaption" runat="server" />
                            </td>
                            <td>
                                <usc:UploadDocument ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscUploadAnnexed" IsDocumentRequired="false" MultipleDocuments="true" runat="server" Type="Resl" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 150px">&nbsp;
                </td>
                <td>
                    <asp:Panel runat="server" ID="pnlImmediatelyExecutive" Width="100%" Visible="false">
                        <asp:CheckBox runat="server" ID="chkImmediatelyExecutive" Text="Immediatamente esecutiva" TextAlign="Right" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <%--Sezione AUSL-RE Soggetto a Controllo--%>
        <asp:Panel ID="pnlAUSLREOC" runat="server" Visible="false">
            <table class="datatable">
                <tr>
                    <th colspan="2">Tipo Delibera
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;</td>
                    <td>
                        <asp:CheckBox ID="chkAUSLREOCNonSoggetta" runat="server" Text="<u>DELIBERA NON SOGGETTA A CONTROLLO</u>. Esecutiva dalla data di pubblicazione ai sensi della L.R. 50/94 art. 37." AutoPostBack="true" />
                        <br />
                        <asp:CheckBox ID="chkAUSLREOCSoggetta" runat="server" Text="<u>DELIBERA SOGGETTA A CONTROLLO</u>. Esecutiva a seguito di approvazione da parte della Regione Emilia Romagna." AutoPostBack="true" />
                        <br />
                        <asp:CustomValidator ID="cvControllaAUSLREOC" runat="server" Display="Dynamic" ClientValidationFunction="ControllaAUSLREOC" ErrorMessage="Tipo Delibera obbligatorio." />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%--Sezione Contenitore--%>
        <table class="datatable">
            <tr>
                <th colspan="2">Contenitore
                </th>
            </tr>
            <tr class="Chiaro">
                <td style="width: 150px">&nbsp;
                </td>
                <td>
                    <asp:DropDownList ID="idContainer" runat="server" AutoPostBack="true" />
                    <asp:RequiredFieldValidator ControlToValidate="idContainer" Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio" ID="ContenitoreValidator" runat="server" />
                </td>
            </tr>
        </table>
        <%--Sezione Tipologia Atto--%>
        <asp:Panel runat="server" ID="pnlResolutionKind" Visible="False">
            <table class="datatable">
                <tr>
                    <th colspan="2">Tipologia Atto
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlResolutionKind" runat="server" AutoPostBack="true" />
                        <asp:RequiredFieldValidator ControlToValidate="ddlResolutionKind" Display="Dynamic" ErrorMessage="Campo Tipologia Atto Obbligatorio" ID="ddlResolutionKindValidator" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%--Sezione Adozione--%>
        <telerik:RadAjaxPanel runat="server" Visible="true">
            <asp:Panel ID="pnlAdozione" runat="server" Visible="false" Width="100%">
                <table class="datatable">
                    <tr>
                        <th colspan="2">Adozione
                        </th>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:CheckBox AutoPostBack="True" Font-Bold="True" ID="chbAdoption" runat="server" Text="Adozione Automatica" TextAlign="Left" />
                        </td>
                        <td style="width: 50%">
                            <asp:Panel ID="pnlAdozioneData" runat="server" Visible="true">
                                <span class="miniLabel">Data:</span>
                                <telerik:RadDatePicker ID="rdpDataAdozione" runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="rdpDataAdozione" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvData" runat="server" />
                            </asp:Panel>
                            <asp:Panel ID="pnlAdozioneNumero" runat="server" Visible="true">
                                <span class="miniLabel">Numero:</span>
                                <asp:TextBox ID="txtServiceNumber" MaxLength="10" runat="server" Width="86px" />
                                <asp:RequiredFieldValidator ControlToValidate="txtServiceNumber" Display="Dynamic" ErrorMessage="Numero obbligatorio" ID="rfvServiceNumber" runat="server" />
                                <asp:CompareValidator ControlToValidate="txtServiceNumber" EnableClientScript="true" ErrorMessage="Formato numero errato" ID="revNumero" Operator="DataTypeCheck" runat="server" Type="Integer" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </telerik:RadAjaxPanel>
        <%--Sezione Destinatari--%>
        <table class="datatable" id="TblDestinatari" runat="server">
            <tr>
                <th colspan="2">Destinatari
                </th>
            </tr>
            <tr class="Chiaro">
                <td style="width: 150px">&nbsp;
                </td>
                <td>
                    <asp:Panel ID="pnlDestInterop" runat="server">
                        <table cellspacing="0" cellpadding="2" width="100%" border="0">
                            <tr>
                                <td style="width: 100%">
                                    <usc:SelContatti ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="true" HeaderVisible="false" ID="uscDestinatari" IsRequired="false" Multiple="true" MultiSelect="true" runat="server" TreeViewCaption="Destinatari" />
                                </td>
                                <td style="white-space: nowrap"></td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <usc:SelContattiTesto ID="uscDestinatariAlt" MaxLunghezzaTesto="1000" runat="server" Type="Resl" />
                </td>
            </tr>
        </table>
        <%--Sezione Autorizzazioni--%>
        <table class="datatable" id="tblCollaborationAuthorize" runat="server" visible="false">
            <tr>
                <th colspan="2">Autorizzazioni da Collaborazione
                </th>
            </tr>
            <tr class="Chiaro">
                <td>
                    <usc:Settori HeaderVisible="false" MultipleRoles="true" ID="collaborationAuthorizedRoles" ReadOnly="true" runat="server" />
                </td>
            </tr>
        </table>
        <%--Sezione Proponente--%>
        <asp:Panel ID="pnlProposer" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="2">Proponente
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <telerik:RadAjaxPanel runat="server" ID="pnlProponenteInterop">
                            <usc:SelContatti ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonSelectDomainVisible="true" ButtonSelectOChartVisible="false" ButtonSelectVisible="true" Caption="Proponente" HeaderVisible="false" ID="SelProponente" IsRequired="True" Multiple="true" MultiSelect="false" RequiredErrorMessage="Campo Proponente Obbligatorio" runat="server" TreeViewCaption="Proponente" />
                            <usc:Settori HeaderVisible="False" MultipleRoles="False" Environment="Resolution" RoleRestictions="OnlyMine" ID="uscRoleProposer" ReadOnly="False" runat="server" />
                        </telerik:RadAjaxPanel>
                        <usc:SelContattiTesto ID="SelProponenteAlt" MaxLunghezzaTesto="1000" runat="server" Type="Resl" />
                        <asp:CustomValidator runat="server" ID="proposerGeneralValidator" OnServerValidate="GeneralProposerValidation" ValidationGroup="ProposerGeneralValidationGroup" ErrorMessage="Campo Proponente Obbligatorio" Display="Dynamic"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <%--Sezione Assegnatario--%>
        <asp:Panel ID="pnlAssegnatario" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="2">Assegnatario
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <asp:Panel ID="pnlAsseInterop" runat="server" Visible="false">
                            <table cellspacing="0" cellpadding="2" width="100%" border="0">
                                <tr>
                                    <td style="width: 100%">
                                        <usc:SelContatti ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" Caption="Assegnatario" EnableCC="true" HeaderVisible="false" ID="SelAssegnatario" IsRequired="false" Multiple="true" MultiSelect="false" runat="server" TreeViewCaption="Assegnatario" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <usc:SelContattiTesto ID="SelAssegnatarioAlt" MaxLunghezzaTesto="1000" runat="server" Type="Resl" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <%--Sezione Responsabile--%>
        <asp:Panel ID="pnlResponsabile" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="2">Responsabile
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <asp:Panel ID="pnlRespInterop" runat="server">
                            <table cellspacing="0" cellpadding="2" width="100%" border="0">
                                <tr>
                                    <td style="width: 100%">
                                        <table cellspacing="0" cellpadding="2" width="100%" border="0">
                                            <tr>
                                                <td style="width: 100%">
                                                    <usc:SelContatti ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" Caption="Responsabile" HeaderVisible="false" ID="SelResponsabile" IsRequired="false" Multiple="true" MultiSelect="false" runat="server" TreeViewCaption="Responsabile" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <usc:SelContattiTesto runat="server" ID="SelResponsabileAlt" Type="Resl" MaxLunghezzaTesto="1000" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%--Sezione Oggetto Privacy--%>
        <asp:Panel runat="server" ID="pnlObjectPrivacy" Visible="false">
            <table class="datatable">
                <tr>
                    <th colspan="2">
                        <asp:Label runat="server" ID="lblObjectPrivacy"></asp:Label></th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <usc:SelOggetto EditMode="true" ID="SelOggettoPrivacy" MaxLength="1500" MultiLine="true" Required="true" runat="server" Type="Resl" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <%--Sezione Oggetto--%>
        <table class="datatable">
            <tr>
                <th colspan="2">Oggetto</th>
            </tr>
            <tr class="Chiaro">
                <td style="width: 150px">&nbsp;
                </td>
                <td>
                    <usc:SelOggetto EditMode="true" ID="SelOggetto" MaxLength="1500" MultiLine="true" Required="true" runat="server" Type="Resl" />
                </td>
            </tr>
        </table>

        <%--Sezione Privacy--%>
        <usc:PrivacyPanel ID="uscPrivacyPanel" runat="server" />

        <%--Sezione Note--%>
        <table class="datatable">
            <tr>
                <th colspan="2">Note
                </th>
            </tr>
            <tr class="Chiaro">
                <td style="width: 150px">&nbsp;
                </td>
                <td>
                    <telerik:RadTextBox runat="server" ID="txtNote" Width="100%" MaxLength="1500">
                        <ClientEvents OnBlur="ChangeStrWithValidCharacterHandler"></ClientEvents>
                    </telerik:RadTextBox>
                </td>
            </tr>
        </table>
        <%--Sezione Autorizzazioni--%>
        <asp:Panel ID="pnlAutorizzazioni" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="2">Autorizzazioni
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <usc:Settori Caption="" MultipleRoles="true" HeaderVisible="false" ID="uscSettori" MultiSelect="true" Required="false" RoleRestictions="None" runat="server" Type="Resl" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <%--Sezione OC--%>
        <asp:Panel ID="pnlOC" runat="server" Visible="false">
            <table class="datatable">
                <tr>
                    <th colspan="2">Organo di controllo
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <asp:Panel ID="pnlOCSupervisoryBoard" runat="server">
                            <asp:CheckBox AutoPostBack="False" ID="chkOCSupervisoryBoard" runat="server" Text="Provvedimento soggetto al Controllo del Collegio Sindacale ai sensi dell’art. 14, Legge 24.1.95 n. 10."></asp:CheckBox>
                            <br>
                        </asp:Panel>
                        <asp:Panel ID="pnlOCConfSindaci" runat="server" Visible="false">
                            <asp:CheckBox AutoPostBack="False" ID="chkOCConfSindaci" runat="server" Text="Provvedimento soggetto al Controllo della Conferenza dei Sindaci." />
                            <br>
                        </asp:Panel>
                        <asp:Panel ID="pnlOCRegion" runat="server">
                            <asp:CheckBox AutoPostBack="False" ID="chkOCRegion" runat="server" Text="Provvedimento soggetto al Controllo della Regione ai sensi della LR 31/92." />
                            <br>
                        </asp:Panel>
                        <asp:Panel ID="pnlOCManagement" runat="server">
                            <asp:CheckBox AutoPostBack="False" ID="chkOCManagement" runat="server" Text="Provvedimento soggetto al Controllo di Gestione ai sensi della Legge 30.07.04 n. 191."></asp:CheckBox>
                            <br>
                        </asp:Panel>
                        <asp:Panel ID="pnlOCCorteConti" runat="server">
                            <asp:CheckBox AutoPostBack="False" ID="chkOCCorteConti" runat="server" Text="Provvedimento soggetto al Controllo della Corte dei Conti." />
                            <br>
                        </asp:Panel>
                        <asp:Panel ID="pnlOCOther" runat="server">
                            <asp:CheckBox AutoPostBack="False" ID="chkOCOther" runat="server" Text="Altro" />
                            &nbsp;
                        <asp:TextBox ID="txtOCOtherDescription" MaxLength="80" runat="server" Width="329px" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Sezione Category --%>
        <asp:Panel ID="pnlCategory" runat="server">
            <table class="datatable">
                <tr>
                    <th>Classificazione
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td class="DXChiaro" style="width: 95%">
                        <usc:SelCategory Caption="" HeaderVisible="false" ID="uscSelCategory" Multiple="false" Required="true" runat="server" Type="Resl" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Sezione Amministrazione Trasparente --%>
        <asp:Panel runat="server" ID="pnlAmmTrasp" Visible="False">
            <table class="datatable">
                <tr>
                    <th>Amministrazione Trasparente
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="vertical-align: middle;">
                        <telerik:RadGrid runat="server" ID="dgvResolutionKindDocumentSeries" Skin="Office2010Blue" Width="100%" AllowMultiRowSelection="true">
                            <MasterTableView AutoGenerateColumns="False" ShowHeader="False">
                                <NoRecordsTemplate>
                                    <div>Nessuna serie documentale associata</div>
                                </NoRecordsTemplate>
                                <Columns>
                                    <telerik:GridTemplateColumn AllowFiltering="false">
                                        <HeaderStyle Width="25%"></HeaderStyle>
                                        <ItemTemplate>
                                            <asp:Label runat="server" Font-Bold="True" ID="lblDocumentSeriesName" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn AllowFiltering="false">
                                        <HeaderStyle Width="75%"></HeaderStyle>
                                        <ItemTemplate>
                                            <asp:Button runat="server" ID="btnDocumentSeriesDraft" Text="Bozza" CausesValidation="False" Width="100px"></asp:Button>
                                            <asp:Button runat="server" ID="btnDocumentSeriesConnect" Text="Collega" CausesValidation="False" Width="100px"></asp:Button>
                                            <asp:HyperLink runat="server" ID="documentSeriesLink" NavigateUrl="javascript:void(0)" CausesValidation="False"></asp:HyperLink>
                                            <asp:ImageButton runat="server" ID="btnRemoveLink" Style="margin-left: 10px; cursor: pointer;" CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" ToolTip="Rimuovi Bozza selezionata" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
    <asp:ValidationSummary DisplayMode="List" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" Width="100%" ID="pnlButtons">
        <asp:Button ID="btnInserimento" runat="server" Text="Conferma Inserimento" />
    </asp:Panel>
</asp:Content>
