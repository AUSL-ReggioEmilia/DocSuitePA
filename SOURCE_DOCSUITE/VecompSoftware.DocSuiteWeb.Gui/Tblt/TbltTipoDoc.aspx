<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltTipoDoc.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTipoDoc" Title="Tipologia spedizione" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent"> 
 <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //Apre la finestra di editazione di un nodo (aggiunta,eliminazione,modifica)
            function OpenEditWindow(name, operation) {
                var treeView = $find("<%= RadTreeView1.ClientID %>");
                var selectedNode = treeView.get_selectedNode();

                var width = WIDTH_EDIT_WINDOW;
                var height = HEIGHT_EDIT_WINDOW;

                var URL = '<%= ResolveUrl("~/Tblt/TbltTipoDocGes.aspx?") %>';
                parameters = "Action=" + operation + "&Type=Comm&BottoneRefresh=" + document.getElementById("<%= btnRefresh.ClientID %>");
                switch (operation) {
                case "Rename":
                    parameters += "&idObject=" + selectedNode.get_value();
                    break;
                case "Delete":
                    parameters += "&idObject=" + selectedNode.get_value();
                    break;
                case "Recovery":
                    parameters += "&idObject=" + selectedNode.get_value();
                    break;
                case "Log":
                    URL = "../Tblt/TbltLog.aspx?Type=Comm&TableName=TableDocType";
                    parameters = "&idRef=" + selectedNode.get_value();
                    var width = WIDTH_LOG_WINDOW;
                    var height = HEIGHT_LOG_WINDOW;
                    break;
                }
                URL += parameters;
                OpenWindow(URL, name, width, height);
            }

            //Apre una finestra generale
            function OpenWindow(url, name, width, height) {
                var manager = $find("<%=RadWindowManager.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                wnd.add_close(CloseEdit);
            }

            //Funzione che viene richiamata alla chiusura della window
            function CloseEdit(sender, args) {
                if (args.get_argument() !== null) {
                    UpdateTableDoc();
                }
            }

            function UpdateTableDoc()
            {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest('Update');
            }
            
            //Handler del clik di apertura del ContextMenu
            function ContextMenuItemClicked(sender, args) {
                var menuItem = args.get_menuItem();
                OpenEditWindow('windowEdit', menuItem.get_value());
            }

            //Handler apertura ContextMenu
            function ContextMenuShowing(sender, args)
            {
                var node = args.get_node();
                var menu = args.get_menu();
                node.set_selected(true);
                if(node.get_value() == 'Root')
                {
                    //ROOT
                    EnableItemByValue(menu,'Add');
                    DisableItemByValue(menu,'Rename');
                    DisableItemByValue(menu,'Delete');
                    DisableItemByValue(menu,'Recovery');
                    DisableItemByValue(menu,'Log');
                }
                else
                {
                    //TYPE DOC
                    EnableItemByValue(menu,'Add');
                    EnableItemByValue(menu,'Rename');
                    if(node.get_attributes().getAttribute("IsActive") == "1")
                    {
                        EnableItemByValue(menu,'Delete');
                        DisableItemByValue(menu,'Recovery');
                    }
                    else
                    {
                        DisableItemByValue(menu,'Delete');
                        EnableItemByValue(menu,'Recovery');
                    }
                    EnableItemByValue(menu,'Log');
                }
                AdjustButtons(menu);
            }
            
            //Allinea i pulsanti con il contextMenu
            function AdjustButtons(menu)
            {
                AdjustButtonWithItem('<%= btnAggiungi.ClientID %>',menu,'Add');
                AdjustButtonWithItem('<%= btnRinomina.ClientID %>',menu,'Rename');
                AdjustButtonWithItem('<%= btnElimina.ClientID %>',menu,'Delete');
                AdjustButtonWithItem('<%= btnRecupera.ClientID %>',menu,'Recovery');
                AdjustButtonWithItem('<%= btnLog.ClientID %>',menu,'Log');
            }
        </script>

    </telerik:RadScriptBlock>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowEdit" ReloadOnShow="false" runat="server" Title="Gestione Tipologia spedizione" />
        </Windows>
    </telerik:RadWindowManager>
    
    <telerik:RadTreeView ID="RadTreeView1" runat="server">
        <Nodes>
            <telerik:RadTreeNode runat="server" Text="Tipologia spedizione" Value="Root" Expanded="true"></telerik:RadTreeNode>
        </Nodes>
    </telerik:RadTreeView>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel Runat="server" ID="pnlButtons">
	    <asp:button id="btnAggiungi" runat="server" Text="Aggiungi" OnClientClick="OpenEditWindow('windowEdit','Add');return false;"></asp:button>
		<asp:button id="btnRinomina" runat="server" Text="Modifica" OnClientClick="OpenEditWindow('windowEdit','Rename');return false;"></asp:button>
		<asp:button id="btnElimina" runat="server" Text="Elimina" OnClientClick="OpenEditWindow('windowEdit','Delete');return false;"></asp:button>
		<asp:button id="btnRecupera" runat="server" Text="Recupera" OnClientClick="OpenEditWindow('windowEdit','Recovery');return false;"></asp:button>
		<asp:button id="btnLog" runat="server" Text="Log" OnClientClick="OpenEditWindow('windowEdit','Log');return false;"></asp:button>
		<asp:button id="btnRefresh" runat="server" Text="Refresh"></asp:button>
	</asp:Panel>
</asp:Content>

