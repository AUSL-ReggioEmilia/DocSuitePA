<%@ Page Title="Template di collaborazione - risultati" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltTemplateCollaborationManager.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTemplateCollaborationManager" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var tbltTemplateCollaborationManager;
            require(["Tblt/TbltTemplateCollaborationManager"], function (TbltTemplateCollaborationManager) {
                $(function () {
                    tbltTemplateCollaborationManager = new TbltTemplateCollaborationManager(tenantModelConfiguration.serviceConfiguration);
                    tbltTemplateCollaborationManager.btnNewId = "<%= btnNew.ClientID %>";
                    tbltTemplateCollaborationManager.btnDeleteId = "<%= btnDelete.ClientID %>";
                    tbltTemplateCollaborationManager.grdTemplateCollaborationId = "<%= grdTemplateCollaboration.ClientID %>";
                    tbltTemplateCollaborationManager.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    tbltTemplateCollaborationManager.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    tbltTemplateCollaborationManager.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    tbltTemplateCollaborationManager.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltTemplateCollaborationManager.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
     <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <div class="radGridWrapper">
        <DocSuite:BindGrid ID="grdTemplateCollaboration" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
            <MasterTableView TableLayout="Fixed" ClientDataKeyNames="Entity.UniqueId" AllowFilteringByColumn="true" GridLines="Both">
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="5%" HeaderStyle-CssClass="headerImage" />
                    <telerik:GridTemplateColumn UniqueName="Entity.Status" CurrentFilterFunction="EqualTo" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/detail_page_item_template.png" HeaderTooltip="Stato Template" Groupable="false" AllowSorting="True">
                        <HeaderStyle HorizontalAlign="Center" Width="200px" />
                        <ItemStyle HorizontalAlign="Center" />
                        <FilterTemplate>
                            <telerik:RadComboBox  runat="server" ID="cmbStatus" Width="100%" AutoPostBack="True" OnSelectedIndexChanged="cmbStatus_SelectedIndexChanged">
                                <Items>
                                    <telerik:RadComboBoxItem Text="" Value="" Selected="true" />
                                    <telerik:RadComboBoxItem Text="Bozza" Value="0" />
                                    <telerik:RadComboBoxItem Text="Attive" Value="1" />
                                    <telerik:RadComboBoxItem Text="Non attive" Value="2" />
                                </Items>
                            </telerik:RadComboBox>
                        </FilterTemplate>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgTemplateStatus"></asp:Image>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Entity.Name" DataField="Entity.Name" HeaderText="Descrizione" AllowFiltering="True" CurrentFilterFunction="Contains" Groupable="false" AllowSorting="True" SortExpression="Entity.Name">
                        <HeaderStyle HorizontalAlign="Center" Width="37%" />
                        <ItemStyle HorizontalAlign="Left" Width="37%" />
                        <ItemTemplate>
                            <asp:HyperLink runat="server" ID="lblNameLink"></asp:HyperLink>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn UniqueName="RegistrationDate" DataField="Entity.RegistrationDate" HeaderText="Data registrazione" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}" SortExpression="Entity.RegistrationDate">
                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn UniqueName="RegistrationUser" DataField="Entity.RegistrationUser" HeaderText="Utente Registrazione" AllowFiltering="false" Groupable="false" SortExpression="Entity.RegistrationUser">
                        <HeaderStyle HorizontalAlign="Center" Width="15%" />
                        <ItemStyle HorizontalAlign="Center" Width="15%" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn UniqueName="LastChangedDate" DataField="Entity.LastChangedDate" HeaderText="Data Ultima Modifica" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}" SortExpression="Entity.RegistrationUser">
                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn UniqueName="LastChangedUser" DataField="Entity.LastChangedUser" HeaderText="Utente Ultima Modifica" AllowFiltering="false" Groupable="false" SortExpression="Entity.RegistrationUser">
                        <HeaderStyle HorizontalAlign="Center" Width="15%" />
                        <ItemStyle HorizontalAlign="Center" Width="15%" />
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
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnNew" Width="150px" Text="Nuovo" AutoPostBack="false"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnDelete" Width="150px" Text="Elimina" AutoPostBack="false"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
