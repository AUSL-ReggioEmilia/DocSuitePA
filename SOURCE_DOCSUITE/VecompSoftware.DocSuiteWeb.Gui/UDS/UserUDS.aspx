<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UserUDS.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserUDS" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var userUDS;
            require(["UDS/UserUDS"], function (UserUDS) {
                $(function () {
                    userUDS = new UserUDS(tenantModelConfiguration.serviceConfiguration);
                    userUDS.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    userUDS.multiTenantEnabled = "<%= MultiTenantEnabled %>";
                    userUDS.tenantCompanyName = '<%= TenantCompanyName %>';
                    userUDS.currentUser = "<%= CurrentDisplayName %>";
                    userUDS.rcbRepositoryNameId = "<%= rcbRepositoryName.ClientID %>";
                    userUDS.chkAuthorizedId = "<%= chkAuthorized.ClientID %>";
                    userUDS.udsGridId = "<%= udsGrid.ClientID%>";
                    userUDS.btnSearchId = "<%= btnSearch.ClientID %>";
                    userUDS.initialize();
                });
            });

            function showLoadingPanel() {
                var loadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var grid_id = "<%=udsGrid.ClientID%>";
                loadingPanel.show(grid_id);
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    userUDS.onPageChanged();
                }
                if (args.get_commandName() === "Sort") {
                    args.set_cancel(true);
                    userUDS.loadResults(0);
                }
            }

            function OnGridDataBound(sender, args) {
                userUDS.onGridDataBound();
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

            function isFromUDSLink() {
                var isFromUDSLink = "<%= (IsFromUDSLink.HasValue AndAlso IsFromUDSLink.Value) %>";
                return isFromUDSLink == "True";
            }

            function isCopyToPec() {
                var copyToPec = "<%= (CopyToPEC.HasValue AndAlso CopyToPEC.Value) %>";
                return copyToPec == "True";
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadPageLayout runat="server" HtmlTag="None" CssClass="col-dsw-10">
        <Rows>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowAuthorized" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblAuthorized" runat="server" Text="Visualizza archivi:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <asp:CheckBox runat="server" ID="chkAuthorized" Text="Solo autorizzati" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowRepositoryName" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblRepositoryName" runat="server" Text="Filtra per archivio:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <telerik:RadComboBox runat="server" ID="rcbRepositoryName" AutoPostBack="true" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                <Content>
                    <telerik:RadButton ID="btnSearch" runat="server" Text="Cerca" AutoPostBack="false" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="pnlUD" runat="server" Width="100%" Height="100%">

        <telerik:RadGrid runat="server" CssClass="udsInvoiceGrid" ID="udsGrid" Skin="Office2010Blue" GridLines="None" Height="100%" PageSize="10" AllowPaging="True" AllowMultiRowSelection="false"
            AllowFilteringByColumn="False" AllowSorting="false">
            <ClientSettings>
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="true" AllowRowResize="true" />
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
                <Selecting AllowRowSelect="false" />
            </ClientSettings>
            <MasterTableView CommandItemDisplay="None"
                AutoGenerateColumns="False"
                TableLayout="fixed"
                NoMasterRecordsText="Nessun elemento trovato nel periodo indicato."
                PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}"
                AllowMultiColumnSorting="false">
                <Columns>
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
                    <telerik:GridTemplateColumn UniqueName="colViewFascicles" HeaderText="Fascicoli" HeaderStyle-Width="20px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/fascicle_open.png" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemTemplate>
                            <telerik:RadButton ButtonType="LinkButton" Height="16px" Visible="false" ID="cmdViewFascicles" runat="server" Width="16px" />
                        </ItemTemplate>
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
                    <telerik:GridTemplateColumn HeaderText="Data registrazione" DataField="RegistrationDate" UniqueName="RegistrationDate" HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center" SortExpression="RegistrationDate" DataType="System.DateTime" AllowFiltering="false">
                        <ClientItemTemplate>
                            <label>#=getDate(RegistrationDate)#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true" PageSizeControlType="None"></PagerStyle>

        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
