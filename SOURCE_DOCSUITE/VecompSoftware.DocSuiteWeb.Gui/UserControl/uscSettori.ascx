<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscSettori.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSettori" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var <%= Me.ClientID %>_uscSettoriTS;
        require(["UserControl/uscSettori"], function (uscSettori) {
            $(function () {
                <%= Me.ClientID %>_uscSettoriTS = new uscSettori(tenantModelConfiguration.serviceConfiguration);
                <%= Me.ClientID %>_uscSettoriTS.dswEnvironment = "<%= Environment.ToString()%>"
                <%= Me.ClientID %>_uscSettoriTS.contentId = "<%= TableContentControl.ClientID%>";
                <%= Me.ClientID %>_uscSettoriTS.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                <%= Me.ClientID %>_uscSettoriTS.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                <%= Me.ClientID %>_uscSettoriTS.multiSelect = "<%= MultiSelect%>";
                <%= Me.ClientID %>_uscSettoriTS.currentTenantId = "<%= CurrentTenantId.ToString()%>";
                <%= Me.ClientID %>_uscSettoriTS.toolBarId = "<%= ToolBar.ClientID%>";
                <%= Me.ClientID %>_uscSettoriTS.validatorAnyNodeId = "<%= AnyNodeCheck.ClientID%>";
                <%= Me.ClientID %>_uscSettoriTS.btnExpandRolesId = "<%= btnExpandRoles.ClientID%>";
                <%= Me.ClientID %>_uscSettoriTS.contentRowId = "<%= contentRow.ClientID%>";
                <%= Me.ClientID %>_uscSettoriTS.rwSettoriId = "<%= windowSelSettori.ClientID%>";
                <%= Me.ClientID %>_uscSettoriTS.initialize();
            });
        });

        function CanceledClientNodeClicking(sender, eventArgs) {
            eventArgs.set_cancel(true);
        }

        // richiamata quando la finestra viene chiusa
        function <%= Me.ClientID %>_CloseFunction(sender, args) {
            var isAjaxModelResult = <%= If(AjaxReturnModelEnabled, "true", "false") %>;
            if (args.get_argument() !== null) {
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                var ajaxResult = $find("<%= AjaxManager.ClientID%>");
                ajaxResult = "<%= Me.ClientID %>" + "|" + args.get_argument();
                if (isAjaxModelResult === true) {
                    ajaxResult = {
                        ActionName: '<%= AjaxModelActionName %>',
                        Value: JSON.stringify(args.get_argument())
                    };
                    ajaxResult = JSON.stringify(ajaxResult);
                }
                ajaxManager.ajaxRequest(ajaxResult);
            }
        }

        function <%= Me.ClientID %>_CloseUserFunction(sender, args) {
            sender.remove_close(<%= Me.ClientID %>_CloseUserFunction);
            if (args.get_argument()) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|User|" + args.get_argument());
            }
        }

        //richiamata quando viene selezionato conferma e nuovo la finestra rubrica NON viene chiusa
        function  <%= Me.ClientID %>_AddUsersToControl(value) {
            if (value) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|User|" + value);
            }
        }


        function <%= Me.ClientID %>_OpenWindow(width, height, parameters) {
            var url = "../UserControl/CommonSelSettori.aspx?" + parameters;

            var wnd = window.radopen(url, "windowSelSettori");
            wnd.setSize(width, height);
            wnd.add_close(<%=Me.ClientID%>_CloseFunction);
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();

            return false;
        }

        function <%= Me.ClientID %>_OpenUserWindow(width, height, parameters) {
            var url = "../UserControl/CommonSelContactDomain.aspx?" + parameters;

            var wnd = window.radopen(url, "windowSelSettori");
            wnd.setSize(width, height);
            wnd.add_close(<%=Me.ClientID%>_CloseUserFunction);
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();

            return false;
        }


        function <%= Me.ClientID %>_OpenWindowFullSize(parameters) {
            var url = "../UserControl/CommonSelSettori.aspx?" + parameters;

            var wnd = window.radopen(url, "windowSelSettori");
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Minimize)
            wnd.add_close(<%=Me.ClientID%>_CloseFunction);
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();

            return false;
        }

        function <%= ClientID %>_ToolBarButtonClicking(sender, args) {
            var btn = args.get_item();
            switch (btn.get_commandName()) {
                case "add":
                    var selected = '';
                    var tenantNodeSelected = '';
                    var tree = $find("<%= RadTreeSettori.ClientID%>");
                    if (tree != undefined) {
                        var nodes = tree.get_allNodes();
                        for (var i = 0; i < nodes.length; i++) {
                            var node = nodes[i];
                            if (node.get_attributes().getAttribute("SELECTED") == 'TRUE') {
                                selected = selected + "|" + node.get_value();
                            }
                        }
                    }
                    var tenantTree = $find("<%= RadTreeRoleTenant.ClientID%>");
                    if (tenantTree != undefined) {
                        var tenantNodes = tenantTree.get_allNodes();
                        for (var i = 0; i < tenantNodes.length; i++) {
                            var tenantNode = tenantNodes[i];
                            if (tenantNode.get_attributes().getAttribute("SELECTED") == 'TRUE') {
                                tenantNodeSelected = tenantNodeSelected + "|" + tenantNode.get_value();
                            }
                        }
                    }

                    var param = '<%= GetWindowParameters()%>';
                    if (selected != '') {

                        param = param + "&Selected=" + selected;
                    }

                    if (tenantNodeSelected != '') {

                        param = param + "&TenantSelected=" + tenantNodeSelected;
                    }

                    <%= ClientID%>_OpenWindowFullSize(param);
                    args.set_cancel(true);
                    break;
            }
        }
        function <%= Me.ClientID %>_ClearSessionStorage() {
            $(function () {
                var key = "<%= TableContentControl.ClientID%>" + "SelectedRoles";
                if (sessionStorage[key] != null) {
                    sessionStorage.removeItem(key);
                }
            })
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerSettori" PreserveClientState="true" runat="server">
    <Windows>
        <telerik:RadWindow Height="480" ID="windowSelSettori" Modal="true" ReloadOnShow="false" runat="server" Title="Selezione Settori" Width="640" Behaviors="Close,Resize,Maximize,Minimize" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator" EnableRoundedCorners="false" runat="server" DecoratedControls="Fieldset"></telerik:RadFormDecorator>



<table class="datatable" id="tblSettori" runat="server">
    <tr id="tblHeader">
        <th>
            <asp:Label ID="lblCaption" runat="server" />
            <telerik:RadButton ID="btnExpandRoles" CssClass="dsw-vertical-middle" runat="server" Width="16px" Height="16px" Visible="true" AutoPostBack="false" CausesValidation="false">
                <Image EnableImageButton="true" />
            </telerik:RadButton>
        </th>
    </tr>
    <tr id="contentRow"> 
        <td>
            <telerik:RadToolBar AutoPostBack="true" RenderMode="Lightweight" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CausesValidation="False" CommandName="add" ID="btnAdd" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png" ToolTip="Aggiungi settori" />
                    <telerik:RadToolBarButton CausesValidation="False" CommandName="user" ID="btnADUser" Visible="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" runat="server" ToolTip="Aggiungi utenti" />
                    <telerik:RadToolBarButton CausesValidation="False" CommandName="delete" ID="btnDelete" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_delete.png" ToolTip="Elimina settore" />
                    <telerik:RadToolBarButton CausesValidation="False" CommandName="viewMode" ID="btnViewMode" ToolTip="Cambia visualizzazione" />
                    <telerik:RadToolBarButton CausesValidation="False" CommandName="viewManagers" ID="btnViewManagers" ToolTip="Cambia visualizzazione" Text="Visualizza responsabili" CheckOnClick="True" AllowSelfUnCheck="True" />
                    <telerik:RadToolBarButton CausesValidation="False" CommandName="copiaConoscenza" ID="btnCopiaConoscenza">
                        <ItemTemplate>
                            <span class="templateText">Spunta per Copia Conoscenza</span>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton CausesValidation="False" ID="btnPrivacy" Value="checkPrivacy" CommandName="checkPrivacy" ToolTip="Riservatezza/privacy" Text="Riservatezza/privacy" />
                    <telerik:RadToolBarButton CausesValidation="False" ID="btnFascicleVisibilityType" Value="checkFascicleVisibilityType" CommandName="checkFascicleVisibilityType" ToolTip="Cambia Visibilità" Text="Rendi i documenti disponibili ai settori autorizzati" CheckOnClick="True" AllowSelfUnCheck="True" />
                    <telerik:RadToolBarButton CausesValidation="False" ID="btnPropagateAuthorizations" Value="checkPropagateAuthorizations" CommandName="checkPropagateAuthorizations" ToolTip="Propaga autorizzazioni alle sottocartelle" Text="Propaga le autorizzazioni alle sottocartelle" CheckOnClick="True" AllowSelfUnCheck="True" />
                </Items>
            </telerik:RadToolBar>
            <div id="decorationZone">
                <fieldset id="fldCurrentTenant" style="display: none;" runat="server">
                    <legend>
                        <asp:Label ID="lblCurrentTenant" CssClass="strongRiLabel" runat="server" />
                    </legend>
                    <telerik:RadTreeView ID="RadTreeSettori" runat="server" Width="100%" CausesValidation="False" OnClientNodeClicked="OnClientNodeClickedExpand" />
                </fieldset>
                <fieldset id="fldOtherTenant" style="display: none;" runat="server">
                    <legend>
                        <asp:Label ID="lblOtherTenant" CssClass="strongRiLabel" runat="server" />
                    </legend>
                    <telerik:RadTreeView ID="RadTreeRoleTenant" runat="server" Width="100%" CausesValidation="False" OnClientNodeClicked="OnClientNodeClickedExpand" />
                </fieldset>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <asp:CustomValidator runat="server" ID="AnyNodeCheck" ControlToValidate="RadTreeSettori" ValidateEmptyText="true" EnableClientScript="true" ClientValidationFunction="anyNodeCheck" Display="Dynamic" ErrorMessage="Campo Settore Obbligatorio" />
        </td>
    </tr>
</table>
