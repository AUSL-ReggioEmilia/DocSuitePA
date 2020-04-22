<%@ Page Title="Attivazione Super Admin" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    CodeBehind="SuperAdmin.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SuperAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function doRedirect(address) {
                window.top.location.href = address;
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server" Visible="false">
    <table class="dataform" id="signIn" runat="server">
        <tr>
            <td class="label" style="width: 20%">
                &nbsp;
            </td>
            <td style="width: 80%">
                <asp:Button ID="activate" runat="server" Text="Attiva" OnClick="activateSuperUser" />
            </td>
        </tr>
    </table>
    <asp:Literal ID="hashed" runat="server"></asp:Literal>
    <table class="dataform" id="signOut" runat="server">
        <tr>
            <td class="label" style="width: 20%">
                &nbsp;
            </td>
            <td style="width: 80%">
                <strong>Vuoi disabilitare le autorizzazioni da Super Admin?</strong>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                &nbsp;
            </td>
            <td style="width: 80%">
                <asp:Button ID="deactivate" runat="server" Text="Disattiva" OnClick="deactivateSuperUser" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server" Visible="false">
</asp:Content>
