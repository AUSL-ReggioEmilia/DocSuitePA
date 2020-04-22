<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserCambioResponsabile.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserCambioResponsabile" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Collaborazione - Autorizzazioni" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbChangeUser" EnableViewState="false">
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

            function UpdateGroups() {
                GetRadWindow().BrowserWindow.Refresh();
            }
        </script>
    </telerik:RadScriptBlock>
    <table class="dataform">
        <tr>
            <td class="label" style="width: 30%;"> Selezione Settore </td>
            <td>
                <asp:dropdownlist AutoPostBack="True" id="ddlRoles" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table width="100%" cellpadding="10px" cellspacing="10px">
        <tr>
            <td valign="top" width="50%">
                <table width="100%" class="datatable">
                    <tr>
                        <th>Origine</th>
                    </tr>
                    <tr>
                        <td>
                            <telerik:RadTreeView BorderColor="black" Height="100%" ID="rtvOrigin" runat="server" Width="100%" />
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top" width="50%">
                <table width="100%" class="datatable">
                    <tr>
                        <th>Destinazione</th>
                    </tr>
                    <tr>
                        <td>
                            <telerik:RadTreeView BorderColor="black" Height="100%" ID="rtvDestination" runat="server" Width="100%" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnConfirm" runat="server" Text="Conferma"></asp:button>
</asp:Content>



