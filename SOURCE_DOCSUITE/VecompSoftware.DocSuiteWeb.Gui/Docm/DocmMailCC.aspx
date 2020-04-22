<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocmMailCC.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmMailCC" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Invio Mail" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">

        <script type="text/javascript" language="javascript">

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }


            function CloseWindow(value) {
                var oWindow = GetRadWindow();
                oWindow.close(value);
            }

        </script>

    </telerik:RadScriptBlock>
    <telerik:RadTreeView ID="RadTreeSettori" runat="server" Width="100%" Height="100%" EnableViewState="true" CheckBoxes="true">
        <Nodes>
            <telerik:RadTreeNode runat="server" Text="Settori Copia Conoscenza" Font-Bold="true" Expanded="true" Checkable="true" EnableViewState="false" Value="Root" />
        </Nodes>
    </telerik:RadTreeView>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <table class="datatable">
        <tr>
            <td>
                <asp:Button ID="btnConferma" runat="server" Text="Invio" />
                <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" />
                <asp:TextBox runat="server" ID="txtIdRole" CssClass="hiddenField" />
            </td>
        </tr>
    </table>
</asp:Content>

