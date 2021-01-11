<%@ Page AutoEventWireup="false" CodeBehind="TbltClassificatore.aspx.vb" EnableViewState="True" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltClassificatore" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Classificatore" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscFasciclePlan.ascx" TagName="uscFasciclePlan" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCustomActionsRest.ascx" TagName="uscCustomActionsRest" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <style type="text/css">
            div.RadGrid_Office2007 input {
                background-color: white;
            }
        </style>
        <script type="text/javascript">
            var tbltClassificatore;
            require(["Tblt/TbltClassificatore"], function (TbltClassificatore) {
                tbltClassificatore = new TbltClassificatore(tenantModelConfiguration.serviceConfiguration);
                tbltClassificatore.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                tbltClassificatore.treeViewCategoryId = "<%= rtvCategories.ClientID %>";
                tbltClassificatore.splPageID = "<%= splPage.ClientID %>";
                tbltClassificatore.radWindowManagerId = "<%= RadWindowManager.ClientID %>";
                tbltClassificatore.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                tbltClassificatore.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                tbltClassificatore.uscFasciclePlanId = "<%= uscFascPlan.PageContentDiv.ClientID %>";
                tbltClassificatore.pnlDetailsId = "<%= pnlDetails.ClientID%>";
                tbltClassificatore.pnlFasciclePlanId = "<%= pnlFasciclePlan.ClientID%>";
                tbltClassificatore.pnlSettoriId = "<%= pnlSettori.ClientID%>";
                tbltClassificatore.lblMetadataId = "<%= lblMetadata.ClientID%>";
                tbltClassificatore.fascicleContainerEnabled = <%= ProtocolEnv.FascicleContainerEnabled.ToString().ToLower() %>;
                tbltClassificatore.metadataRepositoryEnabled = JSON.parse("<%=ProtocolEnv.MetadataRepositoryEnabled%>".toLowerCase());
                tbltClassificatore.actionsToolbarId = "<%= ActionsToolbar.ClientID %>";
                tbltClassificatore.uscCustomActionsRestId = "<%= uscCustomActionsRest.PageContent.ClientID %>";
                tbltClassificatore.btnUpdateCustomActionsId = "<%= btnUpdateCustomActions.ClientID %>";
                tbltClassificatore.initialize();
            });

            function RenameCallback(existGroups, node) {
                tbltClassificatore.renameCallback(existGroups, node);
            }

            function OnContextMenuShowing(sender, args) {
                tbltClassificatore.onContextMenuShowing(sender, args);
            }

            function OnContextMenuItemClicked(sender, args) {
                tbltClassificatore.onContextMenuItemClicked(sender, args);
            }

            function ReloadNodesCallback() {
                tbltClassificatore.updateCategoriesCallback();
            }

            function treeView_ClientNodeClicked(sender, args) {
                tbltClassificatore.treeView_ClientNodeClicked(sender, args);
            }

            function treeView_ClientNodeExpanding(sender, args) {
                tbltClassificatore.treeView_ClientNodeExpanding(sender, args);
            }

            function treeView_ClientNodeExpanded(sender, args) {
                tbltClassificatore.treeView_ClientNodeExpanded(sender, args);
            }

            function UpdateVisibility(visibility) {
                tbltClassificatore.updateVisibility(JSON.parse(visibility.toLowerCase()));
            }

            function sendAjaxRequest(action) {
                tbltClassificatore.sendAjaxRequest(action);
            }

            function runFasciclePlan(sender, args) {
                tbltClassificatore.runFasciclePlan();
            }
            function closeFasciclePlan(sender, args) {
                tbltClassificatore.closeFasciclePlan();
            }

        </script>
         <style>
            #ctl00_cphContent_rtvCategories {
                height: 90%;
            }
        </style>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="rwEdit" runat="server" Title="Gestione Classificatore" />
            <telerik:RadWindow ID="rwLog" runat="server" Title="Classificatore Log" />
            <telerik:RadWindow ID="rwAddMassimario" runat="server" Title="Associa Massimario di scarto" />
            <telerik:RadWindow ID="rwMetadata" runat="server" Title="Associa metadati" />
        </Windows>
    </telerik:RadWindowManager>
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" Orientation="Horizontal" ID="splPage">
            <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" ID="splContent">
                    <telerik:RadPane runat="server" Width="50%">
                        <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" OnButtonClick="ToolBarSearch_ButtonClick" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchDescription">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtSearchCategory" EmptyMessage="Classificatore" runat="server" Width="170px"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Value="searchCode">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtSearchCategoryCode" EmptyMessage="Codice" runat="server" Width="60px"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Piano fascicolazione" CheckOnClick="true" Checked="false" Value="searchOnlyFascicolable" PostBack="false"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" OnButtonClick="ToolBarStatus_ButtonClick" EnableShadows="False" ID="ToolBarStatus" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton>
                                    <ItemTemplate>
                                        <label>Stato del classificatore:</label>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Disattivi" CheckOnClick="true" Group="Disabled" Checked="false" Value="searchDisabled" PostBack="true"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Attivi" CheckOnClick="true" Checked="true" Group="Active" Value="searchActive" PostBack="true"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Value="searchSchema" CheckOnClick="true" PostBack="true">
                                    <ItemTemplate>
                                        <label>Versione:</label>
                                        <telerik:RadComboBox runat="server" ID="rcbSchemas" EmptyMessage="-- Versione corrente" AutoPostBack="true" OnSelectedIndexChanged="rcbSchemas_SelectedIndexChanged"></telerik:RadComboBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ActionsToolbar" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton ID="btnAggiungi" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" AutoPostBack="false" CommandName="AddCategory" ToolTip="Aggiungi classificatore" Text="Aggiungi"/>
                                <telerik:RadToolBarButton ID="btnModifica" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" AutoPostBack="false" CommandName="EditCategory" ToolTip="Modifica classificatore" Enabled="false" Text="Modifica"/>
                                <telerik:RadToolBarButton ID="btnElimina" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" AutoPostBack="false" CommandName="DeleteCategory" ToolTip="Elimina classificatore" Enabled="false" Text="Elimina"/>
                                <telerik:RadToolBarButton ID="btnMassimari" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/pencil_add.png" AutoPostBack="false" CommandName="AddMassimario" ToolTip="Associa massimario di scarto" Enabled="false" Text="Scarto"/>
                                <telerik:RadToolBarButton ID="btnRecovery" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/view_history.png" AutoPostBack="false" CommandName="RecoverCategory" ToolTip="Recupera classificatore" Enabled="false" Text="Recupera"/>
                                <telerik:RadToolBarButton ID="btnMetadata" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/page_add.png" AutoPostBack="false" CommandName="AddMetadata" ToolTip="Associa metadati" Enabled="false" Text="Metadati"/>
                                <telerik:RadToolBarButton ID="btnLog" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/file_extension_log.png" AutoPostBack="false" CommandName="LogCategory" ToolTip="Log" Enabled="false" Text="Log"/>
                                <telerik:RadToolBarButton IsSeparator="true" />
                               <telerik:RadToolBarButton ID="btnRunFasciclePlan" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/fascicle_procedure.png" AutoPostBack="false" CommandName="RunFasciclePlan" ToolTip="Attiva piano di procedimento" Enabled="false" Text="Attiva piano"/>
                               <telerik:RadToolBarButton ID="btnCloseFasciclePlan" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/remove_fascicle.png" AutoPostBack="false" CommandName="CloseFasciclePlan" ToolTip="Annulla piano di procedimento" Enabled="false" Text="Anulla piano"/>
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadTreeView ID="rtvCategories" LoadingStatusPosition="BeforeNodeText" OnClientNodeClicked="treeView_ClientNodeClicked" PersistLoadOnDemandNodes="false" OnClientNodeExpanding="treeView_ClientNodeExpanding" OnClientNodeExpanded="treeView_ClientNodeExpanded" runat="server" Style="margin-top: 10px;" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Classificatore" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </telerik:RadPane>
                    <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>
                    <telerik:RadPane runat="server" Width="50%">
                        <asp:Panel runat="server" ID="pnlInfo" CssClass="dsw-panel" Style="margin-top: 5px;">
                            <div class="dsw-panel-content">
                                <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                                    <Items>
                                        <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                            <ContentTemplate>
                                                <asp:Panel runat="server" ID="pnlInformations">
                                                    <div class="col-dsw-10 dsw-align-center">

                                                        <div class="col-dsw-10">
                                                            <b>Voce:</b>
                                                            <b><asp:Label runat="server" ID="lblCategoryCode" /></b>
                                                            <asp:Label runat="server" ID="lblCategoryName" />
                                                        </div>
                                                    </div>
                                                    <div class="col-dsw-5 dsw-align-left">

                                                        <div class="col-dsw-10">
                                                            <b>Data attivazione:</b>
                                                            <asp:Label runat="server" ID="lblStartDate"></asp:Label>
                                                        </div>
                                                        <div class="col-dsw-10">
                                                            <b>Data disattivazione:</b>
                                                            <asp:Label runat="server" ID="lblEndDate"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="col-dsw-5 dsw-align-left">
                                                        <div class="col-dsw-10" id="metadataDetails" runat="server" visible="false">
                                                            <b>Metadati:</b>
                                                            <asp:Label runat="server" ID="lblMetadata"></asp:Label>
                                                        </div>
                                                        <div class="col-dsw-10">
                                                            <b>Massimario di scarto:</b>
                                                            <asp:Label runat="server" ID="lblMassimarioName"></asp:Label>
                                                        </div>

                                                    </div>
                                                    
                                                    <div class="col-dsw-10 dsw-align-center">
                                                        <div class="col-dsw-10" runat="server" id="divProcedureType">
                                                            <b>Attivato un piano di fascicolazione di procedimento 
                                                                <asp:Label runat="server" ID="lblRegistrationDate" /></b>
                                                        </div>
                                                        <div class="col-dsw-10" runat="server" id="divSubFascicleType">
                                                            <b>Sotto fascicolo</b>
                                                        </div>
                                                        <div class="col-dsw-10" runat="server" id="divNoFasciclePlan">
                                                            <b>Piano di fascicolazione procedimento non presente</b>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                    </Items>
                                </telerik:RadPanelBar>
                            </div>
                        </asp:Panel>                        
                        <asp:Panel runat="server" ID="pnlSettori" CssClass="dsw-panel" Visible="false">
                            <div class="dsw-panel-contenst">
                                <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                                    <Items>
                                        <telerik:RadPanelItem Text="Autorizzazioni" Expanded="true">
                                            <ContentTemplate>
                                                <usc:Settori runat="server" HeaderVisible="false" MultipleRoles="true" ID="uscSettori" Required="False" UseSessionStorage="true" />
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                    </Items>
                                </telerik:RadPanelBar>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlFasciclePlan" CssClass="dsw-panel">
                            <div class="dsw-panel-content">
                                <usc:uscFasciclePlan runat="server" style="height: 100%; width: 100%; border: none 0;" ID="uscFascPlan" EditMode="true" />
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlCustomActions" CssClass="dsw-panel">
                            <div class="dsw-panel-content">
                                <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                                    <Items>
                                        <telerik:RadPanelItem Text="Azioni personalizzate" Expanded="true">
                                            <ContentTemplate>
                                                <usc:uscCustomActionsRest runat="server" ID="uscCustomActionsRest" IsSummary="true" />
                                                <telerik:RadButton runat="server" ID="btnUpdateCustomActions" AutoPostBack="false" Text="Aggiorna" />
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                    </Items>
                                </telerik:RadPanelBar>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel">
                            <div class="dsw-panel-content">
                                <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                                    <Items>
                                        <telerik:RadPanelItem Text="Elenco funzioni dei fascicoli di procedimento" Expanded="true">
                                            <ContentTemplate>
                                                <telerik:RadTreeView BorderStyle="none" CheckBoxes="true" ID="rtvRoleUsers" runat="server" Width="100%" />
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                    </Items>
                                </telerik:RadPanelBar>
                            </div>
                        </asp:Panel>

                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>