<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMassimarioScarto.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMassimarioScarto" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscMassimarioScarto;
        require(["UserControl/uscMassimarioScarto"], function (UscMassimarioScarto) {
            $(function () {
                uscMassimarioScarto = new UscMassimarioScarto(tenantModelConfiguration.serviceConfiguration);
                uscMassimarioScarto.treeMassimarioId = "<%= rtvMassimario.ClientID %>";
                uscMassimarioScarto.toolBarSearchId = "<%= ToolBarSearch.ClientID %>";
                uscMassimarioScarto.hideCanceledFilter = <%= HideCanceledFilterJson %>;
                uscMassimarioScarto.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscMassimarioScarto.initialize();
            });
        });

        function treeView_ClientNodeClicked(sender, args) {
            uscMassimarioScarto.treeView_ClientNodeClicked(sender, args);
        }

        function treeView_ClientNodeExpanding(sender, args) {
            uscMassimarioScarto.treeView_ClientNodeExpanding(sender, args);
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
    <Items>
        <telerik:RadToolBarButton Value="searchDescription">
            <ItemTemplate>
                <telerik:RadTextBox ID="txtSearchName" EmptyMessage="Nome" runat="server" Width="150px"></telerik:RadTextBox>
            </ItemTemplate>
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton Value="searchCode">
            <ItemTemplate>
                <telerik:RadTextBox ID="txtSearchCode" EmptyMessage="Codice" runat="server" Width="90px"></telerik:RadTextBox>
            </ItemTemplate>
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton Value="includeCancel">
            <ItemTemplate>
                <telerik:RadButton ID="btnIncludeCancel" Text="Includi annullati" Style="margin-left: 3px;" AutoPostBack="false" runat="server" ToggleType="CheckBox" ButtonType="LinkButton">
                    <ToggleStates>
                        <telerik:RadButtonToggleState Text="Includi annullati" PrimaryIconCssClass="rbToggleCheckboxChecked" />
                        <telerik:RadButtonToggleState Text="Includi annullati" PrimaryIconCssClass="rbToggleCheckbox" />
                    </ToggleStates>
                </telerik:RadButton>
            </ItemTemplate>
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton IsSeparator="true" />
        <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
    </Items>
</telerik:RadToolBar>
<telerik:RadTreeView ID="rtvMassimario" LoadingStatusPosition="BeforeNodeText" OnClientNodeClicked="treeView_ClientNodeClicked"
    OnClientNodeExpanding="treeView_ClientNodeExpanding" runat="server" Style="margin-top: 10px;" Width="100%">
    <Nodes>
        <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Massimario di scarto" Value="0" />
    </Nodes>
</telerik:RadTreeView>

  <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
