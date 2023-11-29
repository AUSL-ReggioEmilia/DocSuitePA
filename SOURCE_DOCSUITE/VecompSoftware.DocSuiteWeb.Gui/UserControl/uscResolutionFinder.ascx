<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionFinder.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionFinder" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelText.ascx" TagName="uscContattiSelText" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="usc" %>

<script type="text/javascript">
    function VisibleCategorySearch() {
        // nascondi riga per ricerca nei sottocontatti
        var row = document.getElementById("<%= trCategoryExt.ClientID %>");
        row.style.display = "";
    }

    function HideCategorySearch() {
        // nascondi riga per ricerca nei sottocontatti
        var row = document.getElementById("<%= trCategoryExt.ClientID %>");
        row.style.display = "none";
    }
</script>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        Telerik.Web.UI.RadDateInput.prototype.get_selectedDate = function () {
            if (!this._calledAtleastOnce) {
                //update value when it is filled by browser's autofill on back button.
                this._calledAtleastOnce = true;
                this._updateHiddenValue();
            }
            return this._value ? new Date(this._value) : null;
        }
    </script>
</telerik:RadScriptBlock>

<table class="dataform">
    <%--Tipologia--%>
    <tr id="trType" runat="server">
        <td class="label" style="width: 30%;">Tipologia:
        </td>
        <td class="DXChiaro">
            <asp:CheckBox Checked="True" ID="Delibera" runat="server" Text="Delibera" />&nbsp;
            <asp:CheckBox Checked="True" ID="Determina" runat="server" Text="Atto - Disposizione" />
        </td>
    </tr>
    <asp:Panel ID="pnChecked" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 30%;">Tipologia Delibera:
            </td>
            <td class="DXChiaro">
                <asp:RadioButtonList RepeatDirection="Horizontal" ID="rblChecked" runat="server">
                    <asp:ListItem Text="Non soggetta a controllo" Value="0" />
                    <asp:ListItem Text="Soggetta a controllo" Value="1" />
                    <asp:ListItem Selected="true" Text="Tutte" Value="2" />
                </asp:RadioButtonList>
            </td>
        </tr>
    </asp:Panel>
    <%--Anno di adozione--%>
    <tr id="trAdoptionYear" runat="server">
        <td class="label" style="width: 30%;">Anno di adozione:
        </td>
        <td class="DXChiaro">
            <telerik:RadNumericTextBox ID="txtYear" MinValue="1900" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" runat="server" Width="60px" />
            <asp:RegularExpressionValidator ControlToValidate="txtYear" ErrorMessage="Errore formato" ID="vYear" runat="server" ValidationExpression="\d{4}" />
        </td>
    </tr>
    <%--Numero AUSL-PC--%>
    <tr id="trAUSLPCNumber" runat="server" visible="false">
        <td class="label" style="width: 30%;">Numero:
        </td>
        <td class="DXChiaro">
            <asp:TextBox ID="AUSLPCNumber" MaxLength="255" runat="server" Width="96px" />
        </td>
    </tr>
    <%--Numero di servizio--%>
    <tr id="trServiceNumber" runat="server">
        <td class="label" style="width: 30%;">Numero di servizio:
        </td>
        <td class="DXChiaro">
            <asp:TextBox ID="ServiceNumber" MaxLength="255" runat="server" Width="96px" />
        </td>
    </tr>
    <%--Numero--%>
    <tr id="trNumber" runat="server">
        <td class="label" style="width: 30%;">Numero:
        </td>
        <td class="DXChiaro">
            <asp:TextBox ID="Number" MaxLength="7" runat="server" Width="96px" />
            <asp:RegularExpressionValidator ControlToValidate="Number" ErrorMessage="Errore formato" ID="vNumber" runat="server" ValidationExpression="\d*" />
        </td>
    </tr>
    <%--Numero provvisorio--%>
    <tr id="trIdResolution" runat="server">
        <td class="label" style="width: 30%;">Numero provvisorio:
        </td>
        <td class="DXChiaro">
            <asp:TextBox ID="idResolution" MaxLength="7" runat="server" Width="96px" />
            <asp:RegularExpressionValidator ControlToValidate="idResolution" ErrorMessage="Errore formato" ID="vIdResolution" runat="server" ValidationExpression="\d*" />
        </td>
    </tr>
    <%--Immediatamente esecutiva--%>
    <tr id="trImmediatelyExecutive" runat="server" visible="false">
        <td class="label" style="width: 30%;">&nbsp;
        </td>
        <td class="DXChiaro">
            <asp:CheckBox Checked="false" ID="chkImmediatelyExecutive" runat="server" Text="Immediatamente Esecutiva" TextAlign="Right" />
        </td>
    </tr>
    <%--Data Proposta--%>
    <tr id="trProposerDate" runat="server" visible="false" style="vertical-align: middle;">
        <td class="label" style="width: 30%;">Data proposta:
        </td>
        <td class="DXChiaro">
            <telerik:RadDatePicker ID="DateProposerFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
            <telerik:RadDatePicker ID="DateProposerTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
        </td>
    </tr>
    <%--Data Adozione--%>
    <tr id="trAdoptionDate" runat="server" style="vertical-align: middle;" visible="false">
        <td class="label" style="width: 30%;">Data adozione:
        </td>
        <td class="DXChiaro">
            <telerik:RadDatePicker ID="DateAdoptionFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
            <telerik:RadDatePicker ID="DateAdoptionTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
        </td>
    </tr>
    <%--Stato contabilità--%>
    <tr id="trStatoContabilita" runat="server" style="vertical-align: middle;" visible="false">
        <td class="label" style="width: 30%;">Stato contabilità:
        </td>
        <td class="DXChiaro">
            <asp:DropDownList ID="ddlBidType" runat="server" />
        </td>
    </tr>
    <%--Organi di Controllo--%>
    <tr id="trOCList" runat="server" visible="false">
        <td class="label" style="width: 30%; vertical-align: top;">Organo di controllo:
        </td>
        <td class="DXChiaro">
            <asp:Panel ID="pnlSupervisoryBoard" runat="server">
                <asp:CheckBox ID="chkSupervisoryBoard" runat="server" Text="Collegio sindacale" /><br />
            </asp:Panel>
            <asp:Panel ID="pnlConfSind" runat="server" Visible="false">
                <asp:CheckBox ID="chkConfSind" runat="server" Text="Controllo conferenza dei sindaci" /><br />
            </asp:Panel>
            <asp:Panel ID="pnlRegion" runat="server">
                <asp:CheckBox ID="chkRegion" runat="server" Text="Regione" /><br />
            </asp:Panel>
            <asp:Panel ID="pnlManagement" runat="server">
                <asp:CheckBox ID="chkManagement" runat="server" Text="Controllo di gestione" /><br />
            </asp:Panel>
            <asp:Panel ID="pnlCorteConti" runat="server">
                <asp:CheckBox ID="chkCorteConti" runat="server" Text="Corte dei conti" /><br />
            </asp:Panel>
            <asp:Panel ID="pnlOther" runat="server">
                <asp:CheckBox ID="chkOther" runat="server" Text="Altro" />
            </asp:Panel>
        </td>
    </tr>
    <%--Passo del Flusso--%>
    <tr id="trWorkflow" runat="server">
        <td class="label" style="width: 30%;">Passo del flusso:
        </td>
        <td class="DXChiaro">

            <asp:CheckBox ID="Proposta" runat="server" Text="Proposta" Width="90px" AutoPostBack="true" />&nbsp; 
            <asp:CheckBox ID="Adottata" runat="server" Text="Adottata" Width="90px" AutoPostBack="true" />&nbsp; 
            <asp:CheckBox ID="Pubblicata" runat="server" Text="Pubblicata" Width="90px" AutoPostBack="true" />&nbsp; 
            <asp:CheckBox ID="Esecutiva" runat="server" Text="Esecutiva" Width="90px" AutoPostBack="true" />

        </td>
    </tr>

    <tr id="trWorkflowSearchableSteps" runat="server" visible="false">
        <td class="label" style="width: 30%; vertical-align: top;">Passo del flusso:
        </td>
        <td class="DXChiaro">
            <asp:CheckBoxList ID="CheckBoxListWorkflowSteps" runat="server" AutoPostBack="true" />
        </td>
    </tr>
    <%--Organo di Controllo--%>
    <tr id="trOC" runat="server">
        <td class="label" style="width: 30%;">Organo di controllo:
        </td>
        <td class="DXChiaro">

            <asp:CheckBox ID="Spedizione" runat="server" Text="Spedizione" Width="90px" AutoPostBack="true" />&nbsp; 
            <asp:CheckBox ID="Ricezione" runat="server" Text="Ricezione" Width="90px" AutoPostBack="true" />&nbsp; 
            <asp:CheckBox ID="Scadenza" runat="server" Text="Scadenza" Width="90px" AutoPostBack="true" />&nbsp; 
            <asp:CheckBox ID="Risposta" runat="server" Text="Risposta" Width="90px" AutoPostBack="true" />

        </td>
    </tr>
    <%--Limite Passo--%>
    <tr id="trActiveStep" runat="server">
        <td class="label" style="width: 30%;">&nbsp;
        </td>
        <td class="DXChiaro">
            <asp:CheckBox ID="StepAttivo" runat="server" Text="Limita ricerca alle registrazioni con stato uguale al passo selezionato" Width="100%" />
        </td>
    </tr>
    <%--Data Da A--%>
    <tr id="trStepDate" runat="server" style="vertical-align: middle;">
        <td class="label" style="width: 30%;">Data:
        </td>
        <td class="DXChiaro">
            <asp:Panel ID="pnlDateRange" runat="server">
                <telerik:RadDatePicker ID="DateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server"  />
                <telerik:RadDatePicker ID="DateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </asp:Panel>
        </td>
    </tr>
    <%--Contenitore--%>
    <tr id="trContainer" runat="server">
        <td class="label" style="width: 30%;">Contenitore:
        </td>
        <td class="DXChiaro">
            <asp:DropDownList ID="ddlContainer" runat="server" AppendDataBoundItems="true">
                <asp:ListItem />
            </asp:DropDownList>
        </td>
    </tr>
    <%--OGGETTO--%>
    <tr id="trObject" runat="server">
        <td class="label" style="width: 30%;">Oggetto:
        </td>
        <td class="DXChiaro">
            <asp:TextBox ID="txtOggetto" runat="server" Width="300px" MaxLength="511" />
        </td>
    </tr>
    <tr id="trObjectExt" runat="server">
        <td class="label" style="width: 30%;">&nbsp;
        </td>
        <td class="DXChiaro">
            <asp:RadioButtonList ID="rblClausola" RepeatDirection="Horizontal" runat="server" Width="300px">
                <asp:ListItem Selected="True" Text="Tutte le parole." Value="AND" />
                <asp:ListItem Text="Almeno una parola." Value="OR" />
            </asp:RadioButtonList>
        </td>
    </tr>
    <%--Note--%>
    <tr id="trNote" runat="server">
        <td class="label" style="width: 30%;">Note:
        </td>
        <td class="DXChiaro">
            <asp:TextBox ID="Note" MaxLength="255" runat="server" Width="300px" />
        </td>
    </tr>
    <%--Destinatari--%>
    <tr id="trDestContact" runat="server">
        <td class="label" style="width: 30%; vertical-align: middle;">Destinatari:
        </td>
        <td class="DXChiaro">
            <usc:uscContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" HeaderVisible="false" ID="uscDestInterop" IsRequired="false" MultiSelect="false" runat="server" TreeViewCaption="Destinatari" Type="Resl" />
            <usc:uscContattiSelText ID="Recipient" IsMittDest="true" MaxLunghezzaTesto="1000" runat="server" TextBoxWidth="300px" Type="Resl" />
        </td>
    </tr>
    <%--Proponente--%>
    <tr id="trPropContact" runat="server">
        <td class="label" style="width: 30%; vertical-align: middle;">Proponente:
        </td>
        <td class="DXChiaro">
            <usc:uscContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" HeaderVisible="false" ID="uscPropInterop" IsRequired="false" MultiSelect="false" runat="server" TreeViewCaption="Proponente" Type="Resl" />
            <usc:uscContattiSelText ID="Proposer" IsMittDest="true" MaxLunghezzaTesto="1000" runat="server" TextBoxWidth="300px" Type="Resl" />
        </td>
    </tr>
    <%--Assegnatario--%>
    <tr id="trAssContact" runat="server">
        <td class="label" style="width: 30%; vertical-align: middle;">Assegnatario:
        </td>
        <td class="DXChiaro">
            <usc:uscContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" HeaderVisible="false" ID="uscAssiInterop" IsRequired="false" MultiSelect="false" runat="server" TreeViewCaption="Assegnatario" Type="Resl" />
            <usc:uscContattiSelText ID="Assignee" IsMittDest="true" MaxLunghezzaTesto="1000" runat="server" TextBoxWidth="300px" Type="Resl" />
        </td>
    </tr>
    <%--Responsabile--%>
    <tr id="trMgrContact" runat="server">
        <td class="label" style="width: 30%; vertical-align: middle;">Responsabile:
        </td>
        <td class="DXChiaro">
            <usc:uscContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" HeaderVisible="false" ID="uscRespInterop" IsRequired="false" MultiSelect="false" runat="server" TreeViewCaption="Responsabile" Type="Resl" />
            <usc:uscContattiSelText ID="Manager" IsMittDest="true" MaxLunghezzaTesto="1000" runat="server" TextBoxWidth="300px" Type="Resl" />
        </td>
    </tr>
    <%--Commento OC--%>
    <tr id="trOCComment" runat="server">
        <td class="label" style="width: 30%;">Commento OC:
        </td>
        <td class="DXChiaro">
            <asp:DropDownList ID="ddlControllerStatus" runat="server" />
        </td>
    </tr>
    <%--Parere OC--%>
    <tr id="trOCOpinion" runat="server">
        <td class="label" style="width: 30%;">Parere OC:
        </td>
        <td class="DXChiaro">
            <asp:TextBox ID="ControllerOpinion" MaxLength="255" runat="server" Width="300px" />
        </td>
    </tr>
    <%--Classificazione--%>
    <tr id="trCategory" runat="server">
        <td class="label" style="width: 30%;">Classificazione:
        </td>
        <td class="DXChiaro">
            <usc:uscClassificatore HeaderVisible="false" ID="uscCategory" Action="Search" OnlyActive="false" Required="false" runat="server" Type="Resl" />
        </td>
    </tr>
    <tr id="trCategoryExt" runat="server">
        <td class="label" style="width: 30%;"></td>
        <td class="DXChiaro">
            <asp:CheckBox ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle Sottocategorie" />
        </td>
    </tr>
    <%--Atti Annullati--%>
    <tr id="trStatusCancel" runat="server" visible="false">
        <td class="label" style="width: 30%;">Includi Atti Annullati:
        </td>
        <td class="DXChiaro">
            <asp:CheckBox ID="chkStatusCancel" runat="server" />
        </td>
    </tr>
    <tr id="trOnlyStatusCancel" runat="server" visible="false">
        <td class="label" style="width: 30%;">Esclusivamente Atti Annullati:
        </td>
        <td class="DXChiaro">
            <asp:CheckBox ID="chkOnlyStatusCancel" runat="server" />
        </td>
    </tr>
    <%--Atti con pubblicazione privacy--%>
    <tr id="trPrivacyPublication" runat="server" visible="false">
        <td class="label" style="width: 30%;">Pubblicazione Privacy:
        </td>
        <td class="DXChiaro" style="height: 24px">
            <asp:RadioButtonList ID="rblPrivacyPublication" runat="server" Width="300px" RepeatDirection="Vertical">
                <asp:ListItem Selected="True" Text="Tutte le pubblicazioni" Value="" />
                <asp:ListItem Text="Pubblicazioni con privacy" Value="privacy" />
                <asp:ListItem Text="Pubblicazioni senza privacy" Value="noprivacy" />
            </asp:RadioButtonList>
        </td>
    </tr>
</table>
