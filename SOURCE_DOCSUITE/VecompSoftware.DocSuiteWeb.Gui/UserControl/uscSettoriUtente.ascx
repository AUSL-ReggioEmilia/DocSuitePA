<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscSettoriUtente.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSettoriUtente" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script type="text/javascript" language="javascript">
    //apertura della finestra Upload Document
        function <%= Me.ID %>_OpenWindow(name, width, height, parameters) {
            var URL = "../UserControl/CommonSelSettoriUtenti.aspx";
            URL += "?" + parameters;

            var manager = $find("<%=RadWindowManagerUsers.ClientID %>");
            var wnd = manager.open(URL, name);
            wnd.setSize(width, height);
            wnd.center();

            return false;
        }

        //richiamata quando la finestra viene chiusa
        function <%= Me.ID %>_CloseFunction(sender, args) {
            if (args.get_argument() !== null) {
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                ajaxManager.ajaxRequest("<%= Me.ClientID %>" + "|" + args.get_argument());
            }
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerUsers" runat="server">
    <Windows>
        <telerik:RadWindow Height="480" ID="windowSelUsers" ReloadOnShow="false" runat="server" Title="Selezione Utenti" Width="640" />
    </Windows>
</telerik:RadWindowManager>


<table id="tblDistribuzione" class="datatable">
    <tr>
        <th colspan="2" runat="server" id="tblHeader"><asp:Label runat="server" ID="lblCaption" Text="Distribuzione Pratica"></asp:Label></th>
    </tr>
    <tr>
        <td width="100%">
            <telerik:RadTreeView runat="server" ID="RadTreeUsers">
                <Nodes>
                    <telerik:RadTreeNode runat="server" Text="Distribuzione" Expanded="true" Value="Root"></telerik:RadTreeNode>
                </Nodes>
            </telerik:RadTreeView>
        </td>
        <td nowrap runat="server" id="tblCellButtons">
            <asp:ImageButton CausesValidation="False" ID="btnSelUsers" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick.png" runat="server" ToolTip="Assegnazione Utenti" />
        </td>
    </tr>
</table>
