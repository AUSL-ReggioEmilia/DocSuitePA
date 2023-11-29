<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscRoleRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscRoleRest" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var <%= Me.ClientID %>_uscRoleRest;
        require(["UserControl/uscRoleRest"], function (uscRoleRest) {
            $(function () {
                <%= Me.ClientID %>_uscRoleRest = new uscRoleRest(tenantModelConfiguration.serviceConfiguration, <%= ControlConfiguration %>, "<%= Me.ClientID %>");
                <%= Me.ClientID %>_uscRoleRest.actionToolbarId = "<%= actionToolbar.ClientID %>";
                <%= Me.ClientID %>_uscRoleRest.rolesTreeId = "<%= rolesTree.ClientID %>";
                <%= Me.ClientID %>_uscRoleRest.pnlContentId = "<%= tblRoles.ClientID %>";
                <%= Me.ClientID %>_uscRoleRest.windowManagerId = "<%= RadWindowManagerRole.ClientID %>";
                <%= Me.ClientID %>_uscRoleRest.windowSelRoleId = "<%= windowSelRole.ClientID %>";
                <%= Me.ClientID %>_uscRoleRest.validatorAnyNodeId = "<%= AnyNodeCheck.ClientID%>";
                <%= Me.ClientID %>_uscRoleRest.btnExpandRolesId = "<%= btnExpandRoles.ClientID%>";
                <%= Me.ClientID %>_uscRoleRest.contentRowId = "<%= contentRow.ClientID%>";
                <%= Me.ClientID %>_uscRoleRest.multipleRoles = "<%= MultipleRoles %>";
                <%= Me.ClientID %>_uscRoleRest.onlyMyRoles = <%= OnlyMyRoles.ToString().ToLower() %>;
                <%= Me.ClientID %>_uscRoleRest.requiredValidationEnabled = "<%= Required %>";
                <%= Me.ClientID %>_uscRoleRest.expanded = "<%= Expanded %>";
                <%= Me.ClientID %>_uscRoleRest.dswEnvironmentType = "<%= DSWEnvironmentType.ToString() %>";
                <%= Me.ClientID %>_uscRoleRest.allDataButtonEnabled = "<%= AllDataButtonEnabled %>";
                <%= Me.ClientID %>_uscRoleRest.removeAllDataButtonEnabled = "<%= RemoveAllDataButtonEnabled %>";
                <%= Me.ClientID %>_uscRoleRest.raciButtonEnabled = <%= RACIButtonEnabled.ToString().ToLower() %>;
                <%= Me.ClientID %>_uscRoleRest.fascicleVisibilityTypeButtonEnabled = <%= FascicleVisibilityTypeButtonEnabled.ToString().ToLower() %>;
                <%= Me.ClientID %>_uscRoleRest.lblCaptionId = "<%= lblCaption.ClientID%>";
                <%= Me.ClientID %>_uscRoleRest.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerRole" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelRole" Height="500" Width="650" runat="server" Title="Selezionare settori" />
    </Windows>
</telerik:RadWindowManager>


<telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator" EnableRoundedCorners="false" runat="server" DecoratedControls="Fieldset"></telerik:RadFormDecorator>
<table class="datatable" id="tblRoles" runat="server">
    <tr id="tblHeader">
        <th>
            <asp:Label ID="lblCaption" runat="server" />
            <telerik:RadButton ID="btnExpandRoles"
                CssClass="dsw-vertical-middle"
                runat="server" Width="16px" Height="16px"
                Visible="false" AutoPostBack="false" CausesValidation="false">
                <Image EnableImageButton="true" />
            </telerik:RadButton>
        </th>
    </tr>
    <tr id="contentRow">
        <td>
            <telerik:RadToolBar AutoPostBack="false"
                CssClass="ToolBarContainer"
                EnableRoundedCorners="False"
                EnableShadows="False"
                Visible="false"
                RenderMode="Lightweight"
                ID="actionToolbar" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png" 
                        ToolTip="Selezionare settori" Value="add" CommandName="add" PostBack="false" />
                    <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_delete.png" 
                        ToolTip="Elimina settore" Value="delete" CommandName="delete" PostBack="false" />
                    <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/database_add.png" 
                        ToolTip="Aggiungi tutti settori" CommandName="addAll" PostBack="false" Value="addAll" style="display: none;"/>
                    <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/database_delete.png" 
                        ToolTip="Elimina tutti settori" CommandName="deleteAll" PostBack="false" Value="deleteAll" style="display: none;" />
                    <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/coedit.png" 
                        ToolTip="Seleziona come settore abilitato in scrittura" CommandName="setRaciRole" PostBack="false" Value="setRaciRole" Text="Abilitato in scrittura" style="display: none;" />
                    <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/fascicleRoleGrant.png" 
                        ToolTip="Cambia visibilità" ID="btnFascicleVisibilityType" Value="setFascicleVisibilityType" CommandName="setFascicleVisibilityType" 
                        Text="Rendi i documenti disponibili ai settori autorizzati" CheckOnClick="True" AllowSelfUnCheck="True" />
                </Items>
            </telerik:RadToolBar>
            <div id="decorationZone">
                <fieldset id="fldCurrentTenant" runat="server">
                    <telerik:RadTreeView ID="rolesTree" runat="server" Width="100%" />
                </fieldset>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <asp:CustomValidator runat="server" ID="AnyNodeCheck" ControlToValidate="rolesTree" ValidateEmptyText="true" EnableClientScript="true" Enabled="false" ClientValidationFunction="anyNodeCheck" 
                Display="Dynamic" ErrorMessage="Campo settore obbligatorio" />
        </td>
    </tr>
</table>
