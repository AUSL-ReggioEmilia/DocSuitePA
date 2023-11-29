<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContatti.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContatti" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlockTRV">
    <script type="text/javascript">

        function OnToolBarClientButtonClicked(sender, args) {
            var button = args.get_item();
            switch (button.get_commandName()) {
                case 'edit':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'move':
                    return <%= Me.ID %>_OpenWindowMove('windowMove', 650, 450, button.get_commandArgument());
                case 'delete':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'recovery':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'clone':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'detail':
                    return <%= Me.ID %>_OpenWindowDettaglio('windowContactDettaglio', 500, 400);
                case 'log':
                    return <%= Me.ID %>_OpenWindowLog('windowContactLog');
                case 'print':
                    return <%= Me.ID %>_OpenWindowPrint('windowContactPrint', 650, 450);
                case 'legenda':
                    return <%= Me.ID %>_OpenWindowLegenda('windowContactLegenda', 350, 200);
                case 'addAmministrazione':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'addGruppo':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'addSettore':
                    return <%= Me.ID %>_OpenWindowFullScreen('windowContactGes', button.get_commandArgument());
                case 'addAOO':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'addUO':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'addRuolo':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'addPersona':
                    return <%= Me.ID %>_OpenWindow('windowContactGes', 650, 450, button.get_commandArgument());
                case 'addPersoneExcel':
                    return <%= Me.ID %>_OpenWindowExcelUpload('windowContactBusinessExcel', 650, 450, button.get_commandArgument());
                case 'exportToExcel':
                    $find("<%= btnExportToExcel.ClientID %>").click();
            }
            return false;
        }



        function manager() { return $find("<%=RadWindowManagerContact.ClientID %>"); }

        function ContextMenuShowing(sender, args) {
            var treeNode = args.get_node();
            // Il menù contestuale è valido solo se il postback è già avvenuto
            if (!treeNode.get_selected()) {
                var menuItems = treeNode.get_contextMenu().get_items();
                var l = menuItems.get_count();
                for (var i = 0; i < l; i++) {
                    menuItems.getItem(i).set_enabled(false);
                }
            }
            // Imposto la selezione
            treeNode.set_selected(true);
        }

        function ContextMenuItemClicked(sender, args) {
            var toolbar = $find("<%= ToolBar.ClientID%>");
            var items = toolbar.get_items();
            for (var i = 0; i < items.get_count() ; i++) {
                var item = items.getItem(i);
                if (Telerik.Web.UI.RadToolBarButton.isInstanceOfType(item) && item.get_commandName() === args.get_menuItem().get_value()) {
                    item.click();
                    return false;
                }
            }
            return false;
        }

        function <%= Me.ID %>_OpenWindowFullScreen(name, parameters) {
            var strIdContact = "";
            var selectedNode = $find("<%= contactTree.ClientID%>").get_selectedNode();
            if (selectedNode !== null && selectedNode.get_value() !== null) {
                strIdContact = "&idContact=" + selectedNode.get_value();
            }

            var url = "../UserControl/CommonContactGes.aspx?" + parameters + strIdContact;

            var wnd = window.radopen(url, name);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Minimize);
            wnd.add_close(<%=Me.ID%>_CloseFunction);
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();
        }


        function <%= Me.ID %>_OpenWindowMove(name, width, height, parameters) {

            var strIdContact = "";
            var selectedNode = $find("<%= contactTree.ClientID%>").get_selectedNode();
            if (selectedNode !== null && selectedNode.get_value() !== null) {
                strIdContact = "&idContact=" + selectedNode.get_value();
            }
            var url = "../UserControl/CommonSelContactRubrica.aspx?" + parameters + strIdContact;

            var wnd = manager().open(url, name);
            wnd.setSize(width, height);
            wnd.add_close(<%=Me.ID%>_CloseMoveFunction);
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();
        }

        function <%= Me.ID %>_OpenWindow(name, width, height, parameters) {
            var strIdContact = "";
            var selectedNode = $find("<%= contactTree.ClientID%>").get_selectedNode();
            if (selectedNode !== null && selectedNode.get_value() !== null) {
                strIdContact = "&idContact=" + selectedNode.get_value();
            }

            var url = "../UserControl/CommonContactGes.aspx?" + parameters + strIdContact;

            var wnd = manager().open(url, name);
            wnd.setSize(width, height);
            wnd.add_close(<%=Me.ID%>_CloseFunction);
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();
        }

        function <%= Me.ID %>_OpenWindowDettaglio(name, width, height) {
            var selectedNode = $find("<%= contactTree.ClientID%>").get_selectedNode();
            if (!selectedNode || !selectedNode.get_value()) {
                alert("Nessun nodo valido selezionato.");
                return false;
            }
            var url = "../UserControl/CommonSelContactRubrica.aspx?Action=Vis&OnlyDetail=true&Titolo=Dettaglio Contatto&idContact=" + selectedNode.get_value();

            var wnd = manager().open(url, name);
            wnd.setSize(width, height);
            wnd.center();

            return false;
        }

        //apertura della finestra Log Contact
        function <%= Me.ID %>_OpenWindowLog(name) {
            var selectedNode = $find("<%= contactTree.ClientID%>").get_selectedNode();
            var strIdContact = "";
            if (selectedNode) {
                strIdContact = selectedNode.get_value();
            }

            var url = "../Tblt/TbltLog.aspx?Type=Comm&TableName=Contact&idRef=" + strIdContact;

            var wnd = manager().open(url, name);
            wnd.setSize(WIDTH_LOG_WINDOW, HEIGHT_LOG_WINDOW);
            wnd.center();

            return false;
        }

        //apertura della finestra Print Contact
        function <%= Me.ID %>_OpenWindowPrint(name, width, height) {
            var selectedNode = $find("<%= contactTree.ClientID%>").get_selectedNode();

            var url = "../Comm/CommPrint.aspx?Type=Comm&PrintName=SingleContactPrint&IdRef=" + selectedNode.get_value();

            var wnd = manager().open(url, name);
            wnd.setSize(width, height);
            wnd.center();

            return false;
        }

        //apertura della finestra Upload Excel Contact
        function <%= Me.ID %>_OpenWindowExcelUpload(name, width, height) {
            var selectedNode = $find("<%= contactTree.ClientID%>").get_selectedNode();
            var url = "../UserControl/CommonExcelImportBusinessContact.aspx?IdRef=" + selectedNode.get_value();

            var wnd = manager().open(url, name);
            wnd.add_close(<%= Me.ID%>_CloseImportFromExcelFunction)
            wnd.setSize(width, height);
            wnd.center();

            return false;
        }

        //apertura della finestra Legenda Contact
        function <%= Me.ID %>_OpenWindowLegenda(name, width, height) {
            var url = "../Tblt/TbltContattiL.aspx?Type=Comm";

            var wnd = manager().open(url, name);
            wnd.setSize(width, height);
            wnd.center();

            return false;
        }

        //richiamata quando la finestra viene chiusa
        function <%= Me.ID %>_CloseImportFromExcelFunction(sender, args) {
            sender.remove_close(<%= ID%>_CloseImportFromExcelFunction);
            if (args.get_argument() === null) {
                return;
            }
            var serializeContacts = args.get_argument();

            var ajaxManager = $find("<%= AjaxManager.ClientID%>");
            ajaxManager.ajaxRequest("<%= Me.ClientID %>|reload|" + serializeContacts);
        }

        //richiamata quando la finestra viene chiusa
        function <%= Me.ID %>_CloseFunction(sender, args) {
            sender.remove_close(<%= ID%>_CloseFunction);
            if (args.get_argument() === null) {
                return;
            }
            var contactGes = args.get_argument();

            var ajaxManager = $find("<%= AjaxManager.ClientID%>");
            ajaxManager.ajaxRequest("<%= Me.ClientID %>|" + contactGes.Action + "|" + contactGes.IdContact);
        }

        //richiamata quando la finestra viene chiusa
        function <%= Me.ID %>_CloseMoveFunction(sender, args) {
            sender.remove_close(<%= ID%>_CloseFunction);
            if (args.get_argument() === null) {
                return;
            }
            var contactGes = args.get_argument();

            var ajaxManager = $find("<%= AjaxManager.ClientID%>");
            ajaxManager.ajaxRequest("<%= Me.ClientID %>|Move|" + contactGes);
        }
        //restituisce un riferimento alla radwindow
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function GetSelectedValue() {
            var selectedNode = $find("<%= contactTree.ClientID%>").get_selectedNode();
            if (selectedNode.get_attributes().getAttribute("Recovery") == "false") {
                return selectedNode.get_value();
            }
            return null;
        }

        function ReturnCodeValue(code) {
            var values = new Array();
            values[0] = code;
            CloseWindow(values);
        }

        function ReturnValue(close) {
            var values = new Array();
            values[0] = GetSelectedValue();
            if (values.length === 0 || values[0] === null) {
                return;
            }

            if (close) {
                GetRadWindow().close(values);
            } else {
                GetRadWindow().BrowserWindow.<%= CallerId%>_Update(values);
            }
        }

        function btnConferma_OnClick(sender, args) {
            ReturnValue(true);
            return false;
        }

        function btnConfermaMulti_OnClick(sender, args) {
            ReturnMultiValue(true);
            return false;
        }

        function btnConfermaNuovo_OnClick(sender, args) {
            ReturnMultiValue(false);
            return false;
        }

        function ReturnMultiValue(close) {
            var nodes = $find("<%= contactTree.ClientID%>").get_checkedNodes();
            var values = new Array();
            for (var i = 0; i < nodes.length; i++) {
                values[i] = nodes[i].get_value();
            }

            if (values.length === 0 || values[0].length <= 0) {
                return;
            }

            if (close) {
                CloseWindow(values);
                return;
            }
            GetRadWindow().BrowserWindow.<%= CallerId%>_Update(values);

            var ajaxManager = $find("<%= AjaxManager.ClientID%>");
            ajaxManager.ajaxRequest("<%= Me.ClientID %>|reload");
        }

        function CloseWindow(args) {
            var oWindow = GetRadWindow();
            oWindow.close(args);
        }

        function RadSplitterLoad(sender, args) {
            // sender.repaint();
        }

        $(document).ready(function () {
            $("#<%= txtCerca.ClientID%>").focus();
        });

    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerContact" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowContactGes" ReloadOnShow="false" runat="server" Title="Gestione contatti" />
        <telerik:RadWindow ID="windowContactLegenda" Modal="false" ReloadOnShow="false" runat="server" Title="Rubrica legenda" />
        <telerik:RadWindow ID="windowContactLog" ReloadOnShow="false" runat="server" Title="Rubrica log" />
        <telerik:RadWindow ID="windowContactPrint" ReloadOnShow="false" runat="server" Title="Stampa dei contatti" />
        <telerik:RadWindow ID="windowContactDettaglio" ReloadOnShow="false" runat="server" Title="Dettaglio contatto" />
        <telerik:RadWindow ID="windowSelSettori" ReloadOnShow="false" runat="server" Title="Selezione settori" />
        <telerik:RadWindow ID="windowUploadPeople" ReloadOnShow="false" runat="server" Title="Inserisci nuove persone tramite Excel" />
    </Windows>
