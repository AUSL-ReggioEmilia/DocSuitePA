<%@ Page AutoEventWireup="false" CodeBehind="UtltUserGroup.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltUserGroup" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Utenti - Lista Gruppi" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        function openAlertWindow(domain) {
            radalert('La gestione SecurityGroup/Multidominio non è attiva nel sistema e quindi è stato utilizzato il dominio di default: ' + domain, 300, 100, 'Info', "", "../App_Themes/DocSuite2008/imgset32/information.png");
        }
    </script>
</telerik:RadScriptBlock>
    <asp:Panel runat="server" DefaultButton="btnSearch">
        <table class="datatable">
            <tr>
                <td class="label" style="width: 20%">Dominio:</td>
                <td>
                    <asp:TextBox runat="server" ID="txtDomain" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 20%">Account:</td>
                <td>
                    <asp:TextBox runat="server" ID="txtAccount" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <telerik:RadButton runat="server" ID="btnSearch" Text="Cerca" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
        <asp:Table class="datatable" id="tblRicerca" runat="server"></asp:Table>
</asp:Content>
