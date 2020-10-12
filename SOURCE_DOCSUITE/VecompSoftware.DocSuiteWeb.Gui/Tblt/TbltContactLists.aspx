<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltContactLists.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContactLists" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Liste di contatti" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContatti" TagPrefix="uc" %>

<asp:Content ID="ContentHeader" runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">

            function CloseEditList(sender, args) {
                UpdateContactLists();
            }

            function CloseEditContact(sender, args) {
                if (args.get_argument() != null) {
                    UpdateContacts(args.get_argument());
                }
            }

            function OpenEditWindow(name, operation) {
                var parameters = "Action=".concat(operation);
                var treeview = $find("<%= RadTreeViewLists.ClientID%>");
                var selectedNode = treeview.get_selectedNode();
                if (operation == 'Edit' || operation == 'Delete') {
                    if (!!selectedNode) {
                        parameters = parameters.concat("&ContactListId=", selectedNode.get_value());
                    }
                    else {
                        alert('Attenzione: Selezionare una lista di contatti.');
                        return false;
                    }
                }
                var url = "../Tblt/TbltContactListsGes.aspx?Type=Comm&" + parameters;
                return OpenWindow(url, name, WIDTH_EDIT_WINDOW, HEIGHT_EDIT_WINDOW);
            }

            function OpenEditContactWindow(sender, args) {
                var url = "../UserControl/CommonSelContactRubrica.aspx?".concat("Type=Comm&MultiSelect=True")
                return OpenWindow(url, 'widnowAddContact', WIDTH_EDIT_WINDOW, HEIGHT_PRINT_WINDOW);
            }

            function OpenWindow(url, name, width, height) {
                var manager = $find("<%=RadWindowManagerContactLists.ClientID %>");
                var wnd = manager.open(url, name, null);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }

            function UpdateContactLists() {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("LoadContactLists");
            }

            function UpdateContacts(args) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                var parameters = "LoadContacts".concat("|", args)
                ajaxManager.ajaxRequest(parameters);
            }

        </script>
         <style>
            #ctl00_cphContent_rtvUsers {
                height: 90%;
            }
        </style>
    </telerik:RadScriptBlock>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerContactLists" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowAddList" Modal="true" runat="server" ShowContentDuringLoad="false" OnClientClose="CloseEditList" Title="Liste di contatti - Aggiungi" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowModifyList" Modal="true" runat="server" ShowContentDuringLoad="false" OnClientClose="CloseEditList" VisibleStatusbar="false" Title="Liste di contatti - Modifica" />
            <telerik:RadWindow ID="windowDeleteList" Modal="true" runat="server" ShowContentDuringLoad="false" OnClientClose="CloseEditList" Title="Liste di contatti - Elimina" VisibleStatusbar="false" />
            <telerik:RadWindow ID="widnowAddContact" Modal="true" runat="server" ShowContentDuringLoad="false" OnClientClose="CloseEditContact" Title="Contatti - Aggiungi" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowDeleteContact" Modal="true" runat="server" ShowContentDuringLoad="false" OnClientClose="CloseEditContact" Title="Liste di contatti - Elimina" VisibleStatusbar="false" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">

    <telerik:RadSplitter runat="server" ID="Splitter1" Width="100%" Height="100%">
        <telerik:RadPane runat="server" ID="Pane1">
            <telerik:RadSplitter runat="server" Height="100%" Width="100%" Orientation="Horizontal">
                <telerik:RadPane runat="server" Height="100%" Width="100%" Scrolling="None">
                    <telerik:RadToolBar AutoPostBack="false" 
                                        CssClass="ToolBarContainer" 
                                        RenderMode="Lightweight" 
                                        EnableRoundedCorners="False" 
                                        EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                        <Items>
                            <telerik:RadToolBarButton Value="searchList">
                                <ItemTemplate>
                                    <telerik:RadTextBox ID="txtListSearch" EmptyMessage="Nome lista contiene:" runat="server" Width="170px"></telerik:RadTextBox>
                                </ItemTemplate>
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton IsSeparator="true" />
                            <telerik:RadToolBarButton Value="btnListSearch" Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                        </Items>
                    </telerik:RadToolBar>

                    <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%">
                        <Items>
                            <telerik:RadToolBarButton ToolTip="Aggiungi" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                            <telerik:RadToolBarButton ToolTip="Modifica" Value="modify" Text="Modifica" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                            <telerik:RadToolBarButton ToolTip="Elimina" Value="delete" Text="Elimina" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />                            
                        </Items>
                    </telerik:RadToolBar>
                </telerik:RadPane>
                <telerik:RadPane ID="contactListsPane" runat="server" Height="90%">
                    <div style="height: 99%;" class="elementBordered">
                        <telerik:RadTreeView ID="RadTreeViewLists" Height="100%" runat="server">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" runat="server" Text="Liste di contatti" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </div>

                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>

        <telerik:RadSplitBar runat="server" ID="Bar1" />

        <telerik:RadPane runat="server" ID="Splitter2">

            <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel" Style="margin-top: 5px;" Visible="False">
                <div class="dsw-panel-content">
                    <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                        <Items>
                            <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                <ContentTemplate>
                                    <asp:Panel runat="server" ID="pnlInformations">
                                        <div class="col-dsw-10">
                                            <b>Nome:</b>
                                            <asp:Label runat="server" ID="lblListName"></asp:Label>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Id="pnlContacts" runat="server" Expanded="true">
                                <ContentTemplate>
                                    <uc:uscContatti HeaderVisible="False" Caption="Contatti" EnableCompression="true" EnableCC="false" ID="uscContacts" IsRequired="false" Multiple="true" MultiSelect="true" ProtType="true" OnContactAdded="uscContacts_ContactAdded" ReadOnlyProperties="true" runat="server" Type="Comm" />
                                    <asp:Panel runat="server" ID="Panel1">
                                        <telerik:RadButton ID="btnAddContact" runat="server" OnClientClicked="OpenEditContactWindow" Text="Aggiungi" />
                                        <telerik:RadButton ID="btnDeleteContact" runat="server" Text="Elimina" />
                                    </asp:Panel>
                                </ContentTemplate>

                            </telerik:RadPanelItem>
                        </Items>
                    </telerik:RadPanelBar>
                </div>
            </asp:Panel>

        </telerik:RadPane>
    </telerik:RadSplitter>

</asp:Content>
