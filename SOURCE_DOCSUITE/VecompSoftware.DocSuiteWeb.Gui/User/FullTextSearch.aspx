<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FullTextSearch.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.FullTextSearch" Title="Ricerca Full Text" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock" EnableViewState="false">
        <script type="text/javascript">  

            var fullTextSearch;
            require(["user/FullTextSearch"], function (FullTextSearch) {
                $(function () {
                    fullTextSearch = new FullTextSearch(tenantModelConfiguration.serviceConfiguration);
                    fullTextSearch.btnFullTextSearchId = "<%=btnFullTextSearch.ClientID %>";
                    fullTextSearch.ajaxLoadingPanelId = "<%=MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fullTextSearch.pageContentId = "<%= pageContent.ClientID %>";
                    fullTextSearch.tenantId = "<%= IdTenant%>";
                    fullTextSearch.rgvDocumentListsId = "<%= rgvDocumentLists.ClientID %>";
                    fullTextSearch.actionsToolbarId = "<%= ToolbarActions.ClientID %>";
                    fullTextSearch.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadToolBar AutoPostBack="false"
        CssClass="ToolBarContainer"
        RenderMode="Lightweight"
        EnableRoundedCorners="False"
        EnableShadows="False"
        ID="ToolbarActions" runat="server" Width="100%">
        <Items>
            <telerik:RadToolBarButton Value="toolBarFullText" >
                <ItemTemplate>
                    <telerik:RadTextBox ID="txtFullTextSearch" EmptyMessage="Ricerca Full Text" runat="server" Width="300px"></telerik:RadTextBox>
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton ID="btnFullTextSearch" Value="searchFullText" PostBack="false" Text="Cerca"  CommandName="searchFullText" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
        </Items>
    </telerik:RadToolBar>


    <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow Height="100%" HtmlTag="Div" ID="PanelResult">
                <Content>
                    <telerik:RadGrid ID="rgvDocumentLists" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                        <MasterTableView TableLayout="Auto" AllowFilteringByColumn="false" GridLines="Both">
                            <Columns>
                                <telerik:GridTemplateColumn DataField="Environment" UniqueName="Environment" HeaderText="Documento" HeaderStyle-Width="16px">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="16px" />
                                    <ClientItemTemplate>
                                        <a href="#=ViewerUrl#" >
                                        <img class="dsw-text-center" src="#=IconUrl#" height="16px" width="16px" />
                                        </a>                
                                    </ClientItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn UniqueName="Numero" HeaderText="Numero" HeaderStyle-Width="150px">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ClientItemTemplate>
                                        <a href="#=DocumentUnitUrl#" >
                                        #=DocumentUnit.Number#
                                        </a> </ClientItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn UniqueName="Dataregistrazione" HeaderText="Data registrazione" HeaderStyle-Width="150px">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ClientItemTemplate>
                                         #=RegistrationDate#
                                    </ClientItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn UniqueName="Contenitore" HeaderText="Contenitore" HeaderStyle-Width="250px">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ClientItemTemplate>
                                         #=DocumentUnit.Container.Name#
                                    </ClientItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn UniqueName="Classificatore" HeaderText="Classificatore" HeaderStyle-Width="200px">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ClientItemTemplate>
                                         #=DocumentUnit.Category.Name#
                                    </ClientItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="Subject" UniqueName="Subject" HeaderText="Oggetto">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="16px" />
                                    <ClientItemTemplate>#=DocumentUnit.Subject#</ClientItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings>
                            <Selecting AllowRowSelect="false" />
                        </ClientSettings>
                    </telerik:RadGrid>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
