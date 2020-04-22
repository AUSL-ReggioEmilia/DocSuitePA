<%@ Page Title="Risultati Archivi" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSResults.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSResults" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <script type="text/javascript" src="../Scripts/dsw.radgrid.odata.js"></script>
    <style type="text/css">
        /*Controlli Telerik*/
        .RadInput_Bootstrap.riSingle .riTextBox {
            height: 34px !important;
        }

        .RadPicker_Bootstrap {
            height: 34px !important;
        }

        .rgPager {
            display: table-row !important;
        }

        div.RadGrid .rgPager .rgAdvPart {
            display: none;
        }
        .hiddenButtonClass{
            display: none;
        }
    </style>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var hiddenButtonId = "<%= hiddenButton.ClientID %>";
            var column = null;

            function menuShowing(sender, args) {
                if (this.column == null) return;
                var menu = sender; var items = menu.get_items();
                var i = 0;
                while (i < items.get_count()) {
                    if (!(items.getItem(i).get_value() in {
                        'NoFilter': '', 'NotIsEmpty': '', 'IsEmpty': '', 'NotEqualTo': '', 'EqualTo': '', 'GreaterThan': '', 'LessThan': '', 'GreaterThanOrEqualTo': '', 'LessThanOrEqualTo': ''
                    })) {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(false);
                    } i++;
                }
                this.column = null;
                menu.repaint();
            }

            function filterMenuShowing(sender, args) {
                column = args.get_column();
            }

            function resizeGrid(sender, args) {
                var masterTable = $find("<%= dgvUDS.ClientID %>").get_masterTableView();
                masterTable.rebind();
            }

            Sys.Application.add_load(function () {
                odataGridSource.grid = $find("<%=dgvUDS.ClientID%>");
                odataGridSource.loadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                odataGridSource.webApi = "<%=CurrentController %>";
                odataGridSource.defaultFilters = <%=SerializedDefaultFinder %>;
                var sortExpression = new Telerik.Web.UI.GridSortExpression();
                sortExpression.set_fieldName('_year,_number');
                sortExpression.set_sortOrder(<%= If(ProtocolEnv.ForceDescendingOrderElements, 0, 1) %>);
                odataGridSource.defaultSortExpression = sortExpression;
                loadRequest(new Array(), [sortExpression]);
            });

            function showLoadingPanel() {
                showGridLoadingPanel();
            }

            function getDate(date) {
                if (!date) return "";
                var mDate = moment(date);
                return mDate.format("DD/MM/YYYY");
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

            function getParsedJSON(value) {
                try {
                    var tmp = JSON.parse(value);
                    return tmp;
                } catch (e) {
                    return value;
                }
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

            function getLookup(lookup) {
                if (!lookup) return "";

                var parsedValue = getParsedJSON(lookup);
                if (Array.isArray(parsedValue)) {
                    return parsedValue.join(', ');
                }
                return parsedValue;
            }

            function getEnum(enumValue) {
                if (!enumValue) return "";

                var parsedValue = getParsedJSON(enumValue);
                if (Array.isArray(parsedValue)) {
                    return parsedValue.join(', ');
                }
                return parsedValue;
            }

            function hasDocuments(documents) {
                if (!documents) return false;
                return documents.length > 0;
            }

            function isCopyToPec() {
                var copyToPec = "<%= (CopyToPEC.HasValue AndAlso CopyToPEC.Value) %>";
                return copyToPec == "True";
            }

            function isFromUDSLink() {
                var isFromUDSLink = "<%= (IsFromUDSLink.HasValue AndAlso IsFromUDSLink.Value) %>";
                return isFromUDSLink == "True";
            }

            function getDocumentIcon(documents) {
                mainDocument = jQuery.grep(documents, function (document) {
                    return document.DocumentType == 1;
                })[0];
                return getDocumentIconURL(mainDocument);
            }

            function getExtension(filename) {
                if (filename == undefined)
                    return '';

                return filename.split('.').pop().toLowerCase();
            }

            function btnDocumentsClick() {
                btnGridDocumentsClick();
            }

            function btnSelectAll() {
                btnGridSelectAll(<%= ProtocolEnv.SelectableProtocolThreshold %>);
            }

            function btnDeselectAll() {
                btnGridDeselectAll();
            }

            function getRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function confirmConnection(currentIdUDSRepository, currentIdUDS) {
                var destinationIdUDS = "<%= CurrentIdUDS %>";
                var destinationUDSRepositoryId = "<%= CurrentIdUDSRepository %>";
                var confirm = window.confirm("Vuoi collegare l'archivio selezionato?");
                if (confirm) {
                    closeWindow(currentIdUDS + "|" + currentIdUDSRepository);
                }
            }

            function closeWindow(argument) {
                var oWindow = getRadWindow();
                oWindow.close(argument);
            }

            function exportData(){
                loadData();
            }
        </script>
    </telerik:RadCodeBlock>   

    <div style="margin-left:auto; margin-right:0; text-align:right">
        <div>
            <telerik:RadButton ID="btnEsportaPagina" runat="server" Text="Esporta pagina" ToolTip="Esporta in Excel" CausesValidation="false" />
            <telerik:RadButton ID="btnEsportaTutto" runat="server" Text="Esporta tutto" AutoPostBack="false" ToolTip="Esporta in Excel" CausesValidation="false" OnClientClicked="exportData"/>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    
    <telerik:RadAjaxPanel runat="server" CssClass="radGridWrapper">
        <telerik:RadGrid AutoGenerateColumns="false" Skin="Office2010Blue" ID="dgvUDS" AllowFilteringByColumn="True"
            AllowSorting="True" AllowPaging="True" PageSize="30" Width="100%" Height="100%" runat="server">
            <ExportSettings ExportOnlyData="true" IgnorePaging="true"></ExportSettings>
            <ClientSettings>
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="false" />
                <ClientEvents OnCommand="OnCommand" OnFilterMenuShowing="filterMenuShowing" OnColumnResizing="resizeGrid" />
            </ClientSettings>
            <MasterTableView CommandItemDisplay="Top" ClientDataKeyNames="IdUDSRepository,UDSId" AutoGenerateColumns="false" Width="100%" TableLayout="fixed" AllowFilteringByColumn="True" PagerStyle-PageSizeLabelText="Elementi per pagina:"
                PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">

                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" />
                <Columns>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="False" UniqueName="ClientSelectColumn">
                        <HeaderStyle HorizontalAlign="Center" Width="25px" />
                        <ItemStyle HorizontalAlign="Center" Width="25px" />
                        <ItemTemplate>
                            <asp:CheckBox AutoPostBack="False" ID="cbSelect" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="colHasRead" HeaderText="Da leggere" HeaderStyle-Width="20px" HeaderImageUrl="../Comm/Images/File/Mail16.gif" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemTemplate>
                            <asp:Image ID="imgHasRead" runat="server" Visible="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="colViewDocuments" HeaderText="Tipo Documento" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Width="20px" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ClientItemTemplate>
                            # if(hasDocuments(Documents)){#
                                <a href="../Viewers/UDSViewer.aspx?IdUDS=#=UDSId#&IdUDSRepository=#=IdUDSRepository#" onclick="showLoadingPanel()">
                                    <img class="dsw-text-center" src="#=getDocumentIcon(Documents)#" height="16px" width="16px" />
                                </a>
                            #}#
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center" SortExpression="_year,_number" DataField="AnnoNumero" HeaderText="Anno/Numero" AllowFiltering="false">
                        <ClientItemTemplate>
                            # if(isFromUDSLink()){#
                                <a onclick="confirmConnection('#=IdUDSRepository#', '#=UDSId#', '#=_year#|#=_number#|#=IdUDSRepository#')">#=_year#/#=getUDNumber(_number)#</a>
                            #} else {#
                                # if(isCopyToPec()){#
                                    <a href="" onclick="closeWindow('#=_year#|#=_number#|#=IdUDSRepository#')">#=_year#/#=getUDNumber(_number)#</a>
                                #} else {#
                                    <a href="UDSView.aspx?Type=UDS&IdUDS=#=UDSId#&IdUDSRepository=#=IdUDSRepository#" onclick="showLoadingPanel()">#=_year#/#=getUDNumber(_number)#</a>
                                #}#
                            #}#
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderStyle-Width="25%" DataField="_subject" HeaderStyle-HorizontalAlign="Center" UniqueName="_subject" HeaderText="Oggetto"
                        AutoPostBackOnFilter="true" SortExpression="_subject" CurrentFilterFunction="Contains" FilterControlWidth="87%">
                        <ClientItemTemplate>
                            <label>#=_subject#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Classificatore" DataField="Category_FullSearchComputed" HeaderStyle-Width="25%" UniqueName="Category_FullSearchComputed" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center" AutoPostBackOnFilter="true" SortExpression="Category/Name" CurrentFilterFunction="Contains" FilterControlWidth="87%">
                        <ClientItemTemplate>
                            <label>#=getCategory(Category)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Data registrazione" DataField="RegistrationDate" UniqueName="RegistrationDate" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center" SortExpression="RegistrationDate" DataType="System.DateTime" AllowFiltering="false">
                        <ClientItemTemplate>
                            <label>#=getDate(RegistrationDate)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom"></PagerStyle>
            <FilterMenu OnClientShowing="menuShowing" />
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
   
    <telerik:RadGrid Visible="false" runat="server" ID="intermediateGrid" PageSize="30" GridLines="None" Height="100%"
        AllowPaging="True" AllowMultiRowSelection="True" AllowFilteringByColumn="False" ExportSettings-ExportOnlyData="true">
        <ClientSettings>
            <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
            <Resizing AllowColumnResize="true" />
            <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
        </ClientSettings>
        <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed" NoMasterRecordsText="Nessun registro monitoraggio trasparenza trovato."
            PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
            <Columns>
                <telerik:GridBoundColumn Visible="false" />
            </Columns>
        </MasterTableView>
        <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true"></PagerStyle>
    </telerik:RadGrid>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnDocuments" Text="Visualizza documenti" AutoPostBack="false" OnClientClicked="btnDocumentsClick"></telerik:RadButton>
    <telerik:RadButton ID="btnSelectAll" runat="server" AutoPostBack="false" Width="120px" Text="Seleziona tutti" OnClientClicked="btnSelectAll"></telerik:RadButton>
    <telerik:RadButton ID="btnDeselectAll" runat="server" AutoPostBack="false" Width="120px" Text="Annulla selezione" OnClientClicked="btnDeselectAll"></telerik:RadButton>
    <telerik:RadButton ID="hiddenButton" runat="server" Visible="true" CssClass="hiddenButtonClass"/>
    <input type="hidden" id="dgvUDSItems" name="dgvUDSItems" value='<%= dgvUDSItems %>' />
    <input type="hidden" id="dgvColumns" name="dgvColumns" value='<%= dgvColumns %>' />
    <input type="hidden" id="dgvUDSAllItems" name="dgvUDSAllItems" value='<%= dgvUDSAllItems %>' />
</asp:Content>