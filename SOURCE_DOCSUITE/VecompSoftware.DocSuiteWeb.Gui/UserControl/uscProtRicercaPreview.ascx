<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProtRicercaPreview.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtRicercaPreview" %>
<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc1" %>
<%@ Register Src="../UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="uc3" %>
<%@ Register Src="../UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc3" %>

<asp:Panel ID="searchTable" runat="server">
    <table class="dataform">
        <tr id="rowYear" runat="server">
            <td class="label" style="width: 30%;">Anno:
            </td>
            <td style="width: 70%;">
                <telerik:RadNumericTextBox ID="txtYear" ReadOnly="true" DisabledStyle-CssClass="preview-disabled-input" Enabled="false" Width="56px" runat="server" />
            </td>
        </tr>
        <tr id="rowNumber" runat="server">
            <td class="label">Numero:
            </td>
            <td>
                <telerik:RadNumericTextBox ID="txtNumber" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" Width="96px" runat="server" />
            </td>
        </tr>
        <tr id="rowRegistrationDate" runat="server" style="vertical-align: middle;">
            <td class="label">Data:
            </td>
            <td>
                <telerik:RadDatePicker ID="txtRegistrationDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" DateInput-DisabledStyle-CssClass="preview-disabled-input" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" ReadOnly="true" Enabled="false" />
                <telerik:RadDatePicker ID="txtRegistrationDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" DateInput-DisabledStyle-CssClass="preview-disabled-input" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" ReadOnly="true" Enabled="false" />
            </td>
        </tr>
        <tr id="rowLogType" runat="server">
            <td class="label">Protocolli da leggere:
            </td>
            <td>
                <asp:CheckBox ID="chbNoRead" ReadOnly="true" Enabled="false" runat="server" />
            </td>
        </tr>
        <tr id="rowDdlType" runat="server">
            <td class="label">Tipo:
            </td>
            <td>
                <asp:DropDownList AppendDataBoundItems="True" ReadOnly="true" Enabled="false" DataTextField="Description" DataValueField="Id" ID="ddlType" runat="server">
                    <asp:ListItem />
                </asp:DropDownList>
            </td>
        </tr>
        <tr runat="server" id="trLocazione">
            <td class="label">Locazione:
            </td>
            <td>
                <asp:DropDownList AppendDataBoundItems="True" ReadOnly="true" Enabled="false" ID="ddlLocation" runat="server">
                    <asp:ListItem />
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="rowContainer" runat="server">
            <td class="label">Contenitore:
            </td>
            <td>
                <telerik:RadComboBox
                    AutoPostBack="True"
                    CausesValidation="False"
                    EnableLoadOnDemand="True"
                    EnableItemCaching="True"
                    MarkFirstMatch="True"
                    ID="rcbContainer"
                    ItemRequestTimeout="500"
                    runat="server"
                    Width="250px"
                    ReadOnly="true" Enabled="false"
                    Style="display: none;" />
                <asp:DropDownList AppendDataBoundItems="True" ReadOnly="true" Enabled="false" ID="ddlContainer" runat="server" Style="display: none;">
                    <asp:ListItem />
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="rowIdDocType" runat="server">
            <td class="label">Tipologia spedizione:
            </td>
            <td>
                <asp:DropDownList AppendDataBoundItems="true" ReadOnly="true" Enabled="false" ID="idDocType" runat="server">
                    <asp:ListItem />
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="rowClaim" runat="server">
            <td class="label">Reclamo:
            </td>
            <td>
                <asp:RadioButtonList ID="rblClaim" ReadOnly="true" Enabled="false" RepeatDirection="Horizontal" runat="server">
                    <asp:ListItem Value="0">Si</asp:ListItem>
                    <asp:ListItem Value="1">No</asp:ListItem>
                    <asp:ListItem Value="2" Selected="True">Tutti</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr id="rowPackage" runat="server">
            <td class="label">Tipologia:
            </td>
            <td>
                <telerik:RadTextBox ID="Origin" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="20px" />
                <asp:Label runat="server" ID="lblPackage" Font-Bold="true">Scatolone: </asp:Label>
                <telerik:RadTextBox ID="Package" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="50px" />
                <asp:Label runat="server" ID="lblLot" Font-Bold="true">Lotto: </asp:Label>
                <telerik:RadTextBox ID="Lot" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="50px" />
                <asp:Label runat="server" ID="lblIncremental" Font-Bold="true">Progressivo: </asp:Label>
                <telerik:RadTextBox ID="Incremental" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="50px" />
            </td>
        </tr>
        <tr id="rowInvoice" runat="server">
            <td class="label">Numero Fattura:
            </td>
            <td>
                <telerik:RadTextBox ID="InvoiceNumber" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="96px" />
            </td>
        </tr>
        <tr id="rowInvoice2" runat="server" style="vertical-align: middle;">
            <td class="label">Data Fattura:
            </td>
            <td>
                <telerik:RadDatePicker ID="InvoiceDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" DateInput-DisabledStyle-CssClass="preview-disabled-input" Width="200" DateInput-Label="Da" runat="server" ReadOnly="true" Enabled="false" />
                <telerik:RadDatePicker ID="InvoiceDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" DateInput-DisabledStyle-CssClass="preview-disabled-input" Width="200" DateInput-Label="A" runat="server" ReadOnly="true" Enabled="false" />
            </td>
        </tr>
        <tr id="rowInvoice3" runat="server">
            <td class="label">Contabilità Sezionale:
            </td>
            <td>
                <telerik:RadTextBox ID="AccountingSectional" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" />
                <asp:Label runat="server" ID="lblAccountingYear" Font-Bold="true">Anno: </asp:Label>
                <telerik:RadTextBox ID="AccountingYear" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="50px" />
                <asp:Label runat="server" ID="lblAccountingNumber" Font-Bold="true">Numero: </asp:Label>
                <telerik:RadTextBox ID="AccountingNumber" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" />
            </td>
        </tr>
        <tr id="sett" runat="server">
            <td class="label">Settore autorizzato:
            </td>
            <td style="width: 100%" class="column-disabled">
                <uc3:uscSettori HeaderVisible="false" ID="uscSettore" MultiSelect="false" ReadOnly="true" Required="false" runat="server" />
            </td>
        </tr>
        <tr id="rowDocumentNumber" runat="server">
            <td class="label">Numero Prot. del Mittente:
            </td>
            <td>
                <telerik:RadTextBox ID="txtDocumentNumber" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="96px" />
            </td>
        </tr>
        <tr id="rowDocumentDate" runat="server" style="vertical-align: middle;">
            <td class="label">Data Prot. del Mittente:
            </td>
            <td>
                <telerik:RadDatePicker ID="txtDocumentDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" DateInput-DisabledStyle-CssClass="preview-disabled-input" Width="200" DateInput-Label="Da" runat="server" ReadOnly="true" Enabled="false" />
                <telerik:RadDatePicker ID="txtDocumentDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" DateInput-DisabledStyle-CssClass="preview-disabled-input" Width="200" DateInput-Label="A" runat="server" ReadOnly="true" Enabled="false" />
            </td>
        </tr>
        <tr id="rowObjectProtocol" runat="server">
            <td class="label">Oggetto:
            </td>
            <td>
                <telerik:RadTextBox ID="txtObjectProtocol" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="300px" />
            </td>
        </tr>
        <tr id="rowClausola" runat="server">
            <td class="label">&nbsp;
            </td>
            <td>
                <asp:RadioButtonList ID="rblClausola" runat="server" ReadOnly="true" Enabled="false" RepeatDirection="Horizontal">
                    <asp:ListItem Value="AND" Selected="True">Tutte le parole</asp:ListItem>
                    <asp:ListItem Value="OR">Almeno una parola</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr id="rowNote" runat="server">
            <td class="label">Note:
            </td>
            <td>
                <telerik:RadTextBox ID="txtNote" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="300px" />
            </td>
        </tr>
        <tr id="rowInterop" runat="server">
            <td class="label">Contatti:
            </td>
            <td style="width: 500px;">
                <uc3:uscContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" HeaderVisible="false" ID="UscContattiSel1" IsRequired="false" MultiSelect="false" ReadOnly="true" runat="server" SearchAll="true" Type="Prot" />
            </td>
        </tr>
        <tr id="rowInteropExtended" runat="server">
            <td class="label">&nbsp;
            </td>
            <td>
                <asp:CheckBox ID="chbContactChild" runat="server" ReadOnly="true" Enabled="false" Text="Estendi ricerca ai sottocontatti." />
            </td>
        </tr>
        <tr id="rowLegacyDescriptionSearchBehaviour" runat="server">
            <td class="label" style="vertical-align: top; line-height: 20px;">Mittente/Destinatario:
            </td>
            <td>
                <asp:Panel ID="pnlLegacyDescriptionSearchBehaviour" runat="server">
                    <telerik:RadTextBox ID="txtRecipient" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="300px" />
                    &nbsp;
                        <asp:CheckBox Checked="true" ID="chkRecipientContains" ReadOnly="true" Enabled="false" runat="server" Text="Contiene" />
                </asp:Panel>
                <asp:Panel ID="pnlDescriptionSearchBehaviour" runat="server">
                    <telerik:RadTextBox ID="txtContactDescription" runat="server" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" Width="300px" /><br />
                    <asp:RadioButtonList ID="rblTextMatchMode" runat="server" ReadOnly="true" Enabled="false" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Contiene" Value="Contains" Selected="True" />
                        <asp:ListItem Text="Combinazioni" Value="Anywhere" />
                        <asp:ListItem Text="Esattamente" Value="Equals" />
                        <asp:ListItem Text="Inizia con" Value="StartsWith" />
                        <asp:ListItem Text="Finisce con" Value="EndsWith" />
                    </asp:RadioButtonList>
                    <asp:RadioButtonList ID="rblAtLeastOne" runat="server" ReadOnly="true" Enabled="false" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Tutte le parole" Value="0" Selected="True" />
                        <asp:ListItem Text="Almeno una parola" Value="1" />
                    </asp:RadioButtonList>
                </asp:Panel>
            </td>
        </tr>
        <tr id="rowPerson" runat="server">
            <td class="label">Titolo di studio:
            </td>
            <td>
                <asp:DropDownList AppendDataBoundItems="True" ReadOnly="true" Enabled="false" ID="cmbTitoliStudio" runat="server">
                    <asp:ListItem />
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="rowPerson2" runat="server" style="vertical-align: middle;">
            <td class="label">Data di nascita:
            </td>
            <td>
                <telerik:RadDatePicker ID="txtBirthDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" DateInput-DisabledStyle-CssClass="preview-disabled-input" Width="200" DateInput-Label="Da" runat="server" ReadOnly="true" Enabled="false" />
                <telerik:RadDatePicker ID="txtBirthDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" DateInput-DisabledStyle-CssClass="preview-disabled-input" Width="200" DateInput-Label="A" runat="server" ReadOnly="true" Enabled="false" />
            </td>
        </tr>
        <tr id="rowPerson3" runat="server">
            <td class="label">Comune di residenza:
            </td>
            <td>
                <telerik:RadTextBox ID="txtCity" runat="server" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" Width="300px" />
            </td>
        </tr>
        <tr id="rowSubject" runat="server">
            <td class="label">Assegnatario/Proponente:
            </td>
            <td>
                <telerik:RadTextBox ID="txtSubject" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="300px" />
            </td>
        </tr>
        <tr id="rowServiceCategory" runat="server">
            <td class="label">Categoria di servizio:
            </td>
            <td>
                <telerik:RadTextBox ID="txtServiceCategory" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="300px" />
            </td>
        </tr>
        <tr id="rowUscClassificatore1" runat="server">
            <td class="label">Classificazione:
            </td>
            <td style="width: 500px;">
                <uc1:uscClassificatore HeaderVisible="false" ID="UscClassificatore1" ReadOnly="true" Action="Search" Required="false" runat="server" />
                <asp:CheckBox ID="chbCategoryChild" runat="server" ReadOnly="true" Enabled="false" Text="Estendi ricerca alle Sottocategorie" />
            </td>
        </tr>
        <tr id="rowDocumentName" runat="server">
            <td class="label">Nome Documento:
            </td>
            <td>
                <telerik:RadTextBox ID="txtDocumentName" ReadOnly="true" Enabled="false" DisabledStyle-CssClass="preview-disabled-input" runat="server" Width="300px" />
            </td>
        </tr>
        <tr id="rowIncomplete" runat="server">
            <td class="label">Protocolli Incompleti:
            </td>
            <td>
                <asp:CheckBox ID="cbIncomplete" ReadOnly="true" Enabled="false" runat="server" />
            </td>
        </tr>
        <tr id="rowStatusCancel" runat="server">
            <td class="label">Protocolli Annullati:
            </td>
            <td>
                <asp:CheckBox ID="chbStatusCancel" ReadOnly="true" Enabled="false" runat="server" />
            </td>
        </tr>
        <tr id="rowNoRoles" runat="server">
            <td class="label">Protocolli senza Autorizzazioni:
            </td>
            <td>
                <asp:CheckBox ID="chbNoRoles" ReadOnly="true" Enabled="false" runat="server" />
            </td>
        </tr>
        <tr id="rowStatusSearch" runat="server">
            <td class="label">Stato Protocollo:
            </td>
            <td>
                <asp:DropDownList AppendDataBoundItems="True" ReadOnly="true" Enabled="false" ID="ProtocolStatus" runat="server">
                    <asp:ListItem />
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="rowOnlyMyProt" runat="server">
            <td class="label">Solo protocolli creati da me:
            </td>
            <td>
                <asp:CheckBox ID="cbOnlyMyProt" ReadOnly="true" Enabled="false" runat="server" />
            </td>
        </tr>
        <tr id="rowPec" runat="server">
            <td class="label">Protocolli da pec:
            </td>
            <td>
                <asp:CheckBox ID="hasIngoingPec" ReadOnly="true" Enabled="false" runat="server" />
            </td>
        </tr>
        <tr id="rowDistribution" runat="server">
            <td class="label">Protocolli da distribuire:
            </td>
            <td>
                <asp:CheckBox ID="IsProtocolDistribution" ReadOnly="true" Enabled="false" runat="server" />
            </td>
        </tr>
        <tr id="rowHighlight" runat="server">
            <td class="label">Protocolli in evidenza:
            </td>
            <td>
                <asp:CheckBox ID="cbProtocolHighlight" ReadOnly="true" Enabled="false" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>
