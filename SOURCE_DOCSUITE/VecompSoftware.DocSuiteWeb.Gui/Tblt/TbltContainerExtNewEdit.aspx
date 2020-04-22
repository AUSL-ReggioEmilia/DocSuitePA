<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltContainerExtNewEdit.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContainerExtNewEdit" MasterPageFile="~/MasterPages/DocSuite2008.Master" EnableTheming="true" %>

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
        	
            function CloseWindow(action, jsonContainerExt) {
                var oWindow = GetRadWindow();
                var oReturn = new Object();
                oReturn.Action = action;
                oReturn.ContainerExt = jsonContainerExt;
                oWindow.close(oReturn);
            }
            
        </script>			
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="FolderAdd" runat="server" Visible="False">
        <table id="TblAdd" class="datatable">
            <tr>
                <th colspan="2">
                    Cartella
                </th>
            </tr>
            <tr>
                <td style="width: 155px; vertical-align: middle;" class="label">
                    <b>Nome Cartella:</b>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtFolderName" runat="server" Width="100%" MaxLength="50"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ControlToValidate="txtFolderName" Display="Dynamic" ErrorMessage="Campo Nome Cartella Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 155px; vertical-align: middle;" class="label">
                    <b>Documenti Richiesti:</b>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtDocNumber" runat="server" MaxLength="50" Width="10%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ControlToValidate="txtDocNumber" Display="Dynamic" ErrorMessage="Campo Document Richiesti Obbligatorio" ID="Requiredfieldvalidator4" runat="server" />
                    <asp:RegularExpressionValidator ControlToValidate="txtDocNumber" ErrorMessage="Il Campo deve essere numerico" ID="Regularexpressionvalidator2" runat="server" ValidationExpression="\d+" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    
    <asp:Panel ID="FolderRename" runat="server" Visible="False">
        <table id="TblRename" class="datatable">
            <tr>
                <th colspan="2">
                    Cartella
                </th>
            </tr>
            <tr>
                <td style="width: 175px;vertical-align: middle;" class="label">
                    Nome Cartella:
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOldFolderName" runat="server" Width="100%" MaxLength="50" Enabled="False"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 175px;vertical-align: middle;" class="label">
                    <b>Documenti Richiesti:</b>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOldDocNumber" runat="server" MaxLength="50" Width="100%" Enabled="False"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 175px;vertical-align: middle;" class="label">
                    Nuovo Nome Cartella:
                </td>
                <td>
                    <telerik:RadTextBox ID="txtNewFolderName" MaxLength="50" runat="server" Width="100%" />
                    <asp:RequiredFieldValidator ControlToValidate="txtNewFolderName" Display="Dynamic" ErrorMessage="Campo Nome Cartella Obbligatorio" ID="Requiredfieldvalidator2" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 175px;vertical-align: top;" class="label">
                    Nuovi Documenti Richiesti:
                </td>
                <td>
                    <telerik:RadTextBox ID="txtNewDocNumber" MaxLength="50" runat="server" Width="10%" />
                    <asp:RequiredFieldValidator ControlToValidate="txtNewDocNumber" Display="Dynamic" ErrorMessage="Campo Document Richiesti Obbligatorio" ID="Requiredfieldvalidator3" runat="server" />
                    <asp:RegularExpressionValidator ControlToValidate="txtNewDocNumber" ErrorMessage="Il Campo deve essere numerico" ID="Regularexpressionvalidator1" runat="server" ValidationExpression="\d+" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma" />
    <asp:ValidationSummary DisplayMode="List" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
</asp:Content>