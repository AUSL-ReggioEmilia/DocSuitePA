<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltSecurityGroupWizard.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltSecurityGroupWizard"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register TagPrefix="usc" TagName="uscGroupDetails" Src="~/UserControl/uscGroupDetails.ascx" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            function showLoadingPanel(sender, args) {
                currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= pnlMain.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function closeLoadingPanel() {
                currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= pnlMain.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function RadTreeGroups_NodeClicking(sender, args) {
                let nodeType = args.get_node().get_attributes().getAttribute("NodeType");

                if (nodeType !== "SecurityGroup") {
                    args.set_cancel(true);
                } 
            }

            function RadTreeGroups_NodeChecking(sender, args) {
                let nodeIsDefaultChecked = args.get_node().get_attributes().getAttribute("DefaultChecked");

                if (nodeIsDefaultChecked) {
                    args.set_cancel(true);
                }
            }
        </script>
    </telerik:RadScriptBlock>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">

    <asp:Panel ID="pnlMain" runat="server" Height="100%">
        <telerik:RadSplitter runat="server" ID="Splitter1" Width="100%" Height="100%">
            <telerik:RadPane runat="server" ID="TreeViewPane">
                <telerik:RadToolBar RenderMode="Lightweight" 
                                    EnableRoundedCorners="false" 
                                    ID="SearchToolbar" 
                                    runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton Value="searchBtn">
                            <ItemTemplate>
                                <telerik:RadTextBox ID="searchInput" 
                                                    Placeholder="Cerca..." 
                                                    runat="server" 
                                                    Width="200px"></telerik:RadTextBox>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton Text="Cerca" CommandName="search" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadTreeView ID="RadTreeGroups" OnClientNodeClicking="RadTreeGroups_NodeClicking" 
                                     OnClientNodeChecking="RadTreeGroups_NodeChecking"
                                     runat="server" LoadingMessage="" LoadingStatusPosition="BeforeNodeText"
                                     CheckBoxes="True" Height="90%" Width="100%" />
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server" ID="Bar1" />

            <telerik:RadPane runat="server">
                <asp:Panel runat="server" ID="pnlGroupDetails" Height="100%" Visible="false">
                    <usc:uscGroupDetails runat="server" ID="groupDetails" />
                    <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" Width="100%" ID="rpbGroups">
                        <Items>
                            <telerik:RadPanelItem Value="rpiGroupDetails" Text="Impostazioni di sicurezza" Expanded="true" runat="server">
                                <ContentTemplate>
                                    <telerik:RadSplitter runat="server" ID="grpSplitter" Width="100%">
                                        <telerik:RadPane runat="server" ID="groupDetailsPane" ShowContentDuringLoad="false">
                                        </telerik:RadPane>
                                    </telerik:RadSplitter>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                        </Items>
                    </telerik:RadPanelBar>
                </asp:Panel>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnSave" Text="Conferma" OnClientClicking="showLoadingPanel"></telerik:RadButton>
    <telerik:RadButton runat="server" ID="btnBack" Text="Indietro"></telerik:RadButton>
</asp:Content>
