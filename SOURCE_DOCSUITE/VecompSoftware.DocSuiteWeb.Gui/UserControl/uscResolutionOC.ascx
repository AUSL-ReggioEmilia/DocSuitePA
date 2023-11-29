<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionOC.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionOC" %>
<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Data" %>

 <%--Lista Organo di Controllo--%> 
<table id="tblODC" class="datatable" runat="server" summary="Organo di Controllo" visible="false">
    <tr>
        <th colspan="4">
            Organi di controllo
        </th>
    </tr>
    <tr id="trOCCollegioSindacaleRegione" runat="server">
        <td class="label" style="width: 20%;vertical-align:middle;">
            <asp:CheckBox ID="chkSupervisoryBoard" runat="server" Enabled="false" />
        </td>
        <td id="tdCollegioSindacale" style="width: 20%;vertical-align:middle;">
            Collegio Sindacale.
        </td>
        <td class="label" style="width: 20%;vertical-align:middle;">
            <asp:CheckBox ID="chkRegion" runat="server" Enabled="false" />
        </td>
        <td id="tdRegione" style="width: 40%;vertical-align:middle;">
            Regione.
        </td>
    </tr>
    <tr id="trOCControlloGestioneCorteConti" runat="server">
        <td class="label" style="width: 20%;vertical-align:middle;">
            <asp:CheckBox ID="chkManagement" runat="server" Enabled="false" />
        </td>
        <td id="tdControlloDiGestione" style="width: 20%;vertical-align:middle;">
            Controllo di Gestione.
        </td>
        <td class="label" style="width: 20%;vertical-align:middle;">
            <asp:CheckBox ID="chkCorteConti" runat="server" Enabled="false" />
        </td>
        <td id="tdCorteDeiConti" style="width: 40%;vertical-align:middle;">
            Corte dei Conti.
        </td>
    </tr>
    <tr id="trOCAltro" runat="server">
        <td class="label" style="width: 20%;vertical-align:middle;">
            <asp:CheckBox ID="chkOther" runat="server" Enabled="false" />
            <asp:CheckBox ID="chkConfSindaci" runat="server" Enabled="false" Visible="false" />
        </td>
        <td id="tdAltro" style="width: 20%;vertical-align:middle;">
            Altro.
        </td>
        <td class="label" style="width: 20%;vertical-align:middle;">
        </td>
        <td style="width: 40%;vertical-align:middle;">
        </td>
    </tr>
</table>

<%-- OC List --%>
<table id="tblODCOptions" class="datatable" runat="server" summary="Organo di Controllo" visible="false">
    <tr>
        <th colspan="2">
            Organi di controllo
        </th>
    </tr>
    <tr>
        <td class="label" width="20%">&nbsp;</td>
        <td>
            <asp:Panel ID="pnlOCSupervisoryBoard" runat="server">
                <asp:CheckBox ID="chkOCSupervisoryBoard" runat="server" Text="Collegio Sindacale" Enabled="false" />
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlOCConfSindaci" runat="server" Visible="false">
                <asp:CheckBox ID="chkOCConfSindaci" runat="server" Text="Controllo conferenza dei sindaci" Enabled="false"  />
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlOCRegion" runat="server">
                <asp:CheckBox ID="chkOCRegion" runat="server" Text="Regione" Enabled="false" />
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlOCManagement" runat="server">
                <asp:CheckBox ID="chkOCManagement" runat="server" Text="Controllo di gestione" Enabled="false"  />
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlOCCorteConti" runat="server">
                <asp:CheckBox ID="chkOCCorteConti" runat="server" Text="Corte dei conti" Enabled="false"  />
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlOCOther" runat="server">
                <asp:CheckBox ID="chkOCOther" runat="server" Text="Altro" Enabled="false" />
            </asp:Panel>
        </td>
    </tr>
</table>

 <%--OC: Collegio Sindacale--%> 
<table id="tblOCSupervisoryBoard" class="datatable" runat="server" summary="Collegio Sindacale" visible="false">
    <tr>
        <th colspan="4">
            &nbsp; - Collegio Sindacale
        </th>
    </tr>
    <tr>
        <td class="label" style="width: 20%;vertical-align:middle;">Spedizione:</td>
        <td style="width: 40%;vertical-align:middle;"><%= CurrentResolution.SupervisoryBoardWarningDateFormat("{0:dd/MM/yyyy}") %></td>
        <td class="label" style="width: 25%;vertical-align:middle;">
            Prot. n. <%=Resolution.FormatProtocolLink(CurrentResolution.SupervisoryBoardProtocolLink, "")%>
        </td>
        <td style="width: 15%;vertical-align:middle;">
            <asp:ImageButton runat="server" ID="imgSupervisoryBoardLink" ImageUrl="../Comm/Images/DocSuite/Protocollo16.png" AlternateText="Collegamento al Protocollo" Visible="false" />
        </td>
    </tr>
    <tr id="trSupervisoryBoardOpinion" runat="server">
        <td class="label" style="width: 20%;vertical-align:middle;">Rilievo</td>
        <td style="width: 40%;vertical-align:middle;"><%=CurrentResolution.SupervisoryBoardOpinion%></td>
        <td class="label" style="width: 25%;vertical-align:middle;">
            <asp:Label runat="server" ID="lblSupervisoryBoardFile" Text=""></asp:Label>
        </td>
        <td style="width: 15%;vertical-align:middle;">
            <asp:ImageButton runat="server" ID="imgSupervisoryBoardFile" ImageUrl="../Resl/Images/FileOC.gif" AlternateText="Risposta Collegio" ToolTip="Risposta Collegio" Visible="false" />
        </td>
    </tr>
