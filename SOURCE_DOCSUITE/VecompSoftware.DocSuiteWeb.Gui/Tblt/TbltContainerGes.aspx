<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContainerGes" CodeBehind="TbltContainerGes.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltUDSRepositoriesTypologyGes;
            require(["Tblt/TbltContainerGes"], function (TbltUDSRepositoryTypologyGes) {
                $(function () {
                    tbltUDSRepositoriesTypologyGes = new TbltUDSRepositoryTypologyGes(tenantModelConfiguration.serviceConfiguration);
                    tbltUDSRepositoriesTypologyGes.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    tbltUDSRepositoriesTypologyGes.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltUDSRepositoriesTypologyGes.btnSearchId = "<%=btnSearch.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltUDSRepositoriesTypologyGes.pageContentId = "<%= pnlContent.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.txtNameId = "<%= txtName.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.idCategory = "<%= IdUCategory %>";
                    tbltUDSRepositoriesTypologyGes.grdUDSRepositoriesId = "<%= grdContainers.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <asp:Panel runat="server" ID="pnlContent">
        <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
            <Rows>
                <telerik:LayoutRow Style="margin-top: 5px;" runat="server" ID="rowName">
                    <Columns>
                        <telerik:LayoutColumn Span="3" Height="30px">
                            <b>Nome contenitore:</b>
                        </telerik:LayoutColumn>
                        <telerik:LayoutColumn Span="7">
                            <telerik:RadTextBox ID="txtName" EmptyMessage="Nome contenitore" runat="server" Width="200px"></telerik:RadTextBox>
                        </telerik:LayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
                <telerik:LayoutRow>
                    <Columns>
                        <telerik:LayoutColumn>
                            <telerik:RadButton ID="btnSearch" Text="Ricerca" Width="80px" runat="server" TabIndex="1" AutoPostBack="False" CssClass="button" />
                        </telerik:LayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
        <asp:Panel runat="server">
            <telerik:RadGrid runat="server" ID="grdContainers" AutoGenerateColumns="False" Style="margin-top: 2px;" AllowMultiRowSelection="true" GridLines="Both" ItemStyle-BackColor="LightGray">
                <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="Id" AllowFilteringByColumn="false" GridLines="Both" Width="100%" NoMasterRecordsText="Nessun contenitore presente">
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                        </telerik:GridClientSelectColumn>
                        <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Contenitore" AllowFiltering="false" AllowSorting="true" Groupable="false">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ClientItemTemplate>
                        <div class="dsw-text-left">      
                            <span>#=Name#</span>
                        </div>                                            
                            </ClientItemTemplate>
                        </telerik:GridTemplateColumn>

                    </Columns>
                </MasterTableView>
                <ClientSettings>
                    <Selecting AllowRowSelect="true" />
                </ClientSettings>
            </telerik:RadGrid>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConfirm" runat="server" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
</asp:Content>
