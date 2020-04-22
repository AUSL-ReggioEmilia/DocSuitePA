<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CheckPecEmailAddress.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CheckPecEmailAddress" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadAjaxManagerProxy ID="radAMProxy" runat="server">
    </telerik:RadAjaxManagerProxy>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function GetRadWindow() {
                var wnd = null;
                if (window.radWindow) wnd = window.radWindow;
                else if (window.frameElement.radWindow) wnd = window.frameElement.radWindow;
                return wnd;
            }
            function CloseWithArg(flag) {
                if (flag) {
                    var wnd = GetRadWindow();
                    if (wnd) {
                        wnd.close();
                        if (wnd.BrowserWindow) wnd.BrowserWindow.RefreshOnPECAddressesInserted();
                    }
                }
            }
        </script>
    </telerik:RadCodeBlock>
    <table height="100%" width="100%">
        <tr>
            <td style="padding: 2px">
                <asp:Label ID="lblMessage" runat="server" EnableViewState="false" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="content-wrapper">
                <div class="content">
                    <telerik:RadListView ID="dlstAddresses" runat="server" DataKeyNames="Id" Width="100%">
                        <ItemTemplate>
                            <asp:Panel ID="pnlAggiungi" runat="server">
                                <table id="Table1" class="dataform">
                                    <tr>
                                        <td class="label" width="40%">   
                                            <asp:Label id="idPecAddressName" runat="server"></asp:Label>                                                                 
                                        </td>
                                        <td width="60%" colspan="2">
                                            <telerik:RadTextBox ID="txtCertifiedMail" runat="server" Width="100%" MaxLength="100"  ></telerik:RadTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>    
                                            <asp:RequiredFieldValidator ID="reqvalCertifiedMail" runat="server" 
                                                ControlToValidate="txtCertifiedMail"
                                                ErrorMessage="Informazione richiesta"
                                                Display="Dynamic" />
                                            <asp:RegularExpressionValidator id="rexvalCertifiedMail" runat="server"
                                                ControlToValidate="txtCertifiedMail"
                                                ErrorMessage="Formato non corretto"
                                                ValidationExpression="[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ItemTemplate>
                    </telerik:RadListView>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblError" runat="server" EnableViewState="false" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlFooter" runat="server">
                    <div style="float: left;">
                        <telerik:RadAjaxPanel ID="pnlSubmit" runat="server" EnableAJAX="true">
                            <asp:Button ID="btnConferma" Text="Conferma" runat="server"></asp:Button>
                            <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
                                <script type="text/javascript">
                                    CloseWithArg(<%= Me.CloseMe %>);
                                </script>
                            </telerik:RadScriptBlock>
                        </telerik:RadAjaxPanel>
                    </div>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
