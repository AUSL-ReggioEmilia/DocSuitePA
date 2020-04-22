<%@ Control AutoEventWireup="false" Codebehind="uscDocumentFinder.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentFinder" Language="vb" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc2" %>
<%@ Register Src="uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="uc3" %>

<telerik:RadScriptBlock runat="server" ID="rsb" EnableViewState="false">
    <script language="javascript" type="text/javascript">        
        function VisibleContactChild() {
            // visualizza riga per ricerca nei sottocontatti
            var row = document.getElementById("<%= trContactChild.ClientID %>");
            row.style.display = "";
        }

        function HideContactChild() {
            // nascondi riga per ricerca nei sottocontatti
            var row = document.getElementById("<%= trContactChild.ClientID %>");
            row.style.display = "none";
        }

        function VisibleCategoryChild() {
            // nascondi riga per ricerca nei sottocontatti
            var row = document.getElementById("<%= trCategoryChild.ClientID %>");
            row.style.display = "";
        }

        function HideCategoryChild() {
            // nascondi riga per ricerca nei sottocontatti
            var row = document.getElementById("<%= trCategoryChild.ClientID %>");
            row.style.display = "none";
        }
    </script>
</telerik:RadScriptBlock>
<div id="divContent">
    <table class="dataform">
        <thead>
            <tr>
                <th class="datatable tHead" colspan="2">
                    Pratica
                </th>
            </tr>
        </thead>
        <tbody>
        <tr runat="server" id="trYear">
            <td class="label" style="width: 30%;">
                Anno:
            </td>
            <td style="width: 70%;">
                <telerik:RadNumericTextBox ID="txtYear" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" Width="56px" runat="server" />
            </td>
        </tr>
        <tr runat="server" id="trNumber">
            <td class="label">
                Numero:
            </td>
            <td>
                <telerik:RadNumericTextBox ID="txtNumber" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="7" Width="96px" runat="server" />
            </td>
        </tr>
        <tr runat="server" id="trStatus">
            <td class="label">
                Stato:
            </td>
            <td>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Selected="True" Value="0">Tutte</asp:ListItem>
                    <asp:ListItem Value="1">Aperte</asp:ListItem>
                    <asp:ListItem Value="2">Chiuse</asp:ListItem>
                    <asp:ListItem Value="3">Annullate</asp:ListItem>
                    <asp:ListItem Value="4">Archiviate</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr runat="server" id="trStartDate" style="vertical-align: middle;">
            <td class="label">Data Inizio:
            </td>
            <td>
                <telerik:RadDatePicker ID="dtStartDate_From" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <telerik:RadDatePicker ID="dtStartDate_To" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </td>
        </tr>
        <tr runat="server" id="trExpiryDate" style="vertical-align: middle;">
            <td class="label">Data scadenza:
            </td>
            <td>
                <telerik:RadDatePicker ID="dtExpiryDate_From" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <telerik:RadDatePicker ID="dtExpiryDate_To" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </td>
        </tr>
        <tr runat="server" id="trServiceNumber">
            <td class="label">Numero Posizione:
            </td>
            <td>
                <asp:TextBox ID="txtServiceNumber" MaxLength="50" runat="server" Width="300px" />
            </td>
        </tr>
        <tr runat="server" id="trName">
            <td class="label">Nome:
            </td>
            <td>
                <asp:TextBox ID="txtName" MaxLength="255" runat="server" Width="300px" />
            </td>
        </tr>
        <tr runat="server" id="trObjectD">
            <td class="label" style="vertical-align: top">Oggetto:
            </td>
            <td>
                <asp:TextBox ID="txtObjectD" MaxLength="255" runat="server" Width="300px" />
                <asp:RadioButtonList ID="rblClausola" RepeatDirection="Horizontal" runat="server" Width="300px">
                    <asp:ListItem Value="AND" Selected="True">Tutte le parole.</asp:ListItem>
                    <asp:ListItem Value="OR">Almeno una parola.</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr runat="server" id="trManager">
            <td class="label">Responsabile:
            </td>
            <td>
                <asp:TextBox ID="txtManager" MaxLength="50" runat="server" Width="300px" />
            </td>
        </tr>
        <tr runat="server" id="trNote">
            <td class="label">Note:
            </td>
            <td>
                <asp:TextBox ID="txtNote" MaxLength="255" runat="server" Width="300px" />
            </td>
        </tr>
        <asp:Panel ID="pnlInterop" runat="server">
            <tr runat="server" id="trContact">
                <td class="label">Riferimento:
                </td>
                <td>
                    <uc3:uscContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ForceAddressBook="true" HeaderVisible="false" ID="uscContattiSel1" IsRequired="false" MultiSelect="false" runat="server" TreeViewCaption="Riferimento" Type="Docm" />
                </td>
            </tr>
            <asp:Panel ID="pnlInteropSearch" runat="server">
                <tr runat="server" id="trContactChild">
                    <td class="label">
                    </td>
                    <td>
                        <asp:CheckBox ID="chbContactChild" runat="server" Text="Estendi ricerca ai sottocontatti." />
                    </td>
                </tr>
            </asp:Panel>
        </asp:Panel>
        <tr runat="server" id="trContainer">
            <td class="label">Contenitore:
            </td>
            <td>
                <asp:DropDownList AppendDataBoundItems="true" ID="ddlIdContainer" runat="server">
                    <asp:ListItem />
                </asp:DropDownList>
            </td>
        </tr>
        <tr runat="server" id="trRoles">
            <td class="label">Settore Apertura:
            </td>
            <td>
                <uc1:uscSettori Caption="Autorizzazione" HeaderVisible="false" ID="uscSettore1" MultiSelect="false" Required="false" runat="server" ShowActive="True" />
            </td>
        </tr>
        <tr runat="server" id="trCategory">
            <td class="label">Classificazione:
            </td>
            <td>
                <uc2:uscClassificatore Caption="Autorizzazione" HeaderVisible="false" ID="uscClassificatore1" Multiple="true" Action="Search" OnlyActive="false" Required="false" runat="server" />
            </td>
        </tr>
        <asp:Panel ID="pnlCategorySearch" runat="server">
            <tr runat="server" id="trCategoryChild">
                <td class="label">
                </td>
                <td>
                    <asp:CheckBox ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle sottocategorie." />
                </td>
            </tr>
        </asp:Panel>
        </tbody>
    </table>
    <table class="dataform">
        <thead>
            <tr>
                <th class="datatable tHead" colspan="2">
                    Documento
                </th>
            </tr>
        </thead>
        <tbody>
        <tr runat="server" id="trDocumentDescription">
            <td class="label" style="width: 30%;">
                Documento:
            </td>
            <td style="width: 70%;">
                <asp:TextBox ID="txtDocumentDescription" MaxLength="255" runat="server" Width="300px" />
            </td>
        </tr>
        <tr runat="server" id="trDocumentDate" style="vertical-align: middle;">
            <td class="label">Data Documento:
            </td>
            <td>
                <telerik:RadDatePicker ID="dtDocumentDate_From" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <telerik:RadDatePicker ID="dtDocumentDate_To" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </td>
        </tr>
        <tr runat="server" id="trDocumentObject">
            <td class="label">Oggetto:
            </td>
            <td>
                <asp:TextBox ID="txtDocumentObject" MaxLength="255" runat="server" Width="300px" />
            </td>
        </tr>
        <tr runat="server" id="trDocumentReason">
            <td class="label">Motivo:
            </td>
            <td>
                <asp:TextBox ID="txtDocumentReason" MaxLength="255" runat="server" Width="300px" />
            </td>
        </tr>
        <tr runat="server" id="trDocumentNote">
            <td class="label">Note:
            </td>
            <td>
                <asp:TextBox ID="txtDocumentNote" MaxLength="255" runat="server" Width="300px" />
            </td>
        </tr>
        <tr runat="server" id="trDocumentRegDate" style="vertical-align: middle;">
            <td class="label">Data Inser.:
            </td>
            <td>
                <telerik:RadDatePicker ID="dtDocumentRegDate_From" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <telerik:RadDatePicker ID="dtDocumentRegDate_To" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </td>
        </tr>
        </tbody>
    </table>
</div>
<div id="divFooter">
    <asp:Button ID="Search" runat="server" Text="Ricerca" Visible="false" />
    &nbsp;
    <asp:TextBox AutoPostBack="True" ID="IdRole" runat="server" Visible="false" Width="16px" />
</div>
