<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscWorkflowDesignerValidations.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscWorkflowDesignerValidations" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var uscWorkflowDesignerValidations;
        require(["UserControl/uscWorkflowDesignerValidations"], function (uscWorkflowDesignerValidations) {
            $(function () {
                uscWorkflowDesignerValidations = new uscWorkflowDesignerValidations(tenantModelConfiguration.serviceConfiguration);
                uscWorkflowDesignerValidations.pageContentId = "<%=PageContent.ClientID%>";
                uscWorkflowDesignerValidations.actionToolbarId = "<%=actionToolbar.ClientID%>";
                uscWorkflowDesignerValidations.rtvValidationRulesId = "<%=rtvValidationRules.ClientID%>";
                uscWorkflowDesignerValidations.radWindowManagerId = "<%=RadWindowManagerContacts.ClientID%>";
                uscWorkflowDesignerValidations.lblCaptionId = "<%=lblCaption.ClientID%>";
                uscWorkflowDesignerValidations.tblHeaderId = "<%=tblHeader.ClientID%>";
                uscWorkflowDesignerValidations.lblDSWMessageContainerId = "<%=lblDSWMessageContainer.ClientID%>";
                uscWorkflowDesignerValidations.initialize();
            });
        });
    </script>
    <style>
        .rtImg {
            height: 12px;
            width: 12px;
        }
    </style>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerContacts" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelValidation" ReloadOnShow="true" runat="server" Title="Selezione Validazione" Behaviors="Maximize,Resize,Close"/>
        <telerik:RadWindow ID="windowSelModifyValidation" ReloadOnShow="true" runat="server" Title="Selezione Validazione" Behaviors="Maximize,Resize,Close"/>
    </Windows>
</telerik:RadWindowManager>

<table class="datatable" id="tblValidationRules" runat="server">
    <tr id="tblHeader" runat="server">
        <th>
            <asp:Label ID="lblCaption" runat="server" />
        </th>
    </tr>
    <tr id="lblDSWMessageContainer" style="display: none;" runat="server">
        <th>
            <asp:Label ID="lblDSWMessage" runat="server" Text="Funzionalità non supportata per environment di workflow scelto" />
        </th>
    </tr>
    <tr id="contentRow">
        <td>
            <telerik:RadToolBar AutoPostBack="false" ID="actionToolbar" runat="server" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" RenderMode="Lightweight" Width="100%">
                <Items>
                    <telerik:RadToolBarButton ToolTip="Aggiungi" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                    <telerik:RadToolBarButton ToolTip="Modifica" Enabled="false" Value="modify" Text="Modifica" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                    <telerik:RadToolBarButton ToolTip="Elimina" Enabled="false" Value="delete" Text="Elimina" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
                </Items>
            </telerik:RadToolBar>
            <div id="validationRulesId">
                <telerik:RadTreeView ID="rtvValidationRules" runat="server" Width="100%">
                    <Nodes>
                        <telerik:RadTreeNode Expanded="true" Font-Bold="true" runat="server" Text="Regole" Value="Root" />
                    </Nodes>
                </telerik:RadTreeView>
            </div>
        </td>
    </tr>
</table>
