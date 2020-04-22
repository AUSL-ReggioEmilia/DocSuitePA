<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CommonSelTemplate.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelTemplate" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Selezione Template" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
<telerik:RadScriptBlock runat="server" ID="RadScriptBlock2">

        <script type="text/javascript" language="javascript">
            function CloseWindow(value)
            {
                var oWindow = GetRadWindow();
                oWindow.close(value);
            }
        </script>

    </telerik:RadScriptBlock>
    <table id="tblContent" class="datatable">
        <tr class="Chiaro" style="height: 100%">
            <td style="vertical-align: top">
                <telerik:RadTreeView ID="RadTreeTemplate" runat="server" Width="100%">
                </telerik:RadTreeView>
                <br />
            </td>
        </tr>
        <tr class="Spazio">
            <td>
            </td>
        </tr>
    </table>
</asp:Content>
