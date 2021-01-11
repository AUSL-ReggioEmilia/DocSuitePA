<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolution.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolution" %>
<%@ Register Src="uscSettori.ascx" TagName="uscSettori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContactSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscResolutionWorkflow.ascx" TagName="uscWorkFlow" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscResolutionOC.ascx" TagName="uscOC" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscAmmTraspMonitorLog.ascx" TagName="uscAmmTraspMonitorLog" TagPrefix="ucs" %>
<%@ Register Src="~/UserControl/uscMulticlassificationRest.ascx" TagName="uscMulticlassificationRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentUnitReferences.ascx" TagName="uscDocumentUnitReferences" TagPrefix="usc" %>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
        //<![CDATA[

        function OpenParerDetail(id) {
            var wnd = window.radopen("<%=ParerDetailUrl() %>?Type=Resl&id=" + id, "parerDetailWindow");
            wnd.setSize(400, 300);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            wnd.set_visibleStatusbar(false);
            wnd.set_modal(true);
            // wnd.add_close(OnClientClose);
            wnd.center();
            return false;
        }

        function OnClientClose(sender, eventArgs) {
            var returnValue = sender.argument;
            if (returnValue) {
                window.location.href = returnValue;
            }
            sender.remove_close(OnClientClose);

        }
        function ExecuteAjaxRequest(operationName) {
            var manager = <%= AjaxManager.ClientID %>;
            manager.ajaxRequest(operationName);
        }

        function GoToDraftSeries(idSeriesItem) {
            ExecuteAjaxRequest("goToDraftSeries|" + idSeriesItem);
            return false;
        }

        function CorrectDraftLink(idSeriesItem) {
            ExecuteAjaxRequest("correctDraftLink|" + idSeriesItem);
            return false;
        }
			 //]]>
    </script>
</telerik:RadCodeBlock>

<telerik:RadAjaxLoadingPanel ID="DefaultLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>

