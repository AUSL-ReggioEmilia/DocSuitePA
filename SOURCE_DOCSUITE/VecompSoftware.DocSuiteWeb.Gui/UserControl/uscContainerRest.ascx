<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContainerRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContainerRest" %>


<telerik:RadScriptBlock runat="server"  ID="RadScriptBlock2_Container" >
    <script type="text/javascript">
        var uscContainerRest;
        require(["UserControl/uscContainerRest"], function (uscContainerRest) {
            uscContainerRest = new uscContainerRest(tenantModelConfiguration.serviceConfiguration, "<%= Me.ClientID %>");
            uscContainerRest.pnlContentId = "<%= tblContainers.ClientID %>";
            uscContainerRest.managerId = "<%= RadWindowManagerRole.ClientID %>";

            uscContainerRest.rtvContainersId = "<%= rtvContainers.ClientID %>";
            uscContainerRest.tbContainersControlId = "<%= tbContainersControl.ClientID%>";
            uscContainerRest.rwContainerId = "<%=rwContainerSelector.ClientID%>";
            uscContainerRest.cmbContainerId = "<%= cbContainer.ClientID %>";
            uscContainerRest.btnContainerSelectorOkId = "<%= btnContainerSelectorOk.ClientID %>";
            uscContainerRest.btnContainerSelectorCancelId = "<%= btnContainerSelectorCancel.ClientID %>";

            uscContainerRest.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
            uscContainerRest.treeViewNodesPageSize = <%= ProtocolEnv.TreeViewNodesPageSize %>;
            uscContainerRest.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";

            uscContainerRest.initialize();
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerRole" runat="server">
    <Windows>
        <telerik:RadWindow runat="server" ID="rwContainerSelector" Title="Seleziona Contenitore" Width="650" Height="100" Behaviors="Close, Move, Maximize" RenderMode="Lightweight">
        <ContentTemplate>           
                
                    <table class="datatable" id="ContainerSelectorWindowTable">
                        <tr>
                            <td class="label">Contenitore</td>
                            <td>
                                <telerik:RadComboBox runat="server" CausesValidation="false"
                                    ID="cbContainer" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                    ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" colspan="2">
                                <telerik:RadButton runat="server" ID="btnContainerSelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnContainerSelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
                            </td>
                        </tr>
                    </table>
               
        </ContentTemplate>
    </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>



<table class="datatable" id="tblContainers" runat="server">
    <thead>
        <tr>
            <th>Contenitori</th>
        </tr>
    </thead>
    <tr>
        <td>
            <telerik:RadToolBar AutoPostBack="False" ID="tbContainersControl" CssClass="ToolBarContainer"
                RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CommandName="ADDNEW" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open_add.png" ToolTip="Aggiungi contenitore esistente"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open_remove.png" ToolTip="Elimina contenitore selezionato"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton CommandName="ADDALL" ImageUrl="~/App_Themes/DocSuite2008/imgset16/database_add.png" ToolTip="Aggiungi tutti contenitori"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton CommandName="REMOVEALL" ImageUrl="~/App_Themes/DocSuite2008/imgset16/database_delete.png" ToolTip="Elimina tutti contenitori"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton IsSeparator="true" />
                    <telerik:RadToolBarButton  Value="searchInput" >
                        <ItemTemplate>
                            <telerik:RadTextBox ID="txtSearch" Placeholder="Cerca serie..." runat="server" AutoPostBack="False" Width="200px"></telerik:RadTextBox>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                      <telerik:RadToolBarButton CommandName="SEARCH" Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" Value="search"
                          PostBack="false" />
                    </Items>
            </telerik:RadToolBar>
        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadTreeView ID="rtvContainers" 
                LoadingStatusPosition="BeforeNodeText"
                PersistLoadOnDemandNodes="false" 
                runat="server" Style="margin-top: 10px;" Width="100%">
            </telerik:RadTreeView>
        </td>
    </tr>
</table>