</table>

 <%--OC: Conferenza dei Sindaci--%> 
<table id="tblOCConfSindaci" class="datatable" runat="server" summary="Conferenza dei Sindaci" visible="false">
    <tr>
        <th colspan="4">
            &nbsp; - Conferenza dei Sindaci
        </th>
    </tr>
    <tr>
        <td class="label" style="width: 20%;vertical-align:middle;">Spedizione:</td>
        <td style="width: 40%;vertical-align:middle;"><%= String.Format("{0:dd/MM/yyyy}", CurrentResolution.ManagementWarningDate)%></td>
        <td class="label" style="width: 25%;vertical-align:middle;">
            Prot. n. <%= Resolution.FormatProtocolLink(CurrentResolution.ManagementProtocolLink, String.Empty)%>
        </td>
        <td style="width: 15%;vertical-align:middle;">
            <asp:ImageButton runat="server" ID="imgConfSindaciProtocolLink" ImageUrl="../Comm/Images/DocSuite/Protocollo16.png" AlternateText="Collegamento al Protocollo" Visible="false" />
        </td>
    </tr>
</table>

 <%--OC: Regione--%> 
<table id="tblOCRegion" class="datatable" runat="server" summary="Regione" visible="false">
    <tr id="trRegionTitolo" runat="server">
        <th colspan="4">
            &nbsp; - Regione
        </th>
    </tr>
    <tr id="trRegionSpedizione" runat="server">
        <td class="label" style="width: 20%;vertical-align:middle;">Spedizione:</td>
        <td style="width: 40%;vertical-align:middle;"><%=CurrentResolution.WarningDateFormat("{0:dd/MM/yyyy}")%></td>
        <td class="label" style="width: 25%;vertical-align:middle;">
            Prot. n. <%=Resolution.FormatProtocolLink(CurrentResolution.RegionProtocolLink, "")%>
        </td>
        <td style="width: 15%">
            <asp:ImageButton runat="server" ID="imgRegionProtocolLink" ImageUrl="../Comm/Images/DocSuite/Protocollo16.png" AlternateText="Collegamento al Protocollo" Visible="false" />
        </td>
    </tr>
    <tr id="trRegionRicezioneScadenza" runat="server">
        <td id="tdConfirmDateLabel" runat="server" class="label" style="width: 20%;vertical-align:middle;">Ricezione:</td>
        <td id="tdConfirmDateContent" runat="server" style="width: 40%;vertical-align:middle;"><%=CurrentResolution.ConfirmDateFormat("{0:dd/MM/yyyy}")%></td>
        <td id="tdWaitDateLabel" runat="server" class="label" style="width: 25%;vertical-align:middle;">Scadenza:</td>
        <td id="tdWaitDateContent" runat="server" style="width: 15%;vertical-align:middle;"><%=CurrentResolution.WaitDateFormat("{0:dd/MM/yyyy}")%></td>
    </tr>
    <tr id="trRegionDGRProtocollo" runat="server">
        <td id="tdResponseLabel" runat="server" class="label" style="width: 20%;vertical-align:middle;">Data DGR:</td>
        <td id="tdResponseContent" runat="server" style="width: 40%;vertical-align:middle;"><%=CurrentResolution.ResponseDateFormat("{0:dd/MM/yyyy}")%></td>
        <td id="tdDGRLabel" runat="server" class="label" style="width: 25%;vertical-align:middle;">DGR:</td>
        <td id="tdDGRContent" runat="server" style="width: 15%;vertical-align:middle;"><%=CurrentResolution.DGR%></td>
        <td id="tdResponseProtocolLink" runat="server" class="label" style="width: 25%;vertical-align:middle;" visible="false">
            Prot. n. <%=Resolution.FormatProtocolLink(CurrentResolution.ResponseProtocol, "")%>
        </td>
        <td style="width: 15%">
            <asp:ImageButton runat="server" ID="imgRegionResponseProtocolLink" ImageUrl="../Comm/Images/DocSuite/Protocollo16.png" AlternateText="Collegamento al Protocollo" Visible="false" />
        </td>
    </tr>
    <tr id="trRegionInvioChiarimenti" runat="server" visible="false">
        <td id="tdRegionInvioChiarimentiLabel" runat="server" class="label" style="width: 20%;vertical-align:middle;">Invio chiarimenti alla Regione:</td>
        <td id="tdRegionInvioChiarimentiContent" runat="server" style="width: 40%;vertical-align:middle;"><%=CurrentResolution.ConfirmDateFormat("{0:dd/MM/yyyy}")%></td>
        <td class="label" style="width: 25%;vertical-align:middle;">
            Prot. n. <%=Resolution.FormatProtocolLink(CurrentResolution.ConfirmProtocol, "")%>
        </td>
        <td style="width: 15%">
            <asp:ImageButton runat="server" ID="imgRegionInvioCommentoRegioneLink" ImageUrl="../Comm/Images/DocSuite/Protocollo16.png" AlternateText="Collegamento al Protocollo" Visible="false" />
        </td>
    </tr>
    <tr id="trRegionCommento" runat="server">
        <td id="tdCommentoLabel" runat="server" class="label" style="width: 20%;vertical-align:middle;">Commento:</td>
        <td id="tdCommentoContent" runat="server" style="width: 40%;vertical-align:middle;"><%= Me.GetControllerDescripton() %></td>
        <td id="tdRegionFileLabel" runat="server" class="label" style="width: 25%;vertical-align:middle;">
            <asp:Label runat="server" ID="lblRegionFile" Text=""></asp:Label>
        </td>
        <td id="tdRegionFileContent" runat="server" style="width: 15%;vertical-align:middle;">
            <asp:ImageButton runat="server" ID="imgRegionFile" ImageUrl="../Resl/Images/FileOC.gif" AlternateText="Risposta Regione" ToolTip="Risposta Regione" Visible="false" />
        </td>
    </tr>
    <tr id="trRegionNote" runat="server">
        <td id="tdControllerOpinionLabel" runat="server" class="label" style="width: 20%;vertical-align:middle;">Note:</td>
        <td id="tdControllerOpinionContent" runat="server" style="width: 80%" colspan="3"><%=CurrentResolution.ControllerOpinion%></td>
    </tr>
    <tr id="trRegionApprovazione" runat="server">
        <td id="tdApprovalNoteLabel" runat="server" class="label" style="width: 20%;vertical-align:middle;">Note Approvazione:</td>
        <td id="tdApprovalNoteContent" runat="server" style="width: 80%" colspan="3"><%=CurrentResolution.ApprovalNote %></td>
    </tr>
    <tr id="trRegionDecadimento" runat="server">
        <td id="tdDeclineNoteLabel" runat="server" class="label" style="width: 20%;vertical-align:middle;">Note Decadimento:</td>
        <td id="tdDeclineNoteContent" runat="server" style="width: 80%" colspan="3"><%=CurrentResolution.DeclineNote %></td>
    </tr>
