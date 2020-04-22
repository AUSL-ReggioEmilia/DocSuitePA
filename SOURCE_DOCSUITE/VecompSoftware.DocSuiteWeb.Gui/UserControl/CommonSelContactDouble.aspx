<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonSelContactDouble.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelContactDouble" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Contatto" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="scriptBlock">
        <script  language="javascript" type="text/javascript">
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }
            
            function ReturnMultiValue()
            {
                var values = new Array();
                values = GetCheckedValues();
                CloseWindow(values);
            }
            
            function GetCheckedValues()
            {
                var treeView = $find('<%= tvwContacts.ClientID %>');
                document.getElementById('<%= txtIdContact.ClientID %>').value = "";
                GetCheckedNodes(treeView.get_nodes().getNode(0));
                var contacts = document.getElementById('<%= txtIdContact.ClientID %>').value;

                return contacts.slice(0, -1).split(';');
            }

            function GetCheckedNodes(node) {
                if (node.get_nodes().get_count() == 0) {
                    return true;
                }
                for (var i = 0; i < node.get_nodes().get_count(); i++) {
                    var myNode = node.get_nodes().getNode(i);
                    if (myNode.get_checked()) {
                        document.getElementById('<%= txtIdContact.ClientID %>').value += myNode.get_value() + ";";
                    }
                    GetCheckedNodes(myNode);
                }
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
        </script>
    </telerik:RadScriptBlock>

<telerik:RadTreeView runat="server" ID="tvwContacts" CheckBoxes="true" MultipleSelect="true">
    <Nodes>
        <telerik:RadTreeNode runat="server" Text="Contatti" Font-Bold="true" Checkable="false" Expanded="true"></telerik:RadTreeNode>
    </Nodes>
</telerik:RadTreeView>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <table>
        <tr>
            <td valign="top" align="left" width="50%">
                <asp:Button ID="btnConferma" runat="server" CausesValidation="false" Text="Conferma Selezione" Width="140px" OnClientClick="return ReturnMultiValue();"></asp:Button>
            </td>
            <td align="right" width="50%">
                <asp:TextBox ID="txtIdContact" runat="server" Width="16px" CssClass="hiddenField"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>