<div id="divProtocollo">

    <%--resolution--%>
    <table id="tblResolution" class="datatable" runat="server">
        <tr>
            <th colspan="4"></th>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                <%=Me.GetProvNumberLabel()%>
            </td>
            <td style="width: 30%">
                <b>
                    <%=Me.GetProvNumber()%>
                </b>
            </td>
            <td class="label" style="width: 20%">
                <%=Me.GetFullNumberLabel()%>
            </td>
            <td style="width: 30%">
                <b>
                    <%=Me.GetFullNumber()%>
                </b>
            </td>
        </tr>
        <%--Check Immediatamente Esecutiva--%>
        <tr id="trResolutionImmediatelyExecutive" runat="server" visible="false">
            <td class="label" style="width: 20%; height: 22px; vertical-align: middle;">
                <asp:CheckBox ID="chkExecutive" runat="server" Enabled="false" />
            </td>
            <td style="height: 22px; vertical-align: middle;" colspan="3">
                <b>Immediatamente Esecutiva.</b>
            </td>
        </tr>
        <%--Protocollo Trasmissione Servizi--%>
        <tr id="trResolutionProposerProtocolLink" runat="server" visible="false">
            <td></td>
            <td style="vertical-align: middle;" colspan="3">
                <b><%=GetAdoptionTrasmissionLabel()%></b>
            </td>
        </tr>
    </table>
    <%--Flusso di Lavoro--%>
    <usc:uscWorkFlow runat="server" ID="uscWorkflow" />
    <table id="tblWorkflow" class="datatable" runat="server" summary="Flusso di Lavoro">
        <tr>
            <th colspan="6">Flusso di Lavoro
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Data Adozione:
            </td>
            <td style="width: 15%">
                <%=String.Format("{0:dd/MM/yyyy}", CurrentResolution.AdoptionDate)%>
            </td>
            <td class="label" style="width: 15%">Data pubblicazione:
            </td>
            <td style="width: 15%">
                <%=String.Format("{0:dd/MM/yyyy}", CurrentResolution.PublishingDate)%>
            </td>
            <td class="label" style="width: 15%">Data Esecuzione:
            </td>
            <td style="width: 20%">
                <%=String.Format("{0:dd/MM/yyyy}", CurrentResolution.EffectivenessDate)%>
            </td>
        </tr>
    </table>
    <%--Messaggi di ritorno di step--%>
    <table id="tblMotivazione" class="datatable" runat="server" visible="false">
        <tr>
            <th colspan="2">
                <asp:Label runat="server" ID="lblResolutionDeleteReason"></asp:Label></th>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Motivazione:</td>
            <td style="width: 80%" id="td1">
                <%= GetMotivazioneDescription() %>
            </td>
        </tr>
    </table>
    <%--Organo di Controllo: ASL-TO2--%>
    <usc:uscOC runat="server" ID="uscOC" Type="Resl" Visible="false" />
    <!-- Organo di controllo -->
    <table id="tblODC" class="datatable" runat="server" summary="Organo di Controllo">
        <tr>
            <th colspan="4">Organo di controllo</th>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Spedizione:</td>
            <td style="width: 20%">
                <%=CurrentResolution.WarningDateFormat("{0:dd/MM/yyyy}")%>
                <%= If(String.IsNullOrEmpty(CurrentResolution.WarningProtocol), String.Empty, " n. " & CurrentResolution.WarningProtocol)%>
            </td>
            <td class="label" style="width: 20%">Ricezione:</td>
            <td style="width: 40%">
                <%=CurrentResolution.ConfirmDateFormat("{0:dd/MM/yyyy}")%>
                <%= If(String.IsNullOrEmpty(CurrentResolution.ConfirmProtocol), String.Empty, " n. " & CurrentResolution.ConfirmProtocol)%>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Scadenza:</td>
            <td style="width: 20%">
                <%=CurrentResolution.WaitDateFormat("{0:dd/MM/yyyy}")%>
            </td>
            <td class="label" style="width: 20%">Risposta:</td>
            <td style="width: 40%">
                <%=CurrentResolution.ResponseDateFormat("{0:dd/MM/yyyy}")%>
                <%= If(String.IsNullOrEmpty(CurrentResolution.ResponseProtocol), String.Empty, " n. " & CurrentResolution.ResponseProtocol)%>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Commento:</td>
            <td style="width: 20%">
                <%=GetControllerDescripton()%>
            </td>
            <td class="label" style="width: 20%">
                <asp:Label runat="server" ID="lblDocumento" Text="Documento:"></asp:Label>
            </td>
            <td style="width: 40%">
                <asp:ImageButton runat="server" ID="imgDocumento" ImageUrl="../Resl/Images/FileOC.gif"
                    AlternateText="Risposta OC" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Parere:</td>
            <td style="width: 20%" colspan="3">
                <%=CurrentResolution.ControllerOpinion%>
            </td>
        </tr>
    </table>
    <%--Stato--%>
    <table id="tblStatus" class="datatable" runat="server">
        <tr id="trStatusTitle">
            <th colspan="2">Stato</th>
        </tr>
        <tr id="trStatusDescrition" runat="server">
            <td class="label" style="width: 20%">Stato:</td>
            <td style="width: 80%" id="tdStatusDescription">
                <%= GetStatusDescription() %>
            </td>
        </tr>
        <tr id="trLastChangedReason" runat="server">
            <td class="label" style="width: 20%">Causale:</td>
            <td style="width: 80%" id="tdAccountingSectional">
                <%=CurrentResolution.LastChangedReason%>
            </td>
        </tr>
        <tr id="trLastConfrimView" runat="server">
            <td class="label" style="width: 20%">Controllato da:</td>
            <td style="width: 80%">
                <%=GetLastResolutioLog%>
            </td>
        </tr>
    </table>
    <%--Oggetto Privacy--%>
    <table id="tblObjectPrivacy" runat="server" class="datatable" visible="false">
        <tr>
            <th colspan="2">
                <asp:Label runat="server" ID="lblObjectPrivacy"></asp:Label></th>
        </tr>
        <tr id="tr3">
            <td class="label" style="width: 20%">
                <asp:Label runat="server" ID="lblObjectPrivacyDetail"></asp:Label></td>
            <td style="width: 80%">
                <%=CurrentResolution.ResolutionObjectPrivacy%>
            </td>
        </tr>
    </table>
    <%--Oggetto--%>
    <table id="tblObject" runat="server" class="datatable">
        <tr>
            <th colspan="2">Oggetto</th>
        </tr>
        <tr id="trObjectDescription">
            <td class="label" style="width: 20%">Oggetto:</td>
            <td style="width: 80%">
                <%=CurrentResolution.ResolutionObject%>
            </td>
        </tr>
        <tr id="trObjectNote">
            <td class="label" style="width: 20%">Note:</td>
            <td style="width: 80%;">
                <span style="font-weight: bold"><%=CurrentResolution.Note%></span>
            </td>
        </tr>
    </table>
    <%--Dati Autorizzazioni--%>
    <table id="tblDataRole" runat="server" class="datatable">
        <tr>
            <th colspan="2">Dati</th>
        </tr>
        <tr id="tr1">
            <td class="label" style="width: 20%">Oggetto:</td>
            <td style="width: 80%">
                <%=CurrentResolution.ResolutionObject%>
            </td>
        </tr>
        <tr id="tr2">
            <td class="label" style="width: 20%">Locazione:</td>
            <td style="width: 80%">
                <%=CurrentResolution.Location.Id%>&nbsp;<%=CurrentResolution.Location.Name%>
            </td>
        </tr>
    </table>
    <%--Dati Economici--%>
    <table class="datatable" id="tblEconomicData" runat="server">
        <tr>
            <th colspan="4">Dati Economici</th>
        </tr>
        <tr>
            <td style="width: 20%;" class="label">Posizione:
            </td>
            <td style="width: 20%;">
                <%=CurrentResolution.Position%>
            </td>
            <td style="width: 20%;" class="label">Validità Contratto:
            </td>
            <td style="width: 40%;">
                <%=GetValidityContractDate()%>
            </td>
        </tr>
        <tr>
            <td style="width: 20%;" class="label">Tipo di Gara:
            </td>
            <td style="width: 80%;" colspan="3">
                <%=GetBidTypeDescription()%>
            </td>
        </tr>
        <tr>
            <td style="width: 20%;" class="label">Fornitore:
            </td>
            <td style="width: 80%;" colspan="3">
                <%=CurrentResolution.SupplierCode%>
                &nbsp;<%=CurrentResolution.SupplierDescription%></td>
        </tr>
    </table>
    <%--Comunicazione--%>
    <table id="tblComunication" class="datatable" runat="server">
        <tr>
            <th colspan="4">Comunicazione</th>
        </tr>
        <%--Destinatario/Proponente--%>
        <tr id="trDestPropAlternative" runat="server">
            <td style="width: 20%;" class="label">Destinatario:
            </td>
            <td style="width: 30%;">
                <%= GetAlternativeRecipient() %>
            </td>
            <td style="width: 20%;" class="label">Proponente:
            </td>
            <td style="width: 30%;">
                <%= GetAlternativeProposer() %>
            </td>
        </tr>
        <tr id="trDestProp" runat="server">
            <td style="width: 50%;" colspan="2">
                <usc:uscContactSel runat="server" ID="uscContactRecipient" TreeViewCaption="Destinatari" Multiple="true"
                    ReadOnly="true" Type="Resl" HeaderVisible="false" IsRequired="false" />
            </td>
            <td style="width: 50%;" colspan="2">
                <usc:uscContactSel runat="server" ID="uscContactProposer" TreeViewCaption="Proponente" Multiple="true"
                    ReadOnly="true" Type="Resl" HeaderVisible="false" IsRequired="false" />
            </td>
        </tr>
        <tr class="Spazio">
            <td colspan="4">&nbsp;</td>
        </tr>
        <%--Assegnatario/Responsabile--%>
        <tr id="trAssMgrAlternative" runat="server">
            <td style="width: 20%;" class="label">Assegnatario:
            </td>
            <td style="width: 30%;">
                <%= GetAlternativeAssignee() %>
            </td>
            <td style="width: 20%;" class="label">Responsabile:
            </td>
            <td style="width: 30%;">
                <%=GetAlternativeManager()%>
            </td>
        </tr>
        <tr id="trAssMgr" runat="server">
            <td style="width: 50%;" colspan="2">
                <usc:uscContactSel runat="server" ID="uscContactAssignee" TreeViewCaption="Assegnatari" Multiple="true"
                    ReadOnly="true" Type="Resl" HeaderVisible="false" IsRequired="false" />
            </td>
            <td style="width: 50%;" colspan="2">
                <usc:uscContactSel runat="server" ID="uscContactManager" TreeViewCaption="Responsabili" Multiple="true"
                    ReadOnly="true" Type="Resl" HeaderVisible="false" IsRequired="false" />
            </td>
        </tr>
    </table>
    <%--Comunicazione Storico--%>
    <table class="datatable" id="tblComunicationStorico" runat="server">
        <tr>
            <th colspan="4">Comunicazione
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 20%; height: 21px;">Destinatario:
            </td>
            <td style="height: 21px">
                <%= GetAlternativeRecipient() %>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Proponente:
            </td>
            <td>
                <%= GetAlternativeProposer() %>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Assegnatario:
            </td>
            <td>
                <%= GetAlternativeAssignee() %>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Responsabile:
            </td>
            <td>
                <%=GetAlternativeManager()%>
            </td>
        </tr>
    </table>
    <%--Autorizzazioni--%>
    <usc:uscSettori ID="uscSettori" ReadOnly="true" Required="false" runat="server" Visible="false" Environment="Resolution" LoadUsers="false" />
    <%--Classificatore--%>
    <table class="datatable" id="tblCategory" runat="server">
        <tr>
            <th colspan="2">Classificazione</th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Codice:<br />
                Descrizione:</td>
            <td style="width: 85%">
                <%=CurrentResolution.CategoryCode%>
                <br />
                <%= Facade.ResolutionFacade.GetCategoryDescription(CurrentResolution)%>
            </td>
        </tr>
    </table>
    <%--Multiclassificazioni--%>
    <usc:uscMulticlassificationRest runat="server" ID="uscMulticlassificationRest" Type="Resl" Visible="false" />
    <%--PARER e Atto dematerializzato--%>
    <table id="tblParer" class="datatable" runat="server" visible="false">
        <tr>
            <th colspan="3" style="width: 100%; border-right: 0;">Stato di conservazione</th>
            <th>
                <asp:ImageButton runat="server" ID="parerInfo" Style="float: right;" ImageUrl="~/Comm/Images/info.png" /></th>
        </tr>
        <tr>
            <td colspan="4" style="margin: 5px">
                <asp:Image runat="server" ID="parerIcon" Style="vertical-align: middle" />
                <asp:Label runat="server" ID="parerLabel" Text="" />
            </td>
        </tr>
        <tr>
            <th colspan="4">Atto dematerializzato</th>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Atto dematerializzato:</td>
            <td colspan="3" style="width: 80%">
                <asp:Label ID="lbDataDematerializzazione" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Data versamento:</td>
            <td style="width: 20%">
                <asp:Label ID="lbDataVersamento" runat="server"></asp:Label></td>
            <td class="label" style="width: 20%">Collegamento:</td>
            <td style="width: 40%">
                <asp:HyperLink ID="linkUriVersamento" runat="server" Text="Parer"></asp:HyperLink></td>
        </tr>
    </table>
    <%--Altre--%>
    <table class="datatable" id="tblOther" runat="server">
        <tr>
            <th colspan="4">Altre</th>
        </tr>
        <tr>
            <td class="label" style="width: 20%">Contenitore:</td>
            <td style="width: 30%; vertical-align: middle;">
                <%=CurrentResolution.Container.Name%>
            </td>
            <td id="tbPublicationLabel" runat="server" class="label" style="width: 20%">
                <asp:Label runat="server" ID="lblPubblication" Text="Pubb. Internet:" />
            </td>
            <td id="tbPublication" runat="server" style="width: 30%">
                <asp:CheckBox ID="chkPublication" runat="server" Enabled="false" />
            </td>
        </tr>
        <tr runat="server" id="tblWebPubblicationState">
            <td class="label" style="width: 20%; height: 21px;"></td>
            <td style="vertical-align: middle; width: 30%; height: 21px;"></td>
            <td runat="server" class="label" style="width: 20%; height: 21px;">
                <asp:Label ID="lbStatoWeb" runat="server" Text="Albo Online:" />
            </td>
            <td id="WebStateTD" style="width: 30%; height: 21px;">
                <asp:Label runat="server" ID="_webStateLabel" />
            </td>
        </tr>
        <tr runat="server" id="tblWebPublicationData">
            <td class="label" style="width: 20%"></td>
            <td style="vertical-align: middle; width: 30%"></td>
            <td runat="server" class="label" style="width: 20%">
                <asp:Label ID="lbDataPubbWeb" runat="server" Text="Data pub. Albo Online:" />
            </td>
            <td runat="server" style="width: 30%">
                <asp:Label runat="server" ID="_webPublicationDateLabel" />
            </td>
        </tr>
        <tr runat="server" id="tblWebRetireData">
            <td class="label" style="width: 20%; height: 21px;"></td>
            <td style="vertical-align: middle; width: 30%; height: 21px;"></td>
            <td runat="server" class="label" style="width: 20%; height: 21px;">
                <asp:Label ID="lbDataRetireWeb" runat="server" Text="Data ritiro Albo Online:" />
            </td>
            <td runat="server" style="width: 30%; height: 21px;">
                <asp:Label runat="server" ID="_webRevokeDateLabel" />
            </td>
        </tr>
        <tr runat="server" id="tblSettoriAutoriz">
            <td class="label" style="width: 20%">Settori Autoriz.:</td>
            <td style="width: 80%; vertical-align: middle;" colspan="3">&nbsp;
            </td>
        </tr>
        <tr id="collaborationPanel" runat="server" visible="false">
            <td class="label" style="width: 20%">Collaborazione:</td>
            <td style="width: 80%; vertical-align: middle;" colspan="3">
                <asp:HyperLink runat="server" ID="cmdCollaboration" />
            </td>
        </tr>
    </table>
    <%--Web Publication Piacenza--%>
    <asp:Panel ID="AuslPcWebPublication" runat="server">
        <table id="tblWebPublication" class="datatable" runat="server" visible="false" border="1">
            <tr>
                <th>Storico pubblicazione</th>
            </tr>
            <tr>
                <td>
                    <div style="margin: 5px">
                        <asp:Repeater ID="WebPublicationRepeater" runat="server">
                            <ItemTemplate>
                                <span><%# GetWebPublicationDescription(Eval("Id"), Eval("IsPrivacy"))%></span><br />
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
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
                                        <asp:HyperLink runat="server" ID="documentSeriesLink" NavigateUrl="javascript:void(0)" CausesValidation="False"></asp:HyperLink>
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

    <%-- Monitoraggio --%>
    <ucs:uscAmmTraspMonitorLog runat="server" ID="uscAmmTraspMonitorLog" />

    <%--serie--%>
    <usc:uscDocumentUnitReferences Visible="true" ID="uscDocumentUnitReferences" runat="server" ShowDocumentSeriesResolutionsLinks="true" ShowResolutionDocumentSeriesLinks="true" ShowResolutionlMessageLinks="true" />

</div>
