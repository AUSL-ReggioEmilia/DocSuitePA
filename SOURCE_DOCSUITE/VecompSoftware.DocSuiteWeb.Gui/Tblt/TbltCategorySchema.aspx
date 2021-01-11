<%@ Page Title="Versione Classificatore" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltCategorySchema.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltCategorySchema" %>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var tbltCategorySchema;
            require(["Tblt/TbltCategorySchema"], function (TbltCategorySchema) {
                $(function () {
                    tbltCategorySchema = new TbltCategorySchema();
                    tbltCategorySchema.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    tbltCategorySchema.radWindowManagerId = "<%= RadWindowManager.ClientID %>";
                    tbltCategorySchema.grdCategorySchemaId = "<%= grdCategorySchema.ClientID %>";
                    tbltCategorySchema.btnEditId = "<%= btnEdit.ClientID %>";
                    tbltCategorySchema.btnDeleteId = "<%= btnDelete.ClientID %>";
                    tbltCategorySchema.initialize();
                });
        });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="rwEdit" runat="server" Title="Gestione Versione" />
        </Windows>
    </telerik:RadWindowManager>

    <div class="radGridWrapper">
        <DocSuite:BindGrid ID="grdCategorySchema" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
            <MasterTableView TableLayout="Auto" DataKeyNames="Id" ClientDataKeyNames="Id" AllowFilteringByColumn="true" GridLines="Both">
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                    </telerik:GridClientSelectColumn>
                    <telerik:GridBoundColumn UniqueName="Version" HeaderText="Versione" DataField="Version" AllowFiltering="false" AllowSorting="true" Groupable="false" SortExpression="Version">
                        <HeaderStyle HorizontalAlign="Center" Width="65px" />
                        <ItemStyle HorizontalAlign="Center" Width="65px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Note" HeaderText="Note" DataField="Note" AllowFiltering="false" Groupable="false" AllowSorting="True">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" Height="40px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn UniqueName="StartDate" DataField="StartDate" HeaderText="Data di attivazione" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Center" Width="160px" />
                        <ItemStyle HorizontalAlign="Center" Width="160px" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn UniqueName="EndDate" DataField="EndDate" HeaderText="Data di Disattivazione" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Center" Width="160px" />
                        <ItemStyle HorizontalAlign="Center" Width="160px" />
                    </telerik:GridDateTimeColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="true" />
            </ClientSettings>
        </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <div class="footer-buttons-wrapper">
        <telerik:RadButton Width="150" runat="server" ID="btnEdit" Text="Modifica" />
        <telerik:RadButton Width="150" runat="server" ID="btnDelete" Text="Elimina" />        
    </div>
</asp:Content>
