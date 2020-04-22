<%@ Control AutoEventWireup="false" CodeBehind="uscServiceCategory.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscServiceCategory" Language="vb" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript" language="javascript">
        function <%= Me.ID %>_OnClose(sender, args) {
            if (args.get_argument() !== null) {
                document.getElementById("<%=txtServiceCategory.ClientID %>").value = args.get_argument();
            }
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerServiceCategory" runat="server">
    <Windows>   
        <telerik:RadWindow ID="windowSelServiceCategory" ReloadOnShow="false" runat="server" Title="Selezione Categoria Servizio" />
    </Windows>
</telerik:RadWindowManager>
 
<table>
    <tr>
        <td id="tdServiceCategoryText" runat="server" width="100%">
            <telerik:RadTextBox ID="txtServiceCategory" MaxLength="511" runat="server" Width="100%" />
            <asp:RequiredFieldValidator ControlToValidate="txtServiceCategory" Display="Dynamic" ErrorMessage="Campo Categoria Servizio Obbligatorio" ID="rfvServiceCategory" runat="server" />
        </td>
        <td style="width: 40px; white-space: nowrap; vertical-align: top;">
            <asp:ImageButton CausesValidation="False" ID="SelO" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" ToolTip="Selezione Categoria Servizio" />
        </td>
    </tr>
</table>


 
   