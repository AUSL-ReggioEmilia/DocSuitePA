<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserAbsentManagers.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserAbsentManagers" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Collaborazione - Direttori assenti" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function Close(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
            
            function OnClientItemSelected(sender, eventArgs) {
                var item = eventArgs.get_item();
                var manager = sender.get_attributes().getAttribute("manager_node");
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("manageManager" + "~" + manager + "~" + item.get_value() + "~" + item.get_text());
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-content">
                            <telerik:RadTreeView BorderColor="black" Height="100%" ID="rtvManagers" runat="server" Width="100%" CheckBoxes="true" MultipleSelect="true">
                                <Nodes>
                                    <telerik:RadTreeNode Checkable="false" Font-Bold="true" ImageUrl="~/Comm/Images/Interop/Ruolo.gif" Expanded="true" runat="server" Selected="true" Text="Direttori" EnableContextMenu="True" />
                                </Nodes>
                            </telerik:RadTreeView>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConfirm" runat="server" Text="Conferma"></asp:Button>
</asp:Content>



