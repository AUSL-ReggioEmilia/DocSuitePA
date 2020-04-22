<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContainerConstraintOptions.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContainerConstraintOptions" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var uscContainerConstraintOptions;
        require(["UserControl/UscContainerConstraintOptions"], function (UscContainerConstraintOptions) {
            $(function () {
                uscContainerConstraintOptions = new UscContainerConstraintOptions(tenantModelConfiguration.serviceConfiguration);
                uscContainerConstraintOptions.rtbConstraintActionsId = "<%= rtbConstraintActions.ClientID %>";
                uscContainerConstraintOptions.rtvConstraintsId = "<%= rtvConstraints.ClientID %>";
                uscContainerConstraintOptions.managerWindowsId = "<%= RadWindowManagerUpload.ClientID %>";
                uscContainerConstraintOptions.windowManageConstraintId = "<%= windowManageConstraint.ClientID %>";
                uscContainerConstraintOptions.txtConstraintNameId = "<%= txtConstraintName.ClientID %>";
                uscContainerConstraintOptions.btnConfirmId = "<%= btnConfirm.ClientID %>";
                uscContainerConstraintOptions.splPageContentId = "<%= splPageContent.ClientID %>";
                uscContainerConstraintOptions.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscContainerConstraintOptions.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscContainerConstraintOptions.windowManagerId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID %>";
                uscContainerConstraintOptions.initialize();
            });
        });

        function loadConstraints(idSeries) {
            $(function () {
                setTimeout(function () {
                    var uscContainerDossierOptionsInstance = $("#<%= splPageContent.ClientID %>").data();
                    if (jQuery.isEmptyObject(uscContainerDossierOptionsInstance)) {
                        loadConstraints(idSeries);
                        return;
                    }
                    uscContainerConstraintOptions.loadConstraints(idSeries);
                }, 200);                
            });
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerUpload" runat="server">
    <Windows>
        <telerik:RadWindow Behaviors="Close" DestroyOnClose="true" ReloadOnShow="True" Height="230px" Id="windowManageConstraint" runat="server" Title="Gestione obbligo trasparenza" Width="400px">
            <ContentTemplate>
                <div class="dsw-panel">
                    <div class="dsw-panel-content">
                        <b>Titolo dell'obbligo: </b>
                        <telerik:RadTextBox runat="server" ID="txtConstraintName" Width="100%" Style="margin-top: 3px"></telerik:RadTextBox>
                    </div>
                </div>                
                <div class="window-footer-wrapper">
                    <telerik:RadButton runat="server" ID="btnConfirm" AutoPostBack="false" Text="Conferma" />
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<telerik:RadSplitter runat="server" Orientation="Horizontal" ID="splPageContent">
    <telerik:RadPane Height="100%" runat="server">
        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="false" ExpandMode="MultipleExpandedItems" Width="100%">
            <Items>
                <telerik:RadPanelItem Text="Gestione obblighi trasparenza" Expanded="true" />
            </Items>
        </telerik:RadPanelBar>
        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="rtbConstraintActions" runat="server" Width="100%">
            <Items>
                <telerik:RadToolBarButton ToolTip="Crea obbligo trasparenza" CheckOnClick="false" Checked="false" Value="createConstraint" ImageUrl="../App_Themes/DocSuite2008/imgset16/add.png" />
                <telerik:RadToolBarButton ToolTip="Modifica obbligo trasparenza" CheckOnClick="false" Checked="false" Value="editConstraint" ImageUrl="../App_Themes/DocSuite2008/imgset16/pencil.png" />
                <telerik:RadToolBarButton ToolTip="Rimuovi obbligo trasparenza" CheckOnClick="false" Checked="false" Value="removeConstraint" ImageUrl="../App_Themes/DocSuite2008/imgset16/remove.png" />
            </Items>
        </telerik:RadToolBar>
        <div class="treeViewWrapper">
            <telerik:RadTreeView runat="server" ID="rtvConstraints">
                <Nodes>
                    <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Obblighi trasparenza" Value="" />
                </Nodes>
            </telerik:RadTreeView>
        </div>
    </telerik:RadPane>
</telerik:RadSplitter>