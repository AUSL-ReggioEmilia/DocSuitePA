<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionChange.ascx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionChange" %>

<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="UscCategory" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="UscDocumentUpload" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="UscContact" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscProtocolSelTree.ascx" TagName="UscProtocolSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>
 <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
       
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

        function CorrectDraftLink(idSeriesItem) {
            ExecuteAjaxRequest("correctDraftLink|" + idSeriesItem);
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
            currentLoadingPanel = $find("<%= DefaultLoadingPanel.ClientID%>");
            currentUpdatedControl = "<%= Container.ClientID%>";
            //show the loading panel over the updated control
            currentLoadingPanel.show(currentUpdatedControl);
        }

        function ResponseEnd() {
            //hide the loading panel and clean up the global variables
            if (currentLoadingPanel != null) {
                currentLoadingPanel.hide(currentUpdatedControl);
            }
            currentUpdatedControl = null;
            currentLoadingPanel = null;
        }

        var seriesToConnect = null;
        function <%= Me.ID %>_OpenDraftSeriesConnectWindow(idSeries, idResolutionKindDocumentSeries) {
            seriesToConnect = idSeries + '|' + idResolutionKindDocumentSeries;
            var manager = $find("<%= RadWindowManagerProtocollo.ClientID%>");
            var path = "../Series/SearchResult.aspx?Type=Series&Action=CopyDocuments&Draft=True&ViewDraftAssociatedResolution=False&LimitDraftToSeries="+idSeries;
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

        function ChangeStrWithValidCharacterHandler(sender, args) {
            window.ChangeStrWithValidCharacter(sender._textBoxElement);
        }

    </script>
</telerik:RadCodeBlock>

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

<telerik:RadAjaxLoadingPanel ID="DefaultLoadingPanel" runat="server"></telerik:RadAjaxLoadingPanel>

<asp:Panel ID="Container" runat="server">    
<div id="divResolution">
    <%-- Tipologia --%>
    <table id="tblType" class="datatable" runat="server" width="100%">
        <tr>
            <th colspan="2">Tipologia</th>
        </tr>
        <tr>
            <td style="width: 20%"></td>
            <td style="width: 80%">
                <asp:RadioButtonList AutoPostBack="True" ID="rblType" RepeatDirection="Horizontal" runat="server" />
            </td>
        </tr>
    </table>
    <%-- Classificazione --%>
    <table id="tblCategory" runat="server" class="datatable" width="100%">
        <tr>
            <th colspan="2">Classificazione</th>
        </tr>
        <tr>
            <td colspan="2">
                <usc:UscCategory HeaderVisible="false" ID="uscCategory" ReadOnly="true" Required="false" runat="server" Type="Resl" />
            </td>
        </tr>
        <tr id="trSubCategory">
            <td class="label" style="width: 20%;">SottoClassificazione:</td>
            <td style="width: 80%;">
                <usc:UscCategory HeaderVisible="false" ID="uscSubCategory" Required="false" runat="server" SubCategoryMode="true" Type="Resl" />
            </td>
        </tr>
    </table>
    <%-- Oggetto Privacy --%>
    <table id="tblObjectPrivacy" runat="server" class="datatable" width="100%">
        <tr>
            <th colspan="2"><asp:Label runat="server" ID="lblObjectPrivacy"></asp:Label></th>
        </tr>
        <tr>
            <td class="label" style="width: 20%;"><asp:Label runat="server" ID="lblObjectPrivacyDetail"></asp:Label></td>
            <td>
                <telerik:RadTextBox ID="txtObjectPrivacy" MaxLength="1500" Rows="4" runat="server" TextMode="MultiLine" Width="100%">
                    <ClientEvents OnBlur="ChangeStrWithValidCharacterHandler"></ClientEvents>
                </telerik:RadTextBox>
            </td>
        </tr>
    </table>
    <%-- Oggetto --%>
    <table id="tblObject" runat="server" class="datatable" width="100%">
        <tr>
            <th colspan="2">Oggetto</th>
        </tr>
        <tr id="trObject" runat="server">
            <td class="label" style="width: 20%;">Oggetto:</td>
            <td>
                <telerik:RadTextBox ID="txtObject" MaxLength="1500" Rows="4" runat="server" TextMode="MultiLine" Width="100%">
                    <ClientEvents OnBlur="ChangeStrWithValidCharacterHandler"></ClientEvents>
                </telerik:RadTextBox>
            </td>
        </tr>
        <tr id="trNote" runat="server">
            <td class="label" style="width: 20%;">Note:</td>
            <td>
                <telerik:RadTextBox ID="txtNote" MaxLength="1500" Rows="4" runat="server" TextMode="MultiLine" Width="100%">
                    <ClientEvents OnBlur="ChangeStrWithValidCharacterHandler"></ClientEvents>
                </telerik:RadTextBox>
            </td>
        </tr>
        <tr id="trIE" runat="server">
            <td class="label" style="width: 20%;"></td>
            <td>
                <asp:CheckBox ID="chkExecutive" runat="server" Text="Immediatamente esecutiva" />
            </td>
        </tr>
    </table>
    <%-- Dati Economici --%>
    <table id="tblEconomicData" runat="server" class="datatable" width="100%">
        <tr>
            <th><asp:Label ID="lblEconomicDataTitle" runat="server">Dati economici</asp:Label></th>
        </tr>
        <tr>
            <td>
                <table cellspacing="0" cellpadding="1" width="100%" border="0">
                    <tr Visible="true" id="economicDataPosizione" runat="server">
                        <td class="label" width="20%">Posizione:</td>
                        <td align="right" width="5%"></td>
                        <td class="label" width="20%">
                            <telerik:RadTextBox ID="txtPosition" runat="server" Width="100%" />
                        </td>
                        <td class="label" width="10%"></td>
                        <td class="label" width="45%"></td>
                    </tr>
                    <tr Visible="true" id="economicDataContratto" runat="server">
                        <td class="label" width="20%">Val. contratto:</td>
                        <td class="label" width="5%">da:</td>
                        <td align="left" width="20%">
                            <telerik:RadDatePicker ID="rdpValidityDateFrom" runat="server" />
                        </td>
                        <td class="label" width="10%">a:</td>
                        <td align="left" width="45%">
                            <telerik:RadDatePicker ID="rdpValidityDateTo" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" width="20%" id="lblBidtype" runat="server">Tipologia gara:</td>
                        <td width="5%"></td>
                        <td align="left" width="20%" Visible="false">
                            <telerik:RadDropDownList runat="server" ID="rlbBidTypes" Width="300px" selected="true" Visible="false"/>
                            <asp:DropDownList ID="ddlBidType" runat="server" />
                        </td>
                        <td width="10%"></td>
                        <td align="left" width="45%"></td>
                    </tr>
                    <tr Visible="true" id="economicDataFornitore" runat="server">
                        <td class="label" width="20%">Fornitore:</td>
                        <td class="label" width="5%">Cod:</td>
                        <td align="left" width="20%">
                            <asp:TextBox ID="txtSupplierCode" runat="server" MaxLength="10"></asp:TextBox>
                        </td>
                        <td class="label" width="10%">Descr:</td>
                        <td align="left" width="45%">
                            <telerik:RadTextBox ID="txtSupplierDescription" runat="server" Width="100%" MaxLength="80"></telerik:RadTextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <%-- Invio Servizi --%>
    <table id="tblService" runat="server" class="datatable" width="100%">
        <tr>
            <th colspan="2">Invio Servizi</th>
        </tr>
        <tr>
            <td class="label" width="20%">&nbsp;</td>
            <td>
                <usc:UscProtocolSel ID="uscProposerProtocolLink" IsRequired="false" Multiple="false" runat="server" Type="Prot" />
            </td>
        </tr>
    </table>
    <%-- Protocollo lettera di avvenuta pubblicazione --%>
    <table id="tblPublicationLetterProtocolLink" runat="server" class="datatable" width="100%" visible="false">
        <tr>
            <th colspan="2">Protocollo lettera di avvenuta pubblicazione</th>
        </tr>
        <tr>
            <td class="label" width="20%">&nbsp;</td>
            <td>
                <usc:UscProtocolSel ID="uscPublicationLetterProtocolLink" IsRequired="false" Multiple="false" runat="server" Type="Prot" />
            </td>
        </tr>
    </table>
    <%-- OC List --%>
    <table id="tblOCList" runat="server" class="datatable" width="100%">
        <tr>
            <th colspan="2">Soggetta ai controlli</th>
        </tr>
        <tr>
            <td class="label" width="20%">&nbsp;</td>
            <td>
                <asp:Panel ID="pnlOCSupervisoryBoard" runat="server">
                    <asp:CheckBox AutoPostBack="True" ID="chkOCSupervisoryBoard" runat="server" Text="Provvedimento soggetto al Controllo del Collegio Sindacale ai sensi dell’art. 14, Legge 24.1.95 n. 10." />
                    <br />
                </asp:Panel>
                <asp:Panel ID="pnlOCConfSindaci" runat="server" Visible="false">
                    <asp:CheckBox AutoPostBack="True" ID="chkOCConfSindaci" runat="server" Text="Provvedimento soggetto al Controllo della Conferenza dei Sindaci." />
                    <br />
                </asp:Panel>
                <asp:Panel ID="pnlOCRegion" runat="server">
                    <asp:CheckBox AutoPostBack="True" ID="chkOCRegion" runat="server" Text="Provvedimento soggetto al Controllo della Regione ai sensi della LR 31/92." />
                    <br />
                </asp:Panel>
                <asp:Panel ID="pnlOCManagement" runat="server">
                    <asp:CheckBox AutoPostBack="True" ID="chkOCManagement" runat="server" Text="Provvedimento soggetto al Controllo di Gestione ai sensi della Legge 30.07.04 n. 191." />
                    <br />
                </asp:Panel>
                <asp:Panel ID="pnlOCCorteConti" runat="server">
                    <asp:CheckBox AutoPostBack="True" ID="chkOCCorteConti" runat="server" Text="Provvedimento soggetto al Controllo della Corte dei Conti." />
                    <br />
                </asp:Panel>
                <asp:Panel ID="pnlOCOther" runat="server">
                    <asp:CheckBox ID="chkOCOther" runat="server" Text="Altro" AutoPostBack="True" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <asp:Panel runat="server" ID="pnlOrganoControllo">
        <%-- Organo di Controllo: Collegio Sindacale --%>
        <table id="tblOCSupervisoryBoard" runat="server" class="datatable" width="100%">
            <tr>
                <th colspan="2">Organo di Controllo - Collegio Sindacale
                </th>
            </tr>
            <tr>
                <td width="100%" colspan="2">
                    <table cellspacing="0" cellpadding="1" width="100%" align="center" border="0">
                        <tr width="100%">
                            <td class="label" style="vertical-align: middle; width: 20%">Spedizione:</td>
                            <td style="vertical-align: middle; text-align: left; width: 60px;">
                                <telerik:RadDatePicker ID="rdpSupervisoryBoardWarningDate" runat="server" />
                            </td>
                            <td class="label" style="vertical-align: middle; width: 80px;">Prot.
                            </td>
                            <td style="vertical-align: middle; text-align: left;">
                                <usc:UscProtocolSel ID="uscSupervisoryBoardProtocolLink" IsRequired="false" Multiple="false" runat="server" Type="Prot" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trOCSupervisoryBoardFile" runat="server">
                <td style="width: 20%;" class="label">Documenti:
                </td>
                <td>
                    <usc:UscDocumentUpload ButtonFDQEnabled="false" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonSharedFolederEnabled="false" HeaderVisible="false" ID="uscOCSupervisoryBoardDocument" IsDocumentRequired="false" MultipleDocuments="false" runat="server" Type="Resl" />
                </td>
            </tr>
            <tr id="trOCSupervisoryBoardOpinion" runat="server">
                <td class="label" style="width: 20%; padding: 0;">Rilievo:
                </td>
                <td style="padding: 0;">
                    <telerik:RadTextBox ID="txtOCSupervisoryBoardOpinion" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox>
                </td>
            </tr>
        </table>
        <%-- Organo di Controllo: Corte dei Conti --%>
        <table id="tblCorteDeiConti" runat="server" class="datatable" width="100%">
            <tr>
                <th>Organo di Controllo - Corte dei Conti</th>
            </tr>
            <tr>
                <td width="100%">
                    <table cellspacing="0" cellpadding="1" width="100%" align="center" border="0">
                        <tr style="width: 100%;">
                            <td class="label" width="20%">Spedizione:</td>
                            <td align="left" width="60">
                                <telerik:RadDatePicker ID="rdpCorteDeiContiWarningDate" runat="server" />
                            </td>
                            <td class="label" width="80">Prot.</td>
                            <td align="left">
                                <usc:UscProtocolSel ID="uscCorteDeiContiProtocolLink" runat="server" Multiple="false" IsRequired="false" Type="Prot" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <%-- Organo di Controllo: Conferenza dei Sindaci --%>
        <table id="tblOCConfSindaci" runat="server" class="datatable" width="100%" style="display: none">
            <tr>
                <th>Organo di Controllo - Conferenza dei Sindaci</th>
            </tr>
            <tr>
                <td width="100%">
                    <table cellspacing="0" cellpadding="1" width="100%" align="center" border="0">
                        <tr style="width: 100%;">
                            <td class="label" width="20%" style="vertical-align: middle;">Spedizione:</td>
                            <td align="left" width="60" style="vertical-align: middle;">
                                <telerik:RadDatePicker ID="rdpConfSindaciWarningDate" runat="server" />
                            </td>
                            <td class="label" width="80" style="vertical-align: middle;">Prot.</td>
                            <td align="left" style="vertical-align: middle;">
                                <usc:UscProtocolSel ID="uscConfSindaciProtocolLink" runat="server" Multiple="false" IsRequired="false" Type="Prot" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <%-- Organo di Controllo: Regione --%>
        <table id="tblOCRegion" runat="server" class="datatable" style="width: 100%; display: none">
            <tr>
                <th colspan="2">Organo di Controllo - Regione
                </th>
            </tr>
            <tr>
                <td width="100%" colspan="2">
                    <table cellspacing="0" cellpadding="1" width="100%" align="center" border="0">
                        <tr id="trOCRegionSpedizione" runat="server">
                            <td class="label" width="20%" style="vertical-align: middle;">Spedizione:
                            </td>
                            <td align="left" width="60" style="vertical-align: middle;">
                                <telerik:RadDatePicker ID="rdpRegionWarningDate" runat="server" />
                            </td>
                            <td class="label" width="80" style="text-align: right; vertical-align: middle;">Prot.
                            </td>
                            <td align="left" style="vertical-align: middle;">
                                <usc:UscProtocolSel ID="uscRegionProtocolLink" runat="server" Multiple="false" IsRequired="false" Type="Prot" />
                            </td>
                        </tr>
                        <tr id="trOCRegionRicezioneEScadenza" runat="server">
                            <td class="label">Ricezione:
                            </td>
                            <td align="left" width="60">
                                <telerik:RadDatePicker ID="rdpRegionConfirmDate" runat="server" />
                            </td>
                            <td class="label" width="80" style="text-align: right">Scadenza:
                            </td>
                            <td align="left">
                                <div style="width: 150px">
                                    <telerik:RadDatePicker ID="rdpRegionWaitDate" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr id="trOCRegionRispostaEDGR" runat="server">
                            <td id="tdRegionResponseDateLabel" runat="server" class="label" style="vertical-align: middle;">Data:
                            </td>
                            <td align="left" width="60" style="vertical-align: middle;">
                                <telerik:RadDatePicker ID="rdpRegionResponseDate" runat="server" />
                            </td>
                            <td id="tdDGRLabel" runat="server" class="label" width="80" style="text-align: right">D.G.R.:
                            </td>
                            <td id="tdDGRtext" runat="server" align="left">
                                <asp:TextBox ID="txtDGR" runat="server" Width="150px" MaxLength="255"></asp:TextBox>
                            </td>
                            <td id="tdResponseProtocolLabel" runat="server" class="label" width="80" style="text-align: right; vertical-align: middle;" visible="false">Prot.
                            </td>
                            <td id="tdResponseProtocolContent" runat="server" align="left" visible="false" style="vertical-align: middle;">
                                <usc:UscProtocolSel ID="uscRegionResponseProtocol" runat="server" Multiple="false" IsRequired="false" Type="Prot" />
                            </td>
                        </tr>
                        <%-- Invio chiarimenti alla regione --%>
                        <tr id="trOCREgionInvioChiarimenti" runat="server" visible="false">
                            <td class="label" style="vertical-align: middle;">Invio chiarimenti alla Regione:
                            </td>
                            <td width="60" align="left" style="vertical-align: middle;">
                                <telerik:RadDatePicker ID="rdpRegionInvioChiarimenti" runat="server" />
                            </td>
                            <td class="label" width="80" style="text-align: right; vertical-align: middle;">Prot.
                            </td>
                            <td align="left" style="vertical-align: middle;">
                                <usc:UscProtocolSel ID="uscRegionInvioChiarimentiProtocolLink" runat="server" Multiple="false" IsRequired="false" Type="Prot" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trRegionComment" runat="server">
                <td style="width: 20%;" class="label">Commento:
                </td>
                <td>
                    <asp:DropDownList ID="ddlRegionComment" runat="server" />
                </td>
            </tr>
            <tr id="trRegionDocument" runat="server">
                <td style="width: 20%;" class="label">Documenti:
                </td>
                <td>
                    <usc:UscDocumentUpload ButtonFDQEnabled="false" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonSharedFolederEnabled="false" HeaderVisible="false" ID="uscOCRegionDocument" IsDocumentRequired="false" MultipleDocuments="false" runat="server" Type="Resl" />
                </td>
            </tr>
            <tr id="trRegionOpinion" runat="server">
                <td id="tdRegionOpinionText" runat="server" style="width: 20%;" class="label">Note Regione
                </td>
                <td>
                    <telerik:RadTextBox ID="txtRegionOpinion" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox>
                </td>
            </tr>
            <tr id="trOCRegionNoteApprovazione" runat="server">
                <td style="width: 20%;" class="label">Note Approvazione
                </td>
                <td>
                    <telerik:RadTextBox ID="txtApprovalNote" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox>
                </td>
            </tr>
            <tr id="trOCRegionNoteDecadimento" runat="server">
                <td style="width: 20%;" class="label">Note Decadimento
                </td>
                <td>
                    <telerik:RadTextBox ID="txtDeclineNote" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox>
                </td>
            </tr>
        </table>
        <%-- Organo di Controllo: Gestione --%>
        <table id="tblOCManagement" runat="server" class="datatable" width="100%" visible="false">
            <tr>
                <th>Organo di Controllo - Gestione
                </th>
            </tr>
            <tr>
                <td width="100%">
                    <table cellspacing="0" cellpadding="1" width="100%" align="center" border="0">
                        <tr width="100%">
                            <td class="label" width="20%">Spedizione:</td>
                            <td align="left" width="60">
                                <telerik:RadDatePicker ID="rdpManagementWarningDate" runat="server" />
                            </td>
                            <td class="label" width="80">Prot.</td>
                            <td align="left">
                                <usc:UscProtocolSel runat="server" ID="uscManagementProtocolLink" IsRequired="false" Type="Prot" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <%-- Organo di Controllo: Altro --%>
        <table id="tblOCOther" runat="server" class="datatable" width="100%" visible="false">
            <tr>
                <th>Organo di Controllo - Altro
                </th>
            </tr>
            <tr>
                <td width="100%">
                    <table cellspacing="0" cellpadding="1" width="100%" align="center" border="0">
                        <tr width="100%">
                            <td class="label" width="20%">Descrizione:</td>
                            <td align="left" width="80%">
                                <telerik:RadTextBox ID="txtOCOtherDescription" runat="server" Width="100%" MaxLength="80"></telerik:RadTextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <%-- Organo di Controllo --%>
        <table id="tblOC" runat="server" class="datatable" width="100%">
            <tr>
                <th colspan="2">Organo di Controllo
                </th>
            </tr>
            <tr>
                <td width="100%" colspan="2">
                    <table cellspacing="0" cellpadding="1" width="100%" align="center" border="0">
                        <tr width="100%">
                            <td class="label" width="20%">Spedizione:</td>
                            <td align="left" width="15%">
                                <telerik:RadDatePicker ID="rdpWarningDate" runat="server" />
                            </td>
                            <td class="label" style="width: 35px">Prot.</td>
                            <td align="left" width="10%">
                                <asp:TextBox ID="txtWarningProt" runat="server" Width="86px" MaxLength="255"></asp:TextBox>
                            </td>
                            <td class="label" width="20%">Ricezione:</td>
                            <td align="left" width="15%">
                                <telerik:RadDatePicker ID="rdpConfirmDate" AutoPostBackControl="Both" runat="server" />
                            </td>
                            <td class="label" width="5%">Prot.</td>
                            <td align="left" width="10%">
                                <asp:TextBox ID="txtConfirmProt" runat="server" Width="86px" MaxLength="255"></asp:TextBox>
                            </td>
                        </tr>
                        <tr width="100%">
                            <td class="label" width="20%">Scadenza:</td>
                            <td align="left" width="15%">
                                <telerik:RadDatePicker ID="rdpWaitDate" runat="server" />
                            </td>
                            <td class="label" style="width: 35px"></td>
                            <td align="left" width="10%"></td>
                            <td class="label" width="20%">Risposta:</td>
                            <td align="left" width="15%">
                                <telerik:RadDatePicker ID="rdpResponseDate" runat="server" />
                            </td>
                            <td class="label" width="5%">Prot.</td>
                            <td align="left" width="10%">
                                <asp:TextBox ID="txtResponseProt" runat="server" Width="86px" MaxLength="255"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trOCComment" runat="server">
                <td style="width: 20%;" class="label">Commento:</td>
                <td>
                    <asp:DropDownList ID="ddlOCComment" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="trOCDocument" runat="server">
                <td style="width: 20%;" class="label">Documenti:</td>
                <td>
                    <usc:UscDocumentUpload ButtonFDQEnabled="false" ButtonFrontespizioEnabled="false" ButtonPdfAndFDQEnabled="false" ButtonPreviewEnabled="false" ButtonSharedFolederEnabled="false" HeaderVisible="false" ID="uscOCDocument" IsDocumentRequired="false" MultipleDocuments="false" runat="server" Type="Resl" />
                </td>
            </tr>
            <tr id="trOCOpinion" runat="server">
                <td style="width: 20%;" class="label">Parere:
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOCOpinion" runat="server" Width="100%" MaxLength="255" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <%-- Comunicazione --%>
    <table id="tblComunication" runat="server" class="datatable" width="100%">
        <tr>
            <th colspan="2">Comunicazione</th>
        </tr>
        <%-- Destinatri --%>
        <tr id="trContactDest" runat="server">
            <td class="label" style="width: 20%; vertical-align: middle;">Destinatari:</td>
            <td>
                <table style="width: 100%;">
                    <tr id="trDest" runat="server">
                        <td>
                            <usc:UscContact ID="uscContactDest" runat="server" TreeViewCaption="Destinatari"
                                ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false"
                                ButtonPropertiesVisible="false" Multiple="true" MultiSelect="true"
                                HeaderVisible="false" IsRequired="false" Type="Resl" />
                        </td>
                    </tr>
                    <tr id="trAlternativeDest" runat="server">
                        <td>
                            <telerik:RadTextBox ID="txtAlternativeDest" runat="server" Width="100%" MaxLength="500"></telerik:RadTextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%-- Proponente --%>
        <tr id="trContactProp" runat="server">
            <td class="label" style="width: 20%; vertical-align: middle;">Proponente:</td>
            <td>
                <telerik:RadAjaxPanel runat="server" Width="100%" style="margin: 2px;" ID="pnlProponente">
                    <usc:UscContact ID="uscContactProp" runat="server" TreeViewCaption="Proponente" ButtonImportVisible="false"
                        ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false"
                        HeaderVisible="false" IsRequired="false" Type="Resl" Multiple="true" />
                    <telerik:RadTextBox ID="txtAlternativeProp" runat="server" Width="100%" MaxLength="500"></telerik:RadTextBox>
                    <%-- Settore proponente --%>
                    <usc:Settori HeaderVisible="False" MultipleRoles="False" Environment="Resolution" RoleRestictions="OnlyMine" ID="uscRoleProposer" ReadOnly="False" runat="server" />
                </telerik:RadAjaxPanel>
            </td>
        </tr>
        <%-- Assegnatario --%>
        <tr id="trContactAss" runat="server">
            <td class="label" style="width: 20%; vertical-align: middle;">Assegnatario:</td>
            <td>
                <table style="width: 100%;">
                    <tr id="trAss" runat="server">
                        <td>
                            <usc:UscContact ID="uscContactAss" runat="server" TreeViewCaption="Assegnatario"
                                ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false"
                                ButtonPropertiesVisible="false" HeaderVisible="false" IsRequired="false"
                                Type="Resl" />
                        </td>
                    </tr>
                    <tr id="trAlternativeAss" runat="server">
                        <td>
                            <telerik:RadTextBox ID="txtAlternativeAss" runat="server" Width="100%" MaxLength="500"></telerik:RadTextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%-- Responsabile --%>
        <tr id="trContactMgr" runat="server">
            <td class="label" style="width: 20%; vertical-align: middle;">Responsabile:</td>
            <td>
                <table style="width: 100%;">
                    <tr id="trMgr" runat="server">
                        <td>
                            <usc:UscContact ID="uscContactMgr" runat="server" TreeViewCaption="Responsabile"
                                ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false"
                                ButtonPropertiesVisible="false" HeaderVisible="false" IsRequired="false"
                                Type="Resl" />
                        </td>
                    </tr>
                    <tr id="trAlternativeMgr" runat="server">
                        <td>
                            <telerik:RadTextBox ID="txtAlternativeMgr" MaxLength="500" runat="server" Width="100%" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <%-- Stato --%>
    <table id="tblStatus" class="datatable" runat="server" width="100%">
        <tr>
            <th colspan="2">Stato</th>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Stato:</td>
            <td style="width: 80%">
                <asp:DropDownList AutoPostBack="True" ID="ddlStatus" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Causale:</td>
            <td style="width: 80%">
                <telerik:RadTextBox ID="txtChangedReason" runat="server" Width="100%" MaxLength="255" />
            </td>
        </tr>
    </table>
    <%-- Altro --%>
    <table id="tblOther" class="datatable" runat="server" width="100%">
        <tr>
            <th colspan="2">Altre</th>
        </tr>
        <tr id="trContainer" runat="server">
            <td class="label" style="width: 20%">Contenitore:</td>
            <td style="width: 80%">
                <asp:DropDownList ID="ddlIdContainer" runat="server" />
            </td>
        </tr>
        <tr id="trPublication" runat="server" visible="false">
            <td class="label" style="width: 20%">Pubb. su Internet:</td>
            <td style="width: 80%">
                <asp:CheckBox ID="ckbPublication" runat="server" Width="100%" />
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
                                            <asp:ImageButton runat="server" ID="btnCorrectSeries" Style="margin-left: 10px; cursor: pointer;" CausesValidation="False" ImageUrl="../App_Themes/DocSuite2008/imgset16/pencil.png" ToolTip="Correggi Bozza selezionata" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </asp:Panel>
</div>
</asp:Panel>
