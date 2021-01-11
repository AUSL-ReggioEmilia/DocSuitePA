<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DossierRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierRicerca" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Dossier - Ricerca" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositorySel.ascx" TagName="uscMetadataRepositorySel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var dossierRicerca;
            require(["Dossiers/DossierRicerca"], function (DossierRicerca) {
                $(function () {
                    dossierRicerca = new DossierRicerca(tenantModelConfiguration.serviceConfiguration);
                    dossierRicerca.btnSearchId = "<%= btnSearch.ClientID%>";
                    dossierRicerca.txtYearId = "<%= txtYear.ClientID%>";
                    dossierRicerca.txtNumberId = "<%= txtNumber.ClientID%>";
                    dossierRicerca.txtSubjectId = "<%= txtSubject.ClientID%>";
                    dossierRicerca.rdlContainerId = "<%= rdlContainer.ClientID%>";
                    dossierRicerca.rdpStartDateFromId = "<%= rdpStartDateFrom.ClientID%>";
                    dossierRicerca.rdpStartDateToId = "<%= rdpStartDateTo.ClientID%>";
                    dossierRicerca.rdpEndDateFromId = "<%= rdpEndDateFrom.ClientID%>";
                    dossierRicerca.rdpEndDateToId = "<%= rdpEndDateTo.ClientID%>";
                    dossierRicerca.txtNoteId = "<%= txtNote.ClientID%>";
                    dossierRicerca.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierRicerca.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierRicerca.searchTableId = "<%= searchTable.ClientID %>";
                    dossierRicerca.btnCleanId = "<%= btnClean.ClientID %>";
                    dossierRicerca.hasTxtYearDefaultValue = false;
                    dossierRicerca.metadataRepositoryEnabled = <%=ProtocolEnv.MetadataRepositoryEnabled.ToString().ToLower()%>;
                    dossierRicerca.rowMetadataRepositoryId = "<%= rowMetadataRepository.ClientID%>";
                    dossierRicerca.uscMetadataRepositorySelId = "<%= uscMetadataRepositorySel.PageContentDiv.ClientID%>";
                    dossierRicerca.currentTenantId = "<%= CurrentTenant.UniqueId %>";
                    dossierRicerca.metadataTableId = "<%= metadataTable.ClientID %>";
                    dossierRicerca.isWindowPopupEnable = <%= IsWindowPopupEnable.ToString().ToLower() %>;
                    dossierRicerca.uscCategoryRestId = "<%= uscCategoryRest.MainContent.ClientID %>";
                    dossierRicerca.rcbDossierTypeId = "<%= rcbDossierType.ClientID %>";
                    dossierRicerca.rblDossierStatusId = "<%= rblDossierStatus.ClientID %>";
                    dossierRicerca.dossierStatusEnabled = <%= DossierStatusEnabled.ToString().ToLower() %>;
                    dossierRicerca.dossierStatusRowId = "<%= dossierStatusRow.ClientID %>";
                    dossierRicerca.initialize();
                });
            });

            $(document).keypress(function (e) {
                if (e.which == 13) {
                    document.getElementById(dossierRicerca.btnSearchId).click();
                    e.preventDefault();
                }
            });
        </script>
    </telerik:RadScriptBlock>

    <table class="datatable" id="searchTable" runat="server">
        <tr>
            <th colspan="2">Dossier</th>
        </tr>
        <tr>
            <td class="label col-dsw-3">Anno:</td>
            <td class="col-dsw-7">
                <telerik:RadTextBox ID="txtYear" runat="server" MaxLength="4" Width="56px" />
                <asp:RegularExpressionValidator runat="server" ID="vYear" ErrorMessage="Errore formato"
                    ControlToValidate="txtYear" ValidationExpression="\d{4}" />
            </td>
        </tr>

        <tr>
            <td class="label col-dsw-3">Numero:</td>
            <td class="col-dsw-7">
                <telerik:RadNumericTextBox ID="txtNumber" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="7" Width="96px" runat="server" />
            </td>
        </tr>

        <tr>
            <td class="label col-dsw-3">Tipologia:</td>
            <td class="col-dsw-7">
                <telerik:RadComboBox ID="rcbDossierType" runat="server" />
            </td>
        </tr>

        <tr id="dossierStatusRow" runat="server">
            <td class="label col-dsw-3">Stato:</td>
            <td class="col-dsw-7">
                <asp:RadioButtonList ID="rblDossierStatus" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="All">Tutti</asp:ListItem>
                    <asp:ListItem Value="Open" Selected="True">Aperti</asp:ListItem>
                    <asp:ListItem Value="Canceled">Annullati</asp:ListItem>
                    <asp:ListItem Value="Closed">Chiusi</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>

        <tr class="dsw-vertical-bottom">
            <td class="label dsw-vertical-middle" style="margin-left: -2px;">Data Apertura:</td>
            <td class="dsw-vertical-middle">
                <telerik:RadDatePicker ID="rdpStartDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <telerik:RadDatePicker ID="rdpStartDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </td>
        </tr>

        <tr class="dsw-vertical-bottom">
            <td class="label dsw-vertical-middle" style="margin-left: -2px;">Data Chiusura:</td>
            <td class="dsw-vertical-middle">
                <telerik:RadDatePicker ID="rdpEndDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <telerik:RadDatePicker ID="rdpEndDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </td>
        </tr>

        <tr>
            <td class="label">Oggetto:</td>
            <td class="col-dsw-7">
                <telerik:RadTextBox ID="txtSubject" MaxLength="255" runat="server" Width="300px" />
            </td>
        </tr>
        <tr>
            <td class="label">&nbsp;</td>
            <td>
                <asp:RadioButtonList ID="rblSubjectFilter" runat="server" RepeatDirection="Horizontal" Enabled="False">
                    <asp:ListItem Selected="True">Tutte le parole.</asp:ListItem>
                    <asp:ListItem>Almeno una parola.</asp:ListItem>
                </asp:RadioButtonList></td>
        </tr>

        <tr>
            <td class="label">Note:</td>
            <td>
                <telerik:RadTextBox ID="txtNote" runat="server" MaxLength="255" Width="300px"></telerik:RadTextBox></td>
        </tr>

        <tr>
            <td class="label">Contenitore:</td>
            <td>
                <telerik:RadDropDownList runat="server" ID="rdlContainer" Width="300px" AutoPostBack="False" selected="true" DropDownHeight="200px">
                    <Items>
                        <telerik:DropDownListItem Text="" Value="" />
                    </Items>
                </telerik:RadDropDownList>
            </td>
        </tr>

        <tr>
            <td class="label">Classificatore:</td>
            <td>
                <usc:uscCategoryRest runat="server" ID="uscCategoryRest" IsRequired="false" />
            </td>
        </tr>
    </table>
    <table class="datatable" id="metadataTable" runat="server">
        <tr>
            <th colspan="2">Metadati</th>
        </tr>
        <tr id="rowMetadataRepository">
            <td class="label col-dsw-3" style="vertical-align: top;">Tipologia metadati:</td>
            <td class="col-dsw-7">
                <usc:uscMetadataRepositorySel runat="server" 
                    ID="uscMetadataRepositorySel" 
                    EnableAdvancedMetadataSearch="true" 
                    Caption="Metadati dinamici" 
                    Required="False" 
                    UseSessionStorage="true" />
            </td>
        </tr>
    </table>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>


<asp:Content ID="footerContent" runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnSearch" Text="Ricerca" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
    <telerik:RadButton ID="btnClean" Text="Svuota ricerca" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
</asp:Content>
