<%@ Page AutoEventWireup="false" CodeBehind="SelContattiIPA.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SelContattiIPA" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function OpenWindow(name, url) {
                var wnd = $find(name);
                wnd.setUrl(url);
                wnd.show();
                return false;
            }

            function ReturnValuesJSon(action, idContact, contact, close) {
                var contactIpa = new Object();
                contactIpa.Action = action;
                contactIpa.IdContact = idContact;
                contactIpa.Contact = contact;
                if (close == "true") {
                    CloseWindow(contactIpa);
                } else {
                    GetRadWindow().BrowserWindow.<%= CallerId%>_UpdateManual(contactIpa.Contact, contactIpa.Action);
                }
            }

            function CloseWindow(contact) {
                var oRadWindow = GetRadWindow();
                oRadWindow.close(contact);
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerDetail" runat="server">
        <Windows>
            <telerik:RadWindow Height="400" ID="windowDetails" ReloadOnShow="false" runat="server" Title="Contatto - Proprietà" Width="450" />
        </Windows>
    </telerik:RadWindowManager>

    <table class="datatable">
        <tr>
            <td class="label labelPanel col-dsw-2">Ricerca:</td>
            <td>
                <asp:Panel runat="server" ID="pnlCerca" DefaultButton="btnSearch" CssClass="dsw-display-inline">
                    <asp:TextBox ID="txtSearch" MaxLength="30" runat="server" Width="200px" />
                </asp:Panel>
                <asp:CheckBoxList ID="rblFilterTypeObject" runat="server" RepeatDirection="Horizontal">                
                    <asp:ListItem Text="Amministrazione" Value="Amministrazione" Selected="True" />
                    <asp:ListItem Text="AOO" Value="aoo"  Selected="True" />
                    <asp:ListItem Text="OU" Value="organizationalUnit"  Selected="True" />
                </asp:CheckBoxList>
                <telerik:RadButton ID="btnSearch" runat="server" Text="Ricerca">
                    <Icon PrimaryIconUrl="../App_Themes/DocSuite2008/images/search-transparent.png" />
                </telerik:RadButton>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView ID="tvwIPA" runat="server" Height="100%"   />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <table class="col-dsw-10">
        <tr>
            <td>
                <telerik:RadButton ID="btnConferma" runat="server" Text="Conferma" />
                <telerik:RadButton ID="btnConfermaNuovo" runat="server" Text="Conferma e Nuovo" />
            </td>

            <td class="dsw-text-right">
                <telerik:RadButton Enabled="False" ID="cmdDetail" runat="server" Text="Proprietà" />
            </td>
        </tr>
    </table>
</asp:Content>
