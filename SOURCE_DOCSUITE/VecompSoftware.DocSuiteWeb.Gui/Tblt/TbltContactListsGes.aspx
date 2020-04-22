<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltContactListsGes.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContactListsGes" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(obj) {
                var oWindow = GetRadWindow();
                oWindow.close(obj);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlInserimento" runat="server" Width="100%">
        <table id="tableList" class="datatable">
            <tr runat="server" id="trName" visible="true">
                <td class="label">
                    Nome:
                </td>
                <td>
                    <telerik:RadTextBox ID="txtName" MaxLength="100" runat="server" Width="100%" />
                    <br>
                    <asp:RequiredFieldValidator ControlToValidate="txtName" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ID="rfvNome" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnConferma" runat="server" Text="Conferma" />
</asp:Content>
