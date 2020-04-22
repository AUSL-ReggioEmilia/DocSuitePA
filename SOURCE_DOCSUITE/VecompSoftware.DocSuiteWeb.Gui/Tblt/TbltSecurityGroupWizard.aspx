<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltSecurityGroupWizard.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltSecurityGroupWizard"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register TagPrefix="usc" TagName="uscGroupDetails" Src="~/UserControl/uscGroupDetails.ascx" %>
<%@ Register Src="../UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc3" %>

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
        </script>
    </telerik:RadScriptBlock>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">

    <asp:Panel ID="pnlMain" runat="server" Height="100%">
        <telerik:RadSplitter runat="server" ID="Splitter1" Width="100%" Height="100%">
            <telerik:RadPane runat="server" ID="Pane1">
                <telerik:RadToolBar RenderMode="Lightweight" EnableRoundedCorners="false" ID="ToolBarSearchConfig" OnButtonClick="ToolBarSearch_ButtonClick" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton Value="btnConfigs">
                            <ItemTemplate>
                                <label>Gruppi configurati su:</label>
                                <telerik:RadComboBox runat="server" ID="rcbConfigType" AutoPostBack="true" OnSelectedIndexChanged="rcbConfigType_SelectedIndexChanged" />                                
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                         <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadToolBar RenderMode="Lightweight" EnableRoundedCorners="false" ID="ToolBarSearchContainer" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton Value="btnContainers">
                            <ItemTemplate>
                                <telerik:RadComboBox runat="server" ID="rcbContainer" AllowCustomText="true" Filter="Contains" AutoPostBack="true" Width="300px" EnableVirtualScrolling="true" DropDownHeight="200px" DataTextField="Name" DataValueField="Id" />
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadToolBar RenderMode="Lightweight" EnableRoundedCorners="false" ID="ToolBarSearchRole" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton Value="btnRolesUsc" Width="100%">
                            <ItemTemplate>
                                <telerik:RadComboBox runat="server" ID="rcbRole" AllowCustomText="true" Filter="Contains" AutoPostBack="true" Width="300px" EnableVirtualScrolling="true" DropDownHeight="200px" DataTextField="Name" DataValueField="id" />
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadTreeView ID="RadTreeGroups" runat="server" CheckBoxes="True" Height="90%" Width="100%">
                    <Nodes>
                        <telerik:RadTreeNode Expanded="true" runat="server" Text="Gruppi" Checkable="false"/>
                    </Nodes>
                </telerik:RadTreeView>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server" ID="Bar1" />

            <telerik:RadPane runat="server">
                <asp:Panel runat="server" ID="pnlGroupDetails" Height="100%">
                    <usc:uscGroupDetails runat="server" ID="groupDetails" />
                </asp:Panel>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnSave" Text="Conferma" OnClientClicking="showLoadingPanel"></telerik:RadButton>
    <telerik:RadButton runat="server" ID="btnBack" Text="Indietro"></telerik:RadButton>
</asp:Content>
