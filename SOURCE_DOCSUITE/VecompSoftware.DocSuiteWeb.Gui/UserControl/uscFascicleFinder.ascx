<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicleFinder.ascx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicleFinder" %>

<%@ Register Src="uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="uc3" %>
<%@ Register Src="uscMetadataRepositorySel.ascx" TagName="uscMetadataRepositorySel" TagPrefix="usc" %>
<%@ Register Src="uscSettori.ascx" TagName="uscSettori" TagPrefix="uc2" %>
<%@ Register Src="uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>

<telerik:RadAjaxManagerProxy runat="server" ID="ctrlAjaxManager">
</telerik:RadAjaxManagerProxy>

<telerik:RadScriptBlock runat="server" ID="rsb" EnableViewState="false">
    <script type="text/javascript">     

        function VisibleCategorySearch() {
            // nascondi riga per ricerca nei sottocontatti
            var row = document.getElementById("<%= rowCategorySearch.ClientID %>");
            row.style.display = "";
        }
    </script>
</telerik:RadScriptBlock>

<table class="datatable">
    <tr>
        <th colspan="2">Fascicoli</th>
    </tr>
    <%-- YEAR --%>
    <tr>
        <td class="label col-dsw-3">Anno:</td>
        <td class="col-dsw-7">
            <asp:TextBox ID="txtYear" runat="server" MaxLength="4" Width="56px" />
            <asp:RegularExpressionValidator runat="server" ID="vYear" ErrorMessage="Errore formato"
                ControlToValidate="txtYear" ValidationExpression="\d{4}" />
        </td>
    </tr>
    <tr>
        <td class="label col-dsw-3">Numero:</td>
        <td class="col-dsw-7">
            <asp:TextBox ID="txtNumber" runat="server" MaxLength="255" Width="300px"/>
        </td>
    </tr>
    <tr>
        <td class="label col-dsw-3">Stato:</td>
        <td class="col-dsw-7">
            <asp:RadioButtonList ID="rblFascicleState" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="All">Tutti</asp:ListItem>
                <asp:ListItem Value="Open" Selected="True">Aperto</asp:ListItem>
                <asp:ListItem Value="Closed">Chiuso</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <%-- VISIBILITA' --%>
    <tr id="trVisibility" runat="server">
        <td class="label" style="width: 30%;">Tipo di autorizzazione:
        </td>
        <td class="DXChiaro">
            <asp:CheckBox ID="Accessible" runat="server" Text="Documenti disponibili ai settori autorizzati" Width="340px" AutoPostBack="true" />&nbsp; 
            <asp:CheckBox ID="Confidential" runat="server" Text="Riservato" Width="90px" AutoPostBack="true" Visible="false" />&nbsp; 
        </td>
    </tr>

    <%-- DATA APERTURA DA A --%>
    <tr class="dsw-vertical-bottom">
        <td class="label dsw-vertical-middle" style="margin-left: -2px;">Data Apertura:</td>
        <td class="dsw-vertical-middle">
            <telerik:RadDatePicker ID="txtStartDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
            <telerik:RadDatePicker ID="txtStartDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
        </td>
    </tr>
    <%-- DATA CHIUSURA DA A --%>
    <tr class="dsw-vertical-bottom">
        <td class="label dsw-vertical-middle" style="margin-left: -2px;">Data Chiusura:</td>
        <td class="dsw-vertical-middle">
            <telerik:RadDatePicker ID="txtEndDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
            <telerik:RadDatePicker ID="txtEndDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
        </td>
    </tr>
    <!-- OGGETTO -->
    <tr>
        <td class="label">Oggetto:</td>
        <td>
            <asp:TextBox ID="txtObjectProtocol" runat="server" MaxLength="255" Width="300px" /></td>
    </tr>
    <tr>
        <td class="label">&nbsp;</td>
        <td>
            <asp:RadioButtonList ID="rblClausola" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="AND" Selected="True">Tutte le parole.</asp:ListItem>
                <asp:ListItem Value="OR">Almeno una parola.</asp:ListItem>
            </asp:RadioButtonList></td>
    </tr>
    <!-- NOTE -->
    <tr>
        <td class="label">Note:</td>
        <td>
            <asp:TextBox ID="txtNote" runat="server" MaxLength="255" Width="300px"></asp:TextBox></td>
    </tr>
    <tr runat="server" id="rowContainer">
        <td class="label">Contenitore:</td>
        <td style="width: 500px;">
            <telerik:RadDropDownList runat="server" ID="rdlContainers" Width="300px" DataTextField="Name" DataValueField="Id" DropDownHeight="300px">
                <Items>
                    <telerik:DropDownListItem Text="" Value="" />
                </Items>
            </telerik:RadDropDownList>
        </td>
    </tr>
    <!-- CLASSIFICAZIONE -->
    <tr>
        <td class="label">Classificazione:</td>
        <td style="width: 500px;">
            <usc:uscCategoryRest HeaderVisible="false" ID="uscCategoryRest" ShowAuthorizedFascicolable="false" IsRequired="false" runat="server" />
        </td>
    </tr>
    <tr id="rowCategorySearch" runat="server">
        <td class="label">&nbsp;</td>
        <td>
            <asp:CheckBox ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle sottocategorie" />
        </td>
    </tr>
    <tr id="settResp" runat="server">
        <td class="label">Settore responsabile:
        </td>
        <td style="width: 100%">
            <uc2:uscSettori HeaderVisible="false" ID="uscSettoriResp" MultiSelect="false" Required="false" runat="server" />
        </td>
    </tr>
    <tr id="sett" runat="server">
        <td class="label">Settore autorizzato:
        </td>
        <td style="width: 100%">
            <uc2:uscSettori HeaderVisible="false" ID="uscSettori" MultiSelect="false" Required="false" runat="server" />
        </td>
    </tr>    
</table>
<table class="datatable">
    <tr>
        <th colspan="2"><asp:Label ID="lblRP" runat="server"/>
        </th>
    </tr>
    <!-- RIFERIMENTO -->
    <tr id="rowInterop" runat="server">
        <td class="label col-dsw-3">Filtra da rubrica:</td>
        <td class="col-dsw-7">
            <uc3:uscContattiSel ID="UscRiferimentoSel1" runat="server" HeaderVisible="false"
                IsRequired="false" Type="Prot" MultiSelect="false" ButtonImportVisible="false"
                ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonSelectOChartVisible="false" ButtonSelectDomainVisible="false" TreeViewCaption="Riferimento" ForceAddressBook="true" FascicleContactEnabled="true" ExcludeRoleRoot="True" />
        </td>
    </tr>
</table>
<table class="datatable" id="pnlMetadata" runat="server">
    <tr>
        <th colspan="2">Metadati
        </th>
    </tr>
    <tr>
        <td class="label col-dsw-3" style="vertical-align: top;">Tipologia metadati:</td>
        <td>
            <usc:uscMetadataRepositorySel runat="server" 
                ID="uscMetadataRepositorySel" 
                Caption="Metadati dinamici" 
                EnableAdvancedMetadataSearch="true" 
                Required="False" 
                UseSessionStorage="true" />
        </td>
    </tr>
</table>

