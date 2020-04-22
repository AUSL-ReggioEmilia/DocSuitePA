<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Base.Master" CodeBehind="DesignerResults.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DesignerResults" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHead">
    <link type="text/css" rel="Stylesheet" href="Content/bootstrap.css" />
    <link type="text/css" rel="Stylesheet" href="Content/nprogress.css" />
    <link type="text/css" rel="Stylesheet" href="Content/site.css" />
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

        html span.RadInput{
            height: auto !important;
        }
    </style>
    <script type="text/javascript" src="../Scripts/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="Scripts/rivets.bundled.min.js"></script>
    <script type="text/javascript" src="../Scripts/moment.js"></script>
    <script type="text/javascript" src="Scripts/nprogress.js"></script>
    <script type="text/javascript" src="Scripts/App/Service/PathService.js"></script>

    <script type="text/javascript" src="Scripts/App/Controllers/DesignerResultsController.js"></script>

    <script type="text/javascript" src="Scripts/App/utils.js"></script>
    <script type="text/javascript" src="Scripts/App/main.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var controller = null;

            function menuShowing(sender, args) {
                if (controller == null || controller == undefined) controller = new UdsDesigner.DesignerResultsController();
                controller.menuShowing(sender, args);
            }

            function filterMenuShowing(sender, args) {
                if (controller == null || controller == undefined) controller = new UdsDesigner.DesignerResultsController();
                controller.filterMenuShowing(sender, args);
            }

            function resizeGrid(sender, args) {
                var masterTable = $find("<%= dgvRepositories.ClientID %>").get_masterTableView();
                masterTable.rebind();
            }
        </script>
    </telerik:RadCodeBlock>
    <div class="navbar navbar-default navbar-fixed-top">
        <div class="container-fluid">
            <!-- Brand and toggle get grouped for better mobile display -->
            <div class="navbar-header">
                <a class="navbar-brand" href="#">Risultati</a>
            </div>
            <!-- /.navbar-collapse -->
        </div>
        <!-- /.container-fluid -->
    </div>

    <div class="alert alert-danger alert-dismissible collapse" id="alertErrorMessage" role="alert">
        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <strong>Errore!</strong> E' avvenuto un errore durante la richiesta: { message }
    </div>

    <div class="radGridWrapper">
        <telerik:RadGrid AutoGenerateColumns="false" ID="dgvRepositories" Skin="Bootstrap" AllowFilteringByColumn="True"
            AllowSorting="True" AllowPaging="True" Width="100%" Height="100%" runat="server">
            <ClientSettings>
                <DataBinding Location="DesignerService.aspx" SelectMethod="LoadRepositories" SortParameterType="List" FilterParameterType="List" EnableCaching="true" />
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="false" />
                <ClientEvents OnFilterMenuShowing="filterMenuShowing" OnColumnResizing="resizeGrid" />
            </ClientSettings>
            <MasterTableView AutoGenerateColumns="false" Width="100%" TableLayout="fixed" AllowFilteringByColumn="True" PagerStyle-PageSizeLabelText="Elementi per pagina:"
                PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
                <Columns>
                    <telerik:GridBoundColumn HeaderStyle-Width="25%" DataField="Name" UniqueName="Name" HeaderText="Nome"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" FilterControlWidth="150px">
                    </telerik:GridBoundColumn>
                    <telerik:GridNumericColumn DataField="Version" UniqueName="Version" HeaderText="Versione" FilterControlWidth="50px" HeaderStyle-Width="10%"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="EqualTo">
                    </telerik:GridNumericColumn>
                    <telerik:GridDateTimeColumn HeaderStyle-Width="20%" DataField="ActiveDate" UniqueName="ActiveDate" HeaderText="Data attivazione"
                        PickerType="DatePicker" AutoPostBackOnFilter="true" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn HeaderStyle-Width="20%" DataField="ExpiredDate" UniqueName="ExpiredDate" HeaderText="Data disattivazione"
                        PickerType="DatePicker" AutoPostBackOnFilter="true" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="Stato" UniqueName="Status">
                        <FilterTemplate>
                            <telerik:RadComboBox ID="RadComboBoxCountry" Skin="Bootstrap" EmptyMessage="Seleziona uno stato" runat="server" DataTextField="Text"
                                DataValueField="Value" Width="100%" EnableLoadOnDemand="true" EnableVirtualScrolling="true" OnClientSelectedIndexChanged="StateIndexChanged">
                                <WebServiceSettings Method="LoadStatuses" Path="DesignerService.aspx" />
                            </telerik:RadComboBox>
                            <telerik:RadScriptBlock runat="server">
                                <script type="text/javascript">
                                    function StateIndexChanged(sender, args) {
                                        var tableView = $find("<%# dgvRepositories.ClientID %>").get_masterTableView();
                                        tableView.filter("Status", args.get_item().get_value(), "EqualTo");
                                    }
                                </script>
                            </telerik:RadScriptBlock>
                        </FilterTemplate>
                        <ClientItemTemplate>
                            <span class="label label-#=UdsDesigner.DesignerResultsController.getStatusCssClass(Status)#">#=UdsDesigner.DesignerResultsController.setStatus(Status)#</span>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderStyle-Width="10%" AllowFiltering="false">
                        <ClientItemTemplate>
                            <input type="button" class="btn btn-primary" onclick="window.location.href = '#= String.format(UdsDesigner.DesignerResultsController.designerUrlFormat, Id)#'" value="Visualizza" />
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom"></PagerStyle>
            <FilterMenu OnClientShowing="menuShowing" />
        </telerik:RadGrid>
    </div>

</asp:Content>