</table>

 <%--OC: Corte dei Conti--%> 
<table id="tblOCCorteDeiConti" class="datatable" runat="server" summary="Collegio Sindacale" visible="false">
    <tr>
        <th colspan="4">
            &nbsp; - Corte dei Conti
        </th>
    </tr>
    <tr>
        <td class="label" style="width: 20%;vertical-align:middle;">Spedizione:</td>
        <td style="width: 40%;vertical-align:middle;"><%= String.Format("{0:dd/MM/yyyy}", CurrentResolution.CorteDeiContiWarningDate)%></td>
        <td class="label" style="width: 25%;vertical-align:middle;">
            Prot. n. <%= Resolution.FormatProtocolLink(CurrentResolution.CorteDeiContiProtocolLink, String.Empty)%>
        </td>
        <td style="width: 15%;vertical-align:middle;">
            <asp:ImageButton runat="server" ID="imgCorteDeiContiProtocolLink" ImageUrl="../Comm/Images/DocSuite/Protocollo16.png" AlternateText="Collegamento al Protocollo" Visible="false" />
        </td>
    </tr>
</table>

 <%--OC: Controllo di Gestione--%> 
<table id="tblOCManagement" class="datatable" runat="server" summary="Controllo di Gestione" visible="false">
    <tr>
        <th colspan="4">
            &nbsp; - Controllo di Gestione
        </th>
    </tr>
    <tr>
        <td class="label" style="width:20%;vertical-align:middle;">Spedizione</td>
        <td style="width: 40%;vertical-align:middle;"><%=CurrentResolution.ManagementWarningDateFormat("{0:dd/MM/yyyy}")%></td>
        <td class="label" style="width:25%;vertical-align:middle;">
            Prot. n. <%=Resolution.FormatProtocolLink(CurrentResolution.ManagementProtocolLink, "")%>
        </td>
        <td style="width: 15%;vertical-align:middle;">
            <asp:ImageButton runat="server" ID="imgManagmentProtocolLink" ImageUrl="../Comm/Images/DocSuite/Protocollo16.png" AlternateText="Collegamento al Protocollo" Visible="false" />
        </td>
    </tr>
</table>


 <%--OC: Altro--%> 
<table id="tblOCOther" class="datatable" runat="server" summary="Altro" visible="false">
    <tr>
        <th colspan="2">
            &nbsp; - Altro
        </th>
    </tr>
    <tr>
        <td class="label" style="width: 20%;vertical-align:middle;">Descrizione:</td>
        <td style="width: 80%;vertical-align:middle;"><%=CurrentResolution.OtherDescription %></td>
    </tr>
</table>