</telerik:RadWindowManager>

<div class="splitterWrapper">
    <telerik:RadSplitter BorderSize="0" CssClass="noBorderSplitter" Height="100%" ID="mainSplitter" OnClientLoad="RadSplitterLoad" ResizeWithParentPane="False" Orientation="Horizontal" PanesBorderSize="0" ResizeMode="Proportional" runat="server" Scrolling="Both" Width="100%">
        <telerik:RadPane ID="headerPane" runat="server" BorderStyle="None" Scrolling="None">
            <table class="dataform">
                <tr>
                    <td class="label" style="width: 20%">Descrizione:</td>
                    <td>
                        <asp:Panel runat="server" ID="pnlSearch" DefaultButton="btnSearch" CssClass="inlineBlock">
                            <asp:TextBox ID="txtCerca" runat="server" Width="280px" />
                            <telerik:RadButton ID="btnSearch" runat="server" Text="Cerca" ToolTip="Ricerca per descrizione">
                                <Icon PrimaryIconUrl="../App_Themes/DocSuite2008/images/search-transparent.png" />
                            </telerik:RadButton>
                            &nbsp;
                        <asp:CheckBox ID="chbContiene" runat="server" Text="Contiene" />
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 20%">Codice Ricerca:</td>
                    <td>
                        <asp:Panel runat="server" ID="pnlSearchCode" DefaultButton="btnSearchCode" CssClass="inlineBlock">
                            <asp:TextBox ID="txtSearchCode" MaxLength="255" runat="server" Width="280px" />
                            <telerik:RadButton ID="btnSearchCode" runat="server" Text="Cerca e Seleziona" ToolTip="Ricerca per codice e seleziona">
                                <Icon PrimaryIconUrl="../App_Themes/DocSuite2008/images/search-transparent.png" />
                            </telerik:RadButton>
                        </asp:Panel>
                    </td>
                </tr>
                 <tr id="pnlContactListFilter" runat="server" Visible="False">
                    <td class="label" style="width: 20%">Liste di contatti:</td>
                    <td>
                        <asp:Panel runat="server" ID="pnlSearchList" CssClass="inlineBlock">
                            <asp:DropDownList runat="server" ID="ddlContactLists" DataTextField="Name" DataValueField="Id" Width="280px" AutoPostBack="True">
                            </asp:DropDownList>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </telerik:RadPane>
        <telerik:RadPane BorderStyle="None" ID="mainPane" runat="server" Scrolling="None" Width="100%">
            <%-- Pannello Principale --%>
            <telerik:RadSplitter BorderSize="0" Height="100%" ID="contactSplitter" PanesBorderSize="0" ResizeMode="Proportional" ResizeWithParentPane="True" runat="server" Width="100px">
                <telerik:RadPane runat="server" ID="contactTreePane" Scrolling="None" Height="100%" Width="100%">
                    <%-- Pannello Contatti --%>
                    <telerik:RadSplitter BorderStyle="None" Height="100%" ID="treeSplitter" Orientation="Horizontal" PanesBorderSize="0" ResizeMode="Proportional" ResizeWithParentPane="True" runat="server" Width="100%">
                        <telerik:RadPane BorderStyle="None" Height="12%" ID="contactTopPane" runat="server" Scrolling="None">
                            <asp:Panel ID="pnlSearchDescription" runat="server" Width="35%" Height="40%" Style="margin-top: 1%">
                                <table>
                                    <tr>
                                        <td class="label" style="width: 20%"><b style="margin-left: 10%">Descrizione: </b></td>
                                        <td>
                                            <asp:Panel runat="server" ID="pnlSearchTbltRubrica" DefaultButton="btnSearchTbltRubrica" Height="80%" CssClass="inlineBlock">
                                                <asp:TextBox ID="txtCercaTbltRubrica" runat="server" Width="250px" Style="margin-left:5%" />
                                                <asp:Button ID="btnSearchTbltRubrica" runat="server" Text="Cerca" ToolTip="Ricerca per Descrizione" />
                                                &nbsp;
                        <asp:CheckBox ID="chbContieneTbltRubrica" runat="server" Text="Contiene" />
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>



                            <%-- Toolbar Contatti --%>
                            <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" OnClientButtonClicked="OnToolBarClientButtonClicked" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%">
                                <Items>
                                    <telerik:RadToolBarButton CommandName="edit" Group="edit" CausesValidation="False" PostBack="False" ToolTip="Modifica" />
                                    <telerik:RadToolBarButton CommandName="delete" Group="edit" CausesValidation="False" PostBack="False" ToolTip="Elimina" />
                                    <telerik:RadToolBarButton CommandName="recovery" Group="edit" CausesValidation="False" PostBack="False" ToolTip="Ripristina" />
                                    <telerik:RadToolBarButton CommandName="move" Group="edit" CausesValidation="False" PostBack="False" ToolTip="Sposta" />
                                    <telerik:RadToolBarButton CommandName="clone" Group="edit" CausesValidation="False" PostBack="False" ToolTip="Clona" />
                                    <telerik:RadToolBarButton CommandName="log" Group="edit" CausesValidation="False" PostBack="False" ToolTip="Log" />
                                    <telerik:RadToolBarButton CommandName="print" Group="edit" CausesValidation="False" PostBack="False" ToolTip="Stampa" />
                                    <telerik:RadToolBarButton CommandName="detail" Group="utility" CausesValidation="False" PostBack="False" Visible="False" ToolTip="Dettaglio" Text="Dettaglio" />
                                    <telerik:RadToolBarButton CommandName="legenda" Group="utility" CausesValidation="False" PostBack="False" ToolTip="Legenda" Text="Legenda" />
                                    <telerik:RadToolBarButton IsSeparator="true" Group="selection" />
                                    <telerik:RadToolBarButton CommandName="checkAllContacts" Group="selection" Text="Seleziona tutti" />
                                    <telerik:RadToolBarButton CommandName="uncheckAllContacts" Group="selection" Text="Deseleziona tutti" />
                                    <telerik:RadToolBarButton IsSeparator="true" Group="add" />
                                    <telerik:RadToolBarButton CommandName="addAmministrazione" CausesValidation="False" PostBack="False" Group="add" ToolTip="Nuova Amministrazione" />
                                    <telerik:RadToolBarButton CommandName="addGruppo" CausesValidation="False" PostBack="False" Group="add" ToolTip="Nuovo gruppo" />
                                    <telerik:RadToolBarButton CommandName="addSettore" CausesValidation="False" PostBack="False" Group="add" ToolTip="Nuovo Settore" />
                                    <telerik:RadToolBarButton CommandName="addAOO" CausesValidation="False" PostBack="False" Group="add" ToolTip="Nuova Area Organizzativa Omogenea" />
                                    <telerik:RadToolBarButton CommandName="addUO" CausesValidation="False" PostBack="False" Group="add" ToolTip="Nuova Unità Organizzativa" />
                                    <telerik:RadToolBarButton CommandName="addRuolo" CausesValidation="False" PostBack="False" Group="add" ToolTip="Nuovo Ruolo" />
                                    <telerik:RadToolBarButton CommandName="addPersona" CausesValidation="False" PostBack="False" Group="add" ToolTip="Nuova Persona" />
                                    <telerik:RadToolBarButton CommandName="addPersoneExcel" CausesValidation="False" PostBack="False" Group="add" ToolTip="Nuove Persone tramite excel" />
                                    <telerik:RadToolBarButton CommandName="exportToExcel" CausesValidation="False" PostBack="false" Visible="false" ToolTip="Esporta in excel" />
                                </Items>
                            </telerik:RadToolBar>
                        </telerik:RadPane>
                        <telerik:RadPane runat="server" ID="contactInnerTreePane" BorderStyle="None" Height="100%">
                            <%-- Treeview Contatti --%>
                            <telerik:RadTreeView EnableViewState="true" ID="contactTree" OnClientContextMenuItemClicked="ContextMenuItemClicked" OnClientContextMenuShowing="ContextMenuShowing" runat="server">
                                <Nodes>
                                    <telerik:RadTreeNode Checkable="false" Expanded="true" runat="server" Selected="true" Text="Rubrica" EnableContextMenu="True" />
                                </Nodes>
                                <CollapseAnimation Duration="100" Type="None" />
                                <ExpandAnimation Duration="100" Type="None" />
                            </telerik:RadTreeView>
                        </telerik:RadPane>
                    </telerik:RadSplitter>
                </telerik:RadPane>
                <telerik:RadSplitBar runat="server" ID="contactSplitBar" />
                <telerik:RadPane runat="server" ID="contactDetailPane">
                    <%-- Pannello Dettagli --%>
                    <telerik:RadAjaxPanel runat="server" ID="contactDetailAjaxPanel">
                        <asp:Table ID="contactDetailTable" runat="server" Width="100%" />
                    </telerik:RadAjaxPanel>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>
        <telerik:RadPane ID="footerPane" runat="server" Height="5%" BorderStyle="None" Scrolling="None">
            <div class="inlineBlock">
                <telerik:RadButton ID="btnConferma" runat="server" AutoPostBack="false" Text="Conferma Selezione" Width="130px" />

                <telerik:RadButton ID="btnConfermaNuovo" runat="server" Text="Conferma e Nuovo" Width="130px" CausesValidation="False" AutoPostBack="false" OnClientClicked="btnConfermaNuovo_OnClick" />
                <telerik:RadButton ID="btnGestione" runat="server" Text="Gestione contatti" Width="130px" />
                <telerik:RadButton ID="btnSblocca" runat="server" Width="130px" Text="Sblocca" />
                <telerik:RadButton ID="btnExportToExcel" runat="server" Style="display: none;"></telerik:RadButton>
            </div>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>
