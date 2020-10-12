<%@ Page Language="vb" Title="Cruscotto" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSInvoiceSearch.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSInvoiceSearch" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="ContattiSel" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">

            var udsInvoice;


            require(["UDS/udsInvoiceSearch"], function (UDSInvoice) {
                $(function () {
                    udsInvoice = new UDSInvoice(tenantModelConfiguration.serviceConfiguration);
                    udsInvoice.udsInvoiceGridId = "<%= udsInvoiceGrid.ClientID%>";
                    udsInvoice.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    udsInvoice.radWindowManagerId = "<%= radWindowManager.ClientID %>";

                    udsInvoice.btnSearchId = "<%= btnSearch.ClientID%>";
                    udsInvoice.btnUploadId = "<%= btnUpload.ClientID%>";
                    udsInvoice.btnCleanId = "<%= btnClean.ClientID %>";

                    udsInvoice.btnInvoiceDeleteId = "<%= btnInvoiceDelete.ClientID%>";
                    udsInvoice.btnInvoiceMovedId = "<%= btnInvoiceMove.ClientID%>";

                    udsInvoice.dpStartDateFromId = "<%= dtpDateFrom.ClientID%>";
                    udsInvoice.dpEndDateFromId = "<%= dtpDateTo.ClientID%>";
                    udsInvoice.cmbStatoId = "<%= cmbStato.ClientID %>";
                    udsInvoice.udsInvoiceTypology = "<%= UDSInvoiceTypology %>";
                    udsInvoice.cmdRepositoriNameId = "<%= cmdRepositoriName.ClientID %>";
                    udsInvoice.txtNumeroFatturaId ="<%=txtNumeroFattura.ClientID%>";
                    udsInvoice.txtImportoId = "<%= txtImporto.ClientID %>";

                    udsInvoice.txtDenominazioneManualId = "<%= txtDenominazioneManual.ClientID %>";
                    udsInvoice.uscDenominazioneId = "<%= uscDenominazione.ClientID %>";
                    udsInvoice.txtYearId = "<%= txtYear.ClientID %>";

                    udsInvoice.dtpDataIvaFromId = "<%= dtpDataIvaFrom.ClientID %>";
                    udsInvoice.dtpDataIvaToId = "<%= dtpDataIvaTo.ClientID %>";

                    udsInvoice.dtpDataReciveSDIFromId = "<%= dtpDataReciveSDIfrom.ClientID %>";
                    udsInvoice.dtpDataReciveSDIToId = "<%= dtpDataReciveSDIto.ClientID %>";

                    udsInvoice.dtpDataAcceptFromId = "<%= dtpDataAcceptFrom.ClientID %>";
                    udsInvoice.dtpDataAcceptToId = "<%= dtpDataAcceptTo.ClientID %>";
                    udsInvoice.txtIndirizzoPECId = "<%= txtPecMail.ClientID%>";

                    var invoiceDirection = '<%= InvoiceDirection %>';
                    var invoiceTKind = '<%= InvoiceKind %>';
                    var invoiceTStatus = '<%=  InvoiceStatus %>';
                    udsInvoice.direction = invoiceDirection;
                    udsInvoice.invoiceKind = invoiceTKind;
                    udsInvoice.invoiceStatus = invoiceTStatus;
                    udsInvoice.tenantCompanyName = '<%=  TenantCompanyName %>';
                    udsInvoice.initialize();
                });
            });

            function btnDocumentsClick() {
                btnGridDocumentsClick();
            }

            function btnSelectAll() {
                btnGridSelectAll(<%= ProtocolEnv.SelectableProtocolThreshold %>);
            }

            function btnDeselectAll() {
                btnGridDeselectAll();
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    udsInvoice.onPageChanged();
                }
                if (args.get_commandName() === "Sort") {
                    args.set_cancel(true);
                    udsInvoice.loadResults(0);
                }
            }

            function OnGridDataBound(sender, args) {
                udsInvoice.onGridDataBound();
            }

            function getDate(date) {
                if (!date) return "";
                var mDate = moment(date);
                return mDate.format("DD/MM/YYYY");
            }

            function formatValue(number) {
                if (!number) return "";
                return Number(number).toLocaleString("es-ES", { minimumFractionDigits: 2 });

            }

            function getCategory(category) {
                if (!category) return "";
                return category.Name;
            }

            function getContact(IdUDS, label) {
                var getContactR = "";
                var murl = "<%= UDSContactUrl %>?$filter=IdUDS eq " + IdUDS + " and ContactLabel eq '" + label + "'&$expand=Relation";
                $.ajaxSetup({
                    async: false
                });
                console.log(murl);
                $.getJSON(murl, function (result) {
                    var nvalue = result.value.length;
                    if (result.value[0]) {
                        if (result.value[0].hasOwnProperty("ContactManual") && result.value[0].ContactManual != null) {
                            var obj = jQuery.parseJSON(result.value[0].ContactManual);
                            getContactR = "" + obj.Contact.Description;
                        } else if (result.value[0].hasOwnProperty("Relation")) {
                            var obj = result.value[0].Relation.Description.replace("|", " ");
                            getContactR = "" + obj;
                        }
                        if (nvalue > 1) {
                            getContactR = getContactR + ",..";
                        }
                    }
                });
                return getContactR;
            }

            function getAuthorization(IdUDS) {
                var getAutho = "";
                var murl = "<%= UDSRoleUrl %>?$filter=IdUDS eq " + IdUDS + "&$expand=Relation ";
                $.ajaxSetup({
                    async: false
                });
                $.getJSON(murl, function (result) {
                    var nvalue = result.value.length;
                    if (result.value[0] && result.value[0].hasOwnProperty("Relation")) {
                        getAutho = "" + result.value[0].Relation.Name;
                        if (nvalue > 1) { getAutho = getAutho + ",.."; }
                    }
                });
                return getAutho;
            }

            function getUDNumber(number) {
                if (!number) return "";
                return number.padLeft(7);
            }

            function getNumber(number) {
                if (!number) return "";
                return number;
            }

            function getBoolean(bool) {
                if (!bool) return false;
                return bool;
            }

            function checkEmpty(value) {
                if (!value && value == null) return "";
                return value;
            }

            function getLookup(lookup) {
                if (!lookup) return "";
                var values = JSON.parse(lookup);
                return values.join(', ');
            }

            function hasDocuments(documents) {
                if (!documents) return false;
                return documents.$values.length > 0;
            }

            function isCopyToPec() {
                var copyToPec = "<%= (CopyToPEC.HasValue AndAlso CopyToPEC.Value) %>";
                return copyToPec == "True";
            }

            function getDocumentIcon(documents) {
                mainDocument = jQuery.grep(documents.$values, function (document) {
                    return document.DocumentType == 1;
                })[0];
                return getDocumentIconURL(mainDocument);
            }

            function getExtension(filename) {
                if (filename == undefined)
                    return '';
                return filename.split('.').pop().toLowerCase();
            }


            function selectOnlyThis(id) {
                var checkboxes = document.getElementsByName("checkBoxInvoice");
                udsInvoice.saveInvoiceSelectionsToSessionStorage(checkboxes);
                var atLeastOneSelected = false;
                Array.prototype.forEach.call(checkboxes, function (el) {
                    if (el.checked) {
                        atLeastOneSelected = true;
                    }
                });
                udsInvoice.changeEnableBtn(atLeastOneSelected, id.value);
            }

            function onClientCheckedChanged(ischecked) {
                var checkboxes = document.getElementsByName("checkBoxInvoice");
                udsInvoice.saveInvoiceSelectionsToSessionStorage(checkboxes);
                var atLeastOneSelected = false;
                var lastId = 0;
                Array.prototype.forEach.call(checkboxes, function (el) {
                    el.checked = ischecked;
                    atLeastOneSelected = el.checked;
                    lastId = el.value;
                });
                udsInvoice.changeEnableBtn(atLeastOneSelected, lastId);
            }

            $(document).ready(function () {
                $('#checkAllCheckBox').change(function () {
                    onClientCheckedChanged($('#checkAllCheckBox').prop("checked"));
                });
            });

        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 150px;">Nome Archivio:</td>
            <td style="width: 450px;">
                <telerik:RadComboBox runat="server" ID="cmdRepositoriName" DataTextField="Text" DataValueField="Value" Width="50%" AutoPostBack="false" />
            </td>

            <td class="label labelPanel" style="width: 150px;">Dati fiscali:</td>
            <td></td>
        </tr>

        <tr>
            <td class="label labelPanel">Data fattura:</td>
            <td>
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" DateInput-Label="Da " ID="dtpDateFrom" runat="server" />
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" DateInput-Label="A " ID="dtpDateTo" runat="server" />
            </td>

            <td class="label labelPanel">Anno IVA:</td>
            <td>
                <telerik:RadNumericTextBox ID="txtYear" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" Width="200px" runat="server" />

            </td>

        </tr>
        <tr>
            <td class="label labelPanel">Numero fattura:</td>
            <td>
                <telerik:RadTextBox ID="txtNumeroFattura" runat="server" Width="50%" />
                <span>
                    <input type="checkbox" name="chkNumeroFatturafilter" id="chkNumeroFatturafilter" value="Contiene" checked="checked" />Contiene</span>
            </td>

            <td class="label labelPanel">Data IVA:</td>
            <td>
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" DateInput-Label="Da" ID="dtpDataIvaFrom" runat="server" />
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" DateInput-Label="A" ID="dtpDataIvaTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">Importo:</td>
            <td>
                <telerik:RadNumericTextBox ID="txtImporto" NumberFormat-DecimalDigits="2" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="10" Width="50%" runat="server" />
            </td>
            <td class="label labelPanel">Data ricezione SDI:</td>
            <td>
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" DateInput-Label="Da" ID="dtpDataReciveSDIfrom" runat="server" />
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" DateInput-Label="A" ID="dtpDataReciveSDIto" runat="server" />
            </td>


        </tr>
        <tr>
            <td class="label labelPanel">
                <span id="ltlCustomer">Cliente</span>
                <span id="ltlSupplier">Fornitore</span>
            </td>
            <td>
                <usc:ContattiSel ButtonImportManualVisible="false" Visible="false" IsRequired="false" ButtonImportVisible="False" ButtonManualVisible="true" ButtonPropertiesVisible="True" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="true" ButtonSelectVisible="False" Caption="Denominazione" HeaderVisible="false" EnableCC="false" ID="uscDenominazione" Multiple="false" MultiSelect="false" ProtType="True" ReadOnly="False" runat="server" TreeViewCaption="Denominazione" Type="Prot" />
                <telerik:RadTextBox ID="txtDenominazioneManual" runat="server" Width="50%" />
                <span>
                    <input type="checkbox" name="chkDenominazioneManualfilter" id="chkDenominazioneManualfilter" value="Contiene" checked="checked" />Contiene</span>
            </td>
            <td class="label labelPanel">Data Accettazione:
            </td>
            <td>
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" DateInput-Label="Da" ID="dtpDataAcceptFrom" runat="server" />
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" DateInput-Label="A" ID="dtpDataAcceptTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">Stato:
            </td>
            <td>
                <telerik:RadComboBox runat="server" ID="cmbStato" DataTextField="Text" DataValueField="Value" Width="50%" AutoPostBack="false" EmptyMessage="" />
            </td>
            <td class="label labelPanel"><span id="ltlPecMail">Indirizzo PEC</span></td>
            <td>
                <span id="ltlchktxtPecMail">
                    <telerik:RadTextBox ID="txtPecMail" Width="200px" runat="server" />
                    <input type="checkbox" name="chktxtPecMail" id="chktxtPecMail" value="Vuoto" />
                    Vuoto
                </span>
            </td>
    </table>

    <div style="margin-left: 2px;">
        <telerik:RadButton ID="btnSearch" Text="Aggiorna visualizzazione" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
        <telerik:RadButton ID="btnClean" Text="Azzera filtri" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
    </div>
    <telerik:RadWindowManager EnableViewState="False" ID="radWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow Height="600" ID="managerUploadDocument" runat="server" Title="Importazione fatture da cassetto fiscale - Agenzia delle Entrate" Width="750" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="udsInvoiceGrid" ID="udsInvoiceGrid" Skin="Office2010Blue" GridLines="None" Height="100%" PageSize="30" AllowPaging="True" AllowMultiRowSelection="True"
            AllowFilteringByColumn="False" AllowSorting="true">
            <ClientSettings>
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="true" AllowRowResize="true" />
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
            </ClientSettings>
            <MasterTableView CommandItemDisplay="None"
                AutoGenerateColumns="False"
                TableLayout="fixed"
                NoMasterRecordsText="Nessun elemento trovato nel periodo indicato."
                PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}"
                AllowMultiColumnSorting="True">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="" HeaderStyle-Width="3%" HeaderStyle-HorizontalAlign="Center" AllowFiltering="false">
                        <HeaderTemplate>
                            <asp:CheckBox runat="server" ID="checkAllCheckBox" ClientIDMode="Static" />
                        </HeaderTemplate>
                        <ClientItemTemplate>
                                 <input type="checkbox" name="checkBoxInvoice" value="#=UDSId#" onclick="selectOnlyThis(this)"/>                            
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="colViewDocuments" HeaderText="Tipo Documento" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" AllowFiltering="false" Groupable="false" AllowSorting="True">
                        <HeaderStyle HorizontalAlign="Center" Width="20px" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ClientItemTemplate>
                        # if(hasDocuments(Documents)){#
                        <a href="../Viewers/UDSViewer.aspx?IdUDS=#=UDSId#&IdUDSRepository=#=IdUDSRepository#">
                            <img class="dsw-text-center" src="#=getDocumentIcon(Documents)#" height="16px" width="16px" />
                        </a>
                        #}#
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Numero Fattura" DataField="NumeroFattura" UniqueName="NumeroFattura" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="NumeroFattura" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                             <a href="UDSView.aspx?Type=UDS&IdUDS=#=UDSId#&IdUDSRepository=#=IdUDSRepository#">#=getNumber(NumeroFattura)#</a>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn HeaderText="Data Fattura" DataField="DataFattura" UniqueName="DataFattura" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="DataFattura" DataType="System.DateTime" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=getDate(DataFattura)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Denominazione" DataField="Denominazione" UniqueName="Denominazione" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="Denominazione" AllowFiltering="false">
                        <ClientItemTemplate>
                                        <label>#=Denominazione#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn HeaderText="Importo" DataField="Importo" UniqueName="Importo" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Right" SortExpression="Importo" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=formatValue(Importo)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>


                    <telerik:GridTemplateColumn HeaderText="Piva/CF" DataField="Pivacf" UniqueName="Pivacf" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="Pivacf" AllowFiltering="false" Visible="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=Pivacf#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Anno Iva" DataField="AnnoIva" UniqueName="AnnoIva" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="AnnoIva" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=checkEmpty(AnnoIva)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Data Iva" DataField="DataIva" UniqueName="DataIva" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="DataIva" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=getDate(DataIva)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Sezionale Iva" DataField="SezionaleIva" UniqueName="SezionaleIva" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="SezionaleIva" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=checkEmpty(SezionaleIva)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Protocollo Iva" DataField="ProtocolloIva" UniqueName="ProtocolloIva" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="ProtocolloIva" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=checkEmpty(ProtocolloIva)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Data Ricezione Sdi" DataField="DataRicezioneSdi" UniqueName="DataRicezioneSdi" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="DataRicezioneSdi" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=getDate(DataRicezioneSdi)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Identificativo Sdi" DataField="IdentificativoSdi" UniqueName="IdentificativoSdi" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="IdentificativoSdi" AllowFiltering="false" Visible="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=checkEmpty(IdentificativoSdi)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Progessivo Invio Sdi" DataField="ProgessivoInvioSdi" UniqueName="ProgessivoInvioSdi" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="ProgessivoInvioSdi" AllowFiltering="false" Visible="false">
                        <ClientItemTemplate>
                                        <label>#=checkEmpty(ProgessivoInvioSdi)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Stato Fattura" DataField="StatoFattura" UniqueName="StatoFattura" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="StatoFattura" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>
                                        <label>#=StatoFattura#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Indirizzo PEC" DataField="IndirizzoPec" UniqueName="IndirizzoPec" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        SortExpression="IndirizzoPec" AllowFiltering="false" AllowSorting="True">
                        <ClientItemTemplate>                  
                               <label>#if(!(typeof IndirizzoPec === "undefined")){##=checkEmpty(IndirizzoPec)##}#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true" PageSizeControlType="None"></PagerStyle>

        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton ID="btnUpload" runat="server" AutoPostBack="false" Width="120px" Text="Cassetto fiscale" ToolTip="Importazione fatture da cassetto fiscale - Agenzia delle Entrate" />
        <telerik:RadButton ID="btnInvoiceDelete" runat="server" AutoPostBack="false" Width="120px" Text="Annulla fattura" ToolTip="Annulla fattura" />
        <telerik:RadButton ID="btnInvoiceMove" runat="server" AutoPostBack="false" Width="120px" Text="Sposta fattura" ToolTip="Sposta fattura (funzione abilitata per le sole fatture attive)" />
    </asp:Panel>
</asp:Content>
