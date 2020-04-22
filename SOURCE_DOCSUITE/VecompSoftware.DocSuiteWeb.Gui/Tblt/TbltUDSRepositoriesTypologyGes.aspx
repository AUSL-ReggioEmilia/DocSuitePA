<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltUDSRepositoriesTypologyGes" CodeBehind="TbltUDSRepositoriesTypologyGes.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltUDSRepositoriesTypologyGes;
            require(["Tblt/TbltUDSRepositoriesTypologyGes"], function (TbltUDSRepositoryTypologyGes) {
                $(function () {
                    tbltUDSRepositoriesTypologyGes = new TbltUDSRepositoryTypologyGes(tenantModelConfiguration.serviceConfiguration);
                    tbltUDSRepositoriesTypologyGes.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    tbltUDSRepositoriesTypologyGes.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltUDSRepositoriesTypologyGes.currentUDSTypologyId = "<%= IdUDSTypology%>";
                    tbltUDSRepositoriesTypologyGes.rdlContainerId = "<%=rdlContainer.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.btnSearchId = "<%=btnSearch.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltUDSRepositoriesTypologyGes.pageContentId = "<%= pnlContent.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.txtNameId = "<%= txtName.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.txtAliasId = "<%= txtAlias.ClientID%>";
                    tbltUDSRepositoriesTypologyGes.grdUDSRepositoriesId = "<%= grdUDSRepositories.ClientID%>";                    
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
                        <b>Nome Archivio:</b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="7">
                        <telerik:RadTextBox ID="txtName" EmptyMessage="Nome Archivio" runat="server" Width="200px"></telerik:RadTextBox>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-top: 5px;" runat="server" ID="rowAlias">
                <Columns>
                    <telerik:LayoutColumn Span="3" Height="30px">
                        <b>Alias Archivio:</b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="7">
                        <telerik:RadTextBox ID="txtAlias" EmptyMessage="Alias Archivio" runat="server" Width="200px" MaxLength="4"></telerik:RadTextBox>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn Span="3">
                        <b>Contenitore:</b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="7">
                        <telerik:RadDropDownList runat="server" ID="rdlContainer" Width="200px" AutoPostBack="False" selected="true" DropDownHeight="200px">
                            <Items>
                                <telerik:DropDownListItem Text="" Value="" />
                            </Items>
                </telerik:RadDropDownList>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn>
                        <telerik:RadButton ID="btnSearch" Text="Ricerca" Width="80px" runat="server" TabIndex="1" AutoPostBack="False" CssClass="button"/>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <asp:Panel runat="server">
        <telerik:RadGrid runat="server" ID="grdUDSRepositories" AutoGenerateColumns="False" Style="margin-top: 2px;" AllowMultiRowSelection="true" GridLines="Both" ItemStyle-BackColor="LightGray">
            <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="Id" AllowFilteringByColumn="false" GridLines="Both" Width="100%" NoMasterRecordsText="Nessun archivio presente">
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                    </telerik:GridClientSelectColumn>
                    <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Archivio" AllowFiltering="false" AllowSorting="true" Groupable="false">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ClientItemTemplate>
                        <div class="dsw-text-left">      
                            <span>#=Name#</span>
                        </div>                                            
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Alias" HeaderText="Alias" AllowFiltering="false" AllowSorting="true" Groupable="false">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ClientItemTemplate>
                        <div class="dsw-text-left">      
                            <span>#=Alias#</span>
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

