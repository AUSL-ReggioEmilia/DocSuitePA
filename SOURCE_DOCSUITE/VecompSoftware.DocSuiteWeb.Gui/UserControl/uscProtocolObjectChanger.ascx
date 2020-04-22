<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProtocolObjectChanger.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtocolObjectChanger" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">

<script type="text/javascript" language="javascript">
    //richiamata quando la finestra viene chiusa
    function <%= Me.ID %>_OnClose(sender, args)
    {
        var values = args.get_argument().split("|");
        document.getElementById("<%=txtObject.ClientID %>").value = values[0];
        document.getElementById("<%=txtObjectChanged.ClientID %>").value = values[0];
        document.getElementById("<%=txtChangeReason.ClientID %>").value = values[1];
    }
</script>
</telerik:RadScriptBlock>


<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerOggetto" runat="server">
    <Windows>   
        <telerik:RadWindow ID="windowChangeOggetto" runat="server" Title="Modifica Oggetto" />
    </Windows>
</telerik:RadWindowManager>

<table>
    <tr>
        <td width="100%">
            <telerik:RadTextBox ID="txtObject" runat="server" Width="100%" Enabled="false"></telerik:RadTextBox>
            <asp:TextBox ID="txtObjectChanged" runat="server" Width="1px" Height="1px" CssClass="hiddenField"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvObject" runat="server" Display="Dynamic" ErrorMessage="Campo Oggetto Obbligatorio"
                ControlToValidate="txtObject"></asp:RequiredFieldValidator>
            <asp:TextBox ID="txtChangeReason" runat="server" Width="1px" Height="1px" CssClass="hiddenField"></asp:TextBox>
        </td>
        <td valign="top" nowrap style="width: 21px">
            <asp:ImageButton CausesValidation="False" ID="btnOpenObject" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" ToolTip="Modifica Oggetto" />
        </td>
    </tr>
</table>