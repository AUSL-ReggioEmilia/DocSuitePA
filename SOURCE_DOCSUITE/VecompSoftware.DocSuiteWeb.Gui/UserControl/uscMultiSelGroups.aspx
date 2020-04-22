<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="uscMultiSelGroups.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMultiSelGroups" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Assegna gruppi" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
        
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseGroupsWindow() {
                var treeView = $find('<%= RadTreeGroups.ClientID %>');
                var values = '';
                var rootNode = treeView.get_nodes().getNode(0);
                var selectedNodes = rootNode.get_allNodes();
                selectedNodes = selectedNodes.filter(function (item) {
                    return item.get_checked() == true;
                });
                if (selectedNodes.length > 0) {
                    $.each(selectedNodes, function (i, item) {
                        values = values + item.get_value() + ';';
                    });
                    var oWindow = GetRadWindow();
                    oWindow.close(values);
                    return false;
                }
                else {
                    alert('Selezionare almeno un gruppo.');
                    return false;
                }               
            }
        </script>
    </telerik:RadScriptBlock>

      <asp:Panel runat="server" DefaultButton="btnSearch" CssClass="datatable">
        <asp:TextBox ID="txtSearch" runat="server" />
        <asp:Button ID="btnSearch" runat="server" Text="Cerca" />
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView ID="RadTreeGroups" runat="server" EnableViewState="false" Checkboxes="true" Height="100%" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConfirm" runat="server" OnClientClicked="CloseGroupsWindow" Text="Conferma Selezione"/>
</asp:Content>
