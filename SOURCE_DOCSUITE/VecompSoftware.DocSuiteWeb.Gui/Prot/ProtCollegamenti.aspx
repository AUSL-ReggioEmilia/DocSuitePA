<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtCollegamenti" Codebehind="ProtCollegamenti.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Collegamenti" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="rsb" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //Apre la finestra per l'aggiunta di un protocollo collegato
            function OpenProtocolLinksWindow(name,width,height) {
                // Nasconde pulsanti di azione
                hideActionButtons();
                var url = "../Prot/ProtCollegamentiGes.aspx?Titolo=Aggiungi Collegamento&";
                var treeView = $find("<%= tvwProtocolLink.ClientID %>");
                var selectedNode = treeView.get_selectedNode();
                var value = selectedNode.get_value().split("/");
                var year = value[0];
                var number = value[1];
                url += "year=" + year + "&number=" + number;
                return OpenWindow(url, name, width, height);
            }

            //Apre una finestra generale
            function OpenWindow(url,name,width,height) {
                var manager = $find("<%=RadWindowManagerCollegamenti.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }

            function tvwProtocolLink_ClientNodeClicked( sender, eventArgs ) {
                var node = eventArgs.get_node();
                if (node) {
                    node.expand();
                    // menu contestuale
                    var contextMenu = node.get_contextMenu();
                    if (contextMenu) {
                        var btnView = document.getElementById('btnView');
                        var btnAdd = document.getElementById('btnAdd');
                        var btnDelete = document.getElementById('btnDelete');
                        // Elementi del menu contestuale
                        var items = contextMenu.get_items();
                        if (items.get_count() > 0) {
                            var hasViewBtn = "none", hasAddBtn = "none", hasDeleteBtn = "none";
                            for (var i = 0; i < items.get_count(); i++) {
                                var value = items.getItem(i).get_value().toLowerCase();
                                // visualizza/nasconde pulsante visualizza
                                if (value == "view") {
                                    hasViewBtn = "";
                                }
                                // visualizza/nasconde pulsante aggiungi
                                if (value == "add") {
                                    hasAddBtn = "";
                                }
                                if (value == "delete") {
                                    hasDeleteBtn = "";
                                }
                            }
                            btnView.disabled = hasViewBtn;
                            btnAdd.disabled = hasAddBtn;
                            btnDelete.disabled = hasDeleteBtn;
                        } else {
                            hideActionButtons();
                        }
                    }
                } else {
                    hideActionButtons();
                }
            }

            function ContextMenuItemClicked(sender, args) {
                var menuItem = args.get_menuItem();
                var treeNode = args.get_node();
                treeNode.set_selected(true);
                switch (menuItem.get_value()) {
                case "View":
                    ViewProtocol();
                    break;
                case "Add":
                    OpenProtocolLinksWindow('windowAddCollegamento', 650, 500);
                    break;
                case "Delete":
                    DeleteProtocol();
                    break;
                }
            }

            function hideActionButtons() {
                // Pulsanti di azione visualizza/aggiungi/cancella
                var btnView = document.getElementById('btnView');
                var btnAdd = document.getElementById('btnAdd');
                var btnDelete = document.getElementById('btnDelete');
                btnView.disabled = "disabled";
                btnAdd.disabled = "disabled";
                btnDelete.disabled = "disabled";
            }

            function showActionButtons() {

                // Pulsanti di azione visualizza/aggiungi/cancella
                var btnView = document.getElementById('btnView');
                var btnAdd = document.getElementById('btnAdd');
                var btnDelete = document.getElementById('btnDelete');
                btnView.disabled = false;
                btnAdd.disabled = false;
                btnDelete.disabled = false;
            }

            function ViewProtocol() {
                // Nasconde pulsanti di azione
                hideActionButtons();
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest('View');
            }

            function DeleteProtocol() {
                // Nasconde pulsanti di azione
                hideActionButtons();
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest('Delete');
            }

            function RefreshNode() {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest('Refresh');
            }
            
        </script>
    </telerik:RadScriptBlock>
    
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerCollegamenti" runat="server">
        <Windows>
            <telerik:RadWindow Height="500" ID="windowAddCollegamento" OnClientClose="RefreshNode" runat="server" Title="Aggiungi Collegamento" Width="650" />
            <telerik:RadWindow Height="550" ID="wndSearch" runat="server" Width="700" />
        </Windows>
    </telerik:RadWindowManager>

    <table class="datatable">
        <tr>
            <th>
                <telerik:RadScriptBlock runat="server" ID="rsb1" EnableViewState="false">
                Protocollo:<%=CurrentProtocol.Id.ToString()%> 
                </telerik:RadScriptBlock>
            </th>
        </tr>
        <tr>
            <td>
                 <telerik:RadTreeView ID="tvwProtocolLink" Width="100%" runat="server" OnClientNodeClicked="tvwProtocolLink_ClientNodeClicked" OnClientContextMenuItemClicked="ContextMenuItemClicked" PersistLoadOnDemandNodes="true">
                    <ContextMenus>
                    <telerik:RadTreeViewContextMenu ID="ContextMenuCollegamentiEmpty" runat="server" />
                    <telerik:RadTreeViewContextMenu ID="ContextMenuCollegamentiCurrentProtocollo" runat="server">
                        <Items>
                            <telerik:RadMenuItem Value="View" Text="Visualizza..." Enabled="true" Visible="true" />
                            <telerik:RadMenuItem Value="Add" Text="Aggiungi..." Enabled="false" Visible="false" />
                        </Items>
                    </telerik:RadTreeViewContextMenu>
                    <telerik:RadTreeViewContextMenu ID="ContextMenuCollegamenti" runat="server">
                        <Items>
                            <telerik:RadMenuItem Value="View" Text="Visualizza..." Enabled="true" Visible="true" />
                            <telerik:RadMenuItem Value="Add" Text="Aggiungi..." Enabled="false" Visible="false" />
                            <telerik:RadMenuItem Value="Delete" Text="Cancella..." Enabled="false" Visible="false" />
                        </Items>
                    </telerik:RadTreeViewContextMenu>
                </ContextMenus>
                    <Nodes>
                        <telerik:RadTreeNode runat="server" Text="Collegamenti" Font-Bold="true" Expanded="true" EnableViewState="true" EnableContextMenu="false" ContextMenuID="ContextMenuCollegamentiEmpty" />
                    </Nodes>
                </telerik:RadTreeView>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
	<div class="yui-g">
	    <div class="yui-u first">
			<input type="button" id="btnView" disabled="disabled" value="Visualizza" onclick="ViewProtocol()" class="button" />
	        <input type="button" id="btnAdd" disabled="disabled" value="Aggiungi" onclick="OpenProtocolLinksWindow('windowAddCollegamento',650,500);" class="button" />
	        <input type="button" id="btnDelete" disabled="disabled" value="Cancella" onclick="DeleteProtocol();" class="button" />
		</div>
		<div class="yui-u right">
		    <asp:button id="btnMail" runat="server" Text="Invia" Visible="false"  />
			<asp:button id="btnStampa" runat="server" Text="Stampa" Visible="false"  />
		</div>
	</div>
</asp:Content>