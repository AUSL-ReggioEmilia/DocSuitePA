<%@ Page Language="vb" AutoEventWireup="false" SmartNavigation="False" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmInserimento"
    CodeBehind="DocmInserimento.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratiche - Inserimento" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="ContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelText.ascx" TagName="ContattiSelText" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="Oggetto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="Classificatore" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsb">
        <script language="javascript" type="text/javascript">
            function DisplayControl(control, display) {
                var tbl = document.getElementById(control);
                if (tbl != null) {
                    tbl.style.display = display;
                }
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerDocm" runat="server">
        <Windows>
            <telerik:RadWindow Height="500" ID="windowSelContact" ReloadOnShow="false" runat="server" Title="Selezione Contatto" Width="700" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <%-- Sezione Classificatore --%>
    <table class="datatable">
        <tr>
            <th colspan="2">Contenitore</th>
        </tr>
        <tr>
            <td style="width: 150px"></td>
            <td>
                <asp:DropDownList AppendDataBoundItems="true" AutoPostBack="True" ID="ddlContainer" runat="server">
                    <asp:ListItem Text="" Value="" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="ddlContainer" Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio" ID="RequiredFieldValidator1" runat="server" />
            </td>
        </tr>
    </table>
    <%-- Sezione Settore --%>
    <usc:Settori Caption="Settore" HeaderVisible="True" ID="uscSettori" MultipleRoles="False" MultiSelect="false" Required="true" RequiredMessage="Campo Settori Obbligatorio" RoleRestictions="OnlyMine" runat="server" Type="Docm" />
    <%-- Sezione Riferimento --%>
    <table class="datatable" runat="server" id="tblInterop" style="display: table !important;">
        <tr>
            <th>Riferimento</th>
        </tr>
        <tr>
            <td>
                <usc:ContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" EnableCC="false" ForceAddressBook="True" HeaderVisible="false" ID="uscContatti" IsRequired="true" Multiple="true" MultiSelect="true" RequiredErrorMessage="Campo Riferimento Obbligatorio" runat="server" TreeViewCaption="Riferimento" Type="Docm" />
            </td>
        </tr>
    </table>
    <%-- Sezione dati pratica --%>
    <table class="datatable">
        <tr>
            <th colspan="2">Dati Pratica</th>
        </tr>
        <tr class="Chiaro">
            <td style="width: 150px" class="label">Numero Posizione:</td>
            <td>
                <telerik:RadTextBox ID="ServiceNumber" MaxLength="50" runat="server" Width="100%" />
            </td>
        </tr>
        <tr class="Chiaro">
            <td style="width: 150px" class="label">Nome Pratica:</td>
            <td>
                <telerik:RadTextBox ID="Name" MaxLength="255" runat="server" Width="100%" />
            </td>
        </tr>
        <tr class="Chiaro">
            <td style="width: 150px" class="label">Oggetto:</td>
            <td>
                <usc:Oggetto runat="server" ID="uscOggetto" EditMode="true" MultiLine="false" MaxLength="255" Required="true" Type="Docm" />
            </td>
        </tr>
        <tr class="Chiaro">
            <td style="width: 150px" class="label">Responsabile:</td>
            <td class="noPadding">
                <usc:ContattiSelText ForceAddressBook="True" ID="uscResponsabile" MaxLunghezzaTesto="1000" runat="server" Type="Docm" />
            </td>
        </tr>
        <tr class="Chiaro">
            <td style="width: 150px" class="label">Note:</td>
            <td>
                <telerik:RadTextBox ID="Note" MaxLength="255" runat="server" Width="100%" />
            </td>
        </tr>
        <tr class="Chiaro">
            <td style="width: 150px" class="label">Data inizio:</td>
            <td>
                <telerik:RadDatePicker ID="rdpDataInizio" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="rdpDataInizio" Display="Dynamic" ErrorMessage="Data inizio obbligatoria" ID="rfvDataInizio" runat="server" />
            </td>
        </tr>
        <tr class="Chiaro">
            <td style="width: 150px" class="label">Data scadenza:</td>
            <td style="padding-bottom: 5px;">
                <telerik:RadDatePicker ID="rdpDataScadenza" runat="server" />
            </td>
        </tr>
    </table>
    <%-- Sezione classificatore --%>
    <usc:Classificatore runat="server" ID="uscClassificatore" Caption="Classificazione" Type="Docm" Required="true" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnInserimento" runat="server" Text="Conferma Inserimento" />
    <asp:ValidationSummary DisplayMode="List" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
</asp:Content>
