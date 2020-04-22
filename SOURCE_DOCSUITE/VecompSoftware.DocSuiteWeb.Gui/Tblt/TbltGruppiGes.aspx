<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltGruppiGes.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltGruppiGes" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

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
    <telerik:RadTreeView EnableViewState="false" ID="RadTreeViewSelectedGroup" runat="server" />
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlInserimento" runat="server" Width="100%">
        <table id="tableRen" class="datatable">
            <tr runat="server" id="trOldName" visible="false">
                <td class="label" width="25%">
                    Nome attuale:
                </td>
                <td>
                    <asp:TextBox ID="txtOldName" MaxLength="100" runat="server" Width="100%" Enabled="false" />
                </td>
            </tr>
            <tr runat="server" id="trName" visible="true">
                <td class="label" width="25%">
                    Nome:
                </td>
                <td>
                    <telerik:RadTextBox ID="txtName" MaxLength="100" runat="server" Width="100%" />
                    <br>
                    <asp:RequiredFieldValidator ControlToValidate="txtName" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ID="rfvNome" runat="server" />
                </td>
            </tr>
            <tr runat="server" id="trAllUsers" visible="true">
                <td class="label" width="25%">
                    Gruppo 'All Users'?:
                </td>
                <td>
                    <asp:CheckBox ID="cbxAllUsers" runat="server" Checked="false"/>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnConferma" runat="server" Text="Conferma" />
</asp:Content>
