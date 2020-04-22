<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltUDSTypology" CodeBehind="TbltUDSTypology.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Tipologie" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltUDSTypology;
            require(["Tblt/TbltUDSTypology"], function (TbltUDSTypology) {
                $(function () {
                    tbltUDSTypology = new TbltUDSTypology(tenantModelConfiguration.serviceConfiguration);
                    tbltUDSTypology.pnlDetailsId = "<%= pnlDetails.ClientID %>";
                    tbltUDSTypology.pnlInformationsId = "<%= pnlInformations.ClientID %>";
                    tbltUDSTypology.pnlUDSRepositoriesId = "<%= pnlUDSRepositories.ClientID %>";                    
                    tbltUDSTypology.pnlButtonsId = "<%= pnlButtons.ClientID %>";
                    tbltUDSTypology.toolBarSearchId = "<%= ToolBarSearch.ClientID %>";
                    tbltUDSTypology.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltUDSTypology.rtvTypologyId = "<%= rtvTypology.ClientID %>";
                    tbltUDSTypology.windowAddUDSTypologyId = "<%= windowAddUDSTypology.ClientID %>";
                    tbltUDSTypology.paneSelectionId = "<%= paneSelection.ClientID %>";
                    tbltUDSTypology.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltUDSTypology.lblStatusId = "<%=lblStatus.ClientID%>";
                    tbltUDSTypology.lblActiveFromId = "<%=lblActiveFrom.ClientID%>";
                    tbltUDSTypology.grdUDSRepositoriesId = "<%=grdUDSRepositories.ClientID%>";
                    tbltUDSTypology.btnAggiungiId = "<%= btnAggiungi.ClientID%>";
                    tbltUDSTypology.btnRimuoviId = "<%= btnElimina.ClientID%>";
                    tbltUDSTypology.folderToolBarId = "<%= FolderToolBar.ClientID %>";
                    tbltUDSTypology.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerUDSTypology" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowAddUDSTypology" runat="server" />
        </Windows>
    </telerik:RadWindowManager>


    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" ResizeWithParentPane="False" Height="100%">
            <telerik:RadPane runat="server" ID="paneSelection" Width="50%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Height="100%" Width="100%" Orientation="Horizontal">
                    <telerik:RadPane runat="server" Height="100%" Width="100%" Scrolling="Y">
                        <%--OnButtonClick=""--%>
                        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchDescription">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtDescription" EmptyMessage="Descrizione" runat="server" Width="170px"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Disattive" CheckOnClick="true" Group="Disabled" Checked="false" Value="Inactive" PostBack="false"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Attive" CheckOnClick="true" Checked="true" Group="Active" Value="Active" PostBack="false"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%">
                        <Items>
                            <telerik:RadToolBarButton ToolTip="Aggiungi" Value="create" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                            <telerik:RadToolBarButton ToolTip="Modifica" Value="modify" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />                     
                        </Items>
                    </telerik:RadToolBar>
                        
                        <telerik:RadTreeView ID="rtvTypology" runat="server" Style="margin-top: 10px;" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" Selected="true" runat="server" Font-Bold="true" Text="Tipologie" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>

            <telerik:RadPane runat="server" Width="50%">
                <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel" Style="margin-top: 5px;">
                    <div class="dsw-panel-content">
                        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                            <Items>
                                <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlInformations">
                                            <div class="col-dsw-10">
                                                <b>Stato:</b>
                                                <asp:Label runat="server" ID="lblStatus"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Data creazione:</b>
                                                <asp:Label runat="server" ID="lblActiveFrom"></asp:Label>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                                <telerik:RadPanelItem Text="Archivi" Expanded="true">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlUDSRepositories">
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
                                                                    <span>#=Name+' ('+Alias+')'#</span>
                                                                </div>                                            
                                                            </ClientItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn UniqueName="publishDate" HeaderText="Data pubblicazione" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                            <HeaderStyle HorizontalAlign="Left" Width="20%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                            <ClientItemTemplate>
                                                                <div class="dsw-text-left">      
                                                                    <span>#=moment(ActiveDate).format("DD/MM/YYYY")#</span>
                                                                </div>                                            
                                                            </ClientItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn UniqueName="Version" HeaderText="Versione" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                            <HeaderStyle HorizontalAlign="Left" Width="8%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                            <ClientItemTemplate>
                                                                <div class="dsw-text-left">      
                                                                    <span>#=Version#</span>
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
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                        </telerik:RadPanelBar>
                    </div>                   
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlButtons">
                    <telerik:RadButton ID="btnAggiungi" AutoPostBack="false" runat="server" Text="Aggiungi" />
                    <telerik:RadButton ID="btnElimina" AutoPostBack="false" runat="server" Text="Elimina" />
                </asp:Panel>
            </telerik:RadPane>

        </telerik:RadSplitter>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
        <telerik:RadButton ID="btnDelete" runat="server" Text="Elimina" AutoPostBack="false" Visible="false" />
</asp:Content>


