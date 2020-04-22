<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SelUsersGroup.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SelUsersGroup" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Utente" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script language="javascript" type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }

            function OnClientNodeClicking(seder, args) {
                args.get_domEvent().ctrlKey = true;
            }

            function OnClientNodeClicked(seder, args) {
                var node = args.get_node();
                node.check();
                node.unselect();
            }
        </script>
    </telerik:RadScriptBlock>

    <table style="border: 0; padding: 0; margin: 0; width: 100%">
        <tr>
            <td style="text-align: left; white-space: nowrap; overflow: no-display;">
                <asp:Panel ID="pnlButtons" runat="server">
                    <telerik:RadTreeView EnableViewState="true" ID="rtvAdUsers" runat="server" MultipleSelect="true" CheckBoxes="True" OnClientNodeClicking="OnClientNodeClicking" OnClientNodeClicked="OnClientNodeClicked">
                        <Nodes>
                            <telerik:RadTreeNode Checkable="false" Expanded="true" runat="server" Selected="false" Text="Utenti" />
                        </Nodes>

                    </telerik:RadTreeView>

                    <table style="border: 0; padding: 0; margin: 0; width: 100%">
                        <tr>
                            <td style="text-align: left; white-space: nowrap; overflow: no-display;">
                                <asp:Button ID="btnConferma" runat="server" Text="Conferma Selezione" Width="130px" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
