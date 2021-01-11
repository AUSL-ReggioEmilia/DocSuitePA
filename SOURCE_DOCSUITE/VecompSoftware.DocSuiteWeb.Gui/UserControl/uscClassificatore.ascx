<%@ Control Language="vb" AutoEventWireup="false" Codebehind="uscClassificatore.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscClassificatore" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var <%= Me.ClientID %>_uscClassificatoreTS;
        require(["UserControl/uscClassificatore"], function (uscClassificatore) {
            $(function () {
                <%= Me.ClientID %>_uscClassificatoreTS = new uscClassificatore(tenantModelConfiguration.serviceConfiguration);                
                <%= Me.ClientID %>_uscClassificatoreTS.contentId = "<%= TableContentControl.ClientID%>";
                <%= Me.ClientID %>_uscClassificatoreTS.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                <%= Me.ClientID %>_uscClassificatoreTS.ajaxManagerId = "<%= AjaxManager.ClientID %>";   
                <%= Me.ClientID %>_uscClassificatoreTS.anyNodeCheckId = "<%= AnyNodeCheck.ClientID %>";
                
                <%= Me.ClientID %>_uscClassificatoreTS.initialize();
            });
        });


        // richiamata quando la finestra viene chiusa
        function <%= Me.ClientID %>_CloseFunction(sender, args) {
            if (args.get_argument() !== null) {
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                ajaxManager.ajaxRequest("<%= Me.ClientID %>" + "|" + args.get_argument());
            }
        }
        
        function <%= ClientID%>_OpenWindow(width, height, parameters) {
            var url = "../UserControl/CommonSelCategory.aspx?" + parameters;

            var wnd = window.radopen(url, "windowSelCategory");
            wnd.setSize(width, height);
            wnd.add_close(<%=Me.ClientID%>_CloseFunction);
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();
        }

        function <%= Me.ClientID %>_ClearSessionStorage() {
            $(function () {
                var key = "<%= MainTable.ClientID%>" + "SelectedCategories";
                if (sessionStorage[key] != null) {
                    sessionStorage.removeItem(key);
                }
            })
        }
    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerCategory" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelCategory" ReloadOnShow="false" runat="server" Title="Selezione Classificatore" />
    </Windows>
</telerik:RadWindowManager>
<table class="datatable" runat="server" id="MainTable">
    <tr>
        <th id="tblHeader" runat="server">
            <asp:Label runat="server" ID="lblCaption" Text="Classificazione" />
        </th>
    </tr>
    <tr>
        <td>
            <telerik:RadToolBar AutoPostBack="true" RenderMode="Lightweight" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/drawer_add.png" ToolTip="Selezione Classificatore" CommandName="add" />
                    <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/drawer_delete.png" ToolTip="Elimina Classificazione" CommandName="delete" />
                </Items>
            </telerik:RadToolBar>
            <telerik:RadTreeView ID="RadTreeCategory" runat="server" Width="100%" />
        </td>
    </tr>
</table>
<asp:CustomValidator runat="server" id="AnyNodeCheck" ControlToValidate="RadTreeCategory" ValidateEmptyText="true" EnableClientScript="true" ClientValidationFunction="anyNodeCheck" Display="Dynamic" ErrorMessage="Campo Classificazione Obbligatorio" />