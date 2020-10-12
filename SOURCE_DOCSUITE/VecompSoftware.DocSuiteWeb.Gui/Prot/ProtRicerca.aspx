<%@ Page AutoEventWireup="false" CodeBehind="ProtRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtRicerca" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Ricerca" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc1" %>
<%@ Register Src="../UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="uc3" %>
<%@ Register Src="../UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc3" %>

<asp:Content ID="cphContent" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    var combo = $find("<%= rcbContainer.ClientID%>");
                    if (combo != undefined && combo.get_visible()) {
                        //combo.requestItems("");
                        var savedValue = $get('<%= rcbContainerValue.ClientID%>');
                        if (savedValue.value != "" && combo.findItemByValue(savedValue.value)) {
                            combo.findItemByValue(savedValue.value).select();
                        }
                    }

                }
            }

            function IsNullOrWhiteSpaces(source) {
                return !source.trim();
            }

            function YearValidation(source, args) {
                var txtContactDescription = $get("<%= txtContactDescription.ClientID%>");
                args.IsValid = IsNullOrWhiteSpaces(txtContactDescription.value);
                if (args.IsValid)
                    return;

                var txtYear = $get("<%= txtYear.ClientID%>");
                args.IsValid = !IsNullOrWhiteSpaces(txtYear.value);
            }
        </script>
    </telerik:RadScriptBlock>
    <asp:Panel ID="searchTable" runat="server">
        <table class="dataform">
            <tr id="rowYear" runat="server">
                <td class="label" style="width: 30%;">Anno:
                </td>
                <td style="width: 70%;">
                    <telerik:RadNumericTextBox ID="txtYear" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" Width="56px" runat="server" />
                    <asp:CustomValidator ID="cvYear" Enabled="false" runat="server" Display="Dynamic" ClientValidationFunction="YearValidation" Text="Anno obbligatorio in ricerca per contatto." />
                </td>
            </tr>
            <tr id="rowNumber" runat="server">
                <td class="label">Numero:
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtNumber" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="7" Width="96px" runat="server" />
                </td>
            </tr>
            <tr id="rowRegistrationDate" runat="server" style="vertical-align: middle;">
                <td class="label">Data:
                </td>
                <td>
                    <telerik:RadDatePicker ID="txtRegistrationDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                    <telerik:RadDatePicker ID="txtRegistrationDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                </td>
            </tr>
            <tr id="rowLogType" runat="server">
                <td class="label">Da leggere:
                </td>
                <td>
                    <asp:CheckBox ID="chbNoRead" runat="server" />
                </td>
            </tr>
            <tr id="rowDdlType" runat="server">
                <td class="label">Tipo:
                </td>
                <td>
                    <asp:DropDownList AppendDataBoundItems="True" DataSourceID="odsProtocolType" DataTextField="Description" DataValueField="Id" ID="ddlType" runat="server">
                        <asp:ListItem />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trLocazione">
                <td class="label">Locazione:
                </td>
                <td>
                    <asp:DropDownList AppendDataBoundItems="True" DataSourceID="odsLocation" ID="ddlLocation" runat="server">
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
                        Width="500px"
                        Style="display: none;"
                        OnItemsRequested="RcbContainer_ItemsRequested" />
                    <input type="hidden" id="rcbContainerValue" runat="server" value="" />
                    <asp:DropDownList AppendDataBoundItems="True" ID="ddlContainer" runat="server" Style="display: none;">
                        <asp:ListItem />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="rowIdDocType" runat="server">
                <td class="label">Tipologia spedizione:
                </td>
                <td>
                    <asp:DropDownList AppendDataBoundItems="true" DataSourceID="odsDocumentType" ID="idDocType" runat="server">
                        <asp:ListItem />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="rowClaim" runat="server">
                <td class="label">Reclamo:
                </td>
                <td>
                    <asp:RadioButtonList ID="rblClaim" RepeatDirection="Horizontal" runat="server">
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
                    <asp:TextBox ID="Origin" MaxLength="1" runat="server" Width="20px" />
                    <asp:Label runat="server" ID="lblPackage" Font-Bold="true">Scatolone: </asp:Label>
                    <asp:TextBox ID="Package" MaxLength="6" runat="server" Width="50px" />
                    <asp:RegularExpressionValidator ControlToValidate="Package" Display="Dynamic" ErrorMessage="Errore formato" ID="rPackage" runat="server" ValidationExpression="\d*" />
                    <asp:Label runat="server" ID="lblLot" Font-Bold="true">Lotto: </asp:Label>
                    <asp:TextBox ID="Lot" MaxLength="6" runat="server" Width="50px" />
                    <asp:RegularExpressionValidator ControlToValidate="Lot" Display="Dynamic" ErrorMessage="Errore formato" ID="rLot" runat="server" ValidationExpression="\d*" />
                    <asp:Label runat="server" ID="lblIncremental" Font-Bold="true">Progressivo: </asp:Label>
                    <asp:TextBox ID="Incremental" MaxLength="3" runat="server" Width="50px" />
                    <asp:RegularExpressionValidator ControlToValidate="Incremental" Display="Dynamic" ErrorMessage="Errore formato" ID="rIncremental" runat="server" ValidationExpression="\d*" />
                </td>
            </tr>
            <tr id="rowInvoice" runat="server">
                <td class="label">Numero Fattura:
                </td>
                <td>
                    <asp:TextBox ID="InvoiceNumber" MaxLength="80" runat="server" Width="96px" />
                </td>
            </tr>
            <tr id="rowInvoice2" runat="server" style="vertical-align: middle;">
                <td class="label">Data fattura:
                </td>
                <td>
                    <telerik:RadDatePicker ID="InvoiceDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                    <telerik:RadDatePicker ID="InvoiceDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                </td>
            </tr>
            <tr id="rowInvoice3" runat="server">
                <td class="label">Contabilità Sezionale:
                </td>
                <td>
                    <asp:TextBox ID="AccountingSectional" MaxLength="50" runat="server" />
                    <asp:Label runat="server" ID="lblAccountingYear" Font-Bold="true">Anno: </asp:Label>
                    <asp:TextBox ID="AccountingYear" MaxLength="4" runat="server" Width="50px" />
                    <asp:CompareValidator ControlToValidate="AccountingYear" Display="Dynamic" ErrorMessage="Anno non Valido" ID="CompareValidator6" Operator="GreaterThan" runat="server" Type="Integer" ValueToCompare="0" />
                    <asp:Label runat="server" ID="lblAccountingNumber" Font-Bold="true">Numero: </asp:Label>
                    <asp:TextBox ID="AccountingNumber" MaxLength="10" runat="server" />
                    <asp:CompareValidator ControlToValidate="AccountingNumber" Display="Dynamic" ErrorMessage="Numero non Valido" ID="CompareValidator7" Operator="GreaterThan" runat="server" Type="Integer" ValueToCompare="0" />
                </td>
            </tr>
            <tr id="sett" runat="server">
                <td class="label">Settore autorizzato:
                </td>
                <td style="width: 100%">
                    <uc3:uscSettori HeaderVisible="false" ID="uscSettore" MultiSelect="false" Required="false" runat="server" />
                    <asp:CheckBox ID="chbRoleChild" runat="server" Text="Estendi ricerca ai sotto settori" />
                </td>
            </tr>
            <tr id="rowAssegnatario" runat="server">
                <td class="label">Assegnatario:
                </td>
                <td style="width: 100%">
                    <uc3:uscContattiSel ID="uscContactAss" runat="server" TreeViewCaption="Assegnatario"
                        ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false"
                        ButtonPropertiesVisible="false" HeaderVisible="false" IsRequired="false" btnSelContactDomain="false"
                        Type="Prot" />
                </td>
            </tr>
            <tr id="rowDocumentNumber" runat="server">
                <td class="label">Numero prot. del mittente:
                </td>
                <td>
                    <asp:TextBox ID="txtDocumentNumber" MaxLength="80" runat="server" Width="96px" />
                </td>
            </tr>
            <tr id="rowDocumentDate" runat="server" style="vertical-align: middle;">
                <td class="label">Data prot. del mittente:
                </td>
                <td>
                    <telerik:RadDatePicker ID="txtDocumentDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                    <telerik:RadDatePicker ID="txtDocumentDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                </td>
            </tr>
            <tr id="rowObjectProtocol" runat="server">
                <td class="label">Oggetto:
                </td>
                <td>
                    <asp:TextBox ID="txtObjectProtocol" MaxLength="255" runat="server" Width="300px" />
                </td>
            </tr>
            <tr id="rowClausola" runat="server">
                <td class="label">&nbsp;
                </td>
                <td>
                    <asp:RadioButtonList ID="rblClausola" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="AND" Selected="True">Tutte le parole</asp:ListItem>
                        <asp:ListItem Value="OR">Almeno una parola</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr id="rowNote" runat="server">
                <td class="label">Note:
                </td>
                <td>
                    <asp:TextBox ID="txtNote" MaxLength="255" runat="server" Width="300px" />
                </td>
            </tr>
            <tr id="rowInterop" runat="server">
                <td class="label">Contatti:
                </td>
                <td style="width: 500px;">
                    <uc3:uscContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonSelectDomainVisible="false" ButtonPropertiesVisible="false" HeaderVisible="false" ID="UscContattiSel1" IsRequired="false" MultiSelect="false" runat="server" SearchAll="true" Type="Prot" />
                </td>
            </tr>
            <tr id="rowInteropExtended" runat="server">
                <td class="label">&nbsp;
                </td>
                <td>
                    <asp:CheckBox ID="chbContactChild" runat="server" Text="Estendi ricerca ai sottocontatti." />
                </td>
            </tr>
            <tr id="rowLegacyDescriptionSearchBehaviour" runat="server">
                <td class="label" style="vertical-align: top; line-height: 20px;">Mittente/Destinatario:
                </td>
                <td>
                    <asp:Panel ID="pnlLegacyDescriptionSearchBehaviour" runat="server">
                        <asp:TextBox ID="txtRecipient" MaxLength="255" runat="server" Width="300px" />
                        &nbsp;
                        <asp:CheckBox Checked="true" ID="chkRecipientContains" runat="server" Text="Contiene" />
                    </asp:Panel>
                    <asp:Panel ID="pnlDescriptionSearchBehaviour" runat="server">
                        <asp:TextBox ID="txtContactDescription" runat="server" MaxLength="255" Width="300px" /><br />
                        <asp:CustomValidator ID="cvContactDescription" runat="server" Display="Dynamic" Text="Superato il massimo grado di complessità prevista, riprovare con meno parole." />
                        <asp:RadioButtonList ID="rblTextMatchMode" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Contiene" Value="Contains" Selected="True" />
                            <asp:ListItem Text="Combinazioni" Value="Anywhere" />
                            <asp:ListItem Text="Esattamente" Value="Equals" />
                            <asp:ListItem Text="Inizia con" Value="StartsWith" />
                            <asp:ListItem Text="Finisce con" Value="EndsWith" />
                        </asp:RadioButtonList>
                        <asp:RadioButtonList ID="rblAtLeastOne" runat="server" RepeatDirection="Horizontal">
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
                    <asp:DropDownList AppendDataBoundItems="True" DataSourceID="odsContactTitle" ID="cmbTitoliStudio" runat="server">
                        <asp:ListItem />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="rowPerson2" runat="server" style="vertical-align: middle;">
                <td class="label">Data di nascita:
                </td>
                <td>
                    <telerik:RadDatePicker ID="txtBirthDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                    <asp:CompareValidator ControlToValidate="txtBirthDateFrom" Display="Dynamic" ErrorMessage="Errore formato" ID="Comparevalidator9" Operator="DataTypeCheck" runat="server" Style="height: 100%; vertical-align: middle;" Type="Date" />
                    <telerik:RadDatePicker ID="txtBirthDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                    <asp:CompareValidator ControlToValidate="txtBirthDateTo" Display="Dynamic" ErrorMessage="Errore formato" ID="Comparevalidator8" Operator="DataTypeCheck" runat="server" Type="Date" />
                </td>
            </tr>
            <tr id="rowPerson3" runat="server">
                <td class="label">Comune di residenza:
                </td>
                <td>
                    <asp:TextBox ID="txtCity" runat="server" Width="300px" />
                </td>
            </tr>
            <tr id="rowSubject" runat="server">
                <td class="label">Assegnatario/Proponente:
                </td>
                <td>
                    <asp:TextBox ID="txtSubject" MaxLength="255" runat="server" Width="300px" />
                </td>
            </tr>
            <tr id="rowServiceCategory" runat="server">
                <td class="label">Categoria di servizio:
                </td>
                <td>
                    <asp:TextBox ID="txtServiceCategory" MaxLength="100" runat="server" Width="300px" />
                </td>
            </tr>
            <tr id="rowUscClassificatore1" runat="server">
                <td class="label">Classificazione:
                </td>
                <td style="width: 500px;">
                    <uc1:uscClassificatore HeaderVisible="false" ID="UscClassificatore1" Action="Search" Required="false" runat="server" />
                    <asp:CheckBox ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle sottocategorie" />
                </td>
            </tr>
            <tr id="rowDocumentName" runat="server">
                <td class="label">Nome documento:
                </td>
                <td>
                    <asp:TextBox ID="txtDocumentName" MaxLength="100" runat="server" Width="300px" />
                </td>
            </tr>
            <tr id="rowIncomplete" runat="server">
                <td class="label">Incompleti:
                </td>
                <td>
                    <asp:CheckBox ID="cbIncomplete" runat="server" />
                </td>
            </tr>
            <tr id="rowStatusCancel" runat="server">
                <td class="label">Annullati:
                </td>
                <td>
                    <asp:DropDownList ID="ddlStatusCancel" runat="server">
                        <asp:ListItem Value=""> </asp:ListItem>
                        <asp:ListItem Value="AND">Includi</asp:ListItem>
                        <asp:ListItem Value="OR">Solo Annullati</asp:ListItem>
                    </asp:DropDownList>

                </td>
            </tr>
            <tr id="rowAssignToMe" runat="server">
                <td class="label">Assegnati a me:
                </td>
                <td>
                    <asp:DropDownList ID="ddlAssignToMe" runat="server">
                        <asp:ListItem Value=""> </asp:ListItem>
                        <asp:ListItem Value="assignedtome">Competenza</asp:ListItem>
                        <asp:ListItem Value="assignedtomecc">Conoscenza</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="rowNoRoles" runat="server">
                <td class="label">Senza autorizzazioni:
                </td>
                <td>
                    <asp:CheckBox ID="chbNoRoles" runat="server" />
                </td>
            </tr>
            <tr id="rowStatusSearch" runat="server">
                <td class="label">Stato protocollo:
                </td>
                <td>
                    <asp:DropDownList AppendDataBoundItems="True" DataSourceID="odsProtocolStatus" ID="ProtocolStatus" runat="server">
                        <asp:ListItem />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="rowOnlyMyProt" runat="server">
                <td class="label">Solo creati da me:
                </td>
                <td>
                    <asp:CheckBox ID="cbOnlyMyProt" runat="server" />
                </td>
            </tr>
            <tr id="rowPec" runat="server">
                <td class="label">Da pec:
                </td>
                <td>
                    <asp:CheckBox ID="hasIngoingPec" runat="server" />
                </td>
            </tr>
            <tr id="rowDistribution" runat="server">
                <td class="label">Da distribuire:
                </td>
                <td>
                    <asp:CheckBox ID="IsProtocolDistribution" runat="server" />
                </td>
            </tr>
            <tr id="rowFascicle" runat="server">
                <td class="label">Protocollo fascicolati/referenziati:
                </td>
                <td>
                    <asp:CheckBox ID="cbFascicolated" runat="server" />
                </td>
            </tr>
            <tr id="rowHighlight" runat="server">
                <td class="label">In evidenza:
                </td>
                <td>
                    <asp:CheckBox ID="cbProtocolHighlight" runat="server" />
                </td>
            </tr>
        </table>

        <asp:ObjectDataSource ID="odsProtocolType" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetAllSearch" TypeName="VecompSoftware.DocSuiteWeb.Facade.ProtocolTypeFacade" />
        <asp:ObjectDataSource ID="odsLocation" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.LocationFacade" />
        <asp:ObjectDataSource ID="odsContactTitle" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.ContactTitleFacade" />
        <asp:ObjectDataSource ID="odsProtocolStatus" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.ProtocolStatusFacade" />
        <asp:ObjectDataSource ID="odsDocumentType" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.TableDocTypeFacade" />

    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnSearch" runat="server" TabIndex="1" Text="Ricerca" Width="150px" />
    <asp:Button ID="cmdClearFilters" runat="server" TabIndex="2" Text="Svuota Ricerca" Width="150px" />
    <asp:Button ID="btnExpandSearch" runat="server" TabIndex="3" Text="Ricerca Completa" Width="150px" />
</asp:Content>
