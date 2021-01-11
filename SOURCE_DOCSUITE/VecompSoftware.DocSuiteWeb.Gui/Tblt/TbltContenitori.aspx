<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContenitori" CodeBehind="TbltContenitori.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Contenitori" %>
<%@ Register Src="~/UserControl/uscContainerExtGes.ascx" TagName="ContainerExtGes" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContainerDossierOptions.ascx" TagName="uscContainerDossierOptions" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContainerConstraintOptions.ascx" TagName="uscContainerConstraintOptions" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <style type="text/css">
            div.RadGrid_Office2007 input {
                background-color: white;
            }
        </style>
        <script type="text/javascript">
            var tbltContenitori;
            require(["Tblt/TbltContenitori"], function (TbltContenitori) {
                $(function () {
                    tbltContenitori = new TbltContenitori(tenantModelConfiguration.serviceConfiguration);
                    tbltContenitori.active = "<% = active.Checked %>";

                    tbltContenitori.initialize();
                });
            });
            //Apre la finestra di editazione di un nodo (aggiunta,eliminazione,modifica)
            function OpenEditWindow(name, operation) {
                var parameters = "Action=" + operation;

                var treeView = $find("<%= rtvContainers.ClientID%>");
                var selectedNode = treeView.get_selectedNode();
                if (operation === "Rename" || operation === "Delete" || operation === "Modify" || operation === "Recovery") {
                    if (!selectedNode) {
                        radalert("Selezionare un contenitore.", 100, 50, "Attenzione");
                        return null;
                    }
                    parameters += "&IdContainer=" + selectedNode.get_value();
                }

                var url = "../Tblt/TbltContenitoriGes.aspx?Type=Comm&" + parameters;

                return OpenWindow(url, name, WIDTH_EDIT_WINDOW, HEIGHT_EDIT_WINDOW + 45);
            }

            //Apre la finestra con i Log per il nodo selezionato
            function OpenLogWindow(name) {
                var treeView = $find("<%= rtvContainers.ClientID%>");
                var selectedNode = treeView.get_selectedNode();
                var url = "../Tblt/TbltLog.aspx?Type=Comm&TableName=ContainerGroup";
                if (selectedNode != null) {
                    var attributes = selectedNode.get_attributes();
                    url += "&entityUniqueId=".concat(attributes.getAttribute("UniqueId"));
                }

                return OpenWindow(url, name, WIDTH_LOG_WINDOW, HEIGHT_LOG_WINDOW);
            }

            //Apre la finestra di stampa per il nodo selezione
            function OpenPrintWindow(name) {
                var treeView = $find("<%= rtvContainers.ClientID%>");
                var selectedNode = treeView.get_selectedNode();
                return OpenWindow("../Comm/CommPrint.aspx?Type=Comm&PrintName=SingleContainerPrint&IdRef=" + selectedNode.get_value(), name, WIDTH_PRINT_WINDOW, HEIGHT_PRINT_WINDOW);
            }

            //Apre la finestra di selezione Gruppi
            function OpenGroupsWindow(name, env) {
                var treeView = $find("<%= rtvContainers.ClientID%>");

                var selectedNode = treeView.get_selectedNode();
                var url = "../Tblt/TbltContenitoriGesGruppi.aspx?Type=Comm&idContainer=" + selectedNode.get_value();

                var nodeType = selectedNode.get_attributes().getAttribute("NodeType");
                if (nodeType === "Group") {
                    url += "&GroupName=" + selectedNode.get_text();
                }

                env = !env ? "" : env;

                url += "&Environment=" + env;
                url += "&Active=" + tbltContenitori.active;

                return OpenWindow(url, name, WIDTH_EDIT_WINDOW, 500);
            }

            //Apre la finestra per settare le proprietà del contenitore legate alle fatture
            function OpenPropertiesWindow(name) {
                var treeView = $find("<%= rtvContainers.ClientID%>");
                var selectedNode = treeView.get_selectedNode();
                if (selectedNode && selectedNode.get_value()) {
                    var url = "../Tblt/TbltContenitoriGesProprieta.aspx?Type=Comm&idContainer=" + selectedNode.get_value();
                    return OpenMaxWindow(url, name, WIDTH_EDIT_WINDOW, 500);
                }
            }

            //Apre una finestra generale
            function OpenWindow(url, name, width, height) {
                var manager = $find("<%=RadWindowManagerContainers.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }

            //Apre una finestra generale di grandezza massima
            function OpenMaxWindow(url, name) {
                var manager = $find("<%=RadWindowManagerContainers.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.maximize();
                return false;
            }

            

            // Chiamata dopo la chiusura della finestra di modifica di un contenitore
            function CloseEdit(sender, args) {
                if (args.get_argument() !== null) {
                    UpdateGroups(args.get_argument());
                }
                
            }

            function CloseGroup(sender, args) {
                UpdateGroups(null);
            }

            function UpdateGroups() {
                UpdateGroups(null);
            }

            function UpdateGroups(value) {
                var args;
                if (!value) {
                    args = 'Update';
                } else {
                    args = value.Operation + "|" + value.Name + "|" + value.ID;
                }
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                ajaxManager.ajaxRequest(args);
            }

            function OpenEditWindowUsers(name, groupName) {
                var url = "../Comm/SelUsers.aspx?Type=Comm&GroupName=" + groupName;
                return OpenWindow(url, name, WIDTH_LOG_WINDOW, HEIGHT_LOG_WINDOW);
            }

            function CloseAddUserInGroup(sender, args) {
                if (args.get_argument() !== null) {
                    var userSelected = args.get_argument();
                    var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                    ajaxManager.ajaxRequest("AddUser|" + userSelected);
                }
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>



<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerContainers" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowEditContainers" OnClientClose="CloseEdit" runat="server" />
            <telerik:RadWindow ID="windowAddUsers" OnClientClose="CloseAddUserInGroup" Title="Aggiungi utente" runat="server" />
            <telerik:RadWindow ID="windowLogContainers" runat="server" />
            <telerik:RadWindow ID="windowGroupContainers" OnClientClose="CloseGroup" runat="server" Title="Gestione Gruppi Contenitore" />
            <telerik:RadWindow ID="windowPrintContainers" runat="server" Title="Stampa Contenitore" />
            <telerik:RadWindow Height="480" ID="windowSelSettori" ReloadOnShow="false" runat="server" Title="Selezione Settori" Width="640" />
        </Windows>
    </telerik:RadWindowManager>
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" ID="splPage">
            <telerik:RadPane runat="server" Height="100%" Width="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" ID="splContent">

                    <telerik:RadPane runat="server">
                        <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton Value="searchDescription">
                            <ItemTemplate>
                                <telerik:RadTextBox ID="txtSearchContainer" EmptyMessage="Contenitore" runat="server" Width="130px"></telerik:RadTextBox>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton Value="btnEnvironments">
                            <ItemTemplate>
                                <telerik:RadComboBox runat="server" Filter="Contains" ID="ddlEnvironments" OnSelectedIndexChanged="ddlEnvironments_SelectedIndexChanged" Width="150px"
                                    CausesValidation="false" AutoPostBack="true" />
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton ID="inActive" Text="Disattivi" CheckOnClick="true" Group="Disabled" Checked="false" Value="searchDisabled" PostBack="true"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton ID="active" Text="Attivi" CheckOnClick="true" Checked="true" Group="Active" Value="searchActive" PostBack="true"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton ID="search" Value="search" Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                    </Items>
                </telerik:RadToolBar>
                        <telerik:RadToolBar EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%" RenderMode="Lightweight">
                            <Items>
                                <telerik:RadToolBarButton ToolTip="Aggiungi" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                                <telerik:RadToolBarButton ToolTip="Modifica" Enabled="false" Value="modify" Text="Modifica" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                                <telerik:RadToolBarButton ToolTip="Elimina" Enabled="false" Value="delete" Text="Elimina" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
                                <telerik:RadToolBarButton ToolTip="Gruppi" Enabled="false" Value="modifica" Text="Gruppi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/GroupMembers.png" />
                                <telerik:RadToolBarButton ToolTip="Opzioni" Enabled="false" Value="option" Text="Opzioni" ImageUrl="~/App_Themes/DocSuite2008/imgset16/option.png" />
                                <telerik:RadToolBarButton ToolTip="Parametri" Value="properties" Text="Parametri" ImageUrl="~/App_Themes/DocSuite2008/imgset16/property.png" />
                                <telerik:RadToolBarButton ToolTip="Recupera" Enabled="false" Value="recover" Text="Recupera" ImageUrl="~/App_Themes/DocSuite2008/imgset16/view_history.png" />
                                <telerik:RadToolBarButton ToolTip="Stampa" Enabled="false" Value="print" Text="Stampa" ImageUrl="~/App_Themes/DocSuite2008/imgset16/printer.png" />
                                <telerik:RadToolBarButton ToolTip="Log" Value="log" Text="Log" ImageUrl="~/App_Themes/DocSuite2008/imgset16/file_extension_log.png" />
                            </Items>
                        </telerik:RadToolBar>

                        <telerik:RadTreeView ID="rtvContainers" LoadingStatusPosition="BeforeNodeText" runat="server" Style="margin-top: 10px;" Width="100%" Height="91%">

                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Contenitore" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </telerik:RadPane>

                    <telerik:RadSplitBar runat="server" CollapseMode="None" ></telerik:RadSplitBar>
                    <telerik:RadPane runat="server">
                        <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel" Style="margin-top: 5px;">
                            <div class="dsw-panel-content">
                                <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                                    <Items>
                                        <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                            <ContentTemplate>
                                                <asp:Panel runat="server" ID="pnlInformations">
                                                    <div class="col-dsw-10">
                                                        <b>Nome:</b>
                                                        <asp:Label ID="lblContainerNome" runat="server"></asp:Label>
                                                    </div>
                                                    <div class="col-dsw-10">
                                                        <b>Note:</b>
                                                        <asp:Label ID="lblContainerNote" runat="server"></asp:Label>
                                                    </div>
                                                    <asp:Panel CssClass="col-dsw-10" ID="pnlPrivacy" runat="server">
                                                        <b><%= PrivacyLabelTitle %>:</b>
                                                        <asp:Label ID="lblIsPrivacy" runat="server"></asp:Label>
                                                    </asp:Panel>
                                                    <asp:Panel CssClass="col-dsw-10" ID="pnlPrivacyLevel" runat="server" Visible="false">
                                                        <b><%= String.Concat("Livello di ", PRIVACY_LABEL) %>:</b>
                                                        <asp:Label ID="lblLevel" runat="server"></asp:Label>
                                                    </asp:Panel>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                        <telerik:RadPanelItem Text="Deposito documentale" ID="pnlLocations" Expanded="true">
                                            <ContentTemplate>
                                                <asp:Repeater runat="server" ID="locationRepeater">
                                                    <HeaderTemplate>
                                                        <table border="0">
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" Text="<%# Container.DataItem %>"></asp:Label></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                        <telerik:RadPanelItem Text="Gruppi" Expanded="true">
                                            <ContentTemplate>
                                                <telerik:RadTextBox runat="server" ID="txtSearchUser" EmptyMessage="Cerca utente" Width="250px" Style="margin-left: 2px;"></telerik:RadTextBox>
                                                <telerik:RadButton runat="server" ID="btnSearchUser" Text="Cerca" Style="margin-top: 2px;"></telerik:RadButton>
                                                <telerik:RadGrid runat="server" ID="grdGroups" AutoGenerateColumns="False" Style="margin-top: 2px;" GridLines="none" ItemStyle-BackColor="LightGray">
                                                    <MasterTableView Width="100%" DataKeyNames="GroupName" NoMasterRecordsText="Nessun gruppo presente">
                                                        <DetailTables>
                                                            <telerik:GridTableView Width="100%" ShowHeader="false" AlternatingItemStyle-BackColor="WhiteSmoke" ItemStyle-BackColor="Snow" ItemStyle-BorderWidth="2px" Name="UsersGrid">
                                                                <Columns>
                                                                    <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                                        <ItemTemplate>
                                                                            <asp:Image runat="server" ID="imgUser" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" />
                                                                            <asp:ImageButton runat="server" ID="btnAddUser" ImageUrl="../App_Themes/DocSuite2008/imgset16/add.png" />
                                                                        </ItemTemplate>
                                                                    </telerik:GridTemplateColumn>
                                                                    <telerik:GridBoundColumn HeaderText="Utente" DataField="Name"></telerik:GridBoundColumn>
                                                                </Columns>
                                                            </telerik:GridTableView>
                                                        </DetailTables>
                                                        <GroupByExpressions>
                                                            <telerik:GridGroupByExpression>
                                                                <SelectFields>
                                                                    <telerik:GridGroupByField FieldName="Location" />
                                                                </SelectFields>
                                                                <GroupByFields>
                                                                    <telerik:GridGroupByField FieldName="Location" />
                                                                </GroupByFields>
                                                            </telerik:GridGroupByExpression>
                                                        </GroupByExpressions>
                                                        <Columns>
                                                            <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                                <ItemTemplate>
                                                                    <asp:Image runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/GroupMembers.png" />
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridBoundColumn HeaderText="NomeGruppo" DataField="GroupName" UniqueName="GroupName" />
                                                            <telerik:GridBoundColumn HeaderText="Autorizzazioni" DataField="Authorization" UniqueName="Authorization" />
                                                        </Columns>
                                                    </MasterTableView>

                                                </telerik:RadGrid>
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>

                                    </Items>
                                </telerik:RadPanelBar>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlOptions">
                            <telerik:RadSplitter runat="server" Orientation="Horizontal">
                                <telerik:RadPane runat="server" Height="65px">
                                    <telerik:RadPanelBar runat="server" AllowCollapseAllItems="false" ExpandMode="MultipleExpandedItems" Width="100%">
                                        <Items>
                                            <telerik:RadPanelItem Text="Opzioni disponibili" Expanded="true" />
                                        </Items>
                                    </telerik:RadPanelBar>
                                    <telerik:RadDropDownList runat="server" Width="300px" AutoPostBack="true" DefaultMessage="--Seleziona una delle opzioni disponibili"
                                        Style="margin: 4px 0 0 10px;" ID="rdlOptions">
                                    </telerik:RadDropDownList>
                                </telerik:RadPane>
                                <telerik:RadPane runat="server" Height="100%">
                                    <asp:Panel runat="server" Height="100%" ID="pnlOptionActions">
                                        <telerik:RadMultiPage runat="server" ID="optionMultiPage" RenderSelectedPageOnly="true" Height="100%" Width="100%">
                                            <telerik:RadPageView ID="rpvContainerExtGes" runat="server">
                                                <usc:ContainerExtGes ID="uscContainerExtGes" runat="server" />
                                            </telerik:RadPageView>
                                            <telerik:RadPageView ID="rpvContainerDossier" runat="server">
                                                <usc:uscContainerDossierOptions runat="server" ID="uscContainerDossierOptions"></usc:uscContainerDossierOptions>
                                            </telerik:RadPageView>
                                            <telerik:RadPageView ID="rpvContainerConstraint" runat="server">
                                                <usc:uscContainerConstraintOptions runat="server" ID="uscContainerConstraintOptions"></usc:uscContainerConstraintOptions>
                                            </telerik:RadPageView>
                                        </telerik:RadMultiPage>
                                    </asp:Panel>
                                </telerik:RadPane>
                            </telerik:RadSplitter>
                        </asp:Panel>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
        </telerik:RadSplitter>
   </div>
</asp:Content>
