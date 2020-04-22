<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscSelLocation.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSelLocation" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script type="text/javascript">
        //richiamata quando la finestra viene chiusa
        function <%= ID%>_OnClose(sender, args) {
            if (args._argument != null) {
                document.getElementById('<%= txtIdLocation.ClientID %>').value = args._argument.ID;
                document.getElementById('<%= txtLocation.ClientID %>').value = args._argument.Description;
            }
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerLocazioni" runat="server">
    <Windows>
        <telerik:RadWindow Height="480" ID="windowSelLocazioni" ReloadOnShow="false" runat="server" Title="Selezione Deposito Documentale" Width="640" />
    </Windows>
</telerik:RadWindowManager>

<table runat="server" class="datatable" id="tblLocation" width="100%">
    <tr>
        <td class="label" style="vertical-align:middle; width: 20%;">
            <asp:Label runat="server" ID="lblCaption" />    
        </td>
        <td style="vertical-align:middle; width: 60%;">
            <telerik:RadTextBox ID="txtLocation" runat="server" Width="100%" />
        </td>
        <td style="vertical-align:middle;text-align:left;padding-left:5px; white-space: nowrap;">
            <asp:ImageButton ID="btnSelLocation" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" />
            <asp:ImageButton CausesValidation="False" ID="btnDelLocation" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" runat="server" />
        </td>
        <td style="vertical-align:middle;text-align:left;padding-left:5px; white-space: nowrap;">
            <asp:TextBox ID="txtIdLocation" runat="server" CssClass="hiddenField" Width="5px"/>
        </td>
    </tr>
</table>

