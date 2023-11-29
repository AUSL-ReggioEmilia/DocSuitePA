<%@ Page Title="Gestione Organigramma" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltOChart.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltOChart" %>

<%@ Register TagPrefix="usc" TagName="RoleControl" Src="~/Control/RoleControl.ascx" %>
<%@ Register TagPrefix="usc" TagName="ContainersControl" Src="~/Control/ContainersControl.ascx" %>
<%@ Register TagPrefix="usc" TagName="ContactControl" Src="~/Control/ContactControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">

    <style type="text/css">
        .datatable .label {
            width: 150px;
        }

        .datatable .buttons {
            text-align: right;
        }
    </style>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" language="javascript">

            function <%= ID %>_tbItemButtonClicking(sender, args) {
                //var btn = args.get_item();
                //switch (btn.get_commandName()) {
                //    case "ADD":
                //        AddNewHead();
                //        args.set_cancel(true);
                //        break;
                //}
                var btn = args.get_item();
                if (btn.get_commandName() == "MOVECHART") {
                    OpenMoveOChartSelector();
                    args.set_cancel(true);
                }
            }

            function tbContainersControlButtonClicking(sender, args) {
                var btn = args.get_item();
                switch (btn.get_commandName()) {
                    case "ADD":
                        OpenWindow('<%= rwContainerSelector.ClientID%>');
                        args.set_cancel(true);
                        break;
                    case "ADDNEW":
                        OpenNewContainer();
                        args.set_cancel(true);
                        break;
                }
            }

            function tbContactControlButtonClicking(sender, args) {
                var btn = args.get_item();
                switch (btn.get_commandName()) {
                    case "ADD":
                        OpenContactSelector();
                        args.set_cancel(true);
                        break;
                }
            }

            function tbRoleControlButtonClicking(sender, args) {
                var btn = args.get_item();
                switch (btn.get_commandName()) {
                    case "ADD":
                        OpenRoleSelector();
                        args.set_cancel(true);
                        break;
                    case "ADDNEW":
                        OpenNewRole();
                        args.set_cancel(true);
                        break;
                }
            }
            function OpenWindow(id, title) {
                var oWnd = $find(id);
                oWnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                if (title) {
                    oWnd.set_title(title);
                }

                oWnd.show();
                return false;
            }

            function CloseWindow(id) {
                var oWnd = $find(id);
                oWnd.close();
                MyAjaxRequest(id, "REFRESH", "NONE");
            }

            function OpenRoleSelector() {
                var url = "../UserControl/CommonSelSettori.aspx?<%=GetWindowParameters() %>";
                var wnd = window.radopen(url, "windowSelSettori");
                wnd.setSize(<%= ProtocolEnv.DocumentPreviewWidth%>, <%= ProtocolEnv.DocumentPreviewHeight%>);
                wnd.add_close(RoleSelectorCloseFunction);
                wnd.set_destroyOnClose(true);
                wnd.set_modal(true);
                wnd.center();
            }

            function OpenMoveOChartSelector() {
                var url = "../UserControl/CommonSelOChart.aspx?<%=GetOChartSelectorParameters() %>";
                var wnd = window.radopen(url, "windowSelOChart");
                wnd.add_close(MoveOChartSelectorCloseFunction);
                wnd.set_destroyOnClose(true);
                wnd.set_modal(true);
                wnd.center();
            }

            function OpenContactSelector() {
                var url = "../UserControl/CommonSelContactRubrica.aspx?<%=GetContactSelectorParameters()%>";
                var wnd = window.radopen(url, "windowSelContacts");
                wnd.setSize(<%= ProtocolEnv.DocumentPreviewWidth%>, <%= ProtocolEnv.DocumentPreviewHeight%>);
                wnd.add_close(ContactSelectorCloseFunction);

                wnd.set_destroyOnClose(true);
                wnd.set_modal(true);
                wnd.center();
            }

            function OpenNewRole() {
                var prompt = radprompt("Nome", NewRoleCloseFunction);
                prompt.set_width(400);
                prompt.set_maxWidth(400);
                prompt.set_title("Inserimento nuovo settore");
                prompt.set_autoSize(false);
                prompt.set_initialBehaviors(4);
                prompt.center();
            }

            function NewRoleCloseFunction(arg) {
                if (arg) {
                    MyAjaxRequest("ROLES", "NEW", arg);
                }
            }

            function OpenNewContainer() {
                var prompt = radprompt("Nome", NewContainerCloseFunction);
                prompt.set_width(400);
                prompt.set_maxWidth(400);
                prompt.set_title("Inserimento nuovo contenitore");
                prompt.set_autoSize(false);
                prompt.set_initialBehaviors(4);
                prompt.center();
            }

            function NewContainerCloseFunction(arg) {
                if (arg) {
                    MyAjaxRequest("CONTAINERS", "NEW", arg);
                }
            }

            // richiamata quando la finestra viene chiusa
            function RoleSelectorCloseFunction(sender, args) {
                if (args.get_argument() !== null && args.get_argument().length > 0) {
                    MyAjaxRequest("ROLES", "ADD", args.get_argument());
                }
            }

            function MoveOChartSelectorCloseFunction(sender, args) {
                if (args !== null) {
                    MyAjaxRequest("NODES", "MOVE", args.get_argument());
                }
            }

            // richiamata quando la finestra viene chiusa
            function ContactSelectorCloseFunction(sender, args) {
                if (args.get_argument() !== null) {
                    MyAjaxRequest("CONTACTS", "ADD", args.get_argument());
                }
            }
            function MyAjaxRequest(sender, action, args) {
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                ajaxManager.ajaxRequest(sender + "|" + action + "|" + args);
            }

        </script>

    </telerik:RadCodeBlock>

    <telerik:RadWindow runat="server" ID="rwContainerSelector" Title="Seleziona Contenitore" Width="480" Height="95" Behaviors="Close"
        IconUrl="../App_Themes/DocSuite2008/imgset16/box_open.png">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="ContainerSelectorUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table class="datatable" id="ContainerSelectorWindowTable">
                        <tr>
                            <td class="label">Contenitore</td>
                            <td>
                                <telerik:RadComboBox runat="server" CausesValidation="false"
                                    ID="cbContainer" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                    ItemRequestTimeout="500" Width="300px">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" colspan="2">
                                <asp:Button runat="server" ID="cmdContainerSelectorOk" Text="Conferma" Width="100px" />
                                <asp:Button runat="server" ID="cmdContainerSelectorCancel" Text="Annulla" Width="100px" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwItemDetail" Width="480" Height="250" Behaviors="Close"
        IconUrl="../App_Themes/DocSuite2008/imgset16/network-share.png">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="ItemDetailUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>

                    <table class="datatable" id="ItemDetailWindowTable">
                        <tr>
                            <td class="label">Nodo padre</td>
                            <td>
                                <asp:Label ID="lblItemDetailSource" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Nome</td>
                            <td>
                                <asp:TextBox ID="txtItemDetailTitle" runat="server" Width="280" />
                                <asp:RequiredFieldValidator ValidationGroup="vgItemDetail" runat="server" ErrorMessage="Campo nome obbligatorio" Display="Dynamic" ControlToValidate="txtItemDetailTitle"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr class="label">
                            <td class="label">Descrizione</td>
                            <td>
                                <asp:TextBox ID="txtItemDetailDescription" runat="server" Rows="5" Width="280" TextMode="MultiLine" /></td>
                        </tr>
                        <tr class="label">
                            <td class="label">Acronimo</td>
                            <td>
                                <asp:TextBox ID="txtItemDetailAcronym" runat="server" Width="280" /></td>
                        </tr>
                        <tr>
                            <td class="label">Codice</td>
                            <td>
                                <asp:TextBox ID="txtItemDetailCode" runat="server" Width="280" />
                                <asp:RequiredFieldValidator ValidationGroup="vgItemDetail" runat="server" Display="Dynamic" ErrorMessage="Campo codice obbligatorio"
                                    ControlToValidate="txtItemDetailCode"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="ItemDetailCodeValidator" ValidationGroup="vgItemDetail" Display="Dynamic" runat="server"
                                    ControlToValidate="txtItemDetailCode" EnableClientScript="False"></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Attivo</td>
                            <td>
                                <asp:CheckBox Checked="True" ID="cbItemDetailEnabled" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" colspan="2">
                                <asp:Button runat="server" ID="cmdItemDetailEditOk" Text="Conferma" Visible="False" CommandName="EDITOK" Width="100px" ValidationGroup="vgItemDetail" CausesValidation="True" />
                                <asp:Button runat="server" ID="cmdItemDetailAdd" Text="Aggiungi" Visible="False" CommandName="ADD" Width="100px" ValidationGroup="vgItemDetail" CausesValidation="True" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwChartDetail" Title="Organigramma" Width="470" Height="185" Behaviors="Close"
        IconUrl="../App_Themes/DocSuite2008/imgset16/network-share.png">
        <ContentTemplate>
            <asp:UpdatePanel ID="ChartDetailUpdatePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="ChartDetailWindowTable" class="datatable">
                        <tr>
                            <td class="label">Nome</td>
                            <td>
                                <asp:TextBox ID="txtChartDetailTitle" runat="server" Width="300" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtChartDetailTitle" ValidationGroup="vgChartDetail"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Descrizione</td>
                            <td>
                                <asp:TextBox ID="txtChartDetailDescription" runat="server" Rows="5" Width="300" TextMode="MultiLine" /></td>
                        </tr>
                        <tr>
                            <td class="label">Valido dal</td>
                            <td>
                                <telerik:RadDatePicker ID="dpChartDetailDateFrom" runat="server" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="dpChartDetailDateFrom" ValidationGroup="vgChartDetail"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="display: none">
                            <td class="label">Valido al</td>
                            <td>
                                <telerik:RadDatePicker ID="dpChartDetailDateTo" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="buttons">
                                <asp:Button ID="cmdChartDetailAdd" runat="server" Text="Conferma" Width="100px" ValidationGroup="vgChartDetail" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwChartRemove" Width="480" Height="150"
        IconUrl="../App_Themes/DocSuite2008/imgset16/network-share.png">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="ChartRemoveUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="ChartDetailRemove" class="datatable">
                        <tr>
                            <td class="label">Organigramma selezionato</td>
                            <td>
                                <asp:Label runat="server" ID="lblChartRemoveTitle"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="label">valido dal</td>
                            <td>
                                <asp:Label runat="server" ID="lblChartRemoveDate"></asp:Label></td>
                        </tr>
                        <tr>
                            <td colspan="2" class="buttons">
                                <asp:Button runat="server" ID="cmdChartRemoveOk" Text="Conferma" Width="100px" />
                                <asp:Button runat="server" ID="cmdChartRemoveCancel" Text="Annulla" Width="100px" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwItemRemoveConfirm" Width="480" Height="120" Behaviors="Close"
        IconUrl="../App_Themes/DocSuite2008/imgset16/network-share.png">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="ItemRemoveConfirmPanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table id="ChartItemRemove" class="datatable">
                        <tr>
                            <td class="label">Unità Organizzativa</td>
                            <td>
                                <asp:Label runat="server" ID="lblItemRemoveConfirmTitle"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="label">Codice</td>
                            <td>
                                <asp:Label runat="server" ID="lblItemRemoveConfirmCode"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="label">Codice Padre</td>
                            <td>
                                <asp:Label runat="server" ID="lblItemRemoveConfirmParentCode"></asp:Label></td>
                        </tr>
                        <tr>
                            <td colspan="2" class="buttons">
                                <asp:Button runat="server" ID="cmdItemRemoveConfirmOk" Text="Conferma" />
                                <asp:Button runat="server" ID="cmdItemRemoveConfirmCancel" Text="Annulla" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerRoles" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowSelOChart" Modal="true" runat="server" ShowContentDuringLoad="false" Title="Selezione Organigramma" VisibleStatusbar="false" />
        </Windows>
    </telerik:RadWindowManager>

    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Height="100%" Width="100%">
            <telerik:RadPane runat="server" Scrolling="None">
                <telerik:RadSplitter runat="server" ID="treeSplitter" Width="100%" Height="100%" Orientation="Horizontal" ResizeMode="Proportional">
                    <telerik:RadPane runat="server" ID="treeToolbarPane" BorderStyle="None" Scrolling="None" Height="53px" MinHeight="53" MaxHeight="53">
                        <telerik:RadToolBar runat="server" ID="tbItem" EnableRoundedCorners="False" EnableShadows="False" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton CommandName="ADDCHART" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/add.png" ToolTip="Aggiungi nuovo organigramma" />
                                <telerik:RadToolBarButton CommandName="REMOVECHART" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/remove.png" ToolTip="Elimina organigramma" />
                                <telerik:RadToolBarButton CommandName="EDITCHART" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/pencil.png" ToolTip="Modifica organigramma" />
                                <telerik:RadToolBarButton CommandName="MOVECHART" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/move_to_folder.png" Value="moveChart" ToolTip="Sposta" />
                                <telerik:RadToolBarButton runat="server" IsSeparator="true"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton CommandName="ADDROOT" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/network-share-star.png" ToolTip="Aggiungi nuovo nodo radice" />
                                <telerik:RadToolBarButton CommandName="ADDCHILD" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/network-share-add.png" ToolTip="Aggiungi nodo figlio" />
                                <telerik:RadToolBarButton CommandName="EDIT" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/pencil.png" ToolTip="Modifica nodo selezionato" />
                                <telerik:RadToolBarButton CommandName="REMOVE" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/network-share-remove.png" ToolTip="Elimina nodo selezionato" />
                            </Items>
                        </telerik:RadToolBar>
                        <asp:DropDownList Width="100%" AutoPostBack="True" DataTextField="Title" DataValueField="Id" ID="ddlOCharts" runat="server" />
                    </telerik:RadPane>
                    <telerik:RadPane runat="server" ID="treeDetailPane" BorderStyle="None" Scrolling="None" MinHeight="69" Height="89">
                        <table class="datatable">
                            <tr>
                                <td class="label">Nome
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadName"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">Descrizione
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadDescription"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">Stato
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadStatus"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">Data Inizio
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadDateFrom"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">Data Fine
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadDateTo"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </telerik:RadPane>
                    <telerik:RadPane runat="server" ID="treeInnerPane" BorderStyle="None" Height="80%">
                        <telerik:RadTreeView ID="OChartTree" runat="server" />
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
            <telerik:RadSplitBar runat="server" ID="Bar1" />
            <telerik:RadPane runat="server">
                <asp:Panel runat="server" ID="DetailPanel" Enabled="false">

                    <table class="datatable" id="ItemDetailTable">
                        <tr>
                            <th colspan="2">Dettaglio unità organizzativa</th>
                        </tr>
                        <tr>
                            <td class="label">Nome
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblItemName"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Descrizione
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblItemDescription"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Acronimo
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblItemAcronym"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Codice
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblItemCode"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Codice padre
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblItemParentCode"></asp:Label>
                            </td>
                        </tr>
                    </table>

                    <table class="datatable">
                        <tr>
                            <th>Contatti collegati</th>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadToolBar runat="server" ID="tbContactControl" CssClass="ToolBarContainer"
                                    EnableRoundedCorners="False" EnableShadows="False" Width="100%" OnClientButtonClicking="tbContactControlButtonClicking">
                                    <Items>
                                        <telerik:RadToolBarButton runat="server" CommandName="ADD" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png" ToolTip="Aggiungi settore esistente"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_delete.png" ToolTip="Elimina settore selezionato"></telerik:RadToolBarButton>
                                    </Items>
                                </telerik:RadToolBar>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <usc:ContactControl runat="server" ID="myContactControl" DefaultGroup=""></usc:ContactControl>
                            </td>
                        </tr>
                    </table>

                    <table class="datatable">
                        <tr>
                            <th>Settori collegati</th>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadToolBar runat="server" ID="tbRoleControl" CssClass="ToolBarContainer"
                                    EnableRoundedCorners="False" EnableShadows="False" Width="100%" OnClientButtonClicking="tbRoleControlButtonClicking">
                                    <Items>
                                        <telerik:RadToolBarButton runat="server" CommandName="ADD" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png" ToolTip="Aggiungi settore esistente"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="ADDNEW" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_edit.png" ToolTip="Aggiungi nuovo settore"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_delete.png" ToolTip="Elimina settore selezionato"></telerik:RadToolBarButton>
                                    </Items>
                                </telerik:RadToolBar>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <usc:RoleControl runat="server" ID="myRolecontrol" DefaultGroup=""></usc:RoleControl>
                            </td>
                        </tr>
                    </table>

                    <table class="datatable">
                        <thead>
                            <tr>
                                <th>Contenitori</th>
                            </tr>
                        </thead>
                        <tr>
                            <td>
                                <telerik:RadToolBar runat="server" ID="tbContainersControl" CssClass="ToolBarContainer"
                                    EnableRoundedCorners="False" EnableShadows="False" Width="100%" OnClientButtonClicking="tbContainersControlButtonClicking">
                                    <Items>
                                        <telerik:RadToolBarButton runat="server" CommandName="ADD" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open_add.png" ToolTip="Aggiungi contenitore esistente"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="ADDNEW" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open_edit.png" ToolTip="Aggiungi nuovo contenitore"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open_remove.png" ToolTip="Elimina contenitore selezionato"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="MASTER" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" ToolTip="Gestione proprietario"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="REJECTION" ImageUrl="~/App_Themes/DocSuite2008/imgset16/copyleft.png" ToolTip="Punto di distribuzione"></telerik:RadToolBarButton>
                                    </Items>
                                </telerik:RadToolBar>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <usc:ContainersControl runat="server" ID="myContainersControl" DefaultGroup="" />
                            </td>
                        </tr>
                    </table>

                </asp:Panel